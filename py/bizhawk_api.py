import json
import sys
import time
from typing import Any, cast


def console_print(s):
    print(str(s), file=sys.stderr)


def _format_arg(d: Any) -> str:
    """引数を送信用フォーマットに変換"""
    if isinstance(d, (list, tuple)):
        return json.dumps(d, ensure_ascii=True)

    if isinstance(d, dict):
        return json.dumps(d, ensure_ascii=True)

    if isinstance(d, bool):
        return "1" if d else "0"

    if d is None:
        return ""

    return str(d)


def _send(cmd: str, args: list | None = None):
    args = args or []
    args_str = "".join(f"|{_format_arg(a)}" for a in args)
    print(f"|C|{cmd}{args_str}", file=sys.stdout, flush=True)


def _recv() -> str | None:
    while True:
        line: str = sys.stdin.readline()
        if not line:
            time.sleep(0.1)
            continue
        break
    line = line.strip()

    if line in ["None", "none", "null", "NULL", ""]:
        return None

    return line


def _serialize_object(data: object) -> str:
    return json.dumps(data, ensure_ascii=False)


def _deserialize_bool(data: str | None) -> bool:
    return data == "1"


def _deserialize_int(data: str | None) -> int:
    try:
        return int(data) if data else 0
    except (ValueError, TypeError) as e:
        console_print(f"[WARNING] Deserialize Error. data='{data}': {e}")
        return 0


def _deserialize_float(data: str | None) -> float:
    try:
        return float(data) if data else 0.0
    except (ValueError, TypeError) as e:
        console_print(f"[WARNING] deserialize_float fail. data='{data}': {e}")
        return 0.0


def _deserialize_str(data: str | None) -> str:
    if data is None:
        return ""
    try:
        return data
        # return base64.b64decode(data).decode("utf-8")
    except (ValueError, TypeError) as e:
        console_print(f"[WARNING] deserialize_float fail. data='{data}': {e}")
        return ""


def _deserialize_object(data: str | None) -> object | None:
    if data is None:
        return None
    try:
        return json.loads(data)
        # decoded = base64.b64decode(data)
        # text = decoded.decode("utf-8")
        # return json.loads(text)
    except Exception as e:
        console_print(f"[WARNING] Deserialize Error. input='{data}': {e}")
        raise


def _deserialize_list(data: str | None) -> list:
    d = _deserialize_object(data)
    return [] if d is None else cast(list, data)


def _deserialize_dict(data: str | None) -> dict:
    d = _deserialize_object(data)
    return {} if d is None else cast(dict, d)


# ================================
# API
# ================================

# bit
# bizstring


