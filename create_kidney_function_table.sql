-- إنشاء جدول تحليل وظائف الكلى
-- Sky CASA Medical Laboratory Analysis System

CREATE TABLE IF NOT EXISTS kidney_function (
    -- Primary fields
    test_result_id INTEGER PRIMARY KEY AUTOINCREMENT,
    patient_id INTEGER NOT NULL,
    analysis_type_id INTEGER DEFAULT 5,
    technician_id INTEGER,
    test_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    -- Basic kidney function tests
    creatinine DECIMAL(6,2),             -- Creatinine (mg/dL)
    bun DECIMAL(6,1),                    -- Blood Urea Nitrogen (mg/dL)
    uric_acid DECIMAL(5,2),              -- Uric Acid (mg/dL)
    urea DECIMAL(6,1),                   -- Urea (mg/dL)
    
    -- Calculated values
    bun_creatinine_ratio DECIMAL(6,2),   -- BUN/Creatinine Ratio
    egfr DECIMAL(6,1),                   -- Estimated GFR (mL/min/1.73m²)
    creatinine_clearance DECIMAL(6,1),   -- Creatinine Clearance (mL/min)
    
    -- Electrolytes (kidney-related)
    sodium DECIMAL(5,1),                 -- Sodium (mEq/L)
    potassium DECIMAL(4,2),              -- Potassium (mEq/L)
    chloride DECIMAL(5,1),               -- Chloride (mEq/L)
    carbon_dioxide DECIMAL(4,1),         -- CO2 (mEq/L)
    
    -- Additional kidney markers
    albumin DECIMAL(4,2),                -- Albumin (g/dL)
    total_protein DECIMAL(4,2),          -- Total Protein (g/dL)
    phosphorus DECIMAL(4,2),             -- Phosphorus (mg/dL)
    calcium DECIMAL(4,2),                -- Calcium (mg/dL)
    
    -- Microalbumin (from urine)
    microalbumin_urine DECIMAL(8,2),     -- Microalbumin in urine (mg/L)
    albumin_creatinine_ratio DECIMAL(8,2), -- ACR (mg/g)
    
    -- Reference ranges and interpretations
    creatinine_interpretation TEXT,
    bun_interpretation TEXT,
    egfr_interpretation TEXT,
    overall_interpretation TEXT,
    
    -- Clinical significance
    kidney_stage TEXT,                   -- CKD Stage (1-5)
    risk_category TEXT,                  -- Low, Moderate, High, Very High
    
    -- Quality control and flags
    critical_value_flag BOOLEAN DEFAULT FALSE,
    abnormal_flag BOOLEAN DEFAULT FALSE,
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
CREATE INDEX idx_kidney_patient_id ON kidney_function(patient_id);
CREATE INDEX idx_kidney_test_date ON kidney_function(test_date);
CREATE INDEX idx_kidney_technician_id ON kidney_function(technician_id);
CREATE INDEX idx_kidney_qc_status ON kidney_function(qc_status);
CREATE INDEX idx_kidney_critical_flag ON kidney_function(critical_value_flag);

-- إضافة trigger لتحديث modified_date تلقائياً
CREATE TRIGGER kidney_update_modified_date 
    AFTER UPDATE ON kidney_function
    FOR EACH ROW
BEGIN
    UPDATE kidney_function SET modified_date = CURRENT_TIMESTAMP WHERE test_result_id = NEW.test_result_id;
END;

-- إضافة بيانات تجريبية واقعية
INSERT INTO kidney_function (
    patient_id, test_date, creatinine, bun, uric_acid, urea, bun_creatinine_ratio, 
    egfr, sodium, potassium, chloride, albumin, total_protein, phosphorus, calcium,
    creatinine_interpretation, bun_interpretation, egfr_interpretation, 
    overall_interpretation, kidney_stage, comments, qc_status
) VALUES 
(1, '2025-09-01 08:00:00', 1.0, 15, 5.2, 32, 15.0, 90, 140, 4.0, 102, 4.2, 7.1, 3.5, 9.8, 
 'طبيعي', 'طبيعي', 'طبيعي', 'وظائف الكلى طبيعية', 'Normal', 'جميع القيم ضمن المعدل الطبيعي', 'Approved'),

(2, '2025-09-01 09:30:00', 1.8, 28, 8.1, 60, 15.6, 45, 142, 4.8, 98, 3.8, 6.9, 4.2, 9.2, 
 'مرتفع', 'مرتفع', 'منخفض', 'اختلال في وظائف الكلى', 'Stage 3A', 'قصور كلوي متوسط - يحتاج متابعة', 'Review Required'),

(3, '2025-09-01 11:00:00', 0.8, 12, 4.8, 26, 15.0, 110, 138, 3.8, 104, 4.5, 7.3, 3.2, 10.1, 
 'طبيعي', 'طبيعي', 'طبيعي', 'وظائف الكلى ممتازة', 'Normal', 'وظائف كلى جيدة جداً', 'Approved'),

(4, '2025-09-01 12:30:00', 2.5, 45, 9.8, 96, 18.0, 25, 145, 5.2, 95, 3.2, 6.2, 5.1, 8.8, 
 'مرتفع جداً', 'مرتفع جداً', 'منخفض جداً', 'قصور كلوي شديد', 'Stage 4', 'قصور كلوي متقدم - يحتاج تدخل عاجل', 'Review Required'),

(5, '2025-09-01 14:00:00', 1.2, 18, 6.1, 38, 15.0, 75, 139, 4.2, 101, 4.0, 7.0, 3.8, 9.5, 
 'طبيعي', 'طبيعي', 'طبيعي مع انخفاض طفيف', 'وظائف كلى طبيعية مع مراقبة', 'Normal', 'ضمن المعدل الطبيعي مع المراقبة', 'Approved');