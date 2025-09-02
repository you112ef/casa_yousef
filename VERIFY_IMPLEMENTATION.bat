@echo off
echo ======================================================
echo Sky CASA Application - Implementation Verification
echo ======================================================
echo.

echo Checking for required files...
echo.

REM Check for new utility classes
if exist "ErrorHandling.cs" (
    echo [PASS] ErrorHandling.cs found
) else (
    echo [FAIL] ErrorHandling.cs not found
)

if exist "Logger.cs" (
    echo [PASS] Logger.cs found
) else (
    echo [FAIL] Logger.cs not found
)

if exist "ConfigManager.cs" (
    echo [PASS] ConfigManager.cs found
) else (
    echo [FAIL] ConfigManager.cs not found
)

if exist "DatabaseHelper.cs" (
    echo [PASS] DatabaseHelper.cs found
) else (
    echo [FAIL] DatabaseHelper.cs not found
)

if exist "PatientService.cs" (
    echo [PASS] PatientService.cs found
) else (
    echo [FAIL] PatientService.cs not found
)

if exist "ReportGenerator.cs" (
    echo [PASS] ReportGenerator.cs found
) else (
    echo [FAIL] ReportGenerator.cs not found
)

if exist "DataValidator.cs" (
    echo [PASS] DataValidator.cs found
) else (
    echo [FAIL] DataValidator.cs not found
)

if exist "AppSettings.cs" (
    echo [PASS] AppSettings.cs found
) else (
    echo [FAIL] AppSettings.cs not found
)

if exist "DatabaseBackup.cs" (
    echo [PASS] DatabaseBackup.cs found
) else (
    echo [FAIL] DatabaseBackup.cs not found
)

if exist "PatientServiceTests.cs" (
    echo [PASS] PatientServiceTests.cs found
) else (
    echo [FAIL] PatientServiceTests.cs not found
)

echo.
echo Checking for documentation files...
echo.

if exist "BEST_PRACTICES_IMPLEMENTATION.md" (
    echo [PASS] BEST_PRACTICES_IMPLEMENTATION.md found
) else (
    echo [FAIL] BEST_PRACTICES_IMPLEMENTATION.md not found
)

if exist "IMPROVEMENTS_SUMMARY.md" (
    echo [PASS] IMPROVEMENTS_SUMMARY.md found
) else (
    echo [FAIL] IMPROVEMENTS_SUMMARY.md not found
)

if exist "USER_GUIDE.md" (
    echo [PASS] USER_GUIDE.md found
) else (
    echo [FAIL] USER_GUIDE.md not found
)

if exist "IMPLEMENTATION_SUMMARY.txt" (
    echo [PASS] IMPLEMENTATION_SUMMARY.txt found
) else (
    echo [FAIL] IMPLEMENTATION_SUMMARY.txt not found
)

echo.
echo Checking for updated files...
echo.

REM Check if key files have been updated (check for specific content)
findstr /C:"patientService = new PatientService()" "Rec.cs" >nul
if %errorlevel% == 0 (
    echo [PASS] Rec.cs updated with PatientService integration
) else (
    echo [FAIL] Rec.cs not properly updated
)

findstr /C:"DatabaseBackup.AutoBackupIfNeeded" "Program.cs" >nul
if %errorlevel% == 0 (
    echo [PASS] Program.cs updated with DatabaseBackup integration
) else (
    echo [FAIL] Program.cs not properly updated
)

findstr /C:"PatientService patientService" "MainForm.cs" >nul
if %errorlevel% == 0 (
    echo [PASS] MainForm.cs updated with PatientService integration
) else (
    echo [FAIL] MainForm.cs not properly updated
)

echo.
echo Checking for configuration file...
echo.

if exist "appsettings.xml" (
    echo [PASS] appsettings.xml found (will be created on first run if missing)
) else (
    echo [INFO] appsettings.xml will be created on first run
)

echo.
echo Checking for reports directory...
echo.

if exist "reports\" (
    echo [PASS] reports directory found
) else (
    echo [INFO] reports directory will be created when first report is generated
)

echo.
echo Verification complete.
echo.
echo If all checks show [PASS] or [INFO], the implementation was successful.
echo If any checks show [FAIL], please review the installation.
echo.
pause