import unittest
from pathlib import Path
import importlib.util

spec = importlib.util.spec_from_file_location("grade_audit", Path(__file__).with_name("grade_audit.py"))
module = importlib.util.module_from_spec(spec)
spec.loader.exec_module(module)

class AuditTests(unittest.TestCase):
    def test_missing_student_visible(self):
        rows = module.audit([{"student_id":"123","group":"G1"}], [])
        self.assertEqual(rows[0]["missing_count"], 8)
    def test_approved(self):
        rows = module.audit([{"student_id":"123","group":"G1"}],
            [{"student_id":"123","lab":"lab01","status":"approved","grade":"4.0"}])
        self.assertEqual(rows[0]["lab01_grade"], "4.0")
        self.assertEqual(rows[0]["approved_count"], 1)
    def test_duplicates_rejected(self):
        with self.assertRaises(ValueError):
            module.audit([{"student_id":"123","group":"G1"},{"student_id":"123","group":"G1"}], [])
    def test_unapproved_grade_rejected(self):
        with self.assertRaises(ValueError):
            module.audit([{"student_id":"123","group":"G1"}],
                [{"student_id":"123","lab":"lab01","status":"received","grade":"5.0"}])

if __name__ == "__main__":
    unittest.main()
