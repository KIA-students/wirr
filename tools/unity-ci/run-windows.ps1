param(
  [string]$Editor = "",
  [switch]$PreflightOnly,
  [switch]$KeepProject,
  [switch]$NoGraphics,
  [int]$TimeoutMinutes = 45,
  [string]$PackageRoot = ""
)
$ErrorActionPreference = "Stop"
$repo = (Resolve-Path (Join-Path $PSScriptRoot "../..")).Path
if (-not $Editor) {
  $Editor = @("D:\Unity\UnityEditors\6000.6.5f1\Editor\Unity.exe",
    "$env:ProgramFiles\Unity\Hub\Editor\6000.6.5f1\Editor\Unity.exe") |
    Where-Object { Test-Path $_ -PathType Leaf } | Select-Object -First 1
}
if (-not $Editor) { throw "Unity 6000.6.5f1 not found; supply -Editor with the existing Unity.exe path" }
if (-not (Test-Path $Editor -PathType Leaf)) { throw "Unity editor not found: $Editor" }
if (-not $PackageRoot) { $PackageRoot = $repo }
$PackageRoot = (Resolve-Path $PackageRoot).Path
if (-not (Test-Path (Join-Path $PackageRoot "package.json"))) { throw "Missing WiRR package.json: $PackageRoot" }
if (-not (Get-Command py -ErrorAction SilentlyContinue)) { throw "Python launcher py is required" }
if ($TimeoutMinutes -lt 1) { throw "TimeoutMinutes must be positive" }
$matchVersion = [regex]::Match($Editor,'6000\.\d+\.\d+f\d+')
if (-not $matchVersion.Success) { throw "Editor version not found in path" }
$version = $matchVersion.Value
if ($version -ne "6000.6.5f1") { throw "Use the course editor Unity 6000.6.5f1" }
Write-Host "Unity Windows: $Editor; version: $version; package: $PackageRoot"
if ($PreflightOnly) { Write-Host "Preflight PASS; no tests executed."; exit 0 }
$project = Join-Path "D:\Unity\cli" ("wirr-native-" + [guid]::NewGuid().ToString("N"))
$results = Join-Path $repo "artifacts\unity-native"
New-Item -ItemType Directory -Force -Path $results,$project | Out-Null
# Never leave a stale PASS/FAIL summary from a previous package or revision.
Remove-Item -LiteralPath (Join-Path $results "summary.json") -ErrorAction SilentlyContinue
# Clear results from both suites up front; PlayMode may be skipped after EditMode failure.
foreach ($suiteName in @("EditMode", "PlayMode")) {
  foreach ($extension in @("xml", "log")) {
    Remove-Item -LiteralPath (Join-Path $results "$suiteName.$extension") -ErrorAction SilentlyContinue
  }
}
try {
  & py -3 (Join-Path $repo "tools/unity-ci/prepare_project.py") --package $PackageRoot --project $project --unity $version --stage-package
  if ($LASTEXITCODE -ne 0) { throw "Unity project preparation failed" }
  $summary = @()
  $commit = (& git -C $PackageRoot rev-parse HEAD 2>$null | Out-String).Trim()
  $passedAll = $false
  $dirty = [bool]((& git -C $PackageRoot status --porcelain | Out-String).Trim())
  Write-Host "Project for diagnostics: $project"
  foreach ($mode in @("EditMode","PlayMode")) {
    $xml = Join-Path $results "$mode.xml"
    $log = Join-Path $results "$mode.log"
    Remove-Item -LiteralPath $xml,$log -ErrorAction SilentlyContinue
    Write-Host "Unity native $mode..."
    # Quote path-valued arguments explicitly: Start-Process joins ArgumentList into a command line.
    $quotedArgs = @("-batchmode","-projectPath",('"' + $project + '"'),
      "-runTests","-testPlatform",$mode,"-testResults",('"' + $xml + '"'),
      "-logFile",('"' + $log + '"'))
    if ($NoGraphics) { $quotedArgs += "-nographics" }
    # Process.Start with UseShellExecute=false provides a stable ExitCode after WaitForExit.
    $psi = New-Object System.Diagnostics.ProcessStartInfo
    $psi.FileName = $Editor
    $psi.Arguments = ($quotedArgs -join " ")
    $psi.UseShellExecute = $false
    $psi.CreateNoWindow = $true
    $process = New-Object System.Diagnostics.Process
    $process.StartInfo = $psi
    if (-not $process.Start()) { throw "Unity did not start for $mode" }
    try {
      $finished = $process.WaitForExit($TimeoutMinutes * 60000)
      if (-not $finished) {
        Write-Warning "Unity $mode timed out after $TimeoutMinutes minutes (PID $($process.Id))"
        try { $process.Kill(); $process.WaitForExit() } catch { Write-Warning $_ }
        $exitCode = -1
      } else {
        $process.WaitForExit()
        $exitCode = [int]$process.ExitCode
      }
    } finally {
      $process.Dispose()
    }
    Write-Host "Unity $mode process exit code: $exitCode"
    if ($exitCode -eq -1073741819) {
      Write-Warning "Unity terminated with 0xC0000005 (access violation). This is a native process crash, not an NUnit failure."
      Write-Host "Inspect Windows Application Event Log for Unity.exe faulting module and Windows Error Reporting details."
    }
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
    if (-not (Test-Path $xml)) {
      Write-Warning "Missing NUnit results for $mode. Unity log: $log"
      if (Test-Path $log) { Get-Content -LiteralPath $log -Tail 60 }
    }
    & py -3 (Join-Path $repo "tools/unity-ci/validate_nunit.py") $xml --suite $mode
    $xmlPassed = ($LASTEXITCODE -eq 0)
    $passed = ($exitCode -eq 0 -and $xmlPassed)
    $summary += [pscustomobject]@{ Mode=$mode; ExitCode=$exitCode; Total=$total; Failed=$failed; Result=$result; Pass=$passed }
    if (-not $passed) { Write-Warning "$mode failed; skipping remaining suites to preserve primary error"; break }
  }
  $summary | Format-Table -AutoSize
  [pscustomobject]@{ Unity=$version; Commit=$commit; WorkingTreeDirty=$dirty; Project=$project; Suites=$summary } | ConvertTo-Json -Depth 5 | Set-Content (Join-Path $results "summary.json") -Encoding UTF8
  if ($summary.Count -ne 2 -or ($summary | Where-Object { -not $_.Pass })) { throw "Unity tests failed. Inspect $results" }
  $passedAll = $true
  Write-Host "PASS: Unity EditMode + PlayMode"
} finally {
  if ($passedAll -and -not $KeepProject) {
    Remove-Item -LiteralPath $project -Recurse -Force -ErrorAction SilentlyContinue
  } else {
    Write-Host "Project retained for diagnostics: $project"
  }
}
