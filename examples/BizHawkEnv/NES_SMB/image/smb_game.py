import os
from typing import Any, Optional

import env_client
import gymnasium as gym
from bizhawk_api import Utils, client, emu, gui, memory


class GameController(env_client.IGameController):
    def __init__(self):
        self.action_space = gym.spaces.Discrete(6)
        self.observation_space = gym.spaces.Tuple(
            [
                gym.spaces.Discrete(256),  # screen x
                gym.spaces.Discrete(492),  # y
                gym.spaces.Discrete(256, start=-127),  # speed x
                gym.spaces.Discrete(256, start=-127),  # speed y
                gym.spaces.Discrete(256),  # state
                gym.spaces.Discrete(256),  # state2
                gym.spaces.Discrete(10000),  # time
            ]
        )
        self.rom = os.environ["SMB_PATH"]
        self.rom_hash = "EA343F4E445A9050D4B4FBAC2C77D0693B1D0922"

    def setup(self, info: dict):
        self.debug = info["debug"]

    def reset(self, seed: Optional[int] = None, options: Optional[dict] = None) -> tuple[Any, dict]:
        client.reboot_core()

        # --- title skip
        for _ in range(40):
            emu.frameadvance()
        Utils.set_key("P1 Start", frameadvance=True)
        for _ in range(180):
            emu.frameadvance()

        self._max_stage_mario_x = 0
        self._stepout = 0

        self._max_stage_mario_x = 0

        if self.debug:
            self._draw_display()

        return self._get_state(), self._get_info()

    def _get_state(self):
        y = memory.readbyte(0xB5) * 0x100 + memory.readbyte(0xCE)
        if y > 256 + 235:
            y = 256 + 235

        time_ = memory.readbyte(0x7F7) * 1000 + memory.readbyte(0x7F8) * 100 + memory.readbyte(0x7F9) * 10 + memory.readbyte(0x7FA)

        return (
            memory.readbyte(0x3AD),  # screen x
            y,
            memory.read_s8(0x57),  # speed x
            memory.read_s8(0x9F),  # speed y
            memory.readbyte(0x1D),  # player state
            memory.readbyte(0x0E),  # status1
            time_,
        )

    def _get_info(self) -> dict:
        return {"max_x": self._max_stage_mario_x}

    def step(self, action: Any) -> tuple[Any, float, bool, bool, dict]:
        if action == 0:
            Utils.set_keys(["P1 B"], frameadvance=True)
        elif action == 1:
            Utils.set_keys(["P1 A", "P1 B"], frameadvance=True)
        elif action == 2:
            Utils.set_keys(["P1 Left", "P1 B"], frameadvance=True)
        elif action == 3:
            Utils.set_keys(["P1 Right", "P1 B"], frameadvance=True)
        elif action == 4:
            Utils.set_keys(["P1 Left", "P1 A", "P1 B"], frameadvance=True)
        elif action == 5:
            Utils.set_keys(["P1 Right", "P1 A", "P1 B"], frameadvance=True)

        if self.debug:
            self._draw_display()

        stage_mario_x = memory.readbyte(0x6D) * 0x100 + memory.readbyte(0x86)
        if stage_mario_x > self._max_stage_mario_x:
            self._max_stage_mario_x = stage_mario_x

        # --- goal
        if memory.readbyte(0xE) == 0x4:
            return self._get_state(), 100, True, False, self._get_info()

        # --- dead
        if memory.readbyte(0xE) == 11:
            return self._get_state(), -1, True, False, self._get_info()
        mario_y = (memory.readbyte(0xB5) - 1) * 0x100 + memory.readbyte(0xCE)
        if mario_y > 210:
            return self._get_state(), -1, True, False, self._get_info()
        if memory.readbyte(0xE) == 0x0:
            return self._get_state(), -1, True, False, self._get_info()

        # --- reward
        scroll_amount = memory.readbyte(0x775)
        reward = scroll_amount
        if reward == 0:
            reward = -0.01

        # --- time
        time_ = memory.readbyte(0x7F8) * 100 + memory.readbyte(0x7F9) * 10 + memory.readbyte(0x7FA)
        if time_ == 0:
            return self._get_state(), reward, True, False, self._get_info()

        return self._get_state(), reward, False, False, self._get_info()

    def backup(self):
        return [self._max_stage_mario_x]

    def restore(self, dat):
        self._max_stage_mario_x = dat[0]

    def _draw_display(self):
        mario_x = memory.readbyte(0x6D) * 0x100 + memory.readbyte(0x86)
        mario_y = (memory.readbyte(0xB5) - 1) * 0x100 + memory.readbyte(0xCE)
        speed_x = memory.read_s8(0x57)
        speed_y = memory.read_s8(0x9F)
        gui.text(5, 15, f"mario({mario_x},{mario_y}) a({speed_x},{speed_y})")


if __name__ == "__main__":
    env_client.run(GameController())
