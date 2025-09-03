using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

public static class AiRunner
{
    public static string ResolvePythonShim()
    {
        try
        {
            var baseDir = Application.StartupPath;
            string[] candidates =
            {
                Path.Combine(baseDir, "python.exe"),
                Path.Combine(baseDir, "ai_sperm_onnx", "python.exe")
            };
            foreach (var c in candidates)
            {
                if (File.Exists(c)) return c;
            }
            return "python"; // fallback to PATH
        }
        catch
        {
            return "python";
        }
    }

    public static async Task<(int ExitCode, string StdOut, string StdErr)> RunAsync(
        string analysisType, string mediaPath, int patientId, int durationSeconds = 0, int timeoutSeconds = 300)
    {
        var exe = ResolvePythonShim();
        var script = Path.Combine(Application.StartupPath, "ai_sperm_analysis", "analyze_media.py");

        string args = string.Empty;
        // Our ONNX shim ignores the .py argument if present, so keeping compatibility here is safe
        if (File.Exists(script))
        {
            args = $"\"{script}\" --type {analysisType} --media \"{mediaPath}\" --patient {patientId}";
        }
        else
        {
            // If script isn't present, still pass a dummy token; the shim will ignore it
            args = $"dummy.py --type {analysisType} --media \"{mediaPath}\" --patient {patientId}";
        }
        if (analysisType == "video" && durationSeconds > 0)
            args += $" --duration {durationSeconds}";

        var psi = new ProcessStartInfo
        {
            FileName = exe,
            Arguments = args,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            WorkingDirectory = Application.StartupPath
        };

        using (var proc = new Process { StartInfo = psi, EnableRaisingEvents = true })
        {
            proc.Start();
            var stdOutTask = proc.StandardOutput.ReadToEndAsync();
            var stdErrTask = proc.StandardError.ReadToEndAsync();

            if (!proc.WaitForExit(timeoutSeconds * 1000))
            {
                try { proc.Kill(); } catch { }
                return (-1, await stdOutTask, await stdErrTask + "\nTimeout");
            }
            var outTxt = await stdOutTask;
            var errTxt = await stdErrTask;
            return (proc.ExitCode, outTxt, errTxt);
        }
    }

    public static bool IsAiAvailable()
    {
        var exe = ResolvePythonShim();
        if (exe.Equals("python", StringComparison.OrdinalIgnoreCase))
        {
            // Not guaranteed, but indicate available if either python.exe is in PATH or shim found
            // We try a quick check: if PATH "python" fails to resolve, the call will fail later and be handled
            return true;
        }
        return File.Exists(exe);
    }
}
