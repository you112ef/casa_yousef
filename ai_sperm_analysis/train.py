#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Sky CASA - AI Sperm Analysis Training Script
YOLOv8 Model Training for Sperm Detection

Requirements:
pip install ultralytics opencv-python-headless numpy pandas scipy torch torchvision
"""

from ultralytics import YOLO
import os

def train_sperm_model():
    """
    تدريب نموذج الذكاء الاصطناعي لكشف الحيوانات المنوية
    """
    
    print("🧬 بدء تدريب نموذج كشف الحيوانات المنوية...")
    print("=" * 50)
    
    # تحميل النموذج الأساسي
    model = YOLO('yolov8n.pt')  # YOLOv8 Nano - سريع ومناسب للتطبيق الطبي
    
    # معاملات التدريب
    training_params = {
        'data': 'dataset/data.yaml',
        'epochs': 100,              # عدد دورات التدريب
        'imgsz': 640,              # حجم الصورة
        'batch': 8,                # حجم الدفعة
        'name': 'sperm-analyzer-v1', # اسم النموذج
        'patience': 20,            # عدد العصور بدون تحسن قبل التوقف
        'save': True,              # حفظ النموذج
        'val': True,               # التحقق من صحة النموذج
        'plots': True,             # رسوم بيانية للنتائج
        'device': 0,               # استخدام GPU إذا متوفر
        'workers': 4,              # عدد العمال لتحميل البيانات
        'project': './outputs',    # مجلد النتائج
        'exist_ok': True,          # السماح بالكتابة فوق المجلد
        'pretrained': True,        # استخدام النموذج المدرب مسبقاً
        'optimizer': 'AdamW',      # محسن التدريب
        'lr0': 0.01,              # معدل التعلم الأولي
        'weight_decay': 0.0005,    # انحلال الوزن
        'warmup_epochs': 3,        # عصور الإحماء
        'box': 7.5,               # وزن خسارة الصندوق
        'cls': 0.5,               # وزن خسارة التصنيف
        'dfl': 1.5,               # وزن خسارة DFL
    }
    
    # بدء التدريب
    try:
        results = model.train(**training_params)
        
        print("\n✅ تم التدريب بنجاح!")
        print(f"📁 موقع النموذج: ./outputs/sperm-analyzer-v1/weights/best.pt")
        print(f"📊 النتائج: {results}")
        
        # اختبار النموذج
        test_model()
        
    except Exception as e:
        print(f"❌ خطأ في التدريب: {e}")
        return False
    
    return True

def test_model():
    """
    اختبار النموذج المدرب
    """
    try:
        # تحميل النموذج المدرب
        model_path = "./outputs/sperm-analyzer-v1/weights/best.pt"
        
        if os.path.exists(model_path):
            model = YOLO(model_path)
            
            # اختبار على بيانات التحقق
            results = model.val(data='dataset/data.yaml')
            
            print("\n📊 نتائج الاختبار:")
            print(f"mAP50: {results.box.map50:.3f}")
            print(f"mAP50-95: {results.box.map:.3f}")
            print(f"Precision: {results.box.mp:.3f}")
            print(f"Recall: {results.box.mr:.3f}")
            
        else:
            print("❌ لم يتم العثور على النموذج المدرب")
            
    except Exception as e:
        print(f"❌ خطأ في الاختبار: {e}")

def export_model():
    """
    تصدير النموذج لأشكال مختلفة
    """
    try:
        model_path = "./outputs/sperm-analyzer-v1/weights/best.pt"
        
        if os.path.exists(model_path):
            model = YOLO(model_path)
            
            # تصدير إلى ONNX للاستخدام في التطبيقات
            model.export(format="onnx")
            print("✅ تم تصدير النموذج إلى ONNX")
            
            # تصدير إلى TorchScript للاستخدام المحمول
            model.export(format="torchscript")
            print("✅ تم تصدير النموذج إلى TorchScript")
            
            # تصدير إلى OpenVINO للأداء السريع
            # model.export(format="openvino")
            # print("✅ تم تصدير النموذج إلى OpenVINO")
            
        else:
            print("❌ لم يتم العثور على النموذج للتصدير")
            
    except Exception as e:
        print(f"❌ خطأ في التصدير: {e}")

if __name__ == "__main__":
    print("🚀 Sky CASA - AI Sperm Analysis Model Training")
    print("=" * 50)
    
    # التحقق من متطلبات النظام
    import torch
    print(f"PyTorch version: {torch.__version__}")
    print(f"CUDA available: {torch.cuda.is_available()}")
    
    # بدء التدريب
    if train_sperm_model():
        print("\n🎯 تصدير النموذج...")
        export_model()
        
        print("\n🎉 تم إنهاء العملية بنجاح!")
        print("🔗 يمكنك الآن استخدام النموذج في التطبيق الرئيسي")
    else:
        print("\n❌ فشل في التدريب")