"""Fault-injection tests with a local queue/GitHub double; no network or student data."""
import base64
import contextlib
import copy
import hashlib
import importlib.util
import io
import json
from pathlib import Path
import unittest
from unittest.mock import patch
from urllib.error import HTTPError
from urllib.parse import parse_qs, urlsplit, unquote
import uuid

SOURCE = Path(__file__).resolve().parents[1] / 'scripts' / 'export_wirr_reports.py'
SPEC = importlib.util.spec_from_file_location('wirr_exporter', SOURCE)
exporter = importlib.util.module_from_spec(SPEC)
SPEC.loader.exec_module(exporter)
ID = '4ac6f686-bbb1-42f3-9668-a678d9187a52'


def queue_row():
    return {'submission_id': ID, 'group_code': 'G1', 'lab_no': 1,
            'report': {'labNumber': 1, 'observation': 'Syntetyczna obserwacja: ąęł'},
            'status': 'pending', 'github_path': f'student-work/G1/lab01-{ID}.json'}


class Services:
    """Model durable side effects and an acknowledgement lost after PR creation."""
    def __init__(self):
        self.row = queue_row()
        self.calls = []
        self.branch = False
        self.branch_sha = 'base-commit'
        self.blob = None
        self.pr = None
        self.receipt = None
        self.fail_ack = False
        self.fail_pr = False
        self.lose_pr_response = False
        self.wrong_pr_head = False
        self.missing_commit = False
        self.merged_blob = None
        self.extra_pr = None
        self.close_on_file_read = False

    def request(self, method, url, headers, data=None):
        self.calls.append((method, url, copy.deepcopy(data)))
        if '/rest/v1/' in url:
            if method == 'GET':
                return [copy.deepcopy(self.row)] if self.row['status'] == 'pending' else []
            if method == 'PATCH':
                if self.fail_ack:
                    self.fail_ack = False
                    raise HTTPError(url, 503, 'acknowledgement unavailable', {}, None)
                self.receipt = copy.deepcopy(data)
                self.row.update(data)
                return {}
        if url.endswith('/git/ref/heads/main'):
            return {'object': {'sha': 'base-commit'}}
        if url.endswith('/git/refs') and method == 'POST':
            if self.branch:
                raise HTTPError(url, 422, 'reference exists', {}, None)
            self.branch = True
            return {}
        if '/git/ref/heads/wirr-report/' in url:
            if not self.branch:
                raise HTTPError(url, 404, 'branch deleted', {}, None)
            return {'object': {'sha': self.branch_sha}}
        if '/contents/' in url:
            if method == 'GET':
                blob = self.merged_blob if parse_qs(urlsplit(url).query).get('ref') == ['merge-commit'] else self.blob
                if blob is None:
                    raise HTTPError(url, 404, 'not found', {}, None)
                if self.close_on_file_read:
                    self.pr['state'] = 'closed'
                # Use git itself as the independent blob-identity oracle in the test.
                import subprocess
                sha = subprocess.run(['git', 'hash-object', '--stdin'], input=blob,
                                     capture_output=True, check=True).stdout.decode().strip()
                return {'sha': sha}
            if method == 'PUT':
                self.blob = base64.b64decode(data['content'])
                self.branch_sha = 'report-commit'
                return {} if self.missing_commit else {'commit': {'sha': self.branch_sha}}
        if '/pulls' in url:
            if method == 'GET':
                state = parse_qs(urlsplit(url).query).get('state', ['open'])[0]
                return [copy.deepcopy(pr) for pr in (self.pr, self.extra_pr)
                        if pr and (state == 'all' or pr['state'] == state)]
            if method == 'POST':
                if self.fail_pr:
                    self.fail_pr = False
                    raise HTTPError(url, 503, 'PR temporarily unavailable', {}, None)
                self.pr = {'html_url': 'https://github.com/KIA-students/wirr/pull/999999',
                           'state': 'open', 'merged_at': None, 'merge_commit_sha': None,
                           'head': {'sha': 'unrelated' if self.wrong_pr_head else self.branch_sha}}
                if self.lose_pr_response:
                    self.lose_pr_response = False
                    raise HTTPError(url, 503, 'PR response lost after creation', {}, None)
                return copy.deepcopy(self.pr)
        raise AssertionError(f'Unexpected request: {method} {url}')

    def count(self, method, path):
        return sum(m == method and path in url for m, url, _ in self.calls)


