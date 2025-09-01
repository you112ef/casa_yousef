using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Business logic for CBC (Complete Blood Count) analysis type
/// </summary>
public class CBCBusinessLogic
{
    /// <summary>
    /// Validates a CBC test result
    /// </summary>
    /// <param name="cbcResult">The CBC test result to validate</param>
    /// <returns>Validation result with any errors found</returns>
    public static ValidationResult ValidateCBCTest(CBCTestResult cbcResult)
    {
        var result = new ValidationResult();
        
        // Validate required fields
        if (cbcResult.PatientID <= 0)
            result.Errors.Add("Patient ID is required");
            
        if (cbcResult.TestDate == DateTime.MinValue)
            result.Errors.Add("Test date is required");
            
        // Validate WBC
        if (cbcResult.WBC.HasValue)
        {
            if (cbcResult.WBC.Value < 0 || cbcResult.WBC.Value > 1000)
                result.Errors.Add("WBC value is out of reasonable range (0-1000)");
        }
        
        // Validate RBC
        if (cbcResult.RBC.HasValue)
        {
            if (cbcResult.RBC.Value < 0 || cbcResult.RBC.Value > 100)
                result.Errors.Add("RBC value is out of reasonable range (0-100)");
        }
        
        // Validate Hemoglobin
        if (cbcResult.Hemoglobin.HasValue)
        {
            if (cbcResult.Hemoglobin.Value < 0 || cbcResult.Hemoglobin.Value > 50)
                result.Errors.Add("Hemoglobin value is out of reasonable range (0-50)");
        }
        
        // Validate Hematocrit
        if (cbcResult.Hematocrit.HasValue)
        {
            if (cbcResult.Hematocrit.Value < 0 || cbcResult.Hematocrit.Value > 100)
                result.Errors.Add("Hematocrit value is out of reasonable range (0-100)");
        }
        
        // Validate Platelet Count
        if (cbcResult.PlateletCount.HasValue)
        {
            if (cbcResult.PlateletCount.Value < 0 || cbcResult.PlateletCount.Value > 5000)
                result.Errors.Add("Platelet count is out of reasonable range (0-5000)");
        }
        
        // Validate differential percentages sum to approximately 100%
        var diffSum = new List<decimal?>
        {
            cbcResult.NeutrophilsPercent,
            cbcResult.LymphocytesPercent,
            cbcResult.MonocytesPercent,
            cbcResult.EosinophilsPercent,
            cbcResult.BasophilsPercent,
            cbcResult.BandsPercent,
            cbcResult.AtypicalLymphocytesPercent
        }.Where(x => x.HasValue).Sum(x => x.Value);
        
        if (diffSum > 0 && (diffSum < 90 || diffSum > 110))
        {
            result.Warnings.Add($"Differential percentages sum to {diffSum:F1}%, expected approximately 100%");
        }
        
        // Validate calculated values consistency
        ValidateCalculatedValues(cbcResult, result);
        
        return result;
    }
    
    /// <summary>
    /// Validates consistency of calculated values
    /// </summary>
    private static void ValidateCalculatedValues(CBCTestResult cbcResult, ValidationResult result)
    {
        // Validate MCH: MCH = (HGB x 10) / RBC
        if (cbcResult.Hemoglobin.HasValue && cbcResult.RBC.HasValue && cbcResult.RBC.Value > 0 && cbcResult.MCH.HasValue)
        {
            var calculatedMCH = (cbcResult.Hemoglobin.Value * 10) / cbcResult.RBC.Value;
            var difference = Math.Abs(calculatedMCH - cbcResult.MCH.Value);
            if (difference > 1) // Allow 1 unit difference due to rounding
                result.Warnings.Add($"MCH value ({cbcResult.MCH.Value:F2}) doesn't match calculated value ({calculatedMCH:F2})");
        }
        
        // Validate MCHC: MCHC = (HGB x 100) / HCT
        if (cbcResult.Hemoglobin.HasValue && cbcResult.Hematocrit.HasValue && cbcResult.Hematocrit.Value > 0 && cbcResult.MCHC.HasValue)
        {
            var calculatedMCHC = (cbcResult.Hemoglobin.Value * 100) / cbcResult.Hematocrit.Value;
            var difference = Math.Abs(calculatedMCHC - cbcResult.MCHC.Value);
            if (difference > 1) // Allow 1 unit difference due to rounding
                result.Warnings.Add($"MCHC value ({cbcResult.MCHC.Value:F2}) doesn't match calculated value ({calculatedMCHC:F2})");
        }
        
        // Validate MCV: MCV = (HCT x 10) / RBC
        if (cbcResult.Hematocrit.HasValue && cbcResult.RBC.HasValue && cbcResult.RBC.Value > 0 && cbcResult.MCV.HasValue)
        {
            var calculatedMCV = (cbcResult.Hematocrit.Value * 10) / cbcResult.RBC.Value;
            var difference = Math.Abs(calculatedMCV - cbcResult.MCV.Value);
            if (difference > 1) // Allow 1 unit difference due to rounding
                result.Warnings.Add($"MCV value ({cbcResult.MCV.Value:F2}) doesn't match calculated value ({calculatedMCV:F2})");
        }
    }
    
