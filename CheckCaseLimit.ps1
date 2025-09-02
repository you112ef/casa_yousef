# PowerShell script to check for case limits in the database
Write-Host "Checking for case limits in database..." -ForegroundColor Green
Write-Host ""

try {
    # Check if database file exists
    if (-not (Test-Path "database.db")) {
        Write-Host "✗ Database file not found!" -ForegroundColor Red
        exit 1
    }
    
    # Get basic file information
    $fileInfo = Get-Item "database.db"
    Write-Host "Database file information:" -ForegroundColor Yellow
    Write-Host "========================" -ForegroundColor Yellow
    Write-Host "File size: $([math]::Round($fileInfo.Length/1024, 2)) KB"
    Write-Host "Last modified: $($fileInfo.LastWriteTime)"
    Write-Host ""
    
    # Try to read the database header to verify it's a SQLite file
    $stream = [System.IO.File]::OpenRead("database.db")
    $reader = New-Object System.IO.BinaryReader($stream)
    
    # Read the first 16 bytes (SQLite header)
    $header = $reader.ReadBytes(16)
    $headerString = [System.Text.Encoding]::ASCII.GetString($header)
    
    Write-Host "Database header:" -ForegroundColor Yellow
    Write-Host "===============" -ForegroundColor Yellow
    Write-Host "Header: $headerString"
    Write-Host ""
    
    # Check if it's a valid SQLite file
    if ($headerString.StartsWith("SQLite format 3")) {
        Write-Host "✓ Valid SQLite database file detected" -ForegroundColor Green
    } else {
        Write-Host "✗ File does not appear to be a valid SQLite database" -ForegroundColor Red
        $reader.Close()
        $stream.Close()
        exit 1
    }
    
    $reader.Close()
    $stream.Close()
    
    # Try to extract table information by reading the file as binary
    Write-Host "Examining database structure..." -ForegroundColor Yellow
    Write-Host "=============================" -ForegroundColor Yellow
    
    # Try using .NET SQLite classes if available
    try {
        Add-Type -AssemblyName "System.Data"
        $connectionString = "Data Source=database.db;Version=3;"
        
        # Try to load SQLite if available
        try {
            # Try to load System.Data.SQLite if available in GAC
            Add-Type -AssemblyName "System.Data.SQLite"
            $useSQLite = $true
        } catch {
            # Try to load from local directory
            try {
                $sqliteDllPath = Get-ChildItem -Path "." -Filter "*System.Data.SQLite*.dll" -ErrorAction SilentlyContinue | Select-Object -First 1
                if ($sqliteDllPath) {
                    Add-Type -Path $sqliteDllPath.FullName
                    $useSQLite = $true
                } else {
                    $useSQLite = $false
                }
            } catch {
                $useSQLite = $false
            }
        }
        
        if ($useSQLite) {
            Write-Host "✓ SQLite .NET provider available" -ForegroundColor Green
            
            $connection = New-Object System.Data.SQLite.SQLiteConnection($connectionString)
            $connection.Open()
            
            # List all tables
            Write-Host ""
            Write-Host "Database tables:" -ForegroundColor Cyan
            Write-Host "===============" -ForegroundColor Cyan
            $cmd = New-Object System.Data.SQLite.SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table'", $connection)
            $reader = $cmd.ExecuteReader()
            
            $tables = @()
            while ($reader.Read()) {
                $tableName = $reader.GetString(0)
                $tables += $tableName
                Write-Host "- $tableName"
            }
            $reader.Close()
            
            # Check for triggers
            Write-Host ""
            Write-Host "Database triggers:" -ForegroundColor Cyan
            Write-Host "=================" -ForegroundColor Cyan
            $cmd = New-Object System.Data.SQLite.SQLiteCommand("SELECT name, tbl_name FROM sqlite_master WHERE type='trigger'", $connection)
            $reader = $cmd.ExecuteReader()
            
            if ($reader.HasRows) {
                while ($reader.Read()) {
                    $triggerName = $reader.GetString(0)
                    $tableName = $reader.GetString(1)
                    Write-Host "- $triggerName on $tableName"
                }
            } else {
                Write-Host "No triggers found"
            }
            $reader.Close()
            
            # Check for any constraints or limits in the database
            Write-Host ""
            Write-Host "Checking for constraints that might limit cases..." -ForegroundColor Yellow
            Write-Host "================================================" -ForegroundColor Yellow
            
            # Look for any tables that might store cases/patients
            $possibleCaseTables = @("patients", "cases", "records", "sem", "cbc", "stool", "urine", "vis")
            
            foreach ($tableName in $possibleCaseTables) {
                $cmd = New-Object System.Data.SQLite.SQLiteCommand("SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name=@tableName", $connection)
                $cmd.Parameters.AddWithValue("@tableName", $tableName) | Out-Null
                $result = $cmd.ExecuteScalar()
                
                if ($result -ne $null -and [int]$result -gt 0) {
                    Write-Host "Found table: $tableName" -ForegroundColor Green
                    
                    # Count records in the table
                    $countCmd = New-Object System.Data.SQLite.SQLiteCommand("SELECT COUNT(*) FROM [$tableName]", $connection)
                    $countResult = $countCmd.ExecuteScalar()
                    Write-Host "  Record count: $countResult"
                    
                    # Check table schema for any constraints
                    Write-Host "  Schema:" -ForegroundColor Gray
                    $schemaCmd = New-Object System.Data.SQLite.SQLiteCommand("PRAGMA table_info([$tableName])", $connection)
                    $schemaReader = $schemaCmd.ExecuteReader()
                    
                    while ($schemaReader.Read()) {
                        $columnName = $schemaReader["name"]
                        $columnType = $schemaReader["type"]
                        $notNull = if ($schemaReader["notnull"] -eq 1) { "NOT NULL" } else { "" }
                        $defaultValue = $schemaReader["dflt_value"]
                        $primaryKey = if ($schemaReader["pk"] -eq 1) { "PRIMARY KEY" } else { "" }
                        
                        Write-Host "    $columnName $columnType $notNull $primaryKey DEFAULT $defaultValue" -ForegroundColor Gray
                    }
                    $schemaReader.Close()
                    
                    Write-Host ""
                }
            }
            
            $connection.Close()
        } else {
            Write-Host "⚠ SQLite .NET provider not available, using basic file analysis" -ForegroundColor Yellow
            
            # Read the file content as text to look for table definitions
            $content = [System.IO.File]::ReadAllText("database.db", [System.Text.Encoding]::ASCII)
            
            # Look for CREATE TABLE statements
            $createTableMatches = [regex]::Matches($content, "CREATE\s+TABLE\s+(\w+)", [System.Text.RegularExpressions.RegexOptions]::IgnoreCase)
            
            if ($createTableMatches.Count -gt 0) {
                Write-Host "Tables found:" -ForegroundColor Cyan
                $tables = @()
                foreach ($match in $createTableMatches) {
                    $tableName = $match.Groups[1].Value
                    $tables += $tableName
                    Write-Host "  - $tableName"
                }
            } else {
                Write-Host "No CREATE TABLE statements found in file content" -ForegroundColor Gray
            }
        }
    } catch {
        Write-Host "Error examining database: $($_.Exception.Message)" -ForegroundColor Red
    }
    
} catch {
    Write-Host "Error examining database: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Exception type: $($_.Exception.GetType().FullName)" -ForegroundColor Red
}

Write-Host ""
Write-Host "Database examination complete!" -ForegroundColor Green