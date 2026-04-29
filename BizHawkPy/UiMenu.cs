using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace BizHawkPy;



internal sealed class UiMenu
{
    private readonly MenuStrip _menu = new();
    private readonly ToolStrip _toolbar = new();
    private readonly MainConsole _top;

    internal ToolStripMenuItem? StopOnException;

    public UiMenu(MainConsole top)
    {
        _top = top;

        // --- toolbar
        _toolbar.Dock = DockStyle.Top;
        _toolbar.ImageScalingSize = new Size(24, 24);
        _top.Controls.Add(_toolbar);

        AddButton("Open", OpenScript);

        _toolbar.Items.Add(new ToolStripSeparator());

        AddButton("Start", () => _top.uiScripts.StartSelected());
        AddButton("Stop", () => _top.uiScripts.StopSelected());
        AddButton("Restart", () => _top.uiScripts.RestartSelected());
        AddButton("StopAll", () => _top.uiScripts.StopAll());

        _toolbar.Items.Add(new ToolStripSeparator());

        AddButton("Pause", () => _top.uiScripts.SetPauseSelected(true));
        AddButton("Resume", () => _top.uiScripts.SetPauseSelected(false));

        _toolbar.Items.Add(new ToolStripSeparator());

        AddButton("Up", () => _top.uiScripts.MoveSelectedUp());
        AddButton("Down", () => _top.uiScripts.MoveSelectedDown());
        AddButton("Separator", () => _top.uiScripts.InsertSeparator());
        AddButton("Remove", () => _top.uiScripts.RemoveSelected());


        // --- menu
        _menu.Dock = DockStyle.Top;
        _top.Controls.Add(_menu);

        _menu.Items.Add(CreateFileMenu());
        _menu.Items.Add(CreateScriptMenu());
        _menu.Items.Add(CreateSettingstMenu());
        _menu.Items.Add(CreateHelpMenu());

    }

    private ToolStripMenuItem CreateFileMenu()
    {
        var menu = new ToolStripMenuItem("File");

        menu.DropDownItems.Add(Create("New Session", NewSession, Keys.Control | Keys.Shift | Keys.N));
        menu.DropDownItems.Add(Create("Open Session", OpenSession, Keys.Control | Keys.Shift | Keys.O));
        menu.DropDownItems.Add(Create("Save Session", SaveSession, Keys.Control | Keys.S));
        menu.DropDownItems.Add(Create("Save Session As...", SaveSessionAs, Keys.Control | Keys.Shift | Keys.S));

        menu.DropDownItems.Add(new ToolStripSeparator());

        menu.DropDownItems.Add(Create("New Script", NewScript, Keys.Control | Keys.N));
        menu.DropDownItems.Add(Create("Open Script", OpenScript, Keys.Control | Keys.O));

        return menu;
    }

    private ToolStripMenuItem CreateScriptMenu()
    {
        var menu = new ToolStripMenuItem("View");

        menu.DropDownItems.Add("Show Log Window", null, (_, _) => _top.uiLogWindow.Show());

        return menu;
    }
    private ToolStripMenuItem CreateSettingstMenu()
    {
        var menu = new ToolStripMenuItem("Settings");

        StopOnException = CreateToggle(
            "Stop on Exception",
            () => _top.StopOnException,
            v => _top.StopOnException = v
        );
        menu.DropDownItems.Add(StopOnException);

        // add IToolFormAutoConfig

        return menu;
    }


