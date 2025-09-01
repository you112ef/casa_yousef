# PowerShell script to help update UI code to use GetAllData instead of GetSampleData
Write-Host "UPDATING UI CODE FOR CASE LIMIT FIX" -ForegroundColor Green
Write-Host "===================================" -ForegroundColor Green
Write-Host ""

# Function to update C# files
function Update-CS-Files {
    param([string]$FilePath)
    
    Write-Host "Processing C# file: $FilePath" -ForegroundColor Yellow
    
    try {
        $content = Get-Content $FilePath -Raw
        
        # Check if file contains GetSampleData calls
        if ($content -match 'GetSampleData\s*\(\s*"[^"]+"\s*,\s*\d+\s*\)') {
            Write-Host "  Found GetSampleData calls, updating..." -ForegroundColor Cyan
            
            # Replace GetSampleData with GetAllData and remove the limit parameter
            $updatedContent = $content -replace 'GetSampleData\s*\(\s*("[^"]+")\s*,\s*\d+\s*\)', 'GetAllData($1)'
            
            # Also handle cases where variable is used
            $updatedContent = $updatedContent -replace 'GetSampleData\s*\(\s*([^,]+)\s*,\s*\d+\s*\)', 'GetAllData($1)'
            
            # Write updated content back to file
            $updatedContent | Out-File -FilePath $FilePath -Encoding UTF8
            Write-Host "  ✓ Updated successfully" -ForegroundColor Green
        }
        else {
            Write-Host "  No GetSampleData calls found" -ForegroundColor Gray
        }
    }
    catch {
        Write-Host "  ✗ Error processing file: $($_.Exception.Message)" -ForegroundColor Red
    }
}

# Function to update VB files
function Update-VB-Files {
    param([string]$FilePath)
    
    Write-Host "Processing VB file: $FilePath" -ForegroundColor Yellow
    
    try {
        $content = Get-Content $FilePath -Raw
        
        # Check if file contains GetSampleData calls
        if ($content -match 'GetSampleData\s*\(\s*"[^"]+"\s*,\s*\d+\s*\)') {
            Write-Host "  Found GetSampleData calls, updating..." -ForegroundColor Cyan
            
            # Replace GetSampleData with GetAllData and remove the limit parameter
            $updatedContent = $content -replace 'GetSampleData\s*\(\s*("[^"]+")\s*,\s*\d+\s*\)', 'GetAllData($1)'
            
            # Also handle cases where variable is used
            $updatedContent = $updatedContent -replace 'GetSampleData\s*\(\s*([^,]+)\s*,\s*\d+\s*\)', 'GetAllData($1)'
            
            # Write updated content back to file
            $updatedContent | Out-File -FilePath $FilePath -Encoding UTF8
            Write-Host "  ✓ Updated successfully" -ForegroundColor Green
        }
        else {
            Write-Host "  No GetSampleData calls found" -ForegroundColor Gray
        }
    }
    catch {
        Write-Host "  ✗ Error processing file: $($_.Exception.Message)" -ForegroundColor Red
    }
}

# Search for potential UI files to update
Write-Host "Searching for UI files to update..." -ForegroundColor Yellow

# Search for .cs files that might contain UI code
$csFiles = Get-ChildItem -Path "." -Recurse -Filter "*.cs" -ErrorAction SilentlyContinue | 
           Where-Object { $_.Name -notlike "DataAccessLayer*" -and $_.Name -notlike "Test*" }

# Search for .vb files that might contain UI code
$vbFiles = Get-ChildItem -Path "." -Recurse -Filter "*.vb" -ErrorAction SilentlyContinue | 
           Where-Object { $_.Name -notlike "DataAccessLayer*" -and $_.Name -notlike "Test*" }

# Process C# files
if ($csFiles.Count -gt 0) {
    Write-Host "Found $($csFiles.Count) C# files to check:" -ForegroundColor Cyan
    foreach ($file in $csFiles) {
        Write-Host "  - $($file.Name)"
        Update-CS-Files -FilePath $file.FullName
    }
} else {
    Write-Host "No C# UI files found to process" -ForegroundColor Gray
}

Write-Host ""

# Process VB files
if ($vbFiles.Count -gt 0) {
    Write-Host "Found $($vbFiles.Count) VB files to check:" -ForegroundColor Cyan
    foreach ($file in $vbFiles) {
        Write-Host "  - $($file.Name)"
        Update-VB-Files -FilePath $file.FullName
    }
} else {
    Write-Host "No VB UI files found to process" -ForegroundColor Gray
}

Write-Host ""
Write-Host "MANUAL UPDATE INSTRUCTIONS:" -ForegroundColor Yellow
Write-Host "==========================" -ForegroundColor Yellow
Write-Host "If no files were automatically updated, you'll need to manually update your UI code:"
Write-Host ""
Write-Host "C# - Change from:" -ForegroundColor Cyan
Write-Host '  var sampleData = dal.GetSampleData("table_name", 10);' -ForegroundColor White
Write-Host "To:" -ForegroundColor Cyan
Write-Host '  var allData = dal.GetAllData("table_name");' -ForegroundColor White
Write-Host ""
Write-Host "VB.NET - Change from:" -ForegroundColor Cyan
Write-Host '  Dim sampleData = dal.GetSampleData("table_name", 10)' -ForegroundColor White
Write-Host "To:" -ForegroundColor Cyan
Write-Host '  Dim allData = dal.GetAllData("table_name")' -ForegroundColor White
Write-Host ""
Write-Host "Refer to UI_UPDATE_GUIDE.txt for detailed instructions." -ForegroundColor Yellow
Write-Host ""
Write-Host "UPDATE COMPLETE" -ForegroundColor Green