"""Instructor awards small optional team bonus points; no automated judging."""
from __future__ import annotations

import argparse
import json
from datetime import UTC, datetime
from pathlib import Path

from update_score import validate

DEFAULT = Path(__file__).with_name("scoreboard.json")
TASKS = ("xr-optics", "robot-failsafe", "accessible-hud", "ar-measurement")
MAX_BONUS = 8


def add_bonus(board: dict, group: str, task: str, points: int, reason: str) -> dict:
    validate(board)
    if group not in ("G1","G2") or task not in TASKS:
        raise ValueError("Nieznana grupa albo wyzwanie")
    if type(points) is not int or points not in (1,2):
        raise ValueError("Za misję można przyznać 1 lub 2 punkty")
    if not 4 <= len(reason.strip()) <= 300:
        raise ValueError("Podaj krótkie uzasadnienie bez danych osobowych")
    entries = board["sidequests"][group]
    if any(x["task"] == task for x in entries):
        raise ValueError("Ta misja została już zatwierdzona dla grupy")
    if sum(x["points"] for x in entries) + points > MAX_BONUS:
        raise ValueError("Limit 8 punktów poza zajęciami na grupę")
    now = datetime.now(UTC).isoformat(timespec="seconds")
    entries.append({"task":task,"points":points,"reason":reason.strip(),"approved_at":now})
    board["revision"] += 1
    board["updated_at"] = now
    board["history"].append({
        "revision":board["revision"],"group":group,"action":"sidequest",
        "task":task,"points":points,"at":now
    })
    validate(board)
    return board


def main() -> None:
    ap = argparse.ArgumentParser(description=__doc__)
    ap.add_argument("--file",type=Path,default=DEFAULT)
    ap.add_argument("--group",choices=("G1","G2"),required=True)
    ap.add_argument("--task",choices=TASKS,required=True)
    ap.add_argument("--points",type=int,choices=(1,2),required=True)
    ap.add_argument("--reason",required=True)
    args=ap.parse_args()
    data=json.loads(args.file.read_text(encoding="utf-8"))
    try:
        add_bonus(data,args.group,args.task,args.points,args.reason)
    except (TypeError,KeyError,ValueError) as exc:
        ap.error(str(exc))
    args.file.write_text(json.dumps(data,ensure_ascii=False,indent=2)+"\n",encoding="utf-8")
    print("Zatwierdzono dodatkową misję. Wynik zostanie opublikowany po scaleniu zmian.")


if __name__=="__main__":
    main()
