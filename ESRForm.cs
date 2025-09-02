using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace SkyCASA
{
    public class ESRForm : Form
    {
        private NumericUpDown numPatientId;
        private DateTimePicker dtTestDate;
        private NumericUpDown numESR;
        private TextBox txtMethod;
        private Button btnSave, btnClose;
        private DataAccessLayer dal;

        public ESRForm()
        {
            dal = new DataAccessLayer("database.db");
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "ESR | معدل الترسيب";
            Size = new Size(420, 260);
            StartPosition = FormStartPosition.CenterParent;

            var lblPatient = new Label { Text = "Patient ID | رقم المريض:", Location = new Point(20, 20), AutoSize = true };
            numPatientId = new NumericUpDown { Location = new Point(200, 18), Width = 100, Minimum = 1, Maximum = 100000, Value = 1 };

            var lblDate = new Label { Text = "Date | التاريخ:", Location = new Point(20, 55), AutoSize = true };
            dtTestDate = new DateTimePicker { Location = new Point(200, 50), Width = 180, Format = DateTimePickerFormat.Custom, CustomFormat = "yyyy-MM-dd HH:mm" };

            var lblESR = new Label { Text = "ESR (mm/hr)", Location = new Point(20, 90), AutoSize = true };
            numESR = new NumericUpDown { Location = new Point(200, 85), DecimalPlaces = 0, Minimum = 0, Maximum = 200, Width = 100 };

            var lblMethod = new Label { Text = "Method | الطريقة:", Location = new Point(20, 125), AutoSize = true };
            txtMethod = new TextBox { Location = new Point(200, 120), Width = 180, Text = "Westergren" };

            btnSave = new Button { Text = "💾 Save | حفظ", Location = new Point(20, 170), Size = new Size(100, 32), BackColor = Color.FromArgb(39,174,96), ForeColor = Color.White };
            btnSave.Click += (s, e) => SaveRecord();

            btnClose = new Button { Text = "إغلاق | Close", Location = new Point(130, 170), Size = new Size(100, 32), BackColor = Color.Gray, ForeColor = Color.White };
            btnClose.Click += (s, e) => Close();

            Controls.AddRange(new Control[] { lblPatient, numPatientId, lblDate, dtTestDate, lblESR, numESR, lblMethod, txtMethod, btnSave, btnClose });
        }

        private void EnsureTable()
        {
            string cols = "id INTEGER PRIMARY KEY AUTOINCREMENT, patient_id INTEGER NOT NULL, test_date DATETIME DEFAULT CURRENT_TIMESTAMP, esr_mm_hr REAL, method TEXT, FOREIGN KEY(patient_id) REFERENCES patients(id)";
            DatabaseHelper.CreateTableIfNotExists("esr", cols);
        }

        private void SaveRecord()
        {
            try
            {
                EnsureTable();
                string sql = "INSERT INTO esr (patient_id, test_date, esr_mm_hr, method) VALUES (@param0, @param1, @param2, @param3)";
                dal.ExecuteNonQuery(sql,
                    (int)numPatientId.Value,
                    dtTestDate.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                    (decimal)numESR.Value,
                    txtMethod.Text.Trim()
                );
                MessageBox.Show("تم حفظ نتيجة ESR بنجاح", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في الحفظ: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
