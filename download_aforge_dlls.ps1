# PowerShell script to download AForge.NET DLLs
Write-Host "Downloading AForge.NET DLLs..." -ForegroundColor Green

# Create a temporary directory
$tempDir = "temp_aforge"
New-Item -ItemType Directory -Path $tempDir -Force | Out-Null

try {
    # Download the AForge.NET framework ZIP file
    Write-Host "Downloading AForge.NET framework..." -ForegroundColor Yellow
    $url = "https://github.com/andrewkirillov/AForge.NET/archive/master.zip"
    $output = "$tempDir\aforge.zip"
    
    Invoke-WebRequest -Uri $url -OutFile $output
    
    Write-Host "Extracting files..." -ForegroundColor Yellow
    # Extract the ZIP file
    Expand-Archive -Path $output -DestinationPath $tempDir -Force
    
    Write-Host "Locating required DLLs..." -ForegroundColor Yellow
    # Find the required DLLs (this is a simplified approach)
    # In practice, you would need to build the project or extract from the proper release
    
    Write-Host "NOTE: The direct download approach may require building the libraries." -ForegroundColor Red
    Write-Host "It's recommended to use the NuGet approach or download pre-built binaries." -ForegroundColor Red
    
} catch {
    Write-Host "Error occurred: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Please try the manual approach:" -ForegroundColor Yellow
    Write-Host "1. Visit https://github.com/andrewkirillov/AForge.NET" -ForegroundColor Yellow
    Write-Host "2. Download the release binaries or build from source" -ForegroundColor Yellow
    Write-Host "3. Copy AForge.dll, AForge.Video.dll, and AForge.Video.DirectShow.dll to this folder" -ForegroundColor Yellow
}

Write-Host "Press any key to continue..."
$host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")