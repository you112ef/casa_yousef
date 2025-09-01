using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;

/// <summary>
/// SQL Logic Error Detection and Fixing System
/// This class can be integrated into the Sky CASA application to detect and log SQL logic errors
/// </summary>
public class SQLLogicErrorDetector
{
    private string logFilePath;
    private string databasePath;
    
    public SQLLogicErrorDetector(string dbPath, string logPath)
    {
        databasePath = dbPath;
        logFilePath = logPath;
    }
    
    /// <summary>
    /// Logs SQL queries with automatic logic error detection
    /// </summary>
    /// <param name="query">SQL query to execute</param>
    /// <param name="parameters">Query parameters</param>
    /// <returns>Query result</returns>
    public DataTable ExecuteQueryWithValidation(string query, params object[] parameters)
    {
        // Log the query
        LogQuery("Executing query", query, parameters);
        
        // Check for common logic errors
        var issues = CheckForLogicErrors(query);
        if (issues.Count > 0)
        {
            LogIssues(query, issues);
        }
        
        // Execute the query
        try
        {
            string connectionString = $"Data Source={databasePath};Version=3;";
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    // Add parameters if provided
                    if (parameters != null)
                    {
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            cmd.Parameters.AddWithValue($"@param{i}", parameters[i]);
                        }
                    }
                    
                    // Execute based on query type
                    if (query.Trim().ToUpper().StartsWith("SELECT"))
                    {
                        var adapter = new SQLiteDataAdapter(cmd);
                        var dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        LogQuery("Query executed successfully", query, parameters);
                        return dataTable;
                    }
                    else
                    {
                        int rowsAffected = cmd.ExecuteNonQuery();
                        LogQuery($"Query executed successfully. Rows affected: {rowsAffected}", query, parameters);
                        return null;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            LogError($"Query execution failed: {ex.Message}", query, parameters);
            throw;
        }
    }
    
    /// <summary>
    /// Checks for common SQL logic errors
    /// </summary>
    /// <param name="query">SQL query to check</param>
    /// <returns>List of detected issues</returns>
    public List<string> CheckForLogicErrors(string query)
    {
        var issues = new List<string>();
        string upperQuery = query.ToUpper().Trim();
        
        // Check for NULL comparison issues
        if (upperQuery.Contains("= NULL") || upperQuery.Contains("!= NULL") || upperQuery.Contains("<> NULL"))
        {
            issues.Add("DIRECT NULL COMPARISON: Use IS NULL or IS NOT NULL instead of = NULL or != NULL");
        }
        
        // Check for missing WHERE clause in UPDATE/DELETE
        if ((upperQuery.StartsWith("UPDATE") || upperQuery.StartsWith("DELETE")) && 
            !upperQuery.Contains("WHERE"))
        {
            issues.Add("MISSING WHERE CLAUSE: UPDATE/DELETE without WHERE will affect all rows");
        }
        
        // Check for Cartesian products (JOIN without ON condition)
        if (System.Text.RegularExpressions.Regex.IsMatch(upperQuery, @"JOIN\s+\w+\s*(,|WHERE|$)") && 
            !upperQuery.Contains(" ON "))
        {
            issues.Add("POSSIBLE CARTESIAN PRODUCT: JOIN without proper ON condition");
        }
        
        // Check for subquery issues
        if (System.Text.RegularExpressions.Regex.IsMatch(upperQuery, @"\w+\s*=\s*\(") && 
            !upperQuery.Contains("IN ("))
        {
            issues.Add("POSSIBLE SUBQUERY ISSUE: Single value comparison with subquery - consider using IN");
        }
        
        return issues;
    }
    
    /// <summary>
    /// Logs a query execution
    /// </summary>
    private void LogQuery(string message, string query, params object[] parameters)
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string logEntry = $"[{timestamp}] {message}\nQuery: {query}\n";
        
        if (parameters != null && parameters.Length > 0)
        {
            logEntry += "Parameters: " + string.Join(", ", parameters) + "\n";
        }
        
