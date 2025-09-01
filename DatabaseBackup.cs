using System;
using System.IO;
using System.Windows.Forms;

/// <summary>
/// Database backup utilities for the Sky CASA application
/// </summary>
public static class DatabaseBackup
{
    /// <summary>
    /// Creates a backup of the database file
    /// </summary>
    /// <param name="databasePath">Path to the database file</param>
    /// <param name="backupDirectory">Directory to store backups (optional)</param>
    /// <returns>True if backup was successful, false otherwise</returns>
    public static bool CreateBackup(string databasePath, string backupDirectory = null)
    {
        try
        {
            // Validate database file exists
            if (!File.Exists(databasePath))
            {
                Logger.LogError($"Database file not found: {databasePath}");
                ErrorHandling.HandleException(
                    new FileNotFoundException($"Database file not found: {databasePath}"),
                    "Creating Database Backup",
                    true);
                return false;
            }
            
            // Determine backup directory
            if (string.IsNullOrWhiteSpace(backupDirectory))
            {
                backupDirectory = Path.Combine(Path.GetDirectoryName(databasePath), "backups");
            }
            
            // Ensure backup directory exists
            if (!Directory.Exists(backupDirectory))
            {
                Directory.CreateDirectory(backupDirectory);
            }
            
            // Create backup file name with timestamp
            string fileName = Path.GetFileNameWithoutExtension(databasePath);
            string extension = Path.GetExtension(databasePath);
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string backupFileName = $"{fileName}_backup_{timestamp}{extension}";
            string backupFilePath = Path.Combine(backupDirectory, backupFileName);
            
            // Copy database file to backup location
            File.Copy(databasePath, backupFilePath, true);
            
            Logger.LogInfo($"Database backup created successfully: {backupFilePath}");
            
            // Update last backup date in settings
            AppSettings.LastBackupDate = DateTime.Now;
            
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, "DatabaseBackup.CreateBackup");
            ErrorHandling.HandleException(ex, "Creating Database Backup");
            return false;
        }
    }
    
    /// <summary>
    /// Restores a database from a backup file
    /// </summary>
    /// <param name="backupFilePath">Path to the backup file</param>
    /// <param name="databasePath">Path to restore the database to</param>
    /// <returns>True if restore was successful, false otherwise</returns>
    public static bool RestoreBackup(string backupFilePath, string databasePath)
    {
        try
        {
            // Validate backup file exists
            if (!File.Exists(backupFilePath))
            {
                Logger.LogError($"Backup file not found: {backupFilePath}");
                ErrorHandling.HandleException(
                    new FileNotFoundException($"Backup file not found: {backupFilePath}"),
                    "Restoring Database Backup",
                    true);
                return false;
            }
            
            // Confirm with user before restoring
            if (AppSettings.ShowConfirmations)
            {
                DialogResult result = MessageBox.Show(
                    "Are you sure you want to restore the database from this backup? This will overwrite the current database.",
                    "Confirm Database Restore",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);
                    
                if (result != DialogResult.Yes)
                {
                    Logger.LogInfo("Database restore cancelled by user");
                    return false;
                }
            }
            
            // Close any existing connections to the database
            // Note: In a real application, you would need to ensure all connections are closed
            
            // Copy backup file to database location
            File.Copy(backupFilePath, databasePath, true);
            
            Logger.LogInfo($"Database restored successfully from: {backupFilePath}");
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, "DatabaseBackup.RestoreBackup");
            ErrorHandling.HandleException(ex, "Restoring Database Backup");
            return false;
        }
    }
    
    /// <summary>
    /// Gets a list of available backup files
    /// </summary>
    /// <param name="backupDirectory">Directory containing backups</param>
    /// <param name="databaseName">Name of the database to filter backups for (optional)</param>
    /// <returns>Array of backup file paths</returns>
    public static string[] GetAvailableBackups(string backupDirectory, string databaseName = null)
    {
        try
        {
            // Ensure backup directory exists
            if (!Directory.Exists(backupDirectory))
            {
                return new string[0];
            }
            
            // Get all database backup files
            string searchPattern = string.IsNullOrWhiteSpace(databaseName) 
                ? "*.db" 
                : $"{databaseName}_backup_*.db";
                
            string[] backupFiles = Directory.GetFiles(backupDirectory, searchPattern);
            
            // Sort by creation time (newest first)
            Array.Sort(backupFiles, (x, y) => 
                File.GetCreationTime(y).CompareTo(File.GetCreationTime(x)));
            
            Logger.LogInfo($"Found {backupFiles.Length} backup files in {backupDirectory}");
            return backupFiles;
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, "DatabaseBackup.GetAvailableBackups");
            ErrorHandling.HandleException(ex, "Getting Available Backups");
            return new string[0];
        }
    }
    
    /// <summary>
    /// Automatically creates a backup if needed based on settings
    /// </summary>
    /// <param name="databasePath">Path to the database file</param>
    /// <returns>True if backup was created or not needed, false if error occurred</returns>
    public static bool AutoBackupIfNeeded(string databasePath)
    {
        try
        {
            // Check if auto backup is enabled
            if (!AppSettings.AutoBackupDatabase)
            {
                return true;
            }
            
            // Check if enough time has passed since last backup
            DateTime lastBackup = AppSettings.LastBackupDate;
            int intervalDays = AppSettings.BackupIntervalDays;
            
            if (lastBackup == DateTime.MinValue || 
                DateTime.Now.Subtract(lastBackup).TotalDays >= intervalDays)
            {
                Logger.LogInfo("Creating automatic database backup");
                return CreateBackup(databasePath);
            }
            
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, "DatabaseBackup.AutoBackupIfNeeded");
            ErrorHandling.HandleException(ex, "Checking Auto Backup");
            return false;
        }
    }
    
    /// <summary>
    /// Deletes old backup files to save space
    /// </summary>
    /// <param name="backupDirectory">Directory containing backups</param>
    /// <param name="keepDays">Number of days to keep backups</param>
    /// <returns>Number of files deleted</returns>
    public static int CleanupOldBackups(string backupDirectory, int keepDays = 30)
    {
        try
        {
            // Ensure backup directory exists
            if (!Directory.Exists(backupDirectory))
            {
                return 0;
            }
            
            // Get all backup files
            string[] backupFiles = Directory.GetFiles(backupDirectory, "*_backup_*.db");
            
            int deletedCount = 0;
            DateTime cutoffDate = DateTime.Now.AddDays(-keepDays);
            
            foreach (string backupFile in backupFiles)
            {
                DateTime fileDate = File.GetCreationTime(backupFile);
                
                if (fileDate < cutoffDate)
                {
                    try
                    {
                        File.Delete(backupFile);
                        deletedCount++;
                        Logger.LogInfo($"Deleted old backup file: {backupFile}");
                    }
                    catch (Exception ex)
                    {
                        Logger.LogException(ex, $"Failed to delete backup file: {backupFile}");
                    }
                }
            }
            
            Logger.LogInfo($"Cleaned up {deletedCount} old backup files");
            return deletedCount;
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, "DatabaseBackup.CleanupOldBackups");
            ErrorHandling.HandleException(ex, "Cleaning Up Old Backups");
            return 0;
        }
    }
}