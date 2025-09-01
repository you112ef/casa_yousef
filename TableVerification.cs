using System;
using System.Data.SQLite;
using System.IO;

class Program
{
    static void Main()
    {
        Console.WriteLine("DEFINITIVE TABLE VERIFICATION");
        Console.WriteLine("============================");
        Console.WriteLine();
        
        try
        {
            string dbPath = "database.db";
            if (!File.Exists(dbPath))
            {
                Console.WriteLine("ERROR: Database file not found!");
                return;
            }
            
            Console.WriteLine("✓ Database file found");
            
            string connectionString = $"Data Source={dbPath};Version=3;";
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                Console.WriteLine("✓ Database connection established");
                Console.WriteLine();
                
                // Method 1: Check using sqlite_master
                Console.WriteLine("Method 1: Checking sqlite_master...");
                using (var cmd = new SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table' AND name='path_vis'", connection))
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        Console.WriteLine("✓ path_vis table found in sqlite_master");
                    }
                    else
                        {
                        Console.WriteLine("✗ path_vis table NOT found in sqlite_master");
                    }
                }
                
                // Method 2: Try to query the table directly
                Console.WriteLine();
                Console.WriteLine("Method 2: Direct table query test...");
                try
                {
                    using (var cmd = new SQLiteCommand("SELECT COUNT(*) FROM path_vis", connection))
                    {
                        var result = cmd.ExecuteScalar();
                        Console.WriteLine("✓ path_vis table accessible - query successful");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("✗ path_vis table NOT accessible: " + ex.Message);
                }
                
                // Method 3: List all tables
                Console.WriteLine();
                Console.WriteLine("Method 3: Complete table listing...");
                Console.WriteLine("---------------------------------");
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
            Console.WriteLine("ERROR: " + ex.Message);
        }
        
        Console.WriteLine();
        Console.WriteLine("Verification complete.");
        Console.ReadKey();
    }
}