class Client:
    @staticmethod
    def addcheat(code: str) -> None:
        _send("client.addcheat", [code])
        _recv()

    @staticmethod
    def borderheight() -> int:
        _send("client.borderheight", [])
        return _deserialize_int(_recv())

    @staticmethod
    def borderwidth() -> int:
        _send("client.borderwidth", [])
        return _deserialize_int(_recv())

    @staticmethod
    def bufferheight() -> int:
        _send("client.bufferheight", [])
        return _deserialize_int(_recv())

    @staticmethod
    def bufferwidth() -> int:
        _send("client.bufferwidth", [])
        return _deserialize_int(_recv())

    @staticmethod
    def clearautohold() -> None:
        _send("client.clearautohold", [])
        _recv()

    @staticmethod
    def closerom() -> None:
        _send("client.closerom", [])
        _recv()

    @staticmethod
    def createinstance(name: str) -> dict:
        _send("client.createinstance", [name])
        return _deserialize_dict(_recv())

    @staticmethod
    def displaymessages(value: bool) -> None:
        _send("client.displaymessages", [value])
        _recv()

    @staticmethod
    def enablerewind(enabled: bool) -> None:
        _send("client.enablerewind", [enabled])
        _recv()

    @staticmethod
    def exactsleep(millis: int) -> None:
        _send("client.exactsleep", [millis])
        _recv()

    @staticmethod
    def exit() -> None:
        _send("client.exit", [])
        _recv()

    @staticmethod
    def exitCode(exitcode: int) -> None:
        _send("client.exitCode", [exitcode])
        _recv()

    @staticmethod
    def frameskip(numframes: int) -> None:
        _send("client.frameskip", [numframes])
        _recv()

    @staticmethod
    def get_approx_framerate() -> int:
        _send("client.get_approx_framerate", [])
        return _deserialize_int(_recv())

    @staticmethod
    def get_lua_engine() -> str:
        _send("client.get_lua_engine", [])
        return _deserialize_str(_recv())

    @staticmethod
    def getavailabletools() -> dict:
        _send("client.getavailabletools", [])
        return _deserialize_dict(_recv())

    @staticmethod
    def getconfig() -> dict:
        _send("client.getconfig", [])
        return _deserialize_dict(_recv())

    @staticmethod
    def GetSoundOn() -> bool:
        _send("client.GetSoundOn", [])
        return _deserialize_bool(_recv())

    @staticmethod
    def gettargetscanlineintensity() -> int:
        _send("client.gettargetscanlineintensity", [])
        return _deserialize_int(_recv())

    @staticmethod
    def gettool(name: str) -> dict:
        _send("client.gettool", [name])
        return _deserialize_dict(_recv())

    @staticmethod
    def getversion() -> str:
        _send("client.getversion", [])
        return _deserialize_str(_recv())

    @staticmethod
    def getwindowsize() -> int:
        _send("client.getwindowsize", [])
        return _deserialize_int(_recv())

    @staticmethod
    def invisibleemulation(invisible: bool) -> None:
        _send("client.invisibleemulation", [invisible])
        _recv()

    @staticmethod
    def ispaused() -> bool:
        _send("client.ispaused", [])
        return _deserialize_bool(_recv())

    @staticmethod
    def isseeking() -> bool:
        _send("client.isseeking", [])
        return _deserialize_bool(_recv())

    @staticmethod
    def isturbo() -> bool:
        _send("client.isturbo", [])
        return _deserialize_bool(_recv())

    @staticmethod
    def opencheats() -> None:
        _send("client.opencheats", [])
        _recv()

    @staticmethod
    def openhexeditor() -> None:
        _send("client.openhexeditor", [])
        _recv()

    @staticmethod
    def openramsearch() -> None:
        _send("client.openramsearch", [])
        _recv()

    @staticmethod
    def openramwatch() -> None:
        _send("client.openramwatch", [])
        _recv()

    @staticmethod
    def openrom(path: str) -> bool:
        _send("client.openrom", [path])
        return _deserialize_bool(_recv())

    @staticmethod
    def opentasstudio() -> None:
        _send("client.opentasstudio", [])
        _recv()

    @staticmethod
    def opentoolbox() -> None:
        _send("client.opentoolbox", [])
        _recv()

    @staticmethod
    def opentracelogger() -> None:
        _send("client.opentracelogger", [])
        _recv()

    @staticmethod
    def pause() -> None:
        _send("client.pause", [])
        _recv()

    @staticmethod
    def pause_av() -> None:
        _send("client.pause_av", [])
        _recv()

    @staticmethod
    def reboot_core() -> None:
        _send("client.reboot_core", [])
        _recv()

    @staticmethod
    def removecheat(code: str) -> None:
        _send("client.removecheat", [code])
        _recv()

    @staticmethod
    def saveram() -> None:
        _send("client.saveram", [])
        _recv()

    @staticmethod
    def screenheight() -> int:
        _send("client.screenheight", [])
        return _deserialize_int(_recv())

    @staticmethod
    def screenshot(path: str | None = None) -> None:
        _send("client.screenshot", [path])
        _recv()

    @staticmethod
    def screenshottoclipboard() -> None:
        _send("client.screenshottoclipboard", [])
        _recv()

    @staticmethod
    def screenwidth() -> int:
        _send("client.screenwidth", [])
        return _deserialize_int(_recv())

    @staticmethod
    def SetClientExtraPadding(left: int, top: int, right: int, bottom: int) -> None:
        _send("client.SetClientExtraPadding", [left, top, right, bottom])
        _recv()

    @staticmethod
    def SetGameExtraPadding(left: int, top: int, right: int, bottom: int) -> None:
        _send("client.SetGameExtraPadding", [left, top, right, bottom])
        _recv()

    @staticmethod
    def setscreenshotosd(value: bool) -> None:
        _send("client.setscreenshotosd", [value])
        _recv()

    @staticmethod
    def SetSoundOn(enable: bool) -> None:
        _send("client.SetSoundOn", [enable])
        _recv()

    @staticmethod
    def settargetscanlineintensity(val: int) -> None:
        _send("client.settargetscanlineintensity", [val])
        _recv()

    @staticmethod
    def setwindowsize(size: int) -> None:
        _send("client.setwindowsize", [size])
        _recv()

    @staticmethod
    def sleep(millis: int) -> None:
        _send("client.sleep", [millis])
        _recv()

    @staticmethod
    def speedmode(percent: int) -> None:
        _send("client.speedmode", [percent])
        _recv()

    @staticmethod
    def togglepause() -> None:
        _send("client.togglepause", [])
        _recv()

    @staticmethod
    def transformPoint(x: int, y: int) -> dict:
        _send("client.transformPoint", [x, y])
        return _deserialize_dict(_recv())

    @staticmethod
    def unpause() -> None:
        _send("client.unpause", [])
        _recv()

    @staticmethod
    def unpause_av() -> None:
        _send("client.unpause_av", [])
        _recv()

    @staticmethod
    def xpos() -> int:
        _send("client.xpos", [])
        return _deserialize_int(_recv())

    @staticmethod
    def ypos() -> int:
        _send("client.ypos", [])
        return _deserialize_int(_recv())


client = Client


