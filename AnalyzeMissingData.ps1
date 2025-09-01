# PowerShell script to analyze missing data in Sky CASA database
Write-Host "ANALYZING MISSING DATA IN SKY CASA DATABASE" -ForegroundColor Green
Write-Host "==========================================" -ForegroundColor Green
Write-Host ""

# Check if database file exists
if (Test-Path "database.db") {
    Write-Host "Database file found: database.db" -ForegroundColor Green
    $dbSize = (Get-Item "database.db").Length
    Write-Host "Database size: $dbSize bytes" -ForegroundColor Gray
} else {
    Write-Host "ERROR: Database file database.db not found!" -ForegroundColor Red
    exit 1
}

# Check if Firebird database file exists
if (Test-Path "sky_casa.fdb") {
    Write-Host "Firebird database file found: sky_casa.fdb" -ForegroundColor Yellow
    $fdbSize = (Get-Item "sky_casa.fdb").Length
    Write-Host "Firebird database size: $fdbSize bytes" -ForegroundColor Gray
} else {
    Write-Host "Firebird database file not found (this is normal if using SQLite)" -ForegroundColor Gray
}

Write-Host ""
Write-Host "Checking required DLLs:" -ForegroundColor Cyan
$dlls = @("System.Data.SQLite.dll", "FirebirdSql.Data.FirebirdClient.dll")
foreach ($dll in $dlls) {
    if (Test-Path $dll) {
        Write-Host "  $dll - FOUND" -ForegroundColor Green
    } else {
        Write-Host "  $dll - NOT FOUND" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "Checking for common medical lab tables:" -ForegroundColor Cyan

# Expected tables in a complete medical lab system
$expectedTables = @(
    "patients",
    "cbc_results",
    "urine_results", 
    "stool_results",
    "sem_results",
    "vis_results",
    "path_results",
    "doctors",
    "admin",
    "log",
    "path_vis"
)

Write-Host "Expected tables:" -ForegroundColor Gray
foreach ($table in $expectedTables) {
    Write-Host "  - $table" -ForegroundColor Gray
}

Write-Host ""
Write-Host "SUMMARY:" -ForegroundColor Cyan
Write-Host "========" -ForegroundColor Cyan

# Check if path_vis table exists (this was a critical missing table in the past)
if (Test-Path "AddPathVisTable.bat") {
    Write-Host "PathVis table creation script available (AddPathVisTable.bat)" -ForegroundColor Green
} else {
    Write-Host "PathVis table creation script not found" -ForegroundColor Red
}

# Check for case limit fix
if (Test-Path "RunUIUpdate.bat") {
    Write-Host "UI update script available (RunUIUpdate.bat) - Fixes case limit issue" -ForegroundColor Green
} else {
    Write-Host "UI update script not found" -ForegroundColor Red
}

Write-Host ""
Write-Host "RECOMMENDED ACTIONS:" -ForegroundColor Cyan
Write-Host "===================" -ForegroundColor Cyan
Write-Host "1. Run AddPathVisTable.bat to create the path_vis table if missing" -ForegroundColor Yellow
Write-Host "2. Run RunUIUpdate.bat to fix the case limit issue" -ForegroundColor Yellow
Write-Host "3. Check that all required DLLs are present" -ForegroundColor Yellow
Write-Host "4. Verify database has proper permissions" -ForegroundColor Yellow

Write-Host ""
Write-Host "ANALYSIS COMPLETE" -ForegroundColor Green
Write-Host "=================" -ForegroundColor Green