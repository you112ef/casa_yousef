@echo off
echo Examining database.db...
echo ========================
echo.

echo Database file information:
echo ------------------------
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
) else (
    echo No CREATE TABLE statements found
)
echo.

echo Database examination complete.
pause