-- SQL Script to Check Record Counts in All Tables
-- This script can be run with any SQLite database browser tool

.headers on
.mode column

-- Show all tables in the database
.tables

-- Count records in each table
SELECT 'sem' as table_name, COUNT(*) as record_count FROM sem
UNION ALL
SELECT 'ret' as table_name, COUNT(*) as record_count FROM ret
UNION ALL
SELECT 'stool' as table_name, COUNT(*) as record_count FROM stool
UNION ALL
SELECT 'urine' as table_name, COUNT(*) as record_count FROM urine
UNION ALL
SELECT 'cbc' as table_name, COUNT(*) as record_count FROM cbc
UNION ALL
SELECT 'vis' as table_name, COUNT(*) as record_count FROM vis
UNION ALL
SELECT 'dr' as table_name, COUNT(*) as record_count FROM dr
UNION ALL
SELECT 'path' as table_name, COUNT(*) as record_count FROM path
UNION ALL
SELECT 'log' as table_name, COUNT(*) as record_count FROM log
UNION ALL
SELECT 'admin' as table_name, COUNT(*) as record_count FROM admin
ORDER BY table_name;

-- Show sample data from each table (first 3 records)
SELECT 'Sample data from sem table:' as info;
SELECT * FROM sem LIMIT 3;

SELECT 'Sample data from cbc table:' as info;
SELECT * FROM cbc LIMIT 3;

-- Check if path_vis table exists (it was missing in previous error)
SELECT 'Checking for path_vis table:' as info;
SELECT name FROM sqlite_master WHERE type='table' AND name='path_vis';