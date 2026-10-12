#!/usr/bin/env python3
"""Build independent, history-free UPM snapshots. Never modify source checkout."""
import argparse
import json
import hashlib
import pathlib
import shutil

ROOT = pathlib.Path(__file__).resolve().parents[1]
def included(path, channel):
    parts = path.parts
    if any(part in {".git", "Library", "Temp", "Logs", "Build", "Builds"} for part in parts):
        return False
    if channel == "upm":
        return not (parts[0] in {".github", "Project~", "competition", "scripts", "tests"} or
                    (len(parts) > 1 and parts[0] == "Textures" and
                     parts[1] in {"HDRI", "Surfaces"}))
    return len(parts) > 1 and parts[0] == "Textures" and parts[1] in {"HDRI", "Surfaces"}

def build(channel, dest):
    if channel == "upm":
        for file in ROOT.rglob("*"):
            rel = file.relative_to(ROOT)
            if not file.is_file() or not included(rel, channel):
                continue
            target = dest / rel
            target.parent.mkdir(parents=True, exist_ok=True)
            shutil.copy2(file, target)
    else:
        for required in ("Textures/HDRI", "Textures/Surfaces"):
            if not (ROOT / required).is_dir():
                raise RuntimeError(f"Missing optional asset directory: {required}")
        for file in (ROOT / "Textures").rglob("*"):
            rel = file.relative_to(ROOT)
            if file.is_file() and included(rel, channel):
                target = dest / "Samples~" / "HDRI-PBR" / rel
                target.parent.mkdir(parents=True, exist_ok=True)
                shutil.copy2(file, target)
        manifest = {
            "name": "pl.prz.kia.wirr.assets",
            "version": "1.0.0",
            "displayName": "WiRR Optional HDRI and PBR Assets",
            "description": "Optional large visual assets for WiRR; not required for laboratories.",
            "unity": "6000.0",
            "author": {"name": "Katedra Informatyki i Automatyki, PRz"},
            "samples": [{"displayName": "HDRI PBR", "description": "Opcjonalne biblioteki HDRI i PBR, importowane na żądanie.", "path": "Samples~/HDRI-PBR"}]
        }
        (dest / "package.json").write_text(json.dumps(manifest, indent=2, ensure_ascii=False)+"\n")
        (dest / "README.md").write_text(
            "# WiRR optional visual assets\n\n"
            "Install as a separate Unity Package Manager git dependency using "
            "`https://github.com/KIA-students/wirr.git#assets`. "
            "The core WiRR package is required separately.\n"
            "HDRI and PBR assets retain their original third-party licenses; "
            "see the source repository THIRD_PARTY_TEXTURES.md.\n")
        src = ROOT / "THIRD_PARTY_TEXTURES.md"
        if src.exists():
            shutil.copy2(src, dest / src.name)
    # Unity treats Git-installed packages as immutable: every importable file
    # must already have a stable .meta sidecar in the distribution.
    # Preserve existing Unity GUIDs; generate deterministic GUIDs only when absent.
    for asset in sorted(dest.rglob("*")):
        if not asset.is_file() or asset.suffix == ".meta" or asset.name == "package.json":
            continue
        if "Documentation~" in asset.relative_to(dest).parts or "Samples~" in asset.relative_to(dest).parts:
            continue
        meta = asset.with_name(asset.name + ".meta")
        if not meta.exists():
            guid = hashlib.md5(("pl.prz.kia.wirr/" + asset.relative_to(dest).as_posix()).encode("utf-8")).hexdigest()
            meta.write_text("fileFormatVersion: 2\\nguid: " + guid + "\\n", encoding="utf-8")
    count = sum(1 for p in dest.rglob("*") if p.is_file())
    size = sum(p.stat().st_size for p in dest.rglob("*") if p.is_file())
    if count < (20 if channel == "upm" else 3):
        raise RuntimeError(f"Unexpectedly small {channel} snapshot: {count} files")
    if channel == "upm" and any((dest / p).exists() for p in ("Project~", "Textures/HDRI", "Textures/Surfaces")):
        raise RuntimeError("Heavy files leaked into core package")
    if channel == "assets" and not any((dest / "Samples~" / "HDRI-PBR" / "Textures" / "HDRI").rglob("*.exr")):
        raise RuntimeError("Optional HDRI package is empty; refusing to publish")
    if not (dest / "package.json").exists():
        raise RuntimeError("UPM package.json missing")
    print(f"{channel}: {count} files, {size} bytes (working snapshot, not network download)")

def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("--output", required=True)
    args = ap.parse_args()
    out = pathlib.Path(args.output).resolve()
    if out == ROOT or ROOT in out.parents:
        raise SystemExit("Output must be outside source repository")
    out.mkdir(parents=True, exist_ok=True)
    for channel in ("upm", "assets"):
        dest = out / channel
        if dest.exists():
            shutil.rmtree(dest)
        dest.mkdir()
        build(channel, dest)

if __name__ == "__main__":
    main()
