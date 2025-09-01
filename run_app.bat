@echo off
echo Starting Sky CASA Application...
echo ===============================
echo.

REM Check if the executable exists
if not exist "Sky_CASA.exe" (
    echo Error: Sky_CASA.exe not found!
    echo Please build the application first using build.bat
    echo.
    pause
    exit /b 1
)

echo Launching Sky CASA...
echo.

REM Run the application
".\Sky_CASA.exe"

echo.
echo Application closed.
pause