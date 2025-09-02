using System;
using System.Data;
using System.Data.SQLite;
using System.Collections.Generic;

/// <summary>
/// Database helper class for the Sky CASA application
/// </summary>
public static class DatabaseHelper
{
    private static string connectionString;
    
    /// <summary>
    /// Initializes the database helper with a connection string
    /// </summary>
    /// <param name="dbPath">Path to the SQLite database file</param>
    public static void Initialize(string dbPath)
    {
        // Use improved connection bootstrap (creates DB if missing)
        connectionString = DatabaseConnectionFix.GetDatabaseConnection(dbPath);
        Logger.LogInfo($"Database helper initialized with connection string: {connectionString}");
    }
    
    public static DataTable ExecuteQuery(string query, params object[] parameters)
    {
        try
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    AddParameters(cmd, parameters);
                    var adapter = new SQLiteDataAdapter(cmd);
                    var dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    Logger.LogInfo($"Executed query: {query}");
                    return dataTable;
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, $"DatabaseHelper.ExecuteQuery: {query}");
            throw new Exception($"Error executing query: {ex.Message}", ex);
        }
    }
    
    public static int ExecuteNonQuery(string query, params object[] parameters)
    {
        try
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    AddParameters(cmd, parameters);
                    int result = cmd.ExecuteNonQuery();
                    Logger.LogInfo($"Executed non-query: {query}, affected rows: {result}");
                    return result;
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, $"DatabaseHelper.ExecuteNonQuery: {query}");
            throw new Exception($"Error executing non-query: {ex.Message}", ex);
        }
    }
    
    public static object ExecuteScalar(string query, params object[] parameters)
    {
        try
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    AddParameters(cmd, parameters);
                    object result = cmd.ExecuteScalar();
                    Logger.LogInfo($"Executed scalar query: {query}");
                    return result;
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, $"DatabaseHelper.ExecuteScalar: {query}");
            throw new Exception($"Error executing scalar query: {ex.Message}", ex);
        }
    }
    
    public static List<string> GetTableNames()
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
                    while (reader.Read()) tables.Add(reader.GetString(0));
                }
            }
            Logger.LogInfo($"Retrieved {tables.Count} table names from database");
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, "DatabaseHelper.GetTableNames");
            throw new Exception($"Error getting table names: {ex.Message}", ex);
        }
        return tables;
    }
    
    public static bool TableExists(string tableName)
    {
        try
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var cmd = new SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table' AND name=@tableName", connection))
                {
                    cmd.Parameters.AddWithValue("@tableName", tableName);
                    using (var reader = cmd.ExecuteReader())
                    {
                        bool exists = reader.HasRows;
                        Logger.LogInfo($"Table '{tableName}' exists: {exists}");
                        return exists;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, $"DatabaseHelper.TableExists: {tableName}");
            return false;
        }
    }
    
    public static bool CreateTableIfNotExists(string tableName, string columns)
    {
        try
        {
            if (TableExists(tableName))
            {
                Logger.LogInfo($"Table '{tableName}' already exists, skipping creation");
                return true;
            }
            string createTableQuery = $"CREATE TABLE {tableName} ({columns})";
            int result = ExecuteNonQuery(createTableQuery);
            Logger.LogInfo($"Created table '{tableName}' successfully");
            return result >= 0;
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, $"DatabaseHelper.CreateTableIfNotExists: {tableName}");
            return false;
        }
    }
    
    private static void AddParameters(SQLiteCommand cmd, object[] parameters)
    {
        if (parameters != null)
        {
            for (int i = 0; i < parameters.Length; i++)
                cmd.Parameters.AddWithValue($"@param{i}", parameters[i] ?? DBNull.Value);
        }
    }
}