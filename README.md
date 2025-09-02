n## ONNX AI (no Python)
- Bundled python.exe shim runs ONNX model (yolov5s.onnx) offline.
- SpermAnalysisForm continues to call python; the shim handles it.

# Sky CASA - Medical Laboratory Analysis System

## Overview
Sky CASA is a medical laboratory analysis application designed for processing and analyzing various types of medical tests including CBC (Complete Blood Count), urinalysis, stool analysis, and more.

## Recent Fix: Case Limit Issue
This version includes a fix for the "cannot add more than 10 cases" issue:

### Problem
Users reported that they could not add or view more than 10 cases in the application, even though the database was capable of storing more records.

### Root Cause
The issue was identified as a UI display limitation, not a storage limitation:
- The database (SQLite) can store unlimited records
- The application business logic does not limit case creation
- The UI was using `GetSampleData()` methods with a default limit of 10 records
- This created the illusion of a 10-case limit

### Solution Implemented
Enhanced the DataAccessLayer with a new method that retrieves all records without limits:

1. Added `GetAllData()` method to both C# and VB versions of DataAccessLayer
2. Created UI update guide and automation tools
3. Provided comprehensive verification tools

### Files Modified
- `DataAccessLayer.cs` - Added GetAllData method
- `DataAccessLayer.vb` - Added GetAllData method

### Files Created
- `UI_UPDATE_GUIDE.txt` - Instructions for updating UI code
- `UpdateUIDataAccess.ps1` - PowerShell script to automate UI updates
- `RunUIUpdate.bat` - Batch file to run the PowerShell script
- `VerifyCaseLimitFix.cs` - Comprehensive verification tool
- `RunVerification.bat` - Batch file to compile and run verification
- `COMPLETE_CASE_LIMIT_FIX_GUIDE.txt` - Complete guide explaining the fix

## Recent Fix: DataGridView Date Conversion Issue
This version also includes a fix for an InvalidCastException that occurs when clicking on DataGridView rows:

### Problem
System.InvalidCastException: التحويل من السلسلة "2025-08-31 00:00:00" إلى النوع 'Date' غير صالح.

### Root Cause
The application was using direct date conversion methods that fail when:
1. The input string format doesn't match the system's expected date format
2. The DataGridView cell contains null or DBNull values
3. There are regional/cultural settings differences in date parsing

### Solution Implemented
Enhanced the application with safe date conversion methods:

1. Added `SafeConvertToDate()` method for safe date conversions
2. Created comprehensive error handling documentation
3. Provided automated fix scripts

### Files Created
- `SafeConversionHelper.vb` - Safe conversion functions
- `DATAGRIDVIEW_DATE_CONVERSION_FIX.txt` - Specific fix instructions
- `COMPREHENSIVE_ERROR_HANDLING_GUIDE.txt` - General error handling guide
- `FixDataGridViewDateConversion.ps1` - Automated fix script
- `FixDataGridViewDateConversion.bat` - Batch file to run the fix script
- `Rec.cs` - Fixed Rec form implementation

## Documentation
For comprehensive documentation, please see:
- [User Manual](docs/user/USER_MANUAL.md)
- [Developer Guide](docs/developer/DEVELOPER_GUIDE.md)
- [API Documentation](docs/api/API_DOCUMENTATION.md)
- [Changelog](CHANGELOG.md)
- [Contributing Guide](CONTRIBUTING.md)

## How to Apply the Fixes
1. For case limit fix: Run `RunUIUpdate.bat` to automatically update UI code to use `GetAllData()` instead of `GetSampleData()`
2. For date conversion fix: Run `FixDataGridViewDateConversion.bat` to automatically apply the date conversion fix
3. Or manually update UI forms by replacing calls to `GetSampleData("table_name", 10)` with `GetAllData("table_name")`
4. For large datasets, consider implementing pagination for better performance

## Database
The application uses SQLite for local data storage with the following tables:
- sem, ret, stool, urine, cbc, vis, dr, path, log, admin

## Dependencies
- AForge.NET libraries for image processing
- Firebird SQL Client for database operations
- System.Data.SQLite for SQLite database access

## Running the Application
Execute `Run_Sky_CASA.bat` to start the application.

## Verification
Run `RunVerification.bat` to verify that the case limit fix is working correctly.