"""Tests of the acceptance gate only. They do not execute Unity or C# tests."""
import contextlib
import importlib.util
import io
import json
from pathlib import Path
import subprocess
import tempfile
import unittest
from unittest.mock import patch
import xml.etree.ElementTree as ET

SPEC = importlib.util.spec_from_file_location('editmode_runner', Path(__file__).with_name('run_wirr_editmode.py'))
runner = importlib.util.module_from_spec(SPEC)
SPEC.loader.exec_module(runner)


def results():
    root = ET.Element('test-run', result='Passed')
    for method, count in runner.EXPECTED.items():
        for i in range(count):
            name = method + (f'("case {i}")' if count > 1 else '')
            ET.SubElement(root, 'test-case', fullname=name, result='Passed')
    return root


class TestWiRREditModeRunner(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.addCleanup(self.temp.cleanup)
        self.root = Path(self.temp.name)
        self.xml = self.root / 'results.xml'

    def save(self, root):
        ET.ElementTree(root).write(self.xml, encoding='utf-8')
        return runner.validate_results(self.xml)

    def test_complete_passed_receipt_and_snapshot_cases_are_accepted(self):
        self.assertEqual(self.save(results())['required'], 15)

    def test_empty_result_is_not_success(self):
        with self.assertRaises(ValueError):
            self.save(ET.Element('test-run', result='Passed'))

    def test_fifteen_unrelated_or_incomplete_cases_are_not_success(self):
        root = results()
        root[-1].set('fullname', 'OtherFixture.UnrelatedTest')
        with self.assertRaises(ValueError):
            self.save(root)

    def test_failed_skipped_inconclusive_and_duplicate_cases_are_rejected(self):
        for state in ('Failed', 'Skipped', 'Inconclusive', 'duplicate'):
            with self.subTest(state=state), self.assertRaises(ValueError):
                root = results()
                if state == 'duplicate':
                    root[-1].set('fullname', root[0].get('fullname'))
                else:
                    root[-1].set('result', state)
                self.save(root)

    def test_missing_or_malformed_xml_does_not_pass(self):
        with self.assertRaises(OSError):
            runner.validate_results(self.xml)
        self.xml.write_text('<test-run', encoding='utf-8')
        with self.assertRaises(ET.ParseError):
            runner.validate_results(self.xml)

    def test_project_manifest_must_resolve_to_package_not_project(self):
        packages = self.root / 'Project~' / 'Packages'
        packages.mkdir(parents=True)
        settings = self.root / 'Project~' / 'ProjectSettings'
        settings.mkdir()
        (settings / 'ProjectVersion.txt').write_text('m_EditorVersion: 6000.6.5f1\n')
        (self.root / 'package.json').write_text(json.dumps({'name': runner.PACKAGE}))
        manifest = {'dependencies': {runner.PACKAGE: 'file:..', 'com.unity.test-framework': '1.8.0'},
                    'testables': [runner.PACKAGE]}
        path = packages / 'manifest.json'
        path.write_text(json.dumps(manifest))
        with self.assertRaises(ValueError):
            runner.preflight(self.root)
        manifest['dependencies'][runner.PACKAGE] = 'file:../..'
        path.write_text(json.dumps(manifest))
        self.assertEqual(runner.preflight(self.root)[1], '6000.6.5f1')

    def test_exit_zero_without_results_and_nonzero_with_results_are_failures(self):
        for exit_code, write_xml in ((0, False), (1, True), (0, True)):
            with self.subTest(exit_code=exit_code, write_xml=write_xml):
                def run(command, **kwargs):
                    self.assertNotIn('-quit', command)
                    if write_xml:
                        destination = Path(command[command.index('-testResults') + 1])
                        ET.ElementTree(results()).write(destination, encoding='utf-8')
                    return subprocess.CompletedProcess(command, exit_code)
                with patch.object(runner, 'preflight', return_value=(self.root, '6000.6.5f1')), \
                        patch.object(runner, 'locate_unity', return_value=self.root / 'Unity'), \
                        patch.object(runner, 'git_metadata', return_value={}), \
                        patch.object(runner.subprocess, 'run', side_effect=run), \
                        contextlib.redirect_stdout(io.StringIO()), contextlib.redirect_stderr(io.StringIO()):
                    self.assertEqual(runner.main([]), 0 if exit_code == 0 and write_xml else 1)
                summaries = list((self.root / 'Logs').glob('*/summary.json'))
                self.assertEqual(len(summaries), 1)
                summary = json.loads(summaries[0].read_text(encoding='utf-8'))
                self.assertEqual(summary['passed'], exit_code == 0 and write_xml)
                import shutil
                shutil.rmtree(self.root / 'Logs')

    def test_timeout_saves_failure_summary(self):
        with patch.object(runner, 'preflight', return_value=(self.root, '6000.6.5f1')), \
                patch.object(runner, 'locate_unity', return_value=self.root / 'Unity'), \
                patch.object(runner, 'git_metadata', return_value={}), \
                patch.object(runner.subprocess, 'run', side_effect=subprocess.TimeoutExpired('Unity', 900)), \
                contextlib.redirect_stdout(io.StringIO()), contextlib.redirect_stderr(io.StringIO()):
            self.assertEqual(runner.main([]), 1)
        summary = json.loads(next((self.root / 'Logs').glob('*/summary.json')).read_text(encoding='utf-8'))
        self.assertFalse(summary['passed'])
        self.assertIn('error', summary)


if __name__ == '__main__':
    unittest.main()
