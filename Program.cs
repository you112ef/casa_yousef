using System;
using System.Windows.Forms;

namespace SkyCASA
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Initialize application utilities
            InitializeApplication();
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
        
        /// <summary>
        /// Initializes application utilities and settings
        /// </summary>
        private static void InitializeApplication()
        {
            try
            {
                // Initialize the database helper
                DatabaseHelper.Initialize("database.db");
                
                // Log application start
                Logger.LogInfo("Sky CASA application started");
                
                // Clear previous log entries for new session
                Logger.ClearLog();
                Logger.LogInfo("Sky CASA application initialized");
                
                // Perform automatic backup if needed
                DatabaseBackup.AutoBackupIfNeeded("database.db");
                
                // Validate application settings
                if (!AppSettings.ValidateSettings())
                {
                    Logger.LogWarning("Application settings validation failed");
                }
            }
            catch (Exception ex)
            {
                // If we can't initialize properly, show a message and exit
                MessageBox.Show(
                    $"Failed to initialize application: {ex.Message}", 
                    "Initialization Error", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
                Environment.Exit(1);
            }
        }
    }
}