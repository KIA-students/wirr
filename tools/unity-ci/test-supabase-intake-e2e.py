#!/usr/bin/env python3
"""Minimal real HTTP E2E of WiRR intake v5, with cleanup of one synthetic row.

Requires WIRR_COURSE_KEY and WIRR_SUPABASE_SERVICE_ROLE_KEY in environment.
Never prints credentials; never calls the GitHub report exporter.
"""
import json
import os
import sys
import uuid
import urllib.error
import urllib.request

BASE = "https://ndebunrsdtulflzpemwd.supabase.co"
FUNCTION = BASE + "/functions/v1/wirr-public-intake"
TABLE = BASE + "/rest/v1/wirr_public_intake"
KEY = os.getenv("WIRR_COURSE_KEY", "")
ADMIN = os.getenv("WIRR_SUPABASE_SERVICE_ROLE_KEY", "")

def call(url, method, headers, payload=None):
    raw = None if payload is None else json.dumps(payload, ensure_ascii=False).encode()
    req = urllib.request.Request(url, data=raw, method=method, headers=headers)
    try:
        with urllib.request.urlopen(req, timeout=20) as response:
            return response.status, json.loads(response.read() or b"{}")
    except urllib.error.HTTPError as exc:
        return exc.code, json.loads(exc.read() or b"{}")

def main():
    if not KEY or not ADMIN:
        print("SKIP: set WIRR_COURSE_KEY and WIRR_SUPABASE_SERVICE_ROLE_KEY locally", file=sys.stderr)
        return 2
    sid = str(uuid.uuid4())
    path = "student-work/G1/lab01-" + sid + ".json"
    report = {"labNumber": 1, "group": "G1",
              "observation": "synthetic-wirr-e2e-" + sid}
    payload = {"submission_id": sid, "lab_no": 1, "report": report}
    endpoint_headers = {"x-wirr-course-key": KEY, "Content-Type": "application/json"}
    db_headers = {"apikey": ADMIN, "Authorization": "Bearer " + ADMIN}
    delete_url = TABLE + "?submission_id=eq." + sid
    try:
        for name, request_body, expected in (
            ("first", payload, 202),
            ("same-id-same-payload", payload, 202),
            ("same-id-different-payload", {**payload, "report": {**report, "observation": "altered"}}, 409),
        ):
            status, body = call(FUNCTION, "POST", endpoint_headers, request_body)
            assert status == expected, (name, status, body.get("error"))
            if expected == 202:
                assert body.get("accepted") is True and body.get("github_path") == path, name
            else:
                assert body.get("error") == "submission_conflict", (name, body.get("error"))
            print(name, "PASS", status)
        status, rows = call(TABLE + "?select=submission_id,report,status&submission_id=eq." + sid,
                            "GET", db_headers)
        assert status == 200 and len(rows) == 1, ("db receipt", status)
        assert rows[0]["report"] == report and rows[0]["status"] == "pending", "stored record was changed"
        print("stored-payload-unchanged PASS")
        return 0
    finally:
        status, _ = call(delete_url, "DELETE", {**db_headers, "Prefer": "return=minimal"})
        if status not in (200, 204):
            print("CLEANUP FAILED for synthetic submission ID:", sid, "status:", status, file=sys.stderr)
            raise SystemExit(3)
        print("cleanup PASS")

if __name__ == "__main__":
    raise SystemExit(main())
