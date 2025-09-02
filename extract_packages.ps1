# PowerShell script to extract NuGet packages and copy DLLs
Write-Host "Extracting NuGet packages..." -ForegroundColor Green

try {
    # Extract the packages we have
    Get-ChildItem -Path "packages\*.nupkg" | ForEach-Object {
        $packageName = $_.BaseName
        $extractPath = "packages\$packageName"
        New-Item -ItemType Directory -Path $extractPath -Force | Out-Null
        # Rename to .zip and extract
        $zipFile = "$extractPath.zip"
        Copy-Item $_.FullName $zipFile
        Expand-Archive -Path $zipFile -DestinationPath $extractPath -Force
        Remove-Item $zipFile -Force
        Write-Host "Extracted $packageName" -ForegroundColor Yellow
    }
    
    # Try to copy the DLLs we can find
    Write-Host "Copying available DLLs to main directory..." -ForegroundColor Yellow
    
    # Look for AForge.dll
    $aForgeDll = Get-ChildItem -Path "packages\*" -Recurse -Filter "AForge.dll" -ErrorAction SilentlyContinue
    if ($aForgeDll) {
        Copy-Item -Path $aForgeDll.FullName -Destination ".\" -Force
        Write-Host "Copied AForge.dll" -ForegroundColor Green
    } else {
        Write-Host "AForge.dll not found in packages" -ForegroundColor Red
    }
    
    # Look for AForge.Video.dll
    $aForgeVideoDll = Get-ChildItem -Path "packages\*" -Recurse -Filter "AForge.Video.dll" -ErrorAction SilentlyContinue
    if ($aForgeVideoDll) {
        Copy-Item -Path $aForgeVideoDll.FullName -Destination ".\" -Force
        Write-Host "Copied AForge.Video.dll" -ForegroundColor Green
    } else {
        Write-Host "AForge.Video.dll not found in packages" -ForegroundColor Red
    }
    
    # Note about missing package
    Write-Host "Note: AForge.Video.DirectShow package is still downloading or failed to download" -ForegroundColor Yellow
    Write-Host "Please wait for it to complete or download it manually" -ForegroundColor Yellow
    
} catch {
    Write-Host "Error occurred: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "Press any key to continue..."
$host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")