#!/usr/bin/env python3
"""Bounded, offline load test of the real WiRR exporter with in-memory service doubles."""
from __future__ import annotations
import argparse
import contextlib
import importlib.util
import io
import json
import sys
import time
import tracemalloc
from pathlib import Path
from unittest.mock import patch

ROOT = Path(__file__).resolve().parents[2]
if not (ROOT / "scripts~" / "test_wirr_report_exporter.py").is_file():
    import os
    ROOT = Path(os.environ.get("WIRR_REPO", str(Path.home() / "repo/wirr-matpom"))).resolve()
if not (ROOT / "scripts~" / "test_wirr_report_exporter.py").is_file():
    raise SystemExit(f"WiRR test module not found in {ROOT}")
TEST = ROOT / "scripts~" / "test_wirr_report_exporter.py"
spec = importlib.util.spec_from_file_location("wirr_exporter_tests", TEST)
module = importlib.util.module_from_spec(spec)
sys.modules[spec.name] = module
spec.loader.exec_module(module)

def run(batch):
    with patch.object(module.exporter, "request", batch.request), \
         patch.object(module.exporter, "SUPABASE_KEY", "offline-test"), \
         patch.object(module.exporter, "GITHUB_KEY", "offline-test"), \
         contextlib.redirect_stdout(io.StringIO()), \
         contextlib.redirect_stderr(io.StringIO()):
        return module.exporter.main()

def verify(label, count, faults):
    batch = module.BatchServices(count)
    if faults:
        for i, item in enumerate(batch.items):
            if i % 7 == 0:
                item.fail_ack = True
            elif i % 11 == 0:
                item.fail_pr = True
    tracemalloc.start()
    start = time.perf_counter()
    first = run(batch)
    after_first = time.perf_counter()
    second = run(batch)
    elapsed = time.perf_counter() - start
    _, peak = tracemalloc.get_traced_memory()
    tracemalloc.stop()
    confirmed = sum(item.receipt is not None for item in batch.items)
    writes = batch.count("PUT", "/contents/")
    prs = batch.count("POST", "/pulls")
    # First scan should fail if faults were injected, second must recover.
    assert first == (1 if faults else 0), (label, "first status", first)
    assert second == 0, (label, "retry status", second)
    assert confirmed == count, (label, "confirmed", confirmed)
    assert writes == count, (label, "duplicate file writes", writes)
    successful_prs = sum(item.pr is not None for item in batch.items)
    expected_failed_attempts = sum(1 for i in range(count) if faults and i % 7 != 0 and i % 11 == 0)
    assert successful_prs == count, (label, "missing created PRs", successful_prs)
    assert prs == count + expected_failed_attempts, (label, "unexpected POST attempts", prs)
    assert all(item.count("POST", "/pulls") <= (2 if faults else 1) for item in batch.items), (label, "extra PR retries")
    assert all(item.row["status"] == "reviewed" for item in batch.items)
    return dict(scenario=label, reports=count, faults=faults,
                first_exit=first, second_exit=second, confirmed=confirmed,
                file_writes=writes, pr_post_attempts=prs, created_prs=successful_prs,
                first_pass_seconds=round(after_first-start, 4),
                total_seconds=round(elapsed, 4),
                peak_tracemalloc_bytes=peak)

def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--reports", type=int, default=60)
    parser.add_argument("--output", type=Path)
    args = parser.parse_args()
    if not 2 <= args.reports <= 150:
        parser.error("--reports must be between 2 and 150 to limit homeserver load")
    results = [
        verify("normal", args.reports, False),
        verify("intermittent_ack_and_pr_failures", args.reports, True),
    ]
    document = {"scope": "OFFLINE mocked Supabase and GitHub; real exporter control flow",
                "results": results, "outcome": "PASS"}
    print(json.dumps(document, indent=2))
    if args.output:
        args.output.parent.mkdir(parents=True, exist_ok=True)
        args.output.write_text(json.dumps(document, indent=2)+"\n", encoding="utf-8")

if __name__ == "__main__":
    main()
