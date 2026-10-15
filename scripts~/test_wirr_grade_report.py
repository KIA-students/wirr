"""Regression checks for simplified and legacy WiRR report validation."""
import importlib.util
import json
import tempfile
import unittest
from pathlib import Path

MODULE_PATH = Path(__file__).with_name("wirr_grade_report.py")
spec = importlib.util.spec_from_file_location("wirr_grade_report", MODULE_PATH)
grader = importlib.util.module_from_spec(spec)
spec.loader.exec_module(grader)


def report(indices=("173382",), mode="basic", missing=None):
    values = {
        "basic.mode": "true",
        "basic.source": "UNITY_EDITOR",
        "basic.observation": "Zaobserwowano różnicę.",
        "basic.change": "Zmieniono jeden warunek.",
        "basic.conclusion": "Efekt zależy od warunku."
    } if mode == "basic" else {"cp30.answer": "Wniosek"}
    if missing:
        values[missing] = ""
    return {
        "schemaVersion": "wirr-report/1.0",
        "labNumber": 1,
        "submissionId": "d65cf1b8cfc04aaba238cf61292d7da7",
        "teamId": "S173382",
        "studentIndices": list(indices),
        "activityTimeline": [{
            "eventName": "report_exported",
            "timestampUtc": "2026-10-14T20:00:00Z",
            "labNumber": 1,
            "durationSeconds": 0.0
        }],
        "activitySummary": {"eventCount": 1},
        "answers": [{"key": k, "value": v} for k, v in values.items()]
    }


def evaluate(data):
    with tempfile.TemporaryDirectory() as folder:
        path = Path(folder) / "report.json"
        path.write_text(json.dumps(data), encoding="utf-8")
        return grader.evaluate(path)


class TestWiRRGradeReport(unittest.TestCase):
    def test_short_report_single_student(self):
        result = evaluate(report())
        self.assertEqual(result["status"], "VALID")
        self.assertEqual(result["reportMode"], "basic")

    def test_short_report_pair(self):
        result = evaluate(report(indices=("173382", "173336")))
        self.assertEqual(result["status"], "VALID")

    def test_missing_required_observation_fails(self):
        result = evaluate(report(missing="basic.observation"))
        self.assertEqual(result["status"], "ERROR")
        self.assertIn("Uzupełnij pole raportu: basic.observation.", result["blockingIssues"])

    def test_duplicate_indices_fail(self):
        self.assertEqual(evaluate(report(indices=("173382", "173382")))["status"], "ERROR")

    def test_legacy_report_remains_accepted(self):
        result = evaluate(report(mode="legacy"))
        self.assertEqual(result["status"], "VALID")
        self.assertEqual(result["reportMode"], "legacy")


if __name__ == "__main__":
    unittest.main()