    /// <summary>
    /// Interprets CBC test results based on normal ranges
    /// </summary>
    /// <param name="cbcResult">The CBC test result to interpret</param>
    /// <param name="patientGender">The patient's gender for gender-specific ranges</param>
    /// <param name="patientAge">The patient's age for age-specific ranges</param>
    /// <returns>Interpretation result with findings</returns>
    public static InterpretationResult InterpretCBCTest(CBCTestResult cbcResult, string patientGender, int patientAge)
    {
        var result = new InterpretationResult();
        
        // Interpret WBC
        if (cbcResult.WBC.HasValue)
        {
            if (cbcResult.WBC.Value < 1.0m)
            {
                result.Findings.Add("WBC: Severe leukopenia - possible bone marrow suppression or overwhelming infection");
                result.CriticalValues.Add("WBC < 1.0 x10^9/L");
            }
            else if (cbcResult.WBC.Value < 4.0m)
            {
                result.Findings.Add("WBC: Leukopenia - increased risk of infection");
            }
            else if (cbcResult.WBC.Value > 50.0m)
            {
                result.Findings.Add("WBC: Severe leukocytosis - possible leukemia or severe infection");
                result.CriticalValues.Add("WBC > 50.0 x10^9/L");
            }
            else if (cbcResult.WBC.Value > 11.0m)
            {
                result.Findings.Add("WBC: Leukocytosis - possible infection, inflammation or stress");
            }
            else
            {
                result.Findings.Add("WBC: Normal");
            }
        }
        
        // Interpret RBC and related values
        InterpretRBCValues(cbcResult, patientGender, patientAge, result);
        
        // Interpret Platelet Count
        if (cbcResult.PlateletCount.HasValue)
        {
            if (cbcResult.PlateletCount.Value < 20)
            {
                result.Findings.Add("Platelet Count: Severe thrombocytopenia - high risk of spontaneous bleeding");
                result.CriticalValues.Add("Platelet Count < 20 x10^9/L");
            }
            else if (cbcResult.PlateletCount.Value < 50)
            {
                result.Findings.Add("Platelet Count: Moderate thrombocytopenia - risk of bleeding with trauma");
            }
            else if (cbcResult.PlateletCount.Value < 150)
            {
                result.Findings.Add("Platelet Count: Mild thrombocytopenia");
            }
            else if (cbcResult.PlateletCount.Value > 1000)
            {
                result.Findings.Add("Platelet Count: Severe thrombocytosis - possible thrombotic risk");
                result.CriticalValues.Add("Platelet Count > 1000 x10^9/L");
            }
            else if (cbcResult.PlateletCount.Value > 450)
            {
                result.Findings.Add("Platelet Count: Thrombocytosis");
            }
            else
            {
                result.Findings.Add("Platelet Count: Normal");
            }
        }
        
        // Interpret Differential Counts
        InterpretDifferentialCounts(cbcResult, result);
        
        // Determine if any critical values exist
        result.HasCriticalValues = result.CriticalValues.Count > 0;
        
        return result;
    }
    
