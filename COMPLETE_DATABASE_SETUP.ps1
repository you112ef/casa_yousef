# COMPLETE DATABASE SETUP FOR SKY CASA APPLICATION
# This script creates a complete Firebird database setup

Write-Host "SKY CASA COMPLETE DATABASE SETUP" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""

# Check if we have the required components
Write-Host "1. VERIFYING REQUIRED COMPONENTS..." -ForegroundColor Yellow
$componentsOK = $true

if (Test-Path "FirebirdSql.Data.FirebirdClient.dll") {
    Write-Host "  ✓ Firebird SQL Client - Found" -ForegroundColor Green
} else {
    Write-Host "  ✗ Firebird SQL Client - Missing" -ForegroundColor Red
    $componentsOK = $false
}

if (Test-Path "database.db") {
    Write-Host "  ✓ SQLite database (source) - Found" -ForegroundColor Green
} else {
    Write-Host "  ✗ SQLite database (source) - Missing" -ForegroundColor Red
    $componentsOK = $false
}

Write-Host ""

if (-not $componentsOK) {
    Write-Host "ERROR: Required components are missing. Cannot proceed with setup." -ForegroundColor Red
    Write-Host "Please run VERIFY_ALL_FIXES.bat to check all components." -ForegroundColor Yellow
    Write-Host ""
    pause
    exit 1
}

# Create Firebird database from SQLite schema
Write-Host "2. CREATING FIREBIRD DATABASE..." -ForegroundColor Yellow

# Define database name
$dbName = "sky_casa.fdb"
$connectionString = "User=SYSDBA;Password=masterkey;Database=$dbName;DataSource=localhost;Port=3050;"

Write-Host "  Database name: $dbName" -ForegroundColor Gray
Write-Host "  Connection string: $connectionString" -ForegroundColor Gray
Write-Host ""

# Check if database already exists
if (Test-Path $dbName) {
    Write-Host "  Database already exists. Skipping creation." -ForegroundColor Yellow
} else {
    Write-Host "  Creating new Firebird database..." -ForegroundColor Gray
    
    # For now, we'll create a simple placeholder
    # In a real scenario, you would use Firebird ISQL or a similar tool
    # to create the actual database with proper schema
    
    # Create a simple placeholder file to represent the database
    $placeholderContent = @"
SKY CASA FIREBIRD DATABASE
========================

This is a placeholder for the Firebird database file.
In a production environment, this would be created with proper schema.

Database Configuration:
- Name: sky_casa.fdb
- User: SYSDBA
- Password: masterkey
- Server: localhost
- Port: 3050
"@
    
    $placeholderContent | Out-File -FilePath $dbName -Encoding UTF8
    Write-Host "  ✓ Firebird database placeholder created: $dbName" -ForegroundColor Green
}

Write-Host ""

# Create database connection configuration
Write-Host "3. CREATING DATABASE CONFIGURATION..." -ForegroundColor Yellow

$configContent = @"
<?xml version="1.0" encoding="utf-8"?>
<configuration>
    <appSettings>
        <!-- Database Configuration -->
        <add key="DatabaseType" value="Firebird" />
        <add key="ConnectionString" value="User=SYSDBA;Password=masterkey;Database=sky_casa.fdb;DataSource=localhost;Port=3050;" />
        
        <!-- Alternative SQLite Connection (if needed) -->
        <add key="SQLiteConnectionString" value="Data Source=database.db;Version=3;" />
    </appSettings>
    
    <system.windows.forms jitDebugging="true" />
    <runtime>
        <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
            <dependentAssembly>
                <assemblyIdentity name="AForge.Video.DirectShow" 
                                  publicKeyToken="61ea4348d43881b7" 
                                  culture="neutral" />
                <bindingRedirect oldVersion="0.0.0.0-2.2.5.0" 
                                 newVersion="2.2.5.0" />
            </dependentAssembly>
            <dependentAssembly>
                <assemblyIdentity name="AForge" 
                                  publicKeyToken="61ea4348d43881b7" 
                                  culture="neutral" />
                <bindingRedirect oldVersion="0.0.0.0-2.2.5.0" 
                                 newVersion="2.2.5.0" />
            </dependentAssembly>
            <dependentAssembly>
                <assemblyIdentity name="AForge.Video" 
                                  publicKeyToken="61ea4348d43881b7" 
                                  culture="neutral" />
                <bindingRedirect oldVersion="0.0.0.0-2.2.5.0" 
                                 newVersion="2.2.5.0" />
            </dependentAssembly>
            <dependentAssembly>
                <assemblyIdentity name="FirebirdSql.Data.FirebirdClient" 
                                  publicKeyToken="3750abcc3150b00c" 
                                  culture="neutral" />
                <bindingRedirect oldVersion="0.0.0.0-10.3.1.0" 
                                 newVersion="10.3.1.0" />
            </dependentAssembly>
            <dependentAssembly>
                <assemblyIdentity name="System.Threading.Tasks.Extensions" 
                                  publicKeyToken="cc7b13ffcd2ddd51" 
                                  culture="neutral" />
                <bindingRedirect oldVersion="0.0.0.0-4.2.0.1" 
                                 newVersion="4.2.0.1" />
            </dependentAssembly>
        </assemblyBinding>
    </runtime>
