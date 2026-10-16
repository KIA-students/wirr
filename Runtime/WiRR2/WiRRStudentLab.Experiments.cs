using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KIA.WiRR
{
    public sealed partial class WiRRStudentLab
    {
        private string initialConfiguration;
        private float staleDelay = -1, recoveryDelay = -1;

        public struct ReplayFrame
        {
            public float time, joint1, joint2;
        }

        public static ReplayFrame[] ReadReplay(string csv)
        {
            var lines = csv.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length < 2) throw new ArgumentException("Brak danych REPLAY.");
            var header = lines[0].Split(',');
            var t = Array.IndexOf(header, "time_s");
            var j1 = Array.IndexOf(header, "joint1_rad");
            var j2 = Array.IndexOf(header, "joint2_rad");
            if (t < 0 || j1 < 0 || j2 < 0) throw new ArgumentException("Nieznane nazwy kolumn REPLAY.");
            var frames = new List<ReplayFrame>();
            foreach (var line in lines.Skip(1))
            {
                var fields = line.Split(',');
                if (fields.Length != header.Length) throw new ArgumentException("Niepełna wiadomość REPLAY.");
                var frame = new ReplayFrame { time = float.Parse(fields[t], CultureInfo.InvariantCulture),
                    joint1 = float.Parse(fields[j1], CultureInfo.InvariantCulture), joint2 = float.Parse(fields[j2], CultureInfo.InvariantCulture) };
                if (new[] { frame.time, frame.joint1, frame.joint2 }.Any(x => float.IsNaN(x) || float.IsInfinity(x))
                    || frame.time < 0 || (frames.Count > 0 && frame.time <= frames[frames.Count - 1].time))
                    throw new ArgumentException("Nieprawidłowy czas lub położenie REPLAY.");
                frames.Add(frame);
            }
            return frames.ToArray();
        }

        private void DrawControls()
        {
            var changed = false;
            if (labNumber == 1)
            {
                var next = GUILayout.Toolbar(count == 10 ? 0 : count == 100 ? 1 : 2, new[] { "10 obiektów", "100 obiektów", "300 obiektów" });
                var number = next == 0 ? 10 : next == 1 ? 100 : 300;
                changed |= number != count; count = number;
                var nextShadows = GUILayout.Toggle(shadows, "Cienie"); changed |= shadows != nextShadows; shadows = nextShadows;
                var nextPhysics = GUILayout.Toggle(physicsLoad, "Fizyka: dynamiczne bryły"); changed |= nextPhysics != physicsLoad; physicsLoad = nextPhysics;
                GUILayout.Label("Pomiar: 2 s rozgrzewki + 8 s zapisu. Stała trasa kamery, mediana i p95 czasu klatki.");
            }
            if (labNumber == 2)
            {
                var next = GUILayout.Toolbar(technique, new[] { "Direct PC", "Ray PC" });
                changed |= next != technique; technique = next;
                GUILayout.Label("Direct PC: strzałki poruszają ręką, E chwyta w zasięgu, Q puszcza. Ray PC: kliknij obiekt, przeciągnij, puść w gnieździe.");
                GUILayout.Label("To symulacja desktopowa; interaktory i teleportacja XRI/HMD pozostają do integracji.");
                GUILayout.Label("Seria obejmuje pięć cykli; każdy ma limit 30 s. Błędne chwyty i upuszczenia pozostają zapisane.");
                using (new GUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Podejdź do lewego punktu")) PreviewTeleport(0);
                    if (GUILayout.Button("Do prawego punktu")) PreviewTeleport(1);
                    if (GUILayout.Button("Widok bazowy")) PreviewTeleport(2);
                }
                GUILayout.Label("Teleportacja PC jest eksploracją poza pomiarem. Rozpoczęcie serii przywraca kontrolowany widok.");
            }
            if (labNumber == 3)
            {
                var next = GUILayout.Toolbar(detail, new[] { "A: wiele cech", "B: mało cech" });
                changed |= next != detail; detail = next;
                GUILayout.Label("SIM_AR: znana referencja, stały seed 2718, trzy odczyty położenia co sekundę. Bez pomiaru telefonu.");
            }
            if (labNumber == 4)
            {
                var next = GUILayout.Toolbar(detail, new[] { "A: zła okluzja", "B: głębia zgodna" });
                changed |= next != detail; detail = next;
                GUILayout.Label("Wskaż dla każdego numeru, czy obiekt powinien być widoczny według geometrii. Podsumowanie pojawi się po trzech odpowiedziach.");
            }
            if (labNumber == 5)
            {
                var next = GUILayout.Toolbar(detail, new[] { "High", "Medium", "Low" });
                changed |= next != detail; detail = next;
                var automatic = GUILayout.Toggle(automaticLod, "Automatyczny LOD według odległości");
                changed |= automatic != automaticLod; automaticLod = automatic;
                if (automaticLod)
                {
                    GUILayout.Label("High → Medium: " + lodNear.ToString("F1") + " m");
                    var threshold = GUILayout.HorizontalSlider(lodNear, 3, 8);
                    changed |= threshold != lodNear; lodNear = threshold;
                    GUILayout.Label("Medium → Low: " + lodFar.ToString("F1") + " m");
                    threshold = GUILayout.HorizontalSlider(lodFar, 9, 14);
                    changed |= threshold != lodFar; lodFar = threshold;
                }
                GUILayout.Label("Próg dotyczy metrów od kamery do środka części. Trasa benchmarku jest jednakowa dla stałego i automatycznego LOD.");
                GUILayout.Label("Siatka dydaktyczna, nie konwersja pliku STEP. High/Medium zachowują otwór, Low go wypełnia.");
            }
            if (labNumber == 6)
            {
                var next = GUILayout.Toolbar(watchdog < 0.4f ? 0 : watchdog < 0.8f ? 1 : 2, new[] { "STALE 0,25 s", "0,5 s", "1 s" });
                watchdog = next == 0 ? 0.25f : next == 1 ? 0.5f : 1;
                next = GUILayout.Toolbar(gap < 2 ? 0 : 1, new[] { "Przerwa 1,5 s", "Przerwa 3 s" });
                gap = next == 0 ? 1.5f : 3;
                GUILayout.Label("Źródło: syntetyczny plik JointState. Nazwy kolumn są mapowane jawnie; plik nie pochodzi z robota.");
            }
            if (labNumber == 7 || labNumber == 8)
            {
                var next = GUILayout.Toolbar(template, labNumber == 7 ? new[] { "Manipulacja", "Nawigacja", "Odczyt stanu" } :
                    new[] { "Trenażer", "Inspekcja", "Stan bliźniaka SIM" });
                changed |= template != next; template = next;
                var mode = GUILayout.Toolbar(participant == "SOLO" ? 0 : participant == "HUMAN" ? 1 : 2,
                    new[] { "SOLO", "GUEST_HUMAN", "GUEST_SIM" });
                participant = mode == 0 ? "SOLO" : mode == 1 ? "HUMAN" : "GUEST_SIM";
                GUILayout.Label(participant == "GUEST_SIM" ? "GUEST_SIM: jawny scenariusz testowy, nie obserwacja człowieka." :
                    "Klikaj cele w kolejności 1–2–3. Błąd pozostaje zapisany. Nie podpowiadaj gościowi w trakcie próby.");
                if (labNumber == 8)
                {
                    var bigger = GUILayout.Toggle(largeTargets, "Moduł 2: większe cele");
                    changed |= bigger != largeTargets; largeTargets = bigger;
                    demonstration = GUILayout.Toggle(demonstration, "Pokaz 60–90 s (zakończ ręcznie po osiągnięciu celu)");
                }
            }
            if (labNumber == 2 || labNumber == 7 || labNumber == 8)
            {
                var next = GUILayout.Toggle(highlight, "Moduł 1: podświetlenie celu"); changed |= next != highlight; highlight = next;
            }
            if (labNumber != 6)
            {
                var next = GUILayout.Toolbar(view, new[] { "Warunek bazowy", "Drugi widok / dystans" });
                changed |= next != view; view = next;
            }
            if (changed) Rebuild();
        }

        private void Update()
        {
            if (world == null || lesson == null) return;
            world.camera.rect = new Rect(PanelWidth / Screen.width, 0, 1 - PanelWidth / Screen.width, 1);
            if (labNumber == 6) UpdateReplay();
            if (labNumber == 5 && !active) ApplyCadLod();
            if (!active) return;
            if (initialConfiguration != Configuration()) { Finish(false, "Zmieniły się warunki lub rozdzielczość w trakcie pomiaru."); return; }
            var elapsed = Time.unscaledTime - start;
            if (labNumber == 1 || labNumber == 5)
            {
                // Restart the camera phase at the beginning of the measured window.
                var phase = Mathf.Max(0, elapsed - 2);
                // Delta time describes the previous rendered frame, before changing LOD.
                if (elapsed >= 2 && Time.unscaledDeltaTime > 0)
                {
                    frames.Add(Time.unscaledDeltaTime * 1000.0);
                    if (labNumber == 5) lodSamples.Add(world.ActiveLod);
                }
                world.camera.transform.position = labNumber == 5 ?
                    new Vector3(0, 1.2f, 1 - (3 + Mathf.PingPong(phase * 3, 12) + view * 4)) :
                    new Vector3(Mathf.Sin(phase * 0.4f) * 1.5f, 2.2f, -6 - view * 4);
                world.camera.transform.LookAt(new Vector3(0, 1, 1));
                if (labNumber == 5) ApplyCadLod();
                if (elapsed >= 10) Finish(true);
            }
            else if (labNumber == 2) UpdateManipulation();
            else if (labNumber == 3)
            {
                if (!placed) return;
                if (!tracking)
                {
                    if (Time.unscaledTime >= recovery) { tracking = true; recoveryDelay = Time.unscaledTime - pauseStart; }
                    else return;
                }
                if (Time.unscaledTime - lastSample >= 1 && sampleIndex < 3)
                {
                    var random = new System.Random(2718 + sampleIndex);
                    var spread = detail == 0 ? 0.005f : 0.04f;
                    var offset = new Vector3((float)random.NextDouble() * spread, detail == 0 ? 0.005f : 0.04f,
                        (float)random.NextDouble() * spread);
                    world.targets[0].transform.position = world.anchorReference.position + offset;
                    positions.Add(world.targets[0].transform.position);
                    sampleIndex++; lastSample = Time.unscaledTime;
                }
                if (sampleIndex == 3) Finish(true);
            }
            else if (labNumber == 7 || labNumber == 8)
            {
                if (participant == "GUEST_SIM")
                {
                    // A deterministic functional scenario, never a model of human reaction time.
                    if (Time.unscaledTime - lastSample > 1)
                    {
                        TaskSelection(selected.Count); lastSample = Time.unscaledTime;
                    }
                }
                else if ((labNumber == 7 && template != 2) || (labNumber == 8 && template != 2))
                {
                    var mouse = Mouse.current;
                    if (mouse != null && mouse.leftButton.wasPressedThisFrame && mouse.position.ReadValue().x > PanelWidth)
                    {
                        if (Physics.Raycast(world.camera.ScreenPointToRay(mouse.position.ReadValue()), out var hit, 100))
                        {
                            var index = world.targets.FindIndex(x => x.transform == hit.transform);
                            if (index >= 0) TaskSelection(index); else errors++;
                        }
                        else errors++;
                    }
                }
            }
        }

        private void ApplyCadLod()
        {
            var distance = Vector3.Distance(world.camera.transform.position, world.lod.transform.position);
            world.SetLod(automaticLod ? WiRRStudentWorld.DistanceLod(distance, lodNear, lodFar) : detail);
        }

        private void TaskSelection(int index)
        {
            if (!active || index < 0 || index > 2) return;
            if (index != selected.Count) { errors++; return; }
            selected.Add(index); successes++;
            if (template == 1 && labNumber == 7)
            {
                world.camera.transform.position = world.targets[index].transform.position + new Vector3(0, 1, -2);
                world.camera.transform.LookAt(world.targets[Mathf.Min(2, index + 1)].transform);
            }
            if (selected.Count == 3 && !demonstration) Finish(true);
        }

        private void UpdateManipulation()
        {
            var keyboard = Keyboard.current; var mouse = Mouse.current;
            if ((technique == 0 && keyboard == null) || (technique == 1 && mouse == null))
            { Finish(false, "Brak urządzenia wejściowego wybranej techniki PC."); return; }
            if (interactionSeries.CurrentSeconds(Time.unscaledTime) >= 30) { CompleteManipulationCycle(false); return; }
            var target = world.targets[0].transform;
            if (technique == 0 && keyboard != null)
            {
                var dx = (keyboard.rightArrowKey.isPressed ? 1 : 0) - (keyboard.leftArrowKey.isPressed ? 1 : 0);
                var dy = (keyboard.upArrowKey.isPressed ? 1 : 0) - (keyboard.downArrowKey.isPressed ? 1 : 0);
                hand += new Vector3(dx, dy, 0) * Time.unscaledDeltaTime * 1.2f;
                handObject.transform.position = hand;
                if (keyboard.eKey.wasPressedThisFrame)
                {
                    if (Vector3.Distance(hand, target.position) < 0.35f) grabbed = true; else interactionSeries.Error();
                }
                if (grabbed) target.position = hand;
                if (keyboard.qKey.wasPressedThisFrame && grabbed) ReleaseObject();
            }
            else if (technique == 1 && mouse != null)
            {
                var ray = world.camera.ScreenPointToRay(mouse.position.ReadValue());
                if (mouse.leftButton.wasPressedThisFrame && mouse.position.ReadValue().x > PanelWidth)
                {
                    if (Physics.Raycast(ray, out var hit, 100) && hit.transform == target) grabbed = true;
                    else interactionSeries.Error();
                }
                if (grabbed && new Plane(Vector3.back, new Vector3(0, 0, 1)).Raycast(ray, out var distance))
                    target.position = ray.GetPoint(distance);
                if (grabbed && mouse.leftButton.wasReleasedThisFrame) ReleaseObject();
            }
        }

        private void ReleaseObject()
        {
            grabbed = false;
            if (Vector3.Distance(world.targets[0].transform.position, world.socket.position) <= 0.45f)
                CompleteManipulationCycle(true);
            else { interactionSeries.Error(); world.targets[0].transform.position = new Vector3(-1, 1, 1); }
        }

        private void CompleteManipulationCycle(bool success)
        {
            if (!active || interactionSeries == null) return;
            interactionSeries.CompleteCycle(Time.unscaledTime, success);
            errors = interactionSeries.TotalErrors; successes = interactionSeries.Successes;
            grabbed = false;
            hand = new Vector3(-1, 1, 1); handObject.transform.position = hand;
            world.targets[0].transform.position = hand;
            if (interactionSeries.IsComplete) Finish(true);
            else
            {
                if (!practice)
                {
                    session.pendingTrial.trial.metrics = interactionSeries.Metrics();
                    if (!Save()) { Finish(false, "Przerwano serię po błędzie zapisu cyklu."); return; }
                }
                feedback = "Cykl zapisany. Rozpocznij cykl " + interactionSeries.CurrentNumber + "/5; położenie startowe przywrócone.";
            }
        }

        public bool PreviewTeleport(int station)
        {
            if (labNumber != 2 || world == null || active) return false;
            world.TeleportPreview(station);
            feedback = "Teleportacja PC: eksploracja, nie część porównania Direct/Ray.";
            return true;
        }

        private void UpdateReplay()
        {
            var now = Time.unscaledTime;
            if (paused && now - pauseStart >= gap)
            {
                paused = false;
                recoveryDelay = now - pauseStart;
                // Freeze source time during a simulated delivery gap; do not flood old frames.
                replayStart += now - pauseStart;
            }
            if (!paused && replay != null && messageIndex < replay.Length && now - replayStart >= replay[messageIndex].time)
            {
                var frame = replay[messageIndex++];
                if (messageIndex > 1 && active) intervals.Add((now - lastMessage) * 1000.0);
                lastMessage = now;
                world.robotJoint1.localRotation = Quaternion.Euler(0, 0, frame.joint1 * Mathf.Rad2Deg);
                world.robotJoint2.localRotation = Quaternion.Euler(0, 0, frame.joint2 * Mathf.Rad2Deg);
            }
            if (!paused && replay != null && messageIndex == replay.Length) { messageIndex = 0; replayStart = now; }
            if (now - lastMessage > watchdog && staleAt < 0) { staleAt = now; staleDelay = now - lastMessage; }
        }

        private void DrawLiveTask()
        {
            if (!active) return;
            GUILayout.Label("Czas próby: " + (Time.unscaledTime - start).ToString("F1") + " s");
            if (labNumber == 2 && interactionSeries != null)
            {
                GUILayout.Label("Cykl " + interactionSeries.CurrentNumber + "/5 | sukcesy " + interactionSeries.Successes +
                    " | pomyłki " + interactionSeries.TotalErrors + " | czas cyklu " + interactionSeries.CurrentSeconds(Time.unscaledTime).ToString("F1") + " s");
                if (GUILayout.Button("Zakończ bieżący cykl jako nieudany")) CompleteManipulationCycle(false);
            }
            if (labNumber == 5)
                GUILayout.Label("LOD " + world.ActiveLod + " | " + world.ActiveTriangleCount + " trójkątów | otwór " +
                    (world.ActiveLod < 2 ? "zachowany" : "utracony"));
            if (labNumber == 3 && !placed && GUILayout.Button("Umieść model przy referencji"))
            { world.targets[0].transform.position = world.anchorReference.position; placed = true; lastSample = Time.unscaledTime; }
            if (labNumber == 3 && recovery < 0 && GUILayout.Button("Wywołaj utratę śledzenia SIM"))
            { tracking = false; pauseStart = Time.unscaledTime; recovery = pauseStart + 1.5f; }
            if (labNumber == 4)
            {
                for (var i = 0; i < 3; i++)
                {
                    if (selected.Contains(i)) continue;
                    GUILayout.Label("Obiekt " + (i + 1) + ": czy powinien być widoczny?");
                    using (new GUILayout.HorizontalScope())
                    {
                        if (GUILayout.Button("Tak")) ClassifyVisibility(i, true);
                        if (GUILayout.Button("Nie")) ClassifyVisibility(i, false);
                    }
                }
            }
            if (labNumber == 6)
            {
                GUILayout.Label("ROS_REPLAY | wiadomość " + messageIndex + " | wiek " + (Time.unscaledTime - lastMessage).ToString("F2") +
                    " s | " + (Time.unscaledTime - lastMessage > watchdog ? "STALE (stan zamrożony)" : "ŚWIEŻE dane replay"));
                if (!paused && GUILayout.Button("Wstrzymaj strumień i przywróć"))
                { paused = true; pauseStart = Time.unscaledTime; staleAt = -1; staleDelay = -1; }
                if (GUILayout.Button("Zapisz obserwację")) Finish(true);
            }
            if ((labNumber == 7 || labNumber == 8) && template == 2 && participant != "GUEST_SIM")
            {
                if (selected.Count < 3)
                {
                    // Three labeled source-age fixtures, independent of rendering frame rate.
                    var age = selected.Count == 1 ? 1.2 : 0.1;
                    GUILayout.Label("Stan testowy " + (selected.Count + 1) + ": wiek danych " + age + " s; próg 0,5 s");
                    using (new GUILayout.HorizontalScope())
                    {
                        if (GUILayout.Button("ŚWIEŻE")) { if (age <= 0.5) TaskSelection(selected.Count); else errors++; }
                        if (GUILayout.Button("STALE")) { if (age > 0.5) TaskSelection(selected.Count); else errors++; }
                    }
                }
            }
            if (labNumber == 8 && demonstration && selected.Count == 3 && GUILayout.Button("Zakończ pokaz"))
            {
                var duration = Time.unscaledTime - start;
                Finish(duration >= 60 && duration <= 90, duration < 60 || duration > 90 ? "Pokaz poza zakresem 60–90 sekund." : null);
            }
        }

        private void ClassifyVisibility(int index, bool visible)
        {
            if (!active || selected.Contains(index)) return;
            if (visible == !world.Occluded(index)) successes++; else errors++;
            selected.Add(index);
            if (selected.Count == 3) Finish(true);
        }
    }
}
