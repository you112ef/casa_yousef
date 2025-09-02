using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace SkyCASA
{
    public class CBCForm : Form
    {
        private NumericUpDown numPatientId;
        private DateTimePicker dtTestDate;
        private NumericUpDown numWBC, numRBC, numHgb, numHct;
        private Button btnSave, btnClose, btnInterpret;
        private RichTextBox txtResult;
        private DataAccessLayer dal;

        public CBCForm()
        {
            dal = new DataAccessLayer("database.db");
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "CBC | فحص الدم الشامل";
            Size = new Size(640, 520);
            StartPosition = FormStartPosition.CenterParent;

            var lblPatient = new Label { Text = "Patient ID | رقم المريض:", Location = new Point(20, 20), AutoSize = true };
            numPatientId = new NumericUpDown { Location = new Point(200, 18), Width = 100, Minimum = 1, Maximum = 100000, Value = 1 };

            var lblDate = new Label { Text = "Date | التاريخ:", Location = new Point(20, 55), AutoSize = true };
            dtTestDate = new DateTimePicker { Location = new Point(200, 50), Width = 200, Format = DateTimePickerFormat.Custom, CustomFormat = "yyyy-MM-dd HH:mm" };

            var lblWBC = new Label { Text = "WBC (10^9/L)", Location = new Point(20, 100), AutoSize = true };
            numWBC = new NumericUpDown { Location = new Point(200, 95), DecimalPlaces = 1, Increment = 0.1M, Minimum = 0, Maximum = 1000, Width = 120 };

            var lblRBC = new Label { Text = "RBC (10^12/L)", Location = new Point(20, 135), AutoSize = true };
            numRBC = new NumericUpDown { Location = new Point(200, 130), DecimalPlaces = 2, Increment = 0.01M, Minimum = 0, Maximum = 100, Width = 120 };

            var lblHgb = new Label { Text = "Hemoglobin (g/dL)", Location = new Point(20, 170), AutoSize = true };
            numHgb = new NumericUpDown { Location = new Point(200, 165), DecimalPlaces = 1, Increment = 0.1M, Minimum = 0, Maximum = 50, Width = 120 };

            var lblHct = new Label { Text = "Hematocrit (%)", Location = new Point(20, 205), AutoSize = true };
            numHct = new NumericUpDown { Location = new Point(200, 200), DecimalPlaces = 1, Increment = 0.1M, Minimum = 0, Maximum = 100, Width = 120 };

            btnSave = new Button { Text = "💾 Save | حفظ", Location = new Point(20, 250), Size = new Size(100, 32), BackColor = Color.FromArgb(39,174,96), ForeColor = Color.White };
            btnSave.Click += (s, e) => SaveRecord();

            btnInterpret = new Button { Text = "🧠 Interpret | تفسير", Location = new Point(130, 250), Size = new Size(120, 32), BackColor = Color.FromArgb(52,152,219), ForeColor = Color.White };
            btnInterpret.Click += (s, e) => Interpret();

            btnClose = new Button { Text = "إغلاق | Close", Location = new Point(260, 250), Size = new Size(110, 32), BackColor = Color.Gray, ForeColor = Color.White };
            btnClose.Click += (s, e) => Close();

            txtResult = new RichTextBox { Location = new Point(20, 300), Size = new Size(580, 160), ReadOnly = true, BackColor = Color.Black, ForeColor = Color.LimeGreen, Font = new Font("Consolas", 9f) };

            Controls.AddRange(new Control[] {
                lblPatient, numPatientId, lblDate, dtTestDate,
                lblWBC, numWBC, lblRBC, numRBC, lblHgb, numHgb, lblHct, numHct,
                btnSave, btnInterpret, btnClose, txtResult
            });
        }

        private void EnsureTable()
        {
            string cols = "test_result_id INTEGER PRIMARY KEY AUTOINCREMENT, patient_id INTEGER NOT NULL, test_date DATETIME DEFAULT CURRENT_TIMESTAMP, wbc REAL, rbc REAL, hemoglobin REAL, hematocrit REAL, FOREIGN KEY(patient_id) REFERENCES patients(id)";
            DatabaseHelper.CreateTableIfNotExists("cbc", cols);
        }

        private void SaveRecord()
        {
            try
            {
                EnsureTable();
                string sql = "INSERT INTO cbc (patient_id, test_date, wbc, rbc, hemoglobin, hematocrit) VALUES (@param0, @param1, @param2, @param3, @param4, @param5)";
                dal.ExecuteNonQuery(sql,
                    (int)numPatientId.Value,
                    dtTestDate.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                    (decimal)numWBC.Value,
                    (decimal)numRBC.Value,
                    (decimal)numHgb.Value,
                    (decimal)numHct.Value
                );
                MessageBox.Show("تم حفظ نتيجة CBC بنجاح", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في الحفظ: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Interpret()
        {
            try
            {
                var result = new CBCTestResult
                {
                    PatientID = (int)numPatientId.Value,
                    TestDate = dtTestDate.Value,
                    WBC = (decimal)numWBC.Value,
                    RBC = (decimal)numRBC.Value,
                    Hemoglobin = (decimal)numHgb.Value,
                    Hematocrit = (decimal)numHct.Value
                };

                var validation = CBCBusinessLogic.ValidateCBCTest(result);
                var interpretation = CBCBusinessLogic.InterpretCBCTest(result, "male", 30);

                txtResult.Clear();
                txtResult.AppendText("CBC VALIDATION\n===================\n");
                if (!validation.IsValid) foreach (var e in validation.Errors) txtResult.AppendText($"- ERROR: {e}\n");
                if (validation.HasWarnings) foreach (var w in validation.Warnings) txtResult.AppendText($"- WARN: {w}\n");
                if (validation.IsValid && !validation.HasWarnings) txtResult.AppendText("OK: Values are within expected ranges.\n");

                txtResult.AppendText("\nINTERPRETATION\n===================\n");
                foreach (var f in interpretation.Findings) txtResult.AppendText($"- {f}\n");
                if (interpretation.HasCriticalValues)
                {
                    txtResult.AppendText("\nCRITICAL VALUES\n-------------------\n");
                    foreach (var c in interpretation.CriticalValues) txtResult.AppendText($"! {c}\n");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