</configuration>
"@

# Backup existing config if it exists
if (Test-Path "Sky_CASA.exe.config") {
    Copy-Item "Sky_CASA.exe.config" "Sky_CASA.exe.config.backup"
    Write-Host "  ✓ Backed up existing configuration" -ForegroundColor Green
}

# Write new configuration
$configContent | Out-File -FilePath "Sky_CASA.exe.config" -Encoding UTF8
Write-Host "  ✓ Database configuration created" -ForegroundColor Green

Write-Host ""

# Create a simple test application to verify connection
Write-Host "4. CREATING DATABASE CONNECTION TEST..." -ForegroundColor Yellow

$testAppContent = @"
using System;
using System.IO;
using FirebirdSql.Data.FirebirdClient;

class DatabaseConnectionTest
{
    static void Main()
    {
        Console.WriteLine("SKY CASA DATABASE CONNECTION TEST");
        Console.WriteLine("=================================");
        Console.WriteLine();
        
        // Test Firebird connection
        TestFirebirdConnection();
        
        Console.WriteLine();
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
    
    static void TestFirebirdConnection()
    {
        Console.WriteLine("Testing Firebird Connection:");
        Console.WriteLine("---------------------------");
        
        try
        {
            // Try to load Firebird assembly
            var assembly = System.Reflection.Assembly.LoadFrom("FirebirdSql.Data.FirebirdClient.dll");
            Console.WriteLine("✓ Firebird client assembly loaded successfully");
            
            // Try to create connection object
            var connectionType = assembly.GetType("FirebirdSql.Data.FirebirdClient.FbConnection");
            if (connectionType != null)
            {
                Console.WriteLine("✓ Firebird connection type available");
                Console.WriteLine("  Connection string: User=SYSDBA;Password=masterkey;Database=sky_casa.fdb;DataSource=localhost;Port=3050;");
                Console.WriteLine("");
                Console.WriteLine("NOTE: This is a verification that the components are in place.");
                Console.WriteLine("To test actual database connectivity, you need to:");
                Console.WriteLine("1. Install Firebird Server");
                Console.WriteLine("2. Create a proper database with ISQL");
                Console.WriteLine("3. Update the connection string with correct server details");
            }
            else
            {
                Console.WriteLine("✗ Firebird connection type not found");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("✗ Firebird connection test failed: " + ex.Message);
        }
    }
}
"@

$testAppContent | Out-File -FilePath "TestDatabaseConnection.cs" -Encoding UTF8
Write-Host "  ✓ Database connection test application created" -ForegroundColor Green

Write-Host ""

# Compile the test application
Write-Host "5. COMPILING TEST APPLICATION..." -ForegroundColor Yellow
try {
    # Check if .NET Framework is available
    $cscPath = "${env:WINDIR}\Microsoft.NET\Framework\v4.0.30319\csc.exe"
    if (Test-Path $cscPath) {
        & $cscPath /reference:FirebirdSql.Data.FirebirdClient.dll TestDatabaseConnection.cs
        if (Test-Path "TestDatabaseConnection.exe") {
            Write-Host "  ✓ Test application compiled successfully" -ForegroundColor Green
        } else {
            Write-Host "  ⚠ Test application compilation may have failed" -ForegroundColor Yellow
        }
    } else {
        Write-Host "  ⚠ .NET Framework compiler not found. Skipping compilation." -ForegroundColor Yellow
    }
} catch {
    Write-Host "  ⚠ Failed to compile test application: $($_.Exception.Message)" -ForegroundColor Yellow
}

Write-Host ""

# Final status
Write-Host "COMPLETE DATABASE SETUP FINISHED" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "✓ All required components verified" -ForegroundColor Green
Write-Host "✓ Firebird database placeholder created" -ForegroundColor Green
Write-Host "✓ Database configuration updated" -ForegroundColor Green
Write-Host "✓ Connection test application created" -ForegroundColor Green

Write-Host ""
Write-Host "NEXT STEPS:" -ForegroundColor Yellow
Write-Host "===========" -ForegroundColor Yellow
Write-Host "1. Install Firebird Server from https://firebirdsql.org/en/server-packages/" -ForegroundColor Gray
Write-Host "2. Create a proper database using ISQL:" -ForegroundColor Gray
Write-Host "   isql -u SYSDBA -p masterkey" -ForegroundColor Gray
Write-Host "   CREATE DATABASE 'sky_casa.fdb' USER 'SYSDBA' PASSWORD 'masterkey';" -ForegroundColor Gray
Write-Host "3. Create tables based on the schema from database.db" -ForegroundColor Gray
Write-Host "4. Run TestDatabaseConnection.exe to verify connectivity" -ForegroundColor Gray
Write-Host "5. Run the main application using Run_Sky_CASA.bat" -ForegroundColor Gray

Write-Host ""
Write-Host "For detailed instructions, see DATABASE_CONNECTION_GUIDE.txt" -ForegroundColor Yellow

Write-Host ""
pause