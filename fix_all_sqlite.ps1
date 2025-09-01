param(
  [string[]]$Roots = @($PSScriptRoot,$env:USERPROFILE,$env:APPDATA,$env:LOCALAPPDATA,$env:PROGRAMDATA),
  [string]$ReportPath = (Join-Path ($PSScriptRoot) 'fix_all_sqlite_report.txt')
)

$ErrorActionPreference = 'Stop'

$baseDir = if ($PSScriptRoot) { $PSScriptRoot } else { Split-Path -Parent $MyInvocation.MyCommand.Path }
$sqliteDll = Join-Path $baseDir 'System.Data.SQLite.dll'
if(-not (Test-Path $sqliteDll)){
  throw "System.Data.SQLite.dll not found at $sqliteDll"
}
Add-Type -Path $sqliteDll

if(Test-Path $ReportPath){ Remove-Item $ReportPath -Force }
function Rep([string]$m){ Add-Content -Path $ReportPath -Value $m }

function Is-SqliteFile([string]$path){
  try{
    if(-not (Test-Path $path)){ return $false }
    $fs = [System.IO.File]::Open($path,[System.IO.FileMode]::Open,[System.IO.FileAccess]::Read,[System.IO.FileShare]::ReadWrite)
    try{
      $buf = New-Object byte[] 16
      if($fs.Read($buf,0,16) -lt 16){ return $false }
      return ([System.Text.Encoding]::ASCII.GetString($buf)) -like 'SQLite format 3*'
    } finally { $fs.Dispose() }
  } catch { return $false }
}

Rep ("Run: " + (Get-Date -Format 'yyyyMMdd_HHmmss'))

$exts = @('*.db','*.sqlite','*.db3','*.s3db')
$targets = New-Object System.Collections.Generic.List[string]
foreach($root in $Roots){
  if([string]::IsNullOrWhiteSpace($root) -or -not (Test-Path $root)){ continue }
  foreach($ext in $exts){
    Get-ChildItem -Path $root -Recurse -File -ErrorAction SilentlyContinue -Filter $ext | ForEach-Object { $targets.Add($_.FullName) | Out-Null }
  }
}
$targets = $targets | Sort-Object -Unique

foreach($db in $targets){
  try{
    if(-not (Is-SqliteFile $db)){ Rep ("SKIP: $db"); continue }
    $conn = New-Object System.Data.SQLite.SQLiteConnection("Data Source=$db;Version=3;")
    $conn.Open()
    try{
      $cmd = $conn.CreateCommand()
      $cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%' ORDER BY name;"
      $da = New-Object System.Data.SQLite.SQLiteDataAdapter($cmd)
      $dt = New-Object System.Data.DataTable
      [void]$da.Fill($dt)
      foreach($row in $dt.Rows){
        $table = [string]$row.name
        $info = New-Object System.Data.DataTable
        $cmd2 = $conn.CreateCommand(); $cmd2.CommandText = "PRAGMA table_info($table);"
        (New-Object System.Data.SQLite.SQLiteDataAdapter($cmd2)).Fill($info) | Out-Null
        $cols = @($info | ForEach-Object { $_.name })
        if($cols -notcontains 'nam'){
          $cmdAdd = $conn.CreateCommand(); $cmdAdd.CommandText = "ALTER TABLE $table ADD COLUMN nam TEXT;"; $cmdAdd.ExecuteNonQuery() | Out-Null
        }
        if($table -eq 'users' -and ($cols -notcontains 'pas')){
          $cmdAdd2 = $conn.CreateCommand(); $cmdAdd2.CommandText = "ALTER TABLE users ADD COLUMN pas TEXT;"; $cmdAdd2.ExecuteNonQuery() | Out-Null
        }
      }
      $cmd = $conn.CreateCommand(); $cmd.CommandText = "UPDATE users SET nam = COALESCE(nam, username); UPDATE users SET pas = COALESCE(pas, password);"; $cmd.ExecuteNonQuery() | Out-Null
      Rep ("OK: $db")
    } finally { $conn.Close() }
  } catch { Rep ("ERR: $db => " + $_.Exception.Message) }
}
Rep 'Done.'
Write-Host ("Wrote: {0}" -f $ReportPath)

