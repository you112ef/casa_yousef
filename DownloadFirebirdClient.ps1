# PowerShell script to download Firebird SQL Client
Write-Host "Downloading Firebird SQL Client..." -ForegroundColor Green
Write-Host ""

try {
    # Create a temporary directory
    $tempDir = "temp_firebird"
    if (!(Test-Path $tempDir)) {
        New-Item -ItemType Directory -Path $tempDir | Out-Null
    }
    
    Write-Host "1. Downloading Firebird SQL Client package..." -ForegroundColor Yellow
    
    # Download the NuGet package
    $url = "https://www.nuget.org/api/v2/package/FirebirdSql.Data.FirebirdClient/10.3.1"
    $output = "$tempDir\firebird.nupkg"
    
    Invoke-WebRequest -Uri $url -OutFile $output -ErrorAction Stop
    
    Write-Host "   Download complete!" -ForegroundColor Green
    
    Write-Host "2. Extracting package..." -ForegroundColor Yellow
    
    # Rename to .zip and extract
    $zipFile = "$tempDir\firebird.zip"
    Rename-Item -Path $output -NewName $zipFile -Force
    Expand-Archive -Path $zipFile -DestinationPath "$tempDir\extracted" -Force
    
    Write-Host "   Extraction complete!" -ForegroundColor Green
    
    Write-Host "3. Locating DLL..." -ForegroundColor Yellow
    
    # Look for the DLL in the extracted files
    $dllPath = Get-ChildItem -Path "$tempDir\extracted" -Recurse -Filter "FirebirdSql.Data.FirebirdClient.dll" -ErrorAction SilentlyContinue
    
    if ($dllPath) {
        Write-Host "   Found DLL: $($dllPath.FullName)" -ForegroundColor Green
        
        Write-Host "4. Copying DLL to application directory..." -ForegroundColor Yellow
        
        # Copy the DLL to the main directory
        Copy-Item -Path $dllPath.FullName -Destination ".\" -Force
        
        Write-Host "   DLL copied successfully!" -ForegroundColor Green
        Write-Host ""
        Write-Host "SUCCESS: Firebird SQL Client has been installed!" -ForegroundColor Green
        Write-Host "You can now run your application without the missing assembly error." -ForegroundColor Cyan
        
        # Clean up
        Remove-Item -Path $tempDir -Recurse -Force -ErrorAction SilentlyContinue
    } else {
        Write-Host "   DLL not found in the package." -ForegroundColor Red
        Write-Host "   You may need to download it manually from:" -ForegroundColor Yellow
        Write-Host "   https://www.nuget.org/packages/FirebirdSql.Data.FirebirdClient/10.3.1" -ForegroundColor Yellow
    }
    
} catch {
    Write-Host "Error occurred: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Write-Host "Alternative solution:" -ForegroundColor Yellow
    Write-Host "1. Visit https://www.nuget.org/packages/FirebirdSql.Data.FirebirdClient/10.3.1" -ForegroundColor Yellow
    Write-Host "2. Download the package manually" -ForegroundColor Yellow
    Write-Host "3. Extract FirebirdSql.Data.FirebirdClient.dll to this directory" -ForegroundColor Yellow
    
    # Create a manual installation guide
    $manualGuide = @"
MANUAL INSTALLATION INSTRUCTIONS:

1. Go to: https://www.nuget.org/packages/FirebirdSql.Data.FirebirdClient/10.3.1
2. Click on "Download package" to download the .nupkg file
3. Rename the file extension from .nupkg to .zip
4. Extract the ZIP file
5. Navigate to lib/net452/ (or the appropriate framework version)
6. Copy FirebirdSql.Data.FirebirdClient.dll to this directory:
   D:\New folder (4)\Sky CASA\
7. Run your application again
"@
    
    $manualGuide | Out-File -FilePath "MANUAL_FIREBIRD_INSTALL.txt" -Encoding UTF8
    Write-Host "Manual installation instructions saved to MANUAL_FIREBIRD_INSTALL.txt" -ForegroundColor Cyan
}

Write-Host ""
Write-Host "Press any key to continue..."
$host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")