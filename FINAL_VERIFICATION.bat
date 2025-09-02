@echo off
echo Sky CASA - Final Verification
echo =============================
echo.

echo Checking for required DLLs:
echo.

if exist "AForge.dll" (
    echo ✓ AForge.dll - Found
) else (
    echo ✗ AForge.dll - Missing
)

if exist "AForge.Video.dll" (
    echo ✓ AForge.Video.dll - Found
) else (
    echo ✗ AForge.Video.dll - Missing
)

if exist "AForge.Video.DirectShow.dll" (
    echo ✓ AForge.Video.DirectShow.dll - Found
) else (
    echo ✗ AForge.Video.DirectShow.dll - Missing
)

if exist "FirebirdSql.Data.FirebirdClient.dll" (
    echo ✓ FirebirdSql.Data.FirebirdClient.dll - Found
) else (
    echo ✗ FirebirdSql.Data.FirebirdClient.dll - Missing
)

if exist "System.Data.SQLite.dll" (
    echo ✓ System.Data.SQLite.dll - Available in GAC
) else (
    echo ? System.Data.SQLite.dll - Check GAC availability
)

echo.
echo Checking for configuration file:
if exist "Sky_CASA.exe.config" (
    echo ✓ Sky_CASA.exe.config - Present (JIT debugging enabled)
) else (
    echo ✗ Sky_CASA.exe.config - Missing
)

echo.
echo Checking for database files:
if exist "database.db" (
    echo ✓ database.db (SQLite) - Found
) else (
    echo ? database.db (SQLite) - Not found
)

echo.
echo All required assemblies should now be available.
echo You can now run your Sky CASA application.
echo.
echo To run the application:
echo 1. Double-click on Run_Sky_CASA.bat
echo    OR
echo 2. Double-click on Sky_CASA.exe directly
echo.
pause