-- SQL script to create the missing path_vis table
-- This table likely combines path data with visualization information

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

-- Create an index for better performance
CREATE INDEX idx_path_vis_path_id ON path_vis(path_id);
CREATE INDEX idx_path_vis_timestamp ON path_vis(timestamp);

-- Sample data insertion (for testing purposes)
-- INSERT INTO path_vis (path_id, x_coordinate, y_coordinate, color, line_width) 
-- VALUES (1, 100.5, 200.3, 'red', 2.0);

-- Verification query
-- SELECT * FROM path_vis LIMIT 5;