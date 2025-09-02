@echo off
echo RUNNING UI CODE UPDATE FOR CASE LIMIT FIX
echo ========================================
echo.

echo This script will update UI code to use GetAllData instead of GetSampleData
echo to fix the "cannot add more than 10 cases" issue.
echo.

echo Running PowerShell script...
echo =============================
echo.

powershell -ExecutionPolicy Bypass -File "UpdateUIDataAccess.ps1"

echo.
echo UI update process completed.
echo.
echo If no files were automatically updated, you'll need to manually update
echo your UI code according to UI_UPDATE_GUIDE.txt
echo.
pause