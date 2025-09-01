using System;
using System.Data;
using System.Windows.Forms;

public partial class MainForm : Form
{
    private DataAccessLayer dal;
    private DataGridView dataGridView;
    private Button loadAllDataButton;
    private ComboBox tableSelector;
    private Label statusLabel;
    private Button patientRecordsButton;
    private PatientService patientService;

    public MainForm()
    {
        InitializeComponent();
        dal = new DataAccessLayer("database.db");
        patientService = new PatientService();
        LoadTableNames();
    }

    private void InitializeComponent()
    {
        this.dataGridView = new DataGridView();
        this.loadAllDataButton = new Button();
        this.tableSelector = new ComboBox();
        this.statusLabel = new Label();
        this.patientRecordsButton = new Button();
        
        // Setup DataGridView
        this.dataGridView.Dock = DockStyle.Fill;
        this.dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        
        // Setup Button
        this.loadAllDataButton.Text = "Load All Data";
        this.loadAllDataButton.Dock = DockStyle.Bottom;
        this.loadAllDataButton.Click += LoadAllDataButton_Click;
        
        // Setup Patient Records Button
        this.patientRecordsButton.Text = "Patient Records";
        this.patientRecordsButton.Dock = DockStyle.Bottom;
        this.patientRecordsButton.Click += PatientRecordsButton_Click;
        
        // Setup ComboBox
        this.tableSelector.Dock = DockStyle.Top;
        this.tableSelector.DropDownStyle = ComboBoxStyle.DropDownList;
        
        // Setup Status Label
        this.statusLabel.Dock = DockStyle.Bottom;
        this.statusLabel.Text = "Ready";
        this.statusLabel.TextAlign = ContentAlignment.MiddleLeft;
        
        // Setup Form
        this.Text = "Sky CASA - Medical Laboratory Analysis System";
        this.Size = new System.Drawing.Size(800, 600);
        
        // Add controls to form
        this.Controls.Add(this.dataGridView);
        this.Controls.Add(this.loadAllDataButton);
        this.Controls.Add(this.patientRecordsButton);
        this.Controls.Add(this.statusLabel);
        this.Controls.Add(this.tableSelector);
    }

    private void LoadTableNames()
    {
        try
        {
            var tables = dal.GetTableNames();
            tableSelector.Items.Clear();
            foreach (var table in tables)
            {
                if (table != "sqlite_sequence") // Skip internal SQLite table
                {
                    tableSelector.Items.Add(table);
                }
            }
            
            if (tableSelector.Items.Count > 0)
            {
                tableSelector.SelectedIndex = 0;
            }
            
            Logger.LogInfo($"Loaded {tables.Count} table names");
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, "MainForm.LoadTableNames");
            ErrorHandling.HandleException(ex, "Loading Table Names");
        }
    }

    private void LoadAllDataButton_Click(object sender, EventArgs e)
    {
        if (tableSelector.SelectedItem == null)
        {
            MessageBox.Show("Please select a table first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            statusLabel.Text = "Loading data...";
            Application.DoEvents(); // Update UI
            
            string selectedTable = tableSelector.SelectedItem.ToString();
            DataTable allData = dal.GetAllData(selectedTable);
            
            dataGridView.DataSource = allData;
            statusLabel.Text = $"Loaded {allData.Rows.Count} records from {selectedTable}";
            
            Logger.LogInfo($"Loaded {allData.Rows.Count} records from table {selectedTable}");
        }
        catch (Exception ex)
        {
            statusLabel.Text = "Error loading data";
            Logger.LogException(ex, "MainForm.LoadAllDataButton_Click");
            ErrorHandling.HandleException(ex, "Loading Data");
        }
    }
    
    private void PatientRecordsButton_Click(object sender, EventArgs e)
    {
        try
        {
            // Open the patient records form
            Rec patientRecordsForm = new Rec();
            patientRecordsForm.Show();
            
            Logger.LogInfo("Opened patient records form");
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, "MainForm.PatientRecordsButton_Click");
            ErrorHandling.HandleException(ex, "Opening Patient Records");
        }
    }
}