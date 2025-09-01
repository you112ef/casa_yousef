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
        
        public Rec()
        {
            InitializeComponent();
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
            
            // Search Button
            this.btnSearch.Text = "Search";
            this.btnSearch.Dock = DockStyle.Top;
            this.btnSearch.Click += new EventHandler(BtnSearch_Click);
            
            // Refresh Button
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.Dock = DockStyle.Top;
            this.btnRefresh.Click += new EventHandler(BtnRefresh_Click);
            
            // Form
            this.Text = "Records";
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
                var dal = new DataAccessLayer("database.db");
                var data = dal.GetAllData("cbc"); // Load CBC data as example
                dataGridView1.DataSource = data;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    
                    // Process each cell in the row
                    foreach (DataGridViewCell cell in selectedRow.Cells)
                    {
                        // Handle date columns specifically
                        if (cell.Value != null && cell.Value.ToString().Contains("-") && cell.Value.ToString().Contains(":"))
                        {
                            // This is likely a date column, use safe conversion
                            string dateString = cell.Value.ToString();
                            DateTime selectedDate = SafeConvertToDate(dateString, DateTime.Now);
                            
                            // You can now safely use selectedDate
                            // For example, display it in a text box or use it for further processing
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing selection: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        // Safe date conversion method
        private DateTime SafeConvertToDate(string input, DateTime defaultValue)
        {
            // Handle null or empty strings
            if (string.IsNullOrEmpty(input))
            {
                return defaultValue;
            }
            
            // Try to parse the string as a date
            if (DateTime.TryParse(input.Trim(), out DateTime result))
            {
                return result;
            }
            else
            {
                // Return default value if parsing fails
                return defaultValue;
            }
        }
        
        private void BtnSearch_Click(object sender, EventArgs e)
        {
            // Implement search functionality
            MessageBox.Show("Search functionality would be implemented here");
        }
        
        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadData();
        }
    }
}