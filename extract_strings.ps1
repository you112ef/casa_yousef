# Extract ASCII and UTF-16LE strings from Sky_CASA.exe and save hints
$ErrorActionPreference = 'Stop'
$exePath = Join-Path (Get-Location) 'Sky_CASA.exe'
if (!(Test-Path $exePath)) { Write-Error "Sky_CASA.exe not found at $exePath" }

$outAll = Join-Path (Get-Location) 'exe_strings.txt'
$outHints = Join-Path (Get-Location) 'exe_conn_hints.txt'

# Read bytes
$bytes = [System.IO.File]::ReadAllBytes($exePath)

# Helper to flush collected string
function Add-CollectedString {
    param(
        [System.Text.StringBuilder]$sb,
        [System.Collections.Generic.List[string]]$bucket,
        [int]$minLen = 4
    )
    if ($sb.Length -ge $minLen) {
        $bucket.Add($sb.ToString()) | Out-Null
    }
    [void]$sb.Clear()
}

$strings = New-Object System.Collections.Generic.List[string]
# ASCII scan
$sb = New-Object System.Text.StringBuilder
for ($i = 0; $i -lt $bytes.Length; $i++) {
    $b = $bytes[$i]
    if ($b -ge 32 -and $b -le 126) {
        [void]$sb.Append([char]$b)
    } else {
        Add-CollectedString -sb $sb -bucket $strings -minLen 4
    }
}
Add-CollectedString -sb $sb -bucket $strings -minLen 4

# UTF-16LE scan
$sb2 = New-Object System.Text.StringBuilder
for ($i = 0; $i -lt $bytes.Length - 1; $i += 2) {
    $lo = $bytes[$i]
    $hi = $bytes[$i+1]
    if ($hi -eq 0 -and $lo -ge 32 -and $lo -le 126) {
        [void]$sb2.Append([char]$lo)
    } else {
        Add-CollectedString -sb $sb2 -bucket $strings -minLen 4
    }
}
Add-CollectedString -sb $sb2 -bucket $strings -minLen 4

# Write all strings
$strings | Sort-Object -Unique | Set-Content -Encoding UTF8 -Path $outAll

# Extract hints
$patterns = @(
    'Data Source','DataSource','Database','AttachDb','Initial Catalog','Server=','Provider=',
    '.db','.sqlite','.db3','.s3db','.fdb','SQLite','Firebird','FbConnection','User Id','User=','Uid=','Pwd=','Password'
)
$regex = [string]::Join('|', ($patterns | ForEach-Object { [Regex]::Escape($_) }))

Select-String -Path $outAll -Pattern $regex -CaseSensitive:$false | ForEach-Object {
    $_.Line
} | Sort-Object -Unique | Set-Content -Encoding UTF8 -Path $outHints

Write-Host "Wrote: $outAll and $outHints"