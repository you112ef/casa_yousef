using System;
using System.Data.SQLite;
using System.IO;

class Program
{
    static void Main()
    {
        try
        {
            Console.WriteLine("Checking for case limits in database...");
            Console.WriteLine();
            
            string dbPath = "database.db";
            string connectionString = $"Data Source={dbPath};Version=3;";
            
            // Check if database exists
            if (!File.Exists(dbPath))
            {
                Console.WriteLine($"Error: Database file not found at {dbPath}");
                return;
            }
            
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                Console.WriteLine("Database connection successful!");
                Console.WriteLine();
                
                // List all tables
                Console.WriteLine("Database tables:");
                Console.WriteLine("===============");
                using (var cmd = new SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table'", connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string tableName = reader.GetString(0);
                        Console.WriteLine("- " + tableName);
                    }
                }
                
                Console.WriteLine();
                
                // Check for triggers
                Console.WriteLine("Database triggers:");
                Console.WriteLine("=================");
                using (var cmd = new SQLiteCommand("SELECT name, tbl_name FROM sqlite_master WHERE type='trigger'", connection))
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            string triggerName = reader.GetString(0);
                            string tableName = reader.GetString(1);
                            Console.WriteLine($"- {triggerName} on {tableName}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("No triggers found");
                    }
                }
                
                Console.WriteLine();
                
                // Check for any constraints or limits in the database
                Console.WriteLine("Checking for constraints that might limit cases...");
                Console.WriteLine("================================================");
                
                // Look for any tables that might store cases/patients
                string[] possibleCaseTables = { "patients", "cases", "records", "sem", "cbc", "stool", "urine", "vis" };
                
                foreach (string tableName in possibleCaseTables)
                {
                    using (var cmd = new SQLiteCommand($"SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='{tableName}'", connection))
                    {
                        var result = cmd.ExecuteScalar();
                        if (result != null && Convert.ToInt32(result) > 0)
                        {
                            Console.WriteLine($"Found table: {tableName}");
                            
                            // Count records in the table
                            using (var countCmd = new SQLiteCommand($"SELECT COUNT(*) FROM {tableName}", connection))
                            {
                                var countResult = countCmd.ExecuteScalar();
                                Console.WriteLine($"  Record count: {countResult}");
                            }
                            
                            // Check table schema for any constraints
                            using (var schemaCmd = new SQLiteCommand($"PRAGMA table_info({tableName})", connection))
                            using (var schemaReader = schemaCmd.ExecuteReader())
                            {
                                Console.WriteLine("  Schema:");
                                while (schemaReader.Read())
                                {
                                    string columnName = schemaReader["name"].ToString();
                                    string columnType = schemaReader["type"].ToString();
                                    string notNull = schemaReader["notnull"].ToString() == "1" ? "NOT NULL" : "";
                                    string defaultValue = schemaReader["dflt_value"].ToString();
                                    string primaryKey = schemaReader["pk"].ToString() == "1" ? "PRIMARY KEY" : "";
                                    
                                    Console.WriteLine($"    {columnName} {columnType} {notNull} {primaryKey} DEFAULT {defaultValue}");
                                }
                            }
                            
                            Console.WriteLine();
                        }
                    }
                }
                
                connection.Close();
            }
            
            Console.WriteLine("Database examination completed!");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            Console.WriteLine("Exception type: " + ex.GetType().FullName);
            
            if (ex.InnerException != null)
            {
                Console.WriteLine("Inner exception: " + ex.InnerException.Message);
            }
        }
        
        Console.WriteLine();
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}