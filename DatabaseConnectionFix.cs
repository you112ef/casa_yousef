using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using FirebirdSql.Data.FirebirdClient;

/// <summary>
/// Database Connection Fix for SQL URI errors
/// This class provides improved database connection handling with better error reporting
/// </summary>
public static class DatabaseConnectionFix
{
    /// <summary>
    /// Validates and fixes SQLite database connection
    /// </summary>
    /// <param name="dbPath">Path to the SQLite database file</param>
    /// <returns>Valid connection string or throws exception with detailed error</returns>
    public static string FixSQLiteConnection(string dbPath)
    {
        try
        {
            // Validate database file exists
            if (string.IsNullOrEmpty(dbPath))
            {
                throw new ArgumentException("Database path cannot be null or empty");
            }
            
            // Handle relative paths
            string fullPath = Path.IsPathRooted(dbPath) ? dbPath : Path.Combine(Environment.CurrentDirectory, dbPath);
            
            // Check if file exists
            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"Database file not found at: {fullPath}", fullPath);
            }
            
            // Validate it's actually a SQLite database
            if (!IsValidSQLiteFile(fullPath))
            {
                throw new InvalidOperationException($"File exists but is not a valid SQLite database: {fullPath}");
            }
            
            // Create proper connection string
            string connectionString = $"Data Source={fullPath};Version=3;Pooling=true;Max Pool Size=100;";
            
            // Test connection
            TestConnection(connectionString, "SQLite");
            