    /// <summary>
    /// Interprets RBC and related values
    /// </summary>
    private static void InterpretRBCValues(CBCTestResult cbcResult, string patientGender, int patientAge, InterpretationResult result)
    {
        // Get gender-specific normal ranges
        decimal rbcMin = (patientGender.ToLower() == "female") ? 3.8m : 4.2m;
        decimal rbcMax = (patientGender.ToLower() == "female") ? 5.2m : 5.9m;
        decimal hgbMin = (patientGender.ToLower() == "female") ? 12.0m : 13.5m;
        decimal hgbMax = (patientGender.ToLower() == "female") ? 15.5m : 17.5m;
        decimal hctMin = (patientGender.ToLower() == "female") ? 35.5m : 38.3m;
        decimal hctMax = (patientGender.ToLower() == "female") ? 44.9m : 48.6m;
        
        // Interpret RBC
        if (cbcResult.RBC.HasValue)
        {
            if (cbcResult.RBC.Value < rbcMin)
                result.Findings.Add("RBC: Decreased - possible anemia");
            else if (cbcResult.RBC.Value > rbcMax)
                result.Findings.Add("RBC: Increased - possible polycythemia");
            else
                result.Findings.Add("RBC: Normal");
        }
        
        // Interpret Hemoglobin
        if (cbcResult.Hemoglobin.HasValue)
        {
            if (cbcResult.Hemoglobin.Value < 5.0m)
            {
                result.Findings.Add("Hemoglobin: Severe anemia - life-threatening");
                result.CriticalValues.Add("Hemoglobin < 5.0 g/dL");
            }
            else if (cbcResult.Hemoglobin.Value < hgbMin)
            {
                result.Findings.Add("Hemoglobin: Decreased - anemia");
            }
            else if (cbcResult.Hemoglobin.Value > 20.0m)
            {
                result.Findings.Add("Hemoglobin: Severe polycythemia");
                result.CriticalValues.Add("Hemoglobin > 20.0 g/dL");
            }
            else if (cbcResult.Hemoglobin.Value > hgbMax)
            {
                result.Findings.Add("Hemoglobin: Increased - possible polycythemia");
            }
            else
            {
                result.Findings.Add("Hemoglobin: Normal");
            }
        }
        
        // Interpret Hematocrit
        if (cbcResult.Hematocrit.HasValue)
        {
            if (cbcResult.Hematocrit.Value < hctMin)
                result.Findings.Add("Hematocrit: Decreased - possible anemia");
            else if (cbcResult.Hematocrit.Value > hctMax)
                result.Findings.Add("Hematocrit: Increased - possible dehydration or polycythemia");
            else
                result.Findings.Add("Hematocrit: Normal");
        }
        
        // Interpret MCV for anemia classification
        if (cbcResult.MCV.HasValue)
        {
            if (cbcResult.MCV.Value < 80)
                result.Findings.Add("MCV: Low - microcytic anemia (e.g., iron deficiency, thalassemia)");
            else if (cbcResult.MCV.Value > 100)
                result.Findings.Add("MCV: High - macrocytic anemia (e.g., B12/folate deficiency, liver disease)");
            else
                result.Findings.Add("MCV: Normal - normocytic anemia");
        }
    }
    
    /// <summary>
    /// Interprets differential counts
    /// </summary>
    private static void InterpretDifferentialCounts(CBCTestResult cbcResult, InterpretationResult result)
    {
        // Interpret Neutrophils
        if (cbcResult.NeutrophilsPercent.HasValue)
        {
            if (cbcResult.NeutrophilsPercent.Value > 75)
                result.Findings.Add("Neutrophils: Increased - neutrophilia (infection, inflammation, stress)");
            else if (cbcResult.NeutrophilsPercent.Value < 40)
                result.Findings.Add("Neutrophils: Decreased - neutropenia (infection risk, bone marrow suppression)");
            else
                result.Findings.Add("Neutrophils: Normal");
        }
        
        // Interpret Lymphocytes
        if (cbcResult.LymphocytesPercent.HasValue)
        {
            if (cbcResult.LymphocytesPercent.Value > 40)
                result.Findings.Add("Lymphocytes: Increased - lymphocytosis (viral infection, lymphoproliferative disorder)");
            else if (cbcResult.LymphocytesPercent.Value < 20)
                result.Findings.Add("Lymphocytes: Decreased - lymphopenia (immunodeficiency, stress)");
            else
                result.Findings.Add("Lymphocytes: Normal");
        }
        
        // Interpret Monocytes
        if (cbcResult.MonocytesPercent.HasValue)
        {
            if (cbcResult.MonocytesPercent.Value > 10)
                result.Findings.Add("Monocytes: Increased - monocytosis (chronic infection, inflammatory disease)");
            else if (cbcResult.MonocytesPercent.Value < 2)
                result.Findings.Add("Monocytes: Decreased - monocytopenia (rare, may indicate bone marrow suppression)");
            else
                result.Findings.Add("Monocytes: Normal");
        }
        
        // Interpret Eosinophils
        if (cbcResult.EosinophilsPercent.HasValue)
        {
            if (cbcResult.EosinophilsPercent.Value > 6)
                result.Findings.Add("Eosinophils: Increased - eosinophilia (allergy, parasitic infection, autoimmune disease)");
            else
                result.Findings.Add("Eosinophils: Normal");
        }
        
        // Interpret Basophils
        if (cbcResult.BasophilsPercent.HasValue)
        {
            if (cbcResult.BasophilsPercent.Value > 2)
                result.Findings.Add("Basophils: Increased - basophilia (allergic reactions, chronic myeloid leukemia)");
            else
                result.Findings.Add("Basophils: Normal");
        }
    }
    
