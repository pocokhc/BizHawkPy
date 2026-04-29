using System.Collections.Generic;

namespace BizHawkPy.BizhawkApi;

internal static class Movie
{
    public static Dictionary<string, BizhawkApi.Handler> Create(MainConsole logger)
    {
        return new()
        {
            ["movie.filename"] = (apis, bridge, args) =>
            {
                var result = apis.Movie.Filename();
                bridge.CmdReturn(result, typeof(string));
            },

            ["movie.getcomments"] = (apis, bridge, args) =>
            {
                var result = apis.Movie.GetComments();
                bridge.CmdReturn(result, typeof(Dictionary<string, string>));
            },

            ["movie.getfps"] = (apis, bridge, args) =>
            {
                var result = apis.Movie.GetFps();
                bridge.CmdReturn(result, typeof(double));
            },

            ["movie.getheader"] = (apis, bridge, args) =>
            {
                var result = apis.Movie.GetHeader();
                bridge.CmdReturn(result, typeof(Dictionary<string, string>));
            },

            ["movie.getinput"] = (apis, bridge, args) =>
            {
                var frame = Utils.Parse<int>(args, 0);
                var controller = Utils.Parse<int?>(args, 1);
                var result = apis.Movie.GetInput(frame, controller);
                bridge.CmdReturn(result, typeof(Dictionary<string, object>));
            },

            ["movie.getinputasmnemonic"] = (apis, bridge, args) =>
            {
                var frame = Utils.Parse<int>(args, 0);
                var result = apis.Movie.GetInputAsMnemonic(frame);
                bridge.CmdReturn(result, typeof(string));
            },

            ["movie.getreadonly"] = (apis, bridge, args) =>
            {
                var result = apis.Movie.GetReadOnly();
                bridge.CmdReturn(result, typeof(bool));
            },

            ["movie.getrerecordcount"] = (apis, bridge, args) =>
            {
                var result = apis.Movie.GetRerecordCount();
                bridge.CmdReturn(result, typeof(ulong));
            },

            ["movie.getrerecordcounting"] = (apis, bridge, args) =>
            {
                var result = apis.Movie.GetRerecordCounting();
                bridge.CmdReturn(result, typeof(bool));
            },

            ["movie.getsubtitles"] = (apis, bridge, args) =>
            {
                var result = apis.Movie.GetSubtitles();
                bridge.CmdReturn(result, typeof(Dictionary<string, string>));
            },

            ["movie.isloaded"] = (apis, bridge, args) =>
            {
                var result = apis.Movie.IsLoaded();
                bridge.CmdReturn(result, typeof(bool));
            },

            ["movie.length"] = (apis, bridge, args) =>
            {
                var result = apis.Movie.Length();
                bridge.CmdReturn(result, typeof(int));
            },

            ["movie.mode"] = (apis, bridge, args) =>
            {
                var result = apis.Movie.Mode();
                bridge.CmdReturn(result, typeof(string));
            },

            ["movie.play_from_start"] = (apis, bridge, args) =>
            {
                var path = Utils.Parse<string?>(args, 0);
                var result = apis.Movie.PlayFromStart(path);
                bridge.CmdReturn(result, typeof(bool));
            },

            ["movie.save"] = (apis, bridge, args) =>
            {
                var filename = Utils.Parse<string?>(args, 0);
                apis.Movie.Save(filename);
                bridge.CmdReturn(null, typeof(void));
            },

            ["movie.setreadonly"] = (apis, bridge, args) =>
            {
                var readOnly = Utils.Parse<bool>(args, 0);
                apis.Movie.SetReadOnly(readOnly);
                bridge.CmdReturn(null, typeof(void));
            },

            ["movie.setrerecordcount"] = (apis, bridge, args) =>
            {
                var count = Utils.Parse<ulong>(args, 0);
                apis.Movie.SetRerecordCount(count);
                bridge.CmdReturn(null, typeof(void));
            },

            ["movie.setrerecordcounting"] = (apis, bridge, args) =>
            {
                var counting = Utils.Parse<bool>(args, 0);
                apis.Movie.SetRerecordCounting(counting);
                bridge.CmdReturn(null, typeof(void));
            },

            ["movie.startsfromsaveram"] = (apis, bridge, args) =>
            {
                var result = apis.Movie.StartsFromSaveram();
                bridge.CmdReturn(result, typeof(bool));
            },

            ["movie.startsfromsavestate"] = (apis, bridge, args) =>
            {
                var result = apis.Movie.StartsFromSavestate();
                bridge.CmdReturn(result, typeof(bool));
            },

            ["movie.stop"] = (apis, bridge, args) =>
            {
                var saveChanges = Utils.Parse<bool?>(args, 0) ?? true;
                apis.Movie.Stop(saveChanges);
                bridge.CmdReturn(null, typeof(void));
            },
        };
    }
}