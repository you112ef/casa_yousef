@echo off
echo === Sky CASA Database Test Compilation ===
echo.

REM تجميع برنامج الاختبار
echo تجميع برنامج اختبار قاعدة البيانات...
csc /r:System.Data.SQLite.dll TestDatabaseConnection.cs DataAccessLayer.cs DatabaseConnectionFix.cs

if %ERRORLEVEL% neq 0 (
    echo خطأ في التجميع!
    pause
    exit /b 1
)

echo تم التجميع بنجاح!
echo.

REM تشغيل برنامج الاختبار
echo تشغيل اختبار قاعدة البيانات...
echo.
TestDatabaseConnection.exe

echo.
echo انتهى الاختبار!
pause