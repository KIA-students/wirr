#!/usr/bin/env python3
"""Create a disposable Unity test project for this UPM package."""
import argparse
import json
import os
from pathlib import Path

def prepare(root: Path, output: Path, editor_version: str) -> None:
    root = root.resolve()
    output = output.resolve()
    package = json.loads((root / "package.json").read_text(encoding="utf-8"))
    if package["name"] != "pl.prz.kia.wirr":
        raise ValueError("Unexpected package identity")
    if output == root or root in output.parents:
        raise ValueError("Test project must be outside the package checkout")
    packages = output / "Packages"
    settings = output / "ProjectSettings"
    (output / "Assets").mkdir(parents=True, exist_ok=True)
    packages.mkdir(parents=True, exist_ok=True)
    settings.mkdir(parents=True, exist_ok=True)
    # Compute the path relative to Packages for Docker and native Windows alike.
    manifest = {
        "dependencies": {
            "pl.prz.kia.wirr": "file:" + os.path.relpath(root, packages).replace(chr(92), "/"),
            "com.unity.test-framework": "1.5.1",
            "com.unity.render-pipelines.universal": "17.0.3",
            "com.unity.inputsystem": package["dependencies"].get("com.unity.inputsystem", "1.20.0"),
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
    args = parser.parse_args()
    prepare(args.package, args.project, args.unity)
