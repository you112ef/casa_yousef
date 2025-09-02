using System;
using System.Windows.Forms;

/// <summary>
/// Comprehensive error handling utilities for the Sky CASA application
/// </summary>
public static class ErrorHandling
{
    /// <summary>
    /// Handles exceptions with appropriate logging and user feedback
    /// </summary>
    /// <param name="ex">The exception to handle</param>
    /// <param name="context">Context information about where the error occurred</param>
    /// <param name="showToUser">Whether to show the error to the user</param>
    public static void HandleException(Exception ex, string context, bool showToUser = true)
    {
        // Log the error
        Logger.LogException(ex, context);
        
        if (showToUser)
        {
            MessageBox.Show(
                $"An error occurred in {context}. Please try again or contact support.\n\nDetails: {ex.Message}",
                "Application Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    }
    
    /// <summary>
    /// Safely converts a string to DateTime with proper error handling
    /// </summary>
    /// <param name="input">The string to convert</param>
    /// <param name="defaultValue">The default value to return if conversion fails</param>
    /// <returns>The converted DateTime or the default value</returns>
    public static DateTime SafeConvertToDateTime(string input, DateTime defaultValue)
    {
        // Handle null or empty strings
        if (string.IsNullOrEmpty(input))
        {
            Logger.LogWarning($"Attempted to convert null or empty string to DateTime. Using default value.");
            return defaultValue;
        }
        
        // Try to parse the string as a date
        if (DateTime.TryParse(input.Trim(), out DateTime result))
        {
            return result;
        }
        else
        {
            // Log the conversion failure
            Logger.LogWarning($"Failed to convert '{input}' to DateTime. Using default value.");
            return defaultValue;
        }
    }
    
    /// <summary>
    /// Safely converts a string to integer with proper error handling
    /// </summary>
    /// <param name="input">The string to convert</param>
    /// <param name="defaultValue">The default value to return if conversion fails</param>
    /// <returns>The converted integer or the default value</returns>
    public static int SafeConvertToInt(string input, int defaultValue)
    {
        // Handle null or empty strings
        if (string.IsNullOrEmpty(input))
        {
            Logger.LogWarning($"Attempted to convert null or empty string to integer. Using default value.");
            return defaultValue;
        }
        
        // Try to parse the string as an integer
        if (int.TryParse(input.Trim(), out int result))
        {
            return result;
        }
        else
        {
            // Log the conversion failure
            Logger.LogWarning($"Failed to convert '{input}' to integer. Using default value.");
            return defaultValue;
        }
    }
    
    /// <summary>
    /// Safely converts a string to double with proper error handling
    /// </summary>
    /// <param name="input">The string to convert</param>
    /// <param name="defaultValue">The default value to return if conversion fails</param>
    /// <returns>The converted double or the default value</returns>
    public static double SafeConvertToDouble(string input, double defaultValue)
    {
        // Handle null or empty strings
        if (string.IsNullOrEmpty(input))
        {
            Logger.LogWarning($"Attempted to convert null or empty string to double. Using default value.");
            return defaultValue;
        }
        
        // Try to parse the string as a double
        if (double.TryParse(input.Trim(), out double result))
        {
            return result;
        }
        else
        {
            // Log the conversion failure
            Logger.LogWarning($"Failed to convert '{input}' to double. Using default value.");
            return defaultValue;
        }
    }
    
    /// <summary>
    /// Validates that a string is not null or empty
    /// </summary>
    /// <param name="input">The string to validate</param>
    /// <param name="fieldName">The name of the field for error messages</param>
    /// <returns>True if valid, false otherwise</returns>
    public static bool ValidateRequiredString(string input, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            Logger.LogWarning($"Validation failed: {fieldName} is required.");
            MessageBox.Show($"{fieldName} is required. Please enter a value.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }
        return true;
    }
    
    /// <summary>
    /// Validates that a date is within a reasonable range
    /// </summary>
    /// <param name="date">The date to validate</param>
    /// <param name="fieldName">The name of the field for error messages</param>
    /// <returns>True if valid, false otherwise</returns>
    public static bool ValidateDateRange(DateTime date, string fieldName)
    {
        // Check if date is in a reasonable range (1900 to 2100)
        if (date.Year < 1900 || date.Year > 2100)
        {
            Logger.LogWarning($"Validation failed: {fieldName} must be between 1900 and 2100.");
            MessageBox.Show($"{fieldName} must be between 1900 and 2100.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }
        return true;
    }
    
    /// <summary>
    /// Validates that a number is within a reasonable range
    /// </summary>
    /// <param name="value">The number to validate</param>
    /// <param name="min">Minimum allowed value</param>
    /// <param name="max">Maximum allowed value</param>
    /// <param name="fieldName">The name of the field for error messages</param>
    /// <returns>True if valid, false otherwise</returns>
    public static bool ValidateNumberRange(double value, double min, double max, string fieldName)
    {
        if (value < min || value > max)
        {
            Logger.LogWarning($"Validation failed: {fieldName} must be between {min} and {max}.");
            MessageBox.Show($"{fieldName} must be between {min} and {max}.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }
        return true;
    }
}