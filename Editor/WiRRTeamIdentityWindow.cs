using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace KIA.WiRR.Editor
{
    /// <summary>Locally editable team identity, independent of the selected lab.</summary>
    public sealed class WiRRTeamIdentityWindow : EditorWindow
    {
        private static readonly string[] Groups = { "Nie wybrano", "G1", "G2" };
        private static readonly string[] Prefixes =
            { "Kwantowe", "Pikselowe", "Wirtualne", "Laserowe", "Holograficzne" };
        private static readonly string[] Subjects =
            { "Kapibary", "Dziobaki", "Roboty", "Wombaty", "Pierogi" };
        private static readonly System.Random Random = new System.Random();
        private string teamName = "";
        private int group;

        [MenuItem("WiRR/Liga XR/Edytuj nazwę i grupę", priority = 30)]
        public static void Open()
        {
            var window = GetWindow<WiRRTeamIdentityWindow>("Drużyna WiRR");
            window.minSize = new Vector2(350, 190);
            window.Show();
        }

        private void OnEnable()
        {
            teamName = EditorPrefs.GetString("KIA.WiRR.TeamName", "");
            group = Mathf.Clamp(EditorPrefs.GetInt("KIA.WiRR.MissionGroup", 0), 0, 2);
        }

        private static bool IsValid(string name)
        {
            return !string.IsNullOrWhiteSpace(name) && name.Trim().Length >= 3
                && name.Trim().Length <= 48
                && name.Trim().All(ch => char.IsLetterOrDigit(ch) || ch == ' ' || ch == '-');
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Moja grupa i nazwa drużyny", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Możesz zmienić nazwę w dowolnej chwili. Zapis jest lokalny; " +
                "publiczną nazwę w Lidze XR zatwierdza prowadzący.", MessageType.Info);
            EditorGUI.BeginChangeCheck();
            group = EditorGUILayout.Popup("Grupa laboratoryjna", group, Groups);
            teamName = EditorGUILayout.TextField("Nazwa drużyny", teamName);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetInt("KIA.WiRR.MissionGroup", group);
                // Save the draft even when temporarily incomplete during typing.
                EditorPrefs.SetString("KIA.WiRR.TeamName", teamName);
            }
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Losuj nazwę"))
                {
                    teamName = Prefixes[Random.Next(Prefixes.Length)] + " " +
                        Subjects[Random.Next(Subjects.Length)];
                    EditorPrefs.SetString("KIA.WiRR.TeamName", teamName);
                }
                using (new EditorGUI.DisabledScope(group == 0 || !IsValid(teamName)))
                {
                    if (GUILayout.Button("Kopiuj propozycję"))
                        EditorGUIUtility.systemCopyBuffer = Groups[group] + ": " + teamName.Trim();
                }
            }
            if (!string.IsNullOrEmpty(teamName) && !IsValid(teamName))
                EditorGUILayout.HelpBox("Użyj 3–48 znaków: liter, cyfr, spacji lub łącznika.", MessageType.Warning);
        }
    }
}
