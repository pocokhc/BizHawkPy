import os
import sys
import tempfile
import traceback
import weakref
from abc import ABC, abstractmethod
from multiprocessing.connection import Client
from typing import Any, Literal, Optional, cast

from bizhawk_api import client, emu, gameinfo, memory, savestate

try:
    import cv2
    import gymnasium as gym
    import numpy as np
except ModuleNotFoundError:
    pass

ObservationTypes = Literal["VALUE", "IMAGE", "BOTH", "RAM"]


class IProcessor(ABC):
    def __init__(self) -> None:
        # required
        self.action_space = gym.spaces.Discrete(2)
        self.observation_space = gym.spaces.Discrete(2)
        self.rom: str
        self.rom_hash: str  # option

    # ------------------------------------------
    # BizHawkEnv function
    # ------------------------------------------
    def setup(self, info: dict):
        pass

    # ------------------------------------------
    # gym functions
    # ------------------------------------------
    @abstractmethod
    def reset(self, seed: Optional[int] = None, options: Optional[dict] = None) -> tuple[Any, dict]:
        raise NotImplementedError()

    @abstractmethod
    def step(self, action: Any) -> tuple[Any, float, bool, bool, dict]:
        raise NotImplementedError()

    # ------------------------------------------
    # SRL functions
    # ------------------------------------------
    def get_invalid_actions(self) -> list:
        return []

    def backup(self) -> Any:
        """QS/QL is performed automatically"""
        return None

    def restore(self, dat: Any):
        """QS/QL is performed automatically"""
        pass


class RPCClient:
    def __init__(self, host: str, port: int):
        self.host = host
        self.port = port
        self.authkey = b"secret"
        self.conn = None

        self._debug = False

        self._connect()
        self._finalizer = weakref.finalize(self, self._close)

    def close(self) -> None:
        if self._finalizer.alive:
            self._finalizer()

    def _close(self) -> None:
        try:
            if self.conn is not None:
                self.conn.close()
        except Exception:
            pass
        finally:
            self.conn = None

    def _connect(self) -> None:
        try:
            print(f"RPCClient.connect({self.host}, {self.port})")
            self.conn = Client((self.host, self.port), authkey=self.authkey)
        except (ConnectionRefusedError, OSError) as e:
            raise ConnectionError(f"Failed to connect RPC server: {e}") from e

    def set_debug(self, debug: bool):
        self._debug = debug

    def recv(self) -> dict[str, Any]:
        if self.conn is None:
            raise ConnectionError("RPC connection is not established")
        try:
            d = self.conn.recv()
            if self._debug:
                print(f"recv: {d}")
            return d
        except (EOFError, ConnectionResetError, BrokenPipeError) as e:
            self.close()
            raise ConnectionError("RPC connection lost") from e

        except (TypeError, ValueError) as e:
            raise ValueError(f"Invalid RPC payload: {e}") from e

        except Exception as e:
            self.close()
            raise RuntimeError(f"Unexpected RPC error: {e}") from e

    def send(self, data: dict[str, Any]):
        if self.conn is None:
            raise ConnectionError("RPC connection is not established")
        try:
            if self._debug:
                print(f"send: {data}")
            self.conn.send(data)
        except (EOFError, ConnectionResetError, BrokenPipeError) as e:
            self.close()
            raise ConnectionError("RPC connection lost") from e

        except (TypeError, ValueError) as e:
            raise ValueError(f"Invalid RPC payload: {e}") from e

        except Exception as e:
            self.close()
            raise RuntimeError(f"Unexpected RPC error: {e}") from e


