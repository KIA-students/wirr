"""Build lightweight Unity teaching assets from the canonical instruction sources.

Run with --course-root pointing to the WiRR course directory in the teaching repo.
No Unity/editor/license/download is required. This does not validate PlayMode.
"""
from __future__ import annotations

import argparse
import hashlib
import json
import math
import random
import re
import uuid
from pathlib import Path

import yaml

ROOT = Path(__file__).resolve().parents[2]
NAMES = (
    "Lab01_Station", "Lab02_Interaction", "Lab03_AnchorGallery", "Lab04_DepthRoom",
    "Lab05_CADWorkshop", "Lab06_DigitalTwin", "Lab07_Expedition", "Lab08_ExperienceStudio",
)


def guid(path: str) -> str:
    return uuid.uuid5(uuid.NAMESPACE_URL, "https://github.com/KIA-students/wirr/" + path).hex


def scene(number: int) -> str:
    script = guid("Runtime/WiRR2/WiRRStudentLab.cs")
    return f"""%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!1 &1000
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: 1001}}
  - component: {{fileID: 1002}}
  m_Layer: 0
  m_Name: {NAMES[number - 1]}
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!4 &1001
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 1000}}
  serializedVersion: 2
  m_LocalRotation: {{x: 0, y: 0, z: 0, w: 1}}
  m_LocalPosition: {{x: 0, y: 0, z: 0}}
  m_LocalScale: {{x: 1, y: 1, z: 1}}
  m_ConstrainProportionsScale: 0
  m_Children: []
  m_Father: {{fileID: 0}}
  m_LocalEulerAnglesHint: {{x: 0, y: 0, z: 0}}
--- !u!114 &1002
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 1000}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: {script}, type: 3}}
  m_Name:
  m_EditorClassIdentifier:
  labNumber: {number}
--- !u!1660057539 &9223372036854775807
SceneRoots:
  m_ObjectHideFlags: 0
  m_Roots:
  - {{fileID: 1001}}
"""


def meta(path: Path) -> None:
    relative = path.relative_to(ROOT).as_posix()
    target = path.with_name(path.name + ".meta")
    if target.exists():
        return
    importer = "MonoImporter" if path.suffix == ".cs" else "TextScriptImporter" if path.suffix in (".json", ".csv", ".md") else "ShaderImporter" if path.suffix == ".shader" else "AssemblyDefinitionImporter" if path.suffix == ".asmdef" else "DefaultImporter"
    extra = "  serializedVersion: 2\n  defaultReferences: []\n  executionOrder: 0\n  icon: {instanceID: 0}\n" if path.suffix == ".cs" else ""
    folder_flag = "folderAsset: yes\n" if path.is_dir() else ""
    target.write_text(f"fileFormatVersion: 2\nguid: {guid(relative)}\n{folder_flag}{importer}:\n  externalObjects: {{}}\n{extra}  userData:\n  assetBundleName:\n  assetBundleVariant:\n", encoding="utf-8")


