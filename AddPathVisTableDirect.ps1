# PowerShell script to directly add the path_vis table to the database
Write-Host "Adding path_vis table to database..." -ForegroundColor Green
Write-Host "====================================" -ForegroundColor Green

# Check if database file exists
if (-not (Test-Path "database.db")) {
    Write-Host "Error: Database file not found!" -ForegroundColor Red
    exit 1
}

Write-Host "Database found. Creating path_vis table..." -ForegroundColor Yellow

# SQL to create the path_vis table
$createTableSQL = @"
CREATE TABLE IF NOT EXISTS path_vis (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    path_id INTEGER,
    x_coordinate REAL,
    y_coordinate REAL,
    timestamp DATETIME DEFAULT CURRENT_TIMESTAMP,
    visibility_status INTEGER DEFAULT 1,
    color TEXT,
    line_width REAL DEFAULT 1.0
);

CREATE INDEX IF NOT EXISTS idx_path_vis_path_id ON path_vis(path_id);
CREATE INDEX IF NOT EXISTS idx_path_vis_timestamp ON path_vis(timestamp);
"@

try {
    # Load SQLite assembly
    Add-Type -Path "System.Data.SQLite.dll"
    
    # Create connection
    $connectionString = "Data Source=database.db;Version=3;"
    $connection = New-Object System.Data.SQLite.SQLiteConnection($connectionString)
    $connection.Open()
    
    # Check if path_vis table already exists
    $checkTableSQL = "SELECT name FROM sqlite_master WHERE type='table' AND name='path_vis'"
    $command = New-Object System.Data.SQLite.SQLiteCommand($checkTableSQL, $connection)
    $reader = $command.ExecuteReader()
    $tableExists = $reader.HasRows
    $reader.Close()
    
    if (-not $tableExists) {
        Write-Host "Creating 'path_vis' table..." -ForegroundColor Yellow
        
        # Create the path_vis table
        $command = New-Object System.Data.SQLite.SQLiteCommand($createTableSQL, $connection)
        $command.ExecuteNonQuery()
        
        Write-Host "Table 'path_vis' created successfully!" -ForegroundColor Green
    } else {
        Write-Host "Table 'path_vis' already exists." -ForegroundColor Green
    }
    
    # Close connection
    $connection.Close()
    
    Write-Host "Process completed!" -ForegroundColor Green
} catch {
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    if ($connection) {
        $connection.Close()
    }
    exit 1
}