using System;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;
using System.Runtime.InteropServices;

class Installer
{
    static int Main(string[] args)
    {
        try
        {
            Console.WriteLine("Sky CASA Installer\n====================\n");

            string programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            string installDir = Path.Combine(programFiles, "Sky CASA");
            string desktopDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory);
            string startMenuDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu), "Programs", "Sky CASA");

            Directory.CreateDirectory(installDir);
            Directory.CreateDirectory(startMenuDir);

            Console.WriteLine($"Installing to: {installDir}");

            // Extract embedded payload.zip
            using var stream = typeof(Installer).Assembly.GetManifestResourceStream("payload.zip");
            if (stream == null)
            {
                Console.Error.WriteLine("Payload not found in installer.");
                return 2;
            }
            string tempZip = Path.Combine(Path.GetTempPath(), "skycasa_payload.zip");
            using (var fs = File.Create(tempZip)) { stream.CopyTo(fs); }

            if (Directory.Exists(Path.Combine(installDir, "backup")) == false)
                Directory.CreateDirectory(Path.Combine(installDir, "backup"));

            // Extract
            ZipFile.ExtractToDirectory(tempZip, installDir, overwriteFiles: true);
            File.Delete(tempZip);

            string exePath = Path.Combine(installDir, "Sky_CASA.exe");
            if (!File.Exists(exePath))
            {
                Console.Error.WriteLine("Sky_CASA.exe not found in payload.");
                return 3;
            }

            // Create desktop and start menu shortcuts via PowerShell + WScript.Shell
            TryCreateShortcut(desktopDir, exePath, "Sky CASA.lnk");
            TryCreateShortcut(startMenuDir, exePath, "Sky CASA.lnk");

            // Create simple uninstaller
            var uninstaller = Path.Combine(installDir, "Uninstall_Sky_CASA.bat");
            File.WriteAllText(uninstaller, "@echo off\r\n" +
                "echo Uninstalling Sky CASA...\r\n" +
                "taskkill /IM Sky_CASA.exe /F >nul 2>&1\r\n" +
                $"rmdir /S /Q \"{installDir}\"\r\n" +
                "echo Done.\r\n" +
                "pause\r\n");

            Console.WriteLine("\nInstallation complete.");
            Console.WriteLine("A desktop and Start Menu shortcut have been created.");
            // Auto-launch the application without user input
            Process.Start(new ProcessStartInfo
            {
                FileName = exePath,
                WorkingDirectory = installDir,
                UseShellExecute = true
            });

            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("Installation failed: " + ex.Message);
            Console.Error.WriteLine(ex);
            return 1;
        }
    }

    static void TryCreateShortcut(string folder, string targetExe, string name)
    {
        try
        {
            Directory.CreateDirectory(folder);
            string shortcutPath = Path.Combine(folder, name);
            // Use PowerShell with WScript.Shell COM to create .lnk
            string ps = "$s=(New-Object -COM WScript.Shell).CreateShortcut(\"" + shortcutPath.Replace("\\", "\\\\") + "\");"+
                       "$s.TargetPath=\"" + targetExe.Replace("\\", "\\\\") + "\";"+
                       "$s.WorkingDirectory=\"" + Path.GetDirectoryName(targetExe)!.Replace("\\", "\\\\") + "\";"+
                       "$s.Save()";
            Process.Start(new ProcessStartInfo
            {
                FileName = "powershell",
                Arguments = "-NoProfile -ExecutionPolicy Bypass -Command \"" + ps + "\"",
                UseShellExecute = false,
                CreateNoWindow = true
            })?.WaitForExit(10000);
        }
        catch { }
    }
}
