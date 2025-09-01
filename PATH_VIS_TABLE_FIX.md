# Fix for "no such table: path_vis" SQL Error

## Problem Description
The Sky CASA application is throwing an error: "no such table: path_vis". This occurs because the application is trying to access a database table named "path_vis" that doesn't exist in the database.

## Database Analysis
From our examination of the database.db file, we found the following tables:
- sem
- ret
- stool
- urine
- cbc
- vis
- dr
- sqlite_sequence
- path
- log

Note that there is a [path](file:///D:/New%20folder%20(4)/Sky%20CASA/path) table but no "path_vis" table.

## Solution
We need to create the missing "path_vis" table in the database. This table likely combines path data with visualization information.

## Method 1: Using SQLite Command Line (Recommended)
If you have SQLite command-line tools installed:

1. Create a SQL file named `add_path_vis_table.sql` with the following content:
```sql
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
CREATE INDEX idx_path_vis_timestamp ON path_vis(timestamp);
```

2. Execute the SQL file:
```bash
sqlite3 database.db ".read add_path_vis_table.sql"
```

## Method 2: Using a SQLite Browser Tool
1. Download and install a SQLite browser tool such as:
   - DB Browser for SQLite (https://sqlitebrowser.org/)
   - SQLite Studio (https://sqlitestudio.pl/)

2. Open the database.db file with the tool

3. Execute the following SQL commands:
```sql
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
CREATE INDEX idx_path_vis_timestamp ON path_vis(timestamp);
```

## Method 3: Using the Provided Batch Files
We've created batch files to help with this process:

1. Run `CreatePathVisTable.bat` - This attempts to create the table using sqlite3 if available
2. Run `FixPathVisError.bat` - This is a comprehensive script that checks for the table and creates it if needed

## Table Structure Explanation
The path_vis table contains the following columns:
- `id`: Primary key with auto-increment
- `path_id`: Foreign key referencing the path table
- `x_coordinate`: X coordinate for visualization (REAL type)
- `y_coordinate`: Y coordinate for visualization (REAL type)
- `timestamp`: When the record was created (defaults to current timestamp)
- `visibility_status`: Status indicator (INTEGER)
- `color`: Color information for visualization (TEXT)
- `line_width`: Width of lines for visualization (REAL)

## Verification
After creating the table, you can verify it exists by:
1. Running `ExamineTables.bat` and checking the output
2. Using a SQLite browser to view the database structure
3. Running a simple query: `SELECT * FROM path_vis LIMIT 5;`

## Testing
Once the table is created, test the application to ensure the "no such table: path_vis" error is resolved.

## Additional Notes
- The table includes foreign key constraints to maintain data integrity
- Indexes are created on frequently queried columns for better performance
- The table structure can be modified based on the specific needs of the application

## Troubleshooting
If you continue to experience issues:
1. Ensure the database file (database.db) is not read-only
2. Check that you have write permissions to the database file
3. Verify that the path table exists (it's referenced by the foreign key)
4. Make sure no other processes are locking the database file

## Contact Support
If you're unable to resolve this issue, please contact technical support with:
- The exact error message
- The steps you've tried
- Any relevant log files