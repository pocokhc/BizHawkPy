using BizHawk.Client.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
namespace BizHawkPy;



internal class PyBridge : IDisposable
{
    public string Name { get; set; }
    public string Path { get; set; }
    public bool IsEnabled { get; set; }
    public bool StartOnLaunch { get; set; }
    public string PythonArgs { get; set; }
    public ListViewItem Item { get; set; }

    private bool isPause { get; set; } = false;
    public bool IsPause() => isPause;

    internal enum UpdateCrossingState
    {
        None,
        Frameadvance,
        LoadSlot,
    }
    internal UpdateCrossingState crossingState { get; set; } = UpdateCrossingState.None;

    public APIState APIState { get; } = new();

    private enum ProcState
    {
        Stopped,
        Starting,
        Running,
        Stopping,
    }
    private int _state = (int)ProcState.Stopped;
    public bool IsRunning() => _state == (int)ProcState.Running;
    private Process? proc = null;
    private int _proc_id = 0;
    public int GetState() => _state;  // debug

    private readonly Dictionary<string, BizhawkApi.BizhawkApi.Handler> _emuapi_funcs;
    private readonly MainConsole logger;
    internal readonly MainConsole _top;

    public PyBridge(
        MainConsole top,
        string name,
        string path,
        bool isEnabled,
        bool startOnLaunch,
        string pythonArgs,
        ListViewItem item
    )
    {
        logger = top;
        _top = top;

        Name = name;
        Path = path;
        IsEnabled = isEnabled;
        StartOnLaunch = startOnLaunch;
        PythonArgs = pythonArgs;
        Item = item;

        _emuapi_funcs = BizhawkApi.BizhawkApi.Create(top);
    }

