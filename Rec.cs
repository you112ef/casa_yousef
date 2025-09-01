using System;
using System.Data;
using System.Windows.Forms;

namespace Sky_CASA
{
    public partial class Rec : Form
    {
        private DataGridView dataGridView1;
        private TextBox txtSearch;
        private Button btnSearch;
        private Button btnRefresh;
        private PatientService patientService;
        
        public Rec()
        {
            InitializeComponent();
            patientService = new PatientService();
            LoadData();
        }
        
        private void InitializeComponent()
        {
            this.dataGridView1 = new DataGridView();
            this.txtSearch = new TextBox();
            this.btnSearch = new Button();
            this.btnRefresh = new Button();
            
            // DataGridView
            this.dataGridView1.Dock = DockStyle.Fill;
            this.dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.Click += new EventHandler(DataGridView1_Click);
            
            // Search TextBox
            this.txtSearch.Dock = DockStyle.Top;
            this.txtSearch.PlaceholderText = "Enter patient name to search...";
            
            // Search Button
            this.btnSearch.Text = "Search";
            this.btnSearch.Dock = DockStyle.Top;
            this.btnSearch.Click += new EventHandler(BtnSearch_Click);
            
            // Refresh Button
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.Dock = DockStyle.Top;
            this.btnRefresh.Click += new EventHandler(BtnRefresh_Click);
            
            // Form
            this.Text = "Patient Records";
            this.Size = new System.Drawing.Size(800, 600);
            
            // Add controls
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.btnRefresh);
        }
        
        private void LoadData()
        {
            try
            {
                // Use the patient service to get all patients
                var data = patientService.GetAllPatients();
                dataGridView1.DataSource = data;
                
                // Update status
                Logger.LogInfo($"Loaded {data.Rows.Count} patients");
            }
            catch (Exception ex)
            {
                ErrorHandling.HandleException(ex, "Loading Patient Data");
            }
        }
        
        private void DataGridView1_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if there are selected rows
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    // Get the selected row
                    DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
                    
                    // Get patient ID from the selected row
                    if (selectedRow.Cells["id"].Value != null)
                    {
                        string patientIdString = selectedRow.Cells["id"].Value.ToString();
                        int patientId = ErrorHandling.SafeConvertToInt(patientIdString, 0);
                        
                        if (patientId > 0)
                        {
                            Logger.LogInfo($"Selected patient ID: {patientId}");
                            // You can now use the patientId for further processing
                            // For example, load detailed patient information or CBC records
                        }
                    }
                    
                    // Process date columns specifically
                    foreach (DataGridViewCell cell in selectedRow.Cells)
                    {
                        // Handle date columns specifically
                        if (cell.Value != null && cell.Value.ToString().Contains("-") && cell.Value.ToString().Contains(":"))
                        {
                            // This is likely a date column, use safe conversion
                            string dateString = cell.Value.ToString();
                            DateTime selectedDate = ErrorHandling.SafeConvertToDateTime(dateString, DateTime.Now);
                            
                            // You can now safely use selectedDate
                            // For example, display it in a text box or use it for further processing
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.HandleException(ex, "Processing DataGridView Selection");
            }
        }
        
        private void BtnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                string searchTerm = txtSearch.Text.Trim();
                
                if (string.IsNullOrEmpty(searchTerm))
                {
                    // If search term is empty, load all data
                    LoadData();
                    return;
                }
                
                // Use the patient service to search for patients
                var data = patientService.SearchPatientsByName(searchTerm);
                dataGridView1.DataSource = data;
                
                Logger.LogInfo($"Search completed for term: {searchTerm}, found {data.Rows.Count} results");
            }
            catch (Exception ex)
            {
                ErrorHandling.HandleException(ex, "Searching Patient Records");
            }
        }
        
        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            LoadData();
        }
    }
}