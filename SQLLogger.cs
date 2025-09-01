using System;
using System.Data.SQLite;
using System.IO;

// This is a sample logger that can help identify SQL logic errors
// You would need to integrate this into your actual application

public class SQLLogger
{
    private string dbPath;
    private string logPath;
    
    public SQLLogger(string databasePath, string logFilePath)
    {
        dbPath = databasePath;
        logPath = logFilePath;
    }
    
    // Method to log SQL queries (integrate this into your data access layer)
    public void LogQuery(string query, params object[] parameters)
    {
        try
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string logEntry = "[" + timestamp + "] QUERY: " + query;
            if (parameters != null && parameters.Length > 0)
            {
                logEntry += " | PARAMETERS: " + string.Join(", ", parameters);
            }
            logEntry += Environment.NewLine;
            
            File.AppendAllText(logPath, logEntry);
        }
        catch (Exception ex)
        {
            // Handle logging errors
            Console.WriteLine("Error logging SQL query: " + ex.Message);
        }
    }
    
    // Method to test queries for common logic errors
    public void TestQuery(string query)
    {
        try
        {
            string connectionString = "Data Source=" + dbPath + ";Version=3;";
            
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    // For SELECT queries, we can check the result structure
                    if (query.Trim().ToUpper().StartsWith("SELECT"))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            LogQuery("QUERY EXECUTION SUCCESS - Columns: " + reader.FieldCount);
                            
                            // Log column names
                            string columns = "";
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                columns += reader.GetName(i) + ", ";
                            }
                            LogQuery("COLUMN NAMES: " + columns.TrimEnd(' ', ','));
                            
                            // Check if we have any rows
                            if (reader.HasRows)
                            {
                                LogQuery("QUERY RESULT: Has data rows");
                            }
                            else
                            {
                                LogQuery("QUERY RESULT: No data rows returned");
                            }
                        }
                    }
                    else
                    {
                        // For INSERT/UPDATE/DELETE, just execute and log affected rows
                        int rowsAffected = cmd.ExecuteNonQuery();
                        LogQuery("QUERY EXECUTION SUCCESS - Rows affected: " + rowsAffected);
                    }
                }
                
                connection.Close();
            }
        }
        catch (Exception ex)
        {
            LogQuery("QUERY EXECUTION ERROR: " + ex.Message);
            LogQuery("QUERY TEXT: " + query);
        }
    }
    
    // Common SQL logic error checks
    public void CheckForCommonErrors(string query)
    {
        query = query.ToUpper().Trim();
        
        // Check for missing WHERE clause in UPDATE/DELETE
        if ((query.StartsWith("UPDATE") || query.StartsWith("DELETE")) && 
            !query.Contains("WHERE"))
        {
            LogQuery("WARNING: UPDATE/DELETE query without WHERE clause - will affect all rows!");
        }
        
        // Check for SELECT * (not necessarily an error, but worth noting)
        if (query.StartsWith("SELECT *"))
        {
            LogQuery("NOTE: SELECT * query - consider specifying columns explicitly");
        }
        
        // Check for potential NULL comparison issues
        if (query.Contains("= NULL") || query.Contains("!= NULL") || query.Contains("<> NULL"))
        {
            LogQuery("WARNING: Direct NULL comparison detected - use IS NULL or IS NOT NULL instead");
        }
    }
}

// Example usage
class Program
{
    static void Main()
    {
        var logger = new SQLLogger("database.db", "sql_log.txt");
        
        // Example of how to use the logger
        string[] testQueries = {
            "SELECT * FROM users",
            "UPDATE users SET name = 'John' WHERE id = 1",
            "DELETE FROM orders", // Missing WHERE clause - potential issue
            "SELECT name FROM users WHERE age = NULL" // Incorrect NULL comparison
        };
        
        foreach (string query in testQueries)
        {
            logger.LogQuery("Testing query: " + query);
            logger.CheckForCommonErrors(query);
            logger.TestQuery(query);
            logger.LogQuery("---");
        }
        
        Console.WriteLine("SQL logging and testing complete.");
        Console.WriteLine("Check sql_log.txt for results.");
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}