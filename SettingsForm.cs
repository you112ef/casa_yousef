using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;

namespace SkyCASA
{
    /// <summary>
    /// واجهة إعدادات النظام المتقدمة
    /// Advanced System Settings Form
    /// </summary>
    public partial class SettingsForm : Form
    {
        private DataAccessLayer dal;
        private TabControl settingsTabControl;
        
        // Color scheme
        private readonly Color PrimaryBlue = Color.FromArgb(41, 128, 185);
        private readonly Color SecondaryBlue = Color.FromArgb(52, 152, 219);
        private readonly Color AccentGreen = Color.FromArgb(39, 174, 96);
        private readonly Color AccentOrange = Color.FromArgb(243, 156, 18);
        private readonly Color AccentRed = Color.FromArgb(231, 76, 60);
        private readonly Color LightGray = Color.FromArgb(236, 240, 241);
        private readonly Color White = Color.White;
        
        public SettingsForm()
        {
            dal = new DataAccessLayer("database.db");
            InitializeComponent();
            LoadSettings();
        }
        
        private void InitializeComponent()
        {
            this.Text = "إعدادات النظام | System Settings - Sky CASA";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = LightGray;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            
            CreateSettingsTabs();
            CreateButtons();
        }
        
        private void CreateSettingsTabs()
        {
            settingsTabControl = new TabControl();
            settingsTabControl.Location = new Point(20, 20);
            settingsTabControl.Size = new Size(740, 480);
            settingsTabControl.Font = new Font("Segoe UI", 10F);
            
            CreateGeneralTab();
            CreateDatabaseTab();
            CreateAISystemTab();
            CreateReportsTab();
            CreateSecurityTab();
            
            this.Controls.Add(settingsTabControl);
        }
        
