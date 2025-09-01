using System;
using System.Data;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Unit tests for the PatientService class
/// </summary>
[TestClass]
public class PatientServiceTests
{
    private PatientService _patientService;
    private string _testDatabasePath;
    
    /// <summary>
    /// Initializes the test environment before each test
    /// </summary>
    [TestInitialize]
    public void TestInitialize()
    {
        // Create a temporary database for testing
        _testDatabasePath = Path.GetTempFileName();
        File.Copy("database.db", _testDatabasePath, true);
        
        // Initialize the patient service with the test database
        _patientService = new PatientService(_testDatabasePath);
    }
    
    /// <summary>
    /// Cleans up the test environment after each test
    /// </summary>
    [TestCleanup]
    public void TestCleanup()
    {
        // Delete the temporary database
        if (File.Exists(_testDatabasePath))
        {
            File.Delete(_testDatabasePath);
        }
    }
    
    /// <summary>
    /// Tests that GetAllPatients returns a DataTable
    /// </summary>
    [TestMethod]
    public void GetAllPatients_ReturnsDataTable()
    {
        // Act
        DataTable result = _patientService.GetAllPatients();
        
        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(DataTable));
    }
    
    /// <summary>
    /// Tests that GetPatientCount returns a non-negative number
    /// </summary>
    [TestMethod]
    public void GetPatientCount_ReturnsNonNegativeNumber()
    {
        // Act
        int result = _patientService.GetPatientCount();
        
        // Assert
        Assert.IsTrue(result >= 0);
    }
    
    /// <summary>
    /// Tests that SearchPatientsByName handles null input gracefully
    /// </summary>
    [TestMethod]
    public void SearchPatientsByName_HandlesNullInput()
    {
        // Act
        DataTable result = _patientService.SearchPatientsByName(null);
        
        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(DataTable));
    }
    
    /// <summary>
    /// Tests that SearchPatientsByName handles empty input gracefully
    /// </summary>
    [TestMethod]
    public void SearchPatientsByName_HandlesEmptyInput()
    {
        // Act
        DataTable result = _patientService.SearchPatientsByName(string.Empty);
        
        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(DataTable));
    }
    
    /// <summary>
    /// Tests that AddPatient validates required fields
    /// </summary>
    [TestMethod]
    public void AddPatient_ValidatesRequiredFields()
    {
        // Act
        bool result = _patientService.AddPatient(
            string.Empty,  // Empty name
            DateTime.Now,
            "M",
            "123-456-7890");
        
        // Assert
        Assert.IsFalse(result);
    }
    
    /// <summary>
    /// Tests that UpdatePatient validates required fields
    /// </summary>
    [TestMethod]
    public void UpdatePatient_ValidatesRequiredFields()
    {
        // Act
        bool result = _patientService.UpdatePatient(
            1,  // Valid patient ID
            string.Empty,  // Empty name
            DateTime.Now,
            "M",
            "123-456-7890");
        
        // Assert
        Assert.IsFalse(result);
    }
    
    /// <summary>
    /// Tests that ErrorHandling provides safe date conversion
    /// </summary>
    [TestMethod]
    public void ErrorHandling_SafeConvertToDateTime_HandlesInvalidInput()
    {
        // Act
        DateTime result = ErrorHandling.SafeConvertToDateTime("invalid-date", DateTime.Now);
        
        // Assert
        Assert.AreEqual(DateTime.Now.Date, result.Date);
    }
    
    /// <summary>
    /// Tests that ErrorHandling provides safe integer conversion
    /// </summary>
    [TestMethod]
    public void ErrorHandling_SafeConvertToInt_HandlesInvalidInput()
    {
        // Act
        int result = ErrorHandling.SafeConvertToInt("invalid-integer", 42);
        
        // Assert
        Assert.AreEqual(42, result);
    }
    
    /// <summary>
    /// Tests that ErrorHandling provides safe double conversion
    /// </summary>
    [TestMethod]
    public void ErrorHandling_SafeConvertToDouble_HandlesInvalidInput()
    {
        // Act
        double result = ErrorHandling.SafeConvertToDouble("invalid-double", 3.14);
        
        // Assert
        Assert.AreEqual(3.14, result);
    }
    
    /// <summary>
    /// Tests that DataValidator validates phone numbers correctly
    /// </summary>
    [TestMethod]
    public void DataValidator_ValidatesPhoneNumbers()
    {
        // Act
        bool validResult = DataValidator.ValidatePhoneNumber("123-456-7890");
        bool invalidResult = DataValidator.ValidatePhoneNumber("invalid-phone");
        
        // Assert
        Assert.IsTrue(validResult);
        Assert.IsFalse(invalidResult);
    }
    
    /// <summary>
    /// Tests that DataValidator validates email addresses correctly
    /// </summary>
    [TestMethod]
    public void DataValidator_ValidatesEmailAddresses()
    {
        // Act
        bool validResult = DataValidator.ValidateEmail("test@example.com");
        bool invalidResult = DataValidator.ValidateEmail("invalid-email");
        
        // Assert
        Assert.IsTrue(validResult);
        Assert.IsFalse(invalidResult);
    }
    
    /// <summary>
    /// Tests that DataValidator validates patient IDs correctly
    /// </summary>
    [TestMethod]
    public void DataValidator_ValidatesPatientIds()
    {
        // Act
        bool validResult = DataValidator.ValidatePatientId(1);
        bool invalidResult = DataValidator.ValidatePatientId(-1);
        
        // Assert
        Assert.IsTrue(validResult);
        Assert.IsFalse(invalidResult);
    }
    
    /// <summary>
    /// Tests that DataValidator validates dates of birth correctly
    /// </summary>
    [TestMethod]
    public void DataValidator_ValidatesDatesOfBirth()
    {
        // Act
        bool validResult = DataValidator.ValidateDateOfBirth(DateTime.Now.AddYears(-30));
        bool invalidResult = DataValidator.ValidateDateOfBirth(DateTime.Now.AddYears(1)); // Future date
        
        // Assert
        Assert.IsTrue(validResult);
        Assert.IsFalse(invalidResult);
    }
}