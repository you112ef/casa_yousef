@echo off
echo Sky CASA - Path Visualization Error Check
echo =======================================
echo.

echo Checking required AForge libraries:
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
echo Checking application configuration:
if exist "Sky_CASA.exe.config" (
    echo ✓ Sky_CASA.exe.config - Found
) else (
    echo ✗ Sky_CASA.exe.config - Missing
)

echo.
echo For detailed path visualization error fixes, please refer to:
echo PATH_VISUALIZATION_FIX.txt
echo.

pause