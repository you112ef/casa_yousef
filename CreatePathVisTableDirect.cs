using System;
using System.Data.SQLite;
using System.IO;

class CreatePathVisTableDirect
{
    static void Main()
    {
        try
        {
            Console.WriteLine("Creating path_vis table directly...");
            Console.WriteLine("==================================");
            
            // Check if database file exists
            if (!File.Exists("database.db"))
            {
                Console.WriteLine("Error: Database file not found!");
                return;
            }
            
            Console.WriteLine("Database found. Creating path_vis table...");
            
            // Read the SQL script
            string sqlScript = @"
                CREATE TABLE IF NOT EXISTS path_vis (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    path_id INTEGER,
                    x_coordinate REAL,
                    y_coordinate REAL,
                    timestamp DATETIME DEFAULT CURRENT_TIMESTAMP,
                    visibility_status INTEGER DEFAULT 1,
                    color TEXT,
                    line_width REAL DEFAULT 1.0
                );

                CREATE INDEX IF NOT EXISTS idx_path_vis_path_id ON path_vis(path_id);
                CREATE INDEX IF NOT EXISTS idx_path_vis_timestamp ON path_vis(timestamp);";
            
            // Create connection
            string connectionString = "Data Source=database.db;Version=3;";
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                
                // Check if path_vis table already exists
                bool tableExists = false;
                using (var cmd = new SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table' AND name='path_vis'", connection))
                using (var reader = cmd.ExecuteReader())
                {
                    tableExists = reader.HasRows;
                }
                
                if (!tableExists)
                {
                    Console.WriteLine("Creating 'path_vis' table...");
                    
                    // Create the path_vis table
                    using (var cmd = new SQLiteCommand(sqlScript, connection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    
                    Console.WriteLine("Table 'path_vis' created successfully!");
                }
                else
                {
                    Console.WriteLine("Table 'path_vis' already exists.");
                }
                
                // Verify the table was created
                using (var cmd = new SQLiteCommand("PRAGMA table_info(path_vis)", connection))
                using (var reader = cmd.ExecuteReader())
                {
                    Console.WriteLine("\nTable structure:");
                    while (reader.Read())
                    {
                        Console.WriteLine($"  {reader["name"]} ({reader["type"]})");
                    }
                }
                
                connection.Close();
            }
            
            Console.WriteLine("\nProcess completed!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }
        }
        
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}