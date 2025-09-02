@echo off
cls
echo SKY CASA COMPLETE DATABASE SETUP
echo ===============================
echo.

echo 1. VERIFYING REQUIRED COMPONENTS...
echo ----------------------------------
if exist "FirebirdSql.Data.FirebirdClient.dll" (
    echo   ✓ Firebird SQL Client - Found
) else (
    echo   ✗ Firebird SQL Client - Missing
    echo.
    echo ERROR: Required components are missing. Cannot proceed with setup.
    echo Please run VERIFY_ALL_FIXES.bat to check all components.
    echo.
    pause
    exit /b 1
)

if exist "database.db" (
    echo   ✓ SQLite database (source) - Found
) else (
    echo   ✗ SQLite database (source) - Missing
    echo.
    echo WARNING: SQLite database not found, but continuing with setup.
    echo.
)

echo.
echo 2. CREATING FIREBIRD DATABASE...
echo --------------------------------

set DB_NAME=sky_casa.fdb
echo   Database name: %DB_NAME%
echo   Connection string: User=SYSDBA;Password=masterkey;Database=%DB_NAME%;DataSource=localhost;Port=3050;
echo.

if exist "%DB_NAME%" (
    echo   Database already exists. Skipping creation.
) else (
    echo   Creating Firebird database placeholder...
    echo SKY CASA FIREBIRD DATABASE > "%DB_NAME%"
    echo ======================== >> "%DB_NAME%"
    echo. >> "%DB_NAME%"
    echo This is a placeholder for the Firebird database file. >> "%DB_NAME%"
    echo In a production environment, this would be created with proper schema. >> "%DB_NAME%"
    echo. >> "%DB_NAME%"
    echo Database Configuration: >> "%DB_NAME%"
    echo - Name: %DB_NAME% >> "%DB_NAME%"
    echo - User: SYSDBA >> "%DB_NAME%"
    echo - Password: masterkey >> "%DB_NAME%"
    echo - Server: localhost >> "%DB_NAME%"
    echo - Port: 3050 >> "%DB_NAME%"
    echo. >> "%DB_NAME%"
    echo   ✓ Firebird database placeholder created: %DB_NAME%
)

echo.
echo 3. CREATING DATABASE CONFIGURATION...
echo ------------------------------------

if exist "Sky_CASA.exe.config" (
    copy "Sky_CASA.exe.config" "Sky_CASA.exe.config.backup" >nul
    echo   ✓ Backed up existing configuration
)

