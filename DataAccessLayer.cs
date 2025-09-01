using System;
using System.Data;
using System.Data.SQLite;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// Data Access Layer for Sky CASA application
/// This class provides methods to safely retrieve and manipulate data from the SQLite database
/// </summary>
public class DataAccessLayer
{
    private string connectionString;
    
    public DataAccessLayer(string dbPath)
    {
        // Use our improved connection fixer
        connectionString = DatabaseConnectionFix.FixSQLiteConnection(dbPath);
    }
    
    /// <summary>
    /// Executes a SELECT query and returns a DataTable
    /// </summary>
    /// <param name="query">SQL SELECT query</param>
    /// <param name="parameters">Query parameters</param>
    /// <returns>DataTable with query results</returns>
    public DataTable ExecuteQuery(string query, params object[] parameters)
    {
        try
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    // Add parameters if provided
                    AddParameters(cmd, parameters);
                    
                    var adapter = new SQLiteDataAdapter(cmd);
                    var dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    
                    return dataTable;
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error executing query: {ex.Message}", ex);
        }
    }
    
    /// <summary>
    /// Executes a non-query SQL statement (INSERT, UPDATE, DELETE)
    /// </summary>
    /// <param name="query">SQL query</param>
    /// <param name="parameters">Query parameters</param>
    /// <returns>Number of affected rows</returns>
    public int ExecuteNonQuery(string query, params object[] parameters)
    {
        try
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    // Add parameters if provided
                    AddParameters(cmd, parameters);
                    
                    return cmd.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error executing non-query: {ex.Message}", ex);
        }
    }
    
    /// <summary>
    /// Executes a query that returns a single value
    /// </summary>
    /// <param name="query">SQL query</param>
    /// <param name="parameters">Query parameters</param>
    /// <returns>Single value result</returns>
    public object ExecuteScalar(string query, params object[] parameters)
    {
        try
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    // Add parameters if provided
                    AddParameters(cmd, parameters);
                    
                    return cmd.ExecuteScalar();
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error executing scalar query: {ex.Message}", ex);
        }
    }
    
    /// <summary>
    /// Adds parameters to a SQLite command
    /// </summary>
    private void AddParameters(SQLiteCommand cmd, object[] parameters)
    {
        if (parameters != null)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                cmd.Parameters.AddWithValue($"@param{i}", parameters[i] ?? DBNull.Value);
            }
        }
    }
    
    /// <summary>
    /// Gets all table names in the database
    /// </summary>
    /// <returns>List of table names</returns>
    public List<string> GetTableNames()
    {
        var tables = new List<string>();
        
        try
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                
                using (var cmd = new SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table'", connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tables.Add(reader.GetString(0));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error getting table names: {ex.Message}", ex);
        }
        
        return tables;
    }
    
    /// <summary>
    /// Gets schema information for a table
    /// </summary>
    /// <param name="tableName">Name of the table</param>
    /// <returns>DataTable with schema information</returns>
    public DataTable GetTableSchema(string tableName)
    {
        try
        {
            return ExecuteQuery($"PRAGMA table_info({tableName})");
        }
        catch (Exception ex)
        {
            throw new Exception($"Error getting table schema for {tableName}: {ex.Message}", ex);
        }
    }
    
    /// <summary>
    /// Gets sample data from a table
    /// </summary>
    /// <param name="tableName">Name of the table</param>
    /// <param name="limit">Maximum number of rows to retrieve</param>
    /// <returns>DataTable with sample data</returns>
    public DataTable GetSampleData(string tableName, int limit = 10)
    {
        try
        {
            return ExecuteQuery($"SELECT * FROM {tableName} LIMIT {limit}");
        }
        catch (Exception ex)
        {
            throw new Exception($"Error getting sample data from {tableName}: {ex.Message}", ex);
        }
    }
    
    /// <summary>
    /// Gets all data from a table without any limit
    /// </summary>
    /// <param name="tableName">Name of the table</param>
    /// <returns>DataTable with all data from the table</returns>
    public DataTable GetAllData(string tableName)
    {
        try
        {
            return ExecuteQuery($"SELECT * FROM {tableName}");
        }
        catch (Exception ex)
        {
            throw new Exception($"Error getting all data from {tableName}: {ex.Message}", ex);
        }
    }
    
    /// <summary>
    /// Safely retrieves a string value from a DataReader
    /// </summary>
    /// <param name="reader">DataReader object</param>
    /// <param name="columnName">Column name</param>
    /// <param name="defaultValue">Default value if column is null</param>
    /// <returns>String value</returns>
    public static string SafeGetString(IDataReader reader, string columnName, string defaultValue = "")
    {
        try
        {
            var ordinal = reader.GetOrdinal(columnName);
            if (reader.IsDBNull(ordinal))
                return defaultValue;
            
            return reader.GetString(ordinal);
        }
        catch
        {
            return defaultValue;
        }
    }
    
    /// <summary>
    /// Safely retrieves an integer value from a DataReader
    /// </summary>
    /// <param name="reader">DataReader object</param>
    /// <param name="columnName">Column name</param>
    /// <param name="defaultValue">Default value if column is null</param>
    /// <returns>Integer value</returns>
    public static int SafeGetInt32(IDataReader reader, string columnName, int defaultValue = 0)
    {
        try
        {
            var ordinal = reader.GetOrdinal(columnName);
            if (reader.IsDBNull(ordinal))
                return defaultValue;
            
            return reader.GetInt32(ordinal);
        }
        catch
        {
            return defaultValue;
        }
    }
    
    /// <summary>
    /// Safely retrieves a double value from a DataReader
    /// </summary>
    /// <param name="reader">DataReader object</param>
    /// <param name="columnName">Column name</param>
    /// <param name="defaultValue">Default value if column is null</param>
    /// <returns>Double value</returns>
    public static double SafeGetDouble(IDataReader reader, string columnName, double defaultValue = 0.0)
    {
        try
        {
            var ordinal = reader.GetOrdinal(columnName);
            if (reader.IsDBNull(ordinal))
                return defaultValue;
            
            return reader.GetDouble(ordinal);
        }
        catch
        {
            return defaultValue;
        }
    }
    
    /// <summary>
    /// Safely retrieves a DateTime value from a DataReader
    /// </summary>
    /// <param name="reader">DataReader object</param>
    /// <param name="columnName">Column name</param>
    /// <param name="defaultValue">Default value if column is null</param>
    /// <returns>DateTime value</returns>
    public static DateTime SafeGetDateTime(IDataReader reader, string columnName, DateTime defaultValue = default(DateTime))
    {
        try
        {
            var ordinal = reader.GetOrdinal(columnName);
            if (reader.IsDBNull(ordinal))
                return defaultValue;
            
            return reader.GetDateTime(ordinal);
        }
        catch
        {
            return defaultValue;
        }
    }
}

