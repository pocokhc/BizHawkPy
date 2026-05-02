import math

from bizhawk_api import emu, gui, movie

while True:
    if movie.isloaded():
        if movie.getheader()["Core"] == "Gambatte":
            clockrate = 2**21
            cycles = emu.totalexecutedcycles()
            tseconds = cycles / clockrate
        elif movie.getheader()["Core"] == "SubGBHawk":
            clockrate = 2**22
            cycles = emu.totalexecutedcycles()
            tseconds = cycles / clockrate
        else:
            fps = movie.getfps()
            frames = emu.framecount()
            tseconds = frames / fps
        secondsraw = tseconds % 60
        shift = 10**2

        seconds = math.floor((secondsraw * shift) + 0.5) / shift
        secondsstr = f"{seconds:05.2f}"

        minutes = (math.floor(tseconds / 60)) % 60
        minutesstr = f"{minutes:02d}"

        hours = (math.floor(tseconds / 60 / 60)) % 24

        time = f"{minutesstr}:{secondsstr}"
        if hours > 0:
            time = f"0{hours}:{time}"
        gui.text(0, 0, time, None, "1")
    emu.frameadvance()
