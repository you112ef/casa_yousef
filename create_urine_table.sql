-- إنشاء جدول تحليل البول المحسن
-- Sky CASA Medical Laboratory Analysis System

-- إزالة الجدول القديم وإعادة تسميته
DROP TABLE IF EXISTS urine_old;
ALTER TABLE urine RENAME TO urine_old;

-- إنشاء جدول تحليل البول الجديد والصحيح
CREATE TABLE urine (
    -- Primary fields
    test_result_id INTEGER PRIMARY KEY AUTOINCREMENT,
    patient_id INTEGER NOT NULL,
    analysis_type_id INTEGER DEFAULT 2,
    technician_id INTEGER,
    test_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    -- Physical examination
    color TEXT,                          -- لون البول
    appearance TEXT,                     -- الشفافية
    specific_gravity DECIMAL(5,3),       -- الكثافة النوعية
    volume_ml INTEGER,                   -- الحجم بالمل
    
    -- Chemical examination
    ph DECIMAL(4,2),                     -- درجة الحموضة
    protein TEXT,                        -- البروتين
    glucose TEXT,                        -- الجلوكوز
    ketones TEXT,                        -- الكيتونات
    bilirubin TEXT,                      -- البيليروبين
    urobilinogen TEXT,                   -- اليوروبيلينوجين
    nitrites TEXT,                       -- النتريت
    leukocyte_esterase TEXT,             -- إنزيم الخلايا البيضاء
    blood TEXT,                          -- الدم
    ascorbic_acid TEXT,                  -- فيتامين ج
    
    -- Microscopic examination
    rbc_hpf TEXT,                        -- كريات الدم الحمراء
    wbc_hpf TEXT,                        -- كريات الدم البيضاء
    epithelial_cells TEXT,               -- الخلايا الطلائية
    bacteria TEXT,                       -- البكتيريا
    yeast TEXT,                          -- الخميرة
    parasites TEXT,                      -- الطفيليات
    crystals TEXT,                       -- البلورات
    casts TEXT,                          -- الأسطوانات
    mucus TEXT,                          -- المخاط
    sperm TEXT,                          -- الحيوانات المنوية
    
    -- Additional tests
    microalbumin DECIMAL(8,2),           -- الميكروألبومين
    creatinine DECIMAL(8,2),             -- الكرياتينين
    protein_creatinine_ratio DECIMAL(8,3), -- نسبة البروتين للكرياتينين
    
    -- Interpretations and flags
    physical_interpretation TEXT,
    chemical_interpretation TEXT,
    microscopic_interpretation TEXT,
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
CREATE INDEX idx_urine_patient_id ON urine(patient_id);
CREATE INDEX idx_urine_test_date ON urine(test_date);
CREATE INDEX idx_urine_technician_id ON urine(technician_id);
CREATE INDEX idx_urine_qc_status ON urine(qc_status);
CREATE INDEX idx_urine_critical_flag ON urine(critical_value_flag);

-- إضافة trigger لتحديث modified_date تلقائياً
CREATE TRIGGER urine_update_modified_date 
    AFTER UPDATE ON urine
    FOR EACH ROW
BEGIN
    UPDATE urine SET modified_date = CURRENT_TIMESTAMP WHERE test_result_id = NEW.test_result_id;
END;

-- نسخ البيانات القديمة إذا كانت متوفرة (mapping محدود)
INSERT INTO urine (patient_id, analysis_type_id, test_date)
SELECT 
    COALESCE(id_path, 1) as patient_id,
    2 as analysis_type_id,
    CURRENT_TIMESTAMP as test_date
FROM urine_old 
WHERE id_path IS NOT NULL;

-- إزالة الجدول القديم
DROP TABLE IF EXISTS urine_old;

-- إضافة بيانات تجريبية واقعية لاختبار النظام
INSERT INTO urine (
    patient_id, test_date, color, appearance, specific_gravity, ph, protein, glucose, 
    ketones, bilirubin, nitrites, leukocyte_esterase, blood, rbc_hpf, wbc_hpf, 
    epithelial_cells, bacteria, crystals, comments, qc_status
) VALUES 
(1, '2025-09-01 08:30:00', 'أصفر فاتح', 'صافي', 1.020, 6.0, 'سالب', 'سالب', 'سالب', 'سالب', 'سالب', 'سالب', 'سالب', '0-2', '0-3', 'قليل', 'لا يوجد', 'لا يوجد', 'تحليل بول طبيعي', 'Approved'),
(2, '2025-09-01 09:45:00', 'أصفر', 'عكر قليلاً', 1.025, 5.5, '+', 'سالب', 'سالب', 'سالب', '+', '+', '+', '5-10', '10-15', 'كثير', 'كثير', 'لا يوجد', 'التهاب في المسالك البولية', 'Approved'),
(3, '2025-09-01 11:00:00', 'أصفر داكن', 'صافي', 1.030, 6.5, 'سالب', '++', 'سالب', 'سالب', 'سالب', 'سالب', 'سالب', '0-1', '0-2', 'قليل', 'لا يوجد', 'أكسالات كالسيوم', 'ارتفاع السكر في البول', 'Review Required'),
(4, '2025-09-01 12:15:00', 'أحمر فاتح', 'عكر', 1.018, 7.0, '++', 'سالب', 'سالب', 'سالب', 'سالب', 'سالب', '+++', '25-30', '3-5', 'متوسط', 'قليل', 'لا يوجد', 'وجود دم في البول - يحتاج متابعة', 'Review Required');