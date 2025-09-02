using System;
using System.Text.RegularExpressions;

/// <summary>
/// Data validation utilities for the Sky CASA application
/// </summary>
public static class DataValidator
{
    /// <summary>
    /// Validates a phone number format
    /// </summary>
    /// <param name="phoneNumber">Phone number to validate</param>
    /// <returns>True if valid, false otherwise</returns>
    public static bool ValidatePhoneNumber(string phoneNumber)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return false;
                
            // Remove all non-digit characters
            string digitsOnly = Regex.Replace(phoneNumber, @"[^\d]", "");
            
            // Check if we have between 7 and 15 digits (international phone number format)
            if (digitsOnly.Length < 7 || digitsOnly.Length > 15)
                return false;
                
            Logger.LogInfo($"Validated phone number: {phoneNumber}");
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, "DataValidator.ValidatePhoneNumber");
            return false;
        }
    }
    
    /// <summary>
    /// Validates an email address format
    /// </summary>
    /// <param name="email">Email address to validate</param>
    /// <returns>True if valid, false otherwise</returns>
    public static bool ValidateEmail(string email)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;
                
            // Basic email validation regex
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            bool isValid = Regex.IsMatch(email, emailPattern);
            
            if (isValid)
            {
                Logger.LogInfo($"Validated email address: {email}");
            }
            else
            {
                Logger.LogWarning($"Invalid email address format: {email}");
            }
            
            return isValid;
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, "DataValidator.ValidateEmail");
            return false;
        }
    }
    
    /// <summary>
    /// Validates a patient ID
    /// </summary>
    /// <param name="patientId">Patient ID to validate</param>
    /// <returns>True if valid, false otherwise</returns>
    public static bool ValidatePatientId(int patientId)
    {
        try
        {
            // Patient ID should be positive
            bool isValid = patientId > 0;
            
            if (isValid)
            {
                Logger.LogInfo($"Validated patient ID: {patientId}");
            }
            else
            {
                Logger.LogWarning($"Invalid patient ID: {patientId}");
            }
            
            return isValid;
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, "DataValidator.ValidatePatientId");
            return false;
        }
    }
    
    /// <summary>
    /// Validates a date of birth
    /// </summary>
    /// <param name="dateOfBirth">Date of birth to validate</param>
    /// <returns>True if valid, false otherwise</returns>
    public static bool ValidateDateOfBirth(DateTime dateOfBirth)
    {
        try
        {
            // Check if date is in the past and reasonable (not more than 150 years ago)
            DateTime minDate = DateTime.Now.AddYears(-150);
            DateTime maxDate = DateTime.Now;
            
            bool isValid = dateOfBirth >= minDate && dateOfBirth <= maxDate;
            
            if (isValid)
            {
                Logger.LogInfo($"Validated date of birth: {dateOfBirth:yyyy-MM-dd}");
            }
            else
            {
                Logger.LogWarning($"Invalid date of birth: {dateOfBirth:yyyy-MM-dd}");
            }
            
            return isValid;
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, "DataValidator.ValidateDateOfBirth");
            return false;
        }
    }
    
    /// <summary>
    /// Validates a gender value
    /// </summary>
    /// <param name="gender">Gender to validate</param>
    /// <returns>True if valid, false otherwise</returns>
    public static bool ValidateGender(string gender)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(gender))
                return false;
                
            // Convert to uppercase for comparison
            string upperGender = gender.ToUpper().Trim();
            
            // Valid gender values
            bool isValid = upperGender == "M" || upperGender == "F" || 
                          upperGender == "MALE" || upperGender == "FEMALE" ||
                          upperGender == "ذكر" || upperGender == "أنثى";
            
            if (isValid)
            {
                Logger.LogInfo($"Validated gender: {gender}");
            }
            else
            {
                Logger.LogWarning($"Invalid gender value: {gender}");
            }
            
            return isValid;
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, "DataValidator.ValidateGender");
            return false;
        }
    }
    
    /// <summary>
    /// Validates a CBC test value within normal ranges
    /// </summary>
    /// <param name="value">Test value to validate</param>
    /// <param name="testName">Name of the test</param>
    /// <param name="minNormal">Minimum normal value</param>
    /// <param name="maxNormal">Maximum normal value</param>
    /// <returns>Validation result with status and message</returns>
    public static ValidationResult ValidateCBCTestValue(double value, string testName, double minNormal, double maxNormal)
    {
        try
        {
            ValidationResult result = new ValidationResult();
            
            if (value < minNormal)
            {
                result.IsValid = false;
                result.Message = $"{testName} is below normal range ({minNormal}-{maxNormal}). Value: {value:F2}";
                result.Status = "LOW";
            }
            else if (value > maxNormal)
            {
                result.IsValid = false;
                result.Message = $"{testName} is above normal range ({minNormal}-{maxNormal}). Value: {value:F2}";
                result.Status = "HIGH";
            }
            else
            {
                result.IsValid = true;
                result.Message = $"{testName} is within normal range ({minNormal}-{maxNormal}). Value: {value:F2}";
                result.Status = "NORMAL";
            }
            
            Logger.LogInfo($"Validated CBC test value - {result.Message}");
            return result;
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, "DataValidator.ValidateCBCTestValue");
            return new ValidationResult
            {
                IsValid = false,
                Message = $"Error validating {testName}: {ex.Message}",
                Status = "ERROR"
            };
        }
    }
}

/// <summary>
/// Result class for validation operations
/// </summary>
public class ValidationResult
{
    public bool IsValid { get; set; }
    public string Message { get; set; }
    public string Status { get; set; }
    
    public ValidationResult()
    {
        IsValid = false;
        Message = string.Empty;
        Status = "UNKNOWN";
    }
}