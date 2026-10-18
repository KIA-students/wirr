#!/usr/bin/env python3
"""Validate public student progress metadata (not academic grading)."""
import json
import re
import sys
from pathlib import Path

def validate(path):
    data = json.loads(Path(path).read_text(encoding="utf-8"))
    sid = data.get("pseudonymousId", "")
    if not re.fullmatch(r"[a-f0-9]{32}", sid):
        raise ValueError("Invalid opaque student ID")
    if Path(path).stem != sid:
        raise ValueError("Filename and student ID differ")
    if data.get("schema") != "wirr-local-progress/1":
        raise ValueError("Invalid progress schema")
    if data.get("group") not in ("G1", "G2"):
        raise ValueError("Invalid group")
    if data.get("gradingStatus") != "not_assessed":
        raise ValueError("Student cannot assign grade")
    if data.get("submissionStatus") != "local_only":
        raise ValueError("Unrecognized submission state")
    labs = data.get("labs")
    if not isinstance(labs, list) or len(labs) != 8:
        raise ValueError("Expected eight laboratories")
    for i, lab in enumerate(labs, start=1):
        if lab.get("lab") != i or type(lab.get("scenePrepared")) is not bool or type(lab.get("reportExported")) is not bool:
            raise ValueError("Invalid lab progress")
    if set(data) != {"schema", "pseudonymousId", "group", "updatedAtUtc", "gradingStatus", "submissionStatus", "unityVersion", "labs"}:
        raise ValueError("Unexpected metadata fields")
    if len(Path(path).read_bytes()) > 8192:
        raise ValueError("Progress report too large")

if __name__ == "__main__":
    try:
        for arg in sys.argv[1:]:
            validate(arg)
        if len(sys.argv) < 2:
            raise ValueError("No reports")
    except (ValueError, KeyError, OSError, json.JSONDecodeError) as exc:
        print(f"Rejected report: {exc}", file=sys.stderr)
        sys.exit(1)
