# PowerShell script to scan all files for SQL queries and identify/fix logic errors
# This script will scan the project for SQL queries and provide suggestions for fixes

param(
    [string]$ProjectPath = ".",
    [string]$LogFilePath = "sql_scan_fix.log"
)

function Write-Log {
    param([string]$Message)
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    $logEntry = "[$timestamp] $Message"
    Add-Content -Path $LogFilePath -Value $logEntry
    Write-Host $logEntry
}

function Test-SQLQuery {
    param([string]$Query, [string]$FilePath, [int]$LineNumber)
    
    $issues = @()
    $suggestions = @()
    
    # Convert to uppercase for easier pattern matching
    $upperQuery = $Query.ToUpper().Trim()
    
    # Check for NULL comparison issues
    if ($upperQuery -match "= NULL" -or $upperQuery -match "!= NULL" -or $upperQuery -match "<> NULL") {
        $issues += "DIRECT NULL COMPARISON"
        $suggestions += "Replace '= NULL' with 'IS NULL' and '!= NULL' or '<> NULL' with 'IS NOT NULL'"
    }
    
    # Check for missing WHERE clause in UPDATE/DELETE
    if (($upperQuery.StartsWith("UPDATE") -or $upperQuery.StartsWith("DELETE")) -and 
        -not $upperQuery.Contains("WHERE")) {
        $issues += "MISSING WHERE CLAUSE"
        $suggestions += "Add a WHERE clause to limit the rows affected by UPDATE/DELETE operations"
    }
    
    # Check for Cartesian products (JOIN without ON condition)
    if ($upperQuery -match "JOIN\s+\w+\s*(;|$)" -and -not $upperQuery.Contains(" ON ")) {
        $issues += "POSSIBLE CARTESIAN PRODUCT"
        $suggestions += "Add proper JOIN conditions using ON clause"
    }
    
    # Check for subquery issues (single value vs multiple values)
    if ($upperQuery -match "\w+\s*=\s*\(" -and -not $upperQuery.Contains("IN (")) {
        $issues += "POSSIBLE SUBQUERY ISSUE"
        $suggestions += "Use IN operator instead of = when subquery might return multiple values"
    }
    
    # Return results
    return @{
        Issues = $issues
        Suggestions = $suggestions
    }
}

function Scan-FileForSQL {
    param([string]$FilePath)
    
    Write-Log ("Scanning file: " + $FilePath)
    
    try {
        $content = Get-Content $FilePath -ErrorAction Stop
        $lineNumber = 0
        
        foreach ($line in $content) {
            $lineNumber++
            
            # Skip empty lines and comments
            if ([string]::IsNullOrWhiteSpace($line) -or $line.Trim().StartsWith("--") -or $line.Trim().StartsWith("'")) {
                continue
            }
            
            # Check for SQL queries (simplified detection)
            if ($line -match "^\s*(SELECT|INSERT|UPDATE|DELETE|CREATE|ALTER|DROP)" -and 
                $line -notmatch "^\s*--" -and $line -notmatch "^\s*'") {
                
                $result = Test-SQLQuery -Query $line -FilePath $FilePath -LineNumber $lineNumber
                if ($result.Issues.Count -gt 0) {
                    Write-Log ("ISSUES FOUND in " + $FilePath + " at line " + $lineNumber + ": " + $line)
                    for ($i = 0; $i -lt $result.Issues.Count; $i++) {
                        Write-Log ("  - ISSUE: " + $result.Issues[$i])
                        Write-Log ("    FIX: " + $result.Suggestions[$i])
                    }
                }
            }
        }
    }
    catch {
        Write-Log ("ERROR scanning file " + $FilePath + ": " + $_.Exception.Message)
    }
}

function Scan-ProjectForSQL {
    param([string]$DirectoryPath = ".")
    
    Write-Log ("Starting SQL scan of project directory: " + $DirectoryPath)
    
    # Look for common source code files that might contain SQL
    $sourceFiles = Get-ChildItem -Path $DirectoryPath -Include *.vb,*.cs,*.sql -Recurse -ErrorAction SilentlyContinue
    
    foreach ($file in $sourceFiles) {
        Scan-FileForSQL -FilePath $file.FullName
    }
    
    Write-Log "SQL scan complete"
}

# Main execution
Write-Log "SQL Scanner and Fixer Started"
Write-Log ("Project Path: " + $ProjectPath)
Write-Log ("Log File: " + $LogFilePath)

# Scan the project for SQL-related issues
Scan-ProjectForSQL -DirectoryPath $ProjectPath

Write-Log "SQL Scanner and Fixer Finished"
Write-Log "=================================================="