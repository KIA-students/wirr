#!/usr/bin/env python3
"""Basic integrity check for WiRR report JSON files."""
from __future__ import annotations

import argparse
import json
import math
import re
import sys
from datetime import datetime, timedelta
from pathlib import Path

SCHEMA = "wirr-report/1.0"
WIRR2_SCHEMA = "wirr-report/2.0-draft.1"
DEVICE_MODES = {"DESKTOP_SIM", "SIM_AR", "REAL_AR", "ROS_REPLAY", "ROS_LIVE"}
CHECKPOINTS = [("3.0", "cp30."), ("3.5", "cp35."), ("4.0", "cp40."), ("4.5", "cp45."), ("5.0", "cp50.")]


def nonempty(value: object) -> bool:
    return value is not None and str(value).strip() != ""


def validate_identity(report: dict) -> list[str]:
    issues = []
    if report.get("schemaVersion") != SCHEMA: issues.append("Nieobsługiwana wersja raportu.")
    if not isinstance(report.get("labNumber"), int) or not 1 <= report["labNumber"] <= 8: issues.append("Nieprawidłowy numer laboratorium.")
    if not nonempty(report.get("submissionId")): issues.append("Brak identyfikatora raportu.")
    if not nonempty(report.get("teamId")): issues.append("Brak identyfikatora studenta.")
    indices = [str(v).strip() for v in (report.get("studentIndices") or []) if nonempty(v)]
    if not 1 <= len(indices) <= 2: issues.append("Raport musi zawierać jeden lub dwa numery indeksów.")
    if any(not re.fullmatch(r"\d{5,12}", value) for value in indices): issues.append("Każdy numer indeksu musi mieć od 5 do 12 cyfr.")
    if len(set(indices)) != len(indices): issues.append("Numery indeksów nie mogą się powtarzać.")
    return issues


def validate_activity(report: dict) -> list[str]:
    issues = []
    timeline = report.get("activityTimeline")
    summary = report.get("activitySummary")
    if not isinstance(timeline, list) or not timeline:
        issues.append("Brak automatycznego timeline'u aktywności.")
        return issues
    if not isinstance(summary, dict):
        issues.append("Brak podsumowania timeline'u aktywności.")
    for event in timeline:
        if not isinstance(event, dict):
            issues.append("Nieprawidłowy wpis timeline'u aktywności.")
            continue
        if not nonempty(event.get("eventName")) or not nonempty(event.get("timestampUtc")):
            issues.append("Wpis timeline'u nie zawiera nazwy zdarzenia lub czasu UTC.")
        if event.get("labNumber") != report.get("labNumber"):
            issues.append("Timeline zawiera zdarzenie przypisane do innego laboratorium.")
        duration = event.get("durationSeconds", 0)
        if not isinstance(duration, (int, float)) or duration < 0:
            issues.append("Nieprawidłowy czas trwania zdarzenia w timeline.")
    return issues


def text_value(value: object) -> bool:
    return isinstance(value, str) and bool(value.strip())


def utc_timestamp(value: object) -> bool:
    if not text_value(value):
        return False
    try:
        if value.endswith("Z"):
            value = value[:-1] + "+00:00"
        parsed = datetime.fromisoformat(value)
        return parsed.tzinfo is not None and parsed.utcoffset() == timedelta(0)
    except ValueError:
        return False


def finite_number(value: object) -> bool:
    try:
        return type(value) in (int, float) and math.isfinite(value)
    except OverflowError:
        return False


