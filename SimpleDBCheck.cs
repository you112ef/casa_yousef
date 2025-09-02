using System;
using System.Data.SQLite;
using System.IO;

class Program
{
    static void Main()
    {
        try
        {
            Console.WriteLine("Checking Sky CASA database structure...");
            Console.WriteLine();
            
            string connectionString = "Data Source=database.db;Version=3;";
            
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                Console.WriteLine("Database connection successful!");
                Console.WriteLine();
                
                // Check specifically for path_vis table
                Console.WriteLine("Checking for path_vis table...");
                bool pathVisExists = false;
                using (var cmd = new SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table' AND name='path_vis'", connection))
                using (var reader = cmd.ExecuteReader())
                {
                    pathVisExists = reader.HasRows;
                }
                
                if (pathVisExists)
                {
                    Console.WriteLine("✓ path_vis table found!");
                }
                else
                {
                    Console.WriteLine("✗ path_vis table NOT found!");
                    Console.WriteLine("  This explains the 'no such table: path_vis' error.");
                    Console.WriteLine();
                    Console.WriteLine("Creating path_vis table...");
                    
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
                }
                
                Console.WriteLine();
                
                // Get all table names
                using (var cmd = new SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table'", connection))
                using (var reader = cmd.ExecuteReader())
                {
                    Console.WriteLine("Tables in the database:");
                    Console.WriteLine("=====================");
                    while (reader.Read())
                    {
                        string tableName = reader.GetString(0);
                        Console.WriteLine("- " + tableName);
                    }
                }
                
                Console.WriteLine();
                Console.WriteLine("Database examination complete!");
                connection.Close();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error examining database: " + ex.Message);
            Console.WriteLine("Exception type: " + ex.GetType().FullName);
        }
        
        Console.WriteLine();
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}