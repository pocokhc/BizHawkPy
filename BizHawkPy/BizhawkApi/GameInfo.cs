using System.Collections.Generic;

namespace BizHawkPy.BizhawkApi;

internal static class GameInfo
{
    public static Dictionary<string, BizhawkApi.Handler> Create(MainConsole logger)
    {
        return new()
        {
            ["gameinfo.getboardtype"] = (apis, bridge, args) =>
            {
                var result = apis.Emulation.GetBoardName();
                bridge.CmdReturn(result, typeof(string));
            },

            ["gameinfo.getoptions"] = (apis, bridge, args) =>
            {
                var result = apis.Emulation.GetGameOptions();
                bridge.CmdReturn(result, typeof(Dictionary<string, string?>));
            },

            ["gameinfo.getromhash"] = (apis, bridge, args) =>
            {
                var result = apis.Emulation.GetGameInfo()?.Hash;
                bridge.CmdReturn(result, typeof(int));
            },

            ["gameinfo.getromname"] = (apis, bridge, args) =>
            {
                var result = apis.Emulation.GetGameInfo()?.Name;
                bridge.CmdReturn(result, typeof(string));
            },

            ["gameinfo.getstatus"] = (apis, bridge, args) =>
            {
                var result = apis.Emulation.GetGameInfo()?.Status;
                bridge.CmdReturn(result, typeof(string));
            },

            ["gameinfo.indatabase"] = (apis, bridge, args) =>
            {
                var result = !apis.Emulation.GetGameInfo()?.NotInDatabase;
                bridge.CmdReturn(result, typeof(bool));
            },

            ["gameinfo.isstatusbad"] = (apis, bridge, args) =>
            {
                var result = apis.Emulation.GetGameInfo()?.Status == BizHawk.Emulation.Common.RomStatus.BadDump;
                bridge.CmdReturn(result, typeof(bool));
            },

        };
    }
}