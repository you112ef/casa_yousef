# Sky CASA Developer Guide

## Table of Contents
1. [Architecture Overview](#architecture-overview)
2. [Code Structure](#code-structure)
3. [Data Access Layer](#data-access-layer)
4. [Business Logic Layer](#business-logic-layer)
5. [User Interface](#user-interface)
6. [Database Schema](#database-schema)
7. [Testing](#testing)
8. [Building and Deployment](#building-and-deployment)
9. [Extending Functionality](#extending-functionality)
10. [Best Practices](#best-practices)

## Architecture Overview
Sky CASA follows a layered architecture pattern with clear separation of concerns:

```
┌─────────────────────────────────────┐
│            Presentation             │
│              Layer                  │
│  (Windows Forms UI, Web API)        │
├─────────────────────────────────────┤
│           Business Logic            │
│              Layer                  │
│  (Validation, Interpretation,       │
│   Calculations)                     │
├─────────────────────────────────────┤
│           Data Access               │
│              Layer                  │
│  (Database Operations,              │
│   Data Mapping)                     │
├─────────────────────────────────────┤
│            Database                 │
│                                     │
│  (SQLite Database)                  │
└─────────────────────────────────────┘
```

## Code Structure
The project is organized as follows:

```
Sky CASA/
├── SkyCASA.sln                 # Solution file
├── SkyCASA.csproj              # Main project file
├── SkyCASATests.csproj         # Test project file
├── Program.cs                  # Application entry point
├── MainForm.cs                 # Main application form
├── DataAccessLayer.cs          # Data access implementation
├── CBC_BusinessLogic.cs        # CBC business logic
├── Tests.cs                    # Unit tests
├── build.bat                   # Build script
├── docs/                       # Documentation
│   ├── api/                    # API documentation
│   ├── developer/              # Developer guides
│   └── user/                   # User manuals
├── packages/                   # NuGet packages
└── *.dll                       # External dependencies
```

## Data Access Layer
The [DataAccessLayer](file:///D:/New%20folder%20(4)/Sky%20CASA/DataAccessLayer.cs#L11-L358) class provides a clean interface for database operations:

### Key Methods
- `ExecuteQuery()`: Executes SELECT queries and returns DataTable
- `ExecuteNonQuery()`: Executes INSERT, UPDATE, DELETE operations
- `ExecuteScalar()`: Executes queries that return a single value
- `GetTableNames()`: Returns a list of all table names
- `GetTableSchema()`: Returns schema information for a table
- `GetSampleData()`: Returns a limited set of data (deprecated)
- `GetAllData()`: Returns all data from a table (new method to fix case limit)

### Example Usage
```csharp
var dal = new DataAccessLayer("database.db");

// Get all patients
var patients = dal.ExecuteQuery("SELECT * FROM Patients");

// Add a new patient
var rowsAffected = dal.ExecuteNonQuery(
    "INSERT INTO Patients (FirstName, LastName) VALUES (@param0, @param1)",
    "John", "Doe");

// Get patient count
var count = (long)dal.ExecuteScalar("SELECT COUNT(*) FROM Patients");
```

## Business Logic Layer
The business logic layer contains validation, interpretation, and calculation logic.

### CBC Business Logic
The [CBCBusinessLogic](file:///D:/New%20folder%20(4)/Sky%20CASA/CBC_BusinessLogic.cs#L5-L233) class provides:

1. **Validation**: Validates CBC test results against reasonable ranges
2. **Interpretation**: Interprets results based on normal ranges
3. **Calculations**: Calculates derived values (absolute counts, etc.)
4. **Critical Value Detection**: Identifies critical values that require immediate attention

### Example Usage
```csharp
var cbcResult = new CBCTestResult
{
    WBC = 5.0m,
    RBC = 4.5m,
    Hemoglobin = 14.0m,
    // ... other values
};

// Validate the result
var validationResult = CBCBusinessLogic.ValidateCBCTest(cbcResult);

// Interpret the result
var interpretationResult = CBCBusinessLogic.InterpretCBCTest(cbcResult, "Male", 30);

// Check for critical values
bool hasCritical = CBCBusinessLogic.HasCriticalValues(cbcResult);
```

## User Interface
The UI is built using Windows Forms with a main form that displays data from the database.

### Key Components
- [MainForm.cs](file:///D:/New%20folder%20(4)/Sky%20CASA/MainForm.cs): Main application window
- DataGridView controls for data display
- ComboBox for table selection
- Buttons for user actions

### UI Best Practices
1. Use data binding for automatic UI updates
2. Implement proper error handling with user-friendly messages
3. Provide visual feedback during long operations
4. Follow consistent layout and styling

## Database Schema
The application uses SQLite with the following key tables:

### Patients Table
```sql
CREATE TABLE Patients (
    PatientID INTEGER PRIMARY KEY AUTOINCREMENT,
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    DateOfBirth DATE,
    Gender VARCHAR(10),
    PhoneNumber VARCHAR(20),
    Email VARCHAR(100),
    Address TEXT,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
);
```

### CBCTestResults Table
```sql
CREATE TABLE CBCTestResults (
    TestResultID INTEGER PRIMARY KEY AUTOINCREMENT,
    PatientID INTEGER NOT NULL,
    AnalysisTypeID INTEGER NOT NULL,
    TechnicianID INTEGER,
    TestDate DATETIME NOT NULL,
    
    -- Basic CBC values
    WBC DECIMAL(10,2),
    RBC DECIMAL(10,2),
    Hemoglobin DECIMAL(10,2),
    Hematocrit DECIMAL(10,2),
    MCV DECIMAL(10,2),
    MCH DECIMAL(10,2),
    MCHC DECIMAL(10,2),
    RDW DECIMAL(10,2),
    PlateletCount DECIMAL(10,2),
    MPV DECIMAL(10,2),
    PDW DECIMAL(10,2),
    
    -- Differential percentages
    NeutrophilsPercent DECIMAL(5,2),
    LymphocytesPercent DECIMAL(5,2),
    MonocytesPercent DECIMAL(5,2),
    EosinophilsPercent DECIMAL(5,2),
    BasophilsPercent DECIMAL(5,2),
    BandsPercent DECIMAL(5,2),
    AtypicalLymphocytesPercent DECIMAL(5,2),
    NRBC DECIMAL(10,2),
    
    -- Absolute counts
    NeutrophilsAbsolute DECIMAL(10,2),
    LymphocytesAbsolute DECIMAL(10,2),
    MonocytesAbsolute DECIMAL(10,2),
    EosinophilsAbsolute DECIMAL(10,2),
    BasophilsAbsolute DECIMAL(10,2),
    BandsAbsolute DECIMAL(10,2),
    
    -- Interpretation and flags
    WBCInterpretation VARCHAR(50),
    RBCInterpretation VARCHAR(50),
    HGBInterpretation VARCHAR(50),
    PLTInterpretation VARCHAR(50),
    DifferentialInterpretation TEXT,
    CriticalValueFlag BOOLEAN DEFAULT FALSE,
    Comments TEXT,
    
    -- Quality control
    QCStatus VARCHAR(20) DEFAULT 'PENDING',
    ReviewedBy INTEGER,
    ReviewDate DATETIME,
    
    FOREIGN KEY (PatientID) REFERENCES Patients(PatientID),
    FOREIGN KEY (AnalysisTypeID) REFERENCES AnalysisTypes(AnalysisTypeID),
    FOREIGN KEY (TechnicianID) REFERENCES Technicians(TechnicianID),
    FOREIGN KEY (ReviewedBy) REFERENCES Technicians(TechnicianID)
);
```

## Testing
The project includes unit tests using MSTest framework.

### Running Tests
1. Open the solution in Visual Studio
2. Build the solution
3. Open the Test Explorer window
4. Run all tests or specific test methods

### Writing Tests
Tests should follow the Arrange-Act-Assert pattern:

```csharp
[TestMethod]
public void TestBusinessLogicValidation()
{
    // Arrange
    var cbcResult = new CBCTestResult
    {
        PatientID = 1,
        TestDate = DateTime.Now,
        WBC = 5.0m,
        // ... other values
    };
    
    // Act
    var validationResult = CBCBusinessLogic.ValidateCBCTest(cbcResult);
    
    // Assert
    Assert.IsNotNull(validationResult);
    Assert.IsTrue(validationResult.IsValid);
}
```

## Building and Deployment
### Prerequisites
- Visual Studio 2019 or later
- .NET Framework 4.7.2 SDK
- NuGet Package Manager

### Building
1. Open `SkyCASA.sln` in Visual Studio
2. Restore NuGet packages: `nuget restore SkyCASA.sln`
3. Build the solution: `msbuild SkyCASA.sln /p:Configuration=Release`

### Deployment
The following files are required for deployment:
- Sky_CASA.exe
- Sky_CASA.exe.config
- database.db (initial database)
- All DLL files
- Documentation files

## Extending Functionality
### Adding New Analysis Types
1. Update the database schema to add new tables
2. Create business logic classes for validation and interpretation
3. Update the UI to display new analysis types
4. Add appropriate unit tests

### Adding New Database Tables
1. Define the table schema in `CBC_DATABASE_SCHEMA.txt`
2. Create the table using SQLite commands
3. Add data access methods in [DataAccessLayer](file:///D:/New%20folder%20(4)/Sky%20CASA/DataAccessLayer.cs#L11-L358)
4. Create business logic classes for the new data
5. Update the UI to display the new data

## Best Practices
### Code Quality
1. Follow C# coding conventions
2. Use meaningful variable and method names
3. Comment complex logic
4. Keep methods small and focused
5. Handle exceptions appropriately

### Database Best Practices
1. Use parameterized queries to prevent SQL injection
2. Follow normalization principles
3. Index frequently queried columns
4. Use appropriate data types
5. Implement proper transaction handling for related operations

### UI Best Practices
1. Maintain consistent layout and styling
2. Provide clear error messages
3. Implement proper data validation
4. Ensure accessibility considerations
5. Use data binding for automatic UI updates

### Security
1. Validate all user input
2. Use parameterized queries
3. Implement proper authentication and authorization
4. Encrypt sensitive data
5. Regularly update dependencies

### Performance
1. Use efficient database queries
2. Implement pagination for large datasets
3. Cache frequently accessed data
4. Optimize database indexes
5. Minimize unnecessary database calls

For additional development questions, please refer to the [DEVELOPER_SETUP.md](file:///D:/New%20folder%20(4)/Sky%20CASA/DEVELOPER_SETUP.md) file or contact the development team.