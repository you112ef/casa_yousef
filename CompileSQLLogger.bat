@echo off
echo Compiling SQL Logger...
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

REM Try to compile with the GAC version of SQLite
csc /reference:"C:\Windows\Microsoft.Net\assembly\GAC_32\System.Data.SQLite\v4.0_1.0.119.0__db937bc2d44ff139\System.Data.SQLite.dll" SQLLogger.cs

if %errorlevel% neq 0 (
    echo.
    echo Error compiling with GAC reference.
    echo Trying with assembly name only...
    echo.
    
    REM Try with just the assembly name
    csc /reference:System.Data.SQLite SQLLogger.cs
    
    if %errorlevel% neq 0 (
        echo.
        echo Error compiling the SQL Logger.
        echo You may need to install System.Data.SQLite separately.
        echo.
        pause
        exit /b
    )
)

echo.
echo Compilation successful!
echo.
echo To run the SQL logger:
echo 1. Make sure database.db is accessible
echo 2. Run: SQLLogger.exe
echo.
echo The program will create a sql_log.txt file with query analysis.
echo.
pause