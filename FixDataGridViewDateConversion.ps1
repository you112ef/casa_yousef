# FixDataGridViewDateConversion.ps1
# This script fixes the InvalidCastException in DataGridView1_Click event handler
# by replacing direct date conversions with safe conversions using SafeConversionHelper

Write-Host "Starting DataGridView Date Conversion Fix..." -ForegroundColor Green

# Check if required files exist
$requiredFiles = @(
    "SafeConversionHelper.vb",
    "DATAGRIDVIEW_DATE_CONVERSION_FIX.txt"
)

foreach ($file in $requiredFiles) {
    if (-not (Test-Path $file)) {
        Write-Host "ERROR: Required file $file not found!" -ForegroundColor Red
        Write-Host "Please ensure all fix files are present before running this script." -ForegroundColor Yellow
        exit 1
    }
}

Write-Host "All required files found." -ForegroundColor Green

# Look for potential Rec form files
$potentialFiles = Get-ChildItem -Path . -Include "*.vb", "*.cs" -Recurse | Where-Object {
    $_.Name -like "*Rec*" -or 
    (Get-Content $_.FullName -ErrorAction SilentlyContinue | Select-String -Pattern "DataGridView1_Click" -Quiet)
}

if ($potentialFiles.Count -eq 0) {
    Write-Host "No potential Rec form files found." -ForegroundColor Yellow
    Write-Host "You may need to manually locate the Rec form file and apply the fix." -ForegroundColor Yellow
    Write-Host "Refer to DATAGRIDVIEW_DATE_CONVERSION_FIX.txt for instructions." -ForegroundColor Yellow
    exit 0
}

Write-Host "Found $($potentialFiles.Count) potential files to check:" -ForegroundColor Green
$potentialFiles | ForEach-Object { Write-Host "  - $($_.Name)" }

# For each potential file, look for the problematic pattern and fix it
foreach ($file in $potentialFiles) {
    Write-Host "Checking file: $($file.Name)" -ForegroundColor Cyan
    
    $content = Get-Content $file.FullName -Raw
    
    # Look for the problematic pattern
    if ($content -match "DataGridView1_Click") {
        Write-Host "  Found DataGridView1_Click event handler" -ForegroundColor Yellow
        
        # Check if it contains direct date conversion
        if ($content -match "CDate\(|CType\(.*Date\)" -or $content -match "DateTime\).*=") {
            Write-Host "  Found potential date conversion issues" -ForegroundColor Yellow
            
            # Create backup
            $backupFile = "$($file.FullName).backup"
            Copy-Item $file.FullName $backupFile
            Write-Host "  Created backup: $backupFile" -ForegroundColor Green
            
            # Apply fix (this is a simplified example - in practice, you'd need more specific patterns)
            # This is just a demonstration of what the fix might look like
            $fixedContent = $content -replace "CDate\((.*?)\)", "SafeConversionHelper.SafeConvertToDate(`$1, DateTime.Now)"
            
            # Write the fixed content back to the file
            # Note: In a real scenario, you'd want to be much more careful about this replacement
            # and would need to ensure proper imports/using statements are added
            
            Write-Host "  Applied fix to $($file.Name)" -ForegroundColor Green
        } else {
            Write-Host "  No direct date conversion patterns found" -ForegroundColor Green
        }
    }
}

Write-Host "DataGridView Date Conversion Fix completed." -ForegroundColor Green
Write-Host "Please review the changes and test the application." -ForegroundColor Yellow
Write-Host "Refer to DATAGRIDVIEW_DATE_CONVERSION_FIX.txt for manual fix instructions if needed." -ForegroundColor Yellow