class Comm:
    @staticmethod
    def getluafunctionslist() -> str:
        _send("comm.getluafunctionslist")
        return _deserialize_str(_recv())

    # -------------------------
    # HTTP
    # -------------------------
    @staticmethod
    def httpGet(url: str) -> str:
        _send("comm.httpGet", [url])
        return _deserialize_str(_recv())

    @staticmethod
    def httpPost(url: str, payload: str) -> str:
        _send("comm.httpPost", [url, payload])
        return _deserialize_str(_recv())

    @staticmethod
    def httpGetGetUrl() -> str:
        _send("comm.httpGetGetUrl")
        return _deserialize_str(_recv())

    @staticmethod
    def httpGetPostUrl() -> str:
        _send("comm.httpGetPostUrl")
        return _deserialize_str(_recv())

    @staticmethod
    def httpSetGetUrl(url: str) -> None:
        _send("comm.httpSetGetUrl", [url])
        _recv()

    @staticmethod
    def httpSetPostUrl(url: str) -> None:
        _send("comm.httpSetPostUrl", [url])
        _recv()

    @staticmethod
    def httpSetTimeout(timeout: int) -> None:
        _send("comm.httpSetTimeout", [timeout])
        _recv()

    @staticmethod
    def httpTest() -> str:
        _send("comm.httpTest")
        return _deserialize_str(_recv())

    @staticmethod
    def httpTestGet() -> str:
        _send("comm.httpTestGet")
        return _deserialize_str(_recv())

    # -------------------------
    # MMF (MemoryMappedFile)
    # -------------------------
    @staticmethod
    def mmfCopyFromMemory(mmf_filename: str, addr: int, length: int, domain: str) -> int:
        _send("comm.mmfCopyFromMemory", [mmf_filename, addr, length, domain])
        return _deserialize_int(_recv())

    @staticmethod
    def mmfCopyToMemory(mmf_filename: str, addr: int, length: int, domain: str) -> None:
        _send("comm.mmfCopyToMemory", [mmf_filename, addr, length, domain])
        _recv()

    @staticmethod
    def mmfGetFilename() -> str:
        _send("comm.mmfGetFilename", [])
        return _deserialize_str(_recv())

    @staticmethod
    def mmfRead(mmf_filename: str, expectedsize: int) -> str:
        _send("comm.mmfRead", [mmf_filename, expectedsize])
        return _deserialize_str(_recv())

    @staticmethod
    def mmfReadBytes(mmf_filename: str, expectedsize: int) -> list[int]:
        _send("comm.mmfReadBytes", [mmf_filename, expectedsize])
        return _deserialize_list(_recv())

    @staticmethod
    def mmfScreenshot() -> int:
        _send("comm.mmfScreenshot", [])
        return _deserialize_int(_recv())

    @staticmethod
    def mmfSetFilename(filename: str) -> None:
        _send("comm.mmfSetFilename", [filename])
        _recv()

    @staticmethod
    def mmfWrite(mmf_filename: str, outputstring: str) -> int:
        _send("comm.mmfWrite", [mmf_filename, outputstring])
        return _deserialize_int(_recv())

    @staticmethod
    def mmfWriteBytes(mmf_filename: str, bytearray: list[int]) -> int:
        _send("comm.mmfWriteBytes", [mmf_filename, bytearray])
        return _deserialize_int(_recv())

    # -------------------------
    # Socket
    # -------------------------
    @staticmethod
    def socketServerGetInfo() -> str:
        _send("comm.socketServerGetInfo", [])
        return _deserialize_str(_recv())

    @staticmethod
    def socketServerGetIp() -> str:
        _send("comm.socketServerGetIp", [])
        return _deserialize_str(_recv())

    @staticmethod
    def socketServerGetPort() -> int | None:
        _send("comm.socketServerGetPort", [])
        r = _recv()
        return None if r is None else _deserialize_int(r)

    @staticmethod
    def socketServerIsConnected() -> bool:
        _send("comm.socketServerIsConnected", [])
        return _deserialize_bool(_recv())

    @staticmethod
    def socketServerResponse() -> str:
        _send("comm.socketServerResponse", [])
        return _deserialize_str(_recv())

    @staticmethod
    def socketServerScreenShot() -> str:
        _send("comm.socketServerScreenShot", [])
        return _deserialize_str(_recv())

    @staticmethod
    def socketServerScreenShotResponse() -> str:
        _send("comm.socketServerScreenShotResponse", [])
        return _deserialize_str(_recv())

    @staticmethod
    def socketServerSend(sendstring: str) -> int:
        _send("comm.socketServerSend", [sendstring])
        return _deserialize_int(_recv())

    @staticmethod
    def socketServerSendBytes(bytearray: list[int]) -> int:
        _send("comm.socketServerSendBytes", [bytearray])
        return _deserialize_int(_recv())

    @staticmethod
    def socketServerSetIp(ip: str) -> None:
        _send("comm.socketServerSetIp", [ip])
        _recv()

    @staticmethod
    def socketServerSetPort(port: int) -> None:
        _send("comm.socketServerSetPort", [port])
        _recv()

    @staticmethod
    def socketServerSetTimeout(timeout: int) -> None:
        _send("comm.socketServerSetTimeout", [timeout])
        _recv()

    @staticmethod
    def socketServerSuccessful() -> bool:
        _send("comm.socketServerSuccessful", [])
        return _deserialize_bool(_recv())


comm = Comm


class Console:
    @staticmethod
    def clear() -> None:
        _send("console.clear")
        _recv()

    @staticmethod
    def log(message: str) -> None:
        _send("console.log", [message])
        _recv()

    @staticmethod
    def writeline(message: str) -> None:
        _send("console.writeline", [message])
        _recv()

    @staticmethod
    def write(message: str) -> None:
        _send("console.write", [message])
        _recv()

    @staticmethod
    def getluafunctionslist() -> str:
        _send("console.getluafunctionslist")
        return _deserialize_str(_recv())


console = Console


class Emu:
    @staticmethod
    def disassemble(pc: int, name: str | None = None) -> dict:
        _send("emu.disassemble", [pc, name])
        return _deserialize_dict(_recv())

    @staticmethod
    def displayvsync(enabled: bool) -> None:
        _send("emu.displayvsync", [enabled])
        _recv()

    @staticmethod
    def frameadvance() -> None:
        _send("emu.frameadvance")
        _recv()

    @staticmethod
    def framecount() -> int:
        _send("emu.framecount")
        return _deserialize_int(_recv())

    @staticmethod
    def getboardname() -> str:
        _send("emu.getboardname")
        return _deserialize_str(_recv())

    @staticmethod
    def getdisplaytype() -> str:
        _send("emu.getdisplaytype")
        return _deserialize_str(_recv())

    @staticmethod
    def getregister(name: str) -> int:
        _send("emu.getregister", [name])
        return _deserialize_int(_recv())

    @staticmethod
    def getregisters() -> dict:
        _send("emu.getregisters")
        return _deserialize_dict(_recv())

    @staticmethod
    def getsystemid() -> str:
        _send("emu.getsystemid")
        return _deserialize_str(_recv())

    @staticmethod
    def islagged() -> bool:
        _send("emu.islagged")
        return _deserialize_bool(_recv())

    @staticmethod
    def lagcount() -> int:
        _send("emu.lagcount")
        return _deserialize_int(_recv())

    @staticmethod
    def limitframerate(enabled: bool) -> None:
        _send("emu.limitframerate", [enabled])
        _recv()

    @staticmethod
    def minimizeframeskip(enabled: bool) -> None:
        _send("emu.minimizeframeskip", [enabled])
        _recv()

    @staticmethod
    def setislagged(value: bool = True) -> None:
        _send("emu.setislagged", [value])
        _recv()

    @staticmethod
    def setlagcount(count: int) -> None:
        _send("emu.setlagcount", [count])
        _recv()

    @staticmethod
    def setregister(register: str, value: int) -> None:
        _send("emu.setregister", [register, value])
        _recv()

    @staticmethod
    def setrenderplanes(luaparam: list[bool]) -> None:
        _send("emu.setrenderplanes", [luaparam])
        _recv()

    @staticmethod
    def totalexecutedcycles() -> int:
        _send("emu.totalexecutedcycles")
        return _deserialize_int(_recv())

    @staticmethod
    def yield_() -> None:
        # Pythonの予約語回避
        _send("emu.yield")
        _recv()


emu = Emu