    /// <summary>
    /// Calculates absolute counts from percentages and WBC
    /// </summary>
    /// <param name="percentage">The percentage value</param>
    /// <param name="wbc">The WBC count</param>
    /// <returns>Absolute count</returns>
    public static decimal? CalculateAbsoluteCount(decimal? percentage, decimal? wbc)
    {
        if (!percentage.HasValue || !wbc.HasValue)
            return null;
            
        return (percentage.Value * wbc.Value) / 100;
    }
    
    /// <summary>
    /// Detects critical values in CBC results
    /// </summary>
    /// <param name="cbcResult">The CBC test result to check</param>
    /// <returns>True if critical values are detected</returns>
    public static bool HasCriticalValues(CBCTestResult cbcResult)
    {
        // Check for critical WBC values
        if (cbcResult.WBC.HasValue && (cbcResult.WBC.Value < 1.0m || cbcResult.WBC.Value > 50.0m))
            return true;
            
        // Check for critical Hemoglobin values
        if (cbcResult.Hemoglobin.HasValue && (cbcResult.Hemoglobin.Value < 5.0m || cbcResult.Hemoglobin.Value > 20.0m))
            return true;
            
        // Check for critical Platelet values
        if (cbcResult.PlateletCount.HasValue && (cbcResult.PlateletCount.Value < 20 || cbcResult.PlateletCount.Value > 1000))
            return true;
            
        return false;
    }
}

/// <summary>
/// Represents a CBC test result
/// </summary>
public class CBCTestResult
{
    public int TestResultID { get; set; }
    public int PatientID { get; set; }
    public int AnalysisTypeID { get; set; }
    public int TechnicianID { get; set; }
    public DateTime TestDate { get; set; }
    
    // Basic CBC values
    public decimal? WBC { get; set; }
    public decimal? RBC { get; set; }
    public decimal? Hemoglobin { get; set; }
    public decimal? Hematocrit { get; set; }
    public decimal? MCV { get; set; }
    public decimal? MCH { get; set; }
    public decimal? MCHC { get; set; }
    public decimal? RDW { get; set; }
    public decimal? PlateletCount { get; set; }
    public decimal? MPV { get; set; }
    public decimal? PDW { get; set; }
    
    // Differential percentages
    public decimal? NeutrophilsPercent { get; set; }
    public decimal? LymphocytesPercent { get; set; }
    public decimal? MonocytesPercent { get; set; }
    public decimal? EosinophilsPercent { get; set; }
    public decimal? BasophilsPercent { get; set; }
    public decimal? BandsPercent { get; set; }
    public decimal? AtypicalLymphocytesPercent { get; set; }
    public decimal? NRBC { get; set; }
    
    // Absolute counts
    public decimal? NeutrophilsAbsolute { get; set; }
    public decimal? LymphocytesAbsolute { get; set; }
    public decimal? MonocytesAbsolute { get; set; }
    public decimal? EosinophilsAbsolute { get; set; }
    public decimal? BasophilsAbsolute { get; set; }
    public decimal? BandsAbsolute { get; set; }
    
    // Interpretation and flags
    public string WBCInterpretation { get; set; }
    public string RBCInterpretation { get; set; }
    public string HGBInterpretation { get; set; }
    public string PLTInterpretation { get; set; }
    public string DifferentialInterpretation { get; set; }
    public bool CriticalValueFlag { get; set; }
    public string Comments { get; set; }
    
    // Quality control
    public string QCStatus { get; set; }
    public int? ReviewedBy { get; set; }
    public DateTime? ReviewDate { get; set; }
}

/// <summary>
/// Represents validation results
/// </summary>
public class ValidationResult
{
    public List<string> Errors { get; set; } = new List<string>();
    public List<string> Warnings { get; set; } = new List<string>();
    
    public bool IsValid => Errors.Count == 0;
    public bool HasWarnings => Warnings.Count > 0;
}

/// <summary>
/// Represents interpretation results
/// </summary>
public class InterpretationResult
{
    public List<string> Findings { get; set; } = new List<string>();
    public List<string> CriticalValues { get; set; } = new List<string>();
    
    public bool HasCriticalValues { get; set; } = false;
}