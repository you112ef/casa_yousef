# PowerShell script to analyze SQL queries for common logic errors
# This script can be integrated into the application to log and analyze SQL queries

param(
    [string]$LogFilePath = "sql_analysis.log",
    [string]$DatabasePath = "database.db"
)

function Write-Log {
    param([string]$Message)
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    $logEntry = "[$timestamp] $Message"
    Add-Content -Path $LogFilePath -Value $logEntry
    Write-Host $logEntry
}

function Test-SQLQuery {
    param([string]$Query)
    
    $issues = @()
    
    # Convert to uppercase for easier pattern matching
    $upperQuery = $Query.ToUpper().Trim()
    
    # Check for NULL comparison issues
    if ($upperQuery -match "= NULL" -or $upperQuery -match "!= NULL" -or $upperQuery -match "<> NULL") {
        $issues += "DIRECT NULL COMPARISON: Use IS NULL or IS NOT NULL instead of = NULL or != NULL"
    }
    
    # Check for missing WHERE clause in UPDATE/DELETE
    if (($upperQuery.StartsWith("UPDATE") -or $upperQuery.StartsWith("DELETE")) -and 
        -not $upperQuery.Contains("WHERE")) {
        $issues += "MISSING WHERE CLAUSE: UPDATE/DELETE without WHERE will affect all rows"
    }
    
    # Check for SELECT * (not necessarily an error, but worth noting)
    if ($upperQuery.StartsWith("SELECT *")) {
        $issues += "SELECT * DETECTED: Consider specifying columns explicitly for better performance"
    }
    
    # Check for Cartesian products (JOIN without ON condition)
    if ($upperQuery -match "JOIN\s+\w+\s*(,|WHERE|$)" -and -not $upperQuery.Contains(" ON ")) {
        $issues += "POSSIBLE CARTESIAN PRODUCT: JOIN without proper ON condition"
    }
    
    # Check for GROUP BY issues (non-aggregate columns not in GROUP BY)
    if ($upperQuery.Contains("GROUP BY")) {
        # This is a simplified check - in practice, this would need more sophisticated analysis
        $issues += "GROUP BY DETECTED: Verify all non-aggregate columns are in GROUP BY clause"
    }
    
    # Check for subquery issues (single value vs multiple values)
    if ($upperQuery -match "\w+\s*=\s*\(" -and -not $upperQuery.Contains("IN (")) {
        $issues += "POSSIBLE SUBQUERY ISSUE: Single value comparison with subquery - consider using IN"
    }
    
    return $issues
}

function Analyze-SQLFile {
    param([string]$FilePath)
    
    Write-Log ("Analyzing SQL file: " + $FilePath)
    
    try {
        $content = Get-Content $FilePath -ErrorAction Stop
        $lineNumber = 0
        
        foreach ($line in $content) {
            $lineNumber++
            
            # Skip empty lines and comments
            if ([string]::IsNullOrWhiteSpace($line) -or $line.Trim().StartsWith("--")) {
                continue
            }
            
            # Check for SQL queries (simplified detection)
            if ($line -match "^\s*(SELECT|INSERT|UPDATE|DELETE|CREATE|ALTER|DROP)" -and 
                $line -notmatch "^\s*--") {
                
                $issues = Test-SQLQuery -Query $line
                if ($issues.Count -gt 0) {
                    Write-Log ("ISSUES FOUND in line " + $lineNumber + ": " + $line)
                    foreach ($issue in $issues) {
                        Write-Log ("  - " + $issue)
                    }
                }
            }
        }
    }
    catch {
        Write-Log ("ERROR analyzing file " + $FilePath + ": " + $_.Exception.Message)
    }
}

function Analyze-ApplicationDirectory {
    param([string]$DirectoryPath = ".")
    
    Write-Log ("Starting SQL analysis of application directory: " + $DirectoryPath)
    
    # Look for common source code files that might contain SQL
    $sourceFiles = Get-ChildItem -Path $DirectoryPath -Include *.vb,*.cs,*.sql -Recurse -ErrorAction SilentlyContinue
    
    foreach ($file in $sourceFiles) {
        Analyze-SQLFile -FilePath $file.FullName
    }
    
    Write-Log "SQL analysis complete"
}

# Main execution
Write-Log "SQL Logic Error Analyzer Started"
Write-Log ("Database Path: " + $DatabasePath)
Write-Log ("Log File: " + $LogFilePath)

# Analyze the current directory for SQL-related issues
Analyze-ApplicationDirectory

Write-Log "SQL Logic Error Analyzer Finished"