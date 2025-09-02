# PowerShell script to add missing data to the Sky CASA database
Write-Host "ADDING MISSING DATA TO SKY CASA DATABASE" -ForegroundColor Green
Write-Host "======================================" -ForegroundColor Green
Write-Host ""

# Check if required files exist
$requiredFiles = @("database.db", "System.Data.SQLite.dll")
$missingFiles = @()

foreach ($file in $requiredFiles) {
    if (-not (Test-Path $file)) {
        Write-Host "Missing file: $file" -ForegroundColor Red
        $missingFiles += $file
    }
}

if ($missingFiles.Count -gt 0) {
    Write-Host "Cannot proceed due to missing files." -ForegroundColor Red
    exit 1
}

Write-Host "All required files found." -ForegroundColor Green
Write-Host ""

# Try to load the SQLite assembly
try {
    Add-Type -Path "System.Data.SQLite.dll"
    Write-Host "SQLite assembly loaded successfully." -ForegroundColor Green
} catch {
    Write-Host "Failed to load SQLite assembly: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Function to execute SQL commands
function Execute-SQL {
    param(
        [string]$sql
    )
    
    try {
        $connectionString = "Data Source=database.db;Version=3;"
        $connection = New-Object System.Data.SQLite.SQLiteConnection($connectionString)
        $connection.Open()
        
        $command = New-Object System.Data.SQLite.SQLiteCommand($sql, $connection)
        $result = $command.ExecuteNonQuery()
        
        $connection.Close()
        return $result
    } catch {
        Write-Host "Error executing SQL: $($_.Exception.Message)" -ForegroundColor Red
        if ($connection) { $connection.Close() }
        return -1
    }
}

# Check if path_vis table exists
try {
    $connectionString = "Data Source=database.db;Version=3;"
    $connection = New-Object System.Data.SQLite.SQLiteConnection($connectionString)
    $connection.Open()
    
    $command = New-Object System.Data.SQLite.SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table' AND name='path_vis'", $connection)
    $reader = $command.ExecuteReader()
    $tableExists = $reader.HasRows
    $reader.Close()
    $connection.Close()
    
    if ($tableExists) {
        Write-Host "path_vis table already exists." -ForegroundColor Green
    } else {
        Write-Host "path_vis table is missing. Creating it..." -ForegroundColor Yellow
        
        # Create the path_vis table
        $createTableSQL = @"
CREATE TABLE path_vis (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    path_id INTEGER,
    x_coordinate REAL,
    y_coordinate REAL,
    timestamp DATETIME DEFAULT CURRENT_TIMESTAMP,
    visibility_status INTEGER DEFAULT 1,
    color TEXT,
    line_width REAL DEFAULT 1.0
);

CREATE INDEX idx_path_vis_path_id ON path_vis(path_id);
CREATE INDEX idx_path_vis_timestamp ON path_vis(timestamp);
"@
        
        $result = Execute-SQL $createTableSQL
        if ($result -ge 0) {
            Write-Host "path_vis table created successfully!" -ForegroundColor Green
        } else {
            Write-Host "Failed to create path_vis table." -ForegroundColor Red
        }
    }
} catch {
    Write-Host "Error checking/creating path_vis table: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""
Write-Host "Checking for other common tables..." -ForegroundColor Cyan

# Common tables that should exist in a medical lab database
$commonTables = @("patients", "cbc_results", "urine_results", "stool_results", "sem_results", "vis_results", "path_results", "doctors", "admin", "log")

foreach ($table in $commonTables) {
    try {
        $connectionString = "Data Source=database.db;Version=3;"
        $connection = New-Object System.Data.SQLite.SQLiteConnection($connectionString)
        $connection.Open()
        
        $command = New-Object System.Data.SQLite.SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table' AND name='$table'", $connection)
        $reader = $command.ExecuteReader()
        $exists = $reader.HasRows
        $reader.Close()
        $connection.Close()
        
        if ($exists) {
            Write-Host "  $table - EXISTS" -ForegroundColor Green
        } else {
            Write-Host "  $table - MISSING" -ForegroundColor Yellow
        }
    } catch {
        Write-Host "  $table - ERROR checking" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "ADDING REFERENCE DATA" -ForegroundColor Cyan
Write-Host "====================" -ForegroundColor Cyan

# Add a default admin user if the admin table is empty
try {
    $connectionString = "Data Source=database.db;Version=3;"
    $connection = New-Object System.Data.SQLite.SQLiteConnection($connectionString)
    $connection.Open()
    
    # Check if admin table exists
    $command = New-Object System.Data.SQLite.SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table' AND name='admin'", $connection)
    $reader = $command.ExecuteReader()
    $adminTableExists = $reader.HasRows
    $reader.Close()
    
    if ($adminTableExists) {
        # Check if admin table is empty
        $command = New-Object System.Data.SQLite.SQLiteCommand("SELECT COUNT(*) FROM admin", $connection)
        $count = $command.ExecuteScalar()
        
        if ($count -eq 0) {
            Write-Host "Adding default admin user..." -ForegroundColor Yellow
            $insertSQL = "INSERT INTO admin (username, password, role) VALUES ('admin', 'admin123', 'administrator')"
            $command = New-Object System.Data.SQLite.SQLiteCommand($insertSQL, $connection)
            $command.ExecuteNonQuery()
            Write-Host "Default admin user added." -ForegroundColor Green
        } else {
            Write-Host "Admin table already has data." -ForegroundColor Green
        }
    } else {
        Write-Host "Admin table does not exist." -ForegroundColor Yellow
    }
    
    $connection.Close()
} catch {
    Write-Host "Error adding reference data: $($_.Exception.Message)" -ForegroundColor Red
    if ($connection) { $connection.Close() }
}

Write-Host ""
Write-Host "DATA ADDITION COMPLETE" -ForegroundColor Green
Write-Host "======================" -ForegroundColor Green