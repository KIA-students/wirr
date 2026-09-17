#!/usr/bin/env python3
"""Basic integrity check for WiRR report JSON files."""
from __future__ import annotations

import argparse
import json
import re
import sys
from pathlib import Path

SCHEMA = "wirr-report/1.0"
CHECKPOINTS = [("3.0", "cp30."), ("3.5", "cp35."), ("4.0", "cp40."), ("4.5", "cp45."), ("5.0", "cp50.")]


def nonempty(value: object) -> bool:
    return value is not None and str(value).strip() != ""


def validate_identity(report: dict) -> list[str]:
    issues = []
    if report.get("schemaVersion") != SCHEMA: issues.append("Nieobsługiwana wersja raportu.")
    if not isinstance(report.get("labNumber"), int) or not 1 <= report["labNumber"] <= 7: issues.append("Nieprawidłowy numer laboratorium.")
    if not nonempty(report.get("submissionId")): issues.append("Brak identyfikatora raportu.")
    if not nonempty(report.get("teamId")): issues.append("Brak identyfikatora zespołu.")
    indices = [str(v).strip() for v in (report.get("studentIndices") or []) if nonempty(v)]
    if len(indices) not in (2, 3): issues.append("Raport musi zawierać 2 albo 3 numery indeksów.")
    if any(not re.fullmatch(r"\d+", value) for value in indices): issues.append("Numery indeksów mogą zawierać tylko cyfry.")
    if len(set(indices)) != len(indices): issues.append("Numery indeksów nie mogą się powtarzać.")
    return issues


def evaluate(path: Path) -> dict:
    report = json.loads(path.read_text(encoding="utf-8"))
    blocking = validate_identity(report)
    answers = {str(x.get("key")): str(x.get("value") or "").strip() for x in report.get("answers") or [] if isinstance(x, dict) and nonempty(x.get("key"))}
    checkpoints = []
    for grade, prefix in CHECKPOINTS:
        count = sum(1 for key, value in answers.items() if key.startswith(prefix) and nonempty(value))
        checkpoints.append({"checkpoint": grade, "filledFieldCount": count})
    return {
        "graderSchema": "wirr-grade/1.0",
        "status": "VALID" if not blocking else "ERROR",
        "requiresInstructorApproval": True,
        "sourceReport": str(path),
        "submissionId": report.get("submissionId"),
        "teamId": report.get("teamId"),
        "labNumber": report.get("labNumber"),
        "blockingIssues": blocking,
        "checkpoints": checkpoints,
        "note": "CI sprawdza integralność raportu. Ocena merytoryczna należy do prowadzącego."
    }


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("report", type=Path)
    parser.add_argument("--output", type=Path, default=Path("grading-result.json"))
    args = parser.parse_args()
    try:
        result = evaluate(args.report)
    except (OSError, json.JSONDecodeError, TypeError, ValueError) as exc:
        print(f"Błąd odczytu raportu: {exc}", file=sys.stderr)
        return 2
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(json.dumps(result, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    print(json.dumps(result, ensure_ascii=False, indent=2))
    return 1 if result["blockingIssues"] else 0


if __name__ == "__main__":
    raise SystemExit(main())
