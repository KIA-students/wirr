#!/usr/bin/env python3
"""Deterministically aggregate instructor-approved individual and league tasks.

No raw student indices. Inputs are explicit approvals, never student-submitted
progress snapshots. The pre-existing two-group scoreboard remains authoritative.
"""
import argparse
import json
from collections import defaultdict
from pathlib import Path

ALLOWED_GROUPS = {"G1", "G2"}
ALLOWED_TASKS = {f"lab{n:02d}" for n in range(1, 9)}

def aggregate(records):
    by_student = defaultdict(dict)
    group_members = {}
    for item in records:
        sid = item["studentId"]
        group = item["group"]
        task = item["task"]
        grade = item["approvedPoints"]
        if not isinstance(sid, str) or not (8 <= len(sid) <= 64) or not all(c.isalnum() or c == "-" for c in sid):
            raise ValueError("Student ID must be opaque and safe for publication")
        if group not in ALLOWED_GROUPS or task not in ALLOWED_TASKS:
            raise ValueError("Unexpected group/task")
        if type(grade) is not int or not (0 <= grade <= 10):
            raise ValueError("Approved points must be an integer in 0..10")
        if item.get("status") != "approved":
            raise ValueError("Only instructor-approved entries may be aggregated")
        if sid in group_members and group_members[sid] != group:
            raise ValueError("One student cannot belong to two groups")
        group_members[sid] = group
        # Idempotent: replaying the same approved task doesn't add points twice.
        old = by_student[sid].get(task)
        if old is not None and old != grade:
            raise ValueError("Conflicting approval: requires an explicit correction process")
        by_student[sid][task] = grade
    students = [
        {"studentId": sid, "group": group_members[sid],
         "tasks": dict(sorted(tasks.items())),
         "total": sum(tasks.values()), "approvedTaskCount": len(tasks)}
        for sid, tasks in sorted(by_student.items())
    ]
    groups = {}
    for group in sorted(ALLOWED_GROUPS):
        members = [s for s in students if s["group"] == group]
        groups[group] = {"members": len(members),
                         "approvedTaskCount": sum(s["approvedTaskCount"] for s in members),
                         "individualPointsSum": sum(s["total"] for s in members)}
    return {"schema": "wirr-approved-individual-totals/1", "students": students, "groups": groups,
            "notice": "Indywidualne wyniki nie są punktami grupowymi Ligi XR. Liga używa zatwierdzonego scoreboard.json."}

def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--approvals", required=True, type=Path)
    parser.add_argument("--output", required=True, type=Path)
    args = parser.parse_args()
    entries = json.loads(args.approvals.read_text(encoding="utf-8"))
    if not isinstance(entries, list):
        raise ValueError("Expected an array of instructor-approved entries")
    summary = aggregate(entries)
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(json.dumps(summary, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")

if __name__ == "__main__":
    main()
