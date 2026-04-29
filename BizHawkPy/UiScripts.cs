using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace BizHawkPy;

internal sealed class UiScripts : UserControl
{
    private readonly MainConsole _top;

    private ListView _listView = new()
    {
        Dock = DockStyle.Fill,
        View = View.Details,
        FullRowSelect = true,
        MultiSelect = true,
        HideSelection = false,
    };

    // 右クリック
    private ToolStripMenuItem _menuStripStart = new("Start");
    private ToolStripMenuItem _menuStripStop = new("Stop");
    private ToolStripMenuItem _menuStripReStart = new("ReStart");
    private ToolStripMenuItem _menuStripPause = new("Pause");
    private ToolStripMenuItem _menuStripResume = new("Resume");
    private ToolStripMenuItem _menuStripRemove = new("Remove");
    private ToolStripMenuItem _menuStripEnable = new("Enable");
    private ToolStripMenuItem _menuStripDisable = new("Disable");

    // icon
    private readonly ImageList _imageList = new()
    {
        ImageSize = new Size(16, 16),
        ColorDepth = ColorDepth.Depth32Bit
    };


    // scripts
    public ListView ScriptsView => _listView;
    private readonly List<PyBridge> _scripts = new();
    public IReadOnlyList<PyBridge> Scripts => _scripts;

    public UiScripts(MainConsole top)
    {
        _top = top;

        // コマンド
        _menuStripStart.Click += (_, _) => StartSelected();
        _menuStripStop.Click += (_, _) => StopSelected();
        _menuStripReStart.Click += (_, _) => { StopSelected(); StartSelected(); };
        _menuStripPause.Click += (_, _) => SetPauseSelected(true);
        _menuStripResume.Click += (_, _) => SetPauseSelected(false);
        _menuStripRemove.Click += (_, _) => RemoveSelected();
        _menuStripEnable.Click += (_, _) => SetEnabledSelected(true);
        _menuStripDisable.Click += (_, _) => SetEnabledSelected(false);

        // iconの登録
        _imageList.Images.Add("run", CreatePlayIcon(Color.Green));
        _imageList.Images.Add("stop", CreateCircle(Color.Red));
        _imageList.Images.Add("pause", CreatePauseIcon(Color.Goldenrod));
        _imageList.Images.Add("disabled", CreateCircle(Color.Gray));
        _listView.SmallImageList = _imageList;

        // UI
        Dock = DockStyle.Fill;
        AutoSize = true;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;

        // --- スクリプト一覧
        _listView.Columns.Add("", 30);
        _listView.Columns.Add("Script", 150);
        _listView.Columns.Add("Path", -2);

        // --- 右クリック
        var menu = new ContextMenuStrip();
        menu.Items.Add(_menuStripStart);
        menu.Items.Add(_menuStripStop);
        menu.Items.Add(_menuStripReStart);
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add(_menuStripEnable);
        menu.Items.Add(_menuStripDisable);
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add(_menuStripPause);
        menu.Items.Add(_menuStripResume);
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add("Insert Seperator", null, (_, _) => InsertSeparator());
        menu.Items.Add(_menuStripRemove);
        menu.Opening += _OnMenuOpening;
        _listView.ContextMenuStrip = menu;

        // 右クリックした行を選択状態にする
        _listView.MouseDown += (sender, e) =>
        {
            if (e.Button != MouseButtons.Right) return;

            var hit = _listView.HitTest(e.Location);
            if (hit.Item != null && !hit.Item.Selected)
            {
                _listView.SelectedItems.Clear();
                hit.Item.Selected = true;
            }
        };

        Controls.Add(_listView);

    }

    private Image CreateCircle(Color color)
    {
        var bmp = new Bitmap(16, 16);
        using var g = Graphics.FromImage(bmp);
        using var brush = new SolidBrush(color);
        g.FillEllipse(brush, 2, 2, 12, 12);
        return bmp;
    }
    private Image CreatePlayIcon(Color color)
    {
        var bmp = new Bitmap(16, 16);
        using var g = Graphics.FromImage(bmp);
        using var brush = new SolidBrush(color);

        // ▶ 三角形
        var points = new[]
        {
            new Point(4, 3),
            new Point(12, 8),
            new Point(4, 13)
        };
        g.FillPolygon(brush, points);

        return bmp;
    }
    private Image CreatePauseIcon(Color color)
    {
        var bmp = new Bitmap(16, 16);
        using var g = Graphics.FromImage(bmp);
        using var brush = new SolidBrush(color);

        // 左右2本バー
        g.FillRectangle(brush, 3, 2, 3, 12);
        g.FillRectangle(brush, 10, 2, 3, 12);

        return bmp;
    }

    private void _OnMenuOpening(object sender, EventArgs e)
    {
        var selected = GetSelectedItems();
        bool hasSelection = selected.Count > 0;

        _menuStripStart.Enabled = hasSelection;
        _menuStripStop.Enabled = hasSelection;
        _menuStripReStart.Enabled = hasSelection;
        _menuStripPause.Enabled = hasSelection;
        _menuStripResume.Enabled = hasSelection;
        _menuStripRemove.Enabled = hasSelection;

        // Enable/Disable の切替制御
        _menuStripEnable.Enabled = selected.Any(x => !x.IsEnabled);
        _menuStripDisable.Enabled = selected.Any(x => x.IsEnabled);

    }

