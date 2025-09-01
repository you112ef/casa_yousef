# FINAL VERIFICATION SCRIPT FOR SKY CASA APPLICATION
# This script verifies that all fixes have been properly applied

Write-Host "FINAL VERIFICATION OF SKY CASA APPLICATION FIXES" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

# 1. Check for required .NET assemblies
Write-Host "1. VERIFYING .NET ASSEMBLY DEPENDENCIES..." -ForegroundColor Yellow
$requiredAssemblies = @(
    "AForge.dll",
    "AForge.Video.DirectShow.dll",
    "AForge.Video.dll",
    "FirebirdSql.Data.FirebirdClient.dll",
    "System.Threading.Tasks.Extensions.dll"
)

$allAssembliesFound = $true
foreach ($assembly in $requiredAssemblies) {
    if (Test-Path $assembly) {
        Write-Host "  ✓ $assembly - Found" -ForegroundColor Green
    } else {
        Write-Host "  ✗ $assembly - Missing" -ForegroundColor Red
        $allAssembliesFound = $false
    }
}

Write-Host ""

# 2. Check for Visual C++ runtime DLLs
Write-Host "2. VERIFYING NATIVE DEPENDENCIES..." -ForegroundColor Yellow
$requiredNativeDLLs = @(
    "msvcp140.dll",
    "vcruntime140.dll",
    "ucrtbase.dll"
)

$allNativeDLLsFound = $true
foreach ($dll in $requiredNativeDLLs) {
    if (Test-Path $dll) {
        Write-Host "  ✓ $dll - Found" -ForegroundColor Green
    } else {
        Write-Host "  ✗ $dll - Missing" -ForegroundColor Red
        $allNativeDLLsFound = $false
    }
}

Write-Host ""

# 3. Check database structure
Write-Host "3. VERIFYING DATABASE STRUCTURE..." -ForegroundColor Yellow
try {
    Add-Type -AssemblyName "System.Data.SQLite"
    $connectionString = "Data Source=database.db;Version=3;"
    $connection = New-Object System.Data.SQLite.SQLiteConnection($connectionString)
    $connection.Open()
    
    $cmd = $connection.CreateCommand()
    $cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='path_vis'"
    $reader = $cmd.ExecuteReader()
    
    if ($reader.HasRows) {
        Write-Host "  ✓ path_vis table - Found" -ForegroundColor Green
    } else {
        Write-Host "  ✗ path_vis table - Missing" -ForegroundColor Red
    }
    $reader.Close()
    
    $connection.Close()
    Write-Host "  ✓ Database connectivity - Working" -ForegroundColor Green
}
catch {
    Write-Host "  ✗ Database verification failed: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""

# 4. Check configuration file
Write-Host "4. VERIFYING CONFIGURATION FILE..." -ForegroundColor Yellow
if (Test-Path "Sky_CASA.exe.config") {
    $configContent = Get-Content "Sky_CASA.exe.config" -Raw
    if ($configContent -match "jitDebugging.*true") {
        Write-Host "  ✓ JIT debugging enabled - Confirmed" -ForegroundColor Green
    } else {
        Write-Host "  ✗ JIT debugging not enabled" -ForegroundColor Red
    }
    
    $requiredBindings = @(
        "AForge.Video.DirectShow",
        "FirebirdSql.Data.FirebirdClient",
        "System.Threading.Tasks.Extensions"
    )
    
    $allBindingsFound = $true
    foreach ($binding in $requiredBindings) {
        if ($configContent -match $binding) {
            Write-Host "  ✓ $binding binding - Found" -ForegroundColor Green
        } else {
            Write-Host "  ✗ $binding binding - Missing" -ForegroundColor Red
            $allBindingsFound = $false
        }
    }
} else {
    Write-Host "  ✗ Configuration file missing" -ForegroundColor Red
}

Write-Host ""

# Final status
Write-Host "FINAL STATUS:" -ForegroundColor Yellow
Write-Host "=============" -ForegroundColor Yellow

if ($allAssembliesFound -and $allNativeDLLsFound -and $allBindingsFound) {
    Write-Host "  ✓ ALL FIXES APPLIED SUCCESSFULLY" -ForegroundColor Green
    Write-Host "  ✓ Sky CASA application should be fully functional" -ForegroundColor Green
} else {
    Write-Host "  ⚠ Some issues detected - please review the output above" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "RECOMMENDED NEXT STEPS:" -ForegroundColor Cyan
Write-Host "======================" -ForegroundColor Cyan
Write-Host "1. Run the application and test all functionality" -ForegroundColor Gray
Write-Host "2. Verify data entry and database operations work correctly" -ForegroundColor Gray
Write-Host "3. Test path visualization features" -ForegroundColor Gray
Write-Host "4. Check for any runtime errors during use" -ForegroundColor Gray

Write-Host ""
Write-Host "Press any key to exit..."
$host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown") | Out-Null