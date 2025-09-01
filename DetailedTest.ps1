# PowerShell script to test Firebird SQL Client loading with detailed error information
Write-Host "Testing Firebird SQL Client loading..." -ForegroundColor Green
Write-Host ""

try {
    Write-Host "1. Attempting to load assembly..." -ForegroundColor Yellow
    $assembly = [System.Reflection.Assembly]::LoadFrom("FirebirdSql.Data.FirebirdClient.dll")
    Write-Host "   Assembly loaded successfully!" -ForegroundColor Green
    Write-Host "   Assembly name: $($assembly.FullName)" -ForegroundColor Cyan
    
    Write-Host "2. Checking referenced assemblies..." -ForegroundColor Yellow
    $references = $assembly.GetReferencedAssemblies()
    foreach ($reference in $references) {
        Write-Host "   - $($reference.Name), Version=$($reference.Version)" -ForegroundColor Cyan
    }
    
    Write-Host "3. Attempting to create FbConnection type..." -ForegroundColor Yellow
    $fbConnectionType = $assembly.GetType("FirebirdSql.Data.FirebirdClient.FbConnection")
    if ($fbConnectionType) {
        Write-Host "   FbConnection type created successfully!" -ForegroundColor Green
    } else {
        Write-Host "   Failed to create FbConnection type" -ForegroundColor Red
    }
    
    Write-Host "4. Test completed successfully!" -ForegroundColor Green
} catch {
    Write-Host "   Error occurred: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "   Exception type: $($_.Exception.GetType().FullName)" -ForegroundColor Red
    
    if ($_.Exception.InnerException) {
        Write-Host "   Inner exception: $($_.Exception.InnerException.Message)" -ForegroundColor Red
    }
    
    # Get detailed loader exceptions if available
    if ($_.Exception -is [System.Reflection.ReflectionTypeLoadException]) {
        Write-Host "   Loader exceptions:" -ForegroundColor Red
        foreach ($loaderException in $_.Exception.LoaderExceptions) {
            Write-Host "     - $($loaderException.Message)" -ForegroundColor Red
        }
    }
}

Write-Host ""
Write-Host "Press any key to continue..."
$host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")