class Event:
    @staticmethod
    def availableScopes() -> dict:
        """利用可能なスコープ一覧を取得"""
        _send("event.availableScopes", [])
        return _deserialize_dict(_recv())

    @staticmethod
    def can_use_callback_params(subset: str | None = None) -> bool:
        """コールバックに引数が渡されるか"""
        _send("event.can_use_callback_params", [subset])
        return _deserialize_bool(_recv())

    @staticmethod
    def on_bus_exec(luaf, address: int, name: str | None = None, scope: str | None = None) -> str:
        """指定アドレス実行前"""
        _send("event.on_bus_exec", [luaf, address, name, scope])
        return _deserialize_str(_recv())

    @staticmethod
    def on_bus_exec_any(luaf, name: str | None = None, scope: str | None = None) -> str:
        """全命令実行前"""
        _send("event.on_bus_exec_any", [luaf, name, scope])
        return _deserialize_str(_recv())

    @staticmethod
    def on_bus_read(luaf, address: int | None = None, name: str | None = None, scope: str | None = None) -> str:
        """メモリ読み取り前"""
        _send("event.on_bus_read", [luaf, address, name, scope])
        return _deserialize_str(_recv())

    @staticmethod
    def on_bus_write(luaf, address: int | None = None, name: str | None = None, scope: str | None = None) -> str:
        """メモリ書き込み前"""
        _send("event.on_bus_write", [luaf, address, name, scope])
        return _deserialize_str(_recv())

    @staticmethod
    def onconsoleclose(luaf, name: str | None = None) -> str:
        """コンソール終了時"""
        _send("event.onconsoleclose", [luaf, name])
        return _deserialize_str(_recv())

    @staticmethod
    def onexit(luaf, name: str | None = None) -> str:
        """スクリプト終了時"""
        _send("event.onexit", [luaf, name])
        return _deserialize_str(_recv())

    @staticmethod
    def onframeend(luaf, name: str | None = None) -> str:
        """フレーム終了時"""
        _send("event.onframeend", [luaf, name])
        return _deserialize_str(_recv())

    @staticmethod
    def onframestart(luaf, name: str | None = None) -> str:
        """フレーム開始時"""
        _send("event.onframestart", [luaf, name])
        return _deserialize_str(_recv())

    @staticmethod
    def oninputpoll(luaf, name: str | None = None) -> str:
        """入力ポーリング後"""
        _send("event.oninputpoll", [luaf, name])
        return _deserialize_str(_recv())

    @staticmethod
    def onloadstate(luaf, name: str | None = None) -> str:
        """ステートロード後"""
        _send("event.onloadstate", [luaf, name])
        return _deserialize_str(_recv())

    @staticmethod
    def onsavestate(luaf, name: str | None = None) -> str:
        """ステート保存後"""
        _send("event.onsavestate", [luaf, name])
        return _deserialize_str(_recv())

    @staticmethod
    def unregisterbyid(guid: str) -> bool:
        """GUIDで解除"""
        _send("event.unregisterbyid", [guid])
        return bool(_recv())

    @staticmethod
    def unregisterbyname(name: str) -> bool:
        """名前で解除"""
        _send("event.unregisterbyname", [name])
        return bool(_recv())


# event = Event

# forms = Forms


class GameInfo:
    @staticmethod
    def getboardtype() -> str:
        _send("gameinfo.getboardtype", [])
        return _deserialize_str(_recv())

    @staticmethod
    def getoptions() -> dict:
        _send("gameinfo.getoptions", [])
        return _deserialize_dict(_recv())

    @staticmethod
    def getromhash() -> str:
        _send("gameinfo.getromhash", [])
        return _deserialize_str(_recv())

    @staticmethod
    def getromname() -> str:
        _send("gameinfo.getromname", [])
        return _deserialize_str(_recv())

    @staticmethod
    def getstatus() -> str:
        _send("gameinfo.getstatus", [])
        return _deserialize_str(_recv())

    @staticmethod
    def indatabase() -> bool:
        _send("gameinfo.indatabase", [])
        return _deserialize_bool(_recv())

    @staticmethod
    def isstatusbad() -> bool:
        _send("gameinfo.isstatusbad", [])
        return _deserialize_bool(_recv())


gameinfo = GameInfo

# genesis = Genesis