echo ^<?xml version="1.0" encoding="utf-8"?^> > "Sky_CASA.exe.config"
echo ^<configuration^> >> "Sky_CASA.exe.config"
echo     ^<appSettings^> >> "Sky_CASA.exe.config"
echo         ^<!-- Database Configuration --^> >> "Sky_CASA.exe.config"
echo         ^<add key="DatabaseType" value="Firebird" /^> >> "Sky_CASA.exe.config"
echo         ^<add key="ConnectionString" value="User=SYSDBA;Password=masterkey;Database=sky_casa.fdb;DataSource=localhost;Port=3050;" /^> >> "Sky_CASA.exe.config"
echo. >> "Sky_CASA.exe.config"
echo         ^<!-- Alternative SQLite Connection ^(if needed^) --^> >> "Sky_CASA.exe.config"
echo         ^<add key="SQLiteConnectionString" value="Data Source=database.db;Version=3;" /^> >> "Sky_CASA.exe.config"
echo     ^</appSettings^> >> "Sky_CASA.exe.config"
echo. >> "Sky_CASA.exe.config"
echo     ^<system.windows.forms jitDebugging="true" /^> >> "Sky_CASA.exe.config"
echo     ^<runtime^> >> "Sky_CASA.exe.config"
echo         ^<assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1"^> >> "Sky_CASA.exe.config"
echo             ^<dependentAssembly^> >> "Sky_CASA.exe.config"
echo                 ^<assemblyIdentity name="AForge.Video.DirectShow" >> "Sky_CASA.exe.config"
echo                                   publicKeyToken="61ea4348d43881b7" >> "Sky_CASA.exe.config"
echo                                   culture="neutral" /^> >> "Sky_CASA.exe.config"
echo                 ^<bindingRedirect oldVersion="0.0.0.0-2.2.5.0" >> "Sky_CASA.exe.config"
echo                                  newVersion="2.2.5.0" /^> >> "Sky_CASA.exe.config"
echo             ^</dependentAssembly^> >> "Sky_CASA.exe.config"
echo             ^<dependentAssembly^> >> "Sky_CASA.exe.config"
echo                 ^<assemblyIdentity name="AForge" >> "Sky_CASA.exe.config"
echo                                   publicKeyToken="61ea4348d43881b7" >> "Sky_CASA.exe.config"
echo                                   culture="neutral" /^> >> "Sky_CASA.exe.config"
echo                 ^<bindingRedirect oldVersion="0.0.0.0-2.2.5.0" >> "Sky_CASA.exe.config"
echo                                  newVersion="2.2.5.0" /^> >> "Sky_CASA.exe.config"
echo             ^</dependentAssembly^> >> "Sky_CASA.exe.config"
echo             ^<dependentAssembly^> >> "Sky_CASA.exe.config"
echo                 ^<assemblyIdentity name="AForge.Video" >> "Sky_CASA.exe.config"
echo                                   publicKeyToken="61ea4348d43881b7" >> "Sky_CASA.exe.config"
echo                                   culture="neutral" /^> >> "Sky_CASA.exe.config"
echo                 ^<bindingRedirect oldVersion="0.0.0.0-2.2.5.0" >> "Sky_CASA.exe.config"
echo                                  newVersion="2.2.5.0" /^> >> "Sky_CASA.exe.config"
echo             ^</dependentAssembly^> >> "Sky_CASA.exe.config"
echo             ^<dependentAssembly^> >> "Sky_CASA.exe.config"
echo                 ^<assemblyIdentity name="FirebirdSql.Data.FirebirdClient" >> "Sky_CASA.exe.config"
echo                                   publicKeyToken="3750abcc3150b00c" >> "Sky_CASA.exe.config"
echo                                   culture="neutral" /^> >> "Sky_CASA.exe.config"
echo                 ^<bindingRedirect oldVersion="0.0.0.0-10.3.1.0" >> "Sky_CASA.exe.config"
echo                                  newVersion="10.3.1.0" /^> >> "Sky_CASA.exe.config"
echo             ^</dependentAssembly^> >> "Sky_CASA.exe.config"
echo             ^<dependentAssembly^> >> "Sky_CASA.exe.config"
echo                 ^<assemblyIdentity name="System.Threading.Tasks.Extensions" >> "Sky_CASA.exe.config"
echo                                   publicKeyToken="cc7b13ffcd2ddd51" >> "Sky_CASA.exe.config"
echo                                   culture="neutral" /^> >> "Sky_CASA.exe.config"
echo                 ^<bindingRedirect oldVersion="0.0.0.0-4.2.0.1" >> "Sky_CASA.exe.config"
echo                                  newVersion="4.2.0.1" /^> >> "Sky_CASA.exe.config"
echo             ^</dependentAssembly^> >> "Sky_CASA.exe.config"
echo         ^</assemblyBinding^> >> "Sky_CASA.exe.config"
echo     ^</runtime^> >> "Sky_CASA.exe.config"
echo ^</configuration^> >> "Sky_CASA.exe.config"

echo   ✓ Database configuration created

echo.
echo 4. CREATING DATABASE CONNECTION TEST...
echo --------------------------------------

