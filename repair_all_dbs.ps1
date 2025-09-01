# Repairs missing auth/log tables across SQLite DBs used by the app
# - Scans common locations
# - Verifies SQLite signature
# - Backs up each DB before change
# - Creates tables users/log if missing and seeds admin/admin

param(
  [string[]]$Roots,
  [string[]]$Extensions = @('*.db','*.sqlite','*.db3','*.s3db')
)

$ErrorActionPreference = 'Stop'

if(-not $Roots -or $Roots.Count -eq 0){
  $Roots = @((Get-Location).Path, $env:APPDATA, $env:LOCALAPPDATA, $env:PROGRAMDATA, $env:USERPROFILE)
}

# Load SQLite provider from local folder
$sqliteDll = Join-Path (Get-Location) 'System.Data.SQLite.dll'
if(-not (Test-Path $sqliteDll)){
  throw "System.Data.SQLite.dll not found at $sqliteDll"
}
Add-Type -Path $sqliteDll

$reportPath = Join-Path (Get-Location) 'repair_report.txt'
if(Test-Path $reportPath){ Remove-Item $reportPath -Force }

function Write-Rep([string]$msg){ Add-Content -Path $reportPath -Value $msg }

function Is-SqliteFile([string]$path){
  try{
    if(-not (Test-Path $path)){ return $false }
    $fs = [System.IO.File]::Open($path,[System.IO.FileMode]::Open,[System.IO.FileAccess]::Read,[System.IO.FileShare]::ReadWrite)
    try{
      $buf = New-Object byte[] 16
      $read = $fs.Read($buf,0,16)
      if($read -lt 16){ return $false }
      $sig = [System.Text.Encoding]::ASCII.GetString($buf)
      return $sig -like 'SQLite format 3*'
    } finally { $fs.Dispose() }
  } catch { return $false }
}

$stamp = Get-Date -Format 'yyyyMMdd_HHmmss'
Write-Rep ("Run at: $stamp")

$targets = New-Object System.Collections.Generic.List[string]
foreach($root in $Roots){
  if([string]::IsNullOrWhiteSpace($root) -or -not (Test-Path $root)){ continue }
  foreach($ext in $Extensions){
    Get-ChildItem -Path $root -Recurse -ErrorAction SilentlyContinue -Include $ext | ForEach-Object {
      $targets.Add($_.FullName) | Out-Null
    }
  }
}

$targets = $targets | Sort-Object -Unique

foreach($dbPath in $targets){
  try{
    if(-not (Is-SqliteFile $dbPath)){
      Write-Rep ("SKIP (not SQLite): $dbPath")
      continue
    }

    # Backup
    $backup = "{0}.{1}.bak" -f $dbPath,$stamp
    try { Copy-Item -LiteralPath $dbPath -Destination $backup -Force } catch {}

    # Connect and repair
    $conn = New-Object System.Data.SQLite.SQLiteConnection(("Data Source={0};Version=3;" -f $dbPath))
    $conn.Open()
    try{
      $cmd = $conn.CreateCommand()
      $cmd.CommandText = "PRAGMA foreign_keys=ON;"
      [void]$cmd.ExecuteNonQuery()

      # Create users
      $cmd.CommandText = @'
CREATE TABLE IF NOT EXISTS users (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  username TEXT NOT NULL UNIQUE,
  password TEXT,
  password_hash TEXT,
  is_admin INTEGER NOT NULL DEFAULT 0,
  is_active INTEGER NOT NULL DEFAULT 1
);
'@
      [void]$cmd.ExecuteNonQuery()

      # Create log
      $cmd.CommandText = @'
CREATE TABLE IF NOT EXISTS log (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  user_id INTEGER,
  username TEXT,
  action TEXT,
  created_at TEXT NOT NULL DEFAULT (datetime('now','localtime')),
  FOREIGN KEY(user_id) REFERENCES users(id)
);
'@
      [void]$cmd.ExecuteNonQuery()

      # Seed admin
      $cmd.CommandText = "INSERT OR IGNORE INTO users (username, password, is_admin, is_active) VALUES ('admin','admin',1,1);"
      [void]$cmd.ExecuteNonQuery()

      Write-Rep ("OK: $dbPath")
    } finally { $conn.Close() }
  }
  catch{
    Write-Rep ("ERR: $dbPath => " + $_.Exception.Message)
  }
}

Write-Rep 'Done.'
Write-Host ("Wrote report: {0}" -f $reportPath)