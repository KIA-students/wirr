"""Lightweight unit tests for voluntary course league updates (no Unity needed)."""
import copy
import json
import unittest
from pathlib import Path

from update_score import validate, award
from set_team_name import change_name
from add_sidequest import add_bonus

BOARD = json.loads(Path(__file__).with_name("scoreboard.json").read_text(encoding="utf-8"))


class LeagueTests(unittest.TestCase):
    def setUp(self):
        self.board = copy.deepcopy(BOARD)

    def test_blank_scoreboard(self):
        validate(self.board)
        self.assertEqual(self.board["groups"][0]["label"], "Grupa laboratoryjna 1")
        self.assertIsNone(self.board["rounds"][0]["scores"]["G1"])
        self.assertEqual(self.board["sidequests"], {"G1": [], "G2": []})

    def test_two_named_teams(self):
        change_name(self.board, "G1", "Neonowe Kapibary")
        change_name(self.board, "G2", "Kwantowe Pierogi")
        validate(self.board)
        self.assertEqual(self.board["groups"][0]["label"], "Neonowe Kapibary")
        with self.assertRaises(ValueError):
            change_name(self.board, "G2", "Neonowe Kapibary")

    def test_approved_bonus_is_group_only(self):
        add_bonus(self.board, "G2", "accessible-hud", 2, "Zbadano dwa warianty czytelności UI")
        validate(self.board)
        self.assertEqual(sum(x["points"] for x in self.board["sidequests"]["G2"]), 2)
        self.assertIsNone(self.board["rounds"][0]["scores"]["G2"])
        with self.assertRaises(ValueError):
            add_bonus(self.board, "G2", "accessible-hud", 2, "Próba ponownego zaliczenia")

    def test_unapproved_or_excess_bonus_rejected(self):
        with self.assertRaises(ValueError):
            add_bonus(self.board, "G1", "xr-optics", 9, "Nieprawidłowe punkty")
        with self.assertRaises(ValueError):
            add_bonus(self.board, "G1", "nonexistent", 1, "Nie ma takiej misji")

    def test_normal_round_scoring_still_works(self):
        award(self.board, 1, "G1", {
            "experiment": 3, "reasoning": 2, "innovation": 2, "cooperation": 1
        }, "Prawidłowo udokumentowano eksperyment")
        self.assertEqual(self.board["rounds"][0]["status"], "in_progress")
        validate(self.board)


if __name__ == "__main__":
    unittest.main()
