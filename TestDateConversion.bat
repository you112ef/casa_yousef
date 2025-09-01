@echo off
echo Testing Date Conversion Fix
echo ==========================
echo.
echo Running PowerShell test script...
echo.
powershell -ExecutionPolicy Bypass -File "%~dp0TestDateConversion.ps1"
echo.
echo Test completed.
echo.
pause