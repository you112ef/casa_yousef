@echo off
cls
echo DIRECT TABLE CREATOR FOR PATH_VIS
echo ================================
echo.

echo Step 1: Creating SQL script to add path_vis table...
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
) > path_vis_creation.sql

echo ✓ SQL script created successfully
echo.

echo Step 2: Attempting to execute SQL script...
echo NOTE: This requires sqlite3 command-line tool
echo.

sqlite3 database.db ".read path_vis_creation.sql" 2>nul
if %errorlevel% == 0 (
    echo ✓ SQL script executed successfully with sqlite3
) else (
    echo ⚠ sqlite3 not available or failed
    echo   Manual intervention may be required
)

echo.
echo Step 3: Cleaning up temporary files...
del path_vis_creation.sql >nul 2>nul

echo.
echo Step 4: Final verification...
echo Running ExamineTables.bat to check for path_vis table:
echo =====================================================
call ExamineTables.bat | findstr /i "path_vis" >nul
if %errorlevel% == 0 (
    echo.
    echo *** SUCCESS: path_vis table found in database! ***
    echo The "no such table: path_vis" error should now be resolved.
) else (
    echo.
    echo *** RESULT: Manual creation may still be needed ***
    echo Please use a SQLite browser tool to manually create the table
    echo using the SQL commands from the path_vis_creation.sql file.
)

echo.
echo Process completed.
pause