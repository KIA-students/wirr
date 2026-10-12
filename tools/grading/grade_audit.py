#!/usr/bin/env python3
"""WiRR: audyt kompletności ocen. Python 3.10+, wyłącznie biblioteka standardowa."""
import argparse
import csv
import json
from pathlib import Path

LABS = tuple(f"lab{x:02d}" for x in range(1, 9))
GRADES = {"3.0", "3.5", "4.0", "4.5", "5.0"}
STATES = {"missing", "received", "needs_review", "approved"}

def read_csv(path):
    with open(path, newline="", encoding="utf-8-sig") as source:
        return list(csv.DictReader(source))

def audit(roster, records):
    students = {}
    for row in roster:
        sid, group = row.get("student_id", "").strip(), row.get("group", "").strip()
        if not sid or not group or (sid in students):
            raise ValueError("Lista studentów zawiera pusty lub powtórzony identyfikator albo brak grupy")
        students[sid] = group
    seen = {}
    for row in records:
        sid = row.get("student_id", "").strip()
        lab = row.get("lab", "").strip().lower()
        state = row.get("status", "").strip().lower()
        grade = row.get("grade", "").strip()
        if sid not in students or lab not in LABS or state not in STATES:
            raise ValueError(f"Niepoprawny rekord: {row!r}")
        if state == "approved" and grade not in GRADES:
            raise ValueError(f"Zatwierdzona ocena poza skalą: {row!r}")
        if state != "approved" and grade:
            raise ValueError("Nie można eksportować niezatwierdzonej oceny")
        key = (sid, lab)
        if key in seen:
            raise ValueError(f"Powtórzony rekord student/laboratorium: {key}")
        seen[key] = (state, grade)
    result = []
    for sid, group in sorted(students.items()):
        output = {"student_id": sid, "group": group}
        for lab in LABS:
            state, grade = seen.get((sid, lab), ("missing", ""))
            output[lab + "_status"] = state
            output[lab + "_grade"] = grade
        output["missing_count"] = sum(output[x + "_status"] == "missing" for x in LABS)
        output["pending_count"] = sum(output[x + "_status"] in ("received", "needs_review") for x in LABS)
        output["approved_count"] = sum(output[x + "_status"] == "approved" for x in LABS)
        result.append(output)
    return result

def main():
    parser = argparse.ArgumentParser(description="Zestawienie braków i zatwierdzonych ocen WiRR")
    parser.add_argument("--roster", required=True, type=Path, help="Prywatny roster CSV")
    parser.add_argument("--records", required=True, type=Path, help="Prywatna ewidencja CSV")
    parser.add_argument("--output", required=True, type=Path, help="Docelowy CSV")
    args = parser.parse_args()
    result = audit(read_csv(args.roster), read_csv(args.records))
    args.output.parent.mkdir(parents=True, exist_ok=True)
    fields = ["student_id", "group"] + [f"{lab}_{name}" for lab in LABS for name in ("status", "grade")] + ["missing_count", "pending_count", "approved_count"]
    with args.output.open("w", newline="", encoding="utf-8-sig") as output:
        writer = csv.DictWriter(output, fieldnames=fields)
        writer.writeheader()
        writer.writerows(result)
    print(json.dumps({"students": len(result), "without_full_approval": sum(row["approved_count"] != 8 for row in result), "missing_submissions": sum(row["missing_count"] for row in result)}, ensure_ascii=False))

if __name__ == "__main__":
    main()
