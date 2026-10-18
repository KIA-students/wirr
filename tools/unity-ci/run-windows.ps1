param(
  [string]$Editor = "D:\Unity\UnityEditors\6000.6.0f1\Editor\Unity.exe",
  [switch]$PreflightOnly
)
$ErrorActionPreference = "Stop"
$repo = (Resolve-Path (Join-Path $PSScriptRoot "../..")).Path
if (-not (Test-Path $Editor -PathType Leaf)) { throw "Unity editor not found: $Editor" }
if (-not (Test-Path (Join-Path $repo "package.json"))) { throw "Missing WiRR package.json" }
if (-not (Get-Command py -ErrorAction SilentlyContinue)) { throw "Python launcher py is required" }
$matchVersion = [regex]::Match($Editor,'6000\.\d+\.\d+f\d+')
if (-not $matchVersion.Success) { throw "Editor version not found in path" }
$version = $matchVersion.Value
Write-Host "Unity Windows: $Editor; version: $version"
if ($PreflightOnly) { Write-Host "Preflight PASS; no tests executed."; exit 0 }
$project = Join-Path "D:\Unity\cli" ("wirr-native-" + [guid]::NewGuid().ToString("N"))
$results = Join-Path $repo "artifacts\unity-native"
New-Item -ItemType Directory -Force -Path $results,$project | Out-Null
try {
  & py -3 (Join-Path $repo "tools/unity-ci/prepare_project.py") --package $repo --project $project --unity $version
  if ($LASTEXITCODE -ne 0) { throw "Unity project preparation failed" }
  $summary = @()
  foreach ($mode in @("EditMode","PlayMode")) {
    $xml = Join-Path $results "$mode.xml"
    $log = Join-Path $results "$mode.log"
    Remove-Item -LiteralPath $xml,$log -ErrorAction SilentlyContinue
    Write-Host "Unity native $mode..."
    $arguments = @("-batchmode","-nographics","-projectPath",$project,"-runTests","-testPlatform",$mode,"-testResults",$xml,"-logFile",$log)
    $process = Start-Process -FilePath $Editor -ArgumentList $arguments -PassThru -Wait -NoNewWindow
    $total = 0
    $failed = -1
    $result = "Missing"
    if (Test-Path $xml) {
      try {
        [xml]$doc = Get-Content -LiteralPath $xml -Raw
        $total = [int]$doc.DocumentElement.GetAttribute("total")
        $failed = [int]$doc.DocumentElement.GetAttribute("failed")
        $result = $doc.DocumentElement.GetAttribute("result")
      } catch { Write-Warning "Invalid NUnit XML for $mode" }
    }
    $passed = ($process.ExitCode -eq 0 -and $total -gt 0 -and $failed -eq 0 -and $result -notin @("Failed","Inconclusive","Missing"))
    $summary += [pscustomobject]@{ Mode=$mode; ExitCode=$process.ExitCode; Total=$total; Failed=$failed; Result=$result; Pass=$passed }
  }
  $summary | Format-Table -AutoSize
  $summary | ConvertTo-Json | Set-Content (Join-Path $results "summary.json") -Encoding UTF8
  if ($summary | Where-Object { -not $_.Pass }) { throw "Unity tests failed. Inspect $results" }
  Write-Host "PASS: Unity EditMode + PlayMode"
} finally {
  Remove-Item -LiteralPath $project -Recurse -Force -ErrorAction SilentlyContinue
}

