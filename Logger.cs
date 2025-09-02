using System;
using System.IO;

/// <summary>
/// Simple logging utility for the Sky CASA application
/// </summary>
public static class Logger
{
    private static readonly string LogFilePath = "application.log";
    
    /// <summary>
    /// Logs an informational message
    /// </summary>
    /// <param name="message">The message to log</param>
    public static void LogInfo(string message)
    {
        LogMessage("INFO", message);
    }
    
    /// <summary>
    /// Logs a warning message
    /// </summary>
    /// <param name="message">The message to log</param>
    public static void LogWarning(string message)
    {
        LogMessage("WARNING", message);
    }
    
    /// <summary>
    /// Logs an error message
    /// </summary>
    /// <param name="message">The message to log</param>
    public static void LogError(string message)
    {
        LogMessage("ERROR", message);
    }
    
    /// <summary>
    /// Logs an exception
    /// </summary>
    /// <param name="ex">The exception to log</param>
    /// <param name="context">Context information about where the error occurred</param>
    public static void LogException(Exception ex, string context)
    {
        string message = $"Exception in {context}: {ex.Message}\nStack Trace: {ex.StackTrace}";
        LogMessage("EXCEPTION", message);
    }
    
    /// <summary>
    /// Internal method to write log messages to file
    /// </summary>
    /// <param name="level">The log level (INFO, WARNING, ERROR, EXCEPTION)</param>
    /// <param name="message">The message to log</param>
    private static void LogMessage(string level, string message)
    {
        try
        {
            string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {message}";
            
            // Ensure the log directory exists
            string logDirectory = Path.GetDirectoryName(LogFilePath);
            if (!string.IsNullOrEmpty(logDirectory) && !Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
            
            // Write to log file
            File.AppendAllText(LogFilePath, logEntry + Environment.NewLine);
        }
        catch
        {
            // If we can't write to the log file, we don't want to throw an exception
            // In a production environment, you might want to handle this differently
        }
    }
    
    /// <summary>
    /// Clears the log file
    /// </summary>
    public static void ClearLog()
    {
        try
        {
            if (File.Exists(LogFilePath))
            {
                File.Delete(LogFilePath);
            }
        }
        catch
        {
            // If we can't clear the log file, we don't want to throw an exception
        }
    }
    
    /// <summary>
    /// Gets the current log content
    /// </summary>
    /// <returns>The content of the log file</returns>
    public static string GetLogContent()
    {
        try
        {
            if (File.Exists(LogFilePath))
            {
                return File.ReadAllText(LogFilePath);
            }
            return string.Empty;
        }
        catch
        {
            return "Error reading log file";
        }
    }
}