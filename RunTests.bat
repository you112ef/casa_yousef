@echo off
cls
echo RUNNING SKY CASA APPLICATION TESTS
echo =================================
echo.

echo Compiling test program...
csc TestApplicationFixes.cs >nul 2>&1
if %errorlevel% == 0 (
    echo ✓ Compilation successful
    echo.
    echo Running tests...
    echo ===============
    TestApplicationFixes.exe
) else (
    echo ✗ Compilation failed
    echo Please ensure csc (C# compiler) is available in your PATH
    echo.
    echo You can manually compile and run the test:
    echo 1. csc TestApplicationFixes.cs
    echo 2. TestApplicationFixes.exe
)

echo.
echo Test process completed.
pause