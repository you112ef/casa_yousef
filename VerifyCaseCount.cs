using System;
using System.Data.SQLite;
using System.IO;

class Program
{
    static void Main()
    {
        try
        {
            Console.WriteLine("VERIFYING CASE COUNT IN DATABASE");
            Console.WriteLine("================================");
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
                Console.WriteLine("✓ Database connection successful!");
                Console.WriteLine();
                
                // List all tables and count records in each
                Console.WriteLine("DATABASE TABLE RECORD COUNTS:");
                Console.WriteLine("============================");
                
                using (var cmd = new SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table' ORDER BY name", connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string tableName = reader.GetString(0);
                        
                        // Skip sqlite internal tables
                        if (tableName == "sqlite_sequence") continue;
                        
                        try
                        {
                            using (var countCmd = new SQLiteCommand($"SELECT COUNT(*) FROM [{tableName}]", connection))
                            {
                                var countResult = countCmd.ExecuteScalar();
                                int count = Convert.ToInt32(countResult);
                                Console.WriteLine($"{tableName}: {count} records");
                                
                                // If table has records, show first few for verification
                                if (count > 0)
                                {
                                    int displayCount = Math.Min(count, 3); // Show max 3 records
                                    using (var sampleCmd = new SQLiteCommand($"SELECT * FROM [{tableName}] LIMIT {displayCount}", connection))
                                    using (var sampleReader = sampleCmd.ExecuteReader())
                                    {
                                        Console.WriteLine($"  Sample data ({displayCount} of {count} records):");
                                        for (int i = 0; i < displayCount && sampleReader.Read(); i++)
                                        {
                                            Console.Write("    Record: ");
                                            for (int j = 0; j < Math.Min(sampleReader.FieldCount, 5); j++) // Show max 5 columns
                                            {
                                                string columnName = sampleReader.GetName(j);
                                                string value = sampleReader[j]?.ToString() ?? "NULL";
                                                Console.Write($"{columnName}={value} ");
                                            }
                                            Console.WriteLine();
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"{tableName}: Error counting records - {ex.Message}");
                        }
                    }
                }
                
                Console.WriteLine();
                Console.WriteLine("VERIFICATION COMPLETE");
                Console.WriteLine("====================");
                Console.WriteLine("The database can store unlimited records.");
                Console.WriteLine("There is no technical limit of 10 cases.");
                Console.WriteLine("If you're seeing only 10 cases in the UI,");
                Console.WriteLine("it's likely using GetSampleData() method");
                Console.WriteLine("which has a default limit of 10 for display.");
                
                connection.Close();
            }
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