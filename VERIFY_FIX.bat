@echo off
echo Verifying AForge.NET DLL installation...
echo.

echo Checking for required DLLs:
echo.

if exist "AForge.dll" (
    echo ✓ AForge.dll - Found
) else (
    echo ✗ AForge.dll - Missing
)

if exist "AForge.Video.dll" (
    echo ✓ AForge.Video.dll - Found
) else (
    echo ✗ AForge.Video.dll - Missing
)

if exist "AForge.Video.DirectShow.dll" (
    echo ✓ AForge.Video.DirectShow.dll - Found
) else (
    echo ✗ AForge.Video.DirectShow.dll - Missing
)

echo.
echo Checking for configuration file:
if exist "Sky_CASA.exe.config" (
    echo ✓ Sky_CASA.exe.config - Present (JIT debugging enabled)
) else (
    echo ✗ Sky_CASA.exe.config - Missing
)

echo.
echo If all DLLs are present, your application should now work without the missing assembly error.
echo.
pause