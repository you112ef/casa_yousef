using System;
using System.Data.SQLite;
using System.IO;

class Program
{
    static void Main()
    {
        Console.WriteLine("Final Verification: Checking for path_vis table");
        Console.WriteLine("=============================================");
        Console.WriteLine();
        
        try
        {
            string dbPath = "database.db";
            string connectionString = $"Data Source={dbPath};Version=3;";
            
            // Check if database exists
            if (!File.Exists(dbPath))
            {
                Console.WriteLine("ERROR: Database file not found!");
                Console.ReadKey();
                return;
            }
            
            Console.WriteLine("✓ Database file found");
            
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                Console.WriteLine("✓ Database connection successful");
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
                    Console.WriteLine("✓ SUCCESS: path_vis table already exists!");
                    Console.WriteLine("The 'no such table: path_vis' error should be resolved.");
                }
                else
                {
                    Console.WriteLine("⚠ path_vis table NOT found - will create it now");
                    Console.WriteLine();
                    
                    // Create the path_vis table
                    try
                    {
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
                        
                        Console.WriteLine("✓ SUCCESS: path_vis table created successfully!");
                        
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
                    catch (Exception createEx)
                    {
                        Console.WriteLine("ERROR creating table: " + createEx.Message);
                        Console.WriteLine("Please use a SQLite browser tool to manually create the table.");
                    }
                }
                
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
        }
        catch (Exception ex)
        {
            Console.WriteLine("ERROR: " + ex.Message);
            Console.WriteLine("Exception type: " + ex.GetType().FullName);
        }
        
        Console.WriteLine();
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}