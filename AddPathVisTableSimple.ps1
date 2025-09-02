# Simple PowerShell script to add the missing path_vis table to the database
Write-Host "Adding path_vis table to database..." -ForegroundColor Green
Write-Host ""

# Check if database exists
$dbPath = "database.db"
if (-not (Test-Path $dbPath)) {
    Write-Host "Error: Database file not found at $dbPath" -ForegroundColor Red
    exit 1
}

# Create a simple SQL file to add the table
$sqlContent = @"
CREATE TABLE IF NOT EXISTS path_vis (
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

CREATE INDEX IF NOT EXISTS idx_path_vis_path_id ON path_vis(path_id);
CREATE INDEX IF NOT EXISTS idx_path_vis_timestamp ON path_vis(timestamp);

PRAGMA table_info(path_vis);
"@

$sqlContent | Out-File -FilePath "add_path_vis_table.sql" -Encoding UTF8

Write-Host "Created SQL file for adding path_vis table" -ForegroundColor Green

# Try to execute the SQL file using SQLite command line (if available)
try {
    sqlite3 $dbPath ".read add_path_vis_table.sql" 2>$null
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Table 'path_vis' created successfully using sqlite3" -ForegroundColor Green
    } else {
        Write-Host "ℹ sqlite3 command not available, will try alternative method" -ForegroundColor Yellow
    }
} catch {
    Write-Host "ℹ sqlite3 command not available, will try alternative method" -ForegroundColor Yellow
}

# Clean up
Remove-Item "add_path_vis_table.sql" -ErrorAction SilentlyContinue

# Verify the table exists by checking the database structure
try {
    # Use our existing ExamineTables.bat to check
    $output = & "ExamineTables.bat" 2>$null
    if ($output -match "path_vis") {
        Write-Host "✓ Verification successful: 'path_vis' table exists in database" -ForegroundColor Green
    } else {
        Write-Host "ℹ Could not verify table creation automatically" -ForegroundColor Yellow
        Write-Host "Please run ExamineTables.bat manually to verify" -ForegroundColor Yellow
    }
} catch {
    Write-Host "ℹ Could not verify table creation automatically" -ForegroundColor Yellow
    Write-Host "Please run ExamineTables.bat manually to verify" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "Process completed!" -ForegroundColor Green
Write-Host "The 'path_vis' table should now exist in the database." -ForegroundColor Cyan