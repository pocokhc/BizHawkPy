using BizHawk.Client.Common;
using System.Collections.Generic;

namespace BizHawkPy.BizhawkApi;

internal static class BizhawkApi
{
    internal delegate void Handler(
        ApiContainer apis,
        PyBridge bridge,
        string[] args
    );


    public static Dictionary<string, Handler> Create(MainConsole logger)
    {

        var dict = new Dictionary<string, Handler>();

        void AddRange(Dictionary<string, Handler> src)
        {
            foreach (var pair in src)
            {
                dict[pair.Key] = pair.Value;
            }
        }

        // bit : python内で完結できるのでskip
        // bizstring : pythonで内で完結できるのでskip
        AddRange(Client.Create(logger));
        AddRange(Comm.Create(logger));
        AddRange(Console.Create(logger));
        AddRange(Emu.Create(logger));
        AddRange(Event.Create(logger));
        // forms  : TODO
        AddRange(GameInfo.Create(logger));
        // genesis: not implemented?
        AddRange(Gui.Create(logger));
        // input  : ユーザ操作は一旦保留
        AddRange(JoyPad.Create(logger));
        // LuaCanvas
        // MainMemory : memoryに統合
        AddRange(Memory.Create(logger));
        AddRange(MemorySaveState.Create(logger));
        AddRange(Movie.Create(logger));
        AddRange(NDS.Create(logger));
        AddRange(Nes.Create(logger));
        AddRange(SaveState.Create(logger));
        AddRange(SNes.Create(logger));
        // SQL : TODO
        // TASStudio
        AddRange(UserData.Create(logger));

        return dict;

    }
}