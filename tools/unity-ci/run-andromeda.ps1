param(
  [string]$LicenseFile = $env:UNITY_LICENSE_FILE,
  [string]$UnityVersion = "6000.3.24f1",
  [string]$Image = "",
  [switch]$PreflightOnly
)
$ErrorActionPreference = "Stop"
$repo = (Resolve-Path (Join-Path $PSScriptRoot "../..")).Path
Set-Location $repo
if (-not (Test-Path (Join-Path $repo "package.json"))) { throw "WiRR package.json missing" }
if (-not (Get-Command docker -ErrorAction SilentlyContinue)) { throw "Docker not in PATH" }
$platform = (& docker info --format '{{.OSType}}/{{.Architecture}}' 2>&1 | Out-String).Trim()
if ($LASTEXITCODE -ne 0 -or $platform -notmatch '^linux/(x86_64|amd64)$') {
  throw "Docker Desktop must run Linux amd64 containers. Current: $platform"
}
Write-Host "Docker Desktop: $platform"
if ($PreflightOnly) {
  Write-Host "Preflight PASS: Unity not executed and no image downloaded."
  exit 0
}
if (-not $Image) { $Image = "unityci/editor:ubuntu-$UnityVersion-base-3.2.2" }
if (-not $LicenseFile) {
  $LicenseFile = @("$env:APPDATA\Unity\Unity_lic.ulf", "$env:PROGRAMDATA\Unity\Unity_lic.ulf") |
    Where-Object { Test-Path $_ -PathType Leaf } | Select-Object -First 1
}
if (-not $LicenseFile -or -not (Test-Path $LicenseFile -PathType Leaf)) {
  throw "Missing Unity .ulf license: use -LicenseFile 'C:\path\Unity_lic.ulf'. Do not commit license files."
}
$LicenseFile = (Resolve-Path $LicenseFile).Path
if (-not (Get-Command py -ErrorAction SilentlyContinue)) { throw "Python launcher py is required" }
$project = Join-Path $env:TEMP ("wirr-unity-ci-" + [Guid]::NewGuid().ToString("N"))
$results = Join-Path $repo "artifacts\unity-ci"
New-Item -ItemType Directory -Force -Path $project,$results | Out-Null
try {
  & py -3 (Join-Path $repo "tools/unity-ci/prepare_project.py") --package $repo --project $project --unity $UnityVersion
  if ($LASTEXITCODE -ne 0) { throw "Could not generate Unity test project" }
  $failed = $false
  foreach ($mode in @("EditMode", "PlayMode")) {
    $xml = Join-Path $results "$mode.xml"
    $log = Join-Path $results "$mode.log"
    Remove-Item -Force -ErrorAction SilentlyContinue $xml,$log
    Write-Host "Running Unity $mode in Docker Desktop..."
    $dockerArgs = @(
      "run", "--rm", "--init", "--platform", "linux/amd64",
      "--mount", "type=bind,source=$repo,target=/workspace/package,readonly",
      "--mount", "type=bind,source=$project,target=/workspace/project",
      "--mount", "type=bind,source=$results,target=/workspace/results",
      "--mount", "type=bind,source=$LicenseFile,target=/root/.local/share/unity3d/Unity/Unity_lic.ulf,readonly",
      "--entrypoint", "/opt/unity/Editor/Unity",
      $Image, "-batchmode", "-nographics", "-projectPath", "/workspace/project",
      "-runTests", "-testPlatform", $mode,
      "-testResults", "/workspace/results/$mode.xml",
      "-logFile", "/workspace/results/$mode.log"
    )
    & docker @dockerArgs
    if ($LASTEXITCODE -ne 0 -or -not (Test-Path $xml)) {
      Write-Warning "${mode}: Unity failed or NUnit XML missing"
      $failed = $true
      continue
    }
    & py -3 -c 'import sys,xml.etree.ElementTree as e; r=e.parse(sys.argv[1]).getroot(); total=int(r.get("total",r.get("totalCount","0"))); failed=int(r.get("failed",r.get("failures","0"))); print("NUnit:",total,"tests,",failed,"failed"); sys.exit(0 if total>0 and failed==0 and r.get("result","Passed") not in ("Failed","Inconclusive") else 1)' $xml
    if ($LASTEXITCODE -ne 0) { $failed = $true }
  }
  if ($failed) { throw "Unity tests failed; see $results" }
  Write-Host "Unity tests passed. Reports: $results"
} finally {
  Remove-Item -LiteralPath $project -Recurse -Force -ErrorAction SilentlyContinue
}