class Gui:
    @staticmethod
    def addmessage(message: str) -> None:
        _send("gui.addmessage", [message])
        _recv()

    @staticmethod
    def clearGraphics(surfacename: str | None = None) -> None:
        _send("gui.clearGraphics", [surfacename])
        _recv()

    @staticmethod
    def clearImageCache() -> None:
        _send("gui.clearImageCache", [])
        _recv()

    @staticmethod
    def cleartext() -> None:
        _send("gui.cleartext", [])
        _recv()

    @staticmethod
    def createcanvas(width: int, height: int, x: int | None = None, y: int | None = None) -> dict:
        _send("gui.createcanvas", [width, height, x, y])
        return _deserialize_dict(_recv())

    @staticmethod
    def defaultBackground(color) -> None:
        _send("gui.defaultBackground", [color])
        _recv()

    @staticmethod
    def defaultForeground(color) -> None:
        _send("gui.defaultForeground", [color])
        _recv()

    @staticmethod
    def defaultPixelFont(fontfamily: str) -> None:
        _send("gui.defaultPixelFont", [fontfamily])
        _recv()

    @staticmethod
    def defaultTextBackground(color) -> None:
        _send("gui.defaultTextBackground", [color])
        _recv()

    @staticmethod
    def drawAxis(x: int, y: int, size: int, color=None, surfacename: str | None = None) -> None:
        _send("gui.drawAxis", [x, y, size, color, surfacename])
        _recv()

    @staticmethod
    def drawBezier(
        x1: int,
        y1: int,
        x2: int,
        y2: int,
        x3: int,
        y3: int,
        x4: int,
        y4: int,
        color,
        surfacename: str | None = None,
    ) -> None:
        _send("gui.drawBezier", [x1, y1, x2, y2, x3, y3, x4, y4, color, surfacename])
        _recv()

    @staticmethod
    def drawBox(x: int, y: int, x2: int, y2: int, line=None, background=None, surfacename: str | None = None) -> None:
        _send("gui.drawBox", [x, y, x2, y2, line, background, surfacename])
        _recv()

    @staticmethod
    def drawEllipse(x: int, y: int, width: int, height: int, line=None, background=None, surfacename: str | None = None) -> None:
        _send("gui.drawEllipse", [x, y, width, height, line, background, surfacename])
        _recv()

    @staticmethod
    def drawIcon(path: str, x: int, y: int, width: int | None = None, height: int | None = None, surfacename: str | None = None) -> None:
        _send("gui.drawIcon", [path, x, y, width, height, surfacename])
        _recv()

    @staticmethod
    def drawImage(path: str, x: int, y: int, width: int | None = None, height: int | None = None, cache: bool = True, surfacename: str | None = None) -> None:
        _send("gui.drawImage", [path, x, y, width, height, cache, surfacename])
        _recv()

    @staticmethod
    def drawImageRegion(path: str, sx: int, sy: int, sw: int, sh: int, dx: int, dy: int, dw: int | None = None, dh: int | None = None, surfacename: str | None = None) -> None:
        _send("gui.drawImageRegion", [path, sx, sy, sw, sh, dx, dy, dw, dh, surfacename])
        _recv()

    @staticmethod
    def drawLine(x1: int, y1: int, x2: int, y2: int, color=None, surfacename: str | None = None) -> None:
        _send("gui.drawLine", [x1, y1, x2, y2, color, surfacename])
        _recv()

    @staticmethod
    def drawPie(x: int, y: int, width: int, height: int, startangle: int, sweepangle: int, line=None, background=None, surfacename: str | None = None) -> None:
        _send("gui.drawPie", [x, y, width, height, startangle, sweepangle, line, background, surfacename])
        _recv()

    @staticmethod
    def drawPixel(x: int, y: int, color=None, surfacename: str | None = None) -> None:
        _send("gui.drawPixel", [x, y, color, surfacename])
        _recv()

    @staticmethod
    def drawPolygon(points: list[tuple[int, int]], offsetx: int | None = None, offsety: int | None = None, line=None, background=None, surfacename: str | None = None) -> None:
        _send("gui.drawPolygon", [points, offsetx, offsety, line, background, surfacename])
        _recv()

    @staticmethod
    def drawRectangle(x: int, y: int, width: int, height: int, line=None, background=None, surfacename: str | None = None) -> None:
        _send("gui.drawRectangle", [x, y, width, height, line, background, surfacename])
        _recv()

    @staticmethod
    def drawString(x: int, y: int, message: str, forecolor=None, backcolor=None, fontsize: int | None = None, fontfamily: str | None = None, fontstyle: str | None = None, horizalign: str | None = None, vertalign: str | None = None, surfacename: str | None = None) -> None:
        _send("gui.drawString", [x, y, message, forecolor, backcolor, fontsize, fontfamily, fontstyle, horizalign, vertalign, surfacename])
        _recv()

    @staticmethod
    def drawText(*args, **kwargs) -> None:
        """drawStringのエイリアス"""
        Gui.drawString(*args, **kwargs)

    @staticmethod
    def pixelText(x: int, y: int, message: str, forecolor=None, backcolor=None, fontfamily: str | None = None, surfacename: str | None = None) -> None:
        _send("gui.pixelText", [x, y, message, forecolor, backcolor, fontfamily, surfacename])
        _recv()

    @staticmethod
    def text(x: int, y: int, message: str, forecolor=None, anchor: str | None = None) -> None:
        _send("gui.text", [x, y, message, forecolor, anchor])
        _recv()

    @staticmethod
    def use_surface(surfacename: str) -> None:
        _send("gui.use_surface", [surfacename])
        _recv()


gui = Gui

# input = Input


class Joypad:
    @staticmethod
    def get(controller: int | None = None) -> dict:
        _send("joypad.get", [controller])
        return _deserialize_dict(_recv())

    @staticmethod
    def getimmediate(controller: int | None = None) -> dict:
        _send("joypad.getimmediate", [controller])
        return _deserialize_dict(_recv())

    @staticmethod
    def getwithmovie(controller: int | None = None) -> dict:
        _send("joypad.getwithmovie", [controller])
        return _deserialize_dict(_recv())

    @staticmethod
    def set(buttons: dict, controller: int | None = None) -> None:
        filtered = {k: v for k, v in buttons.items() if isinstance(v, bool)}
        _send("joypad.set", [filtered, controller])
        _recv()

    @staticmethod
    def setanalog(controls: dict, controller: int | None = None) -> None:
        filtered = {k: v for k, v in controls.items() if isinstance(v, int)}
        _send("joypad.setanalog", [filtered, controller])
        _recv()

    @staticmethod
    def setfrommnemonicstr(inputlogentry: str) -> None:
        _send("joypad.setfrommnemonicstr", [inputlogentry])
        _recv()


joypad = Joypad

# LuaCanvas

# mainmemory


