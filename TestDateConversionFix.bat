@echo off
echo Testing Date Conversion Fix
echo ==========================
echo.
echo Compiling test program...
echo.
csc TestDateConversionFix.cs
if %errorlevel% neq 0 (
    echo.
    echo ERROR: Failed to compile test program
    echo Please ensure csc (C# compiler) is available in your PATH
    echo.
    pause
    exit /b 1
)
echo.
echo Running test...
echo.
TestDateConversionFix.exe
echo.
echo Test completed.
echo.
pause