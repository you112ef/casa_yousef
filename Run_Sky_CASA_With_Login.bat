@echo off
echo Starting Sky CASA Application with Login Verification...
echo ======================================================

REM Check if the login-enabled version exists
if exist "Sky_CASA_With_Login.exe" (
    echo Running Sky_CASA_With_Login.exe...
    start "" "Sky_CASA_With_Login.exe"
) else (
    echo Login-enabled version not found. Creating it now...
    call create_login_app.bat
    if exist "Sky_CASA_With_Login.exe" (
        echo Running Sky_CASA_With_Login.exe...
        start "" "Sky_CASA_With_Login.exe"
    ) else (
        echo Failed to create login-enabled version.
        echo Running original application...
        start "" "Sky_CASA.exe"
    )
)

echo.
echo Application should now be running.
pause