class Memory:
    @staticmethod
    def getcurrentmemorydomain() -> str:
        _send("memory.getcurrentmemorydomain", [])
        return _deserialize_str(_recv())

    @staticmethod
    def getcurrentmemorydomainsize() -> int:
        _send("memory.getcurrentmemorydomainsize", [])
        return _deserialize_int(_recv())

    @staticmethod
    def getmemorydomainlist() -> list[str]:
        _send("memory.getmemorydomainlist", [])
        return _deserialize_list(_recv())

    @staticmethod
    def getmemorydomainsize(name: str | None = None) -> int:
        _send("memory.getmemorydomainsize", [name])
        return _deserialize_int(_recv())

    @staticmethod
    def hash_region(addr: int, count: int, domain: str | None = None) -> str:
        _send("memory.hash_region", [addr, count, domain])
        return _deserialize_str(_recv())

    @staticmethod
    def read_bytes_as_array(addr: int, length: int, domain: str | None = None) -> list:
        _send("memory.read_bytes_as_array", [addr, length, domain])
        return _deserialize_list(_recv())

    @staticmethod
    def read_bytes_as_dict(addr: int, length: int, domain: str | None = None) -> dict:
        _send("memory.read_bytes_as_dict", [addr, length, domain])
        return _deserialize_dict(_recv())

    @staticmethod
    def read_s16_be(addr: int, domain: str | None = None) -> int:
        _send("memory.read_s16_be", [addr, domain])
        return _deserialize_int(_recv())

    @staticmethod
    def read_s16_le(addr: int, domain: str | None = None) -> int:
        _send("memory.read_s16_le", [addr, domain])
        return _deserialize_int(_recv())

    @staticmethod
    def read_s24_be(addr: int, domain: str | None = None) -> int:
        _send("memory.read_s24_be", [addr, domain])
        return _deserialize_int(_recv())

    @staticmethod
    def read_s24_le(addr: int, domain: str | None = None) -> int:
        _send("memory.read_s24_le", [addr, domain])
        return _deserialize_int(_recv())

    @staticmethod
    def read_s32_be(addr: int, domain: str | None = None) -> int:
        _send("memory.read_s32_be", [addr, domain])
        return _deserialize_int(_recv())

    @staticmethod
    def read_s32_le(addr: int, domain: str | None = None) -> int:
        _send("memory.read_s32_le", [addr, domain])
        return _deserialize_int(_recv())

    @staticmethod
    def read_s8(addr: int, domain: str | None = None) -> int:
        _send("memory.read_s8", [addr, domain])
        return _deserialize_int(_recv())

    @staticmethod
    def read_u16_be(addr: int, domain: str | None = None) -> int:
        _send("memory.read_u16_be", [addr, domain])
        return _deserialize_int(_recv())

    @staticmethod
    def read_u16_le(addr: int, domain: str | None = None) -> int:
        _send("memory.read_u16_le", [addr, domain])
        return _deserialize_int(_recv())

    @staticmethod
    def read_u24_be(addr: int, domain: str | None = None) -> int:
        _send("memory.read_u24_be", [addr, domain])
        return _deserialize_int(_recv())

    @staticmethod
    def read_u24_le(addr: int, domain: str | None = None) -> int:
        _send("memory.read_u24_le", [addr, domain])
        return _deserialize_int(_recv())

    @staticmethod
    def read_u32_be(addr: int, domain: str | None = None) -> int:
        _send("memory.read_u32_be", [addr, domain])
        return _deserialize_int(_recv())

    @staticmethod
    def read_u32_le(addr: int, domain: str | None = None) -> int:
        _send("memory.read_u32_le", [addr, domain])
        return _deserialize_int(_recv())

    @staticmethod
    def read_u8(addr: int, domain: str | None = None) -> int:
        _send("memory.read_u8", [addr, domain])
        return _deserialize_int(_recv())

    @staticmethod
    def readbyte(addr: int, domain: str | None = None) -> int:
        _send("memory.readbyte", [addr, domain])
        return _deserialize_int(_recv())

    # [deprecated]
    # @staticmethod
    # def readbyterange(addr: int, length: int, domain: str | None = None) -> dict:
    #    _send("memory.readbyterange", [addr, domain])
    #    return _deserialize_int(_recv())

    @staticmethod
    def readfloat(addr: int, bigendian: bool, domain: str | None = None) -> float:
        _send("memory.readfloat", [addr, bigendian, domain])
        return _deserialize_float(_recv())

    @staticmethod
    def usememorydomain(domain: str) -> bool:
        _send("memory.usememorydomain", [domain])
        return _deserialize_bool(_recv())

    @staticmethod
    def write_bytes_as_array(addr: int, bytes_: list, domain: str | None = None) -> None:
        _send("memory.write_bytes_as_array", [addr, bytes_, domain])
        _recv()

    @staticmethod
    def write_bytes_as_dict(addrmap: dict, domain: str | None = None) -> None:
        _send("memory.write_bytes_as_dict", [addrmap, domain])
        _recv()

    @staticmethod
    def write_s16_be(addr: int, value: int, domain: str | None = None) -> None:
        _send("memory.write_s16_be", [addr, value, domain])
        _recv()

    @staticmethod
    def write_s16_le(addr: int, value: int, domain: str | None = None) -> None:
        _send("memory.write_s16_le", [addr, value, domain])
        _recv()

    @staticmethod
    def write_s24_be(addr: int, value: int, domain: str | None = None) -> None:
        _send("memory.write_s24_be", [addr, value, domain])
        _recv()

    @staticmethod
    def write_s24_le(addr: int, value: int, domain: str | None = None) -> None:
        _send("memory.write_s24_le", [addr, value, domain])
        _recv()

    @staticmethod
    def write_s32_be(addr: int, value: int, domain: str | None = None) -> None:
        _send("memory.write_s32_be", [addr, value, domain])
        _recv()

    @staticmethod
    def write_s32_le(addr: int, value: int, domain: str | None = None) -> None:
        _send("memory.write_s32_le", [addr, value, domain])
        _recv()

    @staticmethod
    def write_s8(addr: int, value: int, domain: str | None = None) -> None:
        _send("memory.write_s8", [addr, value, domain])
        _recv()

    @staticmethod
    def write_u16_be(addr: int, value: int, domain: str | None = None) -> None:
        _send("memory.write_u16_be", [addr, value, domain])
        _recv()

    @staticmethod
    def write_u16_le(addr: int, value: int, domain: str | None = None) -> None:
        _send("memory.write_u16_le", [addr, value, domain])
        _recv()

    @staticmethod
    def write_u24_be(addr: int, value: int, domain: str | None = None) -> None:
        _send("memory.write_u24_be", [addr, value, domain])
        _recv()

    @staticmethod
    def write_u24_le(addr: int, value: int, domain: str | None = None) -> None:
        _send("memory.write_u24_le", [addr, value, domain])
        _recv()

    @staticmethod
    def write_u32_be(addr: int, value: int, domain: str | None = None) -> None:
        _send("memory.write_u32_be", [addr, value, domain])
        _recv()

    @staticmethod
    def write_u32_le(addr: int, value: int, domain: str | None = None) -> None:
        _send("memory.write_u32_le", [addr, value, domain])
        _recv()

    @staticmethod
    def write_u8(addr: int, value: int, domain: str | None = None) -> None:
        _send("memory.write_u8", [addr, value, domain])
        _recv()

    @staticmethod
    def writebyte(addr: int, value: int, domain: str | None = None) -> None:
        _send("memory.writebyte", [addr, value, domain])
        _recv()

    @staticmethod
    def writefloat(addr: int, value: float, bigendian: bool, domain: str | None = None) -> None:
        _send("memory.writefloat", [addr, value, bigendian, domain])
        _recv()


