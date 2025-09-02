@echo off
echo ==========================================
echo Sky CASA - Date Conversion Fix Test
echo ==========================================
echo.

echo Compiling TestDateFix.cs...
csc TestDateFix.cs SafeDateConversion.cs

if errorlevel 1 (
    echo.
    echo Error: Compilation failed!
    pause
    exit /b 1
)

echo.
echo Running TestDateFix.exe...
echo =========================
echo.

TestDateFix.exe

echo.
echo Test completed.
pause