    public void UpdateItemUI(PyBridge data)
    {
        // --- StartOnLaunch
        data.Item.Text = data.StartOnLaunch ? "▶" : "";

        // --- Enabled
        if (!data.IsEnabled)
        {
            data.Item.ImageKey = "disabled";
            //data.Item.BackColor = Color.FromArgb(230, 230, 230);
            data.Item.ForeColor = Color.Gray;
            return;
        }
        data.Item.ForeColor = Color.Black;

        // --- Pause
        if (data.IsPause() && data.IsRunning())
        {
            data.Item.ImageKey = "pause";
            data.Item.BackColor = Color.FromArgb(255, 255, 200);
        }
        else if (data.IsRunning())
        {
            data.Item.ImageKey = "run";
            data.Item.BackColor = Color.FromArgb(200, 255, 200);
        }
        else
        {
            data.Item.ImageKey = "stop";
            data.Item.BackColor = Color.White;
        }
    }

    public void InsertSeparator()
    {
        // 選択がある場合はその下、なければ最後
        int index = _listView.SelectedIndices.Count > 0
            ? _listView.SelectedIndices[0] + 1
            : _listView.Items.Count;

        var item = new ListViewItem("");
        item.SubItems.Add("───────────");
        item.SubItems.Add("");

        // セパレータ識別用
        item.Tag = null;

        _listView.Items.Insert(index, item);
        _top.uiMenu.MarkSessionUnsaved();
    }

    public void MoveSelectedUp()
    {
        var items = _listView.SelectedItems
            .Cast<ListViewItem>()
            .OrderBy(i => i.Index)
            .ToList();

        // 先頭が含まれていたら何もしない
        if (items.Count == 0 || items[0].Index == 0)
            return;

        _listView.BeginUpdate();
        foreach (var item in items)
        {
            int index = item.Index;
            if (index == 0) continue;

            _listView.Items.RemoveAt(index);
            _listView.Items.Insert(index - 1, item);
        }
        _listView.EndUpdate();
        _top.uiMenu.MarkSessionUnsaved();
    }
    public void MoveSelectedDown()
    {
        var items = _listView.SelectedItems
            .Cast<ListViewItem>()
            .OrderByDescending(i => i.Index)
            .ToList();

        if (items.Count == 0 || items[0].Index == _listView.Items.Count - 1)
            return;

        _listView.BeginUpdate();
        foreach (var item in items)
        {
            int index = item.Index;
            if (index >= _listView.Items.Count - 1) continue;

            _listView.Items.RemoveAt(index);
            _listView.Items.Insert(index + 1, item);
        }
        _listView.EndUpdate();
        _top.uiMenu.MarkSessionUnsaved();
    }

    public void AddScript(string script, string path, bool isEnabled, bool StartOnLaunch, string PythonArgs)
    {
        var item = new ListViewItem
        {
            ImageKey = "stop",
        };
        var data = new PyBridge(
            _top,
            script,
            path,
            isEnabled,
            StartOnLaunch,
            PythonArgs,
            item
        );
        _scripts.Add(data);

        item.Tag = data;
        item.SubItems.Add(data.Name);
        item.SubItems.Add(data.Path);

        UpdateItemUI(data);
        _listView.Items.Add(item);
        _top.uiMenu.MarkSessionUnsaved();

        if (StartOnLaunch && isEnabled)
        {
            data.Start();
            UpdateItemUI(data);
        }
    }
    public List<PyBridge> GetSelectedItems()
    {
        var result = new List<PyBridge>();
        foreach (ListViewItem item in _listView.SelectedItems)
        {
            if (item.Tag is PyBridge data)
                result.Add(data);
        }
        return result;
    }
    public void ClearScripts()
    {
        var old_size = _scripts.Count;

        foreach (ListViewItem item in _listView.Items)
        {
            if (item.Tag is not PyBridge data) continue;

            data.Stop();
            _scripts.Remove(data);
        }
        _scripts.Clear();
        _listView.Items.Clear();

        if (old_size != _scripts.Count)
        {
            _top.uiMenu.MarkSessionUnsaved();
        }
    }

    public void SetEnabledSelected(bool enabled)
    {
        foreach (var data in GetSelectedItems())
        {
            data.IsEnabled = enabled;

            // 無効化時は強制停止
            if (!enabled)
            {
                data.Stop();
            }
            UpdateItemUI(data);
        }

        _top.uiMenu.MarkSessionUnsaved();
    }
    public void SetStartOnLaunchSelected(bool value)
    {
        foreach (var data in GetSelectedItems())
        {
            data.StartOnLaunch = value;
            UpdateItemUI(data);
        }

        _top.uiMenu.MarkSessionUnsaved();
    }

    public void StartSelected()
    {
        foreach (var data in GetSelectedItems())
        {
            data.Start();
            UpdateItemUI(data);
        }
    }

    public void StopSelected()
    {
        foreach (var data in GetSelectedItems())
        {
            data.Stop();
            UpdateItemUI(data);
        }
    }

    public void RestartSelected()
    {
        var items = GetSelectedItems();

        foreach (var data in items)
        {
            data.Stop();
        }

        foreach (var data in items)
        {
            data.Start();
            UpdateItemUI(data);
        }
    }

    public void SetPauseSelected(bool isPause)
    {
        foreach (var data in GetSelectedItems())
        {
            data.SetPause(isPause);
            UpdateItemUI(data);
        }
    }

    public void RemoveSelected()
    {
        if (_listView.SelectedItems.Count == 0)
            return;

        var items = _listView.SelectedItems
            .Cast<ListViewItem>()
            .OrderByDescending(i => i.Index)
            .ToList();

        foreach (var item in items)
        {
            if (item.Tag is PyBridge data)
            {
                data.Stop();
                _scripts.Remove(data);
            }
            _listView.Items.Remove(item);
        }
        _top.uiMenu.MarkSessionUnsaved();
    }


    public void StopAll()
    {
        foreach (var data in _scripts)
        {
            data.Stop();  // stopの描画はfreezeの元なのでstop内に任せる
        }
    }
}