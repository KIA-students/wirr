"""Offline checks for the WiRR Data API privilege proposal."""
import pathlib
import re
import unittest

ROOT = pathlib.Path(__file__).resolve().parents[2]
SQL = ROOT / "tools/supabase/wirr_data_api_grants.sql"

class TestDataApiGrants(unittest.TestCase):
    def test_expected_permissions(self):
        sql = SQL.read_text(encoding="utf-8")
        expected = {
            "courses": {"SELECT"},
            "enrollments": {"SELECT"},
            "wirr_submissions": {"SELECT", "INSERT"},
            "wirr_grades": {"SELECT", "INSERT", "UPDATE"},
            "wirr_instructor_overview": {"SELECT"},
        }
        found = {}
        pattern = r"(?im)^GRANT\s+([A-Z,\s]+?)\s+ON\s+TABLE\s+public\.([a-z_]+)\s+TO\s+authenticated\s*;"
        for match in re.finditer(pattern, sql):
            self.assertNotIn(match.group(2), found)
            found[match.group(2)] = {v.strip().upper() for v in match.group(1).split(",")}
        self.assertEqual(found, expected)

    def test_no_unsafe_access_or_schema_changes(self):
        sql = SQL.read_text(encoding="utf-8")
        statements = re.sub(r"(?m)^\s*--[^\n]*$", "", sql).upper()
        self.assertNotRegex(statements, r"\bTO\s+(?:ANON|PUBLIC)\b")
        self.assertNotRegex(statements, r"\b(?:CREATE|DROP|ALTER|TRUNCATE|DELETE|REVOKE)\b")
        self.assertNotRegex(statements, r"\bGRANT\s+ALL\b")
        self.assertNotRegex(statements, r"\bSERVICE_ROLE\b")
        self.assertIn("GRANT USAGE ON SCHEMA public TO authenticated;", sql)

if __name__ == "__main__":
    unittest.main()
