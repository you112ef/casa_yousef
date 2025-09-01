-- SQL script to create the missing path_vis table
-- Execute this script using a SQLite browser or command-line tool

CREATE TABLE IF NOT EXISTS path_vis (
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

CREATE INDEX IF NOT EXISTS idx_path_vis_path_id ON path_vis(path_id);
CREATE INDEX IF NOT EXISTS idx_path_vis_timestamp ON path_vis(timestamp);

-- Verify the table was created
SELECT 'SUCCESS: path_vis table created successfully!' AS result;

-- Show the table structure
PRAGMA table_info(path_vis);