using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KIA.WiRR
{
    [Serializable] public sealed class WiRRStudentAction { public string title, body; }
    [Serializable] public sealed class WiRRStudentLesson
    {
        public int id;
        public string title, question, mode, concept, controls, invalid, conclusion;
        public WiRRStudentAction[] actions;
        public string[] rubric;
    }

    /// <summary>Desktop teaching kit. Evidence is provisional; no claim of XRI/HMD or hardware measurements.</summary>
    [DisallowMultipleComponent]
    public sealed partial class WiRRStudentLab : MonoBehaviour
    {
        [Range(1, 8)] public int labNumber = 1;
        private WiRRStudentLesson lesson;
        private WiRRStudentWorld world;
        private WiRRStudentSession session;
        private string index1 = "", index2 = "", group = "G1", savePath, feedback = "", participant = "SOLO";
        private Vector2 scroll;
        private int stage, detail, view, count = 10, technique, template;
        private bool highlight, largeTargets, demonstration, physicsLoad, shadows = true, active, practice, grabbed, paused;
        private float start, lastSample, lastMessage, staleAt = -1, pauseStart, watchdog = 0.5f, gap = 1.5f;
        private int errors, successes, messageIndex;
        private WiRRAnchorDataset anchorData;
        private WiRRAnchorExperiment anchorExperiment;
        private bool anchorRelocalize, anchorFailRecovery;
        private WiRRDepthExperiment depthExperiment;
        private WiRRDepthObservation[] depthObservations;
        private float depthTolerance;
        private bool depthDimLight, depthLabelFault = true, depthRayFault = true;
        private WiRRInteractionSeries interactionSeries;
        private readonly List<double> frames = new List<double>();
        private readonly List<double> intervals = new List<double>();
        private readonly List<int> lodSamples = new List<int>();
        private WiRRCadBenchmark cadBenchmark;
        private bool cadInspection;
        private int cadMaterials = 1;
        private bool automaticLod;
        private float lodNear = 6, lodFar = 10;
        private readonly HashSet<int> selected = new HashSet<int>();
        private Vector3 hand = new Vector3(-1, 1, 1);
        private GameObject handObject;
        private ReplayFrame[] replay;
        private float replayStart;
        private float PanelWidth => Mathf.Min(460, Screen.width * 0.48f);
        public bool IsTrialActive => active;
        public int RecordedTrials => session == null ? 0 : session.Lab(labNumber).trials.Count;

        private void Start() => OpenLesson(labNumber);

        public void OpenLesson(int number)
        {
            if (active) CancelTrial();
            if (session != null && (number + 1) / 2 != session.block)
            {
                if (!Save()) return;
                session = null; savePath = null;
            }
            labNumber = Mathf.Clamp(number, 1, 8);
            var asset = Resources.Load<TextAsset>("WiRR2/lab" + labNumber.ToString("00"));
            if (asset == null) { feedback = "Brak danych instrukcji. Zgłoś prowadzącemu."; enabled = false; return; }
            lesson = JsonUtility.FromJson<WiRRStudentLesson>(asset.text);
            stage = detail = view = template = technique = 0;
            participant = "SOLO";
            highlight = largeTargets = demonstration = physicsLoad = false; shadows = true; count = 10; paused = false;
            anchorRelocalize = anchorFailRecovery = false; anchorExperiment = null;
            depthTolerance = 0; depthDimLight = false; depthLabelFault = depthRayFault = true; depthExperiment = null;
            automaticLod = false; cadInspection = false; cadMaterials = 1; cadBenchmark = null; lodNear = 6; lodFar = 10;
            if (session != null) session.Lab(number);
            Rebuild();
        }

        private void Rebuild()
        {
            if (world != null) world.Dispose();
            world = new WiRRStudentWorld(transform, labNumber, count, detail, shadows, physicsLoad, highlight, view);
            if (largeTargets) foreach (var target in world.targets) target.transform.localScale *= 1.5f;
            hand = new Vector3(-1, 1, 1);
            if (labNumber == 2)
            {
                handObject = world.Shape("Ręka symulatora PC", PrimitiveType.Sphere, hand, Vector3.one * 0.12f, Color.magenta);
                handObject.SetActive(technique == 0);
            }
            if (labNumber == 5) world.camera.transform.position += Vector3.back * view * 4;
            if (labNumber == 5) { world.ConfigureCad(cadMaterials, cadInspection); ApplyCadLod(); }
            if (labNumber == 3)
            {
                var asset = Resources.Load<TextAsset>("WiRR2/anchor-tracking");
                if (asset == null) throw new InvalidOperationException("Brak modelu SIM_AR.");
                anchorData = JsonUtility.FromJson<WiRRAnchorDataset>(asset.text);
                world.SetAnchorObservation(0); world.SetAnchorPose(Vector3.zero, false);
            }
            if (labNumber == 4) depthObservations = world.ConfigureDepth(depthTolerance, depthDimLight, depthLabelFault, depthRayFault);
            if (labNumber == 6)
            {
                var asset = Resources.Load<TextAsset>("WiRR2/jointstate-replay");
                replay = ReadReplay(asset.text); replayStart = Time.unscaledTime; messageIndex = 0;
                lastMessage = Time.unscaledTime; staleAt = -1;
            }
        }

        private void Resume()
        {
            var indices = string.IsNullOrWhiteSpace(index2) ? new[] { index1.Trim() } : new[] { index1.Trim(), index2.Trim() };
            if (!WiRRStudentEvidence.ValidIndices(indices)) { feedback = "Podaj jeden lub dwa różne indeksy (5–12 cyfr)."; return; }
            var block = (labNumber + 1) / 2;
            savePath = Path.Combine(Application.persistentDataPath, "WiRR", "sessions",
                WiRRStudentEvidence.IdentityKey(indices, group, block) + ".json");
            try
            {
                session = File.Exists(savePath) ? ReadSession(savePath, indices, block) :
                    File.Exists(savePath + ".bak") ? ReadSession(savePath + ".bak", indices, block) :
                    new WiRRStudentSession { block = block, studentIndices = indices, group = group };
                session.Lab(block * 2 - 1); session.Lab(block * 2);
                var recovered = session.RecoverInterruptedTrial();
                if (Save()) feedback = recovered ? "Odzyskano rozpoczętą próbę jako nieudaną, bez wymyślonych pomiarów." :
                    "Sesja bloku otwarta. Zachowujemy wszystkie wcześniejsze próby.";
            }
            catch (Exception error) when (error is IOException || error is UnauthorizedAccessException || error is ArgumentException)
            {
                session = null;
                feedback = "Nie otwarto sesji: " + error.Message + ". Nie nadpisuj pliku; zgłoś prowadzącemu.";
            }
        }

        private WiRRStudentSession ReadSession(string path, string[] indices, int block)
        {
            var loaded = JsonUtility.FromJson<WiRRStudentSession>(File.ReadAllText(path));
            if (loaded == null || loaded.schema != "wirr-block-session/2.0-draft.1" || loaded.block != block || loaded.group != group
                || loaded.studentIndices == null || !loaded.studentIndices.OrderBy(x => x).SequenceEqual(indices.OrderBy(x => x))
                || loaded.labs == null || loaded.labs.Any(x => x == null || x.labNumber < 1 || x.labNumber > 8
                    || (x.labNumber + 1) / 2 != block || x.trials == null || x.trials.Any(t => t == null))
                || loaded.labs.Select(x => x.labNumber).Distinct().Count() != loaded.labs.Count)
                throw new ArgumentException("Plik nie odpowiada profilowi i blokowi");
            return loaded;
        }

        private bool Save()
        {
            if (session == null) return false;
            try { WiRRStudentEvidence.AtomicWrite(savePath, JsonUtility.ToJson(session, true)); return true; }
            catch (Exception error) when (error is IOException || error is UnauthorizedAccessException || error is NotSupportedException)
            { feedback = "Błąd zapisu: " + error.Message; Debug.LogError(feedback); return false; }
        }

        public void BeginTrial()
        {
            if (active || session == null) return;
            Rebuild();
            anchorExperiment = labNumber == 3 ? new WiRRAnchorExperiment(anchorData, detail, anchorRelocalize, anchorFailRecovery) : null;
            depthExperiment = labNumber == 4 ? new WiRRDepthExperiment(depthObservations) : null;
            cadBenchmark = labNumber == 5 ? new WiRRCadBenchmark() : null;
            initialConfiguration = Configuration();
            if (!practice)
            {
                session.BeginTrial(labNumber, lesson.mode, participant, initialConfiguration);
                if (!Save()) { session.pendingTrial = null; return; }
            }
            active = true; start = lastSample = Time.unscaledTime; errors = successes = 0;
            selected.Clear(); frames.Clear(); intervals.Clear(); lodSamples.Clear();
            grabbed = paused = false; staleDelay = recoveryDelay = -1;
            interactionSeries = labNumber == 2 ? new WiRRInteractionSeries(start) : null;
            if (labNumber == 6) { messageIndex = 0; replayStart = lastMessage = start; staleAt = -1; }
            feedback = "Próba trwa. Zmiany konfiguracji są zablokowane do zakończenia.";
        }

        public void CancelTrial()
        {
            if (active) Finish(false, "Próba przerwana przez użytkownika lub zmianę sceny.");
        }

        private string Configuration() => string.Format(CultureInfo.InvariantCulture,
            "variant={0}; view={1}; objects={2}; shadows={3}; physics={4}; highlight={5}; technique={6}; template={7}; watchdog_s={8}; gap_s={9}; training={10}; resolution={11}x{12}; vsync={13}; fps_limit={14}; seed=2718; environment={15}; large_targets={16}; demonstration={17}; auto_distance_lod={18}; lod_near_m={19}; lod_far_m={20}; camera_path={21}; input_technique={22}; cycles_required={23}; cycle_timeout_s={24}; anchor_relocalize={25}; anchor_fail_recovery={26}; anchor_model={27}; depth_tolerance_m={28}; dim_light={29}; label_fault={30}; ray_claim_fault={31}; depth_source={32}; cad_materials={33}; cad_source=procedural-ring/v1",
            detail, view, count, shadows, physicsLoad, highlight, technique, template, watchdog, gap, practice,
            Screen.width, Screen.height, QualitySettings.vSyncCount, Application.targetFrameRate, Application.isEditor ? "Editor" : "Player", largeTargets, demonstration,
            automaticLod, lodNear, lodFar, labNumber == 5 ? "radial3-15/v1" : labNumber == 3 ? "anchor-three-points/v1" : labNumber == 4 ? "depth-two-views/v1" : "baseline/v1",
            labNumber == 2 ? (technique == 0 ? "Direct_PC" : "Ray_PC") : "not_applicable", labNumber == 2 ? 5 : 0, labNumber == 2 ? 30 : 0,
            anchorRelocalize, anchorFailRecovery, labNumber == 3 ? anchorData.version : "not_applicable",
            depthTolerance, depthDimLight, depthLabelFault, depthRayFault, labNumber == 4 ? "SYNTHETIC_GEOMETRY_CENTER_RAY" : "not_applicable", cadMaterials);

        private void Finish(bool valid, string reason = null)
        {
            if (!active) return;
            if (labNumber == 2 && interactionSeries != null)
            {
                if (!interactionSeries.IsComplete) interactionSeries.Abort(Time.unscaledTime);
                errors = interactionSeries.TotalErrors; successes = interactionSeries.Successes;
            }
            if (labNumber == 3 && anchorExperiment != null)
            {
                successes = anchorExperiment.Phase == WiRRAnchorPhase.Complete ? 1 : 0;
                if (valid && successes == 0) { valid = false; reason = "Nie ukończono poprawnych odczytów SIM_AR."; }
            }
            if (labNumber == 4 && depthExperiment != null)
            {
                successes = depthExperiment.Correct; errors = depthExperiment.FalseAlarms + depthExperiment.MissedVisible;
                if (valid && (!depthExperiment.IsComplete || depthExperiment.HasMissingSource))
                { valid = false; reason = "Brak odpowiedzi lub poprawnego trafienia referencyjnego SIM_AR."; }
            }
            active = false;
            var trial = new WiRRStudentTrial { deviceMode = lesson.mode, condition = Configuration(),
                participantMode = participant, validity = valid ? "VALID" : "FAILED", reason = reason };
            trial.metrics.Add(WiRRStudentMetric.Measured("elapsed", Time.unscaledTime - start, "s"));
            trial.metrics.Add(WiRRStudentMetric.Measured("errors", errors, "count"));
            trial.metrics.Add(WiRRStudentMetric.Measured("successes", successes, "count"));
            if (labNumber == 2 && interactionSeries != null) trial.metrics.AddRange(interactionSeries.Metrics());
            if (labNumber == 1 || labNumber == 5)
            {
                if (frames.Count < 30) { trial.validity = "FAILED"; trial.reason = "Za mało próbek czasu klatki (minimum 30)."; }
                else
                {
                    trial.metrics.Add(WiRRStudentMetric.Measured("frame_median", WiRRStudentEvidence.Percentile(frames, 0.5), "ms"));
                    trial.metrics.Add(WiRRStudentMetric.Measured("frame_p95", WiRRStudentEvidence.Percentile(frames, 0.95), "ms"));
                    trial.metrics.Add(WiRRStudentMetric.Measured("frame_samples", frames.Count, "count"));
                }
                if (labNumber == 5)
                {
                    trial.metrics.AddRange(cadBenchmark.Metrics());
                    if (lodSamples.Count > 0)
                    {
                        trial.metrics.Add(WiRRStudentMetric.Measured("lod_level_min", lodSamples.Min(), "index"));
                        trial.metrics.Add(WiRRStudentMetric.Measured("lod_level_max", lodSamples.Max(), "index"));
                        trial.metrics.Add(WiRRStudentMetric.Measured("lod_transitions", lodSamples.Skip(1).Where((level, i) => level != lodSamples[i]).Count(), "count"));
                        trial.metrics.Add(WiRRStudentMetric.Measured("hole_preserved_all_samples", lodSamples.All(level => level < 2) ? 1 : 0, "boolean"));
                    }
                    trial.metrics.Add(WiRRStudentMetric.Measured("triangles_final", world.ActiveTriangleCount, "count"));
                    trial.metrics.Add(WiRRStudentMetric.Measured("hole_preserved_final", world.ActiveLod < 2 ? 1 : 0, "boolean"));
                }
            }
            if (labNumber == 3 && anchorExperiment != null) trial.metrics.AddRange(anchorExperiment.Metrics());
            if (labNumber == 4 && depthExperiment != null) trial.metrics.AddRange(depthExperiment.Metrics());
            if (labNumber == 6)
            {
                if (intervals.Count > 0) trial.metrics.Add(WiRRStudentMetric.Measured("interarrival_median", WiRRStudentEvidence.Percentile(intervals, 0.5), "ms"));
                trial.metrics.Add(staleDelay >= 0 ? WiRRStudentMetric.Measured("last_message_to_stale", staleDelay, "s") :
                    new WiRRStudentMetric { name = "last_message_to_stale", unit = "s", unavailableReason = "Nie zaobserwowano STALE" });
                trial.metrics.Add(new WiRRStudentMetric { name = "rtt", unit = "ms", unavailableReason = "Jednokierunkowy REPLAY nie mierzy RTT" });
            }
            if (labNumber == 6)
                trial.metrics.Add(recoveryDelay >= 0 ? WiRRStudentMetric.Measured("simulated_gap_duration", recoveryDelay, "s") :
                    new WiRRStudentMetric { name = "simulated_gap_duration", unit = "s", unavailableReason = "Brak zakończonej przerwy w próbie" });
            if (!practice) { session.CompleteTrial(labNumber, trial); if (!Save()) return; }
            feedback = practice ? "Trening zakończony. Nie wchodzi do porównania." :
                "Zapisano próbę " + trial.validity + ". To dowód do przeglądu, nie automatyczna ocena.";
        }

        private void Export()
        {
            if (session == null || active) return;
            if (!Save()) return;
            try
            {
                var folder = Path.Combine(Application.persistentDataPath, "WiRR", "exports", session.id);
                var names = new List<string>();
                foreach (var lab in session.labs.OrderBy(x => x.labNumber))
                {
                    var name = "WiRR-lab" + lab.labNumber.ToString("00") + ".json";
                    WiRRStudentEvidence.AtomicWrite(Path.Combine(folder, name),
                        WiRRStudentEvidence.Json(WiRRStudentEvidence.Report(session, lab.labNumber, Application.unityVersion)));
                    names.Add(name);
                }
                WiRRStudentEvidence.AtomicWrite(Path.Combine(folder, "block.json"), WiRRStudentEvidence.Json(new Dictionary<string, object> {
                    { "schemaVersion", "wirr-block-export/2.0-draft.1" }, { "sessionId", session.id },
                    { "reports", names }, { "deliveryStatus", "LOCAL_ONLY" } }));
                feedback = "Zapisano dwa wyniki lokalnie: " + folder + ". Przekaż folder kanałem wskazanym przez prowadzącego.";
                Debug.Log("[WiRR] Eksport lokalny: " + folder);
            }
            catch (Exception error) when (error is IOException || error is UnauthorizedAccessException || error is NotSupportedException)
            { feedback = "Eksport nieukończony: " + error.Message; }
        }

        private void OnApplicationPause(bool pause) { if (pause && active) CancelTrial(); }
        private void OnApplicationFocus(bool focus) { if (!focus && active) CancelTrial(); }
        private void OnDisable() { if (active) CancelTrial(); Save(); }
        private void OnDestroy() { if (world != null) world.Dispose(); }

        private void OnGUI()
        {
            if (lesson == null) return;
            GUILayout.BeginArea(new Rect(8, 8, PanelWidth - 16, Screen.height - 16), GUI.skin.box);
            scroll = GUILayout.BeginScrollView(scroll);
            GUILayout.Label("WiRR | " + lesson.title);
            GUILayout.Label(lesson.mode + " | " + WiRRStudentEvidence.ExperimentVersion + " | brak automatycznej oceny");
            GUILayout.Label(lesson.question);
            GUI.enabled = !active;
            if (session == null)
            {
                GUILayout.Label("Profil na dwa laboratoria bloku");
                index1 = GUILayout.TextField(index1); GUILayout.Label("Drugi indeks tylko przy pracy w parze");
                index2 = GUILayout.TextField(index2);
                group = GUILayout.Toolbar(group == "G1" ? 0 : 1, new[] { "G1", "G2" }) == 0 ? "G1" : "G2";
                if (GUILayout.Button("Rozpocznij / wznów sesję")) Resume();
            }
            else
            {
                GUILayout.Label("Blok " + session.block + " | " + string.Join(", ", session.studentIndices) + " | " + session.group);
                if (GUILayout.Button("Zakończ profil na stanowisku") && Save()) { session = null; index1 = index2 = ""; }
            }
            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Poprzedni lab")) OpenLesson(labNumber == 1 ? 8 : labNumber - 1);
                if (GUILayout.Button("Następny lab")) OpenLesson(labNumber == 8 ? 1 : labNumber + 1);
            }
            stage = GUILayout.Toolbar(stage, new[] { "3.0", "3.5", "4.0", "4.5", "5.0" });
            GUILayout.Label("Cel etapu (do zatwierdzenia): " + lesson.rubric[stage]);
            var action = lesson.actions[stage == 0 ? 0 : stage <= 2 ? 1 : 2];
            GUILayout.Label(action.title); GUILayout.Label(action.body);
            DrawControls();
            practice = GUILayout.Toggle(practice, "Trening (bez zapisu do porównania)");
            GUI.enabled = session != null && !active;
            if (GUILayout.Button("Rozpocznij próbę")) BeginTrial();
            GUI.enabled = true;
            if (active && GUILayout.Button("Przerwij i zachowaj próbę nieudaną")) CancelTrial();
            GUILayout.Label(feedback);
            DrawLiveTask();
            if (session != null)
            {
                GUILayout.Label("Wyniki tego laboratorium: " + RecordedTrials);
                foreach (var trial in session.Lab(labNumber).trials.Skip(Math.Max(0, RecordedTrials - 8)))
                {
                    GUILayout.Label(trial.timestampUtc + " " + trial.validity);
                    GUILayout.Label(trial.condition);
                    GUILayout.Label(string.Join(" | ", trial.metrics.Select(m => m.name + ": " + (m.available ? m.value.ToString("F3", CultureInfo.InvariantCulture) : "brak") + " " + m.unit)));
                }
                GUILayout.Label("Twój wniosek (jeden na laboratorium)");
                var result = session.Lab(labNumber);
                var text = GUILayout.TextArea(result.conclusion ?? "", GUILayout.MinHeight(50));
                if (text != result.conclusion) { result.conclusion = text; Save(); }
                GUI.enabled = !active;
                if (GUILayout.Button("Eksportuj dwa wyniki bloku")) Export();
                GUI.enabled = true;
            }
            GUILayout.Label("Warunki: " + lesson.controls);
            GUILayout.Label("Próba nieważna: " + lesson.invalid);
            GUILayout.EndScrollView(); GUILayout.EndArea();
        }
    }
}
