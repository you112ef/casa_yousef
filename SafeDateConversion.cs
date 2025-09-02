using System;
using System.Globalization;

/// <summary>
/// Safe date conversion utilities for C# code
/// </summary>
public static class SafeDateConversion
{
    /// <summary>
    /// Enhanced safe date conversion with multiple format support
    /// </summary>
    /// <param name="input">The string to convert</param>
    /// <param name="defaultValue">The default value to return if conversion fails</param>
    /// <returns>The converted DateTime value or the default value</returns>
    public static DateTime EnhancedSafeConvertToDate(string input, DateTime defaultValue = default(DateTime))
    {
        // Handle null or empty strings
        if (string.IsNullOrEmpty(input))
        {
            return defaultValue;
        }

        // Trim the input
        string trimmedInput = input.Trim();

        // Try multiple date formats that might be encountered
        string[] dateFormats = {
            "yyyy-MM-dd HH:mm:ss",
            "yyyy-MM-dd HH:mm",
            "yyyy-MM-dd",
            "MM/dd/yyyy HH:mm:ss",
            "MM/dd/yyyy HH:mm",
            "MM/dd/yyyy",
            "dd/MM/yyyy HH:mm:ss",
            "dd/MM/yyyy HH:mm",
            "dd/MM/yyyy"
        };

        // Try to parse with specific formats
        DateTime result;
        foreach (string format in dateFormats)
        {
            if (DateTime.TryParseExact(trimmedInput, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
            {
                return result;
            }
        }

        // Try standard parsing as fallback
        if (DateTime.TryParse(trimmedInput, out result))
        {
            return result;
        }

        // Return default value if all parsing attempts fail
        return defaultValue;
    }

    /// <summary>
    /// Safe conversion of object to DateTime with error handling
    /// </summary>
    /// <param name="value">The value to convert</param>
    /// <param name="defaultValue">The default value to return if conversion fails</param>
    /// <returns>The converted DateTime value or the default value</returns>
    public static DateTime SafeConvertToObject(object value, DateTime defaultValue = default(DateTime))
    {
        try
        {
            // Handle null values
            if (value == null || value == DBNull.Value)
            {
                return defaultValue;
            }

            // If it's already a DateTime, return it directly
            if (value is DateTime)
            {
                return (DateTime)value;
            }

            // Convert to string and use enhanced conversion
            string stringValue = value.ToString();
            return EnhancedSafeConvertToDate(stringValue, defaultValue);
        }
        catch (Exception ex)
        {
            // Log the error and return default value as fallback
            System.Diagnostics.Debug.WriteLine("SafeConvertToObject error: " + ex.Message);
            return defaultValue;
        }
    }
}