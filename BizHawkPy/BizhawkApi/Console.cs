using System;
using System.Collections.Generic;

namespace BizHawkPy.BizhawkApi;

internal static class Console
{
    public static Dictionary<string, BizhawkApi.Handler> Create(MainConsole logger)
    {
        return new()
        {
            ["console.clear"] = (apis, bridge, args) =>
            {
                bridge._top.uiLogWindow.ClearLog();
                bridge.CmdReturn("None", typeof(string));
            },
            ["console.log"] = (apis, bridge, args) =>
            {
                var text = Utils.Parse<string>(args, 0);
                bridge._top.uiLogWindow.Append(text);
                bridge.CmdReturn("None", typeof(string));
            },
            ["console.writeline"] = (apis, bridge, args) =>
            {
                var text = Utils.Parse<string>(args, 0);
                bridge._top.uiLogWindow.Append(text);
                bridge.CmdReturn("None", typeof(string));
            },
            ["console.write"] = (apis, bridge, args) =>
            {
                var text = Utils.Parse<string>(args, 0);
                bridge._top.uiLogWindow.Append(text);
                bridge.CmdReturn("None", typeof(string));
            },
            ["console.getluafunctionslist"] = (apis, bridge, args) =>
            {
                throw new NotImplementedException();
            },

        };
    }
}