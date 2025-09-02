# Initializes SQLite database schema and seeds a default admin user
param(
  [string]$DatabasePath = (Join-Path (Get-Location) 'database.db')
)

$ErrorActionPreference = 'Stop'

# Ensure SQLite provider is available
Add-Type -Path (Join-Path (Get-Location) 'System.Data.SQLite.dll')

# Backup existing DB if present
if (Test-Path $DatabasePath) {
  $fi = Get-Item -LiteralPath $DatabasePath -ErrorAction SilentlyContinue
  $stamp = Get-Date -Format 'yyyyMMdd_HHmmss'
  $backup = Join-Path (Get-Location) ("database_preinit_{0}.db" -f $stamp)
  Copy-Item -LiteralPath $DatabasePath -Destination $backup -Force
  Write-Host ("Backup created: {0}" -f $backup)
}

# Open connection (creates file if it does not exist)
$conn = New-Object System.Data.SQLite.SQLiteConnection(("Data Source={0};Version=3;" -f $DatabasePath))
$conn.Open()
try {
  $cmd = $conn.CreateCommand()

  # Pragmas for sane defaults
  $cmd.CommandText = "PRAGMA foreign_keys=ON; PRAGMA journal_mode=WAL; PRAGMA synchronous=NORMAL;"
  [void]$cmd.ExecuteNonQuery()

  # Create users table (includes extra columns expected by app)
  $cmd.CommandText = @'
CREATE TABLE IF NOT EXISTS users (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  username TEXT NOT NULL UNIQUE,
  password TEXT,
  password_hash TEXT,
  pas TEXT,
  nam TEXT,
  is_admin INTEGER NOT NULL DEFAULT 0,
  is_active INTEGER NOT NULL DEFAULT 1
);
CREATE INDEX IF NOT EXISTS idx_users_username ON users(username);
'@
  [void]$cmd.ExecuteNonQuery()

  # Create log table (include nam for compatibility)
  $cmd.CommandText = @'
CREATE TABLE IF NOT EXISTS log (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  user_id INTEGER,
  username TEXT,
  nam TEXT,
  action TEXT,
  created_at TEXT NOT NULL DEFAULT (datetime('now','localtime')),
  FOREIGN KEY(user_id) REFERENCES users(id)
);
CREATE INDEX IF NOT EXISTS idx_log_created_at ON log(created_at);
'@
  [void]$cmd.ExecuteNonQuery()

  # Seed default admin account if missing
  $cmd.CommandText = "INSERT OR IGNORE INTO users (username, password, pas, nam, is_admin, is_active) VALUES ('admin','admin','admin','admin',1,1);"
  [void]$cmd.ExecuteNonQuery()

  Write-Host 'Schema initialized and admin user ensured.'
}
finally {
  $conn.Close()
}

# Quick verify: list tables
$conn2 = New-Object System.Data.SQLite.SQLiteConnection(("Data Source={0};Version=3;" -f $DatabasePath))
$conn2.Open()
try {
  $cmd2 = $conn2.CreateCommand()
  $cmd2.CommandText = "SELECT type, name FROM sqlite_master WHERE type IN ('table','view') ORDER BY name;"
  $da = New-Object System.Data.SQLite.SQLiteDataAdapter($cmd2)
  $dt = New-Object System.Data.DataTable
  [void]$da.Fill($dt)
  $out = Join-Path (Get-Location) 'post_init_schema.txt'
  ($dt | Format-Table -AutoSize | Out-String -Width 4096) | Set-Content -Encoding UTF8 -Path $out
  Write-Host ("Wrote: {0}" -f $out)
}
finally {
  $conn2.Close()
}