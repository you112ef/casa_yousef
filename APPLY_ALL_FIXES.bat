@echo off
echo ===============================================
echo Sky CASA - Apply All Data Fixes
echo ===============================================
echo.

echo Checking for required files...
if not exist "database.db" (
    echo Error: database.db not found!
    echo Please run this from the Sky CASA application directory.
    pause
    exit /b 1
)

if not exist "System.Data.SQLite.dll" (
    echo Error: System.Data.SQLite.dll not found!
    echo Please ensure all required DLLs are in the application directory.
    pause
    exit /b 1
)

echo All required files found.
echo.

echo Copying 64-bit SQLite Interop DLL if needed...
if exist "x64\SQLite.Interop.dll" (
    copy "x64\SQLite.Interop.dll" "SQLite.Interop.dll" >nul
    echo 64-bit SQLite Interop DLL copied.
) else (
    echo 64-bit SQLite Interop DLL not found, continuing with existing DLL.
)
echo.

echo Creating missing path_vis table...
powershell -ExecutionPolicy Bypass -File "AddPathVisTable.ps1"
if errorlevel 1 (
    echo Warning: Failed to create path_vis table using PowerShell script.
    echo Trying alternative method...
    if exist "AddPathVisTable.bat" (
        call "AddPathVisTable.bat"
    ) else (
        echo Warning: AddPathVisTable.bat not found.
    )
)
echo.

echo Creating essential medical lab tables...
powershell -ExecutionPolicy Bypass -File "CreateEssentialTables.ps1"
if errorlevel 1 (
    echo Error: Failed to create essential tables.
    pause
    exit /b 1
)
echo.

echo Verifying database structure...
powershell -ExecutionPolicy Bypass -File "AddMissingData.ps1"
echo.

echo Running case limit fix...
if exist "RunUIUpdate.bat" (
    call "RunUIUpdate.bat"
) else (
    echo Warning: RunUIUpdate.bat not found.
)
echo.

echo All fixes applied successfully!
echo.
echo You can now run the Sky CASA application.
echo.
echo Recommended next steps:
echo 1. Run Sky_CASA.exe to start the application
echo 2. Log in with username: admin, password: admin123
echo 3. Add your first patient and test results
echo.
pause