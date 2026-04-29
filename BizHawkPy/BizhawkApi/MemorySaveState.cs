using System.Collections.Generic;

namespace BizHawkPy.BizhawkApi;

internal static class MemorySaveState
{
    public static Dictionary<string, BizhawkApi.Handler> Create(MainConsole logger)
    {
        return new()
        {
            ["memorysavestate.clearstatesfrommemory"] = (apis, bridge, args) =>
            {
                apis.MemorySaveState?.ClearInMemoryStates();
                bridge.CmdReturn(null, typeof(void));
            },

            ["memorysavestate.loadcorestate"] = (apis, bridge, args) =>
            {
                var identifier = Utils.Parse<string>(args, 0);
                apis.MemorySaveState?.LoadCoreStateFromMemory(identifier);
                bridge.CmdReturn(null, typeof(void));
            },

            ["memorysavestate.removestate"] = (apis, bridge, args) =>
            {
                var identifier = Utils.Parse<string>(args, 0);
                apis.MemorySaveState?.DeleteState(identifier);
                bridge.CmdReturn(null, typeof(void));
            },

            ["memorysavestate.savecorestate"] = (apis, bridge, args) =>
            {
                var result = apis.MemorySaveState?.SaveCoreStateToMemory();
                bridge.CmdReturn(result, typeof(string));
            },
        };
    }
}