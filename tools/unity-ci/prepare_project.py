#!/usr/bin/env python3
"""Create a disposable Unity test project for this UPM package."""
import argparse
import json
import os
import shutil
from pathlib import Path

def prepare(root: Path, output: Path, editor_version: str, container_package_path: str | None = None, stage_package: bool = False) -> None:
    root = root.resolve()
    output = output.resolve()
    package_path = root / "package.json"
    package_text = package_path.read_text(encoding="utf-8-sig")
    if not package_text.strip():
        raise ValueError(f"Empty Unity package manifest: {package_path}")
    try:
        package = json.loads(package_text)
    except json.JSONDecodeError as exc:
        raise ValueError(f"Invalid Unity package manifest {package_path}: {exc}") from exc
    if package["name"] != "pl.prz.kia.wirr":
        raise ValueError("Unexpected package identity")
    if output == root or root in output.parents:
        raise ValueError("Test project must be outside the package checkout")
    packages = output / "Packages"
    settings = output / "ProjectSettings"
    (output / "Assets").mkdir(parents=True, exist_ok=True)
    packages.mkdir(parents=True, exist_ok=True)
    settings.mkdir(parents=True, exist_ok=True)
    # Local UPM file dependencies may use absolute paths across Windows drives.
    try:
        local_package = os.path.relpath(root, packages).replace(chr(92), "/")
    except ValueError:
        local_package = root.as_posix()
    if container_package_path is not None:
        # Docker mounts project at /workspace/project and package at /workspace/package.
        # The manifest is resolved inside the container, not on the host.
        if container_package_path != "/workspace/package":
            raise ValueError("Unexpected Docker package mount")
        local_package = "../../package"
    if stage_package:
        # Import only package assets, not developer tools or previous test evidence.
        # A writable isolated copy also prevents Unity from generating .meta files in the checkout.
        staged = output / "PackageSource~"
        if staged.exists():
            raise ValueError("Staged package already exists; use a fresh test project")
        staged.mkdir()
        for name in ("Editor", "Runtime", "Tests", "Samples~", "Textures/Grid"):
            source = root / name
            if source.is_dir():
                shutil.copytree(source, staged / name)
                metadata = source.with_name(source.name + ".meta")
                if metadata.is_file():
                    shutil.copy2(metadata, (staged / name).with_name(source.name + ".meta"))
        for name in ("package.json", "package.json.meta", "Editor.meta", "Runtime.meta", "Tests.meta", "Textures.meta"):
            if (root / name).is_file():
                shutil.copy2(root / name, staged / name)
        local_package = "../PackageSource~"
    # Built-in modules must be explicit in a project generated from scratch.
    # WebCamTexture is also forwarded through UnityEngine.AudioModule in Unity 6.
    builtins = (
        "audio", "imageconversion", "physics", "physics2d", "animation",
        "jsonserialize", "ui", "imgui", "screencapture",
        "unitywebrequest", "unitywebrequestassetbundle", "unitywebrequestaudio", "unitywebrequesttexture",
        "xr", "video", "terrain", "terrainphysics",
        "particlesystem", "tilemap", "assetbundle", "director",
        "cloth", "vehicles", "accessibility",
    )
    course_manifest = root / "Project~" / "Packages" / "manifest.json"
    course_dependencies = json.loads(course_manifest.read_text(encoding="utf-8"))["dependencies"] if course_manifest.exists() else {}
    modules = {"com.unity.modules." + name: "1.0.0" for name in builtins}
    modules.update({key: value for key, value in course_dependencies.items() if key.startswith("com.unity.modules.")})
    manifest = {
        "dependencies": {
            "pl.prz.kia.wirr": "file:" + local_package,
            "com.unity.test-framework": course_dependencies.get("com.unity.test-framework", "1.8.0"),
            "com.unity.render-pipelines.universal": course_dependencies.get("com.unity.render-pipelines.universal", "17.6.0"),
            "com.unity.inputsystem": package["dependencies"].get("com.unity.inputsystem", "1.20.0"),
            **modules,
        },
        "testables": ["pl.prz.kia.wirr"],
    }
    (packages / "manifest.json").write_text(json.dumps(manifest, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    (settings / "ProjectVersion.txt").write_text("m_EditorVersion: " + editor_version + "\n", encoding="utf-8")
    (settings / "ProjectSettings.asset").write_text(
        "%YAML 1.1\n%TAG !u! tag:unity3d.com,2011:\n--- !u!129 &1\n"
        "PlayerSettings:\n  m_ObjectHideFlags: 0\n  serializedVersion: 26\n"
        "  activeInputHandler: 1\n", encoding="utf-8")
    print("Created isolated Unity UPM test project:", output)

if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("--package", type=Path, required=True)
    parser.add_argument("--project", type=Path, required=True)
    parser.add_argument("--unity", required=True)
    parser.add_argument("--container-package-path", choices=["/workspace/package"])
    parser.add_argument("--stage-package", action="store_true")
    args = parser.parse_args()
    prepare(args.package, args.project, args.unity, args.container_package_path, args.stage_package)
