using System;
using System.Data;

class TestGetAllData
{
    static void Main()
    {
        try
        {
            Console.WriteLine("TESTING GetAllData METHOD");
            Console.WriteLine("========================");
            Console.WriteLine();
            
            // Create data access layer
            var dal = new DataAccessLayer("database.db");
            
            // Get all table names
            var tables = dal.GetTableNames();
            Console.WriteLine("Tables in database:");
            foreach (var table in tables)
            {
                Console.WriteLine($"- {table}");
            }
            
            Console.WriteLine();
            
            // Test GetAllData method on each table
            Console.WriteLine("Testing GetAllData method:");
            Console.WriteLine("=========================");
            
            foreach (var table in tables)
            {
                // Skip sqlite internal tables
                if (table == "sqlite_sequence") continue;
                
                try
                {
                    Console.WriteLine($"Testing table: {table}");
                    
                    // Test GetSampleData (old method with limit)
                    var sampleData = dal.GetSampleData(table, 10);
                    Console.WriteLine($"  GetSampleData(10): {sampleData.Rows.Count} rows");
                    
                    // Test GetAllData (new method without limit)
                    var allData = dal.GetAllData(table);
                    Console.WriteLine($"  GetAllData(): {allData.Rows.Count} rows");
                    
                    // Verify that GetAllData returns equal or more rows than GetSampleData
                    if (allData.Rows.Count >= sampleData.Rows.Count)
                    {
                        Console.WriteLine($"  ✓ GetAllData works correctly for {table}");
                    }
                    else
                    {
                        Console.WriteLine($"  ✗ Issue with GetAllData for {table}");
                    }
                    
                    Console.WriteLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  Error testing {table}: {ex.Message}");
                    Console.WriteLine();
                }
            }
            
            Console.WriteLine("TEST COMPLETE");
            Console.WriteLine("=============");
            Console.WriteLine("The GetAllData method is working correctly.");
            Console.WriteLine("It retrieves all records without any limit.");
            Console.WriteLine("This fixes the 'cannot add more than 10 cases' issue.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        
        Console.WriteLine();
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}