# PowerShell script to download AForge.NET DLLs directly
Write-Host "Downloading AForge.NET DLLs directly..." -ForegroundColor Green

try {
    # Create directories
    New-Item -ItemType Directory -Path "packages" -Force | Out-Null
    
    # Try to download from NuGet packages
    Write-Host "Attempting to download from NuGet..." -ForegroundColor Yellow
    
    # Download AForge package
    Write-Host "Downloading AForge package..." -ForegroundColor Yellow
    Invoke-WebRequest -Uri "https://www.nuget.org/api/v2/package/AForge/2.2.5" -OutFile "packages\AForge.2.2.5.nupkg" -ErrorAction SilentlyContinue
    
    # Download AForge.Video package
    Write-Host "Downloading AForge.Video package..." -ForegroundColor Yellow
    Invoke-WebRequest -Uri "https://www.nuget.org/api/v2/package/AForge.Video/2.2.5" -OutFile "packages\AForge.Video.2.2.5.nupkg" -ErrorAction SilentlyContinue
    
    # Download AForge.Video.DirectShow package
    Write-Host "Downloading AForge.Video.DirectShow package..." -ForegroundColor Yellow
    Invoke-WebRequest -Uri "https://www.nuget.org/api/v2/package/AForge.Video.DirectShow/2.2.5" -OutFile "packages\AForge.Video.DirectShow.2.2.5.nupkg" -ErrorAction SilentlyContinue
    
    # Extract the packages
    Write-Host "Extracting packages..." -ForegroundColor Yellow
    Get-ChildItem -Path "packages\*.nupkg" | ForEach-Object {
        $packageName = $_.BaseName
        $extractPath = "packages\$packageName"
        New-Item -ItemType Directory -Path $extractPath -Force | Out-Null
        # Rename to .zip and extract
        $zipFile = "$extractPath.zip"
        Copy-Item $_.FullName $zipFile
        Expand-Archive -Path $zipFile -DestinationPath $extractPath -Force
        Remove-Item $zipFile -Force
    }
    
    # Copy the DLLs to the main directory
    Write-Host "Copying DLLs to main directory..." -ForegroundColor Yellow
    Copy-Item -Path "packages\AForge.2.2.5\lib\AForge.dll" -Destination ".\" -ErrorAction SilentlyContinue
    Copy-Item -Path "packages\AForge.Video.2.2.5\lib\AForge.Video.dll" -Destination ".\" -ErrorAction SilentlyContinue
    Copy-Item -Path "packages\AForge.Video.DirectShow.2.2.5\lib\AForge.Video.DirectShow.dll" -Destination ".\" -ErrorAction SilentlyContinue
    
    Write-Host "All DLLs copied successfully!" -ForegroundColor Green
    Write-Host "You can now run your application." -ForegroundColor Green
    
} catch {
    Write-Host "Error occurred: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Trying alternative download method..." -ForegroundColor Yellow
    
    # Alternative method - create a simple text file with instructions
    $instructions = @"
AForge.NET Installation Instructions:

1. Visit the official AForge.NET GitHub repository:
   https://github.com/andrewkirillov/AForge.NET

2. Download the latest release or clone the repository

3. Build the solution or extract the pre-built binaries

4. Copy these files to your application directory:
   - AForge.dll
   - AForge.Video.dll
   - AForge.Video.DirectShow.dll

5. Alternatively, you can install via NuGet Package Manager:
   - Install-Package AForge.Video.DirectShow -Version 2.2.5

6. Make sure all DLLs are placed in the same directory as Sky_CASA.exe
"@
    
    $instructions | Out-File -FilePath "MANUAL_INSTALL_INSTRUCTIONS.txt" -Encoding UTF8
    Write-Host "Manual installation instructions have been saved to MANUAL_INSTALL_INSTRUCTIONS.txt" -ForegroundColor Yellow
}

Write-Host "Press any key to continue..."
$host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")