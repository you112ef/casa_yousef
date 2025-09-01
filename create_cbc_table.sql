-- إنشاء جدول CBC محسن لنتائج فحص الدم الشامل
-- Sky CASA Medical Laboratory Analysis System

-- إزالة الجدول القديم إذا كان موجوداً
DROP TABLE IF EXISTS cbc_old;
ALTER TABLE cbc RENAME TO cbc_old;

-- إنشاء جدول CBC جديد وصحيح
CREATE TABLE cbc (
    -- Primary fields
    test_result_id INTEGER PRIMARY KEY AUTOINCREMENT,
    patient_id INTEGER NOT NULL,
    analysis_type_id INTEGER DEFAULT 1,
    technician_id INTEGER,
    test_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    -- Basic CBC values
    wbc DECIMAL(8,3),                    -- White Blood Cells (thousands/µL)
    rbc DECIMAL(8,3),                    -- Red Blood Cells (millions/µL)
    hemoglobin DECIMAL(6,2),             -- Hemoglobin (g/dL)
    hematocrit DECIMAL(6,2),             -- Hematocrit (%)
    mcv DECIMAL(6,2),                    -- Mean Corpuscular Volume (fL)
    mch DECIMAL(6,2),                    -- Mean Corpuscular Hemoglobin (pg)
    mchc DECIMAL(6,2),                   -- Mean Corpuscular Hemoglobin Concentration (g/dL)
    rdw DECIMAL(6,2),                    -- Red Cell Distribution Width (%)
    platelet_count DECIMAL(8,0),         -- Platelet Count (thousands/µL)
    mpv DECIMAL(6,2),                    -- Mean Platelet Volume (fL)
    pdw DECIMAL(6,2),                    -- Platelet Distribution Width (%)
    
    -- Differential percentages
    neutrophils_percent DECIMAL(6,2),    -- Neutrophils (%)
    lymphocytes_percent DECIMAL(6,2),    -- Lymphocytes (%)
    monocytes_percent DECIMAL(6,2),      -- Monocytes (%)
    eosinophils_percent DECIMAL(6,2),    -- Eosinophils (%)
    basophils_percent DECIMAL(6,2),      -- Basophils (%)
    bands_percent DECIMAL(6,2),          -- Bands (%)
    atypical_lymphocytes_percent DECIMAL(6,2), -- Atypical Lymphocytes (%)
    nrbc DECIMAL(6,2),                   -- Nucleated RBCs per 100 WBCs
    
    -- Absolute counts (thousands/µL)
    neutrophils_absolute DECIMAL(8,3),
    lymphocytes_absolute DECIMAL(8,3),
    monocytes_absolute DECIMAL(8,3),
    eosinophils_absolute DECIMAL(8,3),
    basophils_absolute DECIMAL(8,3),
    bands_absolute DECIMAL(8,3),
    
    -- Interpretations
    wbc_interpretation TEXT,
    rbc_interpretation TEXT,
    hgb_interpretation TEXT,
    plt_interpretation TEXT,
    differential_interpretation TEXT,
    
    -- Flags and quality control
    critical_value_flag BOOLEAN DEFAULT FALSE,
    comments TEXT,
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
CREATE INDEX idx_cbc_patient_id ON cbc(patient_id);
CREATE INDEX idx_cbc_test_date ON cbc(test_date);
CREATE INDEX idx_cbc_technician_id ON cbc(technician_id);
CREATE INDEX idx_cbc_qc_status ON cbc(qc_status);
CREATE INDEX idx_cbc_critical_flag ON cbc(critical_value_flag);

-- إضافة trigger لتحديث modified_date تلقائياً
CREATE TRIGGER cbc_update_modified_date 
    AFTER UPDATE ON cbc
    FOR EACH ROW
BEGIN
    UPDATE cbc SET modified_date = CURRENT_TIMESTAMP WHERE test_result_id = NEW.test_result_id;
END;

-- نسخ البيانات القديمة إذا كانت متوفرة (mapping محدود)
INSERT INTO cbc (patient_id, analysis_type_id, test_date)
SELECT 
    COALESCE(idpath, 1) as patient_id,
    1 as analysis_type_id,
    CURRENT_TIMESTAMP as test_date
FROM cbc_old 
WHERE idpath IS NOT NULL;

-- إزالة الجدول القديم
DROP TABLE IF EXISTS cbc_old;

-- إضافة بيانات تجريبية واقعية لاختبار النظام
INSERT INTO cbc (
    patient_id, test_date, wbc, rbc, hemoglobin, hematocrit, mcv, mch, mchc, rdw, 
    platelet_count, neutrophils_percent, lymphocytes_percent, monocytes_percent,
    eosinophils_percent, basophils_percent, comments, qc_status
) VALUES 
(1, '2025-09-01 08:00:00', 7.2, 4.8, 14.2, 42.1, 87.8, 29.6, 33.7, 12.8, 280, 60.5, 30.2, 6.1, 2.8, 0.4, 'Normal CBC values', 'Approved'),
(2, '2025-09-01 09:15:00', 12.8, 4.2, 12.1, 36.5, 86.9, 28.8, 33.2, 14.2, 450, 75.2, 18.6, 4.8, 1.2, 0.2, 'Elevated WBC, possible infection', 'Approved'),
(3, '2025-09-01 10:30:00', 4.1, 5.1, 15.8, 46.2, 90.6, 31.0, 34.2, 11.9, 320, 55.8, 35.4, 5.9, 2.6, 0.3, 'Low WBC, monitor', 'Approved'),
(4, '2025-09-01 11:45:00', 8.9, 3.9, 11.5, 34.8, 89.2, 29.5, 33.0, 15.6, 180, 62.1, 28.9, 6.8, 1.9, 0.3, 'Low hemoglobin and platelet count', 'Review Required');