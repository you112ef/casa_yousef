@echo off
echo Starting Sky CASA Application...
echo.
echo If the application doesn't appear, check:
echo 1. Windows Taskbar (it might be minimized)
echo 2. Windows Event Viewer for any error messages
echo.
echo To close this window without affecting the application, just close this window.
echo To close the application, close its main window.
echo.
echo Launching application...
echo.
start "" "Sky_CASA.exe"
timeout /t 2 >nul
echo Application should now be running.
echo.
echo You can now close this window safely.
pause