"""Host checks for asset packaging. These are not Unity compilation or PlayMode tests."""
from __future__ import annotations

import csv
import importlib.util
import io
import json
import re
import unittest
from itertools import pairwise
from pathlib import Path

import yaml
from build_student_materials import NAMES, ROOT, guid


class TestStudentMaterials(unittest.TestCase):
    def test_all_eight_instruction_resources_have_three_actions_and_five_criteria(self):
        for lab in range(1, 9):
            with self.subTest(lab=lab):
                data = json.loads((ROOT / f"Runtime/Resources/WiRR2/lab{lab:02}.json").read_text())
                self.assertEqual(data["id"], lab)
                self.assertEqual(len(data["actions"]), 3)
                self.assertEqual(len(data["rubric"]), 5)
                self.assertTrue(data["question"])
                self.assertNotIn("MatPomGit", json.dumps(data))
                self.assertEqual(data["mode"], "SIM_AR" if lab in (3, 4) else "ROS_REPLAY" if lab == 6 else "DESKTOP_SIM")

    def test_scene_references_resolve_to_the_runtime_script(self):
        script_guid = yaml.safe_load((ROOT / "Runtime/WiRR2/WiRRStudentLab.cs.meta").read_text())["guid"]
        for lab, name in enumerate(NAMES, 1):
            with self.subTest(lab=lab):
                scene = (ROOT / f"Samples~/Lab{lab:02}/WiRR2/{name}.unity").read_text()
                self.assertIn(f"guid: {script_guid}", scene)
                plain = re.sub(r"^%.*\n", "", scene, flags=re.MULTILINE)
                plain = re.sub(r"^--- !u!\d+ &\d+", "---", plain, flags=re.MULTILINE)
                documents = list(yaml.safe_load_all(plain))
                behaviour = next(item["MonoBehaviour"] for item in documents if "MonoBehaviour" in item)
                self.assertEqual(behaviour["labNumber"], lab)
                self.assertEqual(len(documents), 4)

    def test_reference_replay_is_finite_monotonic_and_named(self):
        rows = list(csv.DictReader(io.StringIO((ROOT / "Runtime/Resources/WiRR2/jointstate-replay.csv").read_text())))
        self.assertEqual(len(rows), 301)
        self.assertEqual(list(rows[0]), ["time_s", "joint2_rad", "joint1_rad"])
        times = [float(row["time_s"]) for row in rows]
        self.assertTrue(all(a < b for a, b in pairwise(times)))
        self.assertEqual(times[-1], 30)
        self.assertTrue(all(-1 <= float(row[key]) <= 1 for row in rows for key in ("joint1_rad", "joint2_rad")))

    def test_imported_assets_have_unique_meta_guids(self):
        folders = [ROOT / "Runtime/WiRR2", ROOT / "Runtime/Resources/WiRR2", ROOT / "Editor/WiRR2", ROOT / "Tests"]
        folders.extend(ROOT / f"Samples~/Lab{lab:02}/WiRR2" for lab in range(1, 9))
        guids = []
        for folder in folders:
            for path in folder.rglob("*"):
                if path.is_file() and path.suffix != ".meta":
                    value = yaml.safe_load(path.with_name(path.name + ".meta").read_text())["guid"]
                    self.assertEqual(value, guid(path.relative_to(ROOT).as_posix()))
                    guids.append(value)
        self.assertEqual(len(guids), len(set(guids)))

    def test_csharp_syntax(self):
        import tree_sitter
        import tree_sitter_c_sharp
        parser = tree_sitter.Parser(tree_sitter.Language(tree_sitter_c_sharp.language()))
        for folder in (ROOT / "Runtime/WiRR2", ROOT / "Editor/WiRR2", ROOT / "Tests"):
            for path in folder.rglob("*.cs"):
                with self.subTest(path=path):
                    self.assertFalse(parser.parse(path.read_bytes()).root_node.has_error)

    def test_manifest_covers_all_instruction_inputs(self):
        manifest = json.loads((ROOT / "tools/wirr2/instruction-inputs.json").read_text())
        self.assertEqual(len(manifest["inputs_sha256"]), 9)
        for value in manifest["inputs_sha256"].values():
            self.assertRegex(value, r"^[a-f0-9]{64}$")

    def test_existing_grader_accepts_export_contract_without_granting_a_grade(self):
        # Synthetic interoperability fixture. Actual C# output is emitted by the Unity test.
        spec = importlib.util.spec_from_file_location("grader", ROOT / "scripts~/wirr_grade_report.py")
        grader = importlib.util.module_from_spec(spec)
        spec.loader.exec_module(grader)
        data = {"schemaVersion": "wirr-report/2.0-draft.1", "submissionId": "fixture-lab01", "teamId": "fixture",
                "studentIndices": ["999991"], "labNumber": 1, "createdAtUtc": "2026-10-16T08:00:00Z",
                "versions": {"unity": "synthetic", "package": "1.1.4", "experiment": "student-kit/0.1.0"},
                "activityTimeline": [{"eventName": "local_evidence_exported", "labNumber": 1,
                                      "timestampUtc": "2026-10-16T08:00:00Z", "durationSeconds": 0}],
                "activitySummary": {"eventCount": 1}, "checkpointEvidence": {}, "trials": [], "conclusion": ""}
        result = grader.evaluate_wirr2(data, Path("synthetic.json"))
        self.assertEqual(result["status"], "PROVISIONAL")
        self.assertEqual(result["achieved_checkpoints"], [])
        self.assertIsNone(result["proposed_grade"])


if __name__ == "__main__":
    unittest.main()