class TestWiRRReportExporter(unittest.TestCase):
    def run_export(self, services):
        with patch.object(exporter, 'request', services.request), \
                patch.object(exporter, 'SUPABASE_KEY', 'synthetic'), \
                patch.object(exporter, 'GITHUB_KEY', 'synthetic'), \
                contextlib.redirect_stdout(io.StringIO()), \
                contextlib.redirect_stderr(io.StringIO()):
            return exporter.main()

    def test_success_records_exact_commit_after_pr_confirmation(self):
        services = Services()
        self.assertEqual(self.run_export(services), 0)
        self.assertEqual(services.receipt['github_commit_sha'], services.pr['head']['sha'])
        self.assertEqual(services.receipt['status'], 'reviewed')
        self.assertEqual(json.loads(services.blob)['observation'], 'Syntetyczna obserwacja: ąęł')
        self.assertEqual(services.count('PUT', '/contents/'), 1)

    def test_lost_ack_retry_reuses_file_commit_and_pr(self):
        services = Services()
        services.fail_ack = True
        self.assertEqual(self.run_export(services), 1)
        self.assertEqual(services.row['status'], 'pending')
        self.assertIsNone(services.receipt)
        self.assertEqual(self.run_export(services), 0)
        self.assertEqual(services.count('PUT', '/contents/'), 1)
        self.assertEqual(services.count('POST', '/pulls'), 1)
        self.assertEqual(services.receipt['github_commit_sha'], 'report-commit')

    def test_retry_after_pr_failure_reuses_existing_file(self):
        services = Services()
        services.fail_pr = True
        self.assertEqual(self.run_export(services), 1)
        self.assertEqual(self.run_export(services), 0)
        self.assertEqual(services.count('PUT', '/contents/'), 1)
        self.assertEqual(services.receipt['github_commit_sha'], 'report-commit')

    def test_changed_payload_cannot_overwrite_existing_submission(self):
        services = Services()
        services.fail_ack = True
        self.assertEqual(self.run_export(services), 1)
        original = services.blob
        services.row['report']['observation'] = 'Zmieniona wersja'
        self.assertEqual(self.run_export(services), 1)
        self.assertEqual(services.blob, original)
        self.assertEqual(services.row['status'], 'pending')
        self.assertEqual(services.count('PUT', '/contents/'), 1)

    def test_lost_pr_response_retry_finds_the_created_pr(self):
        services = Services()
        services.lose_pr_response = True
        self.assertEqual(self.run_export(services), 1)
        self.assertIsNotNone(services.pr)
        self.assertEqual(self.run_export(services), 0)
        self.assertEqual(services.count('PUT', '/contents/'), 1)
        self.assertEqual(services.count('POST', '/pulls'), 1)

    def test_unrelated_pr_head_is_not_delivery_confirmation(self):
        services = Services()
        services.wrong_pr_head = True
        self.assertEqual(self.run_export(services), 1)
        self.assertEqual(services.count('PATCH', '/rest/v1/'), 0)
        self.assertEqual(services.row['status'], 'pending')

    def test_missing_commit_cannot_acknowledge_queue(self):
        services = Services()
        services.missing_commit = True
        self.assertEqual(self.run_export(services), 1)
        self.assertEqual(services.count('POST', '/pulls'), 0)
        self.assertEqual(services.count('PATCH', '/rest/v1/'), 0)

    def test_invalid_queue_rows_fail_before_github(self):
        for changes in ({'lab_no': True}, {'lab_no': 9}, {'group_code': 'G3'},
                        {'github_path': 'other/report.json'}, {'submission_id': '../other'},
                        {'report': []}, {'report': {'labNumber': 2}},
                        {'report': {'labNumber': True}}):
            with self.subTest(changes=changes):
                services = Services()
                services.row.update(changes)
                self.assertEqual(self.run_export(services), 1)
                self.assertEqual(sum(exporter.API in url for _, url, _ in services.calls), 0)

    def test_second_scan_after_success_does_no_github_work(self):
        services = Services()
        self.assertEqual(self.run_export(services), 0)
        calls = services.count('PUT', '/contents/')
        self.assertEqual(self.run_export(services), 0)
        self.assertEqual(services.count('PUT', '/contents/'), calls)

    def test_git_blob_identity_matches_known_git_object(self):
        self.assertEqual(exporter.report_blob_sha(b'hello\n'), 'ce013625030ba8dba906f756967f9e9ca394464a')
        self.assertNotEqual(exporter.report_blob_sha(b'hello\n'), hashlib.sha1(b'hello\n').hexdigest())

    def pending_after_lost_ack(self):
        services = Services()
        services.fail_ack = True
        self.assertEqual(self.run_export(services), 1)
        services.calls.clear()
        return services

    def assert_no_github_writes(self, services):
        self.assertEqual([(method, url) for method, url, _ in services.calls
                          if exporter.API in url and method != 'GET'], [])

    def test_closed_unmerged_pr_is_not_recreated_or_acknowledged(self):
        services = self.pending_after_lost_ack()
        services.pr['state'] = 'closed'
        for deleted in (False, True):
            with self.subTest(branch_deleted=deleted):
                services.branch = not deleted
                self.assertEqual(self.run_export(services), 1)
                self.assert_no_github_writes(services)
                self.assertIsNone(services.receipt)

    def test_merged_pr_recovers_after_branch_deletion_without_github_writes(self):
        services = self.pending_after_lost_ack()
        services.pr.update(state='closed', merged_at='2026-10-16T10:00:00Z',
                           merge_commit_sha='merge-commit')
        services.merged_blob = services.blob
        services.blob = None
        services.branch = False
        self.assertEqual(self.run_export(services), 0)
        self.assert_no_github_writes(services)
        self.assertEqual(services.receipt['github_commit_sha'], 'merge-commit')
        self.assertEqual(services.receipt['github_pr_url'], services.pr['html_url'])

    def test_merged_pr_with_missing_or_changed_content_stays_pending(self):
        for blob in (None, b'{"changed": true}'):
            with self.subTest(blob=blob):
                services = self.pending_after_lost_ack()
                services.pr.update(state='closed', merged_at='2026-10-16T10:00:00Z',
                                   merge_commit_sha='merge-commit')
                services.merged_blob = blob
                self.assertEqual(self.run_export(services), 1)
                self.assert_no_github_writes(services)
                self.assertIsNone(services.receipt)

    def test_merged_pr_without_commit_is_not_acknowledged(self):
        services = self.pending_after_lost_ack()
        services.pr.update(state='closed', merged_at='2026-10-16T10:00:00Z')
        self.assertEqual(self.run_export(services), 1)
        self.assert_no_github_writes(services)
        self.assertIsNone(services.receipt)

    def test_multiple_historical_prs_require_operator_reconciliation(self):
        services = self.pending_after_lost_ack()
        services.extra_pr = copy.deepcopy(services.pr)
        services.extra_pr.update(state='closed', html_url='https://github.com/KIA-students/wirr/pull/999998')
        self.assertEqual(self.run_export(services), 1)
        self.assert_no_github_writes(services)
        self.assertIsNone(services.receipt)

    def test_pr_closed_during_retry_is_not_acknowledged(self):
        services = self.pending_after_lost_ack()
        services.close_on_file_read = True
        self.assertEqual(self.run_export(services), 1)
        self.assertIsNone(services.receipt)
        self.assertEqual(services.count('POST', '/pulls'), 0)

    def test_merged_recovery_can_retry_lost_queue_acknowledgement(self):
        services = self.pending_after_lost_ack()
        services.pr.update(state='closed', merged_at='2026-10-16T10:00:00Z',
                           merge_commit_sha='merge-commit')
        services.merged_blob = services.blob
        services.fail_ack = True
        self.assertEqual(self.run_export(services), 1)
        self.assertEqual(self.run_export(services), 0)
        self.assert_no_github_writes(services)
        self.assertEqual(services.receipt['github_commit_sha'], 'merge-commit')


