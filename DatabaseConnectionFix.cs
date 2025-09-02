using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using System.Text;
using FirebirdSql.Data.FirebirdClient;

/// <summary>
/// Database Connection Fix and bootstrap utilities
/// </summary>
public static class DatabaseConnectionFix
{
    /// <summary>
    /// Ensures an SQLite database exists and has the minimal schema required by the app.
    /// Returns a verified connection string.
    /// </summary>
    public static string FixSQLiteConnection(string dbPath)
    {
        try
        {
            if (string.IsNullOrEmpty(dbPath))
                throw new ArgumentException("Database path cannot be null or empty");

            string fullPath = Path.IsPathRooted(dbPath) ? dbPath : Path.Combine(Environment.CurrentDirectory, dbPath);

            if (!File.Exists(fullPath))
            {
                CreateEmptySQLiteDatabase(fullPath);
                InitializeDefaultSchema(fullPath);
            }

            if (!IsValidSQLiteFile(fullPath))
                throw new InvalidOperationException($"File exists but is not a valid SQLite database: {fullPath}");

            string connectionString = $"Data Source={fullPath};Version=3;Pooling=true;Max Pool Size=100;";
            TestConnection(connectionString, "SQLite");
            return connectionString;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to prepare SQLite connection for '{dbPath}': {ex.Message}", ex);
        }
    }

