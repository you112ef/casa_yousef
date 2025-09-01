# Definitive check for path_vis table
Write-Host "DEFINITIVE TABLE VERIFICATION" -ForegroundColor Green
Write-Host "============================" -ForegroundColor Green
Write-Host ""

# Check if database exists
$dbPath = "database.db"
if (-not (Test-Path $dbPath)) {
    Write-Host "ERROR: Database file not found!" -ForegroundColor Red
    exit 1
}

Write-Host "✓ Database file found" -ForegroundColor Green

try {
    # Try to read the database file directly to look for CREATE TABLE statement
    Write-Host ""
    Write-Host "Method 1: Searching database file content..." -ForegroundColor Yellow
    $content = [System.IO.File]::ReadAllText($dbPath, [System.Text.Encoding]::ASCII)
    
    if ($content -match "path_vis") {
        Write-Host "✓ Found references to 'path_vis' in database file" -ForegroundColor Green
    } else {
        Write-Host "✗ No references to 'path_vis' found in database file" -ForegroundColor Red
    }
    
    # Try to check if we can connect and query
    Write-Host ""
    Write-Host "Method 2: Attempting direct database query..." -ForegroundColor Yellow
    
    # Try to load SQLite assembly
    try {
        Add-Type -AssemblyName System.Data
        $connectionString = "Data Source=$dbPath;Version=3;"
        
        # Create connection using reflection
        $connectionType = [System.Data.SQLite.SQLiteConnection]
        $connection = New-Object $connectionType($connectionString)
        $connection.Open()
        
        Write-Host "✓ Database connection established" -ForegroundColor Green
        
        # Try to query the table
        try {
            $cmd = $connection.CreateCommand()
            $cmd.CommandText = "SELECT COUNT(*) FROM path_vis"
            $result = $cmd.ExecuteScalar()
            Write-Host "✓ path_vis table accessible - query successful" -ForegroundColor Green
        } catch {
            Write-Host "✗ path_vis table NOT accessible: $($_.Exception.Message)" -ForegroundColor Red
        }
        
        # List all tables
        Write-Host ""
        Write-Host "Method 3: Complete table listing..." -ForegroundColor Yellow
        Write-Host "---------------------------------" -ForegroundColor Yellow
        $cmd = $connection.CreateCommand()
        $cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' ORDER BY name"
        $reader = $cmd.ExecuteReader()
        while ($reader.Read()) {
            $tableName = $reader.GetString(0)
            Write-Host "- $tableName"
        }
        $reader.Close()
        
        $connection.Close()
    } catch {
        Write-Host "⚠ Could not establish database connection: $($_.Exception.Message)" -ForegroundColor Yellow
    }
    
} catch {
    Write-Host "ERROR: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""
Write-Host "Verification complete." -ForegroundColor Green