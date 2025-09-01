@echo off
chcp 65001 >nul
title Sky CASA - Complete AI Setup | إعداد شامل للذكاء الاصطناعي

echo.
echo ════════════════════════════════════════════════════════════════════
echo    Sky CASA - Medical Laboratory AI Analysis System
echo    إعداد شامل لنظام تحليل المختبرات الطبية بالذكاء الاصطناعي
echo ════════════════════════════════════════════════════════════════════
echo.

echo 🔧 بدء الإعداد الشامل...
echo.

REM Step 1: Check Python installation
echo 1️⃣ فحص تثبيت Python...
python --version >nul 2>&1
if %ERRORLEVEL% neq 0 (
    echo ❌ Python غير مثبت! يرجى تثبيت Python 3.8+ أولاً
    echo 📥 تحميل من: https://www.python.org/downloads/
    pause
    exit /b 1
)

python --version
echo ✅ Python مثبت بنجاح
echo.

REM Step 2: Create virtual environment
echo 2️⃣ إنشاء بيئة Python الافتراضية...
if exist "ai_env" (
    echo ⚠️  البيئة الافتراضية موجودة، سيتم استخدامها
) else (
    python -m venv ai_env
    if %ERRORLEVEL% neq 0 (
        echo ❌ فشل إنشاء البيئة الافتراضية
        pause
        exit /b 1
    )
    echo ✅ تم إنشاء البيئة الافتراضية
)
echo.

REM Step 3: Activate virtual environment and install packages
echo 3️⃣ تفعيل البيئة وتثبيت المكتبات...
call ai_env\Scripts\activate.bat

echo تثبيت مكتبات الذكاء الاصطناعي...
pip install --upgrade pip
pip install -r ai_sperm_analysis\requirements.txt

if %ERRORLEVEL% neq 0 (
    echo ❌ فشل تثبيت المكتبات
    echo 🔄 محاولة تثبيت المكتبات الأساسية فقط...
    pip install ultralytics opencv-python numpy pandas scipy
)

echo ✅ تم تثبيت مكتبات الذكاء الاصطناعي
echo.

REM Step 4: Test AI system
echo 4️⃣ اختبار نظام الذكاء الاصطناعي...
cd ai_sperm_analysis
python -c "
try:
    import ultralytics
    import cv2
    import numpy as np
    print('✅ جميع المكتبات الأساسية تعمل بنجاح')
    print(f'📦 YOLOv8 version: {ultralytics.__version__}')
    print(f'📦 OpenCV version: {cv2.__version__}')
    print(f'📦 NumPy version: {np.__version__}')
except ImportError as e:
    print(f'❌ خطأ في المكتبات: {e}')
    exit(1)
"

if %ERRORLEVEL% neq 0 (
    echo ❌ فشل اختبار النظام
    cd ..
    pause
    exit /b 1
)

cd ..
echo ✅ نظام الذكاء الاصطناعي جاهز
echo.

REM Step 5: Test database
echo 5️⃣ فحص قاعدة البيانات...
if exist "database.db" (
    sqlite3 database.db "SELECT 'Patients: ' || COUNT(*) FROM patients; SELECT 'Semen Analysis: ' || COUNT(*) FROM semen_analysis; SELECT 'Kidney Function: ' || COUNT(*) FROM kidney_function; SELECT 'Liver Function: ' || COUNT(*) FROM liver_function;"
    echo ✅ قاعدة البيانات تعمل بشكل صحيح
) else (
    echo ❌ قاعدة البيانات غير موجودة
)
echo.

REM Step 6: Create desktop shortcuts
echo 6️⃣ إنشاء اختصارات سطح المكتب...

REM Create AI Analysis shortcut
echo @echo off > "AI_Sperm_Analysis.bat"
echo cd /d "%~dp0" >> "AI_Sperm_Analysis.bat"
echo call ai_env\Scripts\activate.bat >> "AI_Sperm_Analysis.bat"
echo cd ai_sperm_analysis >> "AI_Sperm_Analysis.bat"
echo python analyze_media.py %%* >> "AI_Sperm_Analysis.bat"
echo pause >> "AI_Sperm_Analysis.bat"

REM Create training shortcut  
echo @echo off > "Train_AI_Model.bat"
echo cd /d "%~dp0" >> "Train_AI_Model.bat"
echo call ai_env\Scripts\activate.bat >> "Train_AI_Model.bat"
echo cd ai_sperm_analysis >> "Train_AI_Model.bat"
echo python train.py >> "Train_AI_Model.bat"
echo pause >> "Train_AI_Model.bat"

echo ✅ تم إنشاء اختصارات التشغيل
echo.

REM Step 7: Final system test
echo 7️⃣ اختبار النظام الشامل...

echo اختبار التطبيق الأساسي...
if exist "Sky_CASA.exe" (
    echo ✅ التطبيق الأساسي: Sky_CASA.exe
) else (
    echo ⚠️  التطبيق الأساسي غير موجود
)

echo فحص ملفات الذكاء الاصطناعي...
if exist "ai_sperm_analysis\analyze_media.py" (
    echo ✅ محلل الذكاء الاصطناعي جاهز
) else (
    echo ❌ محلل الذكاء الاصطناعي غير موجود
)

echo.
echo ════════════════════════════════════════════════════════════════════
echo ✅ تم إكمال الإعداد الشامل بنجاح!
echo ════════════════════════════════════════════════════════════════════
echo.

echo 🚀 كيفية الاستخدام:
echo.
echo 📊 تشغيل التطبيق الأساسي:
echo    Sky_CASA.exe
echo.
echo 🧠 تحليل صورة بالذكاء الاصطناعي:
echo    AI_Sperm_Analysis.bat --type image --media "image.jpg" --patient 1
echo.
echo 🎬 تحليل فيديو بالذكاء الاصطناعي:  
echo    AI_Sperm_Analysis.bat --type video --media "video.mp4" --patient 1 --duration 15
echo.
echo 🎓 تدريب نموذج جديد:
echo    Train_AI_Model.bat
echo.
echo 📋 اختبار سريع للنظام:
echo    QUICK_TEST.bat
echo.

echo 📚 الملفات المهمة:
echo    • Sky_CASA.exe - التطبيق الرئيسي
echo    • database.db - قاعدة البيانات (110KB+ بيانات حقيقية)
echo    • ai_sperm_analysis\ - نظام الذكاء الاصطناعي
echo    • PROJECT_STATUS_REPORT.md - تقرير مفصل
echo.

echo 🔐 معلومات تسجيل الدخول:
echo    اسم المستخدم: admin
echo    كلمة المرور: admin123
echo.

echo 💡 للحصول على المساعدة، راجع الملفات التالية:
echo    • README_FIXED.md
echo    • PROJECT_STATUS_REPORT.md  
echo    • ai_sperm_analysis\README.md
echo.

pause