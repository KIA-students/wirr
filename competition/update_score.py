"""Offline update of public two-team WiRR league scores. No automatic grading."""
from __future__ import annotations

import argparse
import json
from datetime import UTC, datetime
from pathlib import Path

BOARD = Path(__file__).with_name("scoreboard.json")
BOUNDS = {"experiment": 4, "reasoning": 3, "innovation": 2, "cooperation": 1}


def validate(board: dict) -> None:
    if board.get("schema_version") != "wirr-team-league/1":
        raise ValueError("Unsupported scoreboard schema")
    if [g["id"] for g in board["groups"]] != ["G1", "G2"]:
        raise ValueError("Expected G1 and G2")
    if [item["id"] for item in board["rounds"]] != [1, 2, 3, 4]:
        raise ValueError("Expected rounds 1–4")
    for item in board["rounds"]:
        filled = 0
        for group in ("G1", "G2"):
            entry = item["scores"][group]
            if entry is None:
                continue
            points = entry["points"]
            if set(points) != set(BOUNDS):
                raise ValueError("Unknown scoring categories")
            for key, maximum in BOUNDS.items():
                if type(points[key]) is not int or not 0 <= points[key] <= maximum:
                    raise ValueError(f"Invalid {key} for {group} in round {item['id']}")
            if not 4 <= len(entry["reason"].strip()) <= 300:
                raise ValueError("Use a non-identifying explanation, 4–300 characters")
            filled += 1
        state = "completed" if filled == 2 else "in_progress" if filled else "pending"
        if item["status"] != state:
            raise ValueError("Round status inconsistent")
    if set(board.get("sidequests", {})) != {"G1", "G2"}:
        raise ValueError("Missing G1/G2 sidequest tracking")
    for group, entries in board["sidequests"].items():
        if len(entries) > 4 or len({item["task"] for item in entries}) != len(entries):
            raise ValueError("Sidequests must be unique and limited")
        if sum(item["points"] for item in entries) > 8:
            raise ValueError("Too many optional points")
        for entry in entries:
            if entry["task"] not in ("xr-optics", "robot-failsafe", "accessible-hud", "ar-measurement"):
                raise ValueError("Unrecognized optional task")
            if type(entry["points"]) is not int or entry["points"] not in (1, 2):
                raise ValueError("Invalid optional points")
            if not 4 <= len(entry["reason"].strip()) <= 300:
                raise ValueError("Optional mission needs non-personal justification")
    if type(board.get("revision")) is not int or board["revision"] < 0:
        raise ValueError("Bad revision")


def award(board: dict, round_id: int, group: str, points: dict[str, int],
          reason: str, evidence: str | None = None) -> dict:
    validate(board)
    if group not in ("G1", "G2") or round_id not in range(1, 5):
        raise ValueError("Choose group G1/G2 and round 1–4")
    if set(points) != set(BOUNDS):
        raise ValueError("Four scoring categories required")
    for key, maximum in BOUNDS.items():
        if type(points[key]) is not int or not 0 <= points[key] <= maximum:
            raise ValueError(f"{key} must be an integer 0–{maximum}")
    if not 4 <= len(reason.strip()) <= 300:
        raise ValueError("Add 4–300 characters of non-personal justification")
    if evidence and not (evidence.startswith("https://github.com/KIA-students/wirr/") or
                         evidence.startswith("https://kia-students.github.io/wirr/")):
        raise ValueError("Evidence must be a public course URL")
    item = board["rounds"][round_id - 1]
    now = datetime.now(UTC).isoformat(timespec="seconds")
    item["scores"][group] = {
        "points": points,
        "reason": reason.strip(),
        "evidence": evidence or None,
        "approved_at": now,
    }
    filled = sum(item["scores"][name] is not None for name in ("G1", "G2"))
    item["status"] = "completed" if filled == 2 else "in_progress"
    board["revision"] += 1
    board["updated_at"] = now
    board["history"].append({
        "revision": board["revision"], "round": round_id, "group": group,
        "total": sum(points.values()), "at": now, "reason": reason.strip()
    })
    validate(board)
    return board


def main() -> None:
    ap = argparse.ArgumentParser(description=__doc__)
    ap.add_argument("--file", type=Path, default=BOARD)
    ap.add_argument("--round", type=int, choices=range(1, 5))
    ap.add_argument("--group", choices=("G1", "G2"))
    for field in BOUNDS:
        ap.add_argument("--" + field, type=int)
    ap.add_argument("--reason")
    ap.add_argument("--evidence", default="")
    args = ap.parse_args()
    data = json.loads(args.file.read_text(encoding="utf-8"))
    if args.round is None and args.group is None:
        validate(data)
        print("Scoreboard: poprawny")
        return
    if args.round is None or args.group is None or args.reason is None:
        ap.error("Provide round, group, reason and all four point categories")
    try:
        award(data, args.round, args.group, {k: getattr(args, k) for k in BOUNDS},
              args.reason, args.evidence)
    except (ValueError, TypeError) as exc:
        ap.error(str(exc))
    args.file.write_text(json.dumps(data, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    print(f"Zapisano zatwierdzoną punktację {args.group}, runda {args.round}")


if __name__ == "__main__":
    main()
