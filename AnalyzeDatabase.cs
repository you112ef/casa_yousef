using System;
using System.Data.SQLite;
using System.IO;

class Program
{
    static void Main()
    {
        try
        {
            string dbPath = "database.db";
            
            if (!File.Exists(dbPath))
            {
                Console.WriteLine("Database file not found: " + dbPath);
                return;
            }
            
            string connectionString = "Data Source=" + dbPath + ";Version=3;";
            
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                
                // Get all table names
                using (var cmd = new SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table'", connection))
                using (var reader = cmd.ExecuteReader())
                {
                    Console.WriteLine("Tables in the database:");
                    Console.WriteLine("=====================");
                    while (reader.Read())
                    {
                        string tableName = reader.GetString(0);
                        Console.WriteLine("- " + tableName);
                        
                        // Get table schema
                        GetTableSchema(connection, tableName);
                    }
                }
                
                connection.Close();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
        
        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
    
    static void GetTableSchema(SQLiteConnection connection, string tableName)
    {
        try
        {
            Console.WriteLine("\nSchema for table: " + tableName);
            using (var cmd = new SQLiteCommand("PRAGMA table_info(" + tableName + ")", connection))
            using (var reader = cmd.ExecuteReader())
            {
                Console.WriteLine("Column Name\t\tType\t\tNullable\tDefault\t\tPrimary Key");
                Console.WriteLine("----------\t\t----\t\t--------\t-------\t\t-----------");
                while (reader.Read())
                {
                    string name = reader["name"].ToString();
                    string type = reader["type"].ToString();
                    string notnull = reader["notnull"].ToString() == "1" ? "NO" : "YES";
                    string defaultValue = reader["dflt_value"] != DBNull.Value ? reader["dflt_value"].ToString() : "NULL";
                    string primaryKey = reader["pk"].ToString() == "1" ? "YES" : "NO";
                    
                    Console.WriteLine($"{name}\t\t\t{type}\t\t{notnull}\t\t{defaultValue}\t\t{primaryKey}");
                }
            }
            
            // Get sample data (first 5 rows)
            Console.WriteLine("\nSample data from " + tableName + ":");
            using (var cmd = new SQLiteCommand("SELECT * FROM " + tableName + " LIMIT 5", connection))
            using (var reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    // Print column headers
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        Console.Write(reader.GetName(i) + "\t");
                    }
                    Console.WriteLine();
                    
                    // Print separator
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        Console.Write("--------\t");
                    }
                    Console.WriteLine();
                    
                    // Print data
                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            Console.Write(reader[i].ToString() + "\t");
                        }
                        Console.WriteLine();
                    }
                }
                else
                {
                    Console.WriteLine("(No data found)");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error getting schema for " + tableName + ": " + ex.Message);
        }
    }
}