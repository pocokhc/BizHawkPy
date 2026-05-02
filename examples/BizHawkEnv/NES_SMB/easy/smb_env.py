import os

import gymnasium.envs.registration

from py.env_server import BizHawkEnv

gymnasium.envs.registration.register(
    id="SMB-easy-v0",
    entry_point=__name__ + ":GameEnv",
)


class GameEnv(BizHawkEnv):
    def __init__(self, **kwargs):
        assert "BIZHAWK_DIR" in os.environ
        assert "SMB_PATH" in os.environ
        super().__init__(
            bizhawk_dir=os.environ["BIZHAWK_DIR"],
            python_path=os.path.join(os.path.dirname(__file__), "smb_game.py"),
            display_name="BizHawk-SMB-easy",
            **kwargs,
        )


def main():
    from examples.BizHawkEnv.utils import print_logger

    print_logger("debug")

    env = GameEnv(debug=True)

    state, info = env.reset()
    print(f"state : {state}, {len(state)}")
    print(f"info  : {info}")

    for step in range(10):
        action = env.action_space.sample()
        state, reward, terminated, truncated, info = env.step(action)
        done = terminated or truncated
        print(f"--- step {step} ---")
        print(f"action : {action}")
        print(f"state  : {state}")
        print(f"reward : {reward}")
        print(f"done   : {done}")
    env.close()


if __name__ == "__main__":
    main()
