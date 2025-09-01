@echo off
cls
echo FINAL VERIFICATION OF PATH_VIS TABLE
echo ====================================
echo.

echo Checking if database exists...
if not exist "database.db" (
    echo ERROR: database.db not found!
    pause
    exit /b 1
)

echo ✓ Database file found
echo.

echo Creating test SQL file...
(
echo SELECT name FROM sqlite_master WHERE type='table' AND name='path_vis';
) > test_query.sql

echo ✓ Test SQL file created
echo.

echo Attempting to query database for path_vis table...
sqlite3 database.db ".read test_query.sql" > query_result.txt 2>nul
if %errorlevel% == 0 (
    echo ✓ Query executed successfully
    echo Query result:
    type query_result.txt
) else (
    echo ⚠ sqlite3 not available or query failed
)

echo.
echo Cleaning up...
del test_query.sql >nul 2>nul
del query_result.txt >nul 2>nul

echo.
echo Final check using ExamineTables.bat:
echo -----------------------------------
call ExamineTables.bat | findstr /i "path_vis" >nul
if %errorlevel% == 0 (
    echo *** SUCCESS: path_vis table confirmed in database ***
    echo The "no such table: path_vis" error should be resolved.
) else (
    echo *** RESULT: path_vis table not found in database ***
    echo Manual creation using SQLite browser tool is required.
)

echo.
echo Process completed.
pause