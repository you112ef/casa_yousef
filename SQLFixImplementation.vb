''' <summary>
''' SQL Logic Error Detection and Fixing System
''' This class can be integrated into the Sky CASA application to detect and log SQL logic errors
''' </summary>
Public Class SQLLogicErrorDetector
    Private logFilePath As String
    Private databasePath As String
    
    Public Sub New(dbPath As String, logPath As String)
        databasePath = dbPath
        logFilePath = logPath
    End Sub
    
    ''' <summary>
    ''' Logs SQL queries with automatic logic error detection
    ''' </summary>
    ''' <param name="query">SQL query to execute</param>
    ''' <param name="parameters">Query parameters</param>
    ''' <returns>Query result</returns>
    Public Function ExecuteQueryWithValidation(query As String, ParamArray parameters As Object()) As DataTable
        ' Log the query
        LogQuery("Executing query", query, parameters)
        
        ' Check for common logic errors
        Dim issues = CheckForLogicErrors(query)
        If issues.Count > 0 Then
            LogIssues(query, issues)
        End If
        
        ' Execute the query
        Try
            Dim connectionString As String = $"Data Source={databasePath};Version=3;"
            Using connection As New SQLiteConnection(connectionString)
                connection.Open()
                Using cmd As New SQLiteCommand(query, connection)
                    ' Add parameters if provided
                    If parameters IsNot Nothing Then
                        For i As Integer = 0 To parameters.Length - 1
                            cmd.Parameters.AddWithValue($"@param{i}", parameters(i))
                        Next
                    End If
                    
                    ' Execute based on query type
                    If query.Trim().ToUpper().StartsWith("SELECT") Then
                        Dim adapter As New SQLiteDataAdapter(cmd)
                        Dim dataTable As New DataTable()
                        adapter.Fill(dataTable)
                        LogQuery("Query executed successfully", query, parameters)
                        Return dataTable
                    Else
                        Dim rowsAffected As Integer = cmd.ExecuteNonQuery()
                        LogQuery($"Query executed successfully. Rows affected: {rowsAffected}", query, parameters)
                        Return Nothing
                    End If
                End Using
            End Using
        Catch ex As Exception
            LogError($"Query execution failed: {ex.Message}", query, parameters)
            Throw
        End Try
    End Function
    
    ''' <summary>
    ''' Checks for common SQL logic errors
    ''' </summary>
    ''' <param name="query">SQL query to check</param>
    ''' <returns>List of detected issues</returns>
    Public Function CheckForLogicErrors(query As String) As List(Of String)
        Dim issues As New List(Of String)
        Dim upperQuery As String = query.ToUpper().Trim()
        
        ' Check for NULL comparison issues
        If upperQuery.Contains("= NULL") OrElse upperQuery.Contains("!= NULL") OrElse upperQuery.Contains("<> NULL") Then
            issues.Add("DIRECT NULL COMPARISON: Use IS NULL or IS NOT NULL instead of = NULL or != NULL")
        End If
        
        ' Check for missing WHERE clause in UPDATE/DELETE
        If (upperQuery.StartsWith("UPDATE") OrElse upperQuery.StartsWith("DELETE")) AndAlso 
           Not upperQuery.Contains("WHERE") Then
            issues.Add("MISSING WHERE CLAUSE: UPDATE/DELETE without WHERE will affect all rows")
        End If
        
        ' Check for Cartesian products (JOIN without ON condition)
        If System.Text.RegularExpressions.Regex.IsMatch(upperQuery, "JOIN\s+\w+\s*(,|WHERE|$)") AndAlso 
           Not upperQuery.Contains(" ON ") Then
            issues.Add("POSSIBLE CARTESIAN PRODUCT: JOIN without proper ON condition")
        End If
        
        ' Check for subquery issues
        If System.Text.RegularExpressions.Regex.IsMatch(upperQuery, "\w+\s*=\s*\(") AndAlso 
           Not upperQuery.Contains("IN (") Then
            issues.Add("POSSIBLE SUBQUERY ISSUE: Single value comparison with subquery - consider using IN")
        End If
        
        Return issues
    End Function
    
    ''' <summary>
    ''' Logs a query execution
    ''' </summary>
    Private Sub LogQuery(message As String, query As String, parameters As Object())
        Dim timestamp As String = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        Dim logEntry As String = $"[{timestamp}] {message}" & vbCrLf & $"Query: {query}" & vbCrLf
        
        If parameters IsNot Nothing AndAlso parameters.Length > 0 Then
            logEntry += "Parameters: " & String.Join(", ", parameters) & vbCrLf
        End If
        
        logEntry += "---" & vbCrLf
        File.AppendAllText(logFilePath, logEntry)
    End Sub
    
    ''' <summary>
    ''' Logs detected issues
    ''' </summary>
    Private Sub LogIssues(query As String, issues As List(Of String))
        Dim timestamp As String = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        Dim logEntry As String = $"[{timestamp}] SQL LOGIC ISSUES DETECTED" & vbCrLf
        logEntry += $"Query: {query}" & vbCrLf
        
        For Each issue As String In issues
            logEntry += $"ISSUE: {issue}" & vbCrLf
        Next
        
        logEntry += "---" & vbCrLf
        File.AppendAllText(logFilePath, logEntry)
    End Sub
    
    ''' <summary>
    ''' Logs errors
    ''' </summary>
    Private Sub LogError(message As String, query As String, parameters As Object())
        Dim timestamp As String = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        Dim logEntry As String = $"[{timestamp}] ERROR: {message}" & vbCrLf & $"Query: {query}" & vbCrLf
        
        If parameters IsNot Nothing AndAlso parameters.Length > 0 Then
            logEntry += "Parameters: " & String.Join(", ", parameters) & vbCrLf
        End If
        
        logEntry += "---" & vbCrLf
        File.AppendAllText(logFilePath, logEntry)
    End Sub
    
    ''' <summary>
    ''' Provides suggested fixes for common SQL logic errors
    ''' </summary>
    ''' <param name="issue">Detected issue</param>
    ''' <returns>Suggested fix</returns>
    Public Function GetSuggestedFix(issue As String) As String
        Select Case issue.ToUpper()
            Case var As String When var.Contains("NULL COMPARISON")
                Return "Replace '= NULL' with 'IS NULL' and '!= NULL' or '<> NULL' with 'IS NOT NULL'"
                
            Case var As String When var.Contains("MISSING WHERE CLAUSE")
                Return "Add a WHERE clause to limit the rows affected by UPDATE/DELETE operations"
                
            Case var As String When var.Contains("CARTESIAN PRODUCT")
                Return "Add proper JOIN conditions using ON clause"
                
            Case var As String When var.Contains("SUBQUERY ISSUE")
                Return "Use IN operator instead of = when subquery might return multiple values"
                
            Case Else
                Return "Review the query logic and ensure it follows SQL best practices"
        End Select
    End Function
End Class

''' <summary>
''' Example usage and integration patterns
''' </summary>
Public Class SQLFixExamples
    ''' <summary>
    ''' Example of how to integrate the SQL logic error detector
    ''' </summary>
    Public Sub ExampleIntegration()
        Dim detector As New SQLLogicErrorDetector("database.db", "sql_fix_log.txt")
        
        ' Example 1: Correct query
        Try
            Dim result1 = detector.ExecuteQueryWithValidation(
                "SELECT patient_id, first_name, last_name FROM patients WHERE last_name = @param0", 
                "Smith")
        Catch ex As Exception
            Console.WriteLine($"Error: {ex.Message}")
        End Try
        
        ' Example 2: Query with potential logic error (NULL comparison)
        Try
            Dim result2 = detector.ExecuteQueryWithValidation(
                "SELECT * FROM test_results WHERE result = NULL")
        Catch ex As Exception
            Console.WriteLine($"Error: {ex.Message}")
        End Try
        
        ' Example 3: Query with potential logic error (missing WHERE)
        Try
            Dim result3 = detector.ExecuteQueryWithValidation(
                "UPDATE patients SET last_name = 'Anonymous'")
        Catch ex As Exception
            Console.WriteLine($"Error: {ex.Message}")
        End Try
    End Sub
    
    ''' <summary>
    ''' Example of how to manually check queries for logic errors
    ''' </summary>
    Public Sub ManualCheckExample()
        Dim detector As New SQLLogicErrorDetector("database.db", "sql_manual_check.log")
        
        Dim testQueries() As String = {
            "SELECT * FROM patients WHERE middle_name = NULL",  ' Error: NULL comparison
            "UPDATE test_results SET status = 'Reviewed'",      ' Error: Missing WHERE
            "SELECT p.name, t.result FROM patients p JOIN test_results t", ' Error: Cartesian product
            "SELECT * FROM patients WHERE id = (SELECT patient_id FROM test_results)" ' Error: Subquery
        }
        
        For Each query As String In testQueries
            Dim issues = detector.CheckForLogicErrors(query)
            If issues.Count > 0 Then
                Console.WriteLine($"Issues found in query: {query}")
                For Each issue As String In issues
                    Console.WriteLine($"  - {issue}")
                    Console.WriteLine($"    Suggested fix: {detector.GetSuggestedFix(issue)}")
                Next
            Else
                Console.WriteLine($"No issues found in query: {query}")
            End If
        Next
    End Sub
End Class