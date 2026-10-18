# WiRR Unity Docker validation

A manual, opt-in Linux x86_64 Unity 6 test harness. The package checkout is
read-only inside the container. An isolated temporary Unity project installs the
current checkout via UPM `file:` and enables its package test assemblies.
The wrapper runs EditMode and PlayMode separately and preserves NUnit XML and
Unity logs in `artifacts/unity-ci`. It treats missing result XML as failure.

## Usage

Prerequisites: Docker on Linux x86_64 (or Linux under WSL2), Python 3, sufficient
disk space (the image is several GB), network access to Unity's package registry,
and a lawfully activated Unity editor license valid for batch/container use.
Do not commit .ulf, passwords or activation credentials.

```sh
export UNITY_LICENSE_FILE=/secure/location/Unity_lic.ulf
export UNITY_VERSION=6000.3.24f1
bash tools/unity-ci/run-docker.sh
```

Default container is `unityci/editor:ubuntu-${UNITY_VERSION}-base-3.2.2`.
Override `UNITY_IMAGE` to an **existing** tag matching the Unity version
used by the project. Confirm the tag exists before running; do not infer that
every patch release has an image. The pinned baseline is for reproducibility,
not a declaration that it has been validated against WiRR's preferred editor.

For manual GitHub Actions dispatch use `Unity Docker validation (manual,
self-hosted)` on a Linux x86_64 runner. Configure the repository variable
`UNITY_LICENSE_FILE` with a file path present on that runner. The workflow
does not run automatically on pushes or PRs, and does not provision or purchase
any Unity license. Treat runner file permissions as sensitive: do not let
untrusted PR code access production credentials.

Tests in this Docker configuration do not prove Windows `File.Replace`
behavior, GPU rendering accuracy, HMD interactions or real AR/MR support.
A Linux container is **not** a replacement for Windows + hardware acceptance.
Review `EditMode.xml`, `PlayMode.xml`, both logs and the editor version,
then perform a separate workstation smoke test before claiming Unity validation is complete.

The test harness itself has not yet been executed in a Unity container.

## Andromeda (Windows PowerShell + Docker Desktop)

Docker Desktop Linux containers work directly from PowerShell. WSL distro integration is **not required**. Use `powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\\tools\\unity-ci\\run-andromeda.ps1 -PreflightOnly` to verify Docker. Then run the same command without `-PreflightOnly`; a Unity `.ulf` license is required, optionally via `-LicenseFile`. Results appear under `artifacts/unity-ci`. The Windows launcher uses Docker bind mounts and bypasses the Linux-only Bash runner. This configuration has not yet been exercised against a Unity editor image.

## Native Unity Editor on Andromeda (Unity Personal)

Preferred test path for the existing Windows license, requiring no Docker or Linux activation:

```powershell
cd C:\Users\matpo\repo\wirr
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\tools\unity-ci\run-windows.ps1 -PreflightOnly
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\tools\unity-ci\run-windows.ps1
```

The runner uses `D:\Unity\UnityEditors\6000.6.0f1\Editor\Unity.exe`, creates an isolated temporary UPM project and runs EditMode then PlayMode. Output: `artifacts/unity-native/{EditMode.xml,PlayMode.xml,EditMode.log,PlayMode.log,summary.json}`. The package is read through a local `file:` dependency without changing the existing project `D:\Unity\UnityProjects\WiRR`. No extra license purchase or secret copying. The Docker backend remains optional. **This Windows runner is implemented but has not yet been executed on Andromeda.**
