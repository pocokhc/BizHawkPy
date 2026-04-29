import json
import logging
import os
import shutil
import subprocess
import tempfile
import threading
import time
import traceback
import weakref
from multiprocessing.connection import Connection, Listener
from pathlib import Path
from typing import Any, Literal, Optional, cast

import gymnasium as gym
import gymnasium.envs.registration
import numpy as np

ObservationTypes = Literal["VALUE", "IMAGE", "BOTH", "RAM"]

logger = logging.getLogger(__name__)


class BizHawkError(Exception):
    pass


gymnasium.envs.registration.register(
    id="BizHawk-v0",
    entry_point=__name__ + ":BizHawkEnv",
    nondeterministic=True,
)


class BizHawkEnv(gym.Env):
    metadata = {"render_modes": ["rgb_array"], "render_fps": 60}

    def __init__(
        self,
        bizhawk_dir: Path | str,
        python_path: Path | str,
        *,
        observation_type: ObservationTypes = "VALUE",
        frameskip: int = 0,
        silent: bool = True,
        reset_speed: int = 800,
        step_speed: int = 800,
        pause_before_reset: bool = False,
        pause_after_reset: bool = False,
        user_data: str = "",
        debug: bool = False,
        #
        render_mode: str | None = None,
        display_name: str = "",
        **kwargs,
    ):
        self.render_mode = render_mode
        self.display_name = display_name
        self.metadata["render_fps"] /= kwargs.get("frameskip", 0) + 1

        self.bizhawk_dir = Path(bizhawk_dir)
        self.observation_type = cast(ObservationTypes, observation_type.upper())
        self.debug = debug
        self.invalid_actions = []
        self.backup_count = 0

        self._client_config = {
            "observation_type": self.observation_type,
            "frameskip": frameskip,
            "silent": silent,
            "reset_speed": reset_speed,
            "step_speed": step_speed,
            "pause_before_reset": pause_before_reset,
            "pause_after_reset": pause_after_reset,
            "user_data": user_data,
            "debug": debug,
        }

        self.step_img = None
        self.screen = None

        self.emu = BizHawkEmu(bizhawk_dir, python_path)
        self.rpc = RPCServer()
        self.is_reset = False

        self.boot()
        self._finalizer = weakref.finalize(self, self._close)

    def close(self) -> None:
        if self._finalizer.alive:
            self._finalizer()

    def _close(self):
        if self.debug:
            print("GymBizHawk closing. (When the program is terminated, the emu closes, so I stop it for debugging purposes.)")
            input("continue> ")
        try:
            self.rpc.send({"cmd": "close"})
            self.rpc.recv_wait(timeout=60)
            self.rpc.close()
        except Exception:
            print(traceback.format_exc())
        try:
            self.emu.close()
        except Exception:
            print(traceback.format_exc())

    def boot(self):
        self.rpc.start_server()

        assert self.rpc.host is not None
        assert self.rpc.port is not None
        self.emu.boot(self.rpc.host, self.rpc.port)

        self._wait_client()

    def _wait_client(self):
        if self.rpc.conn is not None:
            return

        self.rpc.wait_client()

        # --- 1st send
        self.rpc.send(self._client_config)

        # --- 1st recv
        res = self.rpc.recv_wait(timeout=60)
        self.observation_space = res["observation_space"]
        self.action_space = res["action_space"]
        self.platform = res["platform"]
        self.image_shape = res["image_shape"]
        logger.info(f"observation: {self.observation_space}")
        logger.info(f"action     : {self.action_space}")
        logger.info(f"platform   : {self.platform}")
        logger.info(f"image_shape: {self.image_shape}")

    # -----------------------------
    # gym
    # -----------------------------
    def reset(self, seed: Optional[int] = None, options: Optional[dict] = None):
        super().reset(seed=seed, options=options)

        # clientと未接続なら接続する
        self._wait_client()

        self.rpc.send({"cmd": "reset", "seed": seed, "options": options})
        res = self.rpc.recv_wait()
        state = res["state"]
        info = res["info"]
        self.invalid_actions = res["invalid_actions"]

        self.step_img = None
        if self.observation_type == "IMAGE":
            self.step_img = state
        elif self.observation_type == "BOTH":
            self.step_img = state[0]

        self.is_reset = True
        return state, info

    def step(self, action: list):
        if not self.is_reset:
            raise RuntimeError("reset() must be called before step()")
        try:
            self.rpc.send({"cmd": "step", "action": action})
            res = self.rpc.recv_wait()
            state = res["state"]
            reward = res["reward"]
            terminated = res["terminated"]
            truncated = res["truncated"]
            info = res["info"]
            self.invalid_actions = res["invalid_actions"]

            self.step_img = None
            if self.observation_type == "IMAGE":
                self.step_img = state
            elif self.observation_type == "BOTH":
                self.step_img = state[0]

        except Exception:
            # 受信に失敗したら異常終了扱い
            self.rpc.close_conn()
            self.is_reset = False
            state = None
            reward = 0
            terminated = True
            truncated = True
            info = {}
            self.invalid_actions = []
            logger.error(traceback.format_exc())

        return state, reward, terminated, truncated, info

    def render(self):
        if self.render_mode != "rgb_array":
            print("You are calling render method without specifying any render mode.")
            return

        try:
            import pygame
        except ImportError as e:
            raise BizHawkError("pygame is not installed, run `pip install pygame`") from e
        if self.screen is None:
            pygame.init()
            self.screen = pygame.Surface((self.image_shape[1], self.image_shape[0]))

        if self.step_img is None:
            self.rpc.send({"cmd": "screenshot"})
            res = self.rpc.recv_wait(timeout=60)
            img = res["screenshot"]
        else:
            img = self.step_img

        # BGR → RGB 対応
        img = img[..., ::-1]

        img = img.swapaxes(0, 1)
        surface = pygame.surfarray.make_surface(img)
        self.screen.blit(surface, (0, 0))
        return np.array(pygame.surfarray.pixels3d(self.screen)).swapaxes(0, 1)

    # ------------------------------------------
    # SRL
    # ------------------------------------------
    @property
    def render_image_shape(self) -> Optional[tuple[int, int, int]]:
        return self.image_shape

    def get_display_name(self) -> str:
        return self.display_name

    def get_invalid_actions(self):
        return self.invalid_actions

    def backup(self):
        b = self.backup_count
        self.rpc.send({"cmd": "save", "name": f"t{b}.bat"})
        self.rpc.recv_wait()
        self.backup_count += 1
        return b

    def restore(self, dat) -> None:
        self.rpc.send({"cmd": "load", "name": f"t{dat}.bat"})
        self.rpc.recv_wait()


