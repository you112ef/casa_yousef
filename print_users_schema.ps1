param(
  [string]$DatabasePath = (Join-Path ($PSScriptRoot) 'database.db')
)

$ErrorActionPreference = 'Stop'

$baseDir = if ($PSScriptRoot) { $PSScriptRoot } else { Split-Path -Parent $MyInvocation.MyCommand.Path }
$sqliteDll = Join-Path $baseDir 'System.Data.SQLite.dll'
Add-Type -Path $sqliteDll

$cn = "Data Source=$DatabasePath;Version=3;"
$conn = New-Object System.Data.SQLite.SQLiteConnection($cn)
$conn.Open()
try {
  foreach($t in @('users','log')){
    $cmd = $conn.CreateCommand()
    $cmd.CommandText = "PRAGMA table_info($t);"
    $da = New-Object System.Data.SQLite.SQLiteDataAdapter($cmd)
    $dt = New-Object System.Data.DataTable
    [void]$da.Fill($dt)
    Write-Host "=== $t ==="
    ($dt | Format-Table -AutoSize | Out-String -Width 4096)
  }
}
finally { $conn.Close() }

