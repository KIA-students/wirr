#!/usr/bin/env python3
"""Fail closed on partial, stale, skipped or malformed Unity NUnit results."""
import argparse
import importlib.util
from pathlib import Path
import xml.etree.ElementTree as ET


def validate(path: Path, suite: str) -> dict:
    root = ET.parse(path).getroot()
    if root.tag != 'test-run' or root.get('result') != 'Passed':
        raise ValueError('NUnit root must be test-run with result=Passed')
    cases = root.findall('.//test-case')
    if not cases or int(root.get('total', '0')) != len(cases):
        raise ValueError('Missing tests or inconsistent total')
    if any(int(root.get(name, '0')) != 0 for name in ('failed', 'skipped', 'inconclusive')):
        raise ValueError('Failed, skipped or inconclusive NUnit cases')
    names = [case.get('fullname', '') for case in cases]
    if any(not name for name in names) or len(names) != len(set(names)):
        raise ValueError('Missing or duplicate test case identities')
    if any(case.get('result') != 'Passed' for case in cases):
        raise ValueError('Every test case must pass')
    if suite == 'EditMode':
        runner = Path(__file__).resolve().parents[2] / 'scripts~' / 'run_wirr_editmode.py'
        spec = importlib.util.spec_from_file_location('wirr_required_tests', runner)
        module = importlib.util.module_from_spec(spec)
        spec.loader.exec_module(module)
        module.validate_results(path)
        bridge = ('ForeignDraftWithoutTeamIdIsRejectedWithoutRelabelling',
                  'PairCanBeImportedByEitherMemberWithoutChangingTheDraft',
                  'ForeignGroupIsRejectedBeforeWriting')
        if any(not any(name.endswith('WiRRStudentReportBridgeTests.' + test) for name in names) for test in bridge):
            raise ValueError('Missing report ownership regression tests')
    else:
        expected = (
            'AllEightLessonsCreateADesktopWorld', 'CadLodSwitchesActualMeshAndReturnsToHigh',
            'InteractionTeleportHasStableHomeAndDoesNotMoveTheTask',
            'AnchorViewChangesPreserveReferenceAndHideMissingPose',
            'DepthReferenceIsIndependentOfRenderingLabelsAndLight',
            'CadMaterialsPreserveMeshAndReferenceScale',
            'ReplayStaleReactionPreservesPoseAndRestoresVisibility',
            'MissionTransferChangesDistanceWithoutMovingGoals',
        )
        if any('WiRRStudentKitPlayModeTests.' + test not in names for test in expected):
            raise ValueError('Missing WiRR PlayMode test cases')
    return {'suite': suite, 'passed': len(cases)}


if __name__ == '__main__':
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('xml', type=Path)
    parser.add_argument('--suite', choices=('EditMode', 'PlayMode'), required=True)
    args = parser.parse_args()
    print(validate(args.xml, args.suite))
