import gymnasium as gym
import smb_env  # noqa F401  # load env


def main():
    env = gym.make("SMB-easy-v0", debug=True)
    env.reset()

    done = False
    step = 0
    while not done:
        step += 1
        action = env.action_space.sample()
        observation, reward, terminated, truncated, info = env.step(action)
        done = terminated or truncated
        print(f"--- step {step} ---")
        print(f"obs({len(observation)}): {observation}")
        print(f"action     : {action}")
        print(f"reward     : {reward}")
        print(f"done       : {done}")
    env.close()


if __name__ == "__main__":
    from examples.BizHawkEnv.utils import print_logger

    print_logger()

    main()
