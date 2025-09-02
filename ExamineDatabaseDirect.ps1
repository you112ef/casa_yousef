# PowerShell script to examine the SQLite database directly without external dependencies
Write-Host "Examining database.db directly..." -ForegroundColor Green
Write-Host ""

try {
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
    }
    
    $reader.Close()
    $stream.Close()
    
    # Try to extract some basic information by reading the file as binary
    Write-Host ""
    Write-Host "Attempting to extract table information..." -ForegroundColor Yellow
    Write-Host "=========================================" -ForegroundColor Yellow
    
    # Read the file content as text to look for table definitions
    $content = [System.IO.File]::ReadAllText("database.db", [System.Text.Encoding]::ASCII)
    
    # Look for CREATE TABLE statements
    $createTableMatches = [regex]::Matches($content, "CREATE\s+TABLE\s+(\w+)", [System.Text.RegularExpressions.RegexOptions]::IgnoreCase)
    
    if ($createTableMatches.Count -gt 0) {
        Write-Host "Tables found:" -ForegroundColor Cyan
        foreach ($match in $createTableMatches) {
            Write-Host "  - $($match.Groups[1].Value)"
        }
    } else {
        Write-Host "No CREATE TABLE statements found in file content" -ForegroundColor Gray
    }
    
    # Look for any table-like references
    $tableRefs = [regex]::Matches($content, "FROM\s+(\w+)", [System.Text.RegularExpressions.RegexOptions]::IgnoreCase)
    if ($tableRefs.Count -gt 0) {
        Write-Host ""
        Write-Host "Table references found in queries:" -ForegroundColor Cyan
        $uniqueTables = @{}
        foreach ($match in $tableRefs) {
            $tableName = $match.Groups[1].Value
            if (-not $uniqueTables.ContainsKey($tableName)) {
                $uniqueTables[$tableName] = $true
                Write-Host "  - $tableName"
            }
        }
    }
    
} catch {
    Write-Host "Error examining database: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Exception type: $($_.Exception.GetType().FullName)" -ForegroundColor Red
}

Write-Host ""
Write-Host "Database examination complete!" -ForegroundColor Green