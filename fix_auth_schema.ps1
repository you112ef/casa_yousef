param(
  [string]$DatabasePath = (Join-Path ($PSScriptRoot) 'database.db')
)

$ErrorActionPreference = 'Stop'

$baseDir = if ($PSScriptRoot) { $PSScriptRoot } else { Split-Path -Parent $MyInvocation.MyCommand.Path }
$sqliteDll = Join-Path $baseDir 'System.Data.SQLite.dll'
if(-not (Test-Path $sqliteDll)){
  throw "System.Data.SQLite.dll not found at $sqliteDll"
}
Add-Type -Path $sqliteDll

if(-not (Test-Path $DatabasePath)){
  throw "Database not found: $DatabasePath"
}

$conn = New-Object System.Data.SQLite.SQLiteConnection(("Data Source={0};Version=3;" -f $DatabasePath))
$conn.Open()
try {
  $cmd = $conn.CreateCommand()
  $cmd.CommandText = "PRAGMA foreign_keys=ON;"
  [void]$cmd.ExecuteNonQuery()

  function Ensure-Column($table, $column, $type){
    param([string]$table,[string]$column,[string]$type)
    $cmd2 = $conn.CreateCommand()
    $cmd2.CommandText = "PRAGMA table_info($table);"
    $da = New-Object System.Data.SQLite.SQLiteDataAdapter($cmd2)
    $dt = New-Object System.Data.DataTable
    [void]$da.Fill($dt)
    $exists = $false
    foreach($row in $dt.Rows){ if(($row.name) -eq $column){ $exists = $true; break } }
    if(-not $exists){
      $cmd3 = $conn.CreateCommand()
      $cmd3.CommandText = "ALTER TABLE $table ADD COLUMN $column $type;"
      [void]$cmd3.ExecuteNonQuery()
      Write-Host "Added $table.$column ($type)"
    } else {
      Write-Host "$table.$column already exists"
    }
  }

  Ensure-Column -table 'users' -column 'nam' -type 'TEXT'
  Ensure-Column -table 'log'   -column 'nam' -type 'TEXT'

  # Backfill admin username to 'nam' if null/empty
  $cmd.CommandText = "UPDATE users SET nam = COALESCE(nam, username) WHERE (nam IS NULL OR TRIM(nam)='') AND username IS NOT NULL;"
  [void]$cmd.ExecuteNonQuery()

  Write-Host 'Schema fix complete.'
}
finally {
  $conn.Close()
}

