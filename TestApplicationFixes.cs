using System;
using System.Data.SQLite;
using System.IO;

class TestApplicationFixes
{
    static void Main()
    {
        Console.WriteLine("TESTING SKY CASA APPLICATION FIXES");
        Console.WriteLine("=================================");
        Console.WriteLine();
        
        try
        {
            // Test 1: Check database connection
            Console.WriteLine("Test 1: Database Connection");
            Console.WriteLine("--------------------------");
            
            string dbPath = "database.db";
            if (!File.Exists(dbPath))
            {
                Console.WriteLine("✗ FAIL: Database file not found!");
                return;
            }
            
            Console.WriteLine("✓ PASS: Database file found");
            
            string connectionString = $"Data Source={dbPath};Version=3;";
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                Console.WriteLine("✓ PASS: Database connection successful");
                connection.Close();
            }
            
            // Test 2: Check for path_vis table
            Console.WriteLine();
            Console.WriteLine("Test 2: path_vis Table Existence");
            Console.WriteLine("-------------------------------");
            
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                
                bool tableExists = false;
                using (var cmd = new SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table' AND name='path_vis'", connection))
                using (var reader = cmd.ExecuteReader())
                {
                    tableExists = reader.HasRows;
                }
                
                if (tableExists)
                {
                    Console.WriteLine("✓ PASS: path_vis table exists");
                }
                else
                {
                    Console.WriteLine("⚠ WARNING: path_vis table not found");
                    Console.WriteLine("  This may cause errors when the application tries to access it");
                }
                
                // Test 3: List all tables
                Console.WriteLine();
                Console.WriteLine("Test 3: Database Structure");
                Console.WriteLine("-------------------------");
                using (var cmd = new SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table' ORDER BY name", connection))
                using (var reader = cmd.ExecuteReader())
                {
                    Console.WriteLine("Tables in database:");
                    while (reader.Read())
                    {
                        string tableName = reader.GetString(0);
                        Console.WriteLine("  - " + tableName);
                    }
                }
                
                connection.Close();
            }
            
            // Test 4: Check for required DLLs
            Console.WriteLine();
            Console.WriteLine("Test 4: Required DLLs");
            Console.WriteLine("--------------------");
            
            string[] requiredDlls = {
                "AForge.dll",
                "AForge.Video.dll", 
                "AForge.Video.DirectShow.dll",
                "FirebirdSql.Data.FirebirdClient.dll",
                "System.Threading.Tasks.Extensions.dll"
            };
            
            bool allDllsFound = true;
            foreach (string dll in requiredDlls)
            {
                if (File.Exists(dll))
                {
                    Console.WriteLine("✓ PASS: " + dll);
                }
                else
                {
                    Console.WriteLine("✗ FAIL: " + dll + " not found");
                    allDllsFound = false;
                }
            }
            
            if (allDllsFound)
            {
                Console.WriteLine("✓ PASS: All required .NET assemblies found");
            }
            else
            {
                Console.WriteLine("✗ FAIL: Some required .NET assemblies missing");
            }
            
            Console.WriteLine();
            Console.WriteLine("TEST SUMMARY:");
            Console.WriteLine("============");
            Console.WriteLine("If the Sky CASA application is running without errors,");
            Console.WriteLine("and all tests above show PASS results, then the fixes");
            Console.WriteLine("have been successfully implemented.");
            
        }
        catch (Exception ex)
        {
            Console.WriteLine("ERROR: " + ex.Message);
        }
        
        Console.WriteLine();
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}