-- إنشاء جدول تحليل السائل المنوي مع الذكاء الاصطناعي
-- Sky CASA Medical Laboratory Analysis System - AI Sperm Analysis

CREATE TABLE IF NOT EXISTS semen_analysis (
    -- Primary fields
    test_result_id INTEGER PRIMARY KEY AUTOINCREMENT,
    patient_id INTEGER NOT NULL,
    analysis_type_id INTEGER DEFAULT 7,
    technician_id INTEGER,
    test_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    -- Sample collection information
    abstinence_days INTEGER,             -- فترة الامتناع بالأيام
    collection_time DATETIME,           -- وقت جمع العينة
    analysis_time DATETIME,             -- وقت بدء التحليل
    liquefaction_time INTEGER,          -- وقت الإسالة بالدقائق
    
    -- Physical examination
    volume_ml DECIMAL(4,1),              -- الحجم بالمل
    color TEXT,                          -- اللون
    consistency TEXT,                    -- القوام
    ph DECIMAL(3,1),                     -- درجة الحموضة
    viscosity TEXT,                      -- اللزوجة
    liquefaction TEXT,                   -- حالة الإسالة
    
    -- Basic sperm parameters
    concentration_million_ml DECIMAL(8,2), -- التركيز مليون/مل
    total_count_million DECIMAL(8,2),    -- العدد الكلي بالمليون
    motility_progressive_percent DECIMAL(5,2), -- الحركة التقدمية %
    motility_total_percent DECIMAL(5,2), -- الحركة الكلية %
    morphology_normal_percent DECIMAL(5,2), -- الشكل الطبيعي %
    viability_percent DECIMAL(5,2),      -- الحيوية %
    
    -- AI-powered CASA (Computer Assisted Sperm Analysis) metrics
    ai_analysis_performed BOOLEAN DEFAULT FALSE,
    ai_model_version TEXT,               -- إصدار نموذج الذكاء الاصطناعي
    ai_confidence_score DECIMAL(5,2),    -- معامل الثقة في التحليل
    
    -- Movement velocity parameters (AI calculated)
    vcl_um_s DECIMAL(8,2),               -- Curvilinear Velocity (μm/s)
    vsl_um_s DECIMAL(8,2),               -- Straight-line Velocity (μm/s)  
    vap_um_s DECIMAL(8,2),               -- Average Path Velocity (μm/s)
    lin_percent DECIMAL(5,2),            -- Linearity %
    str_percent DECIMAL(5,2),            -- Straightness %
    wob_percent DECIMAL(5,2),            -- Wobble %
    alh_um DECIMAL(6,2),                 -- Amplitude of Lateral Head displacement (μm)
    bcf_hz DECIMAL(6,2),                 -- Beat Cross Frequency (Hz)
    
    -- Motility classifications (AI detected)
    rapid_progressive_percent DECIMAL(5,2), -- Grade A: سريع ومتقدم
    slow_progressive_percent DECIMAL(5,2),  -- Grade B: بطيء ومتقدم
    non_progressive_percent DECIMAL(5,2),   -- Grade C: حركة في المكان
    immotile_percent DECIMAL(5,2),         -- Grade D: غير متحرك
    
    -- AI tracking data
    total_tracks_detected INTEGER,        -- عدد المسارات المكتشفة
    valid_tracks_count INTEGER,          -- عدد المسارات الصالحة
    tracking_duration_seconds DECIMAL(6,2), -- مدة التتبع
    frames_analyzed INTEGER,             -- عدد الإطارات المحللة
    detection_accuracy_percent DECIMAL(5,2), -- دقة الكشف
    
    -- Media files
    original_image_path TEXT,            -- مسار الصورة الأصلية
    original_video_path TEXT,            -- مسار الفيديو الأصلي
    analyzed_image_path TEXT,            -- مسار الصورة المحللة
    analyzed_video_path TEXT,            -- مسار الفيديو المحلل
    heatmap_image_path TEXT,             -- مسار خريطة الحرارة
    
    -- Additional biochemical tests
    fructose_mg_dl DECIMAL(6,2),         -- الفركتوز
    citric_acid_mg_dl DECIMAL(6,2),      -- حمض الستريك
    zinc_ug_ml DECIMAL(6,2),             -- الزنك
    alpha_glucosidase_mu_ml DECIMAL(8,2), -- إنزيم الألفا جلوكوزيداز
    
    -- White blood cells and other cells
    wbc_million_ml DECIMAL(6,2),         -- كريات الدم البيضاء
    round_cells_million_ml DECIMAL(6,2), -- الخلايا المدورة
    epithelial_cells TEXT,               -- الخلايا الطلائية
    bacteria TEXT,                       -- البكتيريا
    
    -- WHO reference values comparison
    who_concentration_normal BOOLEAN DEFAULT FALSE,
    who_motility_normal BOOLEAN DEFAULT FALSE,
    who_morphology_normal BOOLEAN DEFAULT FALSE,
    who_volume_normal BOOLEAN DEFAULT FALSE,
    
    -- Clinical interpretations
    concentration_interpretation TEXT,
    motility_interpretation TEXT,
    morphology_interpretation TEXT,
    overall_interpretation TEXT,
    fertility_potential TEXT,            -- إمكانية الإنجاب
    
    -- AI analysis insights
    ai_detected_anomalies TEXT,          -- الشذوذات المكتشفة بالذكاء الاصطناعي
    ai_recommendations TEXT,             -- توصيات الذكاء الاصطناعي
    
    -- Quality control and flags
    critical_value_flag BOOLEAN DEFAULT FALSE,
    abnormal_flag BOOLEAN DEFAULT FALSE,
    technical_issues TEXT,               -- مشاكل تقنية في التحليل
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
CREATE INDEX idx_semen_patient_id ON semen_analysis(patient_id);
CREATE INDEX idx_semen_test_date ON semen_analysis(test_date);
CREATE INDEX idx_semen_technician_id ON semen_analysis(technician_id);
CREATE INDEX idx_semen_ai_performed ON semen_analysis(ai_analysis_performed);
CREATE INDEX idx_semen_qc_status ON semen_analysis(qc_status);
CREATE INDEX idx_semen_critical_flag ON semen_analysis(critical_value_flag);

-- إضافة trigger لتحديث modified_date تلقائياً
CREATE TRIGGER semen_update_modified_date 
    AFTER UPDATE ON semen_analysis
    FOR EACH ROW
BEGIN
    UPDATE semen_analysis SET modified_date = CURRENT_TIMESTAMP WHERE test_result_id = NEW.test_result_id;
END;

-- إضافة بيانات تجريبية واقعية
INSERT INTO semen_analysis (
    patient_id, test_date, abstinence_days, volume_ml, ph, concentration_million_ml, 
    total_count_million, motility_progressive_percent, motility_total_percent, 
    morphology_normal_percent, viability_percent, ai_analysis_performed, ai_model_version,
    ai_confidence_score, vcl_um_s, vsl_um_s, lin_percent, rapid_progressive_percent,
    slow_progressive_percent, non_progressive_percent, immotile_percent,
    concentration_interpretation, motility_interpretation, morphology_interpretation,
    overall_interpretation, fertility_potential, who_concentration_normal,
    who_motility_normal, who_morphology_normal, comments, qc_status
) VALUES 
(1, '2025-09-01 09:00:00', 3, 3.2, 7.8, 45, 144, 58, 62, 14, 85, TRUE, 'YOLOv8-DeepSORT-v1.0',
 0.92, 52.3, 28.1, 53.7, 35, 23, 4, 38,
 'طبيعي', 'طبيعي', 'تحت الطبيعي', 'تحليل جيد مع انخفاض طفيف في الشكل', 'جيد', TRUE, TRUE, FALSE,
 'تحليل بالذكاء الاصطناعي - جودة عالية', 'Approved'),

(2, '2025-09-01 10:30:00', 2, 2.1, 7.2, 12, 25.2, 25, 35, 8, 65, TRUE, 'YOLOv8-DeepSORT-v1.0',
 0.88, 38.5, 15.2, 39.5, 15, 10, 10, 65,
 'منخفض', 'منخفض', 'منخفض جداً', 'قلة الحيوانات المنوية مع ضعف الحركة والشكل', 'ضعيف', FALSE, FALSE, FALSE,
 'يحتاج فحص إضافي ومراجعة طبية', 'Review Required'),

(3, '2025-09-01 12:00:00', 4, 4.1, 8.1, 68, 279.4, 65, 72, 18, 90, TRUE, 'YOLOv8-DeepSORT-v1.0',
 0.95, 68.2, 42.1, 61.8, 42, 23, 7, 28,
 'طبيعي', 'ممتاز', 'طبيعي', 'تحليل ممتاز - جميع المعايير طبيعية', 'ممتاز', TRUE, TRUE, TRUE,
 'تحليل مثالي بالذكاء الاصطناعي', 'Approved'),

(4, '2025-09-01 13:30:00', 6, 1.8, 6.9, 8, 14.4, 18, 22, 5, 55, TRUE, 'YOLOv8-DeepSORT-v1.0',
 0.85, 28.1, 8.5, 30.2, 8, 10, 4, 78,
 'منخفض جداً', 'ضعيف جداً', 'منخفض جداً', 'قصور شديد في الحيوانات المنوية', 'ضعيف جداً', FALSE, FALSE, FALSE,
 'يحتاج تقييم شامل من طبيب الذكورة', 'Review Required');