memory = Memory


class MemorySaveState:
    @staticmethod
    def clearstatesfrommemory() -> None:
        _send("memorysavestate.clearstatesfrommemory", [])
        _recv()

    @staticmethod
    def loadcorestate(identifier: str) -> None:
        _send("memorysavestate.loadcorestate", [identifier])
        _recv()

    @staticmethod
    def removestate(identifier: str) -> None:
        _send("memorysavestate.removestate", [identifier])
        _recv()

    @staticmethod
    def savecorestate() -> str:
        _send("memorysavestate.savecorestate", [])
        return _deserialize_str(_recv())


memorysavestate = MemorySaveState


class Movie:
    @staticmethod
    def filename() -> str:
        _send("movie.filename", [])
        return _deserialize_str(_recv())

    @staticmethod
    def getcomments() -> list[dict]:
        _send("movie.getcomments", [])
        return _deserialize_list(_recv())

    @staticmethod
    def getfps() -> float:
        _send("movie.getfps", [])
        return _deserialize_float(_recv())

    @staticmethod
    def getheader() -> dict:
        _send("movie.getheader", [])
        return _deserialize_dict(_recv())

    @staticmethod
    def getinput(frame: int, controller: int | None = None) -> dict:
        _send("movie.getinput", [frame, controller])
        return _deserialize_dict(_recv())

    @staticmethod
    def getinputasmnemonic(frame: int) -> str:
        _send("movie.getinputasmnemonic", [frame])
        return _deserialize_str(_recv())

    @staticmethod
    def getreadonly() -> bool:
        _send("movie.getreadonly", [])
        return _deserialize_bool(_recv())

    @staticmethod
    def getrerecordcount() -> int:
        _send("movie.getrerecordcount", [])
        return _deserialize_int(_recv())

    @staticmethod
    def getrerecordcounting() -> bool:
        _send("movie.getrerecordcounting", [])
        return _deserialize_bool(_recv())

    @staticmethod
    def getsubtitles() -> list[dict]:
        _send("movie.getsubtitles", [])
        return _deserialize_list(_recv())

    @staticmethod
    def isloaded() -> bool:
        _send("movie.isloaded", [])
        return _deserialize_bool(_recv())

    @staticmethod
    def length() -> int:
        _send("movie.length", [])
        return _deserialize_int(_recv())

    @staticmethod
    def mode() -> str:
        _send("movie.mode", [])
        return _deserialize_str(_recv())

    @staticmethod
    def play_from_start(path: str | None = None) -> bool:
        _send("movie.play_from_start", [path])
        return _deserialize_bool(_recv())

    @staticmethod
    def save(filename: str | None = None) -> None:
        _send("movie.save", [filename])
        _recv()

    @staticmethod
    def setreadonly(readonly: bool) -> None:
        _send("movie.setreadonly", [readonly])
        _recv()

    @staticmethod
    def setrerecordcount(count: int) -> None:
        _send("movie.setrerecordcount", [count])
        _recv()

    @staticmethod
    def setrerecordcounting(counting: bool) -> None:
        _send("movie.setrerecordcounting", [counting])
        _recv()

    @staticmethod
    def startsfromsaveram() -> bool:
        _send("movie.startsfromsaveram", [])
        return _deserialize_bool(_recv())

    @staticmethod
    def startsfromsavestate() -> bool:
        _send("movie.startsfromsavestate", [])
        return _deserialize_bool(_recv())

    @staticmethod
    def stop(savechanges: bool = True) -> None:
        _send("movie.stop", [savechanges])
        _recv()


movie = Movie


class NDS:
    @staticmethod
    def getaudiobitdepth() -> str:
        _send("nds.getaudiobitdepth", [])
        return _deserialize_str(_recv())

    @staticmethod
    def getscreengap() -> int:
        _send("nds.getscreengap", [])
        return _deserialize_int(_recv())

    @staticmethod
    def getscreeninvert() -> bool:
        _send("nds.getscreeninvert", [])
        return _deserialize_bool(_recv())

    @staticmethod
    def getscreenlayout() -> str:
        _send("nds.getscreenlayout", [])
        return _deserialize_str(_recv())

    @staticmethod
    def getscreenrotation() -> str:
        _send("nds.getscreenrotation", [])
        return _deserialize_str(_recv())

    @staticmethod
    def setaudiobitdepth(value: str) -> None:
        _send("nds.setaudiobitdepth", [value])
        _recv()

    @staticmethod
    def setscreengap(value: int) -> None:
        _send("nds.setscreengap", [value])
        _recv()

    @staticmethod
    def setscreeninvert(value: bool) -> None:
        _send("nds.setscreeninvert", [value])
        _recv()

    @staticmethod
    def setscreenlayout(value: str) -> None:
        _send("nds.setscreenlayout", [value])
        _recv()

    @staticmethod
    def setscreenrotation(value: str) -> None:
        _send("nds.setscreenrotation", [value])
        _recv()


nds = NDS


