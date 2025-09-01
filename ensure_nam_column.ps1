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
  function Ensure-Column {
    param([string]$Table,[string]$Column,[string]$Type)
    $cmdInfo = $conn.CreateCommand()
    $cmdInfo.CommandText = "PRAGMA table_info($Table);"
    $da = New-Object System.Data.SQLite.SQLiteDataAdapter($cmdInfo)
    $dt = New-Object System.Data.DataTable
    [void]$da.Fill($dt)
    $exists = $false
    foreach($row in $dt.Rows){ if(($row.name) -eq $Column){ $exists = $true; break } }
    if(-not $exists){
      $cmdAdd = $conn.CreateCommand()
      $cmdAdd.CommandText = "ALTER TABLE $Table ADD COLUMN $Column $Type;"
      [void]$cmdAdd.ExecuteNonQuery()
      Write-Host "Added $Table.$Column ($Type)"
    } else {
      Write-Host "$Table.$Column already exists"
    }
  }

  Ensure-Column -Table 'log'   -Column 'nam' -Type 'TEXT'
  Ensure-Column -Table 'users' -Column 'nam' -Type 'TEXT'
  Ensure-Column -Table 'users' -Column 'pas' -Type 'TEXT'

  # Backfill data for existing rows
  $cmd = $conn.CreateCommand()
  $cmd.CommandText = "UPDATE users SET nam = COALESCE(nam, username);"
  [void]$cmd.ExecuteNonQuery()
  $cmd.CommandText = "UPDATE users SET pas = COALESCE(pas, password);"
  [void]$cmd.ExecuteNonQuery()

  # Ensure admin row has nam/pas
  $cmd.CommandText = "INSERT OR IGNORE INTO users (username, password, is_admin, is_active, nam, pas) VALUES ('admin','admin',1,1,'admin','admin');"
  [void]$cmd.ExecuteNonQuery()
}
finally {
  $conn.Close()
}

Write-Host 'Done.'

