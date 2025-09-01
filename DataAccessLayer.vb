''' <summary>
''' Data Access Layer for Sky CASA application
''' This class provides methods to safely retrieve and manipulate data from the SQLite database
''' </summary>
Public Class DataAccessLayer
    Private connectionString As String
    
    Public Sub New(dbPath As String)
        ' Validate database file exists
        If Not File.Exists(dbPath) Then
            Throw New FileNotFoundException($"Database file not found: {dbPath}")
        End If
        
        connectionString = $"Data Source={dbPath};Version=3;"
    End Sub
    
    ''' <summary>
    ''' Executes a SELECT query and returns a DataTable
    ''' </summary>
    ''' <param name="query">SQL SELECT query</param>
    ''' <param name="parameters">Query parameters</param>
    ''' <returns>DataTable with query results</returns>
    Public Function ExecuteQuery(query As String, ParamArray parameters As Object()) As DataTable
        Try
            Using connection As New SQLiteConnection(connectionString)
                connection.Open()
                
                Using cmd As New SQLiteCommand(query, connection)
                    ' Add parameters if provided
                    AddParameters(cmd, parameters)
                    
                    Dim adapter As New SQLiteDataAdapter(cmd)
                    Dim dataTable As New DataTable()
                    adapter.Fill(dataTable)
                    
                    Return dataTable
                End Using
            End Using
        Catch ex As Exception
            Throw New Exception($"Error executing query: {ex.Message}", ex)
        End Try
    End Function
    
    ''' <summary>
    ''' Executes a non-query SQL statement (INSERT, UPDATE, DELETE)
    ''' </summary>
    ''' <param name="query">SQL query</param>
    ''' <param name="parameters">Query parameters</param>
    ''' <returns>Number of affected rows</returns>
    Public Function ExecuteNonQuery(query As String, ParamArray parameters As Object()) As Integer
        Try
            Using connection As New SQLiteConnection(connectionString)
                connection.Open()
                
                Using cmd As New SQLiteCommand(query, connection)
                    ' Add parameters if provided
                    AddParameters(cmd, parameters)
                    
                    Return cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            Throw New Exception($"Error executing non-query: {ex.Message}", ex)
        End Try
    End Function
    
    ''' <summary>
    ''' Executes a query that returns a single value
    ''' </summary>
    ''' <param name="query">SQL query</param>
    ''' <param name="parameters">Query parameters</param>
    ''' <returns>Single value result</returns>
    Public Function ExecuteScalar(query As String, ParamArray parameters As Object()) As Object
        Try
            Using connection As New SQLiteConnection(connectionString)
                connection.Open()
                
                Using cmd As New SQLiteCommand(query, connection)
                    ' Add parameters if provided
                    AddParameters(cmd, parameters)
                    
                    Return cmd.ExecuteScalar()
                End Using
            End Using
        Catch ex As Exception
            Throw New Exception($"Error executing scalar query: {ex.Message}", ex)
        End Try
    End Function
    
    ''' <summary>
    ''' Adds parameters to a SQLite command
    ''' </summary>
    Private Sub AddParameters(cmd As SQLiteCommand, parameters As Object())
        If parameters IsNot Nothing Then
            For i As Integer = 0 To parameters.Length - 1
                cmd.Parameters.AddWithValue($"@param{i}", If(parameters(i), DBNull.Value))
            Next
        End If
    End Sub
    
    ''' <summary>
    ''' Gets all table names in the database
    ''' </summary>
    ''' <returns>List of table names</returns>
    Public Function GetTableNames() As List(Of String)
        Dim tables As New List(Of String)
        
        Try
            Using connection As New SQLiteConnection(connectionString)
                connection.Open()
                
                Using cmd As New SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table'", connection)
                    Using reader As SQLiteDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            tables.Add(reader.GetString(0))
                        End While
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Throw New Exception($"Error getting table names: {ex.Message}", ex)
        End Try
        
        Return tables
    End Function
    
    ''' <summary>
    ''' Gets schema information for a table
    ''' </summary>
    ''' <param name="tableName">Name of the table</param>
    ''' <returns>DataTable with schema information</returns>
    Public Function GetTableSchema(tableName As String) As DataTable
        Try
            Return ExecuteQuery($"PRAGMA table_info({tableName})")
        Catch ex As Exception
            Throw New Exception($"Error getting table schema for {tableName}: {ex.Message}", ex)
        End Try
    End Function
    
    ''' <summary>
    ''' Gets sample data from a table
    ''' </summary>
    ''' <param name="tableName">Name of the table</param>
    ''' <param name="limit">Maximum number of rows to retrieve</param>
    ''' <returns>DataTable with sample data</returns>
    Public Function GetSampleData(tableName As String, Optional limit As Integer = 10) As DataTable
        Try
            Return ExecuteQuery($"SELECT * FROM {tableName} LIMIT {limit}")
        Catch ex As Exception
            Throw New Exception($"Error getting sample data from {tableName}: {ex.Message}", ex)
        End Try
    End Function
    
    ''' <summary>
    ''' Gets all data from a table without any limit
    ''' </summary>
    ''' <param name="tableName">Name of the table</param>
    ''' <returns>DataTable with all data from the table</returns>
    Public Function GetAllData(tableName As String) As DataTable
        Try
            Return ExecuteQuery($"SELECT * FROM {tableName}")
        Catch ex As Exception
            Throw New Exception($"Error getting all data from {tableName}: {ex.Message}", ex)
        End Try
    End Function
    
    ''' <summary>
    ''' Safely retrieves a string value from a DataReader
    ''' </summary>
    ''' <param name="reader">DataReader object</param>
    ''' <param name="columnName">Column name</param>
    ''' <param name="defaultValue">Default value if column is null</param>
    ''' <returns>String value</returns>
    Public Shared Function SafeGetString(reader As IDataReader, columnName As String, Optional defaultValue As String = "") As String
        Try
            Dim ordinal As Integer = reader.GetOrdinal(columnName)
            If reader.IsDBNull(ordinal) Then
                Return defaultValue
            End If
            
            Return reader.GetString(ordinal)
        Catch
            Return defaultValue
        End Try
    End Function
    
    ''' <summary>
    ''' Safely retrieves an integer value from a DataReader
    ''' </summary>
    ''' <param name="reader">DataReader object</param>
    ''' <param name="columnName">Column name</param>
    ''' <param name="defaultValue">Default value if column is null</param>
    ''' <returns>Integer value</returns>
    Public Shared Function SafeGetInt32(reader As IDataReader, columnName As String, Optional defaultValue As Integer = 0) As Integer
        Try
            Dim ordinal As Integer = reader.GetOrdinal(columnName)
            If reader.IsDBNull(ordinal) Then
                Return defaultValue
            End If
            
            Return reader.GetInt32(ordinal)
        Catch
            Return defaultValue
        End Try
    End Function
    
    ''' <summary>
    ''' Safely retrieves a double value from a DataReader
    ''' </summary>
    ''' <param name="reader">DataReader object</param>
    ''' <param name="columnName">Column name</param>
    ''' <param name="defaultValue">Default value if column is null</param>
    ''' <returns>Double value</returns>
    Public Shared Function SafeGetDouble(reader As IDataReader, columnName As String, Optional defaultValue As Double = 0.0) As Double
        Try
            Dim ordinal As Integer = reader.GetOrdinal(columnName)
            If reader.IsDBNull(ordinal) Then
                Return defaultValue
            End If
            
            Return reader.GetDouble(ordinal)
        Catch
            Return defaultValue
        End Try
    End Function
    
    ''' <summary>
    ''' Safely retrieves a DateTime value from a DataReader
    ''' </summary>
    ''' <param name="reader">DataReader object</param>
    ''' <param name="columnName">Column name</param>
    ''' <param name="defaultValue">Default value if column is null</param>
    ''' <returns>DateTime value</returns>
    Public Shared Function SafeGetDateTime(reader As IDataReader, columnName As String, Optional defaultValue As DateTime = #1/1/0001#) As DateTime
        Try
            Dim ordinal As Integer = reader.GetOrdinal(columnName)
            If reader.IsDBNull(ordinal) Then
                Return defaultValue
            End If
            
            Return reader.GetDateTime(ordinal)
        Catch
            Return defaultValue
        End Try
    End Function
End Class

''' <summary>
''' Example usage of the DataAccessLayer
''' </summary>
Public Class DataAccessExamples
    Public Shared Sub ExampleUsage()
        Try
            ' Create data access layer
            Dim dal As New DataAccessLayer("database.db")
            
            ' Get all table names
            Dim tables = dal.GetTableNames()
            Console.WriteLine("Tables in database:")
            For Each table As String In tables
                Console.WriteLine($"- {table}")
            Next
            
            ' Get sample data from first table
            If tables.Count > 0 Then
                Dim sampleData = dal.GetSampleData(tables(0), 5)
                Console.WriteLine($"{vbLf}Sample data from {tables(0)}:")
                For Each row As DataRow In sampleData.Rows
                    For Each col As DataColumn In sampleData.Columns
                        Console.Write($"{col.ColumnName}: {row(col)} | ")
                    Next
                    Console.WriteLine()
                Next
            End If
            
            ' Example of parameterized query
            Dim patientData = dal.ExecuteQuery(
                "SELECT * FROM patients WHERE last_name = @param0",
                "Smith")
            
            ' Example of insert operation
            Dim rowsAffected = dal.ExecuteNonQuery(
                "INSERT INTO patients (first_name, last_name) VALUES (@param0, @param1)",
                "John", "Doe")
            
            Console.WriteLine($"{vbLf}Inserted {rowsAffected} rows")
        Catch ex As Exception
            Console.WriteLine($"Error: {ex.Message}")
        End Try
    End Sub
End Class