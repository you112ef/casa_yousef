@echo off
echo Sky CASA Application with Login
echo =============================

REM Compile the login verifier
echo Compiling login verifier...
%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\csc.exe /target:library /out:LoginVerifier.dll /reference:System.Data.SQLite.dll /reference:System.Windows.Forms.dll /reference:System.Drawing.dll LoginVerifier.cs

if errorlevel 1 (
    echo ERROR: Failed to compile login verifier
    pause
    exit /b 1
)

REM Create a wrapper executable that includes login verification
echo Creating application wrapper...
(
echo using System;
echo using System.Diagnostics;
echo using System.Windows.Forms;
echo using System.Reflection;
echo.
echo class SkyCASALauncher
echo {
echo     [STAThread]
echo     static void Main^(^)
echo     {
echo         try
echo         {
echo             // Load the login verifier
echo             Assembly loginAssembly = Assembly.LoadFrom^("LoginVerifier.dll"^);
echo             Type loginVerifierType = loginAssembly.GetType^("LoginVerifier"^);
echo             MethodInfo verifyMethod = loginVerifierType.GetMethod^("VerifyLogin"^);
echo.
echo             // Verify login
echo             bool loginSuccess = ^(bool^)verifyMethod.Invoke^(null, new object[] { "database.db" }^);
echo.
echo             if ^(loginSuccess^)
echo             {
echo                 // Start the main application
echo                 ProcessStartInfo startInfo = new ProcessStartInfo^(^);
echo                 startInfo.FileName = "Sky_CASA.exe";
echo                 startInfo.UseShellExecute = true;
echo                 Process.Start^(startInfo^);
echo             }
echo             else
echo             {
echo                 Console.WriteLine^("Login cancelled."^);
echo             }
echo         }
echo         catch ^(Exception ex^)
echo         {
echo             MessageBox.Show^("Error: " + ex.Message, "Launcher Error", MessageBoxButtons.OK, MessageBoxIcon.Error^);
echo         }
echo     }
echo }
) > SkyCASALauncher.cs

REM Compile the launcher
%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\csc.exe /target:winexe /out:Sky_CASA_With_Login.exe /reference:System.Windows.Forms.dll /reference:System.Drawing.dll /reference:System.dll SkyCASALauncher.cs

if errorlevel 1 (
    echo ERROR: Failed to compile launcher
    pause
    exit /b 1
)

echo.
echo SUCCESS: Application with login created!
echo Run Sky_CASA_With_Login.exe to start the application with login verification.
echo.

REM Clean up temporary files (keep LoginVerifier.cs for Git)
del SkyCASALauncher.cs
del LoginVerifier.dll

pause