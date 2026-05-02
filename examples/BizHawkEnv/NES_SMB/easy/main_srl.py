import os

import mlflow
import smb_env  # noqa F401  # load env
import srl
from srl.algorithms import godq_v1, rainbow
from srl.utils import common

common.logger_print()
base_dir = os.path.dirname(__file__)
mlflow.set_tracking_uri(os.environ.get("MLFLOW_TRACKING_URI", "mlruns"))


def _make_rainbow():
    rl_config = rainbow.Config(
        multisteps=1,
        lr=0.0002,
        epsilon=0.1,
        discount=0.999,
        target_model_update_interval=5000,
        window_length=8,
    )
    rl_config.memory.warmup_size = 5000
    rl_config.memory.capacity = 100_000
    rl_config.memory.compress = False
    rl_config.memory.set_proportional()
    rl_config.hidden_block.set_dueling_network((512,))
    return rl_config


def _make_godp():
    rl_config = godq_v1.Config(
        lr=0.0002,
        window_length=8,
    )
    rl_config.memory.warmup_size = 5000
    rl_config.memory.capacity = 100_000
    rl_config.memory.compress = False
    rl_config.memory.set_proportional()
    rl_config.set_model(512)
    return rl_config


def _make_runner():
    env_config = srl.EnvConfig(
        "SMB-easy-v0",
        kwargs=dict(
            frameskip=5,
            # denig=True,
        ),
    )

    # --- select algorithm
    rl_config = _make_rainbow()
    # rl_config = _make_godp()

    return srl.Runner(env_config, rl_config)


def train():
    runner = _make_runner()
    runner.model_summary()

    runner.set_progress(env_info=True, enable_eval=True)
    runner.set_mlflow()

    # runner.train(max_train_count=10)  # debug
    runner.train(max_train_count=100_000)
    # runner.train_mp(actor_num=1, max_train_count=500_000)


def eval():
    runner = _make_runner()
    runner.load_parameter_from_mlflow()
    runner.model_summary()

    # --- eval
    rewards = runner.evaluate(max_episodes=1)
    print(rewards)

    # --- view
    runner.animation_save_gif(os.path.join(base_dir, "_smb.gif"))
    runner.replay_window()


if __name__ == "__main__":
    train()
    eval()
