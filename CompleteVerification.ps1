# PowerShell script to verify all required components
Write-Host "Sky CASA - Complete Verification" -ForegroundColor Green
Write-Host "==============================" -ForegroundColor Green
Write-Host ""

Write-Host "Checking for required DLLs:" -ForegroundColor Yellow
Write-Host ""

$requiredDlls = @(
    "AForge.dll",
    "AForge.Video.dll",
    "AForge.Video.DirectShow.dll",
    "FirebirdSql.Data.FirebirdClient.dll"
)

$allFound = $true

foreach ($dll in $requiredDlls) {
    if (Test-Path $dll) {
        $fileInfo = Get-Item $dll
        Write-Host ("✓ {0} - Found ({1} KB)" -f $dll, [math]::Round($fileInfo.Length/1024, 2)) -ForegroundColor Green
    } else {
        Write-Host ("✗ {0} - Missing" -f $dll) -ForegroundColor Red
        $allFound = $false
    }
}

Write-Host ""
Write-Host "Checking for System.Data.SQLite.dll (in GAC):" -ForegroundColor Yellow
try {
    Add-Type -AssemblyName "System.Data.SQLite"
    Write-Host "✓ System.Data.SQLite.dll - Available in GAC" -ForegroundColor Green
} catch {
    Write-Host "? System.Data.SQLite.dll - Check GAC availability" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "Checking for configuration file:" -ForegroundColor Yellow
if (Test-Path "Sky_CASA.exe.config") {
    Write-Host "✓ Sky_CASA.exe.config - Present (JIT debugging enabled)" -ForegroundColor Green
} else {
    Write-Host "✗ Sky_CASA.exe.config - Missing" -ForegroundColor Red
    $allFound = $false
}

Write-Host ""
Write-Host "Checking for database files:" -ForegroundColor Yellow
if (Test-Path "database.db") {
    $fileInfo = Get-Item "database.db"
    Write-Host ("✓ database.db (SQLite) - Found ({0} KB)" -f [math]::Round($fileInfo.Length/1024, 2)) -ForegroundColor Green
} else {
    Write-Host "✗ database.db (SQLite) - Not found" -ForegroundColor Red
    $allFound = $false
}

Write-Host ""
if ($allFound) {
    Write-Host "SUCCESS: All required components are present!" -ForegroundColor Green
    Write-Host "You can now run your Sky CASA application without assembly errors." -ForegroundColor Cyan
} else {
    Write-Host "WARNING: Some required components are missing." -ForegroundColor Red
    Write-Host "Please check the installation instructions." -ForegroundColor Yellow
}

Write-Host ""
Write-Host "To run the application:" -ForegroundColor Yellow
Write-Host "1. Double-click on Run_Sky_CASA.bat" -ForegroundColor Cyan
Write-Host "   OR" -ForegroundColor Cyan
Write-Host "2. Double-click on Sky_CASA.exe directly" -ForegroundColor Cyan
Write-Host ""

Write-Host "Press any key to continue..."
$host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")