def build(course: Path) -> None:
    rubric = course / "wirr-2.0/07-PROGI-OCEN.md"
    rows = {}
    for line in rubric.read_text(encoding="utf-8").splitlines():
        if re.match(r"\| \*\*0[1-8]\*\* ", line):
            cells = [cell.strip() for cell in line.strip("|").split("|")]
            number = int(re.search(r"\*\*(\d+)\*\*", cells[0])[1])
            if len(cells) != 6 or number in rows:
                raise ValueError("Ambiguous rubric")
            rows[number] = cells[1:]
    if set(rows) != set(range(1, 9)):
        raise ValueError("Expected eight labs and five criteria per lab")
    resources = ROOT / "Runtime/Resources/WiRR2"
    resources.mkdir(parents=True, exist_ok=True)
    sources = {rubric.name: hashlib.sha256(rubric.read_bytes()).hexdigest()}
    for number, name in enumerate(NAMES, 1):
        source = course / f"laboratories/lab-{number:02}/src/instruction.yaml"
        data = yaml.safe_load(source.read_text(encoding="utf-8"))
        if data["id"] != number or len(data["actions"]) != 3:
            raise ValueError(f"Invalid instruction {source}")
        lesson = {key: data[key] for key in ("id", "title", "question", "mode", "concept", "controls", "invalid", "conclusion")}
        lesson.update(actions=[{"title": title, "body": body} for title, body in data["actions"]],
                      rubric=rows[number], instruction_version=data["version"],
                      source_sha256=hashlib.sha256(source.read_bytes()).hexdigest())
        sources[f"lab-{number:02}/instruction.yaml"] = lesson["source_sha256"]
        (resources / f"lab{number:02}.json").write_text(json.dumps(lesson, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
        folder = ROOT / f"Samples~/Lab{number:02}/WiRR2"
        folder.mkdir(parents=True, exist_ok=True)
        (folder / (name + ".unity")).write_text(scene(number), encoding="utf-8")
        (folder / "README.md").write_text(
            f"# {data['title']}\n\nMateriały pilotażowe do instrukcji {data['version']}.\n\n"
            "Prowadzący przygotowuje projekt przez `WiRR → Nowe instrukcje → Przygotuj osiem scen`. "
            "Student otwiera gotową scenę i uruchamia Play. Nie instaluje SDK ani nie pisze kodu.\n\n"
            f"Scena: `{name}.unity`; źródło danych: **{data['mode']}**. "
            "Polecenia i progi są dostępne bez sieci w panelu sceny. Profil solo/para jest wspólny dla dwóch laboratoriów.\n\n"
            "Po pracy eksportuj dwa wyniki bloku. Zapis lokalny nie oznacza wysłania ani uzyskania oceny. "
            "Pełne podłączenie nowych checkpointów i odbiór na stanowiskach pozostają otwarte.\n\n"
            "Materiały kursu: https://github.com/KIA-students/wirr\n", encoding="utf-8")
    replay = ["time_s,joint2_rad,joint1_rad"]
    replay.extend(f"{i / 10:.1f},{0.4 * math.cos(i / 10):.6f},{0.6 * math.sin(i / 10):.6f}" for i in range(301))
    (resources / "jointstate-replay.csv").write_text("\n".join(replay) + "\n", encoding="utf-8")
    def vector(x: float, y: float, z: float) -> dict:
        return {"x": x, "y": y, "z": z}

    anchor = {
        "version": "anchor-model/1", "source": "SYNTHETIC_MODEL_NOT_DEVICE", "seed": 2718,
        "cadence_s": 1, "recovery_delay_s": 1.5, "recovery_timeout_s": 5,
        "profiles": [
            {"name": "A_many_features", "bias_m": vector(0.005, 0, 0),
             "relocalization_shift_m": vector(0.01, 0, 0),
             "jitter_m": [vector(-0.002, 0.001, 0), vector(0.002, -0.001, 0.001), vector(0, 0, -0.001)]},
            {"name": "B_few_features", "bias_m": vector(0.025, 0, 0),
             "relocalization_shift_m": vector(0.03, 0.01, 0),
             "jitter_m": [vector(-0.014, 0.007, 0), vector(0.014, -0.007, 0.007), vector(0, 0, -0.007)]},
        ],
    }
    order = [0, 1, 2]
    random.Random(anchor["seed"]).shuffle(order)
    for profile in anchor["profiles"]:
        profile["jitter_m"] = [profile["jitter_m"][index] for index in order]
    (resources / "anchor-tracking.json").write_text(json.dumps(anchor, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    manifest = ROOT / "tools/wirr2/instruction-inputs.json"
    manifest.write_text(json.dumps({"version": "student-kit/0.1.6", "inputs_sha256": sources}, indent=2) + "\n", encoding="utf-8")
    # Only assets introduced by this kit. Never modify unrelated package metadata.
    for folder in [ROOT / "Runtime/WiRR2", resources, ROOT / "Editor/WiRR2", ROOT / "Tests"]:
        for path in folder.rglob("*"):
            if path.is_file() and path.suffix != ".meta":
                meta(path)
    for number in range(1, 9):
        for path in (ROOT / f"Samples~/Lab{number:02}/WiRR2").glob("*"):
            if path.is_file() and path.suffix != ".meta":
                meta(path)
    folders = [ROOT / "Runtime/WiRR2", ROOT / "Runtime/Resources", resources, ROOT / "Editor/WiRR2",
               ROOT / "Tests", ROOT / "Tests/Editor", ROOT / "Tests/Runtime"]
    folders.extend(ROOT / f"Samples~/Lab{lab:02}/WiRR2" for lab in range(1, 9))
    for folder in folders:
        meta(folder)


if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("--course-root", required=True, type=Path)
    build(parser.parse_args().course_root)
