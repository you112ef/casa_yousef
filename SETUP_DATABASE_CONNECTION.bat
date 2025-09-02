@echo off
cls
echo SKY CASA DATABASE CONNECTION SETUP
echo =================================
echo.

echo CURRENT DATABASE STATUS:
echo ======================
if exist "database.db" (
    echo ✓ SQLite database.db file found
    for %%A in (database.db) do echo   Size: %%~zA bytes
) else (
    echo ✗ SQLite database.db file NOT found
)

if exist "FirebirdSql.Data.FirebirdClient.dll" (
    echo ✓ Firebird SQL Client library found
) else (
    echo ✗ Firebird SQL Client library NOT found
)

echo.
echo DATABASE OPTIONS:
echo ================
echo 1. Use existing SQLite database (database.db)
echo 2. Create new Firebird database
echo 3. Connect to existing Firebird database
echo 4. Exit
echo.

choice /c 1234 /m "Select database option"
if errorlevel 4 goto :exit
if errorlevel 3 goto :connect_firebird
if errorlevel 2 goto :create_firebird
if errorlevel 1 goto :use_sqlite

:use_sqlite
cls
echo USING SQLITE DATABASE
echo ====================
echo.
echo To use the existing SQLite database:
echo 1. Install System.Data.SQLite using NuGet:
echo    Install-Package System.Data.SQLite
echo.
echo 2. Or download System.Data.SQLite from:
echo    https://system.data.sqlite.org/
echo.
echo 3. Extract the files to your application directory
echo.
echo 4. Update the DataAccessLayer.cs and DataAccessLayer.vb files
echo    to use SQLiteConnection instead of FbConnection
echo.
echo 5. Update connection strings to:
echo    "Data Source=database.db;Version=3;"
echo.
goto :end

:create_firebird
cls
echo CREATING NEW FIREBIRD DATABASE
echo =============================
echo.
echo To create a new Firebird database:
echo 1. Download and install Firebird Server from:
echo    https://firebirdsql.org/en/server-packages/
echo.
echo 2. Open Firebird ISQL tool:
echo    isql -u SYSDBA -p masterkey
echo.
echo 3. Create database:
echo    CREATE DATABASE 'sky_casa.fdb' USER 'SYSDBA' PASSWORD 'masterkey';
echo.
echo 4. Exit ISQL:
echo    EXIT;
echo.
echo 5. Update application connection strings to:
echo    "User=SYSDBA;Password=masterkey;Database=sky_casa.fdb;DataSource=localhost;Port=3050;"
echo.
goto :end

:connect_firebird
cls
echo CONNECTING TO EXISTING FIREBIRD DATABASE
echo ======================================
echo.
echo To connect to an existing Firebird database:
echo 1. Copy your .fdb file to this directory
echo.
echo 2. Rename it to sky_casa.fdb (or update connection strings)
echo.
echo 3. Update application connection strings to:
echo    "User=SYSDBA;Password=masterkey;Database=sky_casa.fdb;DataSource=localhost;Port=3050;"
echo.
echo 4. If using embedded Firebird, use:
echo    "User=SYSDBA;Password=masterkey;Database=sky_casa.fdb;ServerType=1;"
echo.
goto :end

:exit
echo.
echo Exiting setup...
echo.

:end
echo.
echo For detailed information, see DATABASE_CONNECTION_GUIDE.txt
echo.
pause