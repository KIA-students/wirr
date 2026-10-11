using System;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

namespace KIA.WiRR
{
    /// <summary>
    /// Optional ten-trial technical watchdog demonstration. Signals are not
    /// health measurements, fall detection or tests of simulator-sickness tolerance.
    /// Only supported XR features are trusted; manual stop always remains available.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class WiRRComfortSafetyGate : MonoBehaviour
    {
        private const int MaxTrials = 10;
        private const float TrialSeconds = 8f;

        [SerializeField] private WiRRVisualMotionStudy visualMotion;
        [SerializeField] private WiRRDepthCueStudy depthCues;
        [SerializeField] private string group = "G1";
        [SerializeField, Range(1.5f, 4f)]
        [Tooltip("Heuristic for technical tracking jumps, NOT a fall detector.")]
        private float positionJumpSpeed = 2.5f;

        private bool running;
        private bool sawPresence;
        private bool sawTracking;
        private bool sawValidHead;
        private bool hasPreviousPosition;
        private Vector3 previousPosition;
        private float lastPositionTime;
        private float startedAt;
        private float lostPresenceAt = -1f;
        private float lostTrackingAt = -1f;
        private int attempts;
        private int autoStops;
        private int simulatedStops;
        private string lastReason = "NONE";
        private string sensorStatus = "UNSUPPORTED / NO HEAD DEVICE";

        [Serializable]
        private sealed class SafetyReport
        {
            public string schema = "wirr-safety-watchdog/1";
            public string groupId;
            public int attemptedTests;
            public int maximumTests = MaxTrials;
            public int detectedStops;
            public int simulatedStops;
            public string lastSignal;
            public string sensorStatus;
            public string exportedAtUtc;
            public string limitation =
                "This is a technical signal log. It does not diagnose falls, nausea or user health.";
        }

        public int Attempts => attempts;
        public int Remaining => Mathf.Max(0, MaxTrials - attempts);
        public bool IsRunning => running;
        public string LastReason => lastReason;

        private string Key(string suffix) => "WiRR.ComfortWatchdog." + group + "." + suffix;

        private void Awake() => LoadCounts();

        public void Configure(WiRRVisualMotionStudy motion, WiRRDepthCueStudy depth)
        {
            visualMotion = motion;
            depthCues = depth;
        }

        private void LoadCounts()
        {
            attempts = Mathf.Clamp(PlayerPrefs.GetInt(Key("Attempts"), 0), 0, MaxTrials);
            autoStops = Mathf.Clamp(PlayerPrefs.GetInt(Key("Stops"), 0), 0, MaxTrials);
            simulatedStops = Mathf.Clamp(PlayerPrefs.GetInt(Key("Simulated"), 0), 0, MaxTrials);
        }

        private void ChooseGroup(string id)
        {
            if (running || (id != "G1" && id != "G2")) return;
            group = id;
            LoadCounts();
        }

        [ContextMenu("Rozpocznij probe watchdog")]
        public void BeginTrial()
        {
            if (running || attempts >= MaxTrials) return;
            attempts++;
            PlayerPrefs.SetInt(Key("Attempts"), attempts);
            PlayerPrefs.Save();

            running = true;
            startedAt = Time.unscaledTime;
            sawPresence = false;
            sawTracking = false;
            sawValidHead = false;
            hasPreviousPosition = false;
            lostPresenceAt = -1f;
            lostTrackingAt = -1f;
            lastReason = "RUNNING";
            sensorStatus = "CHECKING";
            Debug.Log("[WiRR][Safety] Technical trial " + attempts + "/" + MaxTrials);
        }

        [ContextMenu("STOP: przerwij")]
        public void StopStudy() => Finish("MANUAL_STOP", false, false);

        [ContextMenu("Symuluj zdjecie gogli")]
        public void SimulateRemoval() => Finish("SIMULATED_REMOVAL", false, true);

        [ContextMenu("Symuluj brak sledzenia")]
        public void SimulateTrackingLoss() => Finish("SIMULATED_TRACKING_LOSS", false, true);

        [ContextMenu("Symuluj skok pozycji")]
        public void SimulateMotionSpike() => Finish("SIMULATED_POSITION_SPIKE", false, true);

        private void Finish(string reason, bool detected, bool simulated)
        {
            if (!running) return;
            running = false;
            lastReason = reason;
            if (detected)
            {
                autoStops++;
                PlayerPrefs.SetInt(Key("Stops"), autoStops);
            }
            if (simulated)
            {
                simulatedStops++;
                PlayerPrefs.SetInt(Key("Simulated"), simulatedStops);
            }
            PlayerPrefs.Save();
            if (visualMotion != null) visualMotion.StopStudy();
            if (depthCues != null) depthCues.StopStudy();
            Debug.Log("[WiRR][Safety] Technical trial stopped: " + reason);
        }

        private void Update()
        {
            if (Keyboard.current != null)
            {
                if (Keyboard.current.escapeKey.wasPressedThisFrame) StopStudy();
                if (Keyboard.current.spaceKey.wasPressedThisFrame && !running) BeginTrial();
            }
            if (Gamepad.current != null)
            {
                if (Gamepad.current.buttonEast.wasPressedThisFrame) StopStudy();
                if (Gamepad.current.buttonSouth.wasPressedThisFrame && !running) BeginTrial();
            }
            if (!running) return;
            var elapsed = Time.unscaledTime - startedAt;
            if (elapsed >= TrialSeconds)
            {
                Finish("TIME_LIMIT", false, false);
                return;
            }

            var head = InputDevices.GetDeviceAtXRNode(XRNode.Head);
            if (!head.isValid)
            {
                sensorStatus = "NO_VALID_HMD: use simulated buttons";
                // Only an HMD that was valid earlier in the SAME trial can be
                // considered disconnected. An editor without HMD is unsupported.
                if (sawValidHead) Finish("HEAD_DEVICE_DISCONNECTED", true, false);
                return;
            }
            sawValidHead = true;

            bool present;
            if (head.TryGetFeatureValue(CommonUsages.userPresence, out present))
            {
                sensorStatus = "USER_PRESENCE_AVAILABLE";
                if (present)
                {
                    sawPresence = true;
                    lostPresenceAt = -1f;
                }
                else if (sawPresence)
                {
                    if (lostPresenceAt < 0f) lostPresenceAt = Time.unscaledTime;
                    if (Time.unscaledTime - lostPresenceAt >= 0.2f)
                    {
                        Finish("HMD_PRESENCE_LOST", true, false);
                        return;
                    }
                }
            }
            else
            {
                sensorStatus = "USER_PRESENCE_UNSUPPORTED";
            }

            bool tracked;
            if (head.TryGetFeatureValue(CommonUsages.isTracked, out tracked))
            {
                if (tracked)
                {
                    sawTracking = true;
                    lostTrackingAt = -1f;
                }
                else if (sawTracking)
                {
                    if (lostTrackingAt < 0f) lostTrackingAt = Time.unscaledTime;
                    if (Time.unscaledTime - lostTrackingAt >= 0.2f)
                    {
                        Finish("TRACKING_LOST", true, false);
                        return;
                    }
                }
            }

            Vector3 current;
            if (head.TryGetFeatureValue(CommonUsages.devicePosition, out current))
            {
                var now = Time.unscaledTime;
                var dt = now - lastPositionTime;
                if (hasPreviousPosition && dt >= 0.02f && dt <= 0.2f)
                {
                    var distance = Vector3.Distance(current, previousPosition);
                    if (distance >= 0.2f && distance / dt > positionJumpSpeed)
                    {
                        // May be fast voluntary motion or a tracking discontinuity.
                        // Neither observation proves a fall or loss of balance.
                        Finish("POSITION_SPIKE_NOT_FALL_DETECTION", true, false);
                        return;
                    }
                }
                previousPosition = current;
                lastPositionTime = now;
                hasPreviousPosition = true;
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus) Finish("FOCUS_LOST", true, false);
        }

        private void OnApplicationPause(bool paused)
        {
            if (paused) Finish("APPLICATION_PAUSED", true, false);
        }

        private void OnDisable() => Finish("COMPONENT_DISABLED", false, false);

        [ContextMenu("Eksportuj lokalny raport JSON")]
        public void ExportSummary()
        {
            var report = new SafetyReport
            {
                groupId = group,
                attemptedTests = attempts,
                detectedStops = autoStops,
                simulatedStops = simulatedStops,
                lastSignal = lastReason,
                sensorStatus = sensorStatus,
                exportedAtUtc = DateTime.UtcNow.ToString("O")
            };
            var path = Path.Combine(Application.persistentDataPath,
                "WiRR-safety-" + group + ".json");
            try
            {
                Directory.CreateDirectory(Application.persistentDataPath);
                File.WriteAllText(path, JsonUtility.ToJson(report, true));
                Debug.Log("[WiRR][Safety] Local report: " + path);
            }
            catch (Exception error) when (error is IOException || error is UnauthorizedAccessException)
            {
                Debug.LogError("[WiRR][Safety] Could not export report: " + error.Message);
            }
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(18, 310, 440, 275), GUI.skin.box);
            GUILayout.Label("WiRR: BEZPIECZNY OPERATOR XR (TECHNICAL TEST)");
            GUILayout.Label("Grupa: " + group + " | Próby: " + attempts + "/" + MaxTrials);
            GUILayout.Label("Czujnik: " + sensorStatus);
            GUILayout.Label("Ostatni sygnał: " + lastReason);
            GUILayout.Label("To NIE jest detektor upadków lub objawów medycznych.");
            if (!running)
            {
                using (new GUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Wybierz G1")) ChooseGroup("G1");
                    if (GUILayout.Button("Wybierz G2")) ChooseGroup("G2");
                }
                if (Remaining > 0 && GUILayout.Button("Rozpocznij próbę (do 8 s)"))
                    BeginTrial();
            }
            else
            {
                if (GUILayout.Button("PRZERWIJ NATYCHMIAST")) StopStudy();
                using (new GUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Symuluj zdjęcie")) SimulateRemoval();
                    if (GUILayout.Button("Symuluj tracking")) SimulateTrackingLoss();
                    if (GUILayout.Button("Symuluj skok")) SimulateMotionSpike();
                }
            }
            if (GUILayout.Button("Eksportuj techniczny JSON")) ExportSummary();
            GUILayout.EndArea();
        }

        private void OnValidate() =>
            positionJumpSpeed = Mathf.Clamp(positionJumpSpeed, 1.5f, 4f);
    }
}
