''' <summary>
''' Business logic for CBC (Complete Blood Count) analysis type
''' </summary>
Public Class CBCBusinessLogic

    ''' <summary>
    ''' Validates a CBC test result
    ''' </summary>
    ''' <param name="cbcResult">The CBC test result to validate</param>
    ''' <returns>Validation result with any errors found</returns>
    Public Shared Function ValidateCBCTest(cbcResult As CBCTestResult) As ValidationResult
        Dim result As New ValidationResult()

        ' Validate required fields
        If cbcResult.PatientID <= 0 Then
            result.Errors.Add("Patient ID is required")
        End If

        If cbcResult.TestDate = DateTime.MinValue Then
            result.Errors.Add("Test date is required")
        End If

        ' Validate WBC
        If cbcResult.WBC.HasValue Then
            If cbcResult.WBC.Value < 0 Or cbcResult.WBC.Value > 1000 Then
                result.Errors.Add("WBC value is out of reasonable range (0-1000)")
            End If
        End If

        ' Validate RBC
        If cbcResult.RBC.HasValue Then
            If cbcResult.RBC.Value < 0 Or cbcResult.RBC.Value > 100 Then
                result.Errors.Add("RBC value is out of reasonable range (0-100)")
            End If
        End If

        ' Validate Hemoglobin
        If cbcResult.Hemoglobin.HasValue Then
            If cbcResult.Hemoglobin.Value < 0 Or cbcResult.Hemoglobin.Value > 50 Then
                result.Errors.Add("Hemoglobin value is out of reasonable range (0-50)")
            End If
        End If

        ' Validate Hematocrit
        If cbcResult.Hematocrit.HasValue Then
            If cbcResult.Hematocrit.Value < 0 Or cbcResult.Hematocrit.Value > 100 Then
                result.Errors.Add("Hematocrit value is out of reasonable range (0-100)")
            End If
        End If

        ' Validate Platelet Count
        If cbcResult.PlateletCount.HasValue Then
            If cbcResult.PlateletCount.Value < 0 Or cbcResult.PlateletCount.Value > 5000 Then
                result.Errors.Add("Platelet count is out of reasonable range (0-5000)")
            End If
        End If

        ' Validate differential percentages sum to approximately 100%
        Dim diffValues As New List(Of Decimal?)
        diffValues.Add(cbcResult.NeutrophilsPercent)
        diffValues.Add(cbcResult.LymphocytesPercent)
        diffValues.Add(cbcResult.MonocytesPercent)
        diffValues.Add(cbcResult.EosinophilsPercent)
        diffValues.Add(cbcResult.BasophilsPercent)
        diffValues.Add(cbcResult.BandsPercent)
        diffValues.Add(cbcResult.AtypicalLymphocytesPercent)

        Dim diffSum As Decimal = 0
        For Each value As Decimal? In diffValues
            If value.HasValue Then
                diffSum += value.Value
            End If
        Next

        If diffSum > 0 AndAlso (diffSum < 90 Or diffSum > 110) Then
            result.Warnings.Add($"Differential percentages sum to {diffSum:F1}%, expected approximately 100%")
        End If

        ' Validate calculated values consistency
        ValidateCalculatedValues(cbcResult, result)

        Return result
    End Function

    ''' <summary>
    ''' Validates consistency of calculated values
    ''' </summary>
    Private Shared Sub ValidateCalculatedValues(cbcResult As CBCTestResult, result As ValidationResult)
        ' Validate MCH: MCH = (HGB x 10) / RBC
        If cbcResult.Hemoglobin.HasValue AndAlso cbcResult.RBC.HasValue AndAlso cbcResult.RBC.Value > 0 AndAlso cbcResult.MCH.HasValue Then
            Dim calculatedMCH As Decimal = (cbcResult.Hemoglobin.Value * 10) / cbcResult.RBC.Value
            Dim difference As Decimal = Math.Abs(calculatedMCH - cbcResult.MCH.Value)
            If difference > 1 Then ' Allow 1 unit difference due to rounding
                result.Warnings.Add($"MCH value ({cbcResult.MCH.Value:F2}) doesn't match calculated value ({calculatedMCH:F2})")
            End If
        End If

        ' Validate MCHC: MCHC = (HGB x 100) / HCT
        If cbcResult.Hemoglobin.HasValue AndAlso cbcResult.Hematocrit.HasValue AndAlso cbcResult.Hematocrit.Value > 0 AndAlso cbcResult.MCHC.HasValue Then
            Dim calculatedMCHC As Decimal = (cbcResult.Hemoglobin.Value * 100) / cbcResult.Hematocrit.Value
            Dim difference As Decimal = Math.Abs(calculatedMCHC - cbcResult.MCHC.Value)
            If difference > 1 Then ' Allow 1 unit difference due to rounding
                result.Warnings.Add($"MCHC value ({cbcResult.MCHC.Value:F2}) doesn't match calculated value ({calculatedMCHC:F2})")
            End If
        End If

        ' Validate MCV: MCV = (HCT x 10) / RBC
        If cbcResult.Hematocrit.HasValue AndAlso cbcResult.RBC.HasValue AndAlso cbcResult.RBC.Value > 0 AndAlso cbcResult.MCV.HasValue Then
            Dim calculatedMCV As Decimal = (cbcResult.Hematocrit.Value * 10) / cbcResult.RBC.Value
            Dim difference As Decimal = Math.Abs(calculatedMCV - cbcResult.MCV.Value)
            If difference > 1 Then ' Allow 1 unit difference due to rounding
                result.Warnings.Add($"MCV value ({cbcResult.MCV.Value:F2}) doesn't match calculated value ({calculatedMCV:F2})")
            End If
        End If
    End Sub

    ''' <summary>
    ''' Interprets CBC test results based on normal ranges
    ''' </summary>
    ''' <param name="cbcResult">The CBC test result to interpret</param>
    ''' <param name="patientGender">The patient's gender for gender-specific ranges</param>
    ''' <param name="patientAge">The patient's age for age-specific ranges</param>
    ''' <returns>Interpretation result with findings</returns>
    Public Shared Function InterpretCBCTest(cbcResult As CBCTestResult, patientGender As String, patientAge As Integer) As InterpretationResult
        Dim result As New InterpretationResult()

        ' Interpret WBC
        If cbcResult.WBC.HasValue Then
            If cbcResult.WBC.Value < 1.0D Then
                result.Findings.Add("WBC: Severe leukopenia - possible bone marrow suppression or overwhelming infection")
                result.CriticalValues.Add("WBC < 1.0 x10^9/L")
            ElseIf cbcResult.WBC.Value < 4.0D Then
                result.Findings.Add("WBC: Leukopenia - increased risk of infection")
            ElseIf cbcResult.WBC.Value > 50.0D Then
                result.Findings.Add("WBC: Severe leukocytosis - possible leukemia or severe infection")
                result.CriticalValues.Add("WBC > 50.0 x10^9/L")
            ElseIf cbcResult.WBC.Value > 11.0D Then
                result.Findings.Add("WBC: Leukocytosis - possible infection, inflammation or stress")
            Else
                result.Findings.Add("WBC: Normal")
            End If
        End If

        ' Interpret RBC and related values
        InterpretRBCValues(cbcResult, patientGender, patientAge, result)

        ' Interpret Platelet Count
        If cbcResult.PlateletCount.HasValue Then
            If cbcResult.PlateletCount.Value < 20 Then
                result.Findings.Add("Platelet Count: Severe thrombocytopenia - high risk of spontaneous bleeding")
                result.CriticalValues.Add("Platelet Count < 20 x10^9/L")
            ElseIf cbcResult.PlateletCount.Value < 50 Then
                result.Findings.Add("Platelet Count: Moderate thrombocytopenia - risk of bleeding with trauma")
            ElseIf cbcResult.PlateletCount.Value < 150 Then
                result.Findings.Add("Platelet Count: Mild thrombocytopenia")
            ElseIf cbcResult.PlateletCount.Value > 1000 Then
                result.Findings.Add("Platelet Count: Severe thrombocytosis - possible thrombotic risk")
                result.CriticalValues.Add("Platelet Count > 1000 x10^9/L")
            ElseIf cbcResult.PlateletCount.Value > 450 Then
                result.Findings.Add("Platelet Count: Thrombocytosis")
            Else
                result.Findings.Add("Platelet Count: Normal")
            End If
        End If

        ' Interpret Differential Counts
        InterpretDifferentialCounts(cbcResult, result)

        ' Determine if any critical values exist
        result.HasCriticalValues = result.CriticalValues.Count > 0

        Return result
    End Function

    ''' <summary>
    ''' Interprets RBC and related values
    ''' </summary>
    Private Shared Sub InterpretRBCValues(cbcResult As CBCTestResult, patientGender As String, patientAge As Integer, result As InterpretationResult)
        ' Get gender-specific normal ranges
        Dim rbcMin As Decimal = If(patientGender.ToLower() = "female", 3.8D, 4.2D)
        Dim rbcMax As Decimal = If(patientGender.ToLower() = "female", 5.2D, 5.9D)
        Dim hgbMin As Decimal = If(patientGender.ToLower() = "female", 12.0D, 13.5D)
        Dim hgbMax As Decimal = If(patientGender.ToLower() = "female", 15.5D, 17.5D)
        Dim hctMin As Decimal = If(patientGender.ToLower() = "female", 35.5D, 38.3D)
        Dim hctMax As Decimal = If(patientGender.ToLower() = "female", 44.9D, 48.6D)

        ' Interpret RBC
        If cbcResult.RBC.HasValue Then
            If cbcResult.RBC.Value < rbcMin Then
                result.Findings.Add("RBC: Decreased - possible anemia")
            ElseIf cbcResult.RBC.Value > rbcMax Then
                result.Findings.Add("RBC: Increased - possible polycythemia")
            Else
                result.Findings.Add("RBC: Normal")
            End If
        End If

        ' Interpret Hemoglobin
        If cbcResult.Hemoglobin.HasValue Then
            If cbcResult.Hemoglobin.Value < 5.0D Then
                result.Findings.Add("Hemoglobin: Severe anemia - life-threatening")
                result.CriticalValues.Add("Hemoglobin < 5.0 g/dL")
            ElseIf cbcResult.Hemoglobin.Value < hgbMin Then
                result.Findings.Add("Hemoglobin: Decreased - anemia")
            ElseIf cbcResult.Hemoglobin.Value > 20.0D Then
                result.Findings.Add("Hemoglobin: Severe polycythemia")
                result.CriticalValues.Add("Hemoglobin > 20.0 g/dL")
            ElseIf cbcResult.Hemoglobin.Value > hgbMax Then
                result.Findings.Add("Hemoglobin: Increased - possible polycythemia")
            Else
                result.Findings.Add("Hemoglobin: Normal")
            End If
        End If

        ' Interpret Hematocrit
        If cbcResult.Hematocrit.HasValue Then
            If cbcResult.Hematocrit.Value < hctMin Then
                result.Findings.Add("Hematocrit: Decreased - possible anemia")
            ElseIf cbcResult.Hematocrit.Value > hctMax Then
                result.Findings.Add("Hematocrit: Increased - possible dehydration or polycythemia")
            Else
                result.Findings.Add("Hematocrit: Normal")
            End If
        End If

        ' Interpret MCV for anemia classification
        If cbcResult.MCV.HasValue Then
            If cbcResult.MCV.Value < 80 Then
                result.Findings.Add("MCV: Low - microcytic anemia (e.g., iron deficiency, thalassemia)")
            ElseIf cbcResult.MCV.Value > 100 Then
                result.Findings.Add("MCV: High - macrocytic anemia (e.g., B12/folate deficiency, liver disease)")
            Else
                result.Findings.Add("MCV: Normal - normocytic anemia")
            End If
        End If
    End Sub

    ''' <summary>
    ''' Interprets differential counts
    ''' </summary>
    Private Shared Sub InterpretDifferentialCounts(cbcResult As CBCTestResult, result As InterpretationResult)
        ' Interpret Neutrophils
        If cbcResult.NeutrophilsPercent.HasValue Then
            If cbcResult.NeutrophilsPercent.Value > 75 Then
                result.Findings.Add("Neutrophils: Increased - neutrophilia (infection, inflammation, stress)")
            ElseIf cbcResult.NeutrophilsPercent.Value < 40 Then
                result.Findings.Add("Neutrophils: Decreased - neutropenia (infection risk, bone marrow suppression)")
            Else
                result.Findings.Add("Neutrophils: Normal")
            End If
        End If

        ' Interpret Lymphocytes
        If cbcResult.LymphocytesPercent.HasValue Then
            If cbcResult.LymphocytesPercent.Value > 40 Then
                result.Findings.Add("Lymphocytes: Increased - lymphocytosis (viral infection, lymphoproliferative disorder)")
            ElseIf cbcResult.LymphocytesPercent.Value < 20 Then
                result.Findings.Add("Lymphocytes: Decreased - lymphopenia (immunodeficiency, stress)")
            Else
                result.Findings.Add("Lymphocytes: Normal")
            End If
        End If

        ' Interpret Monocytes
        If cbcResult.MonocytesPercent.HasValue Then
            If cbcResult.MonocytesPercent.Value > 10 Then
                result.Findings.Add("Monocytes: Increased - monocytosis (chronic infection, inflammatory disease)")
            ElseIf cbcResult.MonocytesPercent.Value < 2 Then
                result.Findings.Add("Monocytes: Decreased - monocytopenia (rare, may indicate bone marrow suppression)")
            Else
                result.Findings.Add("Monocytes: Normal")
            End If
        End If

        ' Interpret Eosinophils
        If cbcResult.EosinophilsPercent.HasValue Then
            If cbcResult.EosinophilsPercent.Value > 6 Then
                result.Findings.Add("Eosinophils: Increased - eosinophilia (allergy, parasitic infection, autoimmune disease)")
            Else
                result.Findings.Add("Eosinophils: Normal")
            End If
        End If

        ' Interpret Basophils
        If cbcResult.BasophilsPercent.HasValue Then
            If cbcResult.BasophilsPercent.Value > 2 Then
                result.Findings.Add("Basophils: Increased - basophilia (allergic reactions, chronic myeloid leukemia)")
            Else
                result.Findings.Add("Basophils: Normal")
            End If
        End If
    End Sub

    ''' <summary>
    ''' Calculates absolute counts from percentages and WBC
    ''' </summary>
    ''' <param name="percentage">The percentage value</param>
    ''' <param name="wbc">The WBC count</param>
    ''' <returns>Absolute count</returns>
    Public Shared Function CalculateAbsoluteCount(percentage As Decimal?, wbc As Decimal?) As Decimal?
        If Not percentage.HasValue Or Not wbc.HasValue Then
            Return Nothing
        End If

        Return (percentage.Value * wbc.Value) / 100
    End Function

    ''' <summary>
    ''' Detects critical values in CBC results
    ''' </summary>
    ''' <param name="cbcResult">The CBC test result to check</param>
    ''' <returns>True if critical values are detected</returns>
    Public Shared Function HasCriticalValues(cbcResult As CBCTestResult) As Boolean
        ' Check for critical WBC values
        If cbcResult.WBC.HasValue AndAlso (cbcResult.WBC.Value < 1.0D Or cbcResult.WBC.Value > 50.0D) Then
            Return True
        End If

        ' Check for critical Hemoglobin values
        If cbcResult.Hemoglobin.HasValue AndAlso (cbcResult.Hemoglobin.Value < 5.0D Or cbcResult.Hemoglobin.Value > 20.0D) Then
            Return True
        End If

        ' Check for critical Platelet values
        If cbcResult.PlateletCount.HasValue AndAlso (cbcResult.PlateletCount.Value < 20 Or cbcResult.PlateletCount.Value > 1000) Then
            Return True
        End If

        Return False
    End Function
End Class

''' <summary>
''' Represents a CBC test result
''' </summary>
Public Class CBCTestResult
    Public Property TestResultID As Integer
    Public Property PatientID As Integer
    Public Property AnalysisTypeID As Integer
    Public Property TechnicianID As Integer
    Public Property TestDate As DateTime

    ' Basic CBC values
    Public Property WBC As Decimal?
    Public Property RBC As Decimal?
    Public Property Hemoglobin As Decimal?
    Public Property Hematocrit As Decimal?
    Public Property MCV As Decimal?
    Public Property MCH As Decimal?
    Public Property MCHC As Decimal?
    Public Property RDW As Decimal?
    Public Property PlateletCount As Decimal?
    Public Property MPV As Decimal?
    Public Property PDW As Decimal?

    ' Differential percentages
    Public Property NeutrophilsPercent As Decimal?
    Public Property LymphocytesPercent As Decimal?
    Public Property MonocytesPercent As Decimal?
    Public Property EosinophilsPercent As Decimal?
    Public Property BasophilsPercent As Decimal?
    Public Property BandsPercent As Decimal?
    Public Property AtypicalLymphocytesPercent As Decimal?
    Public Property NRBC As Decimal?

    ' Absolute counts
    Public Property NeutrophilsAbsolute As Decimal?
    Public Property LymphocytesAbsolute As Decimal?
    Public Property MonocytesAbsolute As Decimal?
    Public Property EosinophilsAbsolute As Decimal?
    Public Property BasophilsAbsolute As Decimal?
    Public Property BandsAbsolute As Decimal?

    ' Interpretation and flags
    Public Property WBCInterpretation As String
    Public Property RBCInterpretation As String
    Public Property HGBInterpretation As String
    Public Property PLTInterpretation As String
    Public Property DifferentialInterpretation As String
    Public Property CriticalValueFlag As Boolean
    Public Property Comments As String

    ' Quality control
    Public Property QCStatus As String
    Public Property ReviewedBy As Integer?
    Public Property ReviewDate As DateTime?
End Class

''' <summary>
''' Represents validation results
''' </summary>
Public Class ValidationResult
    Public Property Errors As New List(Of String)
    Public Property Warnings As New List(Of String)

    Public ReadOnly Property IsValid As Boolean
        Get
            Return Errors.Count = 0
        End Get
    End Property

    Public ReadOnly Property HasWarnings As Boolean
        Get
            Return Warnings.Count > 0
        End Get
    End Property
End Class

''' <summary>
''' Represents interpretation results
''' </summary>
Public Class InterpretationResult
    Public Property Findings As New List(Of String)
    Public Property CriticalValues As New List(Of String)

    Public Property HasCriticalValues As Boolean = False
End Class