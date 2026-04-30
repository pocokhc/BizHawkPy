using System;
using System.Collections.Generic;

namespace BizHawkPy.BizhawkApi;

internal static class Event
{
    public static Dictionary<string, BizhawkApi.Handler> Create(MainConsole logger)
    {
        return new()
        {
            ["event.onexit"] = (apis, bridge, args) =>
            {
                throw new NotImplementedException();
                //var func = Parse<uint>(args, 0);
                //var name = Parse<string?>(args, 1);
                //var val = apis.Emulation.Disassemble(pc, name);
                //bridge.CmdReturn($"{SerializeDict(val)}");
            },
        };
    }
}