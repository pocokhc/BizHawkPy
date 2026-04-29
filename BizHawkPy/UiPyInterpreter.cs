using System.Windows.Forms;

namespace BizHawkPy;

internal sealed class UiPyInterpreter : UserControl
{
    private readonly MainConsole _top;

    // UI
    private readonly TextBox _pathBox = new() { Dock = DockStyle.Fill };
    private readonly Button _browseButton = new() { Text = "...", Width = 30 };
    private readonly Label _label = new()
    {
        Text = "Python Command",
        Dock = DockStyle.Fill,
        TextAlign = System.Drawing.ContentAlignment.MiddleLeft
    };

    public string PythonPath
    {
        get => _pathBox.Text;
        set
        {
            _top.PythonPath = value;
            _pathBox.Text = value;
        }
    }


    // =====================================
    // コンストラクタ
    // =====================================
    public UiPyInterpreter(MainConsole top)
    {
        _top = top;

        InitializeLayout();

        _pathBox.Text = _top.PythonPath;
        _browseButton.Click += (_, _) => SelectPython();
    }

    private void InitializeLayout()
    {
        Dock = DockStyle.Top;
        AutoSize = true;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 3,
            RowCount = 1,
            Padding = Padding.Empty,
            Margin = Padding.Empty,
        };

        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 30));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        _label.Anchor = AnchorStyles.Left;
        _pathBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _browseButton.Anchor = AnchorStyles.Left;

        layout.Controls.Add(_label, 0, 0);
        layout.Controls.Add(_browseButton, 1, 0);
        layout.Controls.Add(_pathBox, 2, 0);

        Controls.Add(layout);
    }

    private void SelectPython()
    {
        using var dialog = new OpenFileDialog
        {
            Title = "Select Python Interpreter",
            Filter = "Python (*.exe)|python.exe;python3.exe|All files (*.*)|*.*",
            Multiselect = false,
            CheckFileExists = true
        };
        if (dialog.ShowDialog() != DialogResult.OK) return;

        PythonPath = dialog.FileName;
    }

}