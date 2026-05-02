import math
import os
from typing import Any, Optional

import env_client
import gymnasium as gym
from bizhawk_api import Utils, client, emu, gui, memory

# define
W_BLOCKS = 16
H_BLOCKS = 13
BLOCK_SIZE = 16
HALF_BLOCK_SIZE = 8
ENEMY = 5


class GameController(env_client.IGameController):
    def __init__(self):
        self.look_w = 9
        self.look_h = 6

        self.action_space = gym.spaces.Discrete(2)
        self.observation_space = gym.spaces.Tuple(
            [
                gym.spaces.Discrete(123),  # stepout
                gym.spaces.MultiBinary(10),  # mario y
                gym.spaces.MultiBinary(9),  # mario y speed
                gym.spaces.MultiBinary(41),  # mario x speed
                gym.spaces.MultiBinary(self.look_h * self.look_w),  # block
                gym.spaces.MultiBinary(self.look_h * self.look_w),  # enemy
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

        self._read_memory()
        if self.debug:
            self._draw_display()

        return self._get_state(), self._get_info()

    def _read_memory(self):
        self.mario_x = math.floor((memory.readbyte(0x4AC) + memory.readbyte(0x4AE)) / 2)
        self.mario_y = memory.readbyte(0xCE)
        self.stage_mario_x = memory.readbyte(0x6D) * 0x100 + memory.readbyte(0x86)
        self.stage_mario_y = (memory.readbyte(0xB5) - 1) * 0x100 + memory.readbyte(0xCE)
        self.speed_x = memory.read_s8(0x57)
        self.speed_y = memory.read_s8(0x9F)

        self.mario_tile_x = math.floor(self.mario_x / BLOCK_SIZE)
        self.mario_tile_y = math.floor(self.mario_y / BLOCK_SIZE) - 1

        # --- enemy
        self.enemy_view = []
        self.enemy_x = []
        self.enemy_y = []
        self.enemy_tile_x = []
        self.enemy_tile_y = []
        for i in range(ENEMY):
            self.enemy_view.append(memory.readbyte(0xF + i))
            x = (memory.readbyte(0x4B0 + i * 4) + memory.readbyte(0x4B2 + i * 4)) // 2
            y = memory.readbyte(0xCF + i)
            self.enemy_x.append(x)
            self.enemy_y.append(y)
            self.enemy_tile_x.append(x // BLOCK_SIZE)
            self.enemy_tile_y.append(y // BLOCK_SIZE - 1)

        # --- tiles
        self.tiles = []
        for y in range(H_BLOCKS):
            for x in range(W_BLOCKS):
                self.tiles.append(self._get_tile(x, y))

        # --- look range
        self.look_x = self.mario_tile_x - 1

    def _get_tile(self, x: int, y: int):
        base_x = math.floor((-8 + self.stage_mario_x - memory.readbyte(0x3AD)) / BLOCK_SIZE)
        dx = (base_x + x + 1) % 0x20
        page = math.floor(dx / 0x10)
        addr = 0x500 + page * 13 * 0x10 + y * 0x10 + (dx % 0x10)

        val = memory.readbyte(addr)
        if val == 194:  # coin
            return 0
        if val == 0:  # empty
            return 0
        return 1

    def _get_state(self):

        # --- mario y
        mario_y = []
        for y in range(1, 11):
            mario_y.append(1 if y == self.mario_tile_y else 0)

        # --- mario y speed
        mario_y_speed = []
        for y in range(-4, 5):
            mario_y_speed.append(1 if y == self.speed_y else 0)

        # --- mario x speed
        mario_x_speed = []
        mario_x_speed.append(1 if (self.speed_x <= 0) else 0)
        for x in range(1, 41):
            mario_x_speed.append(1 if x == self.speed_x else 0)

        # --- block
        block = []
        for y in range(H_BLOCKS - 1 - self.look_h, H_BLOCKS - 1):
            for x in range(self.mario_tile_x, self.mario_tile_x + self.look_w):
                b = self.tiles[y * W_BLOCKS + x]
                block.append(1 if b != 0 else 0)

        # --- enemy
        enemy = []
        for y in range(H_BLOCKS - 1 - self.look_h, H_BLOCKS - 1):
            for x in range(self.mario_tile_x, self.mario_tile_x + self.look_w):
                n = 0
                for i in range(ENEMY):
                    if self.enemy_view[i] == 1:
                        if (self.enemy_tile_x[i] == x) and (self.enemy_tile_y[i] == y):
                            n = 1
                enemy.append(n)

        return (
            # self.mario_x
            # self.mario_y
            # self.speed_x
            # self.speed_y
            self._stepout,
            mario_y,
            mario_y_speed,
            mario_x_speed,
            block,
            enemy,
        )

    def _get_info(self) -> dict:
        return {}

    def step(self, action: Any) -> tuple[Any, float, bool, bool, dict]:
        Utils.set_keys(
            [
                "P1 Right",
                "" if action == 0 else "P1 A",
                "P1 B",
            ],
            frameadvance=True,
        )

        self._read_memory()
        if self.debug:
            self._draw_display()

        # --- goal
        if memory.readbyte(0xE) == 0x4:
            return self._get_state(), 100, True, False, self._get_info()

        # --- dead
        if memory.readbyte(0xE) == 11:
            return self._get_state(), -1, True, False, self._get_info()
        if self.mario_y > 210:
            return self._get_state(), -1, True, False, self._get_info()
        if memory.readbyte(0xE) == 0x0:
            return self._get_state(), -1, True, False, self._get_info()

        reward = -0.01
        if self.stage_mario_x > self._max_stage_mario_x:
            # 最大値を更新
            reward = (self.stage_mario_x - self._max_stage_mario_x) / 10
            self._max_stage_mario_x = self.stage_mario_x
            self._stepout = 0
        else:
            # 一定時間進まなかったら終わり
            self._stepout += 1
            if self._stepout > 60 * 2:
                return self._get_state(), -1, True, False, self._get_info()

        return self._get_state(), reward, False, False, self._get_info()

    def backup(self):
        d = [
            self._max_stage_mario_x,
            self._stepout,
        ]
        return d

    def restore(self, dat):
        self._max_stage_mario_x = dat[2]
        self._stepout = dat[1]
        self._read_memory()

    def _draw_display(self):
        box_size = 5
        dx = 5
        dy = 5

        # --- gb
        gui.drawBox(
            dx,
            dy,
            dx + box_size * W_BLOCKS,
            dy + box_size * H_BLOCKS,
            0xFF000000,
            0x80808080,
        )

        # --- block
        for y in range(H_BLOCKS):
            for x in range(W_BLOCKS):
                n = self.tiles[y * W_BLOCKS + x]
                if n != 0:
                    self._draw_block(x, y, 0xFF000000, dx, dy, box_size)
        for i in range(ENEMY):
            if self.enemy_view[i] == 1:
                self._draw_block(self.enemy_tile_x[i], self.enemy_tile_y[i], 0xFFFF0000, dx, dy, box_size)
        self._draw_block(self.mario_tile_x, self.mario_tile_y, 0xFF00FF00, dx, dy, box_size)

        # --- look range
        x1 = self.mario_tile_x + 1
        x2 = x1 + self.look_w
        y2 = H_BLOCKS
        y1 = y2 - self.look_h
        gui.drawBox(x1 * box_size, y1 * box_size, x2 * box_size, y2 * box_size, 0xFFFF00FF, 0x00000000)

        # --- other info
        ddrawy = 15
        drawy = dy + box_size * H_BLOCKS + ddrawy + 65
        gui.text(dx, drawy, f"screen({self.stage_mario_x},{self.stage_mario_y})")
        drawy = drawy + ddrawy
        s = f"mario({self.mario_x},{self.mario_y})"
        s += f"({self.mario_tile_x},{self.mario_tile_y})"
        s += f" a({self.speed_x},{self.speed_y})"
        gui.text(dx, drawy, s)
        drawy = drawy + ddrawy
        for i in range(ENEMY):
            s = f"enemy({self.enemy_x[i]},{self.enemy_y[i]})"
            s += f"({self.enemy_tile_x[i]},{self.enemy_tile_y[i]})"
            gui.text(dx, drawy, s)
            drawy = drawy + ddrawy
        gui.text(dx, drawy, f"max {self._max_stage_mario_x}, stepout {self._stepout}")

    def _draw_block(self, x, y, color, base_x, base_y, box_size):
        gui.drawBox(
            base_x + x * box_size,
            base_y + y * box_size,
            base_x + (x + 1) * box_size,
            base_y + (y + 1) * box_size,
            color,
            0x00000000,
        )


if __name__ == "__main__":
    env_client.run(GameController())
