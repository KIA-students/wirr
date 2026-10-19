"""Unit tests: arithmetic anomalies are flags, never allegations."""
import importlib.util
from pathlib import Path
import unittest

SCRIPT = Path(__file__).with_name("wirr_integrity_screen.py")
spec = importlib.util.spec_from_file_location("wirr_integrity_screen", SCRIPT)
mod = importlib.util.module_from_spec(spec)
spec.loader.exec_module(mod)


def report(**values):
    return {"answers": [{"key": k, "value": v} for k, v in values.items()]}


class TestIntegrityScreen(unittest.TestCase):
    def test_consistent_fps(self):
        result = mod.scan(report(**{"table.x.a.mediana_fps": "50", "table.x.a.mediana_ms": "20"}))
        self.assertEqual(result["findings"], [])
        self.assertEqual(result["checks_performed"], 1)

    def test_discrepancy_is_only_flag(self):
        result = mod.scan(report(**{"table.x.a.mediana_fps": "50", "table.x.a.mediana_ms": "50"}))
        self.assertEqual(result["findings"][0]["type"], "fps_ms_check")
        self.assertEqual(result["evidence_status"], "screening_only_not_authenticity_proof")

    def test_raw_trials_median(self):
        values = {"table.lab02_cp30_conditions.a.mediana_s": "3"}
        values.update({"table.lab02_cp30_conditions.a.próba_%s_s" % i: str(v)
                       for i, v in enumerate((1, 4, 3, 5, 2), 1)})
        self.assertEqual(mod.scan(report(**values))["findings"], [])

    def test_incorrect_median(self):
        values = {"table.x.a.mediana_s": "19"}
        values.update({"table.x.a.próba_%s_s" % i: str(v)
                       for i, v in enumerate((1, 2, 3), 1)})
        self.assertEqual(mod.scan(report(**values))["findings"][0]["type"], "median_check")

    def test_no_raw_trials_is_unverified_not_fraud(self):
        result = mod.scan(report(**{"table.x.a.mediana_s": "2.1"}))
        self.assertEqual(result["checks_performed"], 0)
        self.assertEqual(result["reported_measurements"], 1)
        self.assertEqual(result["unchecked_measurements"], 1)
        self.assertEqual(result["findings"], [])

    def test_no_frame_time_is_unverified(self):
        result = mod.scan(report(**{"table.x.a.mediana_fps": "60"}))
        self.assertEqual(result["reported_measurements"], 1)
        self.assertEqual(result["unchecked_measurements"], 1)
        self.assertEqual(result["findings"], [])

    def test_missing_data_is_not_an_offense(self):
        self.assertEqual(mod.scan(report())["findings"], [])


if __name__ == "__main__":
    unittest.main()