    // ------------------------------
    // command
    // ------------------------------
    private ToolStripMenuItem CreateHelpMenu()
    {
        var menu = new ToolStripMenuItem("Help");

        menu.DropDownItems.Add(Create("Lua Document online...", () =>
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://tasvideos.org/BizHawk/LuaFunctions",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                _top.Log($"Failed to open URL: {ex}");
            }
        }));

        menu.DropDownItems.Add(Create("Version", () =>
        {
            try
            {
                var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown";

                MessageBox.Show(
                    $"BizHawkPy\nVersion: {version}",
                    "Version Info",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                _top.Log($"Failed to get version: {ex}");
            }
        }));

        return menu;
    }


    // ------------------------------
    // script
    // ------------------------------
    private void NewScript()
    {
        using var dialog = new SaveFileDialog
        {
            Title = "New File",
            Filter = "Python Files (*.py)|*.py|All Files (*.*)|*.*",
            AddExtension = true,
            DefaultExt = "py",
            OverwritePrompt = true,
            FileName = "sample.py"
        };
        if (dialog.ShowDialog() != DialogResult.OK) return;

        var filePath = dialog.FileName;
        var template = _GetDefaultPythonTemplate();
        File.WriteAllText(filePath, template);
        _top.uiScripts.AddScript(System.IO.Path.GetFileNameWithoutExtension(filePath), filePath, true, false, "");

        // 既定のエディタで開く
        Process.Start(new ProcessStartInfo
        {
            FileName = filePath,
            UseShellExecute = true
        });
    }
    private static string _GetDefaultPythonTemplate()
    {
        return
    @"# How this script works:
# - Runs once when the script is started
# - emu.frameadvance() pauses and resumes after 1 frame
#
# Important:
# - You MUST call emu.frameadvance() when using an infinite loop
# - If not, this script will loop forever and freeze the emulator
#
# About import:
# - You can use the same API as Lua
# - Example: emu, joypad, memory, gui, etc.

from bizhawk_api import emu


def main() -> None:
    while True:
        # --- Code here runs once per frame ---
        print(f""frame: {emu.framecount()}"")

        emu.frameadvance()


if __name__ == ""__main__"":
    main()
";
    }

    private void OpenScript()
    {
        using var dialog = new OpenFileDialog
        {
            Title = "Open File",
            Filter = "Python Files (*.py)|*.py|All Files (*.*)|*.*",
            Multiselect = false,
            CheckFileExists = true,
            CheckPathExists = true
        };
        if (dialog.ShowDialog() != DialogResult.OK) return;

        var filePath = dialog.FileName;
        _top.uiScripts.AddScript(System.IO.Path.GetFileNameWithoutExtension(filePath), filePath, true, false, "");
    }

    // ------------------------------
    // session
    // ------------------------------
    public void MarkSessionUnsaved()
    {
        _top.isSessionSaved = false;
        _UpdateSessionItems();
    }
    private void MarkSessionSaved(string SessionPath)
    {
        _top.SessionPath = SessionPath;
        _top.isSessionSaved = true;
        _UpdateSessionItems();
    }
    private void _UpdateSessionItems()
    {
        var items = _top.GetScriptSessionItems();
        items.Clear();
        foreach (ListViewItem item in _top.uiScripts.ScriptsView.Items)
        {
            if (item.Tag is not PyBridge data)
            {
                // セパレータ
                items.Add(("", "", false, false, ""));
            }
            else
            {
                items.Add((data.Name, data.Path, data.IsEnabled, data.StartOnLaunch, data.PythonArgs));
            }
        }
        _top.SetScriptSessionItems(items);

        _top.UpdateTitle();
    }

    private void SaveSessionAs()
    {
        using var dialog = new SaveFileDialog
        {
            Title = "Save Session",
            Filter = "Session Files (*.json)|*.json|All Files (*.*)|*.*",
            AddExtension = true,
            DefaultExt = "json",
            OverwritePrompt = true
        };
        if (dialog.ShowDialog() != DialogResult.OK) return;
        _top.SessionPath = dialog.FileName;
        SaveSession();
    }


    private void SaveSession()
    {
        if (_top.SessionPath == "")
        {
            SaveSessionAs();
            return;
        }
        _UpdateSessionItems();

        File.WriteAllText(_top.SessionPath, _top.ScriptSessionItemsJson);
        MarkSessionSaved(_top.SessionPath);
    }

    private void OpenSession()
    {
        if (!_CheckUnsaveSession()) return;

        using var dialog = new OpenFileDialog
        {
            Title = "Load Session",
            Filter = "Session Files (*.json)|*.json|All Files (*.*)|*.*",
            Multiselect = false,
            CheckFileExists = true,
            CheckPathExists = true
        };
        if (dialog.ShowDialog() != DialogResult.OK) return;
        var path = dialog.FileName;

        // load json
        try
        {
            var json = File.ReadAllText(path);
            var items = _top.ConvertScriptSessionItems(json);
            NewSessionScripts(items);
            MarkSessionSaved(path);

        }
        catch (Exception ex)
        {
            _top.Log($"[SYSTEM][ERROR] Exception while loading session: path='{path}', type={ex.GetType().Name}, message={ex.Message}");
            return;
        }
    }

    public void NewSessionScripts(SessionItemsType items)
    {
        // 既存クリア
        _top.uiScripts.ClearScripts();

        // 再構築
        foreach (var (nameObj, pathObj, isEnabled, StartOnLanch, PythonArgs) in items.ToList())
        {
            if (nameObj is not string name || pathObj is not string path2)
                continue;

            if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(path2))
            {
                _top.uiScripts.InsertSeparator();
            }
            else if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(path2))
            {
                _top.uiScripts.AddScript(name, path2, isEnabled, StartOnLanch, PythonArgs);
            }
        }
    }

    private void NewSession()
    {
        if (!_CheckUnsaveSession()) return;

        _top.uiScripts.ClearScripts();
        MarkSessionSaved("");
    }

    private bool _CheckUnsaveSession()
    {
        if (!_top.isSessionSaved)
        {
            var result = MessageBox.Show(
            "Changes are not saved. Save now?",
            "Unsaved Session",
            MessageBoxButtons.YesNoCancel,
            MessageBoxIcon.Warning
        );

            if (result == DialogResult.Cancel)
            {
                // キャンセル → 何もしない
                return false;
            }

            if (result == DialogResult.Yes)
            {
                SaveSessionAs();

                // 保存ダイアログをキャンセルした場合など
                if (!_top.isSessionSaved)
                    return false;
            }
            // No → そのまま続行
        }
        return true;
    }

    // =========================
    // helper
    // =========================
    private ToolStripMenuItem Create(string text, Action action, Keys? shortcut = null)
    {
        var item = new ToolStripMenuItem(text, null, (_, _) => action());

        if (shortcut.HasValue)
            item.ShortcutKeys = shortcut.Value;

        return item;
    }
    private void AddButton(string text, Action onClick)
    {
        var btn = new ToolStripButton(text);
        btn.Click += (_, _) => onClick();
        _toolbar.Items.Add(btn);
    }
    private void AddButton(Image icon, string tipText, Action onClick)
    {
        var btn = new ToolStripButton
        {
            Image = icon,
            DisplayStyle = ToolStripItemDisplayStyle.Image,
            ToolTipText = tipText
        };

        btn.Click += (_, _) => onClick();
        _toolbar.Items.Add(btn);
    }

    private ToolStripMenuItem CreateToggle(string text, Func<bool> getter, Action<bool> setter)
    {
        var item = new ToolStripMenuItem(text)
        {
            Checked = getter()
        };

        item.Click += (_, _) =>
        {
            var newValue = !getter();
            setter(newValue);
            item.Checked = newValue;
        };

        return item;
    }
}