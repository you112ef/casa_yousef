@echo off
chcp 65001 >nul
title Sky CASA - اختبار سريع للنظام

echo.
echo ============================================
echo    Sky CASA - اختبار سريع للنظام
echo ============================================
echo.

echo 1. اختبار قاعدة البيانات...
echo.

sqlite3 database.db "SELECT 'مستخدمين إداريين: ' || COUNT(*) FROM admin;"
sqlite3 database.db "SELECT 'مرضى مسجلين: ' || COUNT(*) FROM patients;"
sqlite3 database.db "SELECT 'أطباء مسجلين: ' || COUNT(*) FROM doctors;"
sqlite3 database.db "SELECT 'فحوصات CBC: ' || COUNT(*) FROM cbc WHERE wbc IS NOT NULL;"
sqlite3 database.db "SELECT 'تحاليل البول: ' || COUNT(*) FROM urine WHERE color IS NOT NULL;"
sqlite3 database.db "SELECT 'تحاليل البراز: ' || COUNT(*) FROM stool WHERE color IS NOT NULL;"

echo.
echo 2. عرض مثال من بيانات المرضى:
echo.
sqlite3 database.db "SELECT first_name || ' ' || last_name as 'اسم المريض', gender as 'الجنس', phone as 'الهاتف' FROM patients LIMIT 3;"

echo.
echo 3. عرض مثال من نتائج CBC:
echo.
sqlite3 database.db "SELECT patient_id as 'رقم المريض', wbc as 'خلايا بيضاء', rbc as 'خلايا حمراء', hemoglobin as 'هيموجلوبين' FROM cbc WHERE wbc IS NOT NULL LIMIT 2;"

echo.
echo 4. معلومات تسجيل الدخول:
echo.
echo اسم المستخدم: admin
echo كلمة المرور: admin123
echo.

echo ============================================
echo النظام جاهز للاستخدام!
echo.
echo لتشغيل التطبيق الرئيسي:
echo Sky_CASA.exe
echo ============================================
echo.
pause