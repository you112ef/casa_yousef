using System;
using System.Data.SQLite;
using System.IO;

class Program
{
    static void Main()
    {
        try
        {
            Console.WriteLine("Verifying path_vis table in Sky CASA database...");
            Console.WriteLine("==============================================");
            
            string dbPath = "database.db";
            string connectionString = $"Data Source={dbPath};Version=3;";
            
            // Check if database exists
            if (!File.Exists(dbPath))
            {
                Console.WriteLine($"Error: Database file not found at {dbPath}");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                return;
            }
            
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                Console.WriteLine("✓ Database connection successful!");
                Console.WriteLine();
                
                // Method 1: Check if path_vis table exists using sqlite_master
                bool tableExists1 = false;
                using (var cmd = new SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table' AND name='path_vis'", connection))
                using (var reader = cmd.ExecuteReader())
                {
                    tableExists1 = reader.HasRows;
                }
                
                Console.WriteLine("Method 1 - sqlite_master check:");
                if (tableExists1)
                {
                    Console.WriteLine("✓ path_vis table found in sqlite_master");
                }
                else
                {
                    Console.WriteLine("✗ path_vis table NOT found in sqlite_master");
                }
                
                // Method 2: Try to query the table directly
                bool tableExists2 = false;
                try
                {
                    using (var cmd = new SQLiteCommand("SELECT COUNT(*) FROM path_vis", connection))
                    {
                        cmd.ExecuteNonQuery();
                        tableExists2 = true;
                    }
                    Console.WriteLine("Method 2 - Direct query test:");
                    Console.WriteLine("✓ path_vis table accessible via direct query");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Method 2 - Direct query test:");
                    Console.WriteLine("✗ path_vis table NOT accessible: " + ex.Message);
                }
                
                // Method 3: List all tables
                Console.WriteLine();
                Console.WriteLine("All tables in database:");
                Console.WriteLine("======================");
                using (var cmd = new SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table' ORDER BY name", connection))
                using (var reader = cmd.ExecuteReader())
                {
                    bool pathVisFound = false;
                    while (reader.Read())
                    {
                        string tableName = reader.GetString(0);
                        Console.WriteLine("- " + tableName);
                        if (tableName == "path_vis")
                        {
                            pathVisFound = true;
                        }
                    }
                    
                    if (pathVisFound)
                    {
                        Console.WriteLine();
                        Console.WriteLine("✓ path_vis table confirmed in database structure");
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine("✗ path_vis table NOT found in database structure");
                    }
                }
                
                connection.Close();
            }
            
            Console.WriteLine();
            Console.WriteLine("Verification completed!");
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