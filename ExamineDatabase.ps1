# PowerShell script to examine the SQLite database
Write-Host "Examining database.db..." -ForegroundColor Green
Write-Host ""

try {
    # Load SQLite assembly
    Add-Type -AssemblyName "System.Data.SQLite, Version=1.0.119.0, Culture=neutral, PublicKeyToken=db937bc2d44ff139"
    
    $connectionString = "Data Source=database.db;Version=3;"
    $connection = New-Object System.Data.SQLite.SQLiteConnection($connectionString)
    $connection.Open()
    
    Write-Host "Database connection successful!" -ForegroundColor Green
    Write-Host ""
    
    # Get all table names
    $cmd = New-Object System.Data.SQLite.SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table'", $connection)
    $reader = $cmd.ExecuteReader()
    
    $tables = @()
    while ($reader.Read()) {
        $tables += $reader.GetString(0)
    }
    $reader.Close()
    
    Write-Host "Tables in the database:" -ForegroundColor Yellow
    Write-Host "======================" -ForegroundColor Yellow
    foreach ($table in $tables) {
        Write-Host "- $table"
    }
    Write-Host ""
    
    # Examine each table
    foreach ($table in $tables) {
        Write-Host "Schema for table: $table" -ForegroundColor Cyan
        Write-Host "------------------------" -ForegroundColor Cyan
        
        # Get table schema
        $cmd = New-Object System.Data.SQLite.SQLiteCommand("PRAGMA table_info($table)", $connection)
        $reader = $cmd.ExecuteReader()
        
        Write-Host "Column Name          Type             Not Null   Default    Primary Key"
        Write-Host "-----------          ----             --------   -------    -----------"
        
        $columns = @()
        while ($reader.Read()) {
            $name = $reader["name"]
            $type = $reader["type"]
            $notnull = if ($reader["notnull"] -eq 1) { "NO" } else { "YES" }
            $default = if ($reader["dflt_value"] -eq [DBNull]::Value) { "NULL" } else { $reader["dflt_value"] }
            $pk = if ($reader["pk"] -eq 1) { "YES" } else { "NO" }
            
            Write-Host ("{0,-20} {1,-16} {2,-10} {3,-10} {4,-11}" -f $name, $type, $notnull, $default, $pk)
            $columns += @{Name=$name; Type=$type}
        }
        $reader.Close()
        
        Write-Host ""
        
        # Get sample data (first 3 rows)
        Write-Host ("Sample data from {0}:" -f $table) -ForegroundColor Cyan
        $cmd = New-Object System.Data.SQLite.SQLiteCommand("SELECT * FROM $table LIMIT 3", $connection)
        $reader = $cmd.ExecuteReader()
        
        if ($reader.HasRows) {
            # Display column headers
            $header = ""
            foreach ($column in $columns) {
                $header += "{0,-15}" -f $column.Name
            }
            Write-Host $header
            Write-Host ("-" * $header.Length)
            
            # Display data
            while ($reader.Read()) {
                $row = ""
                foreach ($column in $columns) {
                    $value = $reader[$column.Name]
                    if ($value -eq [DBNull]::Value) { $value = "NULL" }
                    $row += "{0,-15}" -f $value
                }
                Write-Host $row
            }
        } else {
            Write-Host "(No data found)"
        }
        $reader.Close()
        
        Write-Host ""
    }
    
    $connection.Close()
    Write-Host "Database examination complete!" -ForegroundColor Green
    
} catch {
    Write-Host "Error examining database: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Exception type: $($_.Exception.GetType().FullName)" -ForegroundColor Red
    
    # Try a simpler approach
    Write-Host ""
    Write-Host "Attempting alternative approach..." -ForegroundColor Yellow
    
    try {
        # Just try to get basic info
        $fileInfo = Get-Item "database.db"
        Write-Host "Database file size: $([math]::Round($fileInfo.Length/1024, 2)) KB"
        Write-Host "Last modified: $($fileInfo.LastWriteTime)"
    } catch {
        Write-Host "Could not access database file information." -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "Press any key to continue..."
$host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")