echo using System; > "TestDatabaseConnection.cs"
echo using System.IO; >> "TestDatabaseConnection.cs"
echo using FirebirdSql.Data.FirebirdClient; >> "TestDatabaseConnection.cs"
echo. >> "TestDatabaseConnection.cs"
echo class DatabaseConnectionTest >> "TestDatabaseConnection.cs"
echo { >> "TestDatabaseConnection.cs"
echo     static void Main^(^) >> "TestDatabaseConnection.cs"
echo     { >> "TestDatabaseConnection.cs"
echo         Console.WriteLine^("SKY CASA DATABASE CONNECTION TEST"^); >> "TestDatabaseConnection.cs"
echo         Console.WriteLine^("================================="^); >> "TestDatabaseConnection.cs"
echo         Console.WriteLine^(""^); >> "TestDatabaseConnection.cs"
echo. >> "TestDatabaseConnection.cs"
echo         // Test Firebird connection >> "TestDatabaseConnection.cs"
echo         TestFirebirdConnection^(^); >> "TestDatabaseConnection.cs"
echo. >> "TestDatabaseConnection.cs"
echo         Console.WriteLine^(^); >> "TestDatabaseConnection.cs"
echo         Console.WriteLine^("Press any key to exit..."^); >> "TestDatabaseConnection.cs"
echo         Console.ReadKey^(^); >> "TestDatabaseConnection.cs"
echo     } >> "TestDatabaseConnection.cs"
echo. >> "TestDatabaseConnection.cs"
echo     static void TestFirebirdConnection^(^) >> "TestDatabaseConnection.cs"
echo     { >> "TestDatabaseConnection.cs"
echo         Console.WriteLine^("Testing Firebird Connection:"^); >> "TestDatabaseConnection.cs"
echo         Console.WriteLine^("---------------------------"^); >> "TestDatabaseConnection.cs"
echo. >> "TestDatabaseConnection.cs"
echo         try >> "TestDatabaseConnection.cs"
echo         { >> "TestDatabaseConnection.cs"
echo             // Try to load Firebird assembly >> "TestDatabaseConnection.cs"
echo             var assembly = System.Reflection.Assembly.LoadFrom^("FirebirdSql.Data.FirebirdClient.dll"^); >> "TestDatabaseConnection.cs"
echo             Console.WriteLine^("✓ Firebird client assembly loaded successfully"^); >> "TestDatabaseConnection.cs"
echo. >> "TestDatabaseConnection.cs"
echo             // Try to create connection object >> "TestDatabaseConnection.cs"
echo             var connectionType = assembly.GetType^("FirebirdSql.Data.FirebirdClient.FbConnection"^); >> "TestDatabaseConnection.cs"
echo             if ^(connectionType != null^) >> "TestDatabaseConnection.cs"
echo             { >> "TestDatabaseConnection.cs"
echo                 Console.WriteLine^("✓ Firebird connection type available"^); >> "TestDatabaseConnection.cs"
echo                 Console.WriteLine^("  Connection string: User=SYSDBA;Password=masterkey;Database=sky_casa.fdb;DataSource=localhost;Port=3050;"^); >> "TestDatabaseConnection.cs"
echo                 Console.WriteLine^(""\^); >> "TestDatabaseConnection.cs"
echo                 Console.WriteLine^("NOTE: This is a verification that the components are in place."\^); >> "TestDatabaseConnection.cs"
echo                 Console.WriteLine^("To test actual database connectivity, you need to:"\^); >> "TestDatabaseConnection.cs"
echo                 Console.WriteLine^("1. Install Firebird Server"\^); >> "TestDatabaseConnection.cs"
echo                 Console.WriteLine^("2. Create a proper database with ISQL:"\^); >> "TestDatabaseConnection.cs"
echo                 Console.WriteLine^("   isql -u SYSDBA -p masterkey"\^); >> "TestDatabaseConnection.cs"
echo                 Console.WriteLine^("   CREATE DATABASE 'sky_casa.fdb' USER 'SYSDBA' PASSWORD 'masterkey';"\^); >> "TestDatabaseConnection.cs"
echo                 Console.WriteLine^("3. Create tables based on the schema from database.db"\^); >> "TestDatabaseConnection.cs"
echo                 Console.WriteLine^("4. Run TestDatabaseConnection.exe to verify connectivity"\^); >> "TestDatabaseConnection.cs"
echo                 Console.WriteLine^("5. Run the main application using Run_Sky_CASA.bat"\^); >> "TestDatabaseConnection.cs"
echo             } >> "TestDatabaseConnection.cs"
echo             else >> "TestDatabaseConnection.cs"
echo             { >> "TestDatabaseConnection.cs"
echo                 Console.WriteLine^("✗ Firebird connection type not found"^); >> "TestDatabaseConnection.cs"
echo             } >> "TestDatabaseConnection.cs"
echo         } >> "TestDatabaseConnection.cs"
echo         catch ^(Exception ex^) >> "TestDatabaseConnection.cs"
echo         { >> "TestDatabaseConnection.cs"
echo             Console.WriteLine^("✗ Firebird connection test failed: " + ex.Message^); >> "TestDatabaseConnection.cs"
echo         } >> "TestDatabaseConnection.cs"
echo     } >> "TestDatabaseConnection.cs"
echo } >> "TestDatabaseConnection.cs"

echo   ✓ Database connection test application created

echo.
echo 5. COMPILING TEST APPLICATION...
echo --------------------------------

echo   To compile the test application, you need to use the .NET compiler:
echo   csc.exe /reference:FirebirdSql.Data.FirebirdClient.dll TestDatabaseConnection.cs
echo.
echo   Or you can compile it from within Visual Studio if available.

echo.
echo COMPLETE DATABASE SETUP FINISHED
echo ================================
echo.
echo ✓ All required components verified
echo ✓ Firebird database placeholder created
echo ✓ Database configuration updated
echo ✓ Connection test application created
echo.
echo NEXT STEPS:
echo ===========
echo 1. Install Firebird Server from https://firebirdsql.org/en/server-packages/
echo 2. Create a proper database using ISQL:
echo    isql -u SYSDBA -p masterkey
echo    CREATE DATABASE 'sky_casa.fdb' USER 'SYSDBA' PASSWORD 'masterkey';
echo 3. Create tables based on the schema from database.db
echo 4. Compile TestDatabaseConnection.cs with:
echo    csc.exe /reference:FirebirdSql.Data.FirebirdClient.dll TestDatabaseConnection.cs
echo 5. Run TestDatabaseConnection.exe to verify connectivity
echo 6. Run the main application using Run_Sky_CASA.bat
echo.
echo For detailed instructions, see DATABASE_CONNECTION_GUIDE.txt
echo.
pause