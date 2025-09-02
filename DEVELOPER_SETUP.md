# Sky CASA - Developer Setup Guide

## Prerequisites
Before you begin development on Sky CASA, ensure you have the following installed:

1. **Visual Studio 2019 or later** (Community edition is sufficient)
   - .NET desktop development workload
   - .NET Framework 4.7.2 SDK
2. **NuGet Package Manager**
3. **Git** (for version control)
4. **SQLite Browser** (optional, for database inspection)

## Getting Started

### 1. Clone the Repository
```bash
git clone https://github.com/you112ef/casa_yousef.git
cd casa_yousef
```

### 2. Restore Dependencies
The project uses several NuGet packages and external libraries:
- AForge.NET for image processing
- SQLite for database operations
- Firebird SQL Client for additional database support

To restore NuGet packages:
```bash
nuget restore SkyCASA.sln
```

### 3. Build the Project
Open `SkyCASA.sln` in Visual Studio and build the solution, or run:
```bash
msbuild SkyCASA.sln /p:Configuration=Debug
```

### 4. Run the Application
After building, you can run the application from:
- Visual Studio: Press F5 or click "Start"
- Command line: Navigate to `bin\Debug` and run `Sky_CASA.exe`

## Project Structure

### Core Components
- **DataAccessLayer.cs/vb**: Handles all database operations
- **CBC_BusinessLogic.cs/vb**: Contains business logic for CBC analysis
- **MainForm.cs**: Main application UI
- **Program.cs**: Application entry point

### Database
- **database.db**: SQLite database file containing all application data
- **CBC_DATABASE_SCHEMA.txt**: Database schema documentation

### Scripts and Utilities
- **build.bat**: Build script
- **RunVerification.bat**: Runs verification tests
- **RunUIUpdate.bat**: Updates UI to fix case limit issue
- **UpdateUIDataAccess.ps1**: PowerShell script for UI updates

## Development Workflow

### 1. Branching Strategy
- `master`: Production-ready code
- `develop`: Main development branch
- `feature/*`: Feature branches
- `hotfix/*`: Bug fix branches

### 2. Making Changes
1. Create a feature branch from `develop`
2. Make your changes
3. Write tests if applicable
4. Run verification scripts
5. Commit and push your changes
6. Create a pull request

### 3. Testing
Run the verification script to ensure your changes don't break existing functionality:
```bash
RunVerification.bat
```

## Common Development Tasks

### Adding New Analysis Types
1. Update the database schema in `CBC_DATABASE_SCHEMA.txt`
2. Create new business logic classes
3. Update the UI to display new analysis types
4. Add appropriate validation and interpretation logic

### Fixing UI Issues
1. Refer to `UI_UPDATE_GUIDE.txt` for common UI fixes
2. Use `GetAllData()` instead of `GetSampleData()` to avoid case limits
3. Implement pagination for large datasets

### Database Modifications
1. Update `CBC_DATABASE_SCHEMA.txt` with schema changes
2. Create appropriate migration scripts if needed
3. Update business logic to handle new fields
4. Update UI components to display new data

## Troubleshooting

### Common Issues

#### "Cannot add more than 10 cases"
This was a known issue that has been fixed. Ensure:
1. UI code uses `GetAllData()` instead of `GetSampleData()`
2. Run `RunUIUpdate.bat` to automatically fix UI code
3. For large datasets, implement pagination

#### Database Connection Issues
1. Ensure `database.db` exists in the application directory
2. Check file permissions
3. Verify the connection string in `Sky_CASA.exe.config`

#### Missing Dependencies
1. Run `nuget restore SkyCASA.sln`
2. Ensure all DLL files are in the application directory
3. Check that the correct .NET Framework version is installed

### Debugging Tips
1. Use the verification tools to test specific components
2. Check the database directly with SQLite Browser
3. Enable detailed logging in the configuration file
4. Use Visual Studio's debugger for step-by-step execution

## Code Standards

### C# Coding Standards
- Follow Microsoft's C# Coding Conventions
- Use meaningful variable and method names
- Comment complex logic
- Keep methods small and focused
- Handle exceptions appropriately

### Database Standards
- Use parameterized queries to prevent SQL injection
- Follow normalization principles
- Index frequently queried columns
- Use appropriate data types

### UI Standards
- Maintain consistent layout and styling
- Provide clear error messages
- Implement proper data validation
- Ensure accessibility considerations

## Deployment

### Building for Release
1. Switch to Release configuration in Visual Studio
2. Build the solution
3. Test the release build
4. Package necessary files

### Required Files for Deployment
- Sky_CASA.exe
- Sky_CASA.exe.config
- database.db (initial database)
- All DLL files
- Documentation files

## Contributing
1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a pull request

## Support
For development questions or issues, please:
1. Check the documentation
2. Review existing issues
3. Contact the development team