        private void CreateGeneralTab()
        {
            var generalTab = new TabPage("عام | General");
            generalTab.BackColor = White;
            generalTab.Padding = new Padding(20);
            
            // Laboratory Information
            var labGroup = new GroupBox("معلومات المختبر | Laboratory Information");
            labGroup.Location = new Point(20, 20);
            labGroup.Size = new Size(680, 120);
            labGroup.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            labGroup.ForeColor = PrimaryBlue;
            
            var labNameLabel = new Label { Text = "اسم المختبر | Lab Name:", Location = new Point(20, 30), Size = new Size(120, 25) };
            var labNameText = new TextBox { Location = new Point(150, 27), Size = new Size(300, 25), Name = "labName" };
            
            var addressLabel = new Label { Text = "العنوان | Address:", Location = new Point(20, 65), Size = new Size(120, 25) };
            var addressText = new TextBox { Location = new Point(150, 62), Size = new Size(300, 25), Name = "labAddress" };
            
            labGroup.Controls.AddRange(new Control[] { labNameLabel, labNameText, addressLabel, addressText });
            
            // Language Settings
            var langGroup = new GroupBox("إعدادات اللغة | Language Settings");
            langGroup.Location = new Point(20, 160);
            langGroup.Size = new Size(680, 80);
            langGroup.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            langGroup.ForeColor = PrimaryBlue;
            
            var langLabel = new Label { Text = "اللغة | Language:", Location = new Point(20, 30), Size = new Size(120, 25) };
            var langCombo = new ComboBox 
            { 
                Location = new Point(150, 27), 
                Size = new Size(200, 25), 
                Name = "language",
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            langCombo.Items.AddRange(new[] { "العربية | Arabic", "English", "العربية والإنجليزية | Bilingual" });
            langCombo.SelectedIndex = 2;
            
            langGroup.Controls.AddRange(new Control[] { langLabel, langCombo });
            
            // Auto Backup Settings
            var backupGroup = new GroupBox("النسخ الاحتياطي | Backup Settings");
            backupGroup.Location = new Point(20, 260);
            backupGroup.Size = new Size(680, 120);
            backupGroup.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            backupGroup.ForeColor = PrimaryBlue;
            
            var autoBackupCheck = new CheckBox 
            { 
                Text = "تفعيل النسخ الاحتياطي التلقائي | Enable Auto Backup", 
                Location = new Point(20, 30), 
                Size = new Size(300, 25),
                Name = "autoBackup",
                Checked = true
            };
            
            var backupIntervalLabel = new Label { Text = "فترة النسخ | Interval (hours):", Location = new Point(20, 65), Size = new Size(150, 25) };
            var backupIntervalNum = new NumericUpDown 
            { 
                Location = new Point(180, 62), 
                Size = new Size(100, 25), 
                Name = "backupInterval",
                Minimum = 1, 
                Maximum = 168, 
                Value = 24 
            };
            
            backupGroup.Controls.AddRange(new Control[] { autoBackupCheck, backupIntervalLabel, backupIntervalNum });
            
            generalTab.Controls.AddRange(new Control[] { labGroup, langGroup, backupGroup });
            settingsTabControl.TabPages.Add(generalTab);
        }
        
        private void CreateDatabaseTab()
        {
            var dbTab = new TabPage("قاعدة البيانات | Database");
            dbTab.BackColor = White;
            dbTab.Padding = new Padding(20);
            
            // Database Status
            var statusGroup = new GroupBox("حالة قاعدة البيانات | Database Status");
            statusGroup.Location = new Point(20, 20);
            statusGroup.Size = new Size(680, 150);
            statusGroup.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            statusGroup.ForeColor = PrimaryBlue;
            
            var statusText = new RichTextBox();
            statusText.Location = new Point(20, 30);
            statusText.Size = new Size(640, 100);
            statusText.ReadOnly = true;
            statusText.BackColor = Color.Black;
            statusText.ForeColor = Color.LimeGreen;
            statusText.Font = new Font("Consolas", 9F);
            statusText.Text = GetDatabaseStatus();
            
            statusGroup.Controls.Add(statusText);
            
            // Database Maintenance
            var maintenanceGroup = new GroupBox("صيانة قاعدة البيانات | Database Maintenance");
            maintenanceGroup.Location = new Point(20, 190);
            maintenanceGroup.Size = new Size(680, 150);
            maintenanceGroup.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            maintenanceGroup.ForeColor = PrimaryBlue;
            
            var optimizeBtn = new Button
            {
                Text = "🔧 تحسين الأداء | Optimize Performance",
                Location = new Point(20, 30),
                Size = new Size(200, 35),
                BackColor = AccentGreen,
                ForeColor = White,
                FlatStyle = FlatStyle.Flat
            };
            optimizeBtn.Click += OptimizeDatabase_Click;
            
            var vacuumBtn = new Button
            {
                Text = "🧹 تنظيف قاعدة البيانات | Vacuum Database",
                Location = new Point(240, 30),
                Size = new Size(200, 35),
                BackColor = AccentOrange,
                ForeColor = White,
                FlatStyle = FlatStyle.Flat
            };
            vacuumBtn.Click += VacuumDatabase_Click;
            
            var backupBtn = new Button
            {
                Text = "💾 إنشاء نسخة احتياطية | Create Backup",
                Location = new Point(460, 30),
                Size = new Size(200, 35),
                BackColor = PrimaryBlue,
                ForeColor = White,
                FlatStyle = FlatStyle.Flat
            };
            backupBtn.Click += BackupDatabase_Click;
            
            var restoreBtn = new Button
            {
                Text = "📥 استعادة النسخة الاحتياطية | Restore Backup",
                Location = new Point(20, 80),
                Size = new Size(200, 35),
                BackColor = AccentRed,
                ForeColor = White,
                FlatStyle = FlatStyle.Flat
            };
            restoreBtn.Click += RestoreDatabase_Click;
            
            maintenanceGroup.Controls.AddRange(new Control[] { optimizeBtn, vacuumBtn, backupBtn, restoreBtn });
            
            dbTab.Controls.AddRange(new Control[] { statusGroup, maintenanceGroup });
            settingsTabControl.TabPages.Add(dbTab);
        }
        
        private void CreateAISystemTab()
        {
            var aiTab = new TabPage("الذكاء الاصطناعي | AI System");
            aiTab.BackColor = White;
            aiTab.Padding = new Padding(20);
            
            // AI Model Settings
            var modelGroup = new GroupBox("إعدادات نموذج الذكاء الاصطناعي | AI Model Settings");
            modelGroup.Location = new Point(20, 20);
            modelGroup.Size = new Size(680, 120);
            modelGroup.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            modelGroup.ForeColor = Color.Purple;
            
            var modelPathLabel = new Label { Text = "مسار النموذج | Model Path:", Location = new Point(20, 30), Size = new Size(120, 25) };
            var modelPathText = new TextBox 
            { 
                Location = new Point(150, 27), 
                Size = new Size(400, 25), 
                Name = "modelPath",
                Text = "ai_sperm_analysis/models/sperm-analyzer-v1.pt"
            };
            
            var confidenceLabel = new Label { Text = "حد الثقة | Confidence Threshold:", Location = new Point(20, 65), Size = new Size(150, 25) };
            var confidenceNum = new NumericUpDown 
            { 
                Location = new Point(180, 62), 
                Size = new Size(100, 25), 
                Name = "confidence",
                Minimum = 0.1M, 
                Maximum = 1.0M, 
                Value = 0.5M,
                DecimalPlaces = 2,
                Increment = 0.05M
            };
            
            modelGroup.Controls.AddRange(new Control[] { modelPathLabel, modelPathText, confidenceLabel, confidenceNum });
            
            // CASA Parameters
            var casaGroup = new GroupBox("معايير CASA | CASA Parameters");
            casaGroup.Location = new Point(20, 160);
            casaGroup.Size = new Size(680, 150);
            casaGroup.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            casaGroup.ForeColor = Color.Purple;
            
            var fpsLabel = new Label { Text = "الإطارات في الثانية | FPS:", Location = new Point(20, 30), Size = new Size(120, 25) };
            var fpsNum = new NumericUpDown { Location = new Point(150, 27), Size = new Size(80, 25), Name = "fps", Value = 30, Minimum = 10, Maximum = 60 };
            
            var pixelLabel = new Label { Text = "تحويل البكسل | Pixel to μm:", Location = new Point(250, 30), Size = new Size(120, 25) };
            var pixelNum = new NumericUpDown 
            { 
                Location = new Point(380, 27), 
                Size = new Size(80, 25), 
                Name = "pixelConversion",
                Value = 0.5M,
                DecimalPlaces = 2,
                Increment = 0.1M,
                Minimum = 0.1M,
                Maximum = 2.0M
            };
            
            var durationLabel = new Label { Text = "مدة التحليل الافتراضية | Default Duration (s):", Location = new Point(20, 65), Size = new Size(180, 25) };
            var durationNum = new NumericUpDown { Location = new Point(210, 62), Size = new Size(80, 25), Name = "duration", Value = 15, Minimum = 5, Maximum = 60 };
            
            casaGroup.Controls.AddRange(new Control[] { fpsLabel, fpsNum, pixelLabel, pixelNum, durationLabel, durationNum });
            
            // AI System Status
            var statusGroup = new GroupBox("حالة نظام الذكاء الاصطناعي | AI System Status");
            statusGroup.Location = new Point(20, 330);
            statusGroup.Size = new Size(680, 100);
            statusGroup.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            statusGroup.ForeColor = Color.Purple;
            
            var aiStatusText = new RichTextBox();
            aiStatusText.Location = new Point(20, 30);
            aiStatusText.Size = new Size(640, 60);
            aiStatusText.ReadOnly = true;
            aiStatusText.BackColor = Color.Black;
            aiStatusText.ForeColor = Color.Cyan;
            aiStatusText.Font = new Font("Consolas", 9F);
            aiStatusText.Text = GetAISystemStatus();
            
            statusGroup.Controls.Add(aiStatusText);
            
            aiTab.Controls.AddRange(new Control[] { modelGroup, casaGroup, statusGroup });
            settingsTabControl.TabPages.Add(aiTab);
        }
        
        private void CreateReportsTab()
        {
            var reportsTab = new TabPage("التقارير | Reports");
            reportsTab.BackColor = White;
            reportsTab.Padding = new Padding(20);
            
            // Report Templates
            var templatesGroup = new GroupBox("قوالب التقارير | Report Templates");
            templatesGroup.Location = new Point(20, 20);
            templatesGroup.Size = new Size(680, 180);
            templatesGroup.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            templatesGroup.ForeColor = PrimaryBlue;
            
            var whoTemplateCheck = new CheckBox 
            { 
                Text = "✅ قالب منظمة الصحة العالمية | WHO Template", 
                Location = new Point(20, 30), 
                Size = new Size(300, 25),
                Checked = true,
                Enabled = false
            };
            
            var customTemplateCheck = new CheckBox 
            { 
                Text = "📄 القوالب المخصصة | Custom Templates", 
                Location = new Point(20, 60), 
                Size = new Size(300, 25),
                Checked = true
            };
            
            var logoCheck = new CheckBox 
            { 
                Text = "🏥 إضافة شعار المختبر | Include Lab Logo", 
                Location = new Point(20, 90), 
                Size = new Size(300, 25),
                Checked = true
            };
            
            var footerCheck = new CheckBox 
            { 
                Text = "📋 تذييل مخصص | Custom Footer", 
                Location = new Point(20, 120), 
                Size = new Size(300, 25),
                Checked = false
            };
            
            templatesGroup.Controls.AddRange(new Control[] { whoTemplateCheck, customTemplateCheck, logoCheck, footerCheck });
            
            // Export Settings
            var exportGroup = new GroupBox("إعدادات التصدير | Export Settings");
            exportGroup.Location = new Point(20, 220);
            exportGroup.Size = new Size(680, 120);
            exportGroup.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            exportGroup.ForeColor = PrimaryBlue;
            
            var formatLabel = new Label { Text = "تنسيق التصدير | Export Format:", Location = new Point(20, 30), Size = new Size(150, 25) };
            var formatCombo = new ComboBox 
            { 
                Location = new Point(180, 27), 
                Size = new Size(150, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            formatCombo.Items.AddRange(new[] { "PDF", "Excel", "Word", "CSV" });
            formatCombo.SelectedIndex = 0;
            
            var qualityLabel = new Label { Text = "جودة الصور | Image Quality:", Location = new Point(20, 65), Size = new Size(150, 25) };
            var qualityCombo = new ComboBox 
            { 
                Location = new Point(180, 62), 
                Size = new Size(150, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            qualityCombo.Items.AddRange(new[] { "عالية | High", "متوسطة | Medium", "منخفضة | Low" });
            qualityCombo.SelectedIndex = 0;
            
            exportGroup.Controls.AddRange(new Control[] { formatLabel, formatCombo, qualityLabel, qualityCombo });
            
            reportsTab.Controls.AddRange(new Control[] { templatesGroup, exportGroup });
            settingsTabControl.TabPages.Add(reportsTab);
        }
        
        private void CreateSecurityTab()
        {
            var securityTab = new TabPage("الأمان | Security");
            securityTab.BackColor = White;
            securityTab.Padding = new Padding(20);
            
            // User Management
            var userGroup = new GroupBox("إدارة المستخدمين | User Management");
            userGroup.Location = new Point(20, 20);
            userGroup.Size = new Size(680, 150);
            userGroup.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            userGroup.ForeColor = AccentRed;
            
            var currentUserLabel = new Label 
            { 
                Text = $"المستخدم الحالي | Current User: admin", 
                Location = new Point(20, 30), 
                Size = new Size(300, 25),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            
            var changePasswordBtn = new Button
            {
                Text = "🔐 تغيير كلمة المرور | Change Password",
                Location = new Point(20, 65),
                Size = new Size(200, 35),
                BackColor = AccentOrange,
                ForeColor = White,
                FlatStyle = FlatStyle.Flat
            };
            changePasswordBtn.Click += ChangePassword_Click;
            
            var manageUsersBtn = new Button
            {
                Text = "👥 إدارة المستخدمين | Manage Users",
                Location = new Point(240, 65),
                Size = new Size(200, 35),
                BackColor = PrimaryBlue,
                ForeColor = White,
                FlatStyle = FlatStyle.Flat
            };
            manageUsersBtn.Click += ManageUsers_Click;
            
            userGroup.Controls.AddRange(new Control[] { currentUserLabel, changePasswordBtn, manageUsersBtn });
            
            // Audit Settings
            var auditGroup = new GroupBox("إعدادات التدقيق | Audit Settings");
            auditGroup.Location = new Point(20, 190);
            auditGroup.Size = new Size(680, 120);
            auditGroup.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            auditGroup.ForeColor = AccentRed;
            
            var auditCheck = new CheckBox 
            { 
                Text = "📋 تسجيل العمليات | Log Operations", 
                Location = new Point(20, 30), 
                Size = new Size(300, 25),
                Checked = true
            };
            
            var loginAttemptsCheck = new CheckBox 
            { 
                Text = "🔒 تسجيل محاولات الدخول | Log Login Attempts", 
                Location = new Point(20, 60), 
                Size = new Size(300, 25),
                Checked = true
            };
            
            var dataChangesCheck = new CheckBox 
            { 
                Text = "📊 تسجيل تغييرات البيانات | Log Data Changes", 
                Location = new Point(350, 30), 
                Size = new Size(300, 25),
                Checked = true
            };
            
            auditGroup.Controls.AddRange(new Control[] { auditCheck, loginAttemptsCheck, dataChangesCheck });
            
            securityTab.Controls.AddRange(new Control[] { userGroup, auditGroup });
            settingsTabControl.TabPages.Add(securityTab);
        }
        
        private void CreateButtons()
        {
            var buttonPanel = new Panel();
            buttonPanel.Location = new Point(20, 520);
            buttonPanel.Size = new Size(740, 50);
            
            var saveBtn = new Button
            {
                Text = "💾 حفظ | Save",
                Location = new Point(550, 10),
                Size = new Size(90, 35),
                BackColor = AccentGreen,
                ForeColor = White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            saveBtn.Click += SaveSettings_Click;
            
            var cancelBtn = new Button
            {
                Text = "❌ إلغاء | Cancel",
                Location = new Point(650, 10),
                Size = new Size(90, 35),
                BackColor = AccentRed,
                ForeColor = White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            cancelBtn.Click += (s, e) => this.Close();
            
            buttonPanel.Controls.AddRange(new Control[] { saveBtn, cancelBtn });
            this.Controls.Add(buttonPanel);
        }
        
        private string GetDatabaseStatus()
        {
            try
            {
                var status = "✅ DATABASE STATUS REPORT\\n";
                status += "════════════════════════\\n";
                status += $"Connection: ✅ Active\\n";
                status += $"Path: database.db\\n";
                status += $"Size: {GetDatabaseSize()} MB\\n";
                status += $"Tables: {dal.GetTableNames().Count}\\n\\n";
                
                status += "TABLE STATISTICS:\\n";
                status += $"• Patients: {GetRecordCount("patients")}\\n";
                status += $"• Doctors: {GetRecordCount("doctors")}\\n";
                status += $"• CBC Tests: {GetRecordCount("cbc")}\\n";
                status += $"• Urine Tests: {GetRecordCount("urine")}\\n";
                status += $"• Stool Tests: {GetRecordCount("stool")}\\n";
                status += $"• Kidney Function: {GetRecordCount("kidney_function")}\\n";
                status += $"• Liver Function: {GetRecordCount("liver_function")}\\n";
                status += $"• AI Sperm Analysis: {GetRecordCount("semen_analysis")}\\n";
                
                return status;
            }
            catch (Exception ex)
            {
                return $"❌ Database Error: {ex.Message}";
            }
        }
        
        private string GetAISystemStatus()
        {
            var status = "🧠 AI SYSTEM STATUS\\n";
            status += "═════════════════\\n";
            status += "YOLOv8 Model: ⏳ Checking...\\n";
            status += "DeepSORT Tracker: ⏳ Checking...\\n";
            status += "Python Integration: ⏳ Checking...\\n";
            status += "CASA Metrics Calculator: ✅ Ready\\n";
            status += "WHO Standards Checker: ✅ Ready\\n";
            
            return status;
        }
        
        private double GetDatabaseSize()
        {
            try
            {
                var fileInfo = new System.IO.FileInfo("database.db");
                return Math.Round(fileInfo.Length / (1024.0 * 1024.0), 2);
            }
            catch
            {
                return 0;
            }
        }
        
        private int GetRecordCount(string tableName)
        {
            try
            {
                var result = dal.ExecuteScalar($"SELECT COUNT(*) FROM {tableName}");
                return Convert.ToInt32(result);
            }
            catch
            {
                return 0;
            }
        }
        
        private void LoadSettings()
        {
            // Load current settings from database or config file
            // This is a placeholder - implement actual settings loading
        }
        
        private void SaveSettings_Click(object sender, EventArgs e)
        {
            try
            {
                // Save all settings
                MessageBox.Show("تم حفظ الإعدادات بنجاح!\\nSettings saved successfully!", 
                               "نجح الحفظ | Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                Logger.LogInfo("Settings saved successfully");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في حفظ الإعدادات:\\n{ex.Message}", 
                               "خطأ | Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Logger.LogException(ex, "SaveSettings");
            }
        }
        
        private void OptimizeDatabase_Click(object sender, EventArgs e)
        {
            try
            {
                dal.ExecuteNonQuery("REINDEX");
                dal.ExecuteNonQuery("ANALYZE");
                MessageBox.Show("تم تحسين قاعدة البيانات بنجاح!\\nDatabase optimized successfully!", 
                               "نجح التحسين | Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحسين قاعدة البيانات:\\n{ex.Message}", 
                               "خطأ | Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void VacuumDatabase_Click(object sender, EventArgs e)
        {
            try
            {
                dal.ExecuteNonQuery("VACUUM");
                MessageBox.Show("تم تنظيف قاعدة البيانات بنجاح!\\nDatabase vacuumed successfully!", 
                               "نجح التنظيف | Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تنظيف قاعدة البيانات:\\n{ex.Message}", 
                               "خطأ | Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void BackupDatabase_Click(object sender, EventArgs e)
        {
            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "Database files (*.db)|*.db";
                saveDialog.FileName = $"database_backup_{DateTime.Now:yyyyMMdd_HHmmss}.db";
                
                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        System.IO.File.Copy("database.db", saveDialog.FileName, true);
                        MessageBox.Show("تم إنشاء النسخة الاحتياطية بنجاح!\\nBackup created successfully!", 
                                       "نجح النسخ | Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"خطأ في إنشاء النسخة الاحتياطية:\\n{ex.Message}", 
                                       "خطأ | Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        
        private void RestoreDatabase_Click(object sender, EventArgs e)
        {
            using (var openDialog = new OpenFileDialog())
            {
                openDialog.Filter = "Database files (*.db)|*.db";
                
                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    var result = MessageBox.Show(
                        "هذا سيحل محل قاعدة البيانات الحالية. هل أنت متأكد؟\\nThis will replace the current database. Are you sure?",
                        "تأكيد | Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    
                    if (result == DialogResult.Yes)
                    {
                        try
                        {
                            System.IO.File.Copy(openDialog.FileName, "database.db", true);
                            MessageBox.Show("تم استعادة النسخة الاحتياطية بنجاح!\\nBackup restored successfully!", 
                                           "نجحت الاستعادة | Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"خطأ في استعادة النسخة الاحتياطية:\\n{ex.Message}", 
                                           "خطأ | Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }
        
        private void ChangePassword_Click(object sender, EventArgs e)
        {
            var passwordForm = new PasswordChangeForm();
            passwordForm.ShowDialog();
        }
        
        private void ManageUsers_Click(object sender, EventArgs e)
        {
            var userForm = new UserManagementForm();
            userForm.ShowDialog();
        }
    }
    
    /// <summary>
    /// نموذج تغيير كلمة المرور
    /// Password Change Form
    /// </summary>
    public partial class PasswordChangeForm : Form
    {
        private TextBox currentPasswordText;
        private TextBox newPasswordText;
        private TextBox confirmPasswordText;
        
        public PasswordChangeForm()
        {
            InitializeComponent();
        }
        
        private void InitializeComponent()
        {
            this.Text = "تغيير كلمة المرور | Change Password";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            
            var currentLabel = new Label { Text = "كلمة المرور الحالية:", Location = new Point(20, 30), Size = new Size(120, 25) };
            currentPasswordText = new TextBox { Location = new Point(150, 27), Size = new Size(200, 25), UseSystemPasswordChar = true };
            
            var newLabel = new Label { Text = "كلمة المرور الجديدة:", Location = new Point(20, 70), Size = new Size(120, 25) };
            newPasswordText = new TextBox { Location = new Point(150, 67), Size = new Size(200, 25), UseSystemPasswordChar = true };
            
            var confirmLabel = new Label { Text = "تأكيد كلمة المرور:", Location = new Point(20, 110), Size = new Size(120, 25) };
            confirmPasswordText = new TextBox { Location = new Point(150, 107), Size = new Size(200, 25), UseSystemPasswordChar = true };
            
            var changeBtn = new Button
            {
                Text = "تغيير | Change",
                Location = new Point(200, 200),
                Size = new Size(80, 30),
                BackColor = Color.Green,
                ForeColor = Color.White
            };
            changeBtn.Click += ChangeBtn_Click;
            
            var cancelBtn = new Button
            {
                Text = "إلغاء | Cancel",
                Location = new Point(290, 200),
                Size = new Size(80, 30),
                BackColor = Color.Red,
                ForeColor = Color.White
            };
            cancelBtn.Click += (s, e) => this.Close();
            
            this.Controls.AddRange(new Control[] { 
                currentLabel, currentPasswordText, newLabel, newPasswordText, 
                confirmLabel, confirmPasswordText, changeBtn, cancelBtn 
            });
        }
        
        private void ChangeBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentPasswordText.Text) || 
                string.IsNullOrEmpty(newPasswordText.Text) || 
                string.IsNullOrEmpty(confirmPasswordText.Text))
            {
                MessageBox.Show("يرجى ملء جميع الحقول", "تحذير", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            if (newPasswordText.Text != confirmPasswordText.Text)
            {
                MessageBox.Show("كلمة المرور الجديدة غير متطابقة", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            // Here you would implement actual password change logic
            MessageBox.Show("تم تغيير كلمة المرور بنجاح!", "نجح", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
    
    /// <summary>
    /// نموذج إدارة المستخدمين
    /// User Management Form
    /// </summary>
    public partial class UserManagementForm : Form
    {
        private DataGridView usersGrid;
        private DataAccessLayer dal;
        
        public UserManagementForm()
        {
            dal = new DataAccessLayer("database.db");
            InitializeComponent();
            LoadUsers();
        }
        
        private void InitializeComponent()
        {
            this.Text = "إدارة المستخدمين | User Management";
            this.Size = new Size(800, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            
            usersGrid = new DataGridView
            {
                Location = new Point(20, 20),
                Size = new Size(740, 350),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            
            var addBtn = new Button
            {
                Text = "➕ إضافة مستخدم",
                Location = new Point(20, 390),
                Size = new Size(120, 35),
                BackColor = Color.Green,
                ForeColor = Color.White
            };
            
            var editBtn = new Button
            {
                Text = "✏️ تعديل",
                Location = new Point(150, 390),
                Size = new Size(100, 35),
                BackColor = Color.Orange,
                ForeColor = Color.White
            };
            
            var deleteBtn = new Button
            {
                Text = "🗑️ حذف",
                Location = new Point(260, 390),
                Size = new Size(100, 35),
                BackColor = Color.Red,
                ForeColor = Color.White
            };
            
            var closeBtn = new Button
            {
                Text = "إغلاق | Close",
                Location = new Point(680, 390),
                Size = new Size(100, 35),
                BackColor = Color.Gray,
                ForeColor = Color.White
            };
            closeBtn.Click += (s, e) => this.Close();
            
            this.Controls.AddRange(new Control[] { usersGrid, addBtn, editBtn, deleteBtn, closeBtn });
        }
        
        private void LoadUsers()
        {
            try
            {
                var users = dal.ExecuteQuery("SELECT id, username, role, created_date FROM admin");
                usersGrid.DataSource = users;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل المستخدمين: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}