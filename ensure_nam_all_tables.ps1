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
  $cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%' ORDER BY name;"
  $da = New-Object System.Data.SQLite.SQLiteDataAdapter($cmd)
  $dt = New-Object System.Data.DataTable
  [void]$da.Fill($dt)

  foreach($row in $dt.Rows){
    $table = [string]$row.name
    $cmdInfo = $conn.CreateCommand()
    $cmdInfo.CommandText = "PRAGMA table_info($table);"
    $da2 = New-Object System.Data.SQLite.SQLiteDataAdapter($cmdInfo)
    $tinfo = New-Object System.Data.DataTable
    [void]$da2.Fill($tinfo)
    $hasNam = $false
    foreach($r in $tinfo.Rows){ if(($r.name) -eq 'nam'){ $hasNam = $true; break } }
    if(-not $hasNam){
      $cmdAdd = $conn.CreateCommand()
      $cmdAdd.CommandText = "ALTER TABLE $table ADD COLUMN nam TEXT;"
      [void]$cmdAdd.ExecuteNonQuery()
      Write-Host "Added $table.nam"
    }
    if($table -eq 'users'){
      $hasPas = $false
      foreach($r in $tinfo.Rows){ if(($r.name) -eq 'pas'){ $hasPas = $true; break } }
      if(-not $hasPas){
        $cmdAdd2 = $conn.CreateCommand()
        $cmdAdd2.CommandText = "ALTER TABLE users ADD COLUMN pas TEXT;"
        [void]$cmdAdd2.ExecuteNonQuery()
        Write-Host "Added users.pas"
      }
      $cmdFill = $conn.CreateCommand()
      $cmdFill.CommandText = "UPDATE users SET nam = COALESCE(nam, username); UPDATE users SET pas = COALESCE(pas, password);"
      [void]$cmdFill.ExecuteNonQuery()
    }
  }
  Write-Host 'All tables checked.'
}
finally {
  $conn.Close()
}

