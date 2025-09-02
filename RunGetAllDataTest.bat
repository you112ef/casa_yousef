@echo off
echo COMPILING AND RUNNING GetAllData TEST
echo ====================================
echo.

REM Check if database exists
if not exist "database.db" (
    echo Error: database.db not found!
    echo Please run this from the Sky CASA application directory.
    pause
    exit /b 1
)

REM Compile the test application
echo Compiling TestGetAllData.cs...
csc TestGetAllData.cs DataAccessLayer.cs

if errorlevel 1 (
    echo.
    echo Error: Compilation failed!
    pause
    exit /b 1
)

echo.
echo Running TestGetAllData.exe...
echo =============================
echo.

TestGetAllData.exe

echo.
echo Test complete.
pause