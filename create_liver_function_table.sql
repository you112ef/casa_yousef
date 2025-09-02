-- إنشاء جدول تحليل وظائف الكبد
-- Sky CASA Medical Laboratory Analysis System

CREATE TABLE IF NOT EXISTS liver_function (
    -- Primary fields
    test_result_id INTEGER PRIMARY KEY AUTOINCREMENT,
    patient_id INTEGER NOT NULL,
    analysis_type_id INTEGER DEFAULT 6,
    technician_id INTEGER,
    test_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    -- Liver enzymes
    alt DECIMAL(6,1),                    -- ALT/SGPT (U/L)
    ast DECIMAL(6,1),                    -- AST/SGOT (U/L)
    alp DECIMAL(6,1),                    -- Alkaline Phosphatase (U/L)
    ggt DECIMAL(6,1),                    -- Gamma-GT (U/L)
    ldh DECIMAL(6,1),                    -- Lactate Dehydrogenase (U/L)
    
    -- Bilirubin tests
    total_bilirubin DECIMAL(5,2),        -- Total Bilirubin (mg/dL)
    direct_bilirubin DECIMAL(5,2),       -- Direct Bilirubin (mg/dL)
    indirect_bilirubin DECIMAL(5,2),     -- Indirect Bilirubin (mg/dL)
    
    -- Protein synthesis tests
    total_protein DECIMAL(4,2),          -- Total Protein (g/dL)
    albumin DECIMAL(4,2),                -- Albumin (g/dL)
    globulin DECIMAL(4,2),               -- Globulin (g/dL)
    ag_ratio DECIMAL(4,2),               -- A/G Ratio
    
    -- Coagulation (liver synthesis)
    pt DECIMAL(4,1),                     -- Prothrombin Time (seconds)
    inr DECIMAL(4,2),                    -- INR
    ptt DECIMAL(4,1),                    -- Partial Thromboplastin Time (seconds)
    
    -- Additional liver markers
    ammonia DECIMAL(5,1),                -- Ammonia (μg/dL)
    alpha_fetoprotein DECIMAL(8,2),      -- Alpha-fetoprotein (ng/mL)
    
    -- Calculated ratios
    ast_alt_ratio DECIMAL(4,2),          -- AST/ALT ratio
    
    -- Reference ranges and interpretations
    alt_interpretation TEXT,
    ast_interpretation TEXT,
    alp_interpretation TEXT,
    bilirubin_interpretation TEXT,
    protein_interpretation TEXT,
    coagulation_interpretation TEXT,
    overall_interpretation TEXT,
    
    -- Clinical patterns
    hepatocellular_pattern BOOLEAN DEFAULT FALSE,  -- ALT/AST elevation
    cholestatic_pattern BOOLEAN DEFAULT FALSE,     -- ALP/GGT elevation
    mixed_pattern BOOLEAN DEFAULT FALSE,           -- Both patterns
    
    -- Severity assessment
    liver_injury_grade TEXT,             -- Mild, Moderate, Severe
    child_pugh_score INTEGER,            -- Child-Pugh Score (5-15)
    
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
CREATE INDEX idx_liver_patient_id ON liver_function(patient_id);
CREATE INDEX idx_liver_test_date ON liver_function(test_date);
CREATE INDEX idx_liver_technician_id ON liver_function(technician_id);
CREATE INDEX idx_liver_qc_status ON liver_function(qc_status);
CREATE INDEX idx_liver_critical_flag ON liver_function(critical_value_flag);

-- إضافة trigger لتحديث modified_date تلقائياً
CREATE TRIGGER liver_update_modified_date 
    AFTER UPDATE ON liver_function
    FOR EACH ROW
BEGIN
    UPDATE liver_function SET modified_date = CURRENT_TIMESTAMP WHERE test_result_id = NEW.test_result_id;
END;

-- إضافة بيانات تجريبية واقعية
INSERT INTO liver_function (
    patient_id, test_date, alt, ast, alp, ggt, total_bilirubin, direct_bilirubin, 
    indirect_bilirubin, total_protein, albumin, globulin, ag_ratio, pt, inr, 
    ast_alt_ratio, alt_interpretation, ast_interpretation, alp_interpretation, 
    bilirubin_interpretation, overall_interpretation, hepatocellular_pattern, 
    cholestatic_pattern, liver_injury_grade, comments, qc_status
) VALUES 
(1, '2025-09-01 08:15:00', 25, 30, 85, 28, 0.8, 0.3, 0.5, 7.2, 4.1, 3.1, 1.32, 12.5, 1.0, 
 1.20, 'طبيعي', 'طبيعي', 'طبيعي', 'طبيعي', 'وظائف الكبد طبيعية', FALSE, FALSE, 'Normal', 
 'جميع وظائف الكبد ضمن المعدل الطبيعي', 'Approved'),

(2, '2025-09-01 10:00:00', 85, 95, 120, 65, 1.8, 1.2, 0.6, 6.8, 3.2, 3.6, 0.89, 15.2, 1.3, 
 1.12, 'مرتفع', 'مرتفع', 'مرتفع قليلاً', 'مرتفع', 'التهاب كبدي خفيف', TRUE, FALSE, 'Mild', 
 'التهاب كبدي خفيف - يحتاج متابعة', 'Review Required'),

(3, '2025-09-01 11:30:00', 22, 28, 78, 25, 0.9, 0.4, 0.5, 7.5, 4.3, 3.2, 1.34, 11.8, 0.9, 
 1.27, 'طبيعي', 'طبيعي', 'طبيعي', 'طبيعي', 'وظائف كبد ممتازة', FALSE, FALSE, 'Normal', 
 'وظائف كبد صحية', 'Approved'),

(4, '2025-09-01 13:00:00', 180, 220, 95, 45, 3.2, 2.1, 1.1, 6.2, 2.8, 3.4, 0.82, 18.5, 1.6, 
 1.22, 'مرتفع جداً', 'مرتفع جداً', 'طبيعي', 'مرتفع جداً', 'التهاب كبدي متوسط إلى شديد', TRUE, FALSE, 'Moderate', 
 'التهاب كبدي حاد - يحتاج علاج فوري', 'Review Required'),

(5, '2025-09-01 14:30:00', 45, 38, 150, 85, 1.2, 0.5, 0.7, 7.0, 3.8, 3.2, 1.19, 13.2, 1.1, 
 0.84, 'مرتفع قليلاً', 'طبيعي', 'مرتفع', 'طبيعي', 'نمط ركودي خفيف', FALSE, TRUE, 'Mild', 
 'ركود صفراوي خفيف - متابعة مطلوبة', 'Approved');