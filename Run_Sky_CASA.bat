@echo off
setlocal enableextensions
cls

echo ======================================
echo   Starting Sky CASA (build + launch)
echo ======================================

echo.
echo 1) Restoring and building the solution...
call build.bat
if errorlevel 1 (
  echo Build failed. Falling back to legacy EXE if available...
) else (
  echo Build completed.
)

echo.
echo 2) Launching application...
set EXE1="bin\Release\net472\Sky_CASA.exe"
set EXE2="bin\Release\Sky_CASA.exe"
set LEGACY="Sky_CASA.exe"

if exist %EXE1% (
  echo Running %EXE1% ...
  start "" %EXE1%
  goto :done
)

if exist %EXE2% (
  echo Running %EXE2% ...
  start "" %EXE2%
  goto :done
)

if exist %LEGACY% (
  echo Warning: Built EXE not found. Running legacy %LEGACY% ...
  start "" %LEGACY%
  goto :done
)

echo ERROR: No executable found to run.
exit /b 1

:done
echo.
echo Application should now be running.
pause
endlocal
