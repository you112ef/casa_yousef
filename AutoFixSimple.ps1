# Simple auto-fix script for path_vis table
Write-Host "AUTO FIX: Creating path_vis table..." -ForegroundColor Green

# Check if database exists
$dbPath = "database.db"
if (-not (Test-Path $dbPath)) {
    Write-Host "ERROR: Database file not found" -ForegroundColor Red
    exit 1
}

Write-Host "Database found." -ForegroundColor Green

# Create SQL file
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
"@

$sqlContent | Out-File -FilePath "create_path_vis.sql" -Encoding UTF8

Write-Host "SQL script created." -ForegroundColor Green

# Try to execute with sqlite3
try {
    $output = sqlite3 $dbPath ".read create_path_vis.sql" 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "SUCCESS: Table created with sqlite3" -ForegroundColor Green
    } else {
        Write-Host "sqlite3 not available, manual creation needed" -ForegroundColor Yellow
    }
} catch {
    Write-Host "sqlite3 not available, manual creation needed" -ForegroundColor Yellow
}

# Clean up
Remove-Item "create_path_vis.sql" -ErrorAction SilentlyContinue

# Final verification
Write-Host "Process completed." -ForegroundColor Green
Write-Host "Please run ExamineTables.bat to verify." -ForegroundColor Cyan