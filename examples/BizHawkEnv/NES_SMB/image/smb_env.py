import os

import gymnasium.envs.registration

from py.env_server import BizHawkEnv

gymnasium.envs.registration.register(
    id="SMB-image-v0",
    entry_point=__name__ + ":GameEnvImage",
)


class GameEnvImage(BizHawkEnv):
    def __init__(self, **kwargs):
        assert "BIZHAWK_DIR" in os.environ
        assert "SMB_PATH" in os.environ
        super().__init__(
            bizhawk_dir=os.environ["BIZHAWK_DIR"],
            python_path=os.path.join(os.path.dirname(__file__), "smb_game.py"),
            observation_type="IMAGE",
            display_name="BizHawk-SMB-image",
            **kwargs,
        )


gymnasium.envs.registration.register(
    id="SMB-both-v0",
    entry_point=__name__ + ":GameEnvBoth",
)


class GameEnvBoth(BizHawkEnv):
    def __init__(self, **kwargs):
        assert "BIZHAWK_DIR" in os.environ
        assert "SMB_PATH" in os.environ
        super().__init__(
            bizhawk_dir=os.environ["BIZHAWK_DIR"],
            python_path=os.path.join(os.path.dirname(__file__), "smb_game.py"),
            observation_type="BOTH",
            display_name="BizHawk-SMB-both",
            **kwargs,
        )


gymnasium.envs.registration.register(
    id="SMB-ram-v0",
    entry_point=__name__ + ":GameEnvRam",
)


class GameEnvRam(BizHawkEnv):
    def __init__(self, **kwargs):
        assert "BIZHAWK_DIR" in os.environ
        assert "SMB_PATH" in os.environ
        super().__init__(
            bizhawk_dir=os.environ["BIZHAWK_DIR"],
            python_path=os.path.join(os.path.dirname(__file__), "smb_game.py"),
            observation_type="RAM",
            display_name="BizHawk-SMB-ram",
            **kwargs,
        )
