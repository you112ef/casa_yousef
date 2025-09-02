@echo off
cls
echo Sky CASA - Fix for "no such table: path_vis" Error
echo =================================================
echo.

echo Step 1: Checking current database structure...
echo ------------------------------------------------
call ExamineTables.bat > temp_output.txt
findstr /i "path_vis" temp_output.txt >nul
if %errorlevel% == 0 (
    echo RESULT: path_vis table already exists in database
    echo No action needed.
    goto :end
) else (
    echo RESULT: path_vis table NOT found - will create it
)

echo.
echo Step 2: Creating SQL script to add path_vis table...
echo ----------------------------------------------------
(
echo CREATE TABLE path_vis (
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
echo CREATE INDEX idx_path_vis_path_id ON path_vis^(path_id^);
echo CREATE INDEX idx_path_vis_timestamp ON path_vis^(timestamp^);
echo.
echo -- Verification query
echo SELECT 'SUCCESS: path_vis table created successfully!' AS result;
) > add_path_vis_table.sql

echo SQL script created: add_path_vis_table.sql

echo.
echo Step 3: Attempting to execute SQL script...
echo --------------------------------------------
echo Note: This step requires sqlite3 command-line tool
echo If sqlite3 is not installed, you'll need to install it or
echo execute the SQL script manually using a SQLite browser tool
echo.

sqlite3 database.db ".read add_path_vis_table.sql" > execution_result.txt 2>&1
if %errorlevel% == 0 (
    echo RESULT: SQL script executed successfully
    type execution_result.txt
) else (
    echo RESULT: sqlite3 not available or execution failed
    echo.
    echo Manual steps needed:
    echo 1. Install SQLite command-line tools, or
    echo 2. Use a SQLite browser tool like DB Browser for SQLite
    echo 3. Execute the add_path_vis_table.sql script manually
    echo.
    echo The SQL script contains:
    echo ========================
    type add_path_vis_table.sql
    echo ========================
)

echo.
echo Step 4: Cleaning up temporary files...
echo ---------------------------------------
del temp_output.txt >nul 2>nul
del execution_result.txt >nul 2>nul
del add_path_vis_table.sql >nul 2>nul

echo.
echo Step 5: Final verification...
echo ----------------------------
call ExamineTables.bat > temp_output2.txt
findstr /i "path_vis" temp_output2.txt >nul
if %errorlevel% == 0 (
    echo RESULT: SUCCESS - path_vis table now exists in database!
    echo The "no such table: path_vis" error should be resolved.
) else (
    echo RESULT: Could not verify table creation
    echo Please check manually using ExamineTables.bat
)

del temp_output2.txt >nul 2>nul

:end
echo.
echo Process completed.
echo =================================================
echo If the error persists, please:
echo 1. Install SQLite command-line tools
echo 2. Or use a SQLite browser to manually create the table
echo 3. Or contact support for further assistance
echo.
pause