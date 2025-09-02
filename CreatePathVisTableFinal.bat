@echo off
cls
echo Creating path_vis table in Sky CASA database
echo ==========================================
echo.

echo Checking if database exists...
if not exist "database.db" (
    echo ERROR: database.db not found!
    echo Please run this script from the Sky CASA directory.
    pause
    exit /b 1
)

echo Database found.
echo.

echo Creating SQL script to add path_vis table...
(
echo CREATE TABLE IF NOT EXISTS path_vis ^(
echo     id INTEGER PRIMARY KEY AUTOINCREMENT,
echo     path_id INTEGER,
echo     x_coordinate REAL,
echo     y_coordinate REAL,
echo     timestamp DATETIME DEFAULT CURRENT_TIMESTAMP,
echo     visibility_status INTEGER DEFAULT 1,
echo     color TEXT,
echo     line_width REAL DEFAULT 1.0,
echo     FOREIGN KEY ^(path_id^) REFERENCES path^(id^)
echo ^);
echo.
echo CREATE INDEX IF NOT EXISTS idx_path_vis_path_id ON path_vis^(path_id^);
echo CREATE INDEX IF NOT EXISTS idx_path_vis_timestamp ON path_vis^(timestamp^);
echo.
echo SELECT 'SUCCESS: path_vis table created successfully!' AS result;
) > create_path_vis_table.sql

echo SQL script created.
echo.

echo Attempting to execute SQL script with sqlite3...
sqlite3 database.db ".read create_path_vis_table.sql" > output.txt 2>&1
if %errorlevel% == 0 (
    echo SUCCESS: SQL script executed without errors
    type output.txt
) else (
    echo INFO: sqlite3 not available or failed to execute
    echo.
    echo Manual steps needed:
    echo 1. Install SQLite command-line tools
    echo 2. Or use a SQLite browser tool like DB Browser for SQLite
    echo 3. Execute the create_path_vis_table.sql script manually
    echo.
    echo The SQL script contains:
    echo ========================
    type create_path_vis_table.sql
    echo ========================
)

echo.
echo Cleaning up...
del create_path_vis_table.sql >nul 2>nul
del output.txt >nul 2>nul

echo.
echo Final verification...
call ExamineTables.bat | findstr /i "path_vis" >nul
if %errorlevel% == 0 (
    echo SUCCESS: path_vis table now exists in database!
    echo The "no such table: path_vis" error should be resolved.
) else (
    echo INFO: Could not automatically verify table creation
    echo Please run ExamineTables.bat manually to check
)

echo.
echo Process completed.
pause