    public void Start()
    {
        if (Path == "") return;
        if (!IsEnabled) return;

        var state = Interlocked.CompareExchange(ref _state, (int)ProcState.Starting, (int)ProcState.Stopped);
        if (state != (int)ProcState.Stopped) return;

        try
        {
            _StartCore();
            Volatile.Write(ref _state, (int)ProcState.Running);
            Update();  // 初回実行
        }
        catch
        {
            Volatile.Write(ref _state, (int)ProcState.Stopped);
            throw;
        }
    }
    private void _StartCore()
    {
        var exeDir = AppDomain.CurrentDomain.BaseDirectory;

        // ----------------------
        // py起動確認
        // ----------------------
        var testPsi = new ProcessStartInfo
        {
            FileName = _top.uiPyInterpreter.PythonPath,
            Arguments = $"ExternalTools/BizHawkPy/bizhawk_main.py TEST",
            WorkingDirectory = exeDir,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        try
        {
            using var testProc = Process.Start(testPsi);
            if (testProc != null)
            {
                testProc.Kill();
                testProc.WaitForExit();
            }
            else
            {
                logger.Log("[SYSTEM] Failed to start. Python command may be incorrect or unavailable.");
                return;
            }
        }
        catch (Exception ex)
        {
            logger.Log($"[SYSTEM] Failed to start. Python command may be incorrect or unavailable: {ex.Message}");
            return;
        }

        // save path
        _top.uiPyInterpreter.PythonPath = _top.uiPyInterpreter.PythonPath;

        // ----------------------
        // 実行
        // ----------------------
        logger.Log($"[SYSTEM] Exec: \"{Path}\" \"{PythonArgs}\"");
        var args = new List<string>
        {
            "ExternalTools/BizHawkPy/bizhawk_main.py",
            $"\"{Convert.ToBase64String(Encoding.UTF8.GetBytes(Path))}\""
        };
        if (!string.IsNullOrEmpty(PythonArgs))
        {
            args.Add($"\"{Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(PythonArgs)))}\"");
        }
        var psi = new ProcessStartInfo
        {
            FileName = _top.uiPyInterpreter.PythonPath,
            Arguments = string.Join(" ", args),
            WorkingDirectory = exeDir,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            StandardOutputEncoding = System.Text.Encoding.UTF8,
            StandardErrorEncoding = System.Text.Encoding.UTF8,
        };
        proc = new Process { StartInfo = psi, EnableRaisingEvents = true };

        // ---------------------
        // 終了時
        // ---------------------
        proc.Exited += (s, e) =>
        {
            var state = Interlocked.CompareExchange(ref _state, (int)ProcState.Stopping, (int)ProcState.Running);
            if (state != (int)ProcState.Running) return;
            var p = Interlocked.Exchange(ref proc, null);
            logger.Log($"[SYSTEM][{Name}] Process exited. (PID: {_proc_id})");
            FinalizeProcess(p);
        };

        // エラー出力は非同期で常に表示
        // 標準出力はコマンド用に同期処理で取得（非同期と同期を混ぜるとエラー）
        proc.ErrorDataReceived += (s, e) =>
        {
            if (e.Data != null)
            {
                logger.Log($"[{Name}] {e.Data}");
            }
        };
        //proc.OutputDataReceived += (s, e) => { if (e.Data != null) logger.Log(e.Data); };

        isPause = false;
        crossingState = UpdateCrossingState.None;

        proc.Start();
        proc.BeginErrorReadLine();
        //proc.BeginOutputReadLine();

        logger.Log($"[SYSTEM][{Name}] Process started. (PID: {proc.Id})");
        _proc_id = proc.Id;
    }
    public void Dispose() { Stop(); }

    public void Stop()
    {
        var state = Interlocked.CompareExchange(ref _state, (int)ProcState.Stopping, (int)ProcState.Running);
        if (state != (int)ProcState.Running) return;

        var p = Interlocked.Exchange(ref proc, null);
        try
        {
            // --- 強制終了処理の概要
            // 1. py側処理 == stdout.ReadLine() でブロッキング中
            // 2. p.Kill()
            // 3. ReadLineのブロッキングが解除され、エラーが発生
            // 4. プロセス終了へ
            if (p != null)
            {
                if (!p.HasExited)
                {
                    logger.Log($"[SYSTEM] Closing process (PID: {p.Id})");
                    try { p.StandardInput?.Close(); } catch { }
                    if (!p.WaitForExit(1000))
                    {
                        p.Kill();
                        p.WaitForExit(5000);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            var pidText = p is null ? "null" : p.Id.ToString();
            logger.Log($"[SYSTEM] Error Closing process (PID: {pidText}): {ex.Message}");
        }
        finally
        {
            FinalizeProcess(p);
        }
    }
    private void FinalizeProcess(Process? p)
    {
        // ReleaseProcessResources
        if (p != null)
        {
            try { p.StandardInput?.Close(); } catch { }
            try { p.StandardOutput?.Close(); } catch { }
            try { p.Close(); } catch { }
            try { p.Dispose(); } catch { }
        }

        // OnStopped
        isPause = false;
        crossingState = UpdateCrossingState.None;
        UpdateItemOnUI();

        // change state
        Volatile.Write(ref _state, (int)ProcState.Stopped);
    }

    private void UpdateItemOnUI()
    {
        var form = (System.Windows.Forms.Form)_top;
        if (form.IsDisposed || !form.IsHandleCreated) return;

        Action action = () => _top.uiScripts.UpdateItemUI(this);

        try
        {
            if (form.InvokeRequired)
            {
                form.BeginInvoke(action);
            }
            else
            {
                action();
            }
        }
        catch (InvalidOperationException)
        {
            // フォーム破棄競合
        }
    }

    public void SetPause(bool pause)
    {
        if (Volatile.Read(ref _state) != (int)ProcState.Running) return;
        isPause = pause;
    }
    public void Update()
    {
        var p = proc;
        if (p == null) return;
        if (Volatile.Read(ref _state) != (int)ProcState.Running) return;

        if (isPause) return;

        System.IO.StreamReader stdout;
        try
        {
            stdout = p.StandardOutput;
        }
        catch (InvalidOperationException)
        {
            Stop();
            return;
        }
        if (stdout == null) return;

        const string prefix = "|C|";
        try
        {
            if (crossingState == UpdateCrossingState.Frameadvance)
            {
                crossingState = UpdateCrossingState.None;
                CmdReturn(null, typeof(void));
            }
            else if (crossingState == UpdateCrossingState.LoadSlot)
            {
                crossingState = UpdateCrossingState.None;
                CmdReturn(null, typeof(void));
            }

            // advanceframeが来るまでは無限ループ
            while (true)
            {
                // procが差し替わってないたら終了
                if (!ReferenceEquals(p, proc)) { return; }

                // データが来るまでブロッキング
                string? line = stdout.ReadLine();

                if (line == null) { return; }
                line = line.Trim();

                if (line.StartsWith(prefix))
                {
                    line = line.Substring(prefix.Length);
                    var parts = line.Split('|');
                    var cmd = parts[0];
                    var args = parts.Skip(1).ToArray();

                    if (_emuapi_funcs.TryGetValue(cmd, out var emuapi_func))
                    {
                        // debug
                        if (false)
                        {
#pragma warning disable CS0162 // 到達できないコードが検出されました
                            logger.Log("--------------------------");
                            logger.Log(cmd + ":" + string.Join(",", args));
                            logger.Log($"method name={emuapi_func.Method.Name}");
                            logger.Log($"declaring type={emuapi_func.Method.DeclaringType}");
                            logger.Log($"target instance={emuapi_func.Target}");
                            var method = emuapi_func.Method;
                            logger.Log($"return type={method.ReturnType}");
                            foreach (var p2 in method.GetParameters())
                            {
                                logger.Log($"param: {p2.ParameterType} {p2.Name}");
                            }
                            logger.Log("--------------------------");
#pragma warning restore CS0162 // 到達できないコードが検出されました
                        }

                        try
                        {
                            emuapi_func(_top.APIs, this, args);
                            if (crossingState != UpdateCrossingState.None)
                            {
                                break;
                            }
                        }
                        catch (NotImplementedException ex)
                        {
                            var type = ex.TargetSite?.DeclaringType?.FullName ?? "Unknown";
                            if (type.Contains("+<>c") && type.Contains("BizHawkPy"))
                            {
                                type = type.Replace("+<>c", "." + cmd);
                            }

                            CmdReturn("", typeof(string));
                            var argsStr = args?.Length > 0 ? string.Join(",", args) : "";
                            var errStr = $"[SYSTEM] NotImplemented Command: {cmd}({argsStr}) (from {type})";
                            if (_top.StopOnException)
                            {
                                throw new Exception(errStr);
                            }
                            else
                            {
                                logger.Log(errStr);
                            }
                        }
                        catch (Exception ex)
                        {
                            CmdReturn("", typeof(string));
                            var argsStr = args?.Length > 0 ? string.Join(",", args) : "";
                            var errStr = $"[SYSTEM] {cmd}({argsStr}) Error: {ex}";
                            if (_top.StopOnException)
                            {
                                throw new Exception(errStr);
                            }
                            else
                            {
                                logger.Log(errStr);
                            }
                        }
                    }
                    else
                    {
                        CmdReturn("", typeof(string));
                        var argsStr = args?.Length > 0 ? string.Join(",", args) : "";
                        var errStr = $"[SYSTEM] Unknown Command: {cmd}({argsStr})";
                        if (_top.StopOnException)
                        {
                            throw new Exception(errStr);
                        }
                        else
                        {
                            logger.Log(errStr);
                        }
                    }
                }
                else
                {
                    // cmd以外はそのまま出力
                    logger.Log($"[{Name}] {line}");
                }
            }
        }
        catch (InvalidOperationException)
        {
            // プロセスがすでにない場合の例外
            Stop();
            return;
        }
        catch (Exception ex)
        {
            logger.Log($"[SYSTEM] {ex}");
            Stop();
        }
    }

    internal void CmdReturn(object? msg, Type type)
    {
        msg = type switch
        {
            // --- void ---
            Type x when x == typeof(void)
                => string.Empty,

            // --- プリミティブ ---
            Type x when x == typeof(bool)
                => msg is bool b ? (b ? "1" : "0") : "0",

            Type x when x == typeof(int)
                || x == typeof(long)
                || x == typeof(short)
                || x == typeof(byte)
                || x == typeof(uint)
                || x == typeof(ulong)
                || x == typeof(ushort)
                => msg?.ToString() ?? "0",

            Type x when x == typeof(float)
                || x == typeof(double)
                || x == typeof(decimal)
                => msg?.ToString() ?? "0",

            // --- string ---
            Type x when x == typeof(string) => msg?.ToString() ?? string.Empty,
            //Type x when x == typeof(string)
            //    => Convert.ToBase64String(
            //        Encoding.UTF8.GetBytes(msg?.ToString() ?? string.Empty)
            //    ),

            // --- other ---
            _ => JsonConvert.SerializeObject(msg),
            //_ => Convert.ToBase64String(
            //        Encoding.UTF8.GetBytes(
            //            JsonConvert.SerializeObject(msg)
            //        )
            //    ),
        };

        var p = proc;
        if (p == null) return;
        p.StandardInput.WriteLine(msg);
        p.StandardInput.Flush();
    }

}

internal class APIState
{
    public BizHawk.Client.Common.DisplaySurfaceID surfaceID = DisplaySurfaceID.EmuCore;
}

