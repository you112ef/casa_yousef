@echo off
echo Adding path_vis table to database...
echo ====================================

echo Checking if database exists...
if not exist "database.db" (
    echo Error: Database file not found!
    pause
    exit /b 1
)

echo Database found. Creating path_vis table...

REM We'll use a simple approach by creating a temporary C# file that adds the table
echo Creating temporary C# program to add table...

(
echo using System;
echo using System.Data.SQLite;
echo using System.IO;
echo.
echo class Program
echo {
echo     static void Main^(^)
echo     {
echo         try
echo         {
echo             string dbPath = "database.db";
echo             string connectionString = "Data Source=" + dbPath + ";Version=3;";
echo.
echo             using ^(var connection = new SQLiteConnection^(connectionString^)^)
echo             {
echo                 connection.Open^(^);
echo.
echo                 // Check if path_vis table already exists
echo                 bool tableExists = false;
echo                 using ^(var cmd = new SQLiteCommand^("SELECT name FROM sqlite_master WHERE type='table' AND name='path_vis'", connection^)^)
echo                 using ^(var reader = cmd.ExecuteReader^(^)^)
echo                 {
echo                     tableExists = reader.HasRows;
echo                 }
echo.
echo                 if ^(!tableExists^)
echo                 {
echo                     Console.WriteLine^("Creating 'path_vis' table..."^);
echo.
echo                     // Create the path_vis table
echo                     string createTableSQL = "CREATE TABLE path_vis (id INTEGER PRIMARY KEY AUTOINCREMENT, path_id INTEGER, x_coordinate REAL, y_coordinate REAL, timestamp DATETIME DEFAULT CURRENT_TIMESTAMP, visibility_status INTEGER DEFAULT 1, color TEXT, line_width REAL DEFAULT 1.0, FOREIGN KEY (path_id) REFERENCES path(id));";
echo.
echo                     using ^(var cmd = new SQLiteCommand^(createTableSQL, connection^)^)
echo                     {
echo                         cmd.ExecuteNonQuery^(^);
echo                     }
echo.
echo                     // Create indexes
echo                     string createIndex1SQL = "CREATE INDEX idx_path_vis_path_id ON path_vis(path_id);";
echo                     using ^(var cmd = new SQLiteCommand^(createIndex1SQL, connection^)^)
echo                     {
echo                         cmd.ExecuteNonQuery^(^);
echo                     }
echo.
echo                     string createIndex2SQL = "CREATE INDEX idx_path_vis_timestamp ON path_vis(timestamp);";
echo                     using ^(var cmd = new SQLiteCommand^(createIndex2SQL, connection^)^)
echo                     {
echo                         cmd.ExecuteNonQuery^(^);
echo                     }
echo.
echo                     Console.WriteLine^("Table 'path_vis' created successfully!"^);
echo                 }
echo                 else
echo                 {
echo                     Console.WriteLine^("Table 'path_vis' already exists."^);
echo                 }
echo.
echo                 connection.Close^(^);
echo             }
echo         }
echo         catch ^(Exception ex^)
echo         {
echo             Console.WriteLine^("Error: " + ex.Message^);
echo         }
echo     }
echo }
) > TempAddTable.cs

echo Compiling temporary program...
csc TempAddTable.cs 2>nul
if errorlevel 1 (
    echo Error: Failed to compile. Trying with reference...
    csc TempAddTable.cs -r:System.Data.SQLite.dll 2>nul
    if errorlevel 1 (
        echo Error: Failed to compile even with reference.
        del TempAddTable.cs >nul 2>nul
        pause
        exit /b 1
    )
)

echo Running program to add table...
TempAddTable.exe

echo Cleaning up...
del TempAddTable.cs >nul 2>nul
del TempAddTable.exe >nul 2>nul

echo.
echo Process completed!
pause