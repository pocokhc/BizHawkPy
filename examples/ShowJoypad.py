from bizhawk_api import joypad

for k, v in joypad.get().items():
    print(f"{k:20s}:{v}")
