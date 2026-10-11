"""Instructor approves a group's public nickname, without student identities."""
from __future__ import annotations

import argparse
import json
import re
from datetime import UTC, datetime
from pathlib import Path

from update_score import validate

DEFAULT = Path(__file__).with_name("scoreboard.json")
SAFE = re.compile(r"^[^\W_]+(?:[ -][^\W_]+)*$", re.UNICODE)


def change_name(board: dict, group: str, name: str) -> dict:
    validate(board)
    clean = " ".join(name.split())
    if group not in ("G1", "G2"):
        raise ValueError("Grupa musi być G1 lub G2")
    if not 3 <= len(clean) <= 48 or not SAFE.fullmatch(clean):
        raise ValueError("Nazwa powinna mieć 3–48 znaków: litery, cyfry, spacje i łączniki")
    if clean.casefold() in {g["label"].casefold() for g in board["groups"] if g["id"] != group}:
        raise ValueError("Druga grupa już używa tej nazwy")
    item = next(g for g in board["groups"] if g["id"] == group)
    old = item["label"]
    item["label"] = clean
    board["revision"] += 1
    board["updated_at"] = datetime.now(UTC).isoformat(timespec="seconds")
    board["history"].append({
        "revision": board["revision"], "group": group, "at": board["updated_at"],
        "action": "rename", "old_label": old, "new_label": clean
    })
    validate(board)
    return board


def main() -> None:
    ap = argparse.ArgumentParser(description=__doc__)
    ap.add_argument("--file", type=Path, default=DEFAULT)
    ap.add_argument("--group", choices=("G1", "G2"), required=True)
    ap.add_argument("--name", required=True)
    args = ap.parse_args()
    data = json.loads(args.file.read_text(encoding="utf-8"))
    try:
        change_name(data,args.group,args.name)
    except (KeyError, TypeError, ValueError) as exc:
        ap.error(str(exc))
    args.file.write_text(json.dumps(data,ensure_ascii=False,indent=2)+"\n",encoding="utf-8")
    print("Zapisano nową nazwę po zatwierdzeniu przez prowadzącego")


if __name__ == "__main__":
    main()
