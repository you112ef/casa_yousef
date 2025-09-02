@echo off
echo Examining database tables...
echo ===========================
echo.

echo Database file information:
echo ========================
for %%A in (database.db) do (
    echo File size: %%~zA bytes
    echo Last modified: %%~tA
)
echo.

echo Looking for SQLite header...
echo --------------------------
powershell -Command "& {try { $bytes = [System.IO.File]::ReadAllBytes('database.db'); $header = [System.Text.Encoding]::ASCII.GetString($bytes[0..15]); Write-Output $header } catch { Write-Output 'Error reading file' }}"
echo.

echo Searching for table definitions...
echo --------------------------------
findstr /i "CREATE TABLE" database.db >nul
if %errorlevel% == 0 (
    echo CREATE TABLE statements found in database
    echo.
    echo Tables found:
    findstr /i "CREATE TABLE" database.db
) else (
    echo No CREATE TABLE statements found
)
echo.

echo Searching for references to 'path_vis'...
echo --------------------------------------
findstr /i "path_vis" database.db >nul
if %errorlevel% == 0 (
    echo References to 'path_vis' found in database:
    findstr /i "path_vis" database.db
) else (
    echo No references to 'path_vis' found
)
echo.

echo Database examination complete.
pause