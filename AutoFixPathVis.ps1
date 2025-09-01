# Auto-fix script for path_vis table
Write-Host "AUTO FIX: Creating path_vis table in database..." -ForegroundColor Green
Write-Host "==============================================" -ForegroundColor Green
Write-Host ""

# Check if database exists
$dbPath = "database.db"
if (-not (Test-Path $dbPath)) {
    Write-Host "ERROR: Database file not found at $dbPath" -ForegroundColor Red
    exit 1
}

Write-Host "✓ Database file found" -ForegroundColor Green

try {
    # Try to create the table directly using .NET SQLite functionality
    Add-Type -AssemblyName System.Data
    
    # Create connection
    $connectionString = "Data Source=$dbPath;Version=3;"
    $connection = New-Object System.Data.SQLite.SQLiteConnection($connectionString)
    $connection.Open()
    
    Write-Host "✓ Database connection successful" -ForegroundColor Green
    
    # Check if path_vis table already exists
    $checkTableCmd = $connection.CreateCommand()
    $checkTableCmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='path_vis'"
    $reader = $checkTableCmd.ExecuteReader()
    $tableExists = $reader.HasRows
    $reader.Close()
    
    if ($tableExists) {
        Write-Host "✓ path_vis table already exists - no action needed" -ForegroundColor Green
    } else {
        Write-Host "⚠ path_vis table not found - creating it now..." -ForegroundColor Yellow
        
        # Create the path_vis table
        $createTableCmd = $connection.CreateCommand()
        $createTableCmd.CommandText = @"
            CREATE TABLE path_vis (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                path_id INTEGER,
                x_coordinate REAL,
                y_coordinate REAL,
                timestamp DATETIME DEFAULT CURRENT_TIMESTAMP,
                visibility_status INTEGER DEFAULT 1,
                color TEXT,
                line_width REAL DEFAULT 1.0,
                FOREIGN KEY (path_id) REFERENCES path(id)
            );
            
            CREATE INDEX idx_path_vis_path_id ON path_vis(path_id);
            CREATE INDEX idx_path_vis_timestamp ON path_vis(timestamp);
"@
        
        $createTableCmd.ExecuteNonQuery()
        
        Write-Host "✓ path_vis table created successfully!" -ForegroundColor Green
        
        # Verify creation
        $verifyCmd = $connection.CreateCommand()
        $verifyCmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='path_vis'"
        $reader = $verifyCmd.ExecuteReader()
        if ($reader.HasRows) {
            Write-Host "✓ VERIFICATION: path_vis table confirmed in database!" -ForegroundColor Green
        } else {
            Write-Host "✗ VERIFICATION FAILED: path_vis table not found after creation" -ForegroundColor Red
        }
        $reader.Close()
    }
    
    # List all tables
    Write-Host ""
    Write-Host "Current database structure:" -ForegroundColor Cyan
    Write-Host "=========================" -ForegroundColor Cyan
    $listTablesCmd = $connection.CreateCommand()
    $listTablesCmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' ORDER BY name"
    $reader = $listTablesCmd.ExecuteReader()
    while ($reader.Read()) {
        $tableName = $reader.GetString(0)
        Write-Host "- $tableName"
    }
    $reader.Close()
    
    $connection.Close()
    
    Write-Host ""
    Write-Host "AUTO FIX COMPLETE!" -ForegroundColor Green
    Write-Host "The 'no such table: path_vis' error should now be resolved." -ForegroundColor Cyan
    
} catch {
    Write-Host "ERROR: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "You may need to manually create the table using a SQLite browser tool." -ForegroundColor Yellow
    Write-Host "Please refer to FINAL_PATH_VIS_SOLUTION.txt for manual instructions." -ForegroundColor Yellow
}