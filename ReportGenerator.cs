using System;
using System.Data;
using System.Text;
using System.IO;

/// <summary>
/// Report generation utilities for the Sky CASA application
/// </summary>
public static class ReportGenerator
{
    /// <summary>
    /// Generates a patient summary report
    /// </summary>
    /// <param name="patientData">DataTable containing patient information</param>
    /// <param name="reportTitle">Title for the report</param>
    /// <returns>String containing the formatted report</returns>
    public static string GeneratePatientSummaryReport(DataTable patientData, string reportTitle = "Patient Summary Report")
    {
        try
        {
            StringBuilder report = new StringBuilder();
            
            // Report header
            report.AppendLine("=" + new string('=', reportTitle.Length) + "=");
            report.AppendLine($" {reportTitle}");
            report.AppendLine("=" + new string('=', reportTitle.Length) + "=");
            report.AppendLine($"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            report.AppendLine($"Total Patients: {patientData.Rows.Count}");
            report.AppendLine();
            
            // Column headers
            StringBuilder header = new StringBuilder();
            foreach (DataColumn column in patientData.Columns)
            {
                header.Append($"{column.ColumnName,-15}");
            }
            report.AppendLine(header.ToString());
            report.AppendLine(new string('-', Math.Max(header.Length, 50)));
            
            // Data rows
            foreach (DataRow row in patientData.Rows)
            {
                StringBuilder dataRow = new StringBuilder();
                foreach (DataColumn column in patientData.Columns)
                {
                    string value = row[column.ColumnName]?.ToString() ?? "";
                    dataRow.Append($"{value,-15}");
                }
                report.AppendLine(dataRow.ToString());
            }
            
            report.AppendLine();
            report.AppendLine("End of Report");
            
            Logger.LogInfo($"Generated patient summary report with {patientData.Rows.Count} records");
            return report.ToString();
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, "ReportGenerator.GeneratePatientSummaryReport");
            ErrorHandling.HandleException(ex, "Generating Patient Report");
            return $"Error generating report: {ex.Message}";
        }
    }
    
    /// <summary>
    /// Saves a report to a text file
    /// </summary>
    /// <param name="reportContent">Content of the report</param>
    /// <param name="fileName">Name of the file to save to</param>
    /// <returns>True if successful, false otherwise</returns>
    public static bool SaveReportToFile(string reportContent, string fileName)
    {
        try
        {
            // Ensure reports directory exists
            string reportsDirectory = "reports";
            if (!Directory.Exists(reportsDirectory))
            {
                Directory.CreateDirectory(reportsDirectory);
            }
            
            // Create full file path
            string fullPath = Path.Combine(reportsDirectory, fileName);
            
            // Write report to file
            File.WriteAllText(fullPath, reportContent, Encoding.UTF8);
            
            Logger.LogInfo($"Saved report to {fullPath}");
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, "ReportGenerator.SaveReportToFile");
            ErrorHandling.HandleException(ex, "Saving Report to File");
            return false;
        }
    }
    
    /// <summary>
    /// Generates a detailed CBC analysis report
    /// </summary>
    /// <param name="cbcData">DataTable containing CBC analysis data</param>
    /// <param name="patientName">Name of the patient</param>
    /// <returns>String containing the formatted report</returns>
    public static string GenerateCBCReport(DataTable cbcData, string patientName = "Unknown Patient")
    {
        try
        {
            StringBuilder report = new StringBuilder();
            
            // Report header
            string reportTitle = $"CBC Analysis Report for {patientName}";
            report.AppendLine("=" + new string('=', reportTitle.Length) + "=");
            report.AppendLine($" {reportTitle}");
            report.AppendLine("=" + new string('=', reportTitle.Length) + "=");
            report.AppendLine($"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            report.AppendLine();
            
            // Check if we have data
            if (cbcData.Rows.Count == 0)
            {
                report.AppendLine("No CBC data available for this patient.");
                return report.ToString();
            }
            
            // Display CBC data
            DataRow row = cbcData.Rows[0]; // Assuming one record per patient
            
            foreach (DataColumn column in cbcData.Columns)
            {
                string columnName = column.ColumnName;
                string value = row[columnName]?.ToString() ?? "N/A";
                report.AppendLine($"{columnName,-20}: {value}");
            }
            
            report.AppendLine();
            report.AppendLine("End of Report");
            
            Logger.LogInfo($"Generated CBC report for patient: {patientName}");
            return report.ToString();
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, "ReportGenerator.GenerateCBCReport");
            ErrorHandling.HandleException(ex, "Generating CBC Report");
            return $"Error generating report: {ex.Message}";
        }
    }
    
    /// <summary>
    /// Generates a summary statistics report
    /// </summary>
    /// <param name="dataTable">DataTable containing data to analyze</param>
    /// <param name="numericColumns">Array of column names to calculate statistics for</param>
    /// <returns>String containing the formatted statistics report</returns>
    public static string GenerateStatisticsReport(DataTable dataTable, string[] numericColumns)
    {
        try
        {
            StringBuilder report = new StringBuilder();
            
            // Report header
            string reportTitle = "Statistical Summary Report";
            report.AppendLine("=" + new string('=', reportTitle.Length) + "=");
            report.AppendLine($" {reportTitle}");
            report.AppendLine("=" + new string('=', reportTitle.Length) + "=");
            report.AppendLine($"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            report.AppendLine($"Total Records: {dataTable.Rows.Count}");
            report.AppendLine();
            
            // Calculate statistics for each numeric column
            foreach (string columnName in numericColumns)
            {
                if (dataTable.Columns.Contains(columnName))
                {
                    double sum = 0;
                    double min = double.MaxValue;
                    double max = double.MinValue;
                    int count = 0;
                    
                    foreach (DataRow row in dataTable.Rows)
                    {
                        if (row[columnName] != null && row[columnName] != DBNull.Value)
                        {
                            if (double.TryParse(row[columnName].ToString(), out double value))
                            {
                                sum += value;
                                if (value < min) min = value;
                                if (value > max) max = value;
                                count++;
                            }
                        }
                    }
                    
                    if (count > 0)
                    {
                        double average = sum / count;
                        report.AppendLine($"{columnName}:");
                        report.AppendLine($"  Count: {count}");
                        report.AppendLine($"  Sum: {sum:F2}");
                        report.AppendLine($"  Average: {average:F2}");
                        report.AppendLine($"  Min: {min:F2}");
                        report.AppendLine($"  Max: {max:F2}");
                        report.AppendLine();
                    }
                }
            }
            
            report.AppendLine("End of Report");
            
            Logger.LogInfo($"Generated statistics report for {numericColumns.Length} columns");
            return report.ToString();
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, "ReportGenerator.GenerateStatisticsReport");
            ErrorHandling.HandleException(ex, "Generating Statistics Report");
            return $"Error generating report: {ex.Message}";
        }
    }
}