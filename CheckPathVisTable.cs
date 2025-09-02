using System;
using System.Data.SQLite;
using System.IO;

class Program
{
    static void Main()
    {
        try
        {
            Console.WriteLine("Checking for path_vis table in database...");
            Console.WriteLine("========================================");
            
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
                
                // Check if path_vis table exists
                bool tableExists = false;
                using (var cmd = new SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table' AND name='path_vis'", connection))
                using (var reader = cmd.ExecuteReader())
                {
                    tableExists = reader.HasRows;
                }
                
                if (tableExists)
                {
                    Console.WriteLine("✓ SUCCESS: path_vis table found in database!");
                    Console.WriteLine("The 'no such table: path_vis' error should now be resolved.");
                }
                else
                {
                    Console.WriteLine("✗ ISSUE: path_vis table NOT found in database!");
                    Console.WriteLine("The 'no such table: path_vis' error will still occur.");
                    Console.WriteLine();
                    Console.WriteLine("SOLUTION:");
                    Console.WriteLine("Please execute the add_path_vis_table.sql script using");
                    Console.WriteLine("a SQLite browser tool or command-line tool to create the table.");
                }
                
                Console.WriteLine();
                Console.WriteLine("All tables in database:");
                Console.WriteLine("=====================");
                using (var cmd = new SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table' ORDER BY name", connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string tableName = reader.GetString(0);
                        Console.WriteLine("- " + tableName);
                    }
                }
                
                connection.Close();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            Console.WriteLine("Exception type: " + ex.GetType().FullName);
        }
        
        Console.WriteLine();
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}