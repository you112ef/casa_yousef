# PowerShell script to add the missing path_vis table to the database using .NET reflection
Write-Host "Adding path_vis table to database using .NET reflection..." -ForegroundColor Green
Write-Host ""

# Check if database exists
$dbPath = "database.db"
if (-not (Test-Path $dbPath)) {
    Write-Host "Error: Database file not found at $dbPath" -ForegroundColor Red
    exit 1
}

try {
    # Try to load SQLite assembly
    try {
        Add-Type -AssemblyName System.Data.SQLite -ErrorAction Stop
        Write-Host "✓ System.Data.SQLite assembly loaded successfully" -ForegroundColor Green
    } catch {
        Write-Host "ℹ System.Data.SQLite assembly not found, will try to load from file" -ForegroundColor Yellow
        
        # Try to find SQLite DLL in the directory
        $sqliteDll = Get-ChildItem -Path "." -Recurse -Name "*System.Data.SQLite*" -ErrorAction SilentlyContinue | Where-Object { $_ -like "*.dll" } | Select-Object -First 1
        
        if ($sqliteDll) {
            Write-Host "Found SQLite DLL: $sqliteDll" -ForegroundColor Green
            Add-Type -Path $sqliteDll -ErrorAction Stop
            Write-Host "✓ SQLite DLL loaded successfully" -ForegroundColor Green
        } else {
            Write-Host "ℹ SQLite DLL not found, will try to use built-in .NET SQLite support" -ForegroundColor Yellow
            
            # Try to use the built-in .NET SQLite support (if available in .NET 6+)
            try {
                Add-Type -AssemblyName Microsoft.Data.Sqlite -ErrorAction Stop
                Write-Host "✓ Microsoft.Data.Sqlite assembly loaded successfully" -ForegroundColor Green
                $useMicrosoftDataSqlite = $true
            } catch {
                Write-Host "✗ Could not load any SQLite assembly" -ForegroundColor Red
                Write-Host "Please install System.Data.SQLite or ensure it's available" -ForegroundColor Red
                exit 1
            }
        }
    }
    
    # Create connection
    if ($useMicrosoftDataSqlite) {
        $connectionString = "Data Source=$dbPath"
        $connectionType = [Microsoft.Data.Sqlite.SqliteConnection]
    } else {
        $connectionString = "Data Source=$dbPath;Version=3;"
        $connectionType = [System.Data.SQLite.SQLiteConnection]
    }
    
    $connection = New-Object $connectionType($connectionString)
    $connection.Open()
    
    Write-Host "✓ Database connection successful!" -ForegroundColor Green
    Write-Host ""
    
    # Check if path_vis table already exists
    $checkTableCmd = $connection.CreateCommand()
    $checkTableCmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='path_vis'"
    $reader = $checkTableCmd.ExecuteReader()
    $tableExists = $reader.HasRows
    $reader.Close()
    
    if ($tableExists) {
        Write-Host "ℹ Table 'path_vis' already exists in the database." -ForegroundColor Yellow
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
        
        Write-Host "✓ Table 'path_vis' created successfully!" -ForegroundColor Green
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
    $listTablesCmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' ORDER BY name"
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