        logEntry += "---\n";
        File.AppendAllText(logFilePath, logEntry);
    }
    
    /// <summary>
    /// Logs detected issues
    /// </summary>
    private void LogIssues(string query, List<string> issues)
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string logEntry = $"[{timestamp}] SQL LOGIC ISSUES DETECTED\n";
        logEntry += $"Query: {query}\n";
        
        foreach (string issue in issues)
        {
            logEntry += $"ISSUE: {issue}\n";
        }
        
        logEntry += "---\n";
        File.AppendAllText(logFilePath, logEntry);
    }
    
    /// <summary>
    /// Logs errors
    /// </summary>
    private void LogError(string message, string query, params object[] parameters)
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string logEntry = $"[{timestamp}] ERROR: {message}\nQuery: {query}\n";
        
        if (parameters != null && parameters.Length > 0)
        {
            logEntry += "Parameters: " + string.Join(", ", parameters) + "\n";
        }
        
        logEntry += "---\n";
        File.AppendAllText(logFilePath, logEntry);
    }
    
    /// <summary>
    /// Provides suggested fixes for common SQL logic errors
    /// </summary>
    /// <param name="issue">Detected issue</param>
    /// <returns>Suggested fix</returns>
    public string GetSuggestedFix(string issue)
    {
        switch (issue.ToUpper())
        {
            case var s when s.Contains("NULL COMPARISON"):
                return "Replace '= NULL' with 'IS NULL' and '!= NULL' or '<> NULL' with 'IS NOT NULL'";
                
            case var s when s.Contains("MISSING WHERE CLAUSE"):
                return "Add a WHERE clause to limit the rows affected by UPDATE/DELETE operations";
                
            case var s when s.Contains("CARTESIAN PRODUCT"):
                return "Add proper JOIN conditions using ON clause";
                
            case var s when s.Contains("SUBQUERY ISSUE"):
                return "Use IN operator instead of = when subquery might return multiple values";
                
            default:
                return "Review the query logic and ensure it follows SQL best practices";
        }
    }
}

/// <summary>
/// Example usage and integration patterns
/// </summary>
public class SQLFixExamples
{
    /// <summary>
    /// Example of how to integrate the SQL logic error detector
    /// </summary>
    public void ExampleIntegration()
    {
        var detector = new SQLLogicErrorDetector("database.db", "sql_fix_log.txt");
        
        // Example 1: Correct query
        try
        {
            var result1 = detector.ExecuteQueryWithValidation(
                "SELECT patient_id, first_name, last_name FROM patients WHERE last_name = @param0", 
                "Smith");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        
        // Example 2: Query with potential logic error (NULL comparison)
        try
        {
            var result2 = detector.ExecuteQueryWithValidation(
                "SELECT * FROM test_results WHERE result = NULL");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        
        // Example 3: Query with potential logic error (missing WHERE)
        try
        {
            var result3 = detector.ExecuteQueryWithValidation(
                "UPDATE patients SET last_name = 'Anonymous'");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Example of how to manually check queries for logic errors
    /// </summary>
    public void ManualCheckExample()
    {
        var detector = new SQLLogicErrorDetector("database.db", "sql_manual_check.log");
        
        string[] testQueries = {
            "SELECT * FROM patients WHERE middle_name = NULL",  // Error: NULL comparison
            "UPDATE test_results SET status = 'Reviewed'",      // Error: Missing WHERE
            "SELECT p.name, t.result FROM patients p JOIN test_results t", // Error: Cartesian product
            "SELECT * FROM patients WHERE id = (SELECT patient_id FROM test_results)" // Error: Subquery
        };
        
        foreach (string query in testQueries)
        {
            var issues = detector.CheckForLogicErrors(query);
            if (issues.Count > 0)
            {
                Console.WriteLine($"Issues found in query: {query}");
                foreach (string issue in issues)
                {
                    Console.WriteLine($"  - {issue}");
                    Console.WriteLine($"    Suggested fix: {detector.GetSuggestedFix(issue)}");
                }
            }
            else
            {
                Console.WriteLine($"No issues found in query: {query}");
            }
        }
    }
}