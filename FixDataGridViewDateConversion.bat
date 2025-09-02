@echo off
echo Sky CASA DataGridView Date Conversion Fix
echo ========================================
echo.
echo This script will fix the InvalidCastException that occurs when
echo clicking on DataGridView rows by replacing direct date conversions
echo with safe conversions using the SafeConversionHelper functions.
echo.
echo Prerequisites:
echo - PowerShell must be installed and enabled
echo - SafeConversionHelper.vb must be present in the project
echo.
echo Press any key to continue or Ctrl+C to cancel...
pause >nul
echo.
echo Running fix script...
echo.
powershell -ExecutionPolicy Bypass -File "%~dp0FixDataGridViewDateConversion.ps1"
echo.
echo Fix script completed.
echo.
echo Next steps:
echo 1. Review the changes made to any files
echo 2. Compile the application
echo 3. Test the DataGridView functionality
echo 4. Refer to DATAGRIDVIEW_DATE_CONVERSION_FIX.txt for manual fix instructions if needed
echo.
pause