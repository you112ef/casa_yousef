-- إنشاء جدول تحليل البراز المحسن
-- Sky CASA Medical Laboratory Analysis System

-- إزالة الجدول القديم وإعادة تسميته
DROP TABLE IF EXISTS stool_old;
ALTER TABLE stool RENAME TO stool_old;

-- إنشاء جدول تحليل البراز الجديد والصحيح
CREATE TABLE stool (
    -- Primary fields
    test_result_id INTEGER PRIMARY KEY AUTOINCREMENT,
    patient_id INTEGER NOT NULL,
    analysis_type_id INTEGER DEFAULT 3,
    technician_id INTEGER,
    test_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    -- Physical examination
    color TEXT,                          -- اللون
    consistency TEXT,                    -- القوام
    shape TEXT,                          -- الشكل
    odor TEXT,                           -- الرائحة
    volume_ml INTEGER,                   -- الحجم
    
    -- Microscopic examination
    rbc TEXT,                            -- كريات الدم الحمراء
    wbc TEXT,                            -- كريات الدم البيضاء
    epithelial_cells TEXT,               -- الخلايا الطلائية
    bacteria TEXT,                       -- البكتيريا
    yeast TEXT,                          -- الخميرة
    mucus TEXT,                          -- المخاط
    
    -- Parasitology
    protozoa TEXT,                       -- الأوليات
    helminths TEXT,                      -- الديدان
    cysts TEXT,                          -- الحويصلات
    trophozoites TEXT,                   -- الطور النشط للطفيليات
    ova TEXT,                            -- البيض
    larvae TEXT,                         -- اليرقات
    
    -- Chemical tests
    occult_blood TEXT,                   -- الدم الخفي
    ph DECIMAL(4,2),                     -- درجة الحموضة
    reducing_substances TEXT,            -- المواد المختزلة
    fat_globules TEXT,                   -- حبيبات الدهون
    starch_granules TEXT,                -- حبيبات النشا
    muscle_fibers TEXT,                  -- الألياف العضلية
    
    -- Digestive enzymes and substances
    elastase DECIMAL(8,2),               -- إنزيم الإيلاستاز
    calprotectin DECIMAL(8,2),           -- الكالبروتكتين
    lactoferrin DECIMAL(8,2),            -- اللاكتوفيرين
    
    -- Bacterial culture (if performed)
    culture_performed BOOLEAN DEFAULT FALSE,
    culture_result TEXT,
    antibiotic_sensitivity TEXT,
    
    -- Special stains and tests
    gram_stain TEXT,                     -- صبغة جرام
    acid_fast_stain TEXT,                -- الصبغة الحمضية السريعة
    methylene_blue_stain TEXT,           -- صبغة الميثيلين الأزرق
    
    -- Interpretations and flags
    physical_interpretation TEXT,
    microscopic_interpretation TEXT,
    parasitology_interpretation TEXT,
    chemical_interpretation TEXT,
    overall_interpretation TEXT,
    critical_value_flag BOOLEAN DEFAULT FALSE,
    comments TEXT,
    
    -- Quality control
    qc_status TEXT DEFAULT 'Pending',
    reviewed_by INTEGER,
    review_date DATETIME,
    
    -- Metadata
    created_date DATETIME DEFAULT CURRENT_TIMESTAMP,
    modified_date DATETIME DEFAULT CURRENT_TIMESTAMP,
    
    -- Foreign key constraints
    FOREIGN KEY (patient_id) REFERENCES patients(id),
    FOREIGN KEY (technician_id) REFERENCES admin(id),
    FOREIGN KEY (reviewed_by) REFERENCES admin(id)
);

-- إنشاء فهارس لتحسين الأداء
CREATE INDEX idx_stool_patient_id ON stool(patient_id);
CREATE INDEX idx_stool_test_date ON stool(test_date);
CREATE INDEX idx_stool_technician_id ON stool(technician_id);
CREATE INDEX idx_stool_qc_status ON stool(qc_status);
CREATE INDEX idx_stool_critical_flag ON stool(critical_value_flag);

-- إضافة trigger لتحديث modified_date تلقائياً
CREATE TRIGGER stool_update_modified_date 
    AFTER UPDATE ON stool
    FOR EACH ROW
BEGIN
    UPDATE stool SET modified_date = CURRENT_TIMESTAMP WHERE test_result_id = NEW.test_result_id;
END;

-- نسخ البيانات القديمة إذا كانت متوفرة (mapping محدود)
INSERT INTO stool (patient_id, analysis_type_id, test_date)
SELECT 
    COALESCE(id_path, 1) as patient_id,
    3 as analysis_type_id,
    CURRENT_TIMESTAMP as test_date
FROM stool_old 
WHERE id_path IS NOT NULL;

-- إزالة الجدول القديم
DROP TABLE IF EXISTS stool_old;

-- إضافة بيانات تجريبية واقعية لاختبار النظام
INSERT INTO stool (
    patient_id, test_date, color, consistency, shape, odor, rbc, wbc, epithelial_cells, 
    bacteria, mucus, protozoa, helminths, cysts, occult_blood, ph, comments, qc_status
) VALUES 
(1, '2025-09-01 09:00:00', 'بني طبيعي', 'متماسك', 'مُشكل', 'طبيعي', 'سالب', 'قليل', 'قليل', 'طبيعي', 'لا يوجد', 'لا يوجد', 'لا يوجد', 'لا يوجد', 'سالب', 7.2, 'فحص براز طبيعي', 'Approved'),
(2, '2025-09-01 10:30:00', 'أخضر فاتح', 'سائل', 'غير مُشكل', 'كريه', '+', '+++', 'كثير', 'كثير', 'كثير', 'لا يوجد', 'لا يوجد', 'لا يوجد', 'موجب', 6.8, 'إسهال مع التهاب معوي', 'Review Required'),
(3, '2025-09-01 11:45:00', 'بني داكن', 'صلب', 'كروي صغير', 'قوي', 'سالب', 'قليل', 'قليل', 'طبيعي', 'لا يوجد', 'Entamoeba coli cysts', 'لا يوجد', 'موجود', 'سالب', 7.5, 'وجود كيسات أميبا غير ممرضة', 'Approved'),
(4, '2025-09-01 13:00:00', 'أحمر داكن', 'طري مع دم', 'غير منتظم', 'دموي', '+++', '++', 'كثير', 'كثير', 'دموي', 'لا يوجد', 'لا يوجد', 'لا يوجد', 'موجب بقوة', 6.5, 'وجود دم في البراز - يتطلب فحص إضافي', 'Review Required');