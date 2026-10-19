"""Regression checks for the WiRR public-export workflow."""
from __future__ import annotations

import re
import subprocess
import unittest
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
WORKFLOW = ROOT / ".github/workflows/sync-kia-students-pr.yml"
TEST_FILE = ROOT / "tools/wirr2/test_student_materials.py"
PRIVATE_OWNER = "Mat" + "PomGit"


class ExportGuardRegressionTests(unittest.TestCase):
    def test_student_test_does_not_self_trigger_literal_scan(self):
        source = TEST_FILE.read_text(encoding="utf-8")
        self.assertIn('self.assertNotIn("Mat" + "PomGit", json.dumps(data))', source)
        self.assertNotIn(PRIVATE_OWNER, source)

    def test_actual_private_references_remain_blocked(self):
        workflow = WORKFLOW.read_text(encoding="utf-8")
        self.assertIn("\'" + PRIVATE_OWNER + "\'", workflow)
        self.assertIn('if [ -n "${forbidden_matches}" ]; then', workflow)
        fixture = "https://github.com/" + PRIVATE_OWNER + "/wirr"
        self.assertIn(PRIVATE_OWNER, fixture)
        self.assertNotIn(PRIVATE_OWNER, "https://github.com/KIA-students/wirr")

    def test_echo_lines_do_not_execute_markdown_backticks(self):
        workflow = WORKFLOW.read_text(encoding="utf-8")
        quoted_echoes = [line.strip() for line in workflow.splitlines()
                         if re.match(r'^\s*echo\s+"', line)]
        self.assertTrue(quoted_echoes)
        for line in quoted_echoes:
            self.assertIsNone(re.search(r"(?<!\\)`", line), line)
        candidates = [line for line in quoted_echoes if "Project~" in line]
        self.assertEqual(len(candidates), 1)
        completed = subprocess.run(["bash", "-e", "-c", candidates[0]],
                                   capture_output=True, text=True, check=False)
        self.assertEqual(completed.returncode, 0, completed.stderr)
        self.assertIn("Project~", completed.stdout)


if __name__ == "__main__":
    unittest.main()
