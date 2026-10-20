#!/usr/bin/env python3
"""Non-punitive, offline screening of WiRR JSON measurement consistency.

Does not establish whether the student actually performed the experiment.
No index numbers or identifying report metadata are printed.
"""
import argparse
import json
import math
from pathlib import Path


def number(value):
    if isinstance(value, bool):
        return None
    try:
        n = float(str(value).strip().replace(",", "."))
        return n if math.isfinite(n) else None
    except (TypeError, ValueError):
        return None


def scan(report):
    answers = {item["key"]: item.get("value") for item in report.get("answers", [])
               if isinstance(item, dict) and isinstance(item.get("key"), str)}
    findings = []
    tested = 0
    reported_measurements = 0
    unchecked_measurements = 0
    for key, value in answers.items():
        if key.endswith(".mediana_fps"):
            if number(value) is not None:
                reported_measurements += 1
            sibling = key[:-len("mediana_fps")] + "mediana_ms"
            fps, ms = number(value), number(answers.get(sibling))
            if fps is not None and fps <= 0:
                findings.append({"type": "nonpositive_fps", "field": key,
                                 "detail": "FPS must be positive"})
            if ms is not None and ms <= 0:
                findings.append({"type": "nonpositive_frame_time", "field": sibling,
                                 "detail": "frame duration must be positive"})
            if fps is not None and ms is not None and fps > 0 and ms > 0:
                tested += 1
                # Independently calculated medians and rounding need not be reciprocals.
                if abs(fps * ms - 1000) > 35:
                    findings.append({"type": "fps_ms_check", "field": key,
                                     "detail": "FPS*ms differs from 1000 by more than 35"})
            elif number(value) is not None:
                unchecked_measurements += 1
        if key.endswith(".mediana_s"):
            if number(value) is not None:
                reported_measurements += 1
            prefix = key[:-len("mediana_s")]
            times = [number(v) for k, v in answers.items()
                     if k.startswith(prefix + "próba_") and k.endswith("_s")]
            times = sorted(v for v in times if v is not None)
            reported = number(value)
            if any(v < 0 for v in times):
                findings.append({"type": "negative_trial_duration", "field": key,
                                 "detail": "recorded trial duration cannot be negative"})
            if reported is not None and reported < 0:
                findings.append({"type": "negative_median_duration", "field": key,
                                 "detail": "median duration cannot be negative"})
            if reported is not None and len(times) < 3:
                unchecked_measurements += 1
            if reported is not None and len(times) >= 3:
                tested += 1
                count = len(times)
                median = times[count // 2] if count % 2 else (times[count // 2 - 1] + times[count // 2]) / 2
                if abs(reported - median) > 0.051:
                    findings.append({"type": "median_check", "field": key,
                                     "detail": "reported median does not match raw trials"})
    return {"checks_performed": tested, "reported_measurements": reported_measurements,
            "unchecked_measurements": unchecked_measurements, "findings": findings,
            "evidence_status": "screening_only_not_authenticity_proof"}


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("paths", nargs="+", type=Path)
    args = parser.parse_args()
    for ordinal, path in enumerate(args.paths, start=1):
        report = json.loads(path.read_text(encoding="utf-8"))
        result = scan(report)
        print(json.dumps({"report_ordinal": ordinal, **result}, ensure_ascii=False))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
