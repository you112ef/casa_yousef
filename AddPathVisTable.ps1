# PowerShell script to add the missing path_vis table to the database
Write-Host "Adding path_vis table to database..." -ForegroundColor Green
Write-Host ""

# Check if database exists
$dbPath = "database.db"
if (-not (Test-Path $dbPath)) {
    Write-Host "Error: Database file not found at $dbPath" -ForegroundColor Red
    exit 1
}

try {
    # Load System.Data assembly
    Add-Type -AssemblyName System.Data
    
    # Create connection
    $connectionString = "Data Source=$dbPath;Version=3;"
    $connection = New-Object System.Data.SQLite.SQLiteConnection($connectionString)
    $connection.Open()
    
    Write-Host "Database connection successful!" -ForegroundColor Green
    Write-Host ""
    
    # Check if path_vis table already exists
    $checkTableCmd = $connection.CreateCommand()
    $checkTableCmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='path_vis'"
    $reader = $checkTableCmd.ExecuteReader()
    $tableExists = $reader.HasRows
    $reader.Close()
    
    if ($tableExists) {
        Write-Host "Table 'path_vis' already exists in the database." -ForegroundColor Yellow
        Write-Host "No action needed." -ForegroundColor Yellow
    } else {
        Write-Host "Creating 'path_vis' table..." -ForegroundColor Cyan
        
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
        
        Write-Host "Table 'path_vis' created successfully!" -ForegroundColor Green
        Write-Host ""
        
        # Verify the table was created
        $verifyCmd = $connection.CreateCommand()
        $verifyCmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='path_vis'"
        $reader = $verifyCmd.ExecuteReader()
        if ($reader.HasRows) {
            Write-Host "✓ Verification successful: 'path_vis' table exists in database" -ForegroundColor Green
        } else {
            Write-Host "✗ Verification failed: 'path_vis' table not found" -ForegroundColor Red
        }
        $reader.Close()
    }
    
    # List all tables to show the updated structure
    Write-Host ""
    Write-Host "Updated database structure:" -ForegroundColor Cyan
    Write-Host "==========================" -ForegroundColor Cyan
    $listTablesCmd = $connection.CreateCommand()
    $listTablesCmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table'"
    $reader = $listTablesCmd.ExecuteReader()
    while ($reader.Read()) {
        $tableName = $reader.GetString(0)
        Write-Host "- $tableName"
    }
    $reader.Close()
    
    $connection.Close()
    
    Write-Host ""
    Write-Host "Process completed successfully!" -ForegroundColor Green
} catch {
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Exception type: $($_.Exception.GetType().FullName)" -ForegroundColor Red
    
    if ($_.Exception.InnerException) {
        Write-Host "Inner exception: $($_.Exception.InnerException.Message)" -ForegroundColor Red
    }
}