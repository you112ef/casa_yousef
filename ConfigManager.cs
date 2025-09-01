using System;
using System.IO;
using System.Xml;

/// <summary>
/// Configuration manager for the Sky CASA application
/// </summary>
public static class ConfigManager
{
    private static readonly string ConfigFilePath = "appsettings.xml";
    
    /// <summary>
    /// Gets a configuration value as a string
    /// </summary>
    /// <param name="key">The configuration key</param>
    /// <param name="defaultValue">The default value if key is not found</param>
    /// <returns>The configuration value or default value</returns>
    public static string GetConfigValue(string key, string defaultValue = "")
    {
        try
        {
            if (!File.Exists(ConfigFilePath))
            {
                // Create default config file if it doesn't exist
                CreateDefaultConfig();
            }
            
            XmlDocument doc = new XmlDocument();
            doc.Load(ConfigFilePath);
            
            XmlNode node = doc.SelectSingleNode($"//setting[@key='{key}']");
            if (node != null)
            {
                return node.InnerText;
            }
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, "ConfigManager.GetConfigValue");
        }
        
        return defaultValue;
    }
    
    /// <summary>
    /// Sets a configuration value
    /// </summary>
    /// <param name="key">The configuration key</param>
    /// <param name="value">The value to set</param>
    public static void SetConfigValue(string key, string value)
    {
        try
        {
            if (!File.Exists(ConfigFilePath))
            {
                // Create default config file if it doesn't exist
                CreateDefaultConfig();
            }
            
            XmlDocument doc = new XmlDocument();
            doc.Load(ConfigFilePath);
            
            XmlNode node = doc.SelectSingleNode($"//setting[@key='{key}']");
            if (node != null)
            {
                node.InnerText = value;
            }
            else
            {
                // Create new setting
                XmlNode settingsNode = doc.SelectSingleNode("//settings");
                if (settingsNode != null)
                {
                    XmlElement newSetting = doc.CreateElement("setting");
                    newSetting.SetAttribute("key", key);
                    newSetting.InnerText = value;
                    settingsNode.AppendChild(newSetting);
                }
            }
            
            doc.Save(ConfigFilePath);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, "ConfigManager.SetConfigValue");
        }
    }
    
    /// <summary>
    /// Gets a configuration value as an integer
    /// </summary>
    /// <param name="key">The configuration key</param>
    /// <param name="defaultValue">The default value if key is not found or conversion fails</param>
    /// <returns>The configuration value or default value</returns>
    public static int GetConfigValueAsInt(string key, int defaultValue = 0)
    {
        string value = GetConfigValue(key, defaultValue.ToString());
        if (int.TryParse(value, out int result))
        {
            return result;
        }
        return defaultValue;
    }
    
    /// <summary>
    /// Gets a configuration value as a boolean
    /// </summary>
    /// <param name="key">The configuration key</param>
    /// <param name="defaultValue">The default value if key is not found or conversion fails</param>
    /// <returns>The configuration value or default value</returns>
    public static bool GetConfigValueAsBool(string key, bool defaultValue = false)
    {
        string value = GetConfigValue(key, defaultValue.ToString().ToLower());
        if (bool.TryParse(value, out bool result))
        {
            return result;
        }
        return defaultValue;
    }
    
    /// <summary>
    /// Creates a default configuration file
    /// </summary>
    private static void CreateDefaultConfig()
    {
        try
        {
            XmlDocument doc = new XmlDocument();
            XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(xmlDeclaration);
            
            XmlElement root = doc.CreateElement("configuration");
            doc.AppendChild(root);
            
            XmlElement settings = doc.CreateElement("settings");
            root.AppendChild(settings);
            
            // Add default settings
            AddSetting(doc, settings, "DatabasePath", "database.db");
            AddSetting(doc, settings, "LogLevel", "INFO");
            AddSetting(doc, settings, "EnableLogging", "true");
            AddSetting(doc, settings, "MaxRecordsPerPage", "50");
            AddSetting(doc, settings, "DefaultDateFormat", "yyyy-MM-dd");
            
            doc.Save(ConfigFilePath);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, "ConfigManager.CreateDefaultConfig");
        }
    }
    
    /// <summary>
    /// Helper method to add a setting to the XML document
    /// </summary>
    /// <param name="doc">The XML document</param>
    /// <param name="settings">The settings node</param>
    /// <param name="key">The setting key</param>
    /// <param name="value">The setting value</param>
    private static void AddSetting(XmlDocument doc, XmlElement settings, string key, string value)
    {
        XmlElement setting = doc.CreateElement("setting");
        setting.SetAttribute("key", key);
        setting.InnerText = value;
        settings.AppendChild(setting);
    }
}