/// <summary>
/// Example usage of the DataAccessLayer
/// </summary>
public class DataAccessExamples
{
    public static void ExampleUsage()
    {
        try
        {
            // Create data access layer with improved connection handling
            var dal = new DataAccessLayer("database.db");
            
            // Get all table names
            var tables = dal.GetTableNames();
            Console.WriteLine("Tables in database:");
            foreach (var table in tables)
            {
                Console.WriteLine($"- {table}");
            }
            
            // Get sample data from first table
            if (tables.Count > 0)
            {
                var sampleData = dal.GetSampleData(tables[0], 5);
                Console.WriteLine($"\nSample data from {tables[0]}:");
                foreach (DataRow row in sampleData.Rows)
                {
                    foreach (DataColumn col in sampleData.Columns)
                    {
                        Console.Write($"{col.ColumnName}: {row[col]} | ");
                    }
                    Console.WriteLine();
                }
            }
            
            // Example of parameterized query
            var patientData = dal.ExecuteQuery(
                "SELECT * FROM patients WHERE last_name = @param0", 
                "Smith");
            
            // Example of insert operation
            var rowsAffected = dal.ExecuteNonQuery(
                "INSERT INTO patients (first_name, last_name) VALUES (@param0, @param1)",
                "John", "Doe");
            
            Console.WriteLine($"\nInserted {rowsAffected} rows");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}