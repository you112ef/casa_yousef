# PowerShell script to check if path_vis table exists in the database
Write-Host "Checking for path_vis table in database..." -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""

# Check if database exists
$dbPath = "database.db"
if (-not (Test-Path $dbPath)) {
    Write-Host "Error: Database file not found at $dbPath" -ForegroundColor Red
    exit 1
}

# Try different methods to check for the table
Write-Host "Method 1: Checking sqlite_master..." -ForegroundColor Yellow

try {
    # Try to read the database file directly to look for path_vis
    $content = [System.IO.File]::ReadAllBytes($dbPath)
    $text = [System.Text.Encoding]::ASCII.GetString($content)
    
    if ($text -match "path_vis") {
        Write-Host "✓ Found references to 'path_vis' in database file" -ForegroundColor Green
    } else {
        Write-Host "✗ No references to 'path_vis' found in database file" -ForegroundColor Red
    }
} catch {
    Write-Host "⚠ Could not read database file directly" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "Method 2: Running ExamineTables.bat..." -ForegroundColor Yellow
try {
    $output = & ".\ExamineTables.bat" 2>$null
    if ($output -match "path_vis") {
        Write-Host "✓ ExamineTables.bat found 'path_vis'" -ForegroundColor Green
    } else {
        Write-Host "✗ ExamineTables.bat did not find 'path_vis'" -ForegroundColor Red
    }
} catch {
    Write-Host "⚠ Could not run ExamineTables.bat" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "CONCLUSION:" -ForegroundColor Cyan
Write-Host "===========" -ForegroundColor Cyan
Write-Host "If the 'path_vis' table doesn't exist, please:" -ForegroundColor Yellow
Write-Host "1. Use a SQLite browser tool to open database.db" -ForegroundColor Yellow
Write-Host "2. Execute the add_path_vis_table.sql script" -ForegroundColor Yellow
Write-Host "3. Or run CreatePathVisTable.bat if sqlite3 is available" -ForegroundColor Yellow