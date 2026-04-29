using System;
using System.Collections.Generic;

namespace BizHawkPy.BizhawkApi;

internal static class Emu
{
    public static Dictionary<string, BizhawkApi.Handler> Create(MainConsole logger)
    {
        return new()
        {
            ["emu.disassemble"] = (apis, bridge, args) =>
            {
                throw new NotImplementedException();
                //var pc = Parse<uint>(args, 0);
                //var name = Parse<string?>(args, 1);
                //var val = apis.Emulation.Disassemble(pc, name);
                //bridge.CmdReturn($"{SerializeDict(val)}");
            },
            ["emu.displayvsync"] = (apis, bridge, args) =>
            {
                var enabled = Utils.Parse<bool>(args, 0);
                apis.Emulation.DisplayVsync(enabled);
                bridge.CmdReturn("None", typeof(string));
            },
            ["emu.frameadvance"] = (apis, bridge, args) =>
            {
                // 特殊処理
                bridge.crossingState = PyBridge.UpdateCrossingState.Frameadvance;
            },
            ["emu.framecount"] = (apis, bridge, args) =>
            {
                var result = apis.Emulation.FrameCount();
                bridge.CmdReturn(result, typeof(int));
            },
            ["emu.getboardname"] = (apis, bridge, args) =>
            {
                var result = apis.Emulation.GetBoardName();
                bridge.CmdReturn(result, typeof(string));
            },
            ["emu.getdisplaytype"] = (apis, bridge, args) =>
            {
                var result = apis.Emulation.GetDisplayType();
                bridge.CmdReturn(result, typeof(string));
            },
            ["emu.getregister"] = (apis, bridge, args) =>
            {
                var name = Utils.Parse<string>(args, 0);
                var result = apis.Emulation.GetRegister(name);
                if (result == null) result = 0;
                bridge.CmdReturn(result, typeof(ulong));
            },
            ["emu.getregisters"] = (apis, bridge, args) =>
            {
                var result = apis.Emulation.GetRegisters();
                bridge.CmdReturn(result, typeof(Dictionary<string, ulong>));
            },
            ["emu.getsystemid"] = (apis, bridge, args) =>
            {
                var result = apis.Emulation.GetSystemId();
                bridge.CmdReturn(result, typeof(string));
            },
            ["emu.islagged"] = (apis, bridge, args) =>
            {
                var result = apis.Emulation.IsLagged();
                bridge.CmdReturn(result, typeof(bool));
            },
            ["emu.lagcount"] = (apis, bridge, args) =>
            {
                var result = apis.Emulation.LagCount();
                bridge.CmdReturn(result, typeof(int));
            },
            ["emu.limitframerate"] = (apis, bridge, args) =>
            {
                var enabled = Utils.Parse<bool>(args, 0);
                apis.Emulation.LimitFramerate(enabled);
                bridge.CmdReturn("null", typeof(string));
            },
            ["emu.minimizeframeskip"] = (apis, bridge, args) =>
            {
                var enabled = Utils.Parse<bool>(args, 0);
                apis.Emulation.MinimizeFrameskip(enabled);
                bridge.CmdReturn("null", typeof(string));
            },
            ["emu.setislagged"] = (apis, bridge, args) =>
            {
                var value = Utils.Parse<bool>(args, 0);
                apis.Emulation.SetIsLagged(value);
                bridge.CmdReturn("null", typeof(string));
            },
            ["emu.setlagcount"] = (apis, bridge, args) =>
            {
                var count = Utils.Parse<int>(args, 0);
                apis.Emulation.SetLagCount(count);
                bridge.CmdReturn("null", typeof(string));
            },
            ["emu.setregister"] = (apis, bridge, args) =>
            {
                var register = Utils.Parse<string>(args, 0);
                var value = Utils.Parse<int>(args, 1);
                apis.Emulation.SetRegister(register, value);
                bridge.CmdReturn("null", typeof(string));
            },
            ["emu.setrenderplanes"] = (apis, bridge, args) =>
            {
                var luaparam = Utils.Parse<bool[]>(args, 0);
                apis.Emulation.SetRenderPlanes(luaparam);
                bridge.CmdReturn("null", typeof(string));
            },
            ["emu.totalexecutedcycles"] = (apis, bridge, args) =>
            {
                var result = apis.Emulation.TotalExecutedCycles();
                bridge.CmdReturn(result, typeof(long));
            },
            ["emu.yield"] = (apis, bridge, args) =>
            {
                throw new NotImplementedException();
            },


        };
    }
}