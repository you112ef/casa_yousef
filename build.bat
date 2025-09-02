@echo off
echo Building Sky CASA Application...
echo ================================

REM Check if MSBuild is available
where msbuild >nul 2>&1
if %errorlevel% neq 0 (
    echo MSBuild not found. Please install Visual Studio or MSBuild tools.
    pause
    exit /b 1
)

REM Restore NuGet packages
echo Restoring NuGet packages...
nuget restore SkyCASA.sln
if %errorlevel% neq 0 (
    echo Failed to restore NuGet packages.
    pause
    exit /b 1
)

REM Build the solution
echo Building solution...
msbuild SkyCASA.sln /p:Configuration=Release /p:Platform="Any CPU"
if %errorlevel% neq 0 (
    echo Build failed.
    pause
    exit /b 1
)

echo.
echo Build completed successfully!
echo Output files are in the bin\Release directory.
echo.
pause