using System;
using System.Data.SQLite;
using System.IO;

class Program
{
    static void Main()
    {
        try
        {
            Console.WriteLine("AUTO FIX: Creating path_vis table in database...");
            Console.WriteLine("==============================================");
            
            string dbPath = "database.db";
            string connectionString = $"Data Source={dbPath};Version=3;";
            
            // Check if database exists
            if (!File.Exists(dbPath))
            {
                Console.WriteLine("ERROR: Database file not found!");
                return;
            }
            
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                Console.WriteLine("✓ Connected to database successfully");
                
                // Check if path_vis table already exists
                bool tableExists = false;
                using (var cmd = new SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table' AND name='path_vis'", connection))
                using (var reader = cmd.ExecuteReader())
                {
                    tableExists = reader.HasRows;
                }
                
                if (tableExists)
                {
                    Console.WriteLine("✓ path_vis table already exists - no action needed");
                }
                else
                {
                    Console.WriteLine("⚠ path_vis table not found - creating it now...");
                    
                    // Create the path_vis table
                    string createTableSQL = @"
                        CREATE TABLE path_vis (
                            id INTEGER PRIMARY KEY AUTOINCREMENT,
                            path_id INTEGER,
                            x_coordinate REAL,
                            y_coordinate REAL,
                            timestamp DATETIME DEFAULT CURRENT_TIMESTAMP,
                            visibility_status INTEGER DEFAULT 1,
                            color TEXT,
                            line_width REAL DEFAULT 1.0,
                            FOREIGN KEY (path_id) REFERENCES path(id)
                        );
                        
                        CREATE INDEX idx_path_vis_path_id ON path_vis(path_id);
                        CREATE INDEX idx_path_vis_timestamp ON path_vis(timestamp);";
                    
                    using (var cmd = new SQLiteCommand(createTableSQL, connection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    
                    Console.WriteLine("✓ path_vis table created successfully!");
                    
                    // Verify creation
                    using (var cmd = new SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table' AND name='path_vis'", connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            Console.WriteLine("✓ VERIFICATION: path_vis table confirmed in database!");
                        }
                        else
                        {
                            Console.WriteLine("✗ VERIFICATION FAILED: path_vis table not found after creation");
                        }
                    }
                }
                
                // List all tables
                Console.WriteLine();
                Console.WriteLine("Current database structure:");
                Console.WriteLine("=========================");
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
            
            Console.WriteLine();
            Console.WriteLine("AUTO FIX COMPLETE!");
            Console.WriteLine("The 'no such table: path_vis' error should now be resolved.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("ERROR: " + ex.Message);
            Console.WriteLine("You may need to manually create the table using a SQLite browser tool.");
            Console.WriteLine("Please refer to FINAL_PATH_VIS_SOLUTION.txt for manual instructions.");
        }
        
        Console.WriteLine();
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}