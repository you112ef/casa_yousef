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

    public MainForm()
    {
        InitializeComponent();
        dal = new DataAccessLayer("database.db");
        LoadTableNames();
    }

    private void InitializeComponent()
    {
        this.dataGridView = new DataGridView();
        this.loadAllDataButton = new Button();
        this.tableSelector = new ComboBox();
        this.statusLabel = new Label();
        
        // Setup DataGridView
        this.dataGridView.Dock = DockStyle.Fill;
        this.dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        
        // Setup Button
        this.loadAllDataButton.Text = "Load All Data";
        this.loadAllDataButton.Dock = DockStyle.Bottom;
        this.loadAllDataButton.Click += LoadAllDataButton_Click;
        
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
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading table names: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        }
        catch (Exception ex)
        {
            statusLabel.Text = "Error loading data";
            MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}