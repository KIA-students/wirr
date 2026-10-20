"""Offline, fail-closed checks of the proposed WiRR Data API grants.

No Supabase credentials, network access or database are required.
"""
from pathlib import Path
import re
import unittest

ROOT = Path(__file__).resolve().parents[2]
SQL = ROOT / "tools/supabase/wirr_data_api_grants.sql"
EXPECTED = {
    "courses": {"SELECT"},
    "enrollments": {"SELECT"},
    "wirr_submissions": {"SELECT", "INSERT"},
    "wirr_grades": {"SELECT", "INSERT"},
    "wirr_instructor_overview": {"SELECT"},
}
GRANT_TABLE = re.compile(
    r"GRANT\s+([A-Z]+(?:\s*,\s*[A-Z]+)*)\s+ON\s+TABLE\s+"
    r"public\.([a-z_]+)\s+TO\s+authenticated",
    re.IGNORECASE | re.ASCII,
)
GRANT_SCHEMA = "GRANT USAGE ON SCHEMA public TO authenticated"


def validate(sql: str) -> list[str]:
    """Return policy violations instead of executing the SQL."""
    clean = re.sub(r"--[^\n]*", "", sql)
    statements = [s.strip() for s in clean.split(";") if s.strip()]
    required = {
        "GRANT UPDATE (grade, graded_by, graded_at) ON TABLE public.wirr_grades TO authenticated",
        "GRANT USAGE ON SCHEMA public TO service_role",
        "GRANT SELECT, INSERT ON TABLE public.wirr_public_intake TO service_role",
    }
    permitted = {"BEGIN", "COMMIT", GRANT_SCHEMA} | required
    seen = {}
    errors = []
    for statement in statements:
        normalized = re.sub(r"\s+", " ", statement).strip()
        if normalized in permitted:
            continue
        match = GRANT_TABLE.fullmatch(normalized)
        if not match:
            errors.append(f"Unexpected SQL statement: {normalized[:100]}")
            continue
        privileges = [p.strip().upper() for p in match.group(1).split(",")]
        name = match.group(2)
        if name in seen:
            errors.append(f"Duplicate grant: {name}")
        if len(privileges) != len(set(privileges)):
            errors.append(f"Duplicate privilege: {name}")
        seen[name] = set(privileges)
    if seen != EXPECTED:
        errors.append(f"Privilege matrix differs: {seen!r}")
    if statements[:1] != ["BEGIN"] or statements[-1:] != ["COMMIT"]:
        errors.append("Missing transaction boundaries")
    if [re.sub(r"\s+", " ", s).strip() for s in statements].count(GRANT_SCHEMA) != 1:
        errors.append("Missing or duplicate schema usage grant")
    for required_statement in required:
        if statements.count(required_statement) != 1:
            errors.append(f"Missing or duplicate required grant: {required_statement}")
    return errors


class TestDataApiGrants(unittest.TestCase):
    def test_reference_grants_are_safe(self):
        self.assertEqual(validate(SQL.read_text(encoding="utf-8")), [])

    def test_preserve_column_update_boundary(self):
        sql = SQL.read_text(encoding="utf-8")
        self.assertTrue(validate(sql.replace(
            "UPDATE (grade, graded_by, graded_at)", "UPDATE")))
        self.assertTrue(validate(sql.replace(
            "UPDATE (grade, graded_by, graded_at)",
            "UPDATE (grade, graded_by, graded_at, student_id)")))

    def test_queue_requires_explicit_server_access(self):
        sql = SQL.read_text(encoding="utf-8")
        grant = "GRANT SELECT, INSERT ON TABLE public.wirr_public_intake TO service_role;"
        self.assertTrue(validate(sql.replace(grant, "")))
        self.assertTrue(validate(sql.replace(grant, grant.replace("service_role", "anon"))))

    def test_reject_anon_grants(self):
        sql = SQL.read_text(encoding="utf-8").replace(
            "TO authenticated;", "TO anon;", 1
        )
        self.assertTrue(validate(sql))

    def test_reject_unauthorized_write_expansion(self):
        sql = SQL.read_text(encoding="utf-8").replace(
            "GRANT SELECT ON TABLE public.courses",
            "GRANT SELECT, INSERT ON TABLE public.courses",
        )
        self.assertTrue(validate(sql))

    def test_reject_extra_statements_and_objects(self):
        sql = SQL.read_text(encoding="utf-8").replace(
            "COMMIT;", "GRANT SELECT ON TABLE public.students TO authenticated;\nCOMMIT;"
        )
        self.assertTrue(validate(sql))
        self.assertTrue(validate(sql + "\nDROP TABLE public.courses;"))

    def test_reject_duplicate_permissions(self):
        sql = SQL.read_text(encoding="utf-8").replace(
            "GRANT SELECT ON TABLE public.courses TO authenticated;",
            "GRANT SELECT, SELECT ON TABLE public.courses TO authenticated;",
        )
        self.assertTrue(validate(sql))


if __name__ == "__main__":
    unittest.main()