            return connectionString;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to create SQLite connection string for '{dbPath}': {ex.Message}", ex);
        }
    }
    
    /// <summary>
    /// Validates and fixes Firebird database connection
    /// </summary>
    /// <param name="connectionString">Firebird connection string</param>
    /// <returns>Valid connection string or throws exception with detailed error</returns>
    public static string FixFirebirdConnection(string connectionString)
    {
        try
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException("Firebird connection string cannot be null or empty");
            }
            
            // Parse connection string to validate components
            var builder = new FbConnectionStringBuilder(connectionString);
            
            // Ensure required components are present
            if (string.IsNullOrEmpty(builder.Database))
            {
                throw new ArgumentException("Firebird connection string must specify Database");
            }
            
            if (string.IsNullOrEmpty(builder.UserID))
            {
                builder.UserID = "SYSDBA"; // Default user
            }
            
            if (string.IsNullOrEmpty(builder.Password))
            {
                builder.Password = "masterkey"; // Default password
            }
            
            // Test connection
            TestConnection(builder.ConnectionString, "Firebird");
            
            return builder.ConnectionString;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to create Firebird connection string: {ex.Message}", ex);
        }
    }
    
    /// <summary>
    /// Tests a database connection
    /// </summary>
    /// <param name="connectionString">Connection string to test</param>
    /// <param name="databaseType">Type of database (SQLite, Firebird, etc.)</param>
    private static void TestConnection(string connectionString, string databaseType)
    {
        try
        {
            if (databaseType.Equals("SQLite", StringComparison.OrdinalIgnoreCase))
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (var cmd = new SQLiteCommand("SELECT 1", connection))
                    {
                        cmd.ExecuteScalar();
                    }
                }
            }
            else if (databaseType.Equals("Firebird", StringComparison.OrdinalIgnoreCase))
            {
                using (var connection = new FbConnection(connectionString))
                {
                    connection.Open();
                    using (var cmd = new FbCommand("SELECT 1 FROM RDB$DATABASE", connection))
                    {
                        cmd.ExecuteScalar();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to connect to {databaseType} database: {ex.Message}", ex);
        }
    }
    
    /// <summary>
    /// Validates if a file is a valid SQLite database
    /// </summary>
    /// <param name="filePath">Path to the file to check</param>
    /// <returns>True if valid SQLite file, false otherwise</returns>
    private static bool IsValidSQLiteFile(string filePath)
    {
        try
        {
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (var binaryReader = new BinaryReader(fileStream))
            {
                // SQLite files start with "SQLite format 3"
                byte[] header = binaryReader.ReadBytes(16);
                string headerString = System.Text.Encoding.UTF8.GetString(header);
                return headerString.StartsWith("SQLite format 3");
            }
        }
        catch
        {
            return false;
        }
    }
    
    /// <summary>
    /// Gets the appropriate database connection based on configuration
    /// </summary>
    /// <param name="configPath">Path to configuration file or database file</param>
    /// <returns>Connection string for the appropriate database</returns>
    public static string GetDatabaseConnection(string configPath = "database.db")
    {
        try
        {
            // First try SQLite (most common case)
            if (File.Exists(configPath) || File.Exists(Path.Combine(Environment.CurrentDirectory, configPath)))
            {
                return FixSQLiteConnection(configPath);
            }
            
            // Check for Firebird database file
            string firebirdPath = Path.ChangeExtension(configPath, ".fdb");
            if (File.Exists(firebirdPath))
            {
                string fbConnectionString = $"User=SYSDBA;Password=masterkey;Database={firebirdPath};DataSource=localhost;Port=3050;Dialect=3;";
                return FixFirebirdConnection(fbConnectionString);
            }
            
            // If no database file found, create SQLite connection with default settings
            return FixSQLiteConnection(configPath);
        }
        catch (Exception ex)
        {
            throw new Exception($"Could not determine database connection: {ex.Message}", ex);
        }
    }
    
    /// <summary>
    /// Creates a diagnostic report for database connection issues
    /// </summary>
    /// <returns>Diagnostic report as string</returns>
    public static string CreateDiagnosticReport()
    {
        var report = new System.Text.StringBuilder();
        report.AppendLine("=== DATABASE CONNECTION DIAGNOSTIC REPORT ===");
        report.AppendLine($"Timestamp: {DateTime.Now}");
        report.AppendLine($"Current Directory: {Environment.CurrentDirectory}");
        report.AppendLine($"Application Directory: {AppDomain.CurrentDomain.BaseDirectory}");
        report.AppendLine();
        
        // Check for database files
        report.AppendLine("DATABASE FILES CHECK:");
        string[] dbFiles = { "database.db", "sky_casa.fdb" };
        foreach (string dbFile in dbFiles)
        {
            string fullPath = Path.Combine(Environment.CurrentDirectory, dbFile);
            if (File.Exists(fullPath))
            {
                report.AppendLine($"  ✓ {dbFile} - EXISTS ({new FileInfo(fullPath).Length} bytes)");
            }
            else
            {
                report.AppendLine($"  ✗ {dbFile} - NOT FOUND");
            }
        }
        report.AppendLine();
        
        // Check for required assemblies
        report.AppendLine("REQUIRED ASSEMBLIES CHECK:");
        string[] assemblies = { "System.Data.SQLite.dll", "FirebirdSql.Data.FirebirdClient.dll" };
        foreach (string assembly in assemblies)
        {
            try
            {
                Assembly.LoadFrom(assembly);
                report.AppendLine($"  ✓ {assembly} - LOADED");
            }
            catch
            {
                report.AppendLine($"  ✗ {assembly} - NOT FOUND or FAILED TO LOAD");
            }
        }
        report.AppendLine();
        
        // Test database connections
        report.AppendLine("CONNECTION TESTS:");
        try
        {
            string sqliteConn = FixSQLiteConnection("database.db");
            report.AppendLine($"  ✓ SQLite Connection - VALID");
        }
        catch (Exception ex)
        {
            report.AppendLine($"  ✗ SQLite Connection - FAILED: {ex.Message}");
        }
        
        try
        {
            string fbConn = $"User=SYSDBA;Password=masterkey;Database=sky_casa.fdb;DataSource=localhost;Port=3050;Dialect=3;";
            FixFirebirdConnection(fbConn);
            report.AppendLine($"  ✓ Firebird Connection - VALID");
        }
        catch (Exception ex)
        {
            report.AppendLine($"  ✗ Firebird Connection - FAILED: {ex.Message}");
        }
        
        return report.ToString();
    }
}