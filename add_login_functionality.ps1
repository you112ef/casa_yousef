# Add Login Functionality to Sky CASA Application
# This script creates a login verification that runs before the main application starts

Write-Host "Adding login functionality to Sky CASA application..." -ForegroundColor Green
Write-Host "====================================================" -ForegroundColor Green

try {
    # Create a login verification script that will be called before running the main application
    $loginScriptContent = @'
using System;
using System.Data.SQLite;
using System.Windows.Forms;

public class LoginVerifier
{
    public static bool VerifyLogin(string databasePath)
    {
        // Create a simple login form
        LoginForm loginForm = new LoginForm(databasePath);
        DialogResult result = loginForm.ShowDialog();
        return result == DialogResult.OK;
    }
}

public class LoginForm : Form
{
    private TextBox txtUsername;
    private TextBox txtPassword;
    private Button btnLogin;
    private Button btnCancel;
    private Label lblUsername;
    private Label lblPassword;
    private string databasePath;

    public LoginForm(string dbPath)
    {
        databasePath = dbPath;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.txtUsername = new TextBox();
        this.txtPassword = new TextBox();
        this.btnLogin = new Button();
        this.btnCancel = new Button();
        this.lblUsername = new Label();
        this.lblPassword = new Label();

        this.SuspendLayout();

        // lblUsername
        this.lblUsername.AutoSize = true;
        this.lblUsername.Location = new System.Drawing.Point(30, 30);
        this.lblUsername.Name = "lblUsername";
        this.lblUsername.Size = new System.Drawing.Size(58, 13);
        this.lblUsername.TabIndex = 0;
        this.lblUsername.Text = "Username:";

        // txtUsername
        this.txtUsername.Location = new System.Drawing.Point(30, 46);
        this.txtUsername.Name = "txtUsername";
        this.txtUsername.Size = new System.Drawing.Size(200, 20);
        this.txtUsername.TabIndex = 1;

        // lblPassword
        this.lblPassword.AutoSize = true;
        this.lblPassword.Location = new System.Drawing.Point(30, 80);
        this.lblPassword.Name = "lblPassword";
        this.lblPassword.Size = new System.Drawing.Size(56, 13);
        this.lblPassword.TabIndex = 2;
        this.lblPassword.Text = "Password:";

        // txtPassword
        this.txtPassword.Location = new System.Drawing.Point(30, 96);
        this.txtPassword.Name = "txtPassword";
        this.txtPassword.PasswordChar = '*';
        this.txtPassword.Size = new System.Drawing.Size(200, 20);
        this.txtPassword.TabIndex = 2;

        // btnLogin
        this.btnLogin.Location = new System.Drawing.Point(30, 130);
        this.btnLogin.Name = "btnLogin";
        this.btnLogin.Size = new System.Drawing.Size(75, 23);
        this.btnLogin.TabIndex = 3;
        this.btnLogin.Text = "Login";
        this.btnLogin.UseVisualStyleBackColor = true;
        this.btnLogin.Click += new System.EventHandler(this.BtnLogin_Click);

        // btnCancel
        this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        this.btnCancel.Location = new System.Drawing.Point(155, 130);
        this.btnCancel.Name = "btnCancel";
        this.btnCancel.Size = new System.Drawing.Size(75, 23);
        this.btnCancel.TabIndex = 4;
        this.btnCancel.Text = "Cancel";
        this.btnCancel.UseVisualStyleBackColor = true;
        this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);

        // LoginForm
        this.AcceptButton = this.btnLogin;
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.CancelButton = this.btnCancel;
        this.ClientSize = new System.Drawing.Size(284, 181);
        this.Controls.Add(this.btnCancel);
        this.Controls.Add(this.btnLogin);
        this.Controls.Add(this.txtPassword);
        this.Controls.Add(this.lblPassword);
        this.Controls.Add(this.txtUsername);
        this.Controls.Add(this.lblUsername);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Name = "LoginForm";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Text = "Sky CASA - Login";
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    private void BtnLogin_Click(object sender, EventArgs e)
    {
        string username = txtUsername.Text.Trim();
        string password = txtPassword.Text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            MessageBox.Show("Please enter both username and password.", "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (AuthenticateUser(username, password))
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        else
        {
            MessageBox.Show("Invalid username or password.", "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            txtPassword.Clear();
            txtPassword.Focus();
        }
    }

    private void BtnCancel_Click(object sender, EventArgs e)
    {
        this.DialogResult = DialogResult.Cancel;
        this.Close();
    }

    private bool AuthenticateUser(string username, string password)
    {
        try
        {
            string connectionString = "Data Source=" + databasePath + ";Version=3;";
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT COUNT(*) FROM admin WHERE username = @username AND password = @password";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", password);

                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count > 0;
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Database error: " + ex.Message, "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
    }
}
'@

    # Save the login script to a temporary file
    $loginScriptPath = "LoginVerifier.cs"
    $loginScriptContent | Out-File -FilePath $loginScriptPath -Encoding UTF8

    Write-Host "Created login verification component." -ForegroundColor Green

    # Create a new launcher script that includes login verification
    $launcherContent = @'
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

REM Clean up temporary files
del LoginVerifier.cs
del SkyCASALauncher.cs
del LoginVerifier.dll

pause
'@

    # Save the launcher script
    $launcherPath = "create_login_app.bat"
    $launcherContent | Out-File -FilePath $launcherPath -Encoding ASCII

    Write-Host "Created application launcher with login functionality." -ForegroundColor Green
    Write-Host "To create the login-enabled application, run: create_login_app.bat" -ForegroundColor Yellow
    Write-Host "The new executable will be named: Sky_CASA_With_Login.exe" -ForegroundColor Yellow

    # Also create a simple batch file to run the application with login
    $runWithLoginContent = @'
@echo off
echo Starting Sky CASA Application with Login Verification...
echo ======================================================

REM Check if the login-enabled version exists
if exist "Sky_CASA_With_Login.exe" (
    echo Running Sky_CASA_With_Login.exe...
    start "" "Sky_CASA_With_Login.exe"
) else (
    echo Login-enabled version not found. Creating it now...
    call create_login_app.bat
    if exist "Sky_CASA_With_Login.exe" (
        echo Running Sky_CASA_With_Login.exe...
        start "" "Sky_CASA_With_Login.exe"
    ) else (
        echo Failed to create login-enabled version.
        echo Running original application...
        start "" "Sky_CASA.exe"
    )
)

echo.
echo Application should now be running.
pause
'@

    $runWithLoginPath = "Run_Sky_CASA_With_Login.bat"
    $runWithLoginContent | Out-File -FilePath $runWithLoginPath -Encoding ASCII

    Write-Host "Created run script: Run_Sky_CASA_With_Login.bat" -ForegroundColor Green
    Write-Host "You can now run the application with login verification using this script." -ForegroundColor Green

    Write-Host "`n=== LOGIN FUNCTIONALITY ADDED SUCCESSFULLY ===" -ForegroundColor Green
    Write-Host "Default credentials:" -ForegroundColor Cyan
    Write-Host "  Username: admin" -ForegroundColor Cyan
    Write-Host "  Password: admin123" -ForegroundColor Cyan

} catch {
    Write-Host "ERROR: $($_.Exception.Message)" -ForegroundColor Red
}