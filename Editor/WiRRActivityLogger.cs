using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using KIA.WiRR;
using UnityEditor;
using UnityEngine;

namespace KIA.WiRR.Editor
{
    [InitializeOnLoad]
    internal static class WiRRActivityLogger
    {
        private const string LabPrefKey = "KIA.WiRR.SelectedLab";
        private const int MaxStoredEvents = 2000;
        private const int MaxAttachedEvents = 500;
        private static DateTime? playModeStartedUtc;
        private static bool initialized;

        [Serializable]
        private sealed class LogFile
        {
            public List<WiRRActivityEvent> events = new List<WiRRActivityEvent>();
        }

        static WiRRActivityLogger()
        {
            EditorApplication.delayCall += InitializeOnMainThread;
        }

        private static void InitializeOnMainThread()
        {
            if (initialized)
                return;

            initialized = true;
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            EditorApplication.quitting -= OnEditorQuitting;
            EditorApplication.quitting += OnEditorQuitting;
            Record("unity_editor_context_loaded", CurrentLab(), "ok");
        }

        public static void Record(string eventName, int labNumber, string result = "")
        {
            if (string.IsNullOrWhiteSpace(eventName))
                return;

            var log = Load();
            log.events.Add(new WiRRActivityEvent
            {
                eventName = eventName.Trim(),
                labNumber = Math.Max(1, Math.Min(7, labNumber)),
                timestampUtc = DateTime.UtcNow.ToString("O"),
                result = result ?? string.Empty,
                durationSeconds = 0d
            });

            Trim(log);
            Save(log);
        }

        public static void RecordDuration(string eventName, int labNumber, DateTime startedUtc, string result = "")
        {
            var seconds = Math.Max(0d, (DateTime.UtcNow - startedUtc).TotalSeconds);
            var log = Load();
            log.events.Add(new WiRRActivityEvent
            {
                eventName = eventName,
                labNumber = Math.Max(1, Math.Min(7, labNumber)),
                timestampUtc = DateTime.UtcNow.ToString("O"),
                result = result ?? string.Empty,
                durationSeconds = Math.Round(seconds, 3)
            });

            Trim(log);
            Save(log);
        }

        public static void AttachTo(WiRRReportDocument document)
        {
            if (document == null)
                return;

            var log = Load();
            var events = log.events
                .Where(e => e != null && e.labNumber == document.labNumber)
                .OrderBy(e => ParseUtc(e.timestampUtc))
                .ToList();

            if (events.Count > MaxAttachedEvents)
                events = events.Skip(events.Count - MaxAttachedEvents).ToList();

            document.activityTimeline = events;
            document.activitySummary = BuildSummary(events);
        }

        private static WiRRActivitySummary BuildSummary(IReadOnlyList<WiRRActivityEvent> events)
        {
            var summary = new WiRRActivitySummary
            {
                eventCount = events.Count,
                playModeSessions = events.Count(e => e.eventName == "play_mode_exited"),
                playModeSeconds = Math.Round(events.Where(e => e.eventName == "play_mode_exited").Sum(e => Math.Max(0d, e.durationSeconds)), 3)
            };

            if (events.Count == 0)
                return summary;

            var first = ParseUtc(events[0].timestampUtc);
            var last = ParseUtc(events[events.Count - 1].timestampUtc);
            summary.firstEventAtUtc = events[0].timestampUtc;
            summary.lastEventAtUtc = events[events.Count - 1].timestampUtc;
            if (first != DateTime.MinValue && last != DateTime.MinValue)
                summary.elapsedSeconds = Math.Round(Math.Max(0d, (last - first).TotalSeconds), 3);

            return summary;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            var lab = CurrentLab();
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                playModeStartedUtc = DateTime.UtcNow;
                Record("play_mode_entered", lab, "ok");
            }
            else if (state == PlayModeStateChange.ExitingPlayMode)
            {
                if (playModeStartedUtc.HasValue)
                    RecordDuration("play_mode_exited", lab, playModeStartedUtc.Value, "ok");
                else
                    Record("play_mode_exited", lab, "ok");
                playModeStartedUtc = null;
            }
        }

        private static void OnEditorQuitting()
        {
            Record("unity_editor_quitting", CurrentLab(), "ok");
        }

        private static int CurrentLab() => Math.Max(1, Math.Min(7, EditorPrefs.GetInt(LabPrefKey, 1)));

        private static string RootPath() =>
            Path.GetFullPath(Path.Combine(Application.dataPath, "../Library/WiRRReports"));

        private static string LogPath() =>
            Path.Combine(RootPath(), "activity-log.json");

        private static LogFile Load()
        {
            var root = RootPath();
            var logPath = LogPath();
            Directory.CreateDirectory(root);
            if (!File.Exists(logPath))
                return new LogFile();

            try
            {
                var parsed = JsonUtility.FromJson<LogFile>(File.ReadAllText(logPath));
                if (parsed != null)
                {
                    parsed.events = parsed.events ?? new List<WiRRActivityEvent>();
                    return parsed;
                }
            }
            catch (Exception exception)
            {
                Debug.LogWarning("[WiRR] Nie udało się odczytać lokalnego logu aktywności: " + exception.Message);
            }

            return new LogFile();
        }

        private static void Save(LogFile log)
        {
            var root = RootPath();
            var logPath = LogPath();
            Directory.CreateDirectory(root);
            File.WriteAllText(logPath, JsonUtility.ToJson(log, true));
        }

        private static void Trim(LogFile log)
        {
            if (log.events.Count <= MaxStoredEvents)
                return;
            log.events.RemoveRange(0, log.events.Count - MaxStoredEvents);
        }

        private static DateTime ParseUtc(string value)
        {
            return DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var parsed)
                ? parsed.ToUniversalTime()
                : DateTime.MinValue;
        }
    }
}
