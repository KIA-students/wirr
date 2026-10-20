"""Integrity regression tests; synthetic data are never Unity acceptance evidence."""
import copy
import json
import subprocess
import sys
import tempfile
import unittest
from pathlib import Path

from test_wirr_grade_report import evaluate, grader, report


def sample(lab=1):
    data = report()
    data.update({
        "schemaVersion": grader.WIRR2_SCHEMA,
        "labNumber": lab,
        "createdAtUtc": "2026-10-15T18:00:00Z",
        "versions": {"unity": "6000.0", "package": "1.1.4", "experiment": "fixture/1"},
        "trials": [{
            "trial_id": "trial-a", "condition": {"preset": "baseline"},
            "device_mode": "DESKTOP_SIM", "participant_mode": "SOLO",
            "timestampUtc": "2026-10-15T18:00:00Z", "validity": "VALID",
            "metrics": {"frame_time": {"value": 0, "unit": "ms"}}
        }],
        "checkpointEvidence": {grade: {"trial_ids": ["trial-a"]} for grade, _ in grader.CHECKPOINTS},
        "conclusion": "Syntetyczny tekst do testu obecności pola."
    })
    data["activityTimeline"][0]["labNumber"] = lab
    return data


class TestWiRR2Evidence(unittest.TestCase):
    def test_all_labs_and_thresholds_remain_provisional(self):
        # 40 structural paths, deliberately NOT task-specific grade validation.
        for lab in range(1, 9):
            for count in range(1, 6):
                with self.subTest(lab=lab, count=count):
                    data = sample(lab)
                    grades = [grade for grade, _ in grader.CHECKPOINTS]
                    data["checkpointEvidence"] = {g: data["checkpointEvidence"][g] for g in grades[:count]}
                    result = evaluate(data)
                    self.assertEqual(result["status"], "PROVISIONAL")
                    self.assertEqual(result["structurally_complete_checkpoints"], grades[:count])
                    self.assertIsNone(result["proposed_grade"])
                    self.assertEqual(result["achieved_checkpoints"], [])
                    self.assertTrue(result["requiresInstructorApproval"])

    def test_missing_zero_and_unavailable_remain_distinct(self):
        data = sample()
        self.assertEqual(evaluate(data)["status"], "PROVISIONAL")
        metric = data["trials"][0]["metrics"]["frame_time"]
        metric["value"] = None
        self.assertEqual(evaluate(data)["status"], "ERROR")
        metric["unavailable_reason"] = "Brak pomiaru"
        self.assertEqual(evaluate(data)["status"], "ERROR")  # no measured value in VALID
        data["trials"][0].update(validity="UNAVAILABLE", reason="Brak miernika")
        result = evaluate(data)
        self.assertEqual(result["status"], "PROVISIONAL")
        self.assertEqual(result["structurally_complete_checkpoints"], [])

    def test_invalid_numeric_measurements_and_units(self):
        for value in (True, "12", float("nan"), float("inf"), -float("inf")):
            with self.subTest(value=value):
                data = sample()
                data["trials"][0]["metrics"]["frame_time"]["value"] = value
                self.assertEqual(evaluate(data)["status"], "ERROR")
        data = sample()
        del data["trials"][0]["metrics"]["frame_time"]["unit"]
        self.assertEqual(evaluate(data)["status"], "ERROR")

    def test_duplicate_unknown_failed_and_mixed_source_references(self):
        for case in ("duplicate_trial", "unknown", "duplicate_reference", "failed", "mixed"):
            with self.subTest(case=case):
                data = sample()
                trial = data["trials"][0]
                if case == "duplicate_trial":
                    data["trials"].append(copy.deepcopy(trial))
                elif case == "unknown":
                    data["checkpointEvidence"]["3.0"]["trial_ids"] = ["missing"]
                elif case == "duplicate_reference":
                    data["checkpointEvidence"]["3.0"]["trial_ids"] *= 2
                elif case == "failed":
                    trial.update(validity="FAILED", reason="Próba przerwana")
                else:
                    other = copy.deepcopy(trial)
                    other.update(trial_id="trial-b", device_mode="REAL_AR")
                    data["trials"].append(other)
                    data["checkpointEvidence"]["3.0"]["trial_ids"].append("trial-b")
                self.assertEqual(evaluate(data)["structurally_complete_checkpoints"], [])

    def test_no_skipping_and_no_essay_at_first_two_levels(self):
        data = sample()
        del data["conclusion"]
        self.assertEqual(evaluate(data)["structurally_complete_checkpoints"], ["3.0", "3.5"])
        del data["checkpointEvidence"]["3.0"]
        self.assertEqual(evaluate(data)["structurally_complete_checkpoints"], [])

    def test_interrupted_session_and_independent_labs(self):
        data = sample(1)
        data.update(trials=[], checkpointEvidence={})
        result = evaluate(data)
        self.assertEqual(result["status"], "PROVISIONAL")
        self.assertEqual(result["structurally_complete_checkpoints"], [])
        self.assertEqual(len(evaluate(sample(2))["structurally_complete_checkpoints"]), 5)

    def test_malformed_fields_return_errors(self):
        changes = {"studentIndices": {}, "labNumber": True, "createdAtUtc": "2026-10-15T18:00:00",
                   "versions": [], "trials": {}, "checkpointEvidence": []}
        for key, value in changes.items():
            with self.subTest(key=key):
                data = sample()
                data[key] = value
                self.assertEqual(evaluate(data)["status"], "ERROR")
        for key in ("device_mode", "participant_mode", "validity", "condition", "metrics"):
            with self.subTest(trial_key=key):
                data = sample()
                data["trials"][0][key] = []
                self.assertEqual(evaluate(data)["status"], "ERROR")

    def test_pair_and_participant_source_are_separate(self):
        data = sample()
        data["studentIndices"].append("173336")
        data["trials"][0]["participant_mode"] = "GUEST_SIM"
        self.assertEqual(evaluate(data)["status"], "PROVISIONAL")

    def test_cli_offline_export_and_exit_codes(self):
        with tempfile.TemporaryDirectory() as folder:
            source = Path(folder) / "report.json"
            output = Path(folder) / "results" / "check.json"
            for data, expected in ((sample(), 0), ({**sample(), "versions": {}}, 1), ([], 2)):
                with self.subTest(expected=expected):
                    source.write_text(json.dumps(data), encoding="utf-8")
                    if output.exists():
                        output.unlink()
                    run = subprocess.run([sys.executable, str(grader.__file__), str(source), "--output", str(output)],
                                         capture_output=True, text=True, check=False)
                    self.assertEqual(run.returncode, expected, run.stderr)
                    self.assertEqual(output.exists(), expected != 2)
                    if output.exists():
                        self.assertTrue(json.loads(output.read_text())["requiresInstructorApproval"])


if __name__ == "__main__":
    unittest.main()