class BizHawkEmu:
    def __init__(self, bizhawk_dir: Path | str, python_path: Path | str):
        self.bizhawk_dir = Path(bizhawk_dir)
        self.python_path = python_path

        self.emu = None
        self.tmp_dir = tempfile.mkdtemp(prefix="biz_")

        self._finalizer = weakref.finalize(self, self._close)

    def close(self) -> None:
        if self._finalizer.alive:
            self._finalizer()

    def _close(self) -> None:
        if self.emu:
            logger.info("bizhawk closing.")
            try:
                self.emu.terminate()
                self.emu.wait(timeout=3)
            except subprocess.TimeoutExpired:
                self.emu.kill()
            finally:
                self.emu = None

        if os.path.isdir(self.tmp_dir):
            shutil.rmtree(self.tmp_dir, ignore_errors=True)

    def boot(self, host: str, port: int):
        bizhawk_exe = self.bizhawk_dir / "EmuHawk.exe"
        if not bizhawk_exe.is_file():
            raise BizHawkError(
                f"EmuHawk.exe not found.\n"  #
                f"- expected path: {bizhawk_exe}\n"
                f"- bizhawk_dir: {self.bizhawk_dir}\n"
                "Fix: set correct BizHawk directory or install BizHawk."
            )

        # --- config check
        biz_cfg = self.bizhawk_dir / "config.ini"
        if not biz_cfg.is_file():
            raise BizHawkError(
                f"config.ini not found.\n"  #
                f"- expected path: {biz_cfg}\n"
                f"- bizhawk_dir: {self.bizhawk_dir}\n"
                "Fix: launch and close BizHawk once to generate config.ini, or verify the directory is correct."
            )
            # 一旦起動してconfigを生成させる
            if False:
                cmd = os.path.join(self.bizhawk_dir, "EmuHawk.exe")
                cmd += " dummy"
                logger.info(f"bizhawk run(create config): {cmd}")
                proc = subprocess.Popen(cmd)
                try:
                    for _ in range(100):  # 最大10秒待つ
                        if os.path.isfile(biz_cfg):
                            break
                        time.sleep(0.1)
                    else:
                        logger.warning("config.ini was not created within timeout")
                finally:
                    try:
                        proc.terminate()
                        proc.wait(timeout=3)
                    except subprocess.TimeoutExpired:
                        proc.kill()

        # --- create config
        assert biz_cfg.is_file()
        tmp_cfg = os.path.join(self.tmp_dir, "config.ini")
        shutil.copy(biz_cfg, tmp_cfg)
        with open(tmp_cfg, "r", encoding="utf-8") as f:
            cfg = json.load(f)

        items = [
            {
                "Item1": "BizHawkGym",
                "Item2": os.path.abspath(os.path.join(os.path.dirname(__file__), self.python_path)),
                "Item3": True,
                "Item4": True,
                "Item5": f"{host} {port}",
            }
        ]
        cfg.setdefault("CustomToolSettings", {}).setdefault("BizHawkPy.MainConsole", {})
        cfg["CustomToolSettings"]["BizHawkPy.MainConsole"]["ScriptSessionItemsJson"] = json.dumps(items, separators=(",", ":"))
        cfg["CustomToolSettings"]["BizHawkPy.MainConsole"]["StopOnException"] = True

        with open(tmp_cfg, "w", encoding="utf-8") as f:
            json.dump(cfg, f, ensure_ascii=False, indent=2)

        logger.info(
            "BizHawk compatibility note:\n"  #
            " - In my environment, '--config' option did not function on v2.9.1\n"
            " - Verified working on v2.10.0\n"
            " - If startup fails, upgrading BizHawk may resolve the issue"
        )

        # --- run bizhawk
        cmd = str(bizhawk_exe)
        cmd += " --config {}".format(tmp_cfg)
        cmd += " --open-ext-tool-dll=BizHawkPy"
        logger.info(f"bizhawk run: {cmd}")
        self.emu = subprocess.Popen(cmd, cwd=self.bizhawk_dir)
        return


