using System;
using System.Collections.Generic;
using System.Drawing;

namespace BizHawkPy.BizhawkApi;

internal static class Client
{
    public static Dictionary<string, BizhawkApi.Handler> Create(MainConsole logger)
    {
        return new()
        {
            ["client.addcheat"] = (apis, bridge, args) =>
            {
                throw new NotImplementedException();
                //var result = apis.Tool.;  # 見つからない
                //bridge.CmdReturn("null", typeof(string));
            },
            ["client.borderheight"] = (apis, bridge, args) =>
            {
                var result = apis.EmuClient.BorderHeight();
                bridge.CmdReturn(result, typeof(int));
            },

            ["client.borderwidth"] = (apis, bridge, args) =>
            {
                var result = apis.EmuClient.BorderWidth();
                bridge.CmdReturn(result, typeof(int));
            },

            ["client.bufferheight"] = (apis, bridge, args) =>
            {
                var result = apis.EmuClient.BufferHeight();
                bridge.CmdReturn(result, typeof(int));
            },

            ["client.bufferwidth"] = (apis, bridge, args) =>
            {
                var result = apis.EmuClient.BufferWidth();
                bridge.CmdReturn(result, typeof(int));
            },
            ["client.clearautohold"] = (apis, bridge, args) =>
            {
                apis.EmuClient.ClearAutohold();
                bridge.CmdReturn(null, typeof(void));
            },
            ["client.closerom"] = (apis, bridge, args) =>
            {
                apis.EmuClient.CloseRom();
                bridge.CmdReturn(null, typeof(void));
            },
            ["client.createinstance"] = (apis, bridge, args) =>
            {
                throw new NotImplementedException();
                //var name = Utils.Parse<bool>(args, 0);
                //var result = apis.EmuClient.CreateInstance(name);
                //bridge.CmdReturn($"{Utils.SerializeDict(result)}");
            },

            ["client.displaymessages"] = (apis, bridge, args) =>
            {
                var value = Utils.Parse<bool>(args, 0);
                apis.EmuClient.DisplayMessages(value);
                bridge.CmdReturn(null, typeof(void));
            },

            ["client.enablerewind"] = (apis, bridge, args) =>
            {
                var enabled = Utils.Parse<bool>(args, 0);
                apis.EmuClient.EnableRewind(enabled);
                bridge.CmdReturn(null, typeof(void));
            },

            ["client.exactsleep"] = (apis, bridge, args) =>
            {
                throw new NotImplementedException();
                //var ms = Parse<int>(args, 0);
                //apis.EmuClient.ExactSleep(ms);
                //bridge.CmdReturn(null, typeof(void));
            },

            ["client.exit"] = (apis, bridge, args) =>
            {
                throw new NotImplementedException();
                //apis.EmuClient.Exit();
                //bridge.CmdReturn(null, typeof(void));
            },

            ["client.exitCode"] = (apis, bridge, args) =>
            {
                throw new NotImplementedException();
                //var code = Parse<int>(args, 0);
                //apis.EmuClient.ExitCode(code);
                //bridge.CmdReturn(null, typeof(void));
            },

            ["client.frameskip"] = (apis, bridge, args) =>
            {
                var num = Utils.Parse<int>(args, 0);
                apis.EmuClient.FrameSkip(num);
                bridge.CmdReturn(null, typeof(void));
            },

            ["client.get_approx_framerate"] = (apis, bridge, args) =>
            {
                var result = apis.EmuClient.GetApproxFramerate();
                bridge.CmdReturn(result, typeof(int));
            },

            ["client.get_lua_engine"] = (apis, bridge, args) =>
            {
                throw new NotImplementedException();
                //var result = apis.EmuClient.GetLuaEngine();
                //bridge.CmdReturn(result);
            },

            ["client.getavailabletools"] = (apis, bridge, args) =>
            {
                throw new NotImplementedException();
                //var tools = apis.EmuClient.AvailableTools();
                //bridge.CmdReturn($"{SerializeDict(tools)}");
            },

            ["client.getconfig"] = (apis, bridge, args) =>
            {
                throw new NotImplementedException();
                //var config = bridge._top.Config.GetAll();
                //bridge.CmdReturn($"{SerializeDict(config)}");
            },

            ["client.GetSoundOn"] = (apis, bridge, args) =>
            {
                var result = apis.EmuClient.GetSoundOn();
                bridge.CmdReturn(result, typeof(bool));
            },

            ["client.gettargetscanlineintensity"] = (apis, bridge, args) =>
            {
                var result = apis.EmuClient.GetTargetScanlineIntensity();
                bridge.CmdReturn(result, typeof(int));
            },

            ["client.gettool"] = (apis, bridge, args) =>
            {
                var name = Utils.Parse<string>(args, 0);
                var tool = apis.Tool.GetTool(name);

                if (tool == null)
                {
                    bridge.CmdReturn(null, typeof(void));
                    return;
                }

                var dict = new Dictionary<string, object?>
                {
                    ["name"] = name,
                    ["type"] = tool.GetType().Name
                };

                bridge.CmdReturn(dict, typeof(Dictionary<string, object?>));
            },

            ["client.getversion"] = (apis, bridge, args) =>
            {
                throw new NotImplementedException();
                //var result = apis.EmuClient.GetVersion();
                //bridge.CmdReturn(result);
            },

            ["client.getwindowsize"] = (apis, bridge, args) =>
            {
                var result = apis.EmuClient.GetWindowSize();
                bridge.CmdReturn(result, typeof(int));
            },

            ["client.invisibleemulation"] = (apis, bridge, args) =>
            {
                throw new NotImplementedException();
                //var invisible = Utils.Parse<bool>(args, 0);
                //apis.EmuClient.InvisibleEmulation(invisible);
                //bridge.CmdReturn(null, typeof(void));
            },

            ["client.ispaused"] = (apis, bridge, args) =>
            {
                var result = apis.EmuClient.IsPaused();
                bridge.CmdReturn(result, typeof(bool));
            },

            ["client.isseeking"] = (apis, bridge, args) =>
            {
                var result = apis.EmuClient.IsSeeking();
                bridge.CmdReturn(result, typeof(bool));
            },

            ["client.isturbo"] = (apis, bridge, args) =>
            {
                var result = apis.EmuClient.IsTurbo();
                bridge.CmdReturn(result, typeof(bool));
            },

            ["client.opencheats"] = (apis, bridge, args) =>
            {
                apis.Tool.OpenCheats();
                bridge.CmdReturn(null, typeof(void));
            },

            ["client.openhexeditor"] = (apis, bridge, args) =>
            {
                apis.Tool.OpenHexEditor();
                bridge.CmdReturn(null, typeof(void));
            },

            ["client.openramsearch"] = (apis, bridge, args) =>
            {
                apis.Tool.OpenRamSearch();
                bridge.CmdReturn(null, typeof(void));
            },

            ["client.openramwatch"] = (apis, bridge, args) =>
            {
                apis.Tool.OpenRamWatch();
                bridge.CmdReturn(null, typeof(void));
            },

            ["client.openrom"] = (apis, bridge, args) =>
            {
                var path = Utils.Parse<string>(args, 0);
                var result = apis.EmuClient.OpenRom(path);
                bridge.CmdReturn(result, typeof(bool));
            },

            ["client.opentasstudio"] = (apis, bridge, args) =>
            {
                apis.Tool.OpenTasStudio();
                bridge.CmdReturn(null, typeof(void));
            },

            ["client.opentoolbox"] = (apis, bridge, args) =>
            {
                apis.Tool.OpenToolBox();
                bridge.CmdReturn(null, typeof(void));
            },

            ["client.opentracelogger"] = (apis, bridge, args) =>
            {
                apis.Tool.OpenTraceLogger();
                bridge.CmdReturn(null, typeof(void));
            },

            ["client.pause"] = (apis, bridge, args) =>
            {
                apis.EmuClient.Pause();
                bridge.CmdReturn(null, typeof(void));
            },

            ["client.pause_av"] = (apis, bridge, args) =>
            {
                apis.EmuClient.PauseAv();
                bridge.CmdReturn(null, typeof(void));
            },

            ["client.reboot_core"] = (apis, bridge, args) =>
            {
                apis.EmuClient.RebootCore();
                bridge.CmdReturn("None", typeof(string));
            },

            ["client.removecheat"] = (apis, bridge, args) =>
            {
                throw new NotImplementedException();
                //var code = Parse<string>(args, 0);
                //apis.Tool.Remove(code);
                //bridge.CmdReturn(null, typeof(void));
            },

            ["client.saveram"] = (apis, bridge, args) =>
            {
                apis.EmuClient.SaveRam();
                bridge.CmdReturn(null, typeof(void));
            },

            ["client.screenheight"] = (apis, bridge, args) =>
            {
                var result = apis.EmuClient.ScreenHeight();
                bridge.CmdReturn(result, typeof(int));
            },

            ["client.screenshot"] = (apis, bridge, args) =>
            {
                var path = Utils.Parse<string?>(args, 0);
                apis.EmuClient.Screenshot(path);
                bridge.CmdReturn(null, typeof(void));
            },

            ["client.screenshottoclipboard"] = (apis, bridge, args) =>
            {
                apis.EmuClient.ScreenshotToClipboard();
                bridge.CmdReturn(null, typeof(void));
            },

            ["client.screenwidth"] = (apis, bridge, args) =>
            {
                var result = apis.EmuClient.ScreenWidth();
                bridge.CmdReturn(result, typeof(bool));
            },

            ["client.SetClientExtraPadding"] = (apis, bridge, args) =>
            {
                var l = Utils.Parse<int>(args, 0);
                var t = Utils.Parse<int>(args, 1);
                var r = Utils.Parse<int>(args, 2);
                var b = Utils.Parse<int>(args, 3);

                apis.EmuClient.SetClientExtraPadding(l, t, r, b);
                bridge.CmdReturn(null, typeof(void));
            },

            ["client.SetGameExtraPadding"] = (apis, bridge, args) =>
            {
                var l = Utils.Parse<int>(args, 0);
                var t = Utils.Parse<int>(args, 1);
                var r = Utils.Parse<int>(args, 2);
                var b = Utils.Parse<int>(args, 3);

                apis.EmuClient.SetGameExtraPadding(l, t, r, b);
                bridge.CmdReturn(null, typeof(void));
            },

            ["client.setscreenshotosd"] = (apis, bridge, args) =>
            {
                var value = Utils.Parse<bool>(args, 0);
                apis.EmuClient.SetScreenshotOSD(value);
                bridge.CmdReturn(null, typeof(void));
            },

            ["client.SetSoundOn"] = (apis, bridge, args) =>
            {
                var enable = Utils.Parse<bool>(args, 0);
                apis.EmuClient.SetSoundOn(enable);
                bridge.CmdReturn(null, typeof(void));
            },

            ["client.settargetscanlineintensity"] = (apis, bridge, args) =>
            {
                var val = Utils.Parse<int>(args, 0);
                apis.EmuClient.SetTargetScanlineIntensity(val);
                bridge.CmdReturn(null, typeof(void));
            },


            ["client.setwindowsize"] = (apis, bridge, args) =>
            {
                var size = Utils.Parse<int>(args, 0);
                apis.EmuClient.SetWindowSize(size);
                bridge.CmdReturn(null, typeof(void));
            },

            ["client.sleep"] = (apis, bridge, args) =>
            {
                throw new NotImplementedException();
                //var ms = Parse<int>(args, 0);
                //apis.EmuClient.Sleep(ms);
                //bridge.CmdReturn(null, typeof(void));
            },

            ["client.speedmode"] = (apis, bridge, args) =>
            {
                var percent = Utils.Parse<int>(args, 0);
                apis.EmuClient.SpeedMode(percent);
                bridge.CmdReturn(null, typeof(void));
            },
            ["client.togglepause"] = (apis, bridge, args) =>
            {
                apis.EmuClient.TogglePause();
                bridge.CmdReturn(null, typeof(void));
            },
            ["client.transformPoint"] = (apis, bridge, args) =>
            {
                var x = Utils.Parse<int>(args, 0);
                var y = Utils.Parse<int>(args, 1);
                var input = new Point(x, y);
                var result = apis.EmuClient.TransformPoint(input);
                var dict = new Dictionary<string, int>
                {
                    ["x"] = result.X,
                    ["y"] = result.Y
                };
                bridge.CmdReturn(dict, typeof(Dictionary<string, int>));
            },
            ["client.unpause"] = (apis, bridge, args) =>
            {
                apis.EmuClient.Unpause();
                bridge.CmdReturn("None", typeof(string));
            },
            ["client.unpause_av"] = (apis, bridge, args) =>
            {
                apis.EmuClient.UnpauseAv();
                bridge.CmdReturn(null, typeof(void));
            },
            ["client.xpos"] = (apis, bridge, args) =>
            {
                var result = apis.EmuClient.Xpos();
                bridge.CmdReturn(result, typeof(int));
            },
            ["client.ypos"] = (apis, bridge, args) =>
            {
                var result = apis.EmuClient.Ypos();
                bridge.CmdReturn(result, typeof(int));
            },
        };
    }
}