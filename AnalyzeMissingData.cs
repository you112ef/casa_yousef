using System;
using System.Data;
using System.Collections.Generic;

class AnalyzeMissingData
{
    static void Main()
    {
        try
        {
            Console.WriteLine("ANALYZING MISSING DATA IN SKY CASA DATABASE");
            Console.WriteLine("==========================================");
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
            
            // Define expected tables for a complete medical lab system
            var expectedTables = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "patients",
                "cbc_results",
                "urine_results",
                "stool_results",
                "sem_results",
                "vis_results",
                "path_results",
                "doctors",
                "admin",
                "log",
                "path_vis"  // This is the critical table that was missing
            };
            
            Console.WriteLine("Checking for missing tables:");
            var missingTables = new List<string>();
            foreach (var expectedTable in expectedTables)
            {
                if (tables.Contains(expectedTable))
                {
                    Console.WriteLine($"  ✓ {expectedTable} - EXISTS");
                }
                else
                {
                    Console.WriteLine($"  ✗ {expectedTable} - MISSING");
                    missingTables.Add(expectedTable);
                }
            }
            
            Console.WriteLine();
            
            // Check for the critical path_vis table
            if (tables.Contains("path_vis"))
            {
                Console.WriteLine("✓ path_vis table exists - Visualization features should work");
                
                // Check structure of path_vis table
                try
                {
                    var schema = dal.GetTableSchema("path_vis");
                    Console.WriteLine("  path_vis table structure:");
                    foreach (DataRow row in schema.Rows)
                    {
                        var cid = row["cid"].ToString();
                        var name = row["name"].ToString();
                        var type = row["type"].ToString();
                        var notnull = row["notnull"].ToString();
                        var dflt_value = row["dflt_value"].ToString();
                        var pk = row["pk"].ToString();
                        Console.WriteLine($"    {name} ({type}) PK:{pk} NOTNULL:{notnull}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  Error checking path_vis structure: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("✗ path_vis table is missing - Visualization features will not work");
                Console.WriteLine("  You need to run AddPathVisTable.bat or CreatePathVisTable.bat");
            }
            
            Console.WriteLine();
            
            // Check record counts in key tables
            Console.WriteLine("Checking record counts in key tables:");
            var keyTables = new[] { "patients", "cbc_results", "urine_results", "stool_results", "sem_results" };
            foreach (var table in keyTables)
            {
                if (tables.Contains(table))
                {
                    try
                    {
                        var count = dal.ExecuteScalar($"SELECT COUNT(*) FROM {table}");
                        Console.WriteLine($"  {table}: {count} records");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"  {table}: Error counting records - {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"  {table}: Table missing");
                }
            }
            
            Console.WriteLine();
            
            // Check for common reference data
            if (tables.Contains("doctors"))
            {
                try
                {
                    var doctorCount = dal.ExecuteScalar("SELECT COUNT(*) FROM doctors");
                    Console.WriteLine($"Doctors table: {doctorCount} records");
                    if (Convert.ToInt32(doctorCount) == 0)
                    {
                        Console.WriteLine("  ⚠ Doctors table is empty - Consider adding doctor records");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error checking doctors table: {ex.Message}");
                }
            }
            
            if (tables.Contains("admin"))
            {
                try
                {
                    var adminCount = dal.ExecuteScalar("SELECT COUNT(*) FROM admin");
                    Console.WriteLine($"Admin table: {adminCount} records");
                    if (Convert.ToInt32(adminCount) == 0)
                    {
                        Console.WriteLine("  ⚠ Admin table is empty - Consider adding admin user");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error checking admin table: {ex.Message}");
                }
            }
            
            Console.WriteLine();
            Console.WriteLine("ANALYSIS COMPLETE");
            Console.WriteLine("=================");
            
            if (missingTables.Count > 0)
            {
                Console.WriteLine("Missing tables that should be added:");
                foreach (var missingTable in missingTables)
                {
                    Console.WriteLine($"- {missingTable}");
                }
                Console.WriteLine();
                Console.WriteLine("Recommended actions:");
                Console.WriteLine("1. Run the appropriate table creation scripts for missing tables");
                Console.WriteLine("2. Add reference data to doctors and admin tables if empty");
            }
            else
            {
                Console.WriteLine("All expected tables are present in the database");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error analyzing database: {ex.Message}");
            Console.WriteLine($"Exception type: {ex.GetType().Name}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }
        }
        
        Console.WriteLine();
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}