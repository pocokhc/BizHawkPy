using System.Collections.Generic;

namespace BizHawkPy.BizhawkApi;

internal static class UserData
{
    public static Dictionary<string, BizhawkApi.Handler> Create(MainConsole logger)
    {
        return new()
        {
            ["userdata.clear"] = (apis, bridge, args) =>
            {
                apis.UserData.Clear();
                bridge.CmdReturn(null, typeof(void));
            },

            ["userdata.containskey"] = (apis, bridge, args) =>
            {
                var key = Utils.Parse<string>(args, 0);
                var result = apis.UserData.ContainsKey(key);
                bridge.CmdReturn(result, typeof(bool));
            },

            ["userdata.get"] = (apis, bridge, args) =>
            {
                var key = Utils.Parse<string>(args, 0);
                var result = apis.UserData.Get(key);
                bridge.CmdReturn(result, typeof(string));
            },

            ["userdata.get_keys"] = (apis, bridge, args) =>
            {
                var result = apis.UserData.Keys;
                bridge.CmdReturn(result, typeof(List<string>));
            },

            ["userdata.remove"] = (apis, bridge, args) =>
            {
                var key = Utils.Parse<string>(args, 0);
                var result = apis.UserData.Remove(key);
                bridge.CmdReturn(result, typeof(bool));
            },

            ["userdata.set"] = (apis, bridge, args) =>
            {
                var name = Utils.Parse<string>(args, 0);
                var value = Utils.Parse<string>(args, 1);
                apis.UserData.Set(name, value);
                bridge.CmdReturn(null, typeof(void));
            },

        };
    }
}