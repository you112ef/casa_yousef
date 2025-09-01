@echo off
echo ======================================================
echo Sky CASA SQL URI Error Fix Tool
echo ======================================================
echo.

echo Checking for database files...
echo.

if exist "database.db" (
    echo [FOUND] SQLite database: database.db
) else (
    echo [MISSING] SQLite database: database.db
)

if exist "sky_casa.fdb" (
    echo [FOUND] Firebird database: sky_casa.fdb
) else (
    echo [MISSING] Firebird database: sky_casa.fdb
)

echo.
echo Checking required DLLs...
echo.

if exist "System.Data.SQLite.dll" (
    echo [FOUND] System.Data.SQLite.dll
) else (
    echo [MISSING] System.Data.SQLite.dll
)

if exist "FirebirdSql.Data.FirebirdClient.dll" (
    echo [FOUND] FirebirdSql.Data.FirebirdClient.dll
) else (
    echo [MISSING] FirebirdSql.Data.FirebirdClient.dll
)

echo.
echo Creating diagnostic report...
echo.

echo === SQL URI ERROR DIAGNOSTIC REPORT === > sql_uri_fix_report.txt
echo Timestamp: %date% %time% >> sql_uri_fix_report.txt
echo Current Directory: %cd% >> sql_uri_fix_report.txt
echo. >> sql_uri_fix_report.txt

echo Database Files Check: >> sql_uri_fix_report.txt
if exist "database.db" (
    echo   database.db - EXISTS >> sql_uri_fix_report.txt
) else (
    echo   database.db - NOT FOUND >> sql_uri_fix_report.txt
)

if exist "sky_casa.fdb" (
    echo   sky_casa.fdb - EXISTS >> sql_uri_fix_report.txt
) else (
    echo   sky_casa.fdb - NOT FOUND >> sql_uri_fix_report.txt
)

echo. >> sql_uri_fix_report.txt
echo Required DLLs Check: >> sql_uri_fix_report.txt
if exist "System.Data.SQLite.dll" (
    echo   System.Data.SQLite.dll - FOUND >> sql_uri_fix_report.txt
) else (
    echo   System.Data.SQLite.dll - NOT FOUND >> sql_uri_fix_report.txt
)

if exist "FirebirdSql.Data.FirebirdClient.dll" (
    echo   FirebirdSql.Data.FirebirdClient.dll - FOUND >> sql_uri_fix_report.txt
) else (
    echo   FirebirdSql.Data.FirebirdClient.dll - NOT FOUND >> sql_uri_fix_report.txt
)

echo. >> sql_uri_fix_report.txt
echo Suggested Fixes: >> sql_uri_fix_report.txt
echo 1. If SQLite database is missing, copy it to this directory >> sql_uri_fix_report.txt
echo 2. If Firebird database is missing but required, create sky_casa.fdb >> sql_uri_fix_report.txt
echo 3. Ensure all required DLLs are in the application directory >> sql_uri_fix_report.txt
echo 4. Check that the application has read/write permissions to database files >> sql_uri_fix_report.txt

echo.
echo Diagnostic report saved to: sql_uri_fix_report.txt
echo.
echo Suggested actions:
echo 1. Check the diagnostic report for missing files
echo 2. Ensure database files exist in the application directory
echo 3. Verify all required DLLs are present
echo 4. Check file permissions
echo.
echo For more detailed diagnostics, run the application and check application.log
echo.
pause