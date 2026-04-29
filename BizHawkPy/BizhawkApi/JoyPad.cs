using System.Collections.Generic;

namespace BizHawkPy.BizhawkApi;

internal static class JoyPad
{
    public static Dictionary<string, BizhawkApi.Handler> Create(MainConsole logger)
    {
        return new()
        {
            ["joypad.get"] = (apis, bridge, args) =>
            {
                var controller = Utils.Parse<int?>(args, 0);
                var result = apis.Joypad.Get(controller);
                bridge.CmdReturn(result, typeof(Dictionary<string, object>));
            },

            ["joypad.getimmediate"] = (apis, bridge, args) =>
            {
                var controller = Utils.Parse<int?>(args, 0);
                var result = apis.Joypad.GetImmediate(controller);
                bridge.CmdReturn(result, typeof(Dictionary<string, object>));
            },

            ["joypad.getwithmovie"] = (apis, bridge, args) =>
            {
                var controller = Utils.Parse<int?>(args, 0);
                var result = apis.Joypad.GetWithMovie(controller);
                bridge.CmdReturn(result, typeof(Dictionary<string, object>));
            },

            ["joypad.set"] = (apis, bridge, args) =>
            {
                var buttons = Utils.Parse<IReadOnlyDictionary<string, bool>?>(args, 0);
                var controller = Utils.Parse<int?>(args, 1);
                apis.Joypad.Set(buttons, controller);
                bridge.CmdReturn(null, typeof(void));
            },

            ["joypad.setanalog"] = (apis, bridge, args) =>
            {
                var controls = Utils.Parse<IReadOnlyDictionary<string, int?>>(args, 0);
                var controller = Utils.Parse<int?>(args, 1);
                apis.Joypad.SetAnalog(controls, controller);
                bridge.CmdReturn(null, typeof(void));
            },

            ["joypad.setfrommnemonicstr"] = (apis, bridge, args) =>
            {
                var input = Utils.Parse<string>(args, 0);
                apis.Joypad.SetFromMnemonicStr(input);
                bridge.CmdReturn(null, typeof(void));
            },


        };
    }
}