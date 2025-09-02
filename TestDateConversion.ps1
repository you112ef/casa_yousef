# TestDateConversion.ps1
# PowerShell script to test the safe date conversion fix

Write-Host "Testing Safe Date Conversion Fix..." -ForegroundColor Green

# Define the safe conversion function
function SafeConvertToDate {
    param(
        [string]$inputString,
        [datetime]$defaultValue
    )
    
    # Handle null or empty strings
    if ([string]::IsNullOrEmpty($inputString)) {
        return $defaultValue
    }
    
    # Try to parse the string as a date
    $result = New-Object DateTime
    if ([datetime]::TryParse($inputString.Trim(), [ref]$result)) {
        return $result
    }
    else {
        # Return default value if parsing fails
        return $defaultValue
    }
}

# Test cases
$testCases = @(
    "2025-08-31 00:00:00",
    "2025-09-01 12:30:45",
    "",
    $null,
    "invalid date",
    "01/01/2025",
    "2025-13-45" # Invalid date
)

Write-Host "Testing SafeConvertToDate function:" -ForegroundColor Yellow

foreach ($testCase in $testCases) {
    try {
        $result = SafeConvertToDate -inputString $testCase -defaultValue (Get-Date)
        Write-Host "Input: '$testCase' -> Output: $result" -ForegroundColor Cyan
    }
    catch {
        Write-Host "Input: '$testCase' -> Exception: $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host "`nTest completed. The fix should prevent InvalidCastException errors." -ForegroundColor Green
Write-Host "Press any key to exit..."
$host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")