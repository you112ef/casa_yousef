# PowerShell script to verify that all required DLLs are present
Write-Host "Verifying AForge.NET DLL installation..." -ForegroundColor Green

# List of required DLLs
$requiredDlls = @(
    "AForge.dll",
    "AForge.Video.dll",
    "AForge.Video.DirectShow.dll"
)

$allPresent = $true

foreach ($dll in $requiredDlls) {
    if (Test-Path $dll) {
        $fileInfo = Get-Item $dll
        Write-Host "✓ $($dll) - Found ($([math]::Round($fileInfo.Length/1024, 2)) KB)" -ForegroundColor Green
    } else {
        Write-Host "✗ $($dll) - Missing" -ForegroundColor Red
        $allPresent = $false
    }
}

Write-Host ""

if ($allPresent) {
    Write-Host "SUCCESS: All required AForge.NET DLLs are present!" -ForegroundColor Green
    Write-Host "You can now run your Sky CASA application without the missing assembly error." -ForegroundColor Yellow
} else {
    Write-Host "ERROR: Some required DLLs are missing." -ForegroundColor Red
    Write-Host "Please check the installation instructions in FIX_INSTRUCTIONS.md" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "Configuration files:" -ForegroundColor Cyan
if (Test-Path "Sky_CASA.exe.config") {
    Write-Host "✓ Sky_CASA.exe.config - Present (JIT debugging enabled)" -ForegroundColor Green
} else {
    Write-Host "✗ Sky_CASA.exe.config - Missing" -ForegroundColor Red
}

Write-Host ""
Write-Host "Press any key to continue..."
$host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")