#!/usr/bin/env bash
# Lightweight WiRR validation for Raspberry Pi / Linux. No Unity, Docker or paid runners.
set -Eeuo pipefail
export GIT_TERMINAL_PROMPT=0
REPO="${WIRR_REPO:-$HOME/repo/wirr-matpom}"
REF="${WIRR_REF:-fix/unity-6000-6-5-student-materials}"
OUT="${WIRR_REPORT_DIR:-$HOME/wirr-validation}"
mkdir -p "$OUT"
stamp="$(date -u +%Y%m%dT%H%M%SZ)"
report="$OUT/$stamp"
mkdir -p "$report"
exec > >(tee -a "$report/console.log") 2>&1
echo "WiRR validation: $stamp, host=$(hostname), ref=$REF"
if ! command -v git >/dev/null || ! command -v python3 >/dev/null; then
  echo "ERROR: git and python3 required"; exit 2
fi
if [[ ! -d "$REPO/.git" && ! -f "$REPO/.git" ]]; then
  if command -v gh >/dev/null && gh auth status >/dev/null 2>&1; then
    mkdir -p "$(dirname "$REPO")"
    gh repo clone MatPomGit/wirr "$REPO" -- --filter=blob:none
  else
    echo "ERROR: No checkout at $REPO. Authenticate with 'gh auth login' and rerun."; exit 2
  fi
fi
git -C "$REPO" fetch --no-tags origin "refs/heads/$REF"
sha="$(git -C "$REPO" rev-parse FETCH_HEAD)"
[[ "$sha" =~ ^[0-9a-f]{40}$ ]] || { echo "ERROR: bad SHA"; exit 2; }
echo "$sha" > "$report/sha.txt"
work="$(mktemp -d "$OUT/work.XXXXXXXX")"
cleanup(){ git -C "$REPO" worktree remove --force "$work" >/dev/null 2>&1 || true; }
trap cleanup EXIT
git -C "$REPO" worktree add --detach "$work" "$sha"
cd "$work"
echo "Checked out SHA $sha"
python3 -m venv "$OUT/.venv"
"$OUT/.venv/bin/python" -m pip -q install --disable-pip-version-check 'PyYAML>=6,<7' 'tree-sitter>=0.25,<0.27' 'tree-sitter-c-sharp>=0.23,<0.25'
python="$OUT/.venv/bin/python"
"$python" --version
"$python" - <<'PY'
import json
from pathlib import Path
p=json.loads(Path("package.json").read_text(encoding="utf-8"))
assert p.get("unity") == "6000.6", f"Unexpected Unity major/minor {p.get('unity')!r}"
assert len([s for s in p.get("samples",[]) if str(s.get("displayName","")).startswith("Laboratorium ")]) == 8
print("PASS: package version and 8 laboratory samples (static)")
PY
ok=1
total=0
passed_cases=0
skipped=0
for folder in 'scripts~' 'tools/wirr2' 'tests' 'competition' 'WebSim~'; do
  [[ -d "$folder" ]] || continue
  logfile="$report/$(echo "$folder" | tr '/~' '__').log"
  # Some directories contain JS assets or non-Python tests; skip these explicitly.
  if ! find "$folder" -type f -name 'test_*.py' -print -quit | grep -q .; then
    echo "SKIP: $folder has no Python unittest files"
    skipped=$((skipped+1))
    continue
  fi
  total=$((total+1))
  echo "Testing: $folder"
  if "$python" -B -m unittest discover -s "$folder" -p 'test_*.py' -v >"$logfile" 2>&1; then
    suite_status=0
  else
    suite_status=1
  fi
  # unittest may report success with 0 collected tests; never count that as PASS.
  cases="$(sed -nE 's/^Ran ([0-9]+) tests? in .*/\1/p' "$logfile" | tail -n 1)"
  cases="${cases:-0}"
  if [[ ! "$cases" =~ ^[0-9]+$ ]]; then
    echo "FAIL: nonnumeric test count in $folder: $cases"
    ok=0
    cases=0
  fi
  if [[ "$suite_status" -ne 0 || "$cases" -eq 0 ]]; then
    echo "FAIL: $folder (exit=$suite_status, tests=$cases)"
    tail -n 60 "$logfile"
    ok=0
  else
    echo "PASS: $folder ($cases tests)"
    passed_cases=$((passed_cases+cases))
    tail -n 4 "$logfile"
  fi
done
if ((total < 4)); then echo "ERROR: expected at least four Python test suites, ran $total"; ok=0; fi
if ((passed_cases < 65)); then echo "ERROR: expected at least 65 successful tests, got $passed_cases"; ok=0; fi
{
  echo "# WiRR offline validation"
  echo "- UTC: $stamp"
  echo "- Host: $(hostname)"
  echo "- SHA: $sha"
  echo "- Ref: $REF"
  echo "- Test suites attempted: $total"
  echo "- Tests passed in successful suites: $passed_cases"
  echo "- Directories without Python tests: $skipped"
  echo "- Outcome: $([[ $ok == 1 ]] && echo PASS || echo FAIL)"
  echo "- Scope: Python/static only. Unity EditMode/PlayMode, XR and Supabase E2E NOT TESTED."
} | tee "$report/summary.md"
echo "Logs: $report"
exit "$((1-ok))"
