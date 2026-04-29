using System.Collections.Generic;
namespace BizHawkPy.BizhawkApi;

internal static class SaveState
{
    public static Dictionary<string, BizhawkApi.Handler> Create(MainConsole logger)
    {
        return new()
        {
            ["savestate.load"] = (apis, bridge, args) =>
            {
                var path = Utils.Parse<string>(args, 0);
                var suppressOsd = Utils.Parse<bool?>(args, 1) ?? false;

                // 特殊処理
                bridge.crossingState = PyBridge.UpdateCrossingState.LoadSlot;
                bridge._top.InvokeOnMain(() =>
                {
                    apis.SaveState.Load(path, suppressOsd);
                });
            },

            ["savestate.loadslot"] = (apis, bridge, args) =>
            {
                var slot = Utils.Parse<int>(args, 0);
                var suppressOsd = Utils.Parse<bool?>(args, 1) ?? false;

                // 特殊処理
                bridge.crossingState = PyBridge.UpdateCrossingState.LoadSlot;
                bridge._top.InvokeOnMain(() =>
                {
                    apis.SaveState.LoadSlot(slot, suppressOsd);
                });
            },

            ["savestate.save"] = (apis, bridge, args) =>
            {
                var path = Utils.Parse<string>(args, 0);
                var suppressOsd = Utils.Parse<bool?>(args, 1) ?? false;
                apis.SaveState.Save(path, suppressOsd);
                bridge.CmdReturn(true, typeof(bool));  // void
            },

            ["savestate.saveslot"] = (apis, bridge, args) =>
            {
                var slot = Utils.Parse<int>(args, 0);
                var suppressOsd = Utils.Parse<bool?>(args, 1) ?? false;
                apis.SaveState.SaveSlot(slot, suppressOsd);
                bridge.CmdReturn(true, typeof(bool));  // void
            },

        };
    }
}