using System;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace KIA.WiRR.Editor
{
    // Local identity. Remote group membership requires instructor-controlled verification.
    internal static class WiRRStudentProfile
    {
        private static string Key(string suffix) =>
            "KIA.WiRR.Student." + Hash128.Compute(Application.dataPath) + "." + suffix;
        public static bool IsRegistered => !string.IsNullOrEmpty(EditorPrefs.GetString(Key("Index"), ""));
        public static string Index => EditorPrefs.GetString(Key("Index"), "");
        private static string GlobalGroupKey(string index) => "KIA.WiRR.Enrollment." + Hash(index) + ".Group";
        private static string Hash(string value)
        {
            using (var sha = SHA256.Create())
                return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(value))).Replace("-", "").ToLowerInvariant();
        }
        public static int Group => IsRegistered ? EditorPrefs.GetInt(GlobalGroupKey(Index), EditorPrefs.GetInt(Key("Group"), 0)) : 0;
        public static string GroupLabel => Group == 1 ? "G1" : Group == 2 ? "G2" : "Nieprzypisana";
        public static string Identifier
        {
            get
            {
                if (!IsRegistered) return "";
                var key = "KIA.WiRR.Enrollment." + Hash(Index) + ".Id";
                var id = EditorPrefs.GetString(key, "");
                if (string.IsNullOrEmpty(id))
                {
                    id = Guid.NewGuid().ToString("N");
                    EditorPrefs.SetString(key, id);
                }
                return id;
            }
        }

        public static bool TryRegister(string index, int group, out string error)
        {
            index = (index ?? "").Trim();
            if (index.Length < 5 || index.Length > 12 || !System.Linq.Enumerable.All(index, char.IsDigit))
            {
                error = "Numer indeksu musi mieć 5–12 cyfr.";
                return false;
            }
            if (group != 1 && group != 2)
            {
                error = "Wybierz grupę G1 albo G2.";
                return false;
            }
            var existing = EditorPrefs.GetInt(GlobalGroupKey(index), 0);
            if (existing != 0 && existing != group)
            {
                error = "Ten indeks ma już przypisaną inną grupę na tym stanowisku.";
                return false;
            }
            if (IsRegistered)
            {
                error = "Profil jest już przypisany. Zmiana grupy wymaga prowadzącego.";
                return false;
            }
            EditorPrefs.SetInt(GlobalGroupKey(index), group);
            EditorPrefs.SetString(Key("Index"), index);
            EditorPrefs.SetInt(Key("Group"), group);
            EditorPrefs.SetInt("KIA.WiRR.MissionGroup", group);
            error = null;
            return true;
        }
    }

    [InitializeOnLoad]
    internal static class WiRRStudentProfilePrompt
    {
        private const string PromptKey = "KIA.WiRR.Student.ProfilePromptShown";
        static WiRRStudentProfilePrompt() => EditorApplication.update += PromptWhenReady;

        private static void PromptWhenReady()
        {
            if (Application.isBatchMode || EditorApplication.isCompiling || EditorApplication.isUpdating ||
                EditorApplication.isPlayingOrWillChangePlaymode) return;
            EditorApplication.update -= PromptWhenReady;
            if (WiRRStudentProfile.IsRegistered || SessionState.GetBool(PromptKey, false)) return;
            SessionState.SetBool(PromptKey, true);
            WiRRStudentProfileWindow.Open();
        }
    }

    public sealed class WiRRStudentProfileWindow : EditorWindow
    {
        private string index = "";
        private int group;
        [MenuItem("WiRR/Student/Profil i grupa", priority = 11)]
        public static void Open()
        {
            var window = GetWindow<WiRRStudentProfileWindow>("Profil WiRR");
            window.minSize = new Vector2(400, 230);
            window.Show();
        }
        private void OnGUI()
        {
            EditorGUILayout.LabelField("Profil laboratoryjny", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Wpisz własny numer indeksu i grupę z planu zajęć. Grupa zostanie zablokowana w tym projekcie. Profil pozostaje lokalny.", MessageType.Info);
            if (WiRRStudentProfile.IsRegistered)
            {
                EditorGUILayout.LabelField("Numer indeksu", WiRRStudentProfile.Index);
                EditorGUILayout.LabelField("Grupa", WiRRStudentProfile.GroupLabel);
                EditorGUILayout.HelpBox("Przydział grupy jest stały. Korekta wymaga weryfikacji przez prowadzącego w systemie uczelnianym.", MessageType.None);
                return;
            }
            index = EditorGUILayout.TextField("Numer indeksu", index);
            group = EditorGUILayout.Popup("Grupa", group, new[] { "Wybierz", "G1", "G2" });
            if (GUILayout.Button("Zapisz profil i przypisz grupę"))
            {
                if (WiRRStudentProfile.TryRegister(index, group, out var error))
                {
                    WiRRProgressTracker.Refresh();
                    Close();
                    WiRRMissionBoardWindow.Open();
                }
                else EditorUtility.DisplayDialog("Profil WiRR", error, "OK");
            }
        }
    }
}
