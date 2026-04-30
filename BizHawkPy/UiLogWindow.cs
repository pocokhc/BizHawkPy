using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BizHawkPy;


internal sealed class UiLogWindowThread
{
    private Thread? _thread;
    private UiLogWindow? _window;
    private readonly ManualResetEventSlim _ready = new(false);

    public void Start(MainConsole top)
    {
        _ready.Reset();
        _thread = new Thread(() =>
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            _window = new UiLogWindow(top);
            _ready.Set();

            Application.Run(_window);
        });

        _thread.SetApartmentState(ApartmentState.STA);
        _thread.IsBackground = true;
        _thread.Start();
        _ready.Wait();
    }

    public void Stop()
    {
        if (_window == null || _window.IsDisposed) return;

        try
        {
            _window.Invoke(() =>
            {
                _window.SavePosition();
                _window.FormClosing -= _window.OnFormClosing;
                _window.Close();
            });
        }
        catch { }
    }

    public void Append(string message)
    {
        if (_window == null) return;

        if (_window.InvokeRequired)
        {
            _window.Invoke(() => _window.Append(message));
        }
        else
        {
            _window.Append(message);
        }
    }
    public void ClearLog()
    {
        if (_window == null) return;

        if (_window.InvokeRequired)
        {
            _window.Invoke(() => _window.ClearLog());
        }
        else
        {
            _window.ClearLog();
        }
    }

    public void Show()
    {
        if (_window == null || _window.IsDisposed) return;
        _window.Invoke(() => _window.Show());
    }

    public void Hide()
    {
        if (_window == null || _window.IsDisposed) return;
        _window.Invoke(() => _window.Hide());
    }

    public void RestoreWindowPosition()
    {
        if (_window == null || _window.IsDisposed) return;
        _window.Invoke(() => _window.RestoreWindowPosition());
    }
}

class UiLogWindow : Form
{
    private readonly MainConsole _top;

    private readonly TextBox _logBox = new()
    {
        Multiline = true,
        Dock = DockStyle.Fill,
        ScrollBars = ScrollBars.Vertical,
    };

    private readonly FlowLayoutPanel _buttonPanel = new()
    {
        Dock = DockStyle.Top,
        Height = 35,
        FlowDirection = FlowDirection.LeftToRight
    };


    public UiLogWindow(MainConsole top)
    {
        _top = top;

        Text = "BizHawkPy Log";
        Width = 800;
        Height = 300;

        Controls.Add(_logBox);
        Controls.Add(_buttonPanel);

        AddButton("Clear", ClearLog);

        AddSeparator();

        AddButton("Start", () => Task.Run(() => _top.uiScripts.StartSelected()));
        AddButton("Stop", () => Task.Run(() => _top.uiScripts.StopSelected()));
        AddButton("ReStart", () => Task.Run(() => _top.uiScripts.RestartSelected()));
        AddButton("StopAll", () => Task.Run(() => _top.uiScripts.StopAll()));

        AddSeparator();

        AddButton("Pause", () => _top.uiScripts.SetPauseSelected(true));
        AddButton("Resume", () => _top.uiScripts.SetPauseSelected(false));

        FormClosing += OnFormClosing;
    }
    public void RestoreWindowPosition()
    {
        StartPosition = FormStartPosition.Manual;
        WindowState = FormWindowState.Normal;
        SetBounds(
            _top.LogWindowX,
            _top.LogWindowY,
            _top.LogWindowWidth,
            _top.LogWindowHeight
        );
    }

    internal void OnFormClosing(object? sender, FormClosingEventArgs e)
    {
        SavePosition();

        // ユーザー操作による閉じるのみキャンセル
        if (e.CloseReason == CloseReason.UserClosing)
        {
            e.Cancel = true;
            Hide();
        }
    }

    public void SavePosition()
    {
        var bounds = WindowState == FormWindowState.Normal ? Bounds : RestoreBounds;
        _top.LogWindowX = bounds.X;
        _top.LogWindowY = bounds.Y;
        _top.LogWindowWidth = bounds.Width;
        _top.LogWindowHeight = bounds.Height;
    }

    public void Append(string message)
    {
        _logBox.AppendText(message + Environment.NewLine);
    }

    public void ClearLog()
    {
        _logBox.Clear();
    }


    // =========================
    // helper
    // =========================
    private void AddButton(string text, Action onClick)
    {
        var button = new Button
        {
            Text = text,
            Width = 60,
            Height = 25,
            Margin = new Padding(5)
        };

        button.Click += (_, _) => onClick();

        _buttonPanel.Controls.Add(button);
    }
    private void AddSeparator()
    {
        var separator = new Panel
        {
            Width = 1,
            Height = 25,
            Margin = new Padding(6, 3, 6, 3),
            BackColor = Color.Gray
        };

        _buttonPanel.Controls.Add(separator);
    }

}