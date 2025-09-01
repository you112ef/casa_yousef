using System;
using System.IO;
using System.Data;
using System.Data.SQLite;
using FirebirdSql.Data.FirebirdClient;

class TestDatabaseConnection
{
    static void Main()
    {
        Console.WriteLine("SKY CASA DATABASE CONNECTION TEST");
        Console.WriteLine("=================================");
        Console.WriteLine();
        
        // Test SQLite connection
        TestSQLiteConnection();
        
        Console.WriteLine();
        
        // Test Firebird connection
        TestFirebirdConnection();
        
        Console.WriteLine();
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
    
    static void TestSQLiteConnection()
    {
        Console.WriteLine("Testing SQLite Connection:");
        Console.WriteLine("-------------------------");
        
        try
        {
            if (File.Exists("database.db"))
            {
                string connectionString = "Data Source=database.db;Version=3;";
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    Console.WriteLine("✓ SQLite connection successful");
                    
                    // Get database info
                    using (var cmd = new SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table'", connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine("Tables in database:");
                        while (reader.Read())
                        {
                            Console.WriteLine("  - " + reader.GetString(0));
                        }
                    }
                    
                    connection.Close();
                }
            }
            else
            {
                Console.WriteLine("✗ SQLite database.db file not found");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("✗ SQLite connection failed: " + ex.Message);
        }
    }
    
    static void TestFirebirdConnection()
    {
        Console.WriteLine("Testing Firebird Connection:");
        Console.WriteLine("---------------------------");
        
        try
        {
            // Try to load Firebird assembly
            var assembly = System.Reflection.Assembly.LoadFrom("FirebirdSql.Data.FirebirdClient.dll");
            Console.WriteLine("✓ Firebird client assembly loaded successfully");
            
            // Try to create connection object
            var connectionType = assembly.GetType("FirebirdSql.Data.FirebirdClient.FbConnection");
            if (connectionType != null)
            {
                Console.WriteLine("✓ Firebird connection type available");
                
                // Note: We can't actually connect without a database file
                Console.WriteLine("  Note: Actual connection requires a Firebird database file (.fdb)");
                Console.WriteLine("  To test full connection, create a Firebird database and update");
                Console.WriteLine("  the connection string in the application code.");
            }
            else
            {
                Console.WriteLine("✗ Firebird connection type not found");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("✗ Firebird connection test failed: " + ex.Message);
        }
    }
}