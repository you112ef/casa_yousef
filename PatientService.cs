using System;
using System.Data;
using System.Collections.Generic;

/// <summary>
/// Service class for patient-related business logic in the Sky CASA application
/// </summary>
public class PatientService
{
    private readonly DatabaseHelper _databaseHelper;
    
    public PatientService()
    {
        // Initialize with default database path
        DatabaseHelper.Initialize("database.db");
    }
    
    public PatientService(string databasePath)
    {
        // Initialize with custom database path
        DatabaseHelper.Initialize(databasePath);
    }
    
    /// <summary>
    /// Gets all patients from the database
    /// </summary>
    /// <returns>DataTable containing all patients</returns>
    public DataTable GetAllPatients()
    {
        try
        {
            return DatabaseHelper.ExecuteQuery("SELECT * FROM patients ORDER BY name");
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, "PatientService.GetAllPatients");
            ErrorHandling.HandleException(ex, "Retrieving Patients");
            return new DataTable();
        }
    }
    
    /// <summary>
    /// Gets a patient by ID
    /// </summary>
    /// <param name="patientId">The ID of the patient to retrieve</param>
    /// <returns>DataRow containing patient information, or null if not found</returns>
    public DataRow GetPatientById(int patientId)
    {
        try
        {
            var dataTable = DatabaseHelper.ExecuteQuery(
                "SELECT * FROM patients WHERE id = @patientId", 
                patientId);
                
            if (dataTable.Rows.Count > 0)
            {
                return dataTable.Rows[0];
            }
            return null;
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, "PatientService.GetPatientById");
            ErrorHandling.HandleException(ex, "Retrieving Patient");
            return null;
        }
    }
    
    /// <summary>
    /// Searches for patients by name
    /// </summary>
    /// <param name="name">The name to search for</param>
    /// <returns>DataTable containing matching patients</returns>
    public DataTable SearchPatientsByName(string name)
    {
        try
        {
            return DatabaseHelper.ExecuteQuery(
                "SELECT * FROM patients WHERE name LIKE @name ORDER BY name", 
                $"%{name}%");
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, "PatientService.SearchPatientsByName");
            ErrorHandling.HandleException(ex, "Searching Patients");
            return new DataTable();
        }
    }
    
    /// <summary>
    /// Adds a new patient to the database
    /// </summary>
    /// <param name="name">Patient name</param>
    /// <param name="birthDate">Patient birth date</param>
    /// <param name="gender">Patient gender</param>
    /// <param name="phoneNumber">Patient phone number</param>
    /// <returns>True if successful, false otherwise</returns>
    public bool AddPatient(string name, DateTime birthDate, string gender, string phoneNumber)
    {
        try
        {
            // Validate required fields
            if (!ErrorHandling.ValidateRequiredString(name, "Patient Name"))
                return false;
                
            if (!ErrorHandling.ValidateRequiredString(gender, "Gender"))
                return false;
            
            // Validate date range
            if (!ErrorHandling.ValidateDateRange(birthDate, "Birth Date"))
                return false;
                
            int result = DatabaseHelper.ExecuteNonQuery(
                "INSERT INTO patients (name, birth_date, gender, phone_number) VALUES (@name, @birthDate, @gender, @phoneNumber)",
                name, birthDate, gender, phoneNumber);
                
            Logger.LogInfo($"Added new patient: {name}");
            return result > 0;
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, "PatientService.AddPatient");
            ErrorHandling.HandleException(ex, "Adding Patient");
            return false;
        }
    }
    
    /// <summary>
    /// Updates an existing patient in the database
    /// </summary>
    /// <param name="patientId">ID of the patient to update</param>
    /// <param name="name">Patient name</param>
    /// <param name="birthDate">Patient birth date</param>
    /// <param name="gender">Patient gender</param>
    /// <param name="phoneNumber">Patient phone number</param>
    /// <returns>True if successful, false otherwise</returns>
    public bool UpdatePatient(int patientId, string name, DateTime birthDate, string gender, string phoneNumber)
    {
        try
        {
            // Validate required fields
            if (!ErrorHandling.ValidateRequiredString(name, "Patient Name"))
                return false;
                
            if (!ErrorHandling.ValidateRequiredString(gender, "Gender"))
                return false;
            
            // Validate date range
            if (!ErrorHandling.ValidateDateRange(birthDate, "Birth Date"))
                return false;
                
            int result = DatabaseHelper.ExecuteNonQuery(
                "UPDATE patients SET name = @name, birth_date = @birthDate, gender = @gender, phone_number = @phoneNumber WHERE id = @patientId",
                name, birthDate, gender, phoneNumber, patientId);
                
            Logger.LogInfo($"Updated patient ID: {patientId}");
            return result > 0;
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, "PatientService.UpdatePatient");
            ErrorHandling.HandleException(ex, "Updating Patient");
            return false;
        }
    }
    
    /// <summary>
    /// Deletes a patient from the database
    /// </summary>
    /// <param name="patientId">ID of the patient to delete</param>
    /// <returns>True if successful, false otherwise</returns>
    public bool DeletePatient(int patientId)
    {
        try
        {
            // Check if patient has any associated records
            var recordCount = DatabaseHelper.ExecuteScalar(
                "SELECT COUNT(*) FROM cbc WHERE patient_id = @patientId",
                patientId);
                
            if (recordCount != null && Convert.ToInt32(recordCount) > 0)
            {
                Logger.LogWarning($"Cannot delete patient ID {patientId} because they have associated CBC records");
                ErrorHandling.HandleException(
                    new Exception("Cannot delete patient with associated records"), 
                    "Deleting Patient", 
                    true);
                return false;
            }
            
            int result = DatabaseHelper.ExecuteNonQuery(
                "DELETE FROM patients WHERE id = @patientId",
                patientId);
                
            Logger.LogInfo($"Deleted patient ID: {patientId}");
            return result > 0;
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, "PatientService.DeletePatient");
            ErrorHandling.HandleException(ex, "Deleting Patient");
            return false;
        }
    }
    
    /// <summary>
    /// Gets the count of all patients
    /// </summary>
    /// <returns>Number of patients in the database</returns>
    public int GetPatientCount()
    {
        try
        {
            var result = DatabaseHelper.ExecuteScalar("SELECT COUNT(*) FROM patients");
            return Convert.ToInt32(result);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, "PatientService.GetPatientCount");
            ErrorHandling.HandleException(ex, "Counting Patients");
            return 0;
        }
    }
}