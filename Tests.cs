using System;
using System.Data;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class SkyCASATests
{
    private string testDbPath = "test_database.db";
    private DataAccessLayer dal;

    [TestInitialize]
    public void Setup()
    {
        // Create a test database
        if (File.Exists(testDbPath))
        {
            File.Delete(testDbPath);
        }
        
        // Copy the existing database or create a new one
        File.Copy("database.db", testDbPath);
        dal = new DataAccessLayer(testDbPath);
    }

    [TestCleanup]
    public void Cleanup()
    {
        // Clean up test database
        if (File.Exists(testDbPath))
        {
            File.Delete(testDbPath);
        }
    }

    [TestMethod]
    public void TestDatabaseConnection()
    {
        // Arrange & Act
        var tables = dal.GetTableNames();
        
        // Assert
        Assert.IsNotNull(tables);
        Assert.IsTrue(tables.Count > 0);
    }

    [TestMethod]
    public void TestGetSampleData()
    {
        // Arrange
        var tables = dal.GetTableNames();
        if (tables.Count == 0 || tables[0] == "sqlite_sequence")
        {
            Assert.Inconclusive("No tables available for testing");
            return;
        }
        
        string tableName = tables[0];
        if (tableName == "sqlite_sequence")
        {
            if (tables.Count > 1)
                tableName = tables[1];
            else
            {
                Assert.Inconclusive("No suitable tables available for testing");
                return;
            }
        }
        
        // Act
        var sampleData = dal.GetSampleData(tableName, 5);
        
        // Assert
        Assert.IsNotNull(sampleData);
        Assert.IsTrue(sampleData.Rows.Count <= 5);
    }

    [TestMethod]
    public void TestGetAllData()
    {
        // Arrange
        var tables = dal.GetTableNames();
        if (tables.Count == 0 || tables[0] == "sqlite_sequence")
        {
            Assert.Inconclusive("No tables available for testing");
            return;
        }
        
        string tableName = tables[0];
        if (tableName == "sqlite_sequence")
        {
            if (tables.Count > 1)
                tableName = tables[1];
            else
            {
                Assert.Inconclusive("No suitable tables available for testing");
                return;
            }
        }
        
        // Act
        var allData = dal.GetAllData(tableName);
        var sampleData = dal.GetSampleData(tableName, 10);
        
        // Assert
        Assert.IsNotNull(allData);
        Assert.IsTrue(allData.Rows.Count >= sampleData.Rows.Count);
    }

    [TestMethod]
    public void TestBusinessLogicValidation()
    {
        // Arrange
        var cbcResult = new CBCTestResult
        {
            PatientID = 1,
            TestDate = DateTime.Now,
            WBC = 5.0m,
            RBC = 4.5m,
            Hemoglobin = 14.0m,
            Hematocrit = 42.0m,
            PlateletCount = 250.0m
        };
        
        // Act
        var validationResult = CBCBusinessLogic.ValidateCBCTest(cbcResult);
        
        // Assert
        Assert.IsNotNull(validationResult);
        Assert.IsTrue(validationResult.IsValid);
    }

    [TestMethod]
    public void TestBusinessLogicInterpretation()
    {
        // Arrange
        var cbcResult = new CBCTestResult
        {
            PatientID = 1,
            TestDate = DateTime.Now,
            WBC = 5.0m,
            RBC = 4.5m,
            Hemoglobin = 14.0m,
            Hematocrit = 42.0m,
            PlateletCount = 250.0m,
            NeutrophilsPercent = 60.0m,
            LymphocytesPercent = 30.0m,
            MonocytesPercent = 8.0m
        };
        
        // Act
        var interpretationResult = CBCBusinessLogic.InterpretCBCTest(cbcResult, "Male", 30);
        
        // Assert
        Assert.IsNotNull(interpretationResult);
        Assert.IsNotNull(interpretationResult.Findings);
        Assert.IsNotNull(interpretationResult.CriticalValues);
    }

    [TestMethod]
    public void TestCriticalValueDetection()
    {
        // Arrange
        var normalResult = new CBCTestResult
        {
            WBC = 5.0m,
            Hemoglobin = 14.0m,
            PlateletCount = 250.0m
        };
        
        var criticalResult = new CBCTestResult
        {
            WBC = 0.5m, // Critical low
            Hemoglobin = 14.0m,
            PlateletCount = 250.0m
        };
        
        // Act
        bool normalHasCritical = CBCBusinessLogic.HasCriticalValues(normalResult);
        bool criticalHasCritical = CBCBusinessLogic.HasCriticalValues(criticalResult);
        
        // Assert
        Assert.IsFalse(normalHasCritical);
        Assert.IsTrue(criticalHasCritical);
    }
}