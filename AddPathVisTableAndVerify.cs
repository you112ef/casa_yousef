using System;
using System.Data.SQLite;
using System.IO;

class Program
{
    static void Main()
    {
        try
        {
            Console.WriteLine("Adding path_vis table to Sky CASA database...");
            Console.WriteLine("===========================================");
            
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
                
                // Check if path_vis table already exists
                bool tableExists = false;
                using (var cmd = new SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table' AND name='path_vis'", connection))
                using (var reader = cmd.ExecuteReader())
                {
                    tableExists = reader.HasRows;
                }
                
                if (tableExists)
                {
                    Console.WriteLine("ℹ Table 'path_vis' already exists in the database.");
                    Console.WriteLine("No action needed.");
                }
                else
                {
                    Console.WriteLine("Creating 'path_vis' table...");
                    
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
                    
                    Console.WriteLine("✓ Table 'path_vis' created successfully!");
                    Console.WriteLine();
                }
                
                // Verify the table was created
                Console.WriteLine("Verifying database structure...");
                using (var cmd = new SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table' AND name='path_vis'", connection))
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        Console.WriteLine("✓ Verification successful: 'path_vis' table exists in database");
                    }
                    else
                    {
                        Console.WriteLine("✗ Verification failed: 'path_vis' table not found");
                    }
                }
                
                // List all tables to show the updated structure
                Console.WriteLine();
                Console.WriteLine("Current database structure:");
                Console.WriteLine("==========================");
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
            Console.WriteLine("Process completed successfully!");
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