class RPCServer:
    def __init__(self):
        self.port: Any | None = None
        self.host: str | None = None
        self.thread = None
        self.server_authkey = b"secret"

        self.listener: Listener | None = None
        self.conn: Connection | None = None
        self._close_lock = threading.Lock()
        self._closed = False
        self._finalizer = weakref.finalize(
            self,
            type(self)._cleanup_all,
            self,
        )

    def _cleanup_all(self) -> None:
        if self.conn is not None:
            try:
                self.conn.close()
            except Exception:
                pass

        if self.listener is not None:
            try:
                self.listener.close()
            except Exception:
                pass

    def close_conn(self) -> None:
        """こちらからclientを閉じる"""
        if self.conn is not None:
            try:
                self.conn.close()
            except Exception:
                logger.exception(f"[{self.port}] Connection close failed")
            finally:
                self.conn = None

    def _close_listener(self) -> None:
        if self.listener is not None:
            try:
                self.listener.close()
            except Exception:
                logger.exception(f"[{self.port}] Listener close failed")
            finally:
                self.listener = None

    def close(self):
        with self._close_lock:
            if self._closed:
                return
            self._closed = True

            logger.info(f"[{self.port}] Server closing...")

            success = False
            try:
                self.close_conn()
                self._close_listener()
                success = True
            finally:
                self.host = None
                self.port = None
                if success:
                    self._finalizer.detach()

    def start_server(self):
        if self._closed:
            raise RuntimeError(f"[{self.port}] Server already closed")
        if self.listener is not None:
            raise RuntimeError(f"[{self.port}] Server already started")

        self.listener = Listener(("127.0.0.1", 0), authkey=self.server_authkey)
        self.host, self.port = self.listener.address
        logger.info(f"Server started({self.host}, {self.port})")

    def wait_client(self):
        if self.listener is None:
            return
        self.close_conn()
        self.conn = self.listener.accept()  # block
        logger.info(f"[{self.port}] Client connected")

    def send(self, obj: dict):
        if self.conn is None:
            raise RuntimeError()
        logger.debug(obj)
        self.conn.send(obj)

    def recv_wait(self, *, timeout: float = 0) -> Any:
        if self.conn is None:
            raise RuntimeError()
        if timeout > 0:
            if not self.conn.poll(timeout):
                raise TimeoutError(f"[{self.port}] recv timeout")
        recv = self.conn.recv()
        logger.debug(recv)
        return recv


if __name__ == "__main__":
    pass
