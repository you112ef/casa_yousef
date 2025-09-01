@echo off
echo Installing AForge.NET dependencies via NuGet...

REM Check if NuGet.exe exists, if not download it
if not exist "nuget.exe" (
    echo Downloading NuGet.exe...
    powershell -Command "Invoke-WebRequest https://dist.nuget.org/win-x86-commandline/latest/nuget.exe -OutFile nuget.exe"
)

REM Create packages.config file
echo Creating packages.config...
echo ^<?xml version="1.0" encoding="utf-8"?^> > packages.config
echo ^<packages^> >> packages.config
echo   ^<package id="AForge" version="2.2.5" targetFramework="net40" /^> >> packages.config
echo   ^<package id="AForge.Video" version="2.2.5" targetFramework="net40" /^> >> packages.config
echo   ^<package id="AForge.Video.DirectShow" version="2.2.5" targetFramework="net40" /^> >> packages.config
echo ^</packages^> >> packages.config

REM Install packages
echo Installing packages...
nuget.exe install packages.config -OutputDirectory packages

echo.
echo Installation complete.
echo Please copy the DLL files from the packages folder to this directory:
echo - AForge.dll
echo - AForge.Video.dll
echo - AForge.Video.DirectShow.dll
echo.
pause