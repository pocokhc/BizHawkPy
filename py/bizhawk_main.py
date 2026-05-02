import base64
import json
import runpy
import sys
import traceback
import types
from contextlib import contextmanager
from typing import Any

sys.stdout.reconfigure(encoding="utf-8")  # type: ignore[attr-defined]
sys.stderr.reconfigure(encoding="utf-8")  # type: ignore[attr-defined]


def console_print(s):
    print(str(s), file=sys.stderr)


# ================================
# bizhawk_api
# ================================
import bizhawk_api  # noqa: E402

api: Any = types.ModuleType("bizhawk_api")
# api.bit = bizhawk_api.bit
# api.bizstring = bizhawk_api.bizstring
api.client = bizhawk_api.client
api.comm = bizhawk_api.comm
api.console = bizhawk_api.console
api.emu = bizhawk_api.emu
# api.event = bizhawk_api.event
api.gameinfo = bizhawk_api.gameinfo
# api.genesis = bizhawk_api.genesis
api.gui = bizhawk_api.gui
# api.input = bizhawk_api.input
api.joypad = bizhawk_api.joypad
# api.LuaCanvas = bizhawk_api.LuaCanvas
# api.mainmemory = bizhawk_api.mainmemory
api.memory = bizhawk_api.memory
api.memorysavestate = bizhawk_api.memorysavestate
api.movie = bizhawk_api.movie
api.nds = bizhawk_api.nds
api.nes = bizhawk_api.nes
api.savestate = bizhawk_api.savestate
api.snes = bizhawk_api.snes
# api.SQL = bizhawk_api.SQL
# api.tasstudio = bizhawk_api.tasstudio
api.userdata = bizhawk_api.userdata
api.Utils = bizhawk_api.Utils  # original
sys.modules["bizhawk_api"] = api

# ================================
# GymEnv
# ================================
import env_client  # noqa: E402

client: Any = types.ModuleType("env_client")
client.run = env_client.run
client.IGameController = env_client.IGameController
sys.modules["env_client"] = client


# ================================
# main
# ================================
def _bizhawk_console_print(*args: object, end: str = "\n", **kwargs: object) -> None:
    text = " ".join(map(str, args))
    sys.stderr.write(text + end)
    # sys.stderr.flush()


def _dummy_input(*args: object, **kwargs: object) -> None:
    print("Standard input is reserved for communication with BizHawk and cannot be used here.", file=sys.stderr, flush=True)


@contextmanager
def _patched_argv(argv: list[str]):
    """sys.argvを一時的に差し替える"""
    old_argv = sys.argv
    sys.argv = argv
    try:
        yield
    finally:
        sys.argv = old_argv


def run_with_injected_module(path: str, args: list[str]):
    globals_dict = {
        "print": _bizhawk_console_print,
        "input": _dummy_input,
    }
    argv = [path, *args]
    with _patched_argv(argv):
        runpy.run_path(path, init_globals=globals_dict, run_name="__main__")


def main():
    if len(sys.argv) < 2:
        print("ERROR: No argument received", file=sys.stderr)
        sys.exit(1)
    path: str = sys.argv[1]
    if path == "TEST":  # 実行確認用
        return

    path = base64.b64decode(sys.argv[1]).decode("utf-8")
    if len(sys.argv) == 3:
        args = json.loads(base64.b64decode(sys.argv[2].strip()).decode("utf-8"))
        args = args.split(" ")
    else:
        args = []
    run_with_injected_module(path, args)


if __name__ == "__main__":
    try:
        main()
    except Exception:
        print(traceback.format_exc(), file=sys.stderr)
    finally:
        sys.stderr.flush()