def evaluate_wirr2(report: dict, path: Path) -> dict:
    """Validate evidence integrity, never substitute for a task-specific rubric."""
    blocking = validate_activity(report)
    lab = report.get("labNumber")
    if type(lab) is not int or not 1 <= lab <= 8:
        blocking.append("labNumber musi być liczbą całkowitą od 1 do 8.")
    for key in ("submissionId", "teamId"):
        if not text_value(report.get(key)):
            blocking.append(f"{key}: wymagany identyfikator tekstowy.")
    indices = report.get("studentIndices")
    if not isinstance(indices, list) or not 1 <= len(indices) <= 2 or any(
        not isinstance(value, str) or not re.fullmatch(r"\d{5,12}", value)
        for value in indices
    ):
        blocking.append("studentIndices: wymagany jeden lub dwa indeksy tekstowe (5–12 cyfr).")
    elif len(set(indices)) != len(indices):
        blocking.append("Numery indeksów nie mogą się powtarzać.")
    if not utc_timestamp(report.get("createdAtUtc")):
        blocking.append("createdAtUtc: wymagany czas UTC z jawną strefą.")
    versions = report.get("versions")
    if not isinstance(versions, dict) or any(
        not text_value(versions.get(key)) for key in ("unity", "package", "experiment")
    ):
        blocking.append("versions: wymagane wersje unity, package i experiment.")
    for event in report.get("activityTimeline") or []:
        if isinstance(event, dict) and (
            not utc_timestamp(event.get("timestampUtc"))
            or not finite_number(event.get("durationSeconds", 0))
        ):
            blocking.append("Timeline: wymagany czas UTC i skończony czas trwania.")

    trials = report.get("trials")
    if not isinstance(trials, list):
        blocking.append("trials musi być listą (może być pusta w przerwanej sesji).")
        trials = []
    by_id = {}
    for index, trial in enumerate(trials):
        prefix = f"trials[{index}]"
        before = len(blocking)
        if not isinstance(trial, dict):
            blocking.append(f"{prefix}: wymagany obiekt.")
            continue
        trial_id = trial.get("trial_id")
        if not text_value(trial_id) or trial_id in by_id:
            blocking.append(f"{prefix}: brak lub powtórzony trial_id.")
        if not isinstance(trial.get("condition"), dict) or not trial["condition"] or any(
            not text_value(key) or not text_value(value)
            for key, value in trial.get("condition", {}).items()
        ):
            blocking.append(f"{prefix}: condition wymaga nazwanych warunków tekstowych.")
        if trial.get("device_mode") not in tuple(DEVICE_MODES):
            blocking.append(f"{prefix}: nieznany device_mode.")
        if trial.get("participant_mode") not in ("SOLO", "HUMAN", "GUEST_SIM"):
            blocking.append(f"{prefix}: wymagany participant_mode.")
        if not utc_timestamp(trial.get("timestampUtc")):
            blocking.append(f"{prefix}: wymagany czas UTC.")
        validity = trial.get("validity")
        if validity not in ("VALID", "FAILED", "UNAVAILABLE"):
            blocking.append(f"{prefix}: nieznana validity.")
        if validity != "VALID" and not text_value(trial.get("reason")):
            blocking.append(f"{prefix}: nieudana/niedostępna próba wymaga reason.")
        metrics = trial.get("metrics")
        measured = False
        if not isinstance(metrics, dict) or not metrics:
            blocking.append(f"{prefix}: wymagane nazwane metryki.")
            metrics = {}
        for name, metric in metrics.items():
            if not text_value(name) or not isinstance(metric, dict):
                blocking.append(f"{prefix}: nieprawidłowa metryka.")
                continue
            value = metric.get("value")
            if not text_value(metric.get("unit")) or "value" not in metric:
                blocking.append(f"{prefix}.{name}: wymagane value i unit.")
            if value is None:
                if not text_value(metric.get("unavailable_reason")):
                    blocking.append(f"{prefix}.{name}: null wymaga unavailable_reason.")
            elif not finite_number(value):
                blocking.append(f"{prefix}.{name}: pomiar musi być skończoną liczbą lub null.")
            else:
                measured = True
        if validity == "VALID" and not measured:
            blocking.append(f"{prefix}: VALID wymaga przynajmniej jednego pomiaru.")
        if text_value(trial_id) and trial_id not in by_id:
            by_id[trial_id] = (trial, len(blocking) == before)

    evidence = report.get("checkpointEvidence")
    if not isinstance(evidence, dict) or any(key not in dict(CHECKPOINTS) for key in evidence):
        blocking.append("checkpointEvidence: wymagany obiekt z kluczami progów 3.0–5.0.")
        evidence = {}
    complete = []
    missing = []
    chain = True
    for grade, _ in CHECKPOINTS:
        entry = evidence.get(grade)
        refs = entry.get("trial_ids") if isinstance(entry, dict) else None
        ready = isinstance(refs, list) and bool(refs) and all(text_value(x) for x in refs)
        if ready:
            ready = len(set(refs)) == len(refs) and all(
                ref in by_id and by_id[ref][1] and by_id[ref][0]["validity"] == "VALID"
                for ref in refs
            )
        if ready:
            # Mixed SIM/REAL or REPLAY/LIVE evidence cannot establish a comparison.
            ready = len({by_id[ref][0]["device_mode"] for ref in refs}) == 1
        if not ready:
            missing.append(f"{grade}: brak poprawnych, jednoznacznych odwołań do prób jednego źródła.")
        if grade in {"4.0", "4.5", "5.0"} and not text_value(report.get("conclusion")):
            ready = False
            missing.append(f"{grade}: brak wspólnego wniosku.")
        chain = chain and ready
        if chain:
            complete.append(grade)
    missing.append("Ocena zablokowana: brak zatwierdzonych reguł dowodów specyficznych dla zadania WiRR 2.0.")
    return {
        "graderSchema": "wirr-evidence/2.0-draft.1",
        "status": "ERROR" if blocking else "PROVISIONAL",
        "requiresInstructorApproval": True,
        "sourceReport": str(path),
        "submissionId": report.get("submissionId"),
        "teamId": report.get("teamId"),
        "labNumber": report.get("labNumber"),
        "blockingIssues": blocking,
        "reportMode": "wirr2",
        "proposed_grade": None,
        "achieved_checkpoints": [],
        "structurally_complete_checkpoints": complete if not blocking else [],
        "missing_evidence": missing,
        "note": "Kontrola struktury dowodów nie potwierdza wykonania zadania ani interpretacji danych."
    }


def evaluate(path: Path) -> dict:
    report = json.loads(path.read_text(encoding="utf-8"))
    if not isinstance(report, dict):
        raise TypeError("Raport musi być obiektem JSON.")
    if report.get("schemaVersion") == WIRR2_SCHEMA:
        return evaluate_wirr2(report, path)
    blocking = validate_identity(report) + validate_activity(report)
    answers = {str(x.get("key")): str(x.get("value") or "").strip() for x in report.get("answers") or [] if isinstance(x, dict) and nonempty(x.get("key"))}
    basic_mode = answers.get("basic.mode") == "true"
    if basic_mode:
        required = ("basic.source", "basic.observation", "basic.change", "basic.conclusion")
        for key in required:
            if not nonempty(answers.get(key)):
                blocking.append(f"Uzupełnij pole raportu: {key}.")
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
        "activityEventCount": len(report.get("activityTimeline") or []),
        "activitySummary": report.get("activitySummary") or {},
        "checkpoints": checkpoints,
        "reportMode": "basic" if basic_mode else "legacy",
        "basicComplete": basic_mode and not any(not nonempty(answers.get(key)) for key in ("basic.source", "basic.observation", "basic.change", "basic.conclusion")),
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
