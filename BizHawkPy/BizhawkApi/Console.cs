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
                bridge.CmdReturn(null, typeof(void));
            },
            ["console.log"] = (apis, bridge, args) =>
            {
                var text = Utils.Parse<string>(args, 0);
                bridge._top.uiLogWindow.Append(text);
                bridge.CmdReturn(null, typeof(void));
            },
            ["console.writeline"] = (apis, bridge, args) =>
            {
                var text = Utils.Parse<string>(args, 0);
                bridge._top.uiLogWindow.Append(text);
                bridge.CmdReturn(null, typeof(void));
            },
            ["console.write"] = (apis, bridge, args) =>
            {
                var text = Utils.Parse<string>(args, 0);
                bridge._top.uiLogWindow.Append(text);
                bridge.CmdReturn(null, typeof(void));
            },
            ["console.getluafunctionslist"] = (apis, bridge, args) =>
            {
                throw new NotImplementedException();
            },

        };
    }
}