class BatchServices:
    """Independent durable submissions with queue filtering applied on every fetch."""
    def __init__(self, count):
        self.items = []
        self.queue_calls = []
        self.active = None
        self.fail_fetch = False
        self.ignore_cursor = False
        for n in range(1, count + 1):
            item = Services()
            identifier = str(uuid.UUID(int=n))
            item.row.update(submission_id=identifier,
                            github_path=f'student-work/G1/lab01-{identifier}.json')
            self.items.append(item)

    def request(self, method, url, headers, data=None):
        query = parse_qs(urlsplit(url).query)
        if '/rest/v1/' in url and method == 'GET':
            self.queue_calls.append(query)
            if self.fail_fetch:
                raise HTTPError(url, 503, 'queue unavailable', {}, None)
            rows = [copy.deepcopy(item.row) for item in self.items if item.row['status'] == 'pending']
            rows.sort(key=lambda row: uuid.UUID(row['submission_id']).int)
            if 'submission_id' in query and not self.ignore_cursor:
                cursor = uuid.UUID(query['submission_id'][0].removeprefix('gt.')).int
                rows = [row for row in rows if uuid.UUID(row['submission_id']).int > cursor]
            return rows[:int(query.get('limit', ['30'])[0])]
        decoded_url = unquote(url)
        for item in self.items:
            if item.row['submission_id'] in decoded_url:
                self.active = item
                break
        if self.active is None:
            raise AssertionError(f'No submission selected: {method} {url}')
        return self.active.request(method, url, headers, data)

    def count(self, method, path):
        return sum(item.count(method, path) for item in self.items)