    public static string FixFirebirdConnection(string connectionString)
    {
        try
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException("Firebird connection string cannot be null or empty");

            var builder = new FbConnectionStringBuilder(connectionString)
            {
                UserID = string.IsNullOrEmpty(new FbConnectionStringBuilder(connectionString).UserID) ? "SYSDBA" : new FbConnectionStringBuilder(connectionString).UserID,
                Password = string.IsNullOrEmpty(new FbConnectionStringBuilder(connectionString).Password) ? "masterkey" : new FbConnectionStringBuilder(connectionString).Password
            };

            if (string.IsNullOrEmpty(builder.Database))
                throw new ArgumentException("Firebird connection string must specify Database");

            TestConnection(builder.ConnectionString, "Firebird");
            return builder.ConnectionString;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to create Firebird connection string: {ex.Message}", ex);
        }
    }

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

    private static bool IsValidSQLiteFile(string filePath)
    {
        try
        {
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (var binaryReader = new BinaryReader(fileStream))
            {
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
    /// Returns a valid connection string. Creates SQLite DB if missing; otherwise tries Firebird fallback.
    /// </summary>
    public static string GetDatabaseConnection(string configPath = "database.db")
    {
        try
        {
            // Prefer SQLite and create if missing
            return FixSQLiteConnection(configPath);
        }
        catch
        {
            // Fallback: Firebird file next to the db name
            string firebirdPath = Path.ChangeExtension(configPath, ".fdb");
            if (File.Exists(firebirdPath))
            {
                string fbConnectionString = $"User=SYSDBA;Password=masterkey;Database={firebirdPath};DataSource=localhost;Port=3050;Dialect=3;";
                return FixFirebirdConnection(fbConnectionString);
            }
            // Re-throw original error for visibility
            throw;
        }
    }

    public static string CreateDiagnosticReport()
    {
        var report = new StringBuilder();
        report.AppendLine("=== DATABASE CONNECTION DIAGNOSTIC REPORT ===");
        report.AppendLine($"Timestamp: {DateTime.Now}");
        report.AppendLine($"Current Directory: {Environment.CurrentDirectory}");
        report.AppendLine($"Application Directory: {AppDomain.CurrentDomain.BaseDirectory}");
        report.AppendLine();

        report.AppendLine("DATABASE FILES CHECK:");
        string[] dbFiles = { "database.db", "sky_casa.fdb" };
        foreach (string dbFile in dbFiles)
        {
            string fullPath = Path.Combine(Environment.CurrentDirectory, dbFile);
            if (File.Exists(fullPath))
                report.AppendLine($"  ✓ {dbFile} - EXISTS ({new FileInfo(fullPath).Length} bytes)");
            else
                report.AppendLine($"  ✗ {dbFile} - NOT FOUND");
        }
        report.AppendLine();

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

        report.AppendLine("CONNECTION TESTS:");
        try { FixSQLiteConnection("database.db"); report.AppendLine("  ✓ SQLite Connection - VALID"); }
        catch (Exception ex) { report.AppendLine($"  ✗ SQLite Connection - FAILED: {ex.Message}"); }

        try
        {
            string fbConn = $"User=SYSDBA;Password=masterkey;Database=sky_casa.fdb;DataSource=localhost;Port=3050;Dialect=3;";
            FixFirebirdConnection(fbConn);
            report.AppendLine("  ✓ Firebird Connection - VALID");
        }
        catch (Exception ex) { report.AppendLine($"  ✗ Firebird Connection - FAILED: {ex.Message}"); }

        return report.ToString();
    }

    private static void CreateEmptySQLiteDatabase(string fullPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath) ?? ".");
        SQLiteConnection.CreateFile(fullPath);
    }

    private static void InitializeDefaultSchema(string fullPath)
    {
        string connStr = $"Data Source={fullPath};Version=3;";
        using (var conn = new SQLiteConnection(connStr))
        {
            conn.Open();
            using (var tx = conn.BeginTransaction())
            using (var cmd = conn.CreateCommand())
            {
                // Core tables
                cmd.CommandText = @"
CREATE TABLE IF NOT EXISTS admin (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  username TEXT NOT NULL UNIQUE,
  password TEXT NOT NULL,
  role TEXT DEFAULT 'admin',
  created_date DATETIME DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS patients (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  name TEXT NOT NULL,
  birth_date DATETIME,
  gender TEXT,
  phone_number TEXT,
  address TEXT,
  created_date DATETIME DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS cbc (
  test_result_id INTEGER PRIMARY KEY AUTOINCREMENT,
  patient_id INTEGER NOT NULL,
  test_date DATETIME DEFAULT CURRENT_TIMESTAMP,
  wbc REAL, rbc REAL, hemoglobin REAL, hematocrit REAL,
  FOREIGN KEY(patient_id) REFERENCES patients(id)
);

CREATE TABLE IF NOT EXISTS urine (
  test_result_id INTEGER PRIMARY KEY AUTOINCREMENT,
  patient_id INTEGER NOT NULL,
  test_date DATETIME DEFAULT CURRENT_TIMESTAMP,
  color TEXT, ph REAL,
  FOREIGN KEY(patient_id) REFERENCES patients(id)
);

CREATE TABLE IF NOT EXISTS stool (
  test_result_id INTEGER PRIMARY KEY AUTOINCREMENT,
  patient_id INTEGER NOT NULL,
  test_date DATETIME DEFAULT CURRENT_TIMESTAMP,
  color TEXT, ph REAL,
  FOREIGN KEY(patient_id) REFERENCES patients(id)
);

CREATE TABLE IF NOT EXISTS kidney_function (
  test_result_id INTEGER PRIMARY KEY AUTOINCREMENT,
  patient_id INTEGER NOT NULL,
  test_date DATETIME DEFAULT CURRENT_TIMESTAMP,
  creatinine REAL, bun REAL,
  FOREIGN KEY(patient_id) REFERENCES patients(id)
);

CREATE TABLE IF NOT EXISTS liver_function (
  test_result_id INTEGER PRIMARY KEY AUTOINCREMENT,
  patient_id INTEGER NOT NULL,
  test_date DATETIME DEFAULT CURRENT_TIMESTAMP,
  alt REAL, ast REAL,
  FOREIGN KEY(patient_id) REFERENCES patients(id)
);

CREATE TABLE IF NOT EXISTS semen_analysis (
  test_result_id INTEGER PRIMARY KEY AUTOINCREMENT,
  patient_id INTEGER NOT NULL,
  test_date DATETIME DEFAULT CURRENT_TIMESTAMP,
  volume_ml REAL, ph REAL,
  ai_analysis_performed INTEGER DEFAULT 0,
  ai_confidence_score REAL,
  rapid_progressive_percent REAL,
  slow_progressive_percent REAL,
  non_progressive_percent REAL,
  immotile_percent REAL,
  analyzed_image_path TEXT,
  heatmap_image_path TEXT,
  FOREIGN KEY(patient_id) REFERENCES patients(id)
);
";
                cmd.ExecuteNonQuery();

                // Seed default admin and demo patient if empty
                cmd.CommandText = "SELECT COUNT(*) FROM admin";
                var adminCount = Convert.ToInt32(cmd.ExecuteScalar());
                if (adminCount == 0)
                {
                    cmd.CommandText = "INSERT INTO admin (username, password, role) VALUES ('admin','admin','admin')";
                    cmd.ExecuteNonQuery();
                }

                cmd.CommandText = "SELECT COUNT(*) FROM patients";
                var pCount = Convert.ToInt32(cmd.ExecuteScalar());
                if (pCount == 0)
                {
                    cmd.CommandText = "INSERT INTO patients (name, birth_date, gender, phone_number, address) VALUES ('Demo Patient','1990-01-01','ذكر','01000000000','')";
                    cmd.ExecuteNonQuery();
                }

                tx.Commit();
            }
        }
    }
}
