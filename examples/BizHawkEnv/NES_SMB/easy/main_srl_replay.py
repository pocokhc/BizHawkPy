import smb_env  # noqa F401  # load env
import srl
from srl.utils import common

common.logger_print()


def main():
    env_config = srl.EnvConfig("SMB-easy-v0", kwargs=dict(frameskip=5))
    runner = srl.Runner(env_config)
    runner.replay_window()


if __name__ == "__main__":
    main()
