using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;

namespace KIA.WiRR
{
    [Serializable]
    public sealed class WiRRLab08Trial
    {
        public string variant;
        public int attempt;
        public bool success;
        public float seconds;
        public string deviceMode;
    }

    [Serializable]
    public sealed class WiRRLab08Session
    {
        public string schema = "wirr-lab08/1";
        public string scenario = "Wirtualna inspekcja trzech elementów";
        public string deviceMode = "UNITY_EDITOR";
        public string exportedAtUtc;
        public List<WiRRLab08Trial> trials = new List<WiRRLab08Trial>();
    }

    /// <summary>
    /// Works in a plain URP desktop scene without XR dependencies.
    /// Records actual local actions and elapsed time, never invented HMD data.
    /// The same mechanic can later be connected to XRI or AR raycasts.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class WiRRLab08Experiment : MonoBehaviour
    {
        [SerializeField] private string scenario = "Inspekcja stanowiska: 3 elementy";
        [SerializeField] private string deviceMode = "UNITY_EDITOR";
        [SerializeField] private Transform[] inspectableTargets = new Transform[3];

        private readonly HashSet<int> selected = new HashSet<int>();
        private WiRRLab08Session session = new WiRRLab08Session();
        private bool activeTrial;
        private float trialStart;
        private string variant = "A";
        private string feedback = "Naciśnij Rozpocznij próbę. Dodatkowe SDK nie są wymagane.";
        public const int MaxPerVariant = 3;
        public bool IsActiveTrial => activeTrial;
        public string LatestPath => Path.Combine(Application.persistentDataPath, "WiRR-Lab08-results.json");

        public void Configure(params Transform[] targets)
        {
            inspectableTargets = targets ?? new Transform[3];
        }

        private void Awake()
        {
            session = new WiRRLab08Session
            {
                scenario = scenario,
                deviceMode = deviceMode,
                trials = new List<WiRRLab08Trial>()
            };
        }

        private int CountFor(string label) => session.trials.Count(t => t.variant == label);
        private int SuccessesFor(string label) => session.trials.Count(t => t.variant == label && t.success);

        public void ChooseVariant(string label)
        {
            if (activeTrial) return;
            if (label != "A" && label != "B") return;
            variant = label;
            feedback = label == "A" ? "Wariant bazowy, bez poprawki." : "Wariant B: po zmianie tylko jednego parametru.";
        }

        public void BeginTrial()
        {
            if (activeTrial || CountFor(variant) >= MaxPerVariant)
            {
                feedback = "Ta seria ma już trzy próby albo próba nadal trwa.";
                return;
            }
            activeTrial = true;
            selected.Clear();
            trialStart = Time.unscaledTime;
            feedback = "Dotknij trzech punktów w dowolnej kolejności. Możesz też użyć przycisków Game View.";
        }

        public void Inspect(int index)
        {
            if (!activeTrial || index < 0 || index > 2 || selected.Contains(index)) return;
            selected.Add(index);
            feedback = $"Sprawdzono elementy: {selected.Count}/3.";
            if (selected.Count == 3) Finish(true);
        }

        public void CancelTrial()
        {
            if (!activeTrial) return;
            Finish(false);
        }

        private void Finish(bool success)
        {
            var seconds = Mathf.Max(0f, Time.unscaledTime - trialStart);
            activeTrial = false;
            session.trials.Add(new WiRRLab08Trial
            {
                variant = variant,
                attempt = CountFor(variant) + 1,
                seconds = seconds,
                success = success,
                deviceMode = deviceMode
            });
            feedback = success ? $"Sukces, czas {seconds.ToString("F2", CultureInfo.InvariantCulture)} s." :
                "Próba nieudana. Nie wpisujemy braków jako zero.";
            ExportResults();  // Every real completed trial automatically refreshes the local JSON.
        }

        public void ExportResults()
        {
            session.exportedAtUtc = DateTime.UtcNow.ToString("O");
            try
            {
                Directory.CreateDirectory(Application.persistentDataPath);
                File.WriteAllText(LatestPath, JsonUtility.ToJson(session, true));
                Debug.Log("[WiRR Lab08] Zapisano lokalny JSON: " + LatestPath);
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                Debug.LogError("[WiRR Lab08] Nie można zapisać JSON: " + ex.Message);
            }
        }

        private static string Median(IEnumerable<float> values)
        {
            var sorted = values.OrderBy(v => v).ToArray();
            if (sorted.Length == 0) return "brak danych";
            var n = sorted.Length;
            var median = n % 2 == 1 ? sorted[n / 2] : (sorted[n / 2 - 1] + sorted[n / 2]) / 2f;
            return median.ToString("F2", CultureInfo.InvariantCulture) + " s";
        }

        private void Update()
        {
            if (!activeTrial) return;
            // Raycast is a desktop fallback; buttons remain usable without camera/input mapping.
            if (UnityEngine.InputSystem.Mouse.current == null ||
                !UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame)
                return;
            if (Camera.main == null) return;
            var ray = Camera.main.ScreenPointToRay(UnityEngine.InputSystem.Mouse.current.position.ReadValue());
            if (!Physics.Raycast(ray, out var hit, 100f)) return;
            for (var i = 0; i < inspectableTargets.Length && i < 3; i++)
                if (inspectableTargets[i] != null &&
                    (hit.transform == inspectableTargets[i] || hit.transform.IsChildOf(inspectableTargets[i])))
                {
                    Inspect(i);
                    break;
                }
        }

        private void OnGUI()
        {
            // IMGUI is deliberately self-contained: no prefabs, TextMeshPro or XRI to install.
            GUILayout.BeginArea(new Rect(12, 12, 465, 340), GUI.skin.box);
            GUILayout.Label("WiRR LAB 08 / WYNALAZCA XR");
            GUILayout.Label("Scenariusz: " + scenario);
            GUILayout.Label("Tryb danych: " + deviceMode + " (deklaracja konfiguracji, nie detekcja HMD)");
            GUILayout.Label("Wariant A: " + CountFor("A") + "/3, sukcesy: " + SuccessesFor("A"));
            GUILayout.Label("Wariant B: " + CountFor("B") + "/3, sukcesy: " + SuccessesFor("B"));
            GUILayout.Label("Mediana A: " + Median(session.trials.Where(t => t.variant == "A" && t.success).Select(t => t.seconds)));
            GUILayout.Label("Mediana B: " + Median(session.trials.Where(t => t.variant == "B" && t.success).Select(t => t.seconds)));
            GUILayout.Label(feedback);
            if (!activeTrial)
            {
                using (new GUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Wariant A")) ChooseVariant("A");
                    if (GUILayout.Button("Wariant B")) ChooseVariant("B");
                }
                if (GUILayout.Button("Rozpocznij próbę " + variant))
                    BeginTrial();
            }
            else
            {
                using (new GUILayout.HorizontalScope())
                {
                    for (var i = 0; i < 3; i++)
                    {
                        var index = i;
                        GUI.enabled = !selected.Contains(index);
                        if (GUILayout.Button("Sprawdź " + (index + 1))) Inspect(index);
                    }
                    GUI.enabled = true;
                }
                if (GUILayout.Button("PRZERWIJ (wynik nieudany)")) CancelTrial();
            }
            if (GUILayout.Button("Eksportuj wyniki JSON")) ExportResults();
            GUILayout.Label("Raport lokalny: " + LatestPath, GUI.skin.label);
            GUILayout.EndArea();
        }
    }
}
