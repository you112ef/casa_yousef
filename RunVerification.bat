@echo off
echo COMPILING AND RUNNING CASE LIMIT FIX VERIFICATION
echo =================================================
echo.

REM Check if database exists
if not exist "database.db" (
    echo Warning: database.db not found!
    echo This test works best with an existing database.
    echo.
)

REM Compile the verification application
echo Compiling VerifyCaseLimitFix.cs...
csc VerifyCaseLimitFix.cs DataAccessLayer.cs

if errorlevel 1 (
    echo.
    echo Error: Compilation failed!
    echo Make sure you're running this from the Sky CASA directory.
    pause
    exit /b 1
)

echo.
echo Running verification...
echo ======================
echo.

VerifyCaseLimitFix.exe

echo.
echo Verification complete.
pause