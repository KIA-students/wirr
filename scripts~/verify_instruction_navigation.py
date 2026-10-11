"""Check WiRR lab Markdown navigation locally; no GitHub Actions needed.

Run: python3 scripts~/verify_instruction_navigation.py
Optional: python3 scripts~/verify_instruction_navigation.py --root PATH
"""
from __future__ import annotations

import argparse
import re
import unicodedata
from pathlib import Path
from urllib.parse import unquote

TOC = "## Spis treści"
LINK = re.compile(r"(?<!!)\[[^\]]+\]\(([^)\s]+)(?:\s+\"[^\"]+\")?\)")
HEADING = re.compile(r"^(#{1,6}) (.+)$", re.MULTILINE)
PUNCTUATION_TO_KEEP = {"_", "-", " "}


def slug(text: str) -> str:
    """Approximate GitHub heading anchors while preserving Polish diacritics."""
    text = re.sub(r"<[^>]+>", "", text).replace("\u0060", "")
    text = re.sub(r"\[([^\]]+)\]\([^)]+\)", r"\1", text)
    chars = [
        c for c in text.lower()
        if unicodedata.category(c)[0] in {"L", "N", "M"} or c in PUNCTUATION_TO_KEEP
    ]
    return "".join(chars).strip().replace(" ", "-")


def anchors(document: str) -> set[str]:
    seen: dict[str, int] = {}
    result: set[str] = set()
    for heading in HEADING.finditer(document):
        stem = slug(heading.group(2))
        count = seen.get(stem, 0)
        seen[stem] = count + 1
        result.add(stem + (f"-{count}" if count else ""))
    for match in re.finditer(r'<a\s+(?:name|id)="([^"]+)"', document):
        result.add(match.group(1))
    return result


def sources(root: Path) -> list[Path]:
    lab = sorted((root / "Samples~").glob("Lab??/README.md"))
    viture = root / "Samples~" / "VITURE XR" / "README.md"
    if viture.exists():
        lab.append(viture)
    reference = root / "Documentation~" / "Reference"
    names = [
        "UNITY_ACCESS_AND_LICENSES.md", "COMFORT_STUDIES.md", "OS_SETUP.md",
        "XR_SAFETY_WATCHDOG.md", "QUICK_START.md", "INDEX.md",
        "ENVIRONMENT_SETUP.md", "BUILD_AND_DEPLOY.md",
        "FOUR_BLOCK_MISSIONS.md", "VITURE_XR.md",
    ] + [f"LAB{i}_STARTER_README.md" for i in range(1, 8)]
    lab += [reference / n for n in names if (reference / n).exists()]
    lab.append(root / "competition" / "OPTIONAL_MISSIONS.md")
    return lab


def inspect(root: Path) -> list[str]:
    errors: list[str] = []
    files = sources(root)
    for document in files:
        if not document.exists():
            errors.append(f"Missing document: {document}")
            continue
        text = document.read_text(encoding="utf-8")
        if TOC not in text:
            errors.append(f"{document.relative_to(root)}: no {TOC}")
        for link in LINK.finditer(text):
            target = link.group(1)
            if target.startswith(("https://", "http://", "mailto:", "data:")):
                continue
            filename, _, fragment = target.partition("#")
            if not filename and not fragment:
                continue
            dest = (document.parent / unquote(filename)).resolve() if filename else document
            if not dest.is_file():
                errors.append(f"{document.relative_to(root)}: missing file {target}")
                continue
            if fragment and dest.suffix.lower() == ".md":
                destination = dest.read_text(encoding="utf-8")
                if unquote(fragment) not in anchors(destination):
                    errors.append(f"{document.relative_to(root)}: missing anchor {target}")
    return errors


def main() -> None:
    ap = argparse.ArgumentParser(description=__doc__)
    ap.add_argument("--root", type=Path, default=Path(__file__).resolve().parent.parent)
    args = ap.parse_args()
    root = args.root.resolve()
    errors = inspect(root)
    if errors:
        raise SystemExit("\n".join(errors))
    print(f"OK: {len(sources(root))} instructions have a TOC and resolvable local links")


if __name__ == "__main__":
    main()