def run(processor: object | IProcessor):
    processor = cast(IProcessor, processor)
    print(f"rom          : {processor.rom}")
    assert client.openrom(processor.rom)

    platform = emu.getsystemid()
    hash_ = gameinfo.getromhash()
    print(f"romname      : {gameinfo.getromname()}")
    print(f"platform     : {platform}")
    hash_str = hash_
    if hasattr(processor, "rom_hash"):
        hash_str += "(match)" if hash_ == processor.rom_hash else f"(mismatch: {processor.rom_hash})"
    print(f"hash         : {hash_str}")
    print(f"memory domain: {memory.getcurrentmemorydomain()}")

    client.pause()
    client.SetSoundOn(True)
    client.speedmode(100)
    emu_speed = 100

    # --- connect
    host = sys.argv[1]
    port = int(sys.argv[2])
    rpc = RPCClient(host, port)

    # --- 1st recv
    recv = rpc.recv()
    observation_type: ObservationTypes = recv["observation_type"]
    frameskip: int = recv["frameskip"]
    silent: bool = recv["silent"]
    reset_speed: int = recv["reset_speed"]
    step_speed: int = recv["step_speed"]
    pause_before_reset: bool = recv["pause_before_reset"]
    pause_after_reset: bool = recv["pause_after_reset"]
    debug: bool = recv["debug"]
    rpc.set_debug(debug)

    if silent:
        client.SetSoundOn(False)

    # --- processor
    if hasattr(processor, "setup"):
        processor.setup(recv)

    # --- create tmp dir
    _tmp = tempfile.TemporaryDirectory()
    ss_tmp_path = os.path.join(_tmp.name, "ss.png")
    qs_dir = _tmp.name
    qs_data = {}

    # 画像サイズを取得
    img = _get_screenshot(ss_tmp_path)
    image_shape = img.shape if img is not None else (0, 0, 0)

    # --- observation_type
    if observation_type == "VALUE":
        observation_space = processor.observation_space
    elif observation_type == "IMAGE":
        observation_space = gym.spaces.Box(0, 255, shape=image_shape, dtype=np.uint8)
    elif observation_type == "RAM":
        mem_size = memory.getcurrentmemorydomainsize()
        observation_space = gym.spaces.MultiDiscrete([255] * mem_size, dtype=np.uint8)
    elif observation_type == "BOTH":
        img_space = gym.spaces.Box(0, 255, shape=image_shape, dtype=np.uint8)
        obs_space = processor.observation_space
        observation_space = gym.spaces.Tuple([img_space, obs_space])

    # --- 1st send
    rpc.send(
        {
            "observation_space": observation_space,
            "action_space": processor.action_space,
            "platform": platform,
            "image_shape": image_shape,
        }
    )

    # --- mode
    if pause_before_reset:
        print("Paused. Run with frameadvance.")
        client.pause()
    else:
        client.unpause()

    # --- cmd loop
    while True:
        try:
            recv = rpc.recv()
            cmd = recv.get("cmd", "")

            if cmd == "reset":
                # 1. set speed
                if reset_speed != emu_speed:
                    client.speedmode(reset_speed)
                    emu_speed = reset_speed

                # 2. processor.reset
                state, info = processor.reset(recv["seed"], recv["options"])
                return_data = {
                    "state": _get_state(observation_type, state, ss_tmp_path, image_shape),
                    "info": info,
                }
                if hasattr(processor, "invalid_actions"):
                    return_data["invalid_actions"] = processor.get_invalid_actions()
                else:
                    return_data["invalid_actions"] = []

                if pause_after_reset:
                    print("Paused. Run with frameadvance.")
                    client.pause()

            elif cmd == "step":
                act = recv["action"]
                prev_frame = emu.framecount()
                if step_speed != emu_speed:
                    client.speedmode(step_speed)
                    emu_speed = step_speed

                reward = 0
                for _ in range(frameskip + 1):
                    state, r, terminated, truncated, info = processor.step(act)
                    reward += r
                    done = terminated or truncated
                    if done:
                        break

                # --- step内でframeが進んでない場合は進める
                if not done:
                    target_frame = prev_frame + frameskip + 1
                    while emu.framecount() < target_frame:
                        emu.frameadvance()

                # --- send obs
                return_data = {
                    "state": _get_state(observation_type, state, ss_tmp_path, image_shape),
                    "reward": reward,
                    "terminated": terminated,
                    "truncated": truncated,
                    "info": info,
                }
                if hasattr(processor, "invalid_actions"):
                    return_data["invalid_actions"] = processor.get_invalid_actions()
                else:
                    return_data["invalid_actions"] = []

            elif cmd == "screenshot":
                return_data = {"screenshot": _get_screenshot(ss_tmp_path, image_shape)}
            elif cmd == "save":
                name = recv["name"]
                if hasattr(processor, "backup"):
                    qs_data[name] = processor.backup()
                savestate.save(os.path.join(qs_dir, name))
                return_data = {}
            elif cmd == "load":
                name = recv["name"]
                savestate.load(os.path.join(qs_dir, name))
                if hasattr(processor, "restore"):
                    processor.restore(qs_data[name])
                return_data = {}
            elif cmd == "close":
                client.speedmode(100)
                client.pause()
                rpc.send({"close": ""})
                rpc.close()
                break
            else:
                return_data = {}

            # 必ず何か返す
            rpc.send(return_data)

        except Exception:
            rpc.send({"error": traceback.format_exc()})


def _get_screenshot(ss_tmp_path: str, resize_shape=None) -> "np.ndarray":
    client.screenshot(ss_tmp_path)
    img = cv2.imread(ss_tmp_path)
    if img is None:
        raise RuntimeError("Failed to read screenshot image")

    if resize_shape is not None:
        h = resize_shape[0]
        w = resize_shape[1]
        if img.shape[:2] != (h, w):
            img = cv2.resize(img, (w, h), interpolation=cv2.INTER_AREA)

    return img


def _get_state(observation_type: ObservationTypes, state, ss_tmp_path: str, image_shape):
    if observation_type == "VALUE":
        return state
    elif observation_type == "IMAGE":
        return _get_screenshot(ss_tmp_path, image_shape)
    elif observation_type == "RAM":
        return memory.read_bytes_as_array(0, memory.getcurrentmemorydomainsize())
    elif observation_type == "BOTH":
        return [_get_screenshot(ss_tmp_path, image_shape), state]


if __name__ == "__main__":
    pass
