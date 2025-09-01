@echo off
echo Compiling Database Analyzer...
echo.

REM Check if csc.exe is available
where csc >nul 2>&1
if %errorlevel% neq 0 (
    echo Error: C# compiler (csc.exe) not found.
    echo Please install .NET Framework SDK or Visual Studio.
    echo.
    pause
    exit /b
)

REM Compile the C# program
csc /reference:"C:\Windows\Microsoft.Net\assembly\GAC_32\System.Data.SQLite\v4.0_1.0.119.0__db937bc2d44ff139\System.Data.SQLite.dll" AnalyzeDatabase.cs

if %errorlevel% neq 0 (
    echo.
    echo Error compiling the program.
    echo Trying alternative approach...
    echo.
    
    REM Try with just the assembly name
    csc /reference:System.Data.SQLite AnalyzeDatabase.cs
    
    if %errorlevel% neq 0 (
        echo.
        echo Error compiling the program.
        echo Make sure System.Data.SQLite is properly installed.
        echo.
        pause
        exit /b
    )
)

echo.
echo Compilation successful!
echo.
echo Running Database Analyzer...
echo.
echo ========================================
AnalyzeDatabase.exe
echo ========================================
echo.
echo Analysis complete.
pause