class TestWiRRBatchExporter(unittest.TestCase):
    run_export = TestWiRRReportExporter.run_export

    def test_invalid_first_report_does_not_block_the_next_student(self):
        for changes in ({'group_code': 'G3'}, {'report': None}, {'report': {'labNumber': 2}}):
            with self.subTest(changes=changes):
                batch = BatchServices(2)
                batch.items[0].row.update(changes)
                self.assertEqual(self.run_export(batch), 1)
                self.assertEqual(batch.items[0].row['status'], 'pending')
                self.assertIsNone(batch.items[0].receipt)
                self.assertEqual(batch.items[1].row['status'], 'reviewed')
                self.assertEqual(batch.items[0].count('POST', '/git/refs'), 0)

    def test_missing_field_does_not_abort_remaining_reports(self):
        batch = BatchServices(2)
        del batch.items[0].row['report']
        self.assertEqual(self.run_export(batch), 1)
        self.assertIsNone(batch.items[0].receipt)
        self.assertIsNotNone(batch.items[1].receipt)

    def test_failed_pr_does_not_block_next_report_and_retry_is_idempotent(self):
        batch = BatchServices(2)
        batch.items[0].fail_pr = True
        self.assertEqual(self.run_export(batch), 1)
        self.assertIsNone(batch.items[0].receipt)
        self.assertIsNotNone(batch.items[1].receipt)
        self.assertEqual(self.run_export(batch), 0)
        self.assertEqual(batch.count('PUT', '/contents/'), 2)
        self.assertEqual(batch.items[1].count('POST', '/pulls'), 1)

    def test_lost_ack_does_not_block_other_report_or_duplicate_retry(self):
        batch = BatchServices(2)
        batch.items[0].fail_ack = True
        self.assertEqual(self.run_export(batch), 1)
        self.assertIsNotNone(batch.items[1].receipt)
        self.assertEqual(self.run_export(batch), 0)
        self.assertEqual(batch.count('PUT', '/contents/'), 2)
        self.assertEqual(batch.count('POST', '/pulls'), 2)

    def test_full_page_of_invalid_reports_does_not_starve_later_student(self):
        batch = BatchServices(31)
        for item in batch.items[:30]:
            item.row['group_code'] = 'G3'
        self.assertEqual(self.run_export(batch), 1)
        self.assertTrue(all(item.receipt is None for item in batch.items[:30]))
        self.assertIsNotNone(batch.items[30].receipt)
        self.assertEqual(batch.queue_calls[1]['submission_id'],
                         ['gt.' + batch.items[29].row['submission_id']])

    def test_acknowledgements_do_not_shift_pages_and_skip_students(self):
        batch = BatchServices(31)
        self.assertEqual(self.run_export(batch), 0)
        self.assertTrue(all(item.receipt is not None for item in batch.items))
        self.assertEqual(batch.count('PUT', '/contents/'), 31)
        self.assertEqual(len(batch.queue_calls), 2)

    def test_global_auth_or_rate_failure_stops_further_github_requests(self):
        for code in (401, 403, 429):
            with self.subTest(code=code):
                batch = BatchServices(2)
                original = batch.request
                github_attempts = []
                def request(method, url, headers, data=None):
                    if exporter.API in url:
                        github_attempts.append(url)
                        raise HTTPError(url, code, 'global access failure', {}, None)
                    return original(method, url, headers, data)
                with patch.object(batch, 'request', request):
                    self.assertEqual(self.run_export(batch), 1)
                self.assertTrue(all(item.receipt is None for item in batch.items))
                self.assertEqual(batch.count('POST', '/git/refs'), 0)
                self.assertEqual(len(github_attempts), 1)

    def test_unavailable_queue_returns_failure_without_github_writes(self):
        batch = BatchServices(2)
        batch.fail_fetch = True
        self.assertEqual(self.run_export(batch), 1)
        self.assertEqual(batch.count('POST', '/git/refs'), 0)

    def test_nonadvancing_queue_cursor_stops_instead_of_looping(self):
        batch = BatchServices(2)
        batch.ignore_cursor = True
        for item in batch.items:
            item.row['group_code'] = 'G3'
        with patch.object(exporter, 'PAGE_SIZE', 2, create=True):
            self.assertEqual(self.run_export(batch), 1)
        self.assertEqual(len(batch.queue_calls), 2)


if __name__ == '__main__':
    unittest.main()
