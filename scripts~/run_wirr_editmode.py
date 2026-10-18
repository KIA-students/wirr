#!/usr/bin/env python3
"""Run local Unity report tests and reject missing, skipped or partial NUnit results."""
import argparse
from datetime import datetime, timezone
import json
import os
from pathlib import Path
import subprocess
import sys
import tempfile
import xml.etree.ElementTree as ET

PACKAGE = 'pl.prz.kia.wirr'
ASSEMBLY = 'KIA.WiRR.Editor.Tests'
EXPECTED = {
    'KIA.WiRR.Editor.Tests.WiRRSubmissionReceiptTests.AcceptsReceiptRegardlessOfFormatting': 4,
    'KIA.WiRR.Editor.Tests.WiRRSubmissionReceiptTests.RejectsMissingRefusedUnrelatedOrMalformedReceipt': 8,
    'KIA.WiRR.Editor.Tests.WiRRExportSnapshotTests.ExportingCorrectionsPreservesPreviousReportAndReceipt': 1,
    'KIA.WiRR.Editor.Tests.WiRRExportSnapshotTests.RepeatedExportDoesNotOverwriteSnapshot': 1,
    'KIA.WiRR.Editor.Tests.WiRRExportSnapshotTests.SnapshotKeepsLabFilenameAndUtf8Content': 1,
}


def preflight(repo):
    project = repo / 'Project~'
    packages = project / 'Packages'
    manifest = json.loads((packages / 'manifest.json').read_text(encoding='utf-8'))
    dependency = manifest.get('dependencies', {}).get(PACKAGE, '')
    if not dependency.startswith('file:') or (packages / dependency[5:]).resolve() != repo.resolve():
        raise ValueError('Projekt nie wskazuje lokalnego pakietu WiRR w katalogu repozytorium.')
    if json.loads((repo / 'package.json').read_text(encoding='utf-8')).get('name') != PACKAGE:
        raise ValueError('Brak właściwego package.json pakietu WiRR.')
    if PACKAGE not in manifest.get('testables', []) or 'com.unity.test-framework' not in manifest['dependencies']:
        raise ValueError('Brak konfiguracji testables lub Unity Test Framework.')
    version_file = project / 'ProjectSettings' / 'ProjectVersion.txt'
    for line in version_file.read_text(encoding='utf-8').splitlines():
        if line.startswith('m_EditorVersion: '):
            return project, line.split(': ', 1)[1].strip()
    raise ValueError('Brak wersji edytora w ProjectVersion.txt.')


def locate_unity(version, explicit=None):
    if explicit:
        candidate = Path(explicit).expanduser()
        if not candidate.is_file():
            raise ValueError('Nie znaleziono wskazanego pliku Unity Editor.')
        return candidate.resolve()
    candidates = [
        Path('C:/Program Files/Unity/Hub/Editor') / version / 'Editor/Unity.exe',
        Path.home() / 'Unity/Hub/Editor' / version / 'Editor/Unity',
        Path('/Applications/Unity/Hub/Editor') / version / 'Unity.app/Contents/MacOS/Unity',
    ]
    for candidate in candidates:
        if candidate.is_file():
            return candidate.resolve()
    raise ValueError('Brak lokalnego Unity ' + version + '. Podaj --unity ze ścieżką do edytora.')


def validate_results(path):
    root = ET.parse(path).getroot()
    if root.tag != 'test-run' or root.get('result') != 'Passed':
        raise ValueError('Przebieg NUnit nie ma statusu Passed.')
    cases = root.findall('.//test-case')
    names = set()
    counts = {name: 0 for name in EXPECTED}
    for case in cases:
        name = case.get('fullname', '')
        if not name or name in names or case.get('result') != 'Passed':
            raise ValueError('Błędny, pominięty lub powtórzony przypadek testowy.')
        names.add(name)
        method = name.split('(', 1)[0]
        if method in counts:
            counts[method] += 1
    if any(counts[name] != count for name, count in EXPECTED.items()):
        raise ValueError('Brakuje wymaganych 15 przypadków lub zmieniono zestaw testów: ' + str(counts))
    return {'executed': len(cases), 'required': sum(counts.values()), 'methods': counts}


def git_metadata(repo):
    try:
        revision = subprocess.check_output(['git', '-C', str(repo), 'rev-parse', 'HEAD'],
                                           stderr=subprocess.DEVNULL, timeout=5).decode().strip()
        status = subprocess.check_output(['git', '-C', str(repo), 'status', '--porcelain'],
                                         stderr=subprocess.DEVNULL, timeout=5).decode().strip()
        return {'source_revision': revision, 'working_tree_dirty': bool(status)}
    except (OSError, subprocess.SubprocessError):
        return {'source_revision': None, 'working_tree_dirty': None}


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--unity', default=os.environ.get('UNITY_EDITOR_PATH'))
    parser.add_argument('--check-only', action='store_true', help='Sprawdź projekt bez uruchamiania Unity.')
    args = parser.parse_args(argv)
    repo = Path(__file__).resolve().parents[1]
    try:
        project, version = preflight(repo)
        if args.check_only:
            print('Konfiguracja poprawna. Unity ' + version + '. Testów Unity NIE uruchomiono.')
            return 0
        unity = locate_unity(version, args.unity)
    except (OSError, ValueError) as exc:
        print(str(exc), file=sys.stderr)
        return 2
    logs = project / 'Logs'
    logs.mkdir(exist_ok=True)
    output = Path(tempfile.mkdtemp(prefix='wirr-editmode-', dir=logs))
    result = output / 'results.xml'
    command = [str(unity), '-batchmode', '-nographics', '-runTests',
               '-projectPath', str(project), '-testPlatform', 'EditMode',
               '-assemblyNames', ASSEMBLY, '-testResults', str(result),
               '-logFile', str(output / 'editor.log')]
    summary = {'passed': False, 'started_at_utc': datetime.now(timezone.utc).isoformat(),
               'project_editor_version': version, 'command': command}
    summary.update(git_metadata(repo))
    print('Wyniki i log: ' + str(output), flush=True)
    try:
        run = subprocess.run(command, timeout=900, check=False)
        summary['unity_exit_code'] = run.returncode
        if run.returncode != 0:
            raise ValueError('Unity zakończyło się błędem. Sprawdź editor.log.')
        summary.update(validate_results(result))
        summary['passed'] = True
    except (OSError, ValueError, ET.ParseError, subprocess.TimeoutExpired) as exc:
        summary['error'] = str(exc)
        print(str(exc), file=sys.stderr)
    finally:
        summary['finished_at_utc'] = datetime.now(timezone.utc).isoformat()
        (output / 'summary.json').write_text(json.dumps(summary, ensure_ascii=False, indent=2) + '\n', encoding='utf-8')
    print('PASS: 15 wymaganych przypadków Unity.' if summary['passed'] else 'FAIL: odbiór Unity niezaliczony.')
    return 0 if summary['passed'] else 1


if __name__ == '__main__':
    raise SystemExit(main())
