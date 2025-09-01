using System;
using System.Data;
using System.Windows.Forms;

class TestDatabaseConnection
{
    [STAThread]
    static void Main()
    {
        try
        {
            Console.WriteLine("Testing database connection for Sky CASA application...");
            Console.WriteLine("=====================================================");
            
            // Initialize the database helper (same as in Program.cs)
            DatabaseHelper.Initialize("database.db");
            
            Console.WriteLine("✓ Database helper initialized successfully");
            
            // Test getting table names
            var tables = DatabaseHelper.GetTableNames();
            Console.WriteLine("✓ Retrieved " + tables.Count + " tables from database:");
            
            foreach (var table in tables)
            {
                Console.WriteLine("  - " + table);
            }
            
            // Test a simple query
            if (tables.Count > 0)
            {
                string firstTable = tables[0];
                Console.WriteLine("\nTesting query on table: " + firstTable);
                
                try
                {
                    var data = DatabaseHelper.ExecuteQuery("SELECT COUNT(*) as count FROM " + firstTable);
                    if (data.Rows.Count > 0)
                    {
                        int count = Convert.ToInt32(data.Rows[0]["count"]);
                        Console.WriteLine("✓ Table '" + firstTable + "' contains " + count + " records");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("⚠ Warning: Could not query table '" + firstTable + "': " + ex.Message);
                }
            }
            
            // Test admin user access
            Console.WriteLine("\nTesting admin user access:");
            try
            {
                var adminData = DatabaseHelper.ExecuteQuery("SELECT * FROM admin WHERE username = 'admin'");
                if (adminData.Rows.Count > 0)
                {
                    Console.WriteLine("✓ Admin user found:");
                    foreach (DataColumn col in adminData.Columns)
                    {
                        Console.WriteLine("  " + col.ColumnName + ": " + adminData.Rows[0][col]);
                    }
                }
                else
                {
                    Console.WriteLine("⚠ No admin user found in database");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("⚠ Warning: Could not access admin table: " + ex.Message);
            }
            
            Console.WriteLine("\n=== DATABASE CONNECTION TEST COMPLETE ===");
            Console.WriteLine("The application is properly linked with the database.");
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ ERROR: " + ex.Message);
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}