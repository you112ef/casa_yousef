using System;
using System.Drawing;
using System.Globalization;

/// <summary>
/// Application settings and preferences management for the Sky CASA application
/// </summary>
public static class AppSettings
{
    /// <summary>
    /// Gets or sets the default database path
    /// </summary>
    public static string DatabasePath
    {
        get => ConfigManager.GetConfigValue("DatabasePath", "database.db");
        set => ConfigManager.SetConfigValue("DatabasePath", value);
    }
    
    /// <summary>
    /// Gets or sets the maximum number of records to display per page
    /// </summary>
    public static int MaxRecordsPerPage
    {
        get => ConfigManager.GetConfigValueAsInt("MaxRecordsPerPage", 50);
        set => ConfigManager.SetConfigValue("MaxRecordsPerPage", value.ToString());
    }
    
    /// <summary>
    /// Gets or sets whether logging is enabled
    /// </summary>
    public static bool EnableLogging
    {
        get => ConfigManager.GetConfigValueAsBool("EnableLogging", true);
        set => ConfigManager.SetConfigValue("EnableLogging", value.ToString().ToLower());
    }
    
    /// <summary>
    /// Gets or sets the default date format
    /// </summary>
    public static string DefaultDateFormat
    {
        get => ConfigManager.GetConfigValue("DefaultDateFormat", "yyyy-MM-dd");
        set => ConfigManager.SetConfigValue("DefaultDateFormat", value);
    }
    
    /// <summary>
    /// Gets or sets the application theme color
    /// </summary>
    public static Color ThemeColor
    {
        get
        {
            string colorString = ConfigManager.GetConfigValue("ThemeColor", "Blue");
            return ParseColor(colorString);
        }
        set
        {
            string colorString = value.Name;
            ConfigManager.SetConfigValue("ThemeColor", colorString);
        }
    }
    
    /// <summary>
    /// Gets or sets the application language
    /// </summary>
    public static string Language
    {
        get => ConfigManager.GetConfigValue("Language", "en-US");
        set => ConfigManager.SetConfigValue("Language", value);
    }
    
    /// <summary>
    /// Gets or sets whether to show confirmation dialogs
    /// </summary>
    public static bool ShowConfirmations
    {
        get => ConfigManager.GetConfigValueAsBool("ShowConfirmations", true);
        set => ConfigManager.SetConfigValue("ShowConfirmations", value.ToString().ToLower());
    }
    
    /// <summary>
    /// Gets or sets the last used directory for file operations
    /// </summary>
    public static string LastUsedDirectory
    {
        get => ConfigManager.GetConfigValue("LastUsedDirectory", "");
        set => ConfigManager.SetConfigValue("LastUsedDirectory", value);
    }
    
    /// <summary>
    /// Gets or sets whether to automatically backup the database
    /// </summary>
    public static bool AutoBackupDatabase
    {
        get => ConfigManager.GetConfigValueAsBool("AutoBackupDatabase", true);
        set => ConfigManager.SetConfigValue("AutoBackupDatabase", value.ToString().ToLower());
    }
    
    /// <summary>
    /// Gets or sets the backup interval in days
    /// </summary>
    public static int BackupIntervalDays
    {
        get => ConfigManager.GetConfigValueAsInt("BackupIntervalDays", 7);
        set => ConfigManager.SetConfigValue("BackupIntervalDays", value.ToString());
    }
    
    /// <summary>
    /// Gets or sets the last backup date
    /// </summary>
    public static DateTime LastBackupDate
    {
        get
        {
            string dateString = ConfigManager.GetConfigValue("LastBackupDate", DateTime.MinValue.ToString("yyyy-MM-dd"));
            if (DateTime.TryParseExact(dateString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
            {
                return date;
            }
            return DateTime.MinValue;
        }
        set
        {
            ConfigManager.SetConfigValue("LastBackupDate", value.ToString("yyyy-MM-dd"));
        }
    }
    
    /// <summary>
    /// Parses a color string to a Color object
    /// </summary>
    /// <param name="colorString">String representation of a color</param>
    /// <returns>Color object</returns>
    private static Color ParseColor(string colorString)
    {
        try
        {
            // Try to parse as a known color name
            if (Enum.TryParse(colorString, true, out KnownColor knownColor))
            {
                return Color.FromKnownColor(knownColor);
            }
            
            // Try to parse as hex color
            if (colorString.StartsWith("#") && colorString.Length == 7)
            {
                string hex = colorString.Substring(1);
                if (int.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int colorValue))
                {
                    return Color.FromArgb(
                        (colorValue >> 16) & 0xFF,  // Red
                        (colorValue >> 8) & 0xFF,   // Green
                        colorValue & 0xFF           // Blue
                    );
                }
            }
            
            // Default to blue if parsing fails
            return Color.Blue;
        }
        catch
        {
            return Color.Blue;
        }
    }
    
    /// <summary>
    /// Resets all application settings to their default values
    /// </summary>
    public static void ResetToDefaults()
    {
        try
        {
            DatabasePath = "database.db";
            MaxRecordsPerPage = 50;
            EnableLogging = true;
            DefaultDateFormat = "yyyy-MM-dd";
            ThemeColor = Color.Blue;
            Language = "en-US";
            ShowConfirmations = true;
            LastUsedDirectory = "";
            AutoBackupDatabase = true;
            BackupIntervalDays = 7;
            LastBackupDate = DateTime.MinValue;
            
            Logger.LogInfo("Application settings reset to defaults");
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, "AppSettings.ResetToDefaults");
            ErrorHandling.HandleException(ex, "Resetting Application Settings");
        }
    }
    
    /// <summary>
    /// Validates all current settings
    /// </summary>
    /// <returns>True if all settings are valid, false otherwise</returns>
    public static bool ValidateSettings()
    {
        try
        {
            // Validate database path
            if (string.IsNullOrWhiteSpace(DatabasePath))
            {
                Logger.LogWarning("Invalid database path in settings");
                return false;
            }
            
            // Validate max records per page
            if (MaxRecordsPerPage <= 0 || MaxRecordsPerPage > 1000)
            {
                Logger.LogWarning("Invalid max records per page in settings");
                return false;
            }
            
            // Validate backup interval
            if (BackupIntervalDays <= 0 || BackupIntervalDays > 365)
            {
                Logger.LogWarning("Invalid backup interval in settings");
                return false;
            }
            
            Logger.LogInfo("Application settings validated successfully");
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, "AppSettings.ValidateSettings");
            return false;
        }
    }
}