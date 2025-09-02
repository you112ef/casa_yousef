@echo off
echo Creating path_vis table in database...
echo ======================================

echo Generating SQL script...
echo CREATE TABLE IF NOT EXISTS path_vis ( > create_table.sql
echo     id INTEGER PRIMARY KEY AUTOINCREMENT, >> create_table.sql
echo     path_id INTEGER, >> create_table.sql
echo     x_coordinate REAL, >> create_table.sql
echo     y_coordinate REAL, >> create_table.sql
echo     timestamp DATETIME DEFAULT CURRENT_TIMESTAMP, >> create_table.sql
echo     visibility_status INTEGER DEFAULT 1, >> create_table.sql
echo     color TEXT, >> create_table.sql
echo     line_width REAL DEFAULT 1.0, >> create_table.sql
echo     FOREIGN KEY ^(path_id^) REFERENCES path^(id^) >> create_table.sql
echo ^); >> create_table.sql
echo. >> create_table.sql
echo CREATE INDEX IF NOT EXISTS idx_path_vis_path_id ON path_vis^(path_id^); >> create_table.sql
echo CREATE INDEX IF NOT EXISTS idx_path_vis_timestamp ON path_vis^(timestamp^); >> create_table.sql

echo SQL script created.

echo Attempting to execute SQL script...
echo This may fail if sqlite3 is not installed, but that's OK.

sqlite3 database.db ".read create_table.sql" >nul 2>nul
if %errorlevel% == 0 (
    echo Successfully executed SQL script with sqlite3
) else (
    echo sqlite3 not available or failed to execute script
    echo The SQL script has been created for manual execution if needed
)

echo Cleaning up...
del create_table.sql >nul 2>nul

echo.
echo Verifying table creation...
call ExamineTables.bat | findstr /i "path_vis" >nul
if %errorlevel% == 0 (
    echo SUCCESS: path_vis table found in database!
) else (
    echo INFO: Could not automatically verify table creation
    echo Please run ExamineTables.bat manually to check
)

echo.
echo Process completed.
pause