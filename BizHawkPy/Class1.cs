// https://github.com/tasemulators/bizhawk
// https://github.com/TASEmulators/BizHawk-ExternalTools/wiki

global using SessionItemsType = System.Collections.Generic.List<(
    string Name,
    string Path,
    bool IsEnabled,
    bool StartOnLaunch,
    string PythonArgs
)>;
using BizHawk.Client.Common;
using BizHawk.Client.EmuHawk;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows.Forms;



namespace BizHawkPy;

[ExternalTool("Python Console")]
[ExternalToolEmbeddedIcon("BizHawkPy.icon.ico")]
public sealed class MainConsole : ToolFormBase, IExternalToolForm, IToolFormAutoConfig
{
    // API
    public ApiContainer? _maybeAPIContainer { get; set; }
    internal ApiContainer APIs => _maybeAPIContainer!;

    // title
    private string _title = "PythonConsole";
    protected override string WindowTitleStatic => _title;

    // Config
    private bool _initConfig = false;
    [ConfigPersist]
    public string PythonPath { get; set; } = "py.exe";
    [ConfigPersist]
    public string SessionPath { get; set; } = "";
    [ConfigPersist]
    public bool isSessionSaved { get; set; } = true;
    [ConfigPersist]
    public string ScriptSessionItemsJson { get; set; } = "";
    internal SessionItemsType GetScriptSessionItems() { return ConvertScriptSessionItems(ScriptSessionItemsJson); }
    internal SessionItemsType ConvertScriptSessionItems(string items_json)
    {
        if (string.IsNullOrWhiteSpace(items_json))
        {
            return new SessionItemsType();
        }

        try
        {
            return JsonConvert.DeserializeObject<SessionItemsType>(items_json)
                   ?? new SessionItemsType();
        }
        catch (JsonException)
        {
            // Why not: 呼び出し側で例外処理を強制しない
            return new SessionItemsType();
        }
    }
    internal void SetScriptSessionItems(SessionItemsType items)
    {
        if (items is null)
        {
            ScriptSessionItemsJson = "";
            return;
        }
        ScriptSessionItemsJson = JsonConvert.SerializeObject(items);
    }
    [ConfigPersist]
    public bool StopOnException { get; set; } = true;
    [ConfigPersist]
    public int LogWindowX { get; set; } = 0;
    [ConfigPersist]
    public int LogWindowY { get; set; } = 0;
    [ConfigPersist]
    public int LogWindowWidth { get; set; } = 800;
    [ConfigPersist]
    public int LogWindowHeight { get; set; } = 300;

    // UI
    internal readonly UiMenu uiMenu;
    internal readonly UiScripts uiScripts;
    internal readonly UiPyInterpreter uiPyInterpreter;
    internal readonly UiLogWindowThread uiLogWindow;

    public MainConsole()
    {
        ClientSize = new System.Drawing.Size(600, 200);

        uiLogWindow = new UiLogWindowThread();
        uiLogWindow.Start(this);

        uiScripts = new UiScripts(this);
        Controls.Add(uiScripts);

        uiPyInterpreter = new UiPyInterpreter(this);
        Controls.Add(uiPyInterpreter);

        uiMenu = new UiMenu(this);

        this.Icon = Properties.Resources.icon;
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        uiLogWindow.Stop();
        base.OnFormClosing(e);
    }

    // executed once after the constructor, and again every time a rom is loaded or reloaded
    public override void Restart()
    {
        if (!_initConfig)
        {
            _initConfig = true;

            // load config
            uiPyInterpreter.PythonPath = PythonPath;

            var conf_isSessionSaved = isSessionSaved;
            uiMenu.NewSessionScripts(GetScriptSessionItems());
            isSessionSaved = conf_isSessionSaved;

            if (uiMenu.StopOnException != null)
            {
                uiMenu.StopOnException.Checked = StopOnException;
            }

            // set window pos
            uiLogWindow.RestoreWindowPosition();

        }

        UpdateTitle();
    }

    internal void InvokeOnMain(Action func)
    {
        if (MainForm is not Form form)
            throw new InvalidOperationException("MainForm is not Form");

        form.BeginInvoke(new Action(() =>
        {
            try
            {
                func();
            }
            catch (Exception ex)
            {
                Log($"Invoke error: {ex}");
            }
        }));
    }


    // executed after every frame (except while turboing, use FastUpdateAfter for that)
    protected override void UpdateAfter()
    {
        foreach (var script in uiScripts.Scripts)
        {
            script.Update();
        }
    }

    protected override void FastUpdateAfter()
    {
        UpdateAfter();
    }

    public void UpdateTitle()
    {
        string? game = APIs.Emulation.GetGameInfo()?.Name;
        if ((game == null) || (game == "Null"))
        {
            _title = "PythonConsole - NoROM";
        }
        else
        {
            _title = $"PythonConsole - {game}";
        }

        if (SessionPath != "")
        {
            var s = isSessionSaved ? "" : "*";
            _title += $"  {Path.GetFileName(SessionPath)}{s}";
        }

        UpdateWindowTitle();
    }

    public void Log(string message)
    {
        uiLogWindow.Append(message);
    }

}