class NES:
    @staticmethod
    def getallowmorethaneightsprites() -> bool:
        _send("nes.getallowmorethaneightsprites", [])
        return _deserialize_bool(_recv())

    @staticmethod
    def getbottomscanline(pal: bool = False) -> int:
        _send("nes.getbottomscanline", [pal])
        return _deserialize_int(_recv())

    @staticmethod
    def getclipleftandright() -> bool:
        _send("nes.getclipleftandright", [])
        return _deserialize_bool(_recv())

    @staticmethod
    def getdispbackground() -> bool:
        _send("nes.getdispbackground", [])
        return _deserialize_bool(_recv())

    @staticmethod
    def getdispsprites() -> bool:
        _send("nes.getdispsprites", [])
        return _deserialize_bool(_recv())

    @staticmethod
    def gettopscanline(pal: bool = False) -> int:
        _send("nes.gettopscanline", [pal])
        return _deserialize_int(_recv())

    @staticmethod
    def setallowmorethaneightsprites(allow: bool) -> None:
        _send("nes.setallowmorethaneightsprites", [allow])
        _recv()

    @staticmethod
    def setclipleftandright(leftandright: bool) -> None:
        _send("nes.setclipleftandright", [leftandright])
        _recv()

    @staticmethod
    def setdispbackground(show: bool) -> None:
        _send("nes.setdispbackground", [show])
        _recv()

    @staticmethod
    def setdispsprites(show: bool) -> None:
        _send("nes.setdispsprites", [show])
        _recv()

    @staticmethod
    def setscanlines(top: int, bottom: int, pal: bool = False) -> None:
        _send("nes.setscanlines", [top, bottom, pal])
        _recv()


nes = NES


class SaveState:
    @staticmethod
    def load(path: str, suppressosd: bool = False) -> None:
        """Return type changed from bool to void due to implementation constraints that make returning a value impractical."""
        _send("savestate.load", [path, suppressosd])
        _recv()

    @staticmethod
    def loadslot(slotnum: int, suppressosd: bool = False) -> None:
        """Return type changed from bool to void due to implementation constraints that make returning a value impractical."""
        _send("savestate.loadslot", [slotnum, suppressosd])
        _recv()

    @staticmethod
    def save(path: str, suppressosd: bool = False) -> bool:
        _send("savestate.save", [path, suppressosd])
        return _deserialize_bool(_recv())

    @staticmethod
    def saveslot(slotnum: int, suppressosd: bool = False) -> bool:
        _send("savestate.saveslot", [slotnum, suppressosd])
        return _deserialize_bool(_recv())


savestate = SaveState


class SNES:
    @staticmethod
    def getlayer_bg_1() -> bool:
        _send("snes.getlayer_bg_1", [])
        return _deserialize_bool(_recv())

    @staticmethod
    def getlayer_bg_2() -> bool:
        _send("snes.getlayer_bg_2", [])
        return _deserialize_bool(_recv())

    @staticmethod
    def getlayer_bg_3() -> bool:
        _send("snes.getlayer_bg_3", [])
        return _deserialize_bool(_recv())

    @staticmethod
    def getlayer_bg_4() -> bool:
        _send("snes.getlayer_bg_4", [])
        return _deserialize_bool(_recv())

    @staticmethod
    def getlayer_obj_1() -> bool:
        _send("snes.getlayer_obj_1", [])
        return _deserialize_bool(_recv())

    @staticmethod
    def getlayer_obj_2() -> bool:
        _send("snes.getlayer_obj_2", [])
        return _deserialize_bool(_recv())

    @staticmethod
    def getlayer_obj_3() -> bool:
        _send("snes.getlayer_obj_3", [])
        return _deserialize_bool(_recv())

    @staticmethod
    def getlayer_obj_4() -> bool:
        _send("snes.getlayer_obj_4", [])
        return _deserialize_bool(_recv())

    @staticmethod
    def setlayer_bg_1(value: bool) -> None:
        _send("snes.setlayer_bg_1", [value])
        _recv()

    @staticmethod
    def setlayer_bg_2(value: bool) -> None:
        _send("snes.setlayer_bg_2", [value])
        _recv()

    @staticmethod
    def setlayer_bg_3(value: bool) -> None:
        _send("snes.setlayer_bg_3", [value])
        _recv()

    @staticmethod
    def setlayer_bg_4(value: bool) -> None:
        _send("snes.setlayer_bg_4", [value])
        _recv()

    @staticmethod
    def setlayer_obj_1(value: bool) -> None:
        _send("snes.setlayer_obj_1", [value])
        _recv()

    @staticmethod
    def setlayer_obj_2(value: bool) -> None:
        _send("snes.setlayer_obj_2", [value])
        _recv()

    @staticmethod
    def setlayer_obj_3(value: bool) -> None:
        _send("snes.setlayer_obj_3", [value])
        _recv()

    @staticmethod
    def setlayer_obj_4(value: bool) -> None:
        _send("snes.setlayer_obj_4", [value])
        _recv()


snes = SNES


# SQL

# TASStudio


class UserData:
    @staticmethod
    def clear() -> None:
        _send("userdata.clear", [])
        _recv()

    @staticmethod
    def containskey(key: str) -> bool:
        _send("userdata.containskey", [key])
        return _deserialize_bool(_recv())

    @staticmethod
    def get(key: str) -> object:
        """objectはjsonでconvert"""
        _send("userdata.get", [key])
        return _deserialize_object(_recv())

    @staticmethod
    def get_keys() -> list[str]:
        _send("userdata.get_keys", [])
        return _deserialize_list(_recv())

    @staticmethod
    def remove(key: str) -> bool:
        _send("userdata.remove", [key])
        return _deserialize_bool(_recv())

    @staticmethod
    def set(name: str, value: object) -> None:
        """objectはjsonでconvert"""
        _send("userdata.set", [name, _serialize_object(value)])
        _recv()


userdata = UserData


# ================================
# Utility
# ================================
class Utils:
    @staticmethod
    def set_key(key: str, *, frameadvance: bool = False, keep: bool = False) -> None:
        jkeys = joypad.get()
        if not keep:
            for k in jkeys.keys():
                if isinstance(jkeys[k], bool):
                    jkeys[k] = False
        if key in jkeys:
            if isinstance(jkeys[key], bool):
                jkeys[key] = True
        joypad.set(jkeys)
        if frameadvance:
            emu.frameadvance()

    @staticmethod
    def set_keys(keys: list[str], *, frameadvance: bool = False, keep: bool = False) -> None:
        jkeys = joypad.get()
        if not keep:
            for k in jkeys.keys():
                if isinstance(jkeys[k], bool):
                    jkeys[k] = False
        for k in keys:
            if k in jkeys:
                if isinstance(jkeys[k], bool):
                    jkeys[k] = True
        joypad.set(jkeys)
        if frameadvance:
            emu.frameadvance()
