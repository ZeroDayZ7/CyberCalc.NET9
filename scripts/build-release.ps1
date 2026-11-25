param(
    [string]$Version = "",
    [switch]$SkipInstaller
)

$ErrorActionPreference = "Stop"
$repo = Split-Path -Parent $PSScriptRoot
Set-Location $repo

if ([string]::IsNullOrWhiteSpace($Version)) {
    $Version = (Get-Content -Raw version.txt).Trim()
}

if ($Version -notmatch '^\d+\.\d+\.\d+$') {
    throw "Invalid version: $Version"
}

Remove-Item -Recurse -Force artifacts -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force publish/win-x64 -ErrorAction SilentlyContinue
New-Item -ItemType Directory -Force artifacts, publish/win-x64 | Out-Null

dotnet restore WPF-CALC-NET-9.sln --runtime win-x64
if ($LASTEXITCODE) { throw "dotnet restore failed" }
dotnet build WPF-CALC-NET-9.sln --configuration Release --no-restore
if ($LASTEXITCODE) { throw "dotnet build failed" }
dotnet publish CyberCalc.NET9/WPF-CALC-NET-9.csproj `
    --configuration Release --runtime win-x64 --self-contained true `
    --output publish/win-x64 --no-restore
if ($LASTEXITCODE) { throw "dotnet publish failed" }

Get-ChildItem publish/win-x64 -Recurse -Include '*.pdb','*.user','*.nupkg','*.snupkg' |
    Remove-Item -Force

if (-not $SkipInstaller) {
    $iscc = "C:\Program Files (x86)\Inno Setup 6\ISCC.exe"
    if (-not (Test-Path $iscc)) { $iscc = "C:\Program Files\Inno Setup 6\ISCC.exe" }
    if (-not (Test-Path $iscc)) { throw "Inno Setup compiler not found. Install Inno Setup 6 or use -SkipInstaller." }
    $numericVersion = ($Version -split '-')[0]
    & $iscc "/DAppVersion=$Version" "/DNumericVersion=$numericVersion" "/DSourceDir=$repo\publish\win-x64" "/DOutputDir=$repo\artifacts" installer/CyberCalc.iss
    if ($LASTEXITCODE) { throw "Inno Setup compilation failed" }
}

$zip = "artifacts/CyberCalc-$Version-win-x64.zip"
Compress-Archive -Path publish/win-x64/* -DestinationPath $zip -CompressionLevel Optimal

$files = Get-ChildItem artifacts -File | Where-Object { $_.Extension -ne '.txt' }
$lines = foreach ($file in $files) {
    $hash = (Get-FileHash -Algorithm SHA256 -LiteralPath $file.FullName).Hash.ToLowerInvariant()
    "$hash  $($file.Name)"
}
$lines | Set-Content -Encoding ascii "artifacts/CyberCalc-$Version-SHA256SUMS.txt"

Write-Host "Artifacts created in $repo\artifacts"
