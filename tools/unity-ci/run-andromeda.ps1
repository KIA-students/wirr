# WiRR PR #88, launcher for Windows 11 + WSL2 + Docker Desktop.
# Usage: powershell -ExecutionPolicy Bypass -File .\tools\unity-ci\run-andromeda.ps1
param(
    [string]$LicenseFile = $env:UNITY_LICENSE_FILE,
    [switch]$PreflightOnly
)
$ErrorActionPreference = "Stop"
$repo = (Resolve-Path (Join-Path $PSScriptRoot "..\..")).Path
if (-not (Test-Path (Join-Path $repo "package.json"))) { throw "Run this script from the WiRR repository." }
if (-not (Get-Command wsl.exe -ErrorAction SilentlyContinue)) {
    throw "WSL2 is not available. Install/configure WSL2 and Docker Desktop Linux integration first."
}
$distros = @(wsl.exe --list --quiet 2>&1 | Out-String)
if ($LASTEXITCODE -ne 0) { throw "WSL is not operational." }
$checks = @(wsl.exe -- bash -lc 'uname -m; command -v docker; docker info --format "{{.OSType}}/{{.Architecture}}"' 2>&1)
if ($LASTEXITCODE -ne 0 -or (($checks | Out-String) -notmatch 'linux/')) {
    Write-Host ($checks | Out-String)
    throw "Docker daemon is not available inside default WSL distribution. Enable Docker Desktop > Resources > WSL Integration."
}
if (($checks | Out-String) -notmatch 'x86_64|amd64') {
    throw "This Unity Docker image needs an x86_64 Linux host."
}
Write-Host "WSL + Docker: OK"
if ($PreflightOnly) { Write-Host "Preflight only. No image downloaded or tests started."; exit 0 }
if ([string]::IsNullOrWhiteSpace($LicenseFile)) {
    $candidates = @(
        "$env:APPDATA\Unity\Unity_lic.ulf",
        "$env:PROGRAMDATA\Unity\Unity_lic.ulf"
    )
    $LicenseFile = $candidates | Where-Object { Test-Path $_ -PathType Leaf } | Select-Object -First 1
}
if (-not $LicenseFile -or -not (Test-Path $LicenseFile -PathType Leaf)) {
    throw "No .ulf license file found. Run with -LicenseFile 'C:\path\to\Unity_lic.ulf'. Never commit the license."
}
$LicenseFile = (Resolve-Path $LicenseFile).Path
$linuxRepo = ((wsl.exe -- wslpath -a $repo) | Out-String).Trim()
$linuxLicense = ((wsl.exe -- wslpath -a $LicenseFile) | Out-String).Trim()
if ($LASTEXITCODE -ne 0 -or -not $linuxRepo -or -not $linuxLicense) {
    throw "Could not map Windows paths into WSL."
}
# Reject apostrophes to keep shell quoting unambiguous.
if ($linuxRepo.Contains("'") -or $linuxLicense.Contains("'")) {
    throw "Paths containing apostrophes are unsupported."
}
Write-Host "Starting EditMode and PlayMode tests from $repo"
Write-Host "The first run may download a large Unity editor image."
$cmd = "cd '$linuxRepo' && UNITY_LICENSE_FILE='$linuxLicense' bash tools/unity-ci/run-docker.sh"
wsl.exe -- bash -lc $cmd
if ($LASTEXITCODE -ne 0) { throw "Unity Docker tests failed (WSL exit $LASTEXITCODE). Examine artifacts/unity-ci." }
Write-Host "Test runner finished successfully. NUnit XML + logs are in artifacts/unity-ci."
