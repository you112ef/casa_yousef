using System;
using System.Data.SQLite;
using System.Windows.Forms;

public class LoginVerifier
{
    public static bool VerifyLogin(string databasePath)
    {
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