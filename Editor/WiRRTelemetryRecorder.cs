using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using KIA.WiRR;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KIA.WiRR.Editor
{
    [InitializeOnLoad]
    internal static class WiRRTelemetryRecorder
    {
        private const double SnapshotIntervalSeconds = 600.0;
        private const string LabPrefKey = "KIA.WiRR.SelectedLab";
        private static readonly string Root = Path.GetFullPath(Path.Combine(Application.dataPath, "../Library/WiRRReports"));
        private static readonly string TelemetryPath = Path.Combine(Root, "telemetry.json");
        private static readonly HashSet<string> KnownAssets = new HashSet<string>(StringComparer.Ordinal);
        private static WiRRTelemetryBundle data;
        private static double lastUpdate;
        private static double lastSnapshot;

        static WiRRTelemetryRecorder()
        {
            Directory.CreateDirectory(Root);
            data = Ensure(Load());
            if (string.IsNullOrEmpty(data.sessionStartedUtc)) data.sessionStartedUtc = UtcNow();
            if (string.IsNullOrEmpty(data.currentStage)) data.currentStage = "UNASSIGNED";
            RefreshKnownAssets();
            lastUpdate = EditorApplication.timeSinceStartup;
            lastSnapshot = lastUpdate - SnapshotIntervalSeconds + 15.0;
            EditorApplication.update += Update;
            EditorApplication.playModeStateChanged += OnPlayMode;
            EditorApplication.projectChanged += () => RecordEditorEvent("project-changed", "Asset database changed");
            AssemblyReloadEvents.beforeAssemblyReload += Save;
            CompilationPipeline.compilationStarted += _ => { data.compileCount++; RecordEditorEvent("compile-start", "Script compilation started"); };
            CompilationPipeline.compilationFinished += _ => RecordEditorEvent("compile-finish", "Script compilation finished");
            EditorSceneManager.sceneOpened += (scene, mode) => RecordEditorEvent("scene-opened", scene.path);
            EditorSceneManager.sceneSaved += scene => RecordEditorEvent("scene-saved", scene.path);
        }

        public static WiRRTelemetryBundle GetCopy()
        {
            Save();
            return JsonUtility.FromJson<WiRRTelemetryBundle>(JsonUtility.ToJson(data));
        }

        public static void SetStage(string stage, string detail = "")
        {
            stage = string.IsNullOrWhiteSpace(stage) ? "UNASSIGNED" : stage.Trim();
            if (string.Equals(data.currentStage, stage, StringComparison.Ordinal)) return;
            data.currentStage = stage;
            GetStage(stage);
            RecordEditorEvent("stage-change", string.IsNullOrEmpty(detail) ? stage : stage + ": " + detail);
        }

        public static void CaptureNow(string reason = "manual")
        {
            CaptureSnapshot();
            RecordEditorEvent("snapshot", reason);
            Save();
        }

        public static void RecordAssetEvents(string[] imported, string[] deleted, string[] moved, string[] movedFrom)
        {
            if (imported != null)
            {
                foreach (var path in imported.Where(IsStudentProjectPath))
                {
                    var action = KnownAssets.Contains(path) ? "modified" : "created";
                    AddFileEvent(action, path);
                    KnownAssets.Add(path);
                }
            }
            if (deleted != null)
            {
                foreach (var path in deleted.Where(IsStudentProjectPath))
                {
                    AddFileEvent("deleted", path, false);
                    KnownAssets.Remove(path);
                }
            }
            if (moved != null && movedFrom != null)
            {
                for (var i = 0; i < Math.Min(moved.Length, movedFrom.Length); i++)
                {
                    if (IsStudentProjectPath(movedFrom[i]))
                    {
                        AddFileEvent("moved-from", movedFrom[i], false);
                        KnownAssets.Remove(movedFrom[i]);
                    }
                    if (IsStudentProjectPath(moved[i]))
                    {
                        AddFileEvent("moved-to", moved[i]);
                        KnownAssets.Add(moved[i]);
                    }
                }
            }
            Trim();
            Save();
        }

        private static void Update()
        {
            var now = EditorApplication.timeSinceStartup;
            var delta = Math.Max(0.0, Math.Min(2.0, now - lastUpdate));
            lastUpdate = now;
            if (InternalEditorUtility.isApplicationActive)
            {
                data.activeEditorSeconds += delta;
                if (EditorApplication.isPlaying) data.playModeSeconds += delta;
                var stage = GetStage(data.currentStage);
                stage.activeSeconds += delta;
                stage.lastSeenUtc = UtcNow();
                data.lastActivityUtc = stage.lastSeenUtc;
            }
            if (now - lastSnapshot >= SnapshotIntervalSeconds)
            {
                lastSnapshot = now;
                CaptureSnapshot();
                Save();
            }
        }

        private static void OnPlayMode(PlayModeStateChange state)
        {
            RecordEditorEvent("play-mode", state.ToString());
            if (state == PlayModeStateChange.ExitingPlayMode || state == PlayModeStateChange.EnteredEditMode)
                CaptureSnapshot();
        }

        private static WiRRStageDuration GetStage(string stage)
        {
            var existing = data.stageDurations.FirstOrDefault(item => item.stage == stage);
            if (existing != null) return existing;
            var now = UtcNow();
            existing = new WiRRStageDuration { stage = stage, firstSeenUtc = now, lastSeenUtc = now, activeSeconds = 0.0 };
            data.stageDurations.Add(existing);
            return existing;
        }

        private static void CaptureSnapshot()
        {
            var scene = SceneManager.GetActiveScene();
            var rootCount = scene.IsValid() ? scene.rootCount : 0;
            var componentCount = 0;
            if (scene.IsValid())
            {
                foreach (var root in scene.GetRootGameObjects())
                    componentCount += root.GetComponentsInChildren<Component>(true).Length;
            }

            var project = HashProjectState();
            data.snapshots.Add(new WiRRTelemetrySnapshot
            {
                capturedAtUtc = UtcNow(),
                labNumber = EditorPrefs.GetInt(LabPrefKey, 1),
                stage = data.currentStage,
                scenePath = scene.IsValid() ? scene.path : string.Empty,
                sceneDirty = scene.IsValid() && scene.isDirty,
                playing = EditorApplication.isPlaying,
                rootObjectCount = rootCount,
                componentCount = componentCount,
                projectFileCount = project.count,
                projectBytes = project.bytes,
                projectStateSha256 = project.hash
            });
            data.snapshotCount = data.snapshots.Count;
            Trim();
        }

        private static (int count, long bytes, string hash) HashProjectState()
        {
            var root = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            var files = new List<string>();
            if (Directory.Exists(Application.dataPath))
                files.AddRange(Directory.EnumerateFiles(Application.dataPath, "*", SearchOption.AllDirectories));
            foreach (var relative in new[] { "Packages/manifest.json", "Packages/packages-lock.json", "ProjectSettings/ProjectVersion.txt" })
            {
                var path = Path.Combine(root, relative);
                if (File.Exists(path)) files.Add(path);
            }
            files = files.Where(path => !path.EndsWith(".meta", StringComparison.OrdinalIgnoreCase))
                .OrderBy(path => path, StringComparer.Ordinal).Take(5000).ToList();

            using var combined = SHA256.Create();
            long bytes = 0;
            foreach (var file in files)
            {
                var info = new FileInfo(file);
                bytes += info.Length;
                var relative = NormalizeRelative(root, file);
                var contentHash = info.Length <= 8 * 1024 * 1024 ? HashFile(file) : "large-file";
                var line = $"{relative}|{info.Length}|{info.LastWriteTimeUtc.Ticks}|{contentHash}\n";
                var buffer = Encoding.UTF8.GetBytes(line);
                combined.TransformBlock(buffer, 0, buffer.Length, null, 0);
            }
            combined.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
            return (files.Count, bytes, Hex(combined.Hash));
        }

        private static void AddFileEvent(string action, string assetPath, bool readFile = true)
        {
            var full = Path.GetFullPath(Path.Combine(Application.dataPath, "..", assetPath));
            var exists = readFile && File.Exists(full);
            var info = exists ? new FileInfo(full) : null;
            data.fileEvents.Add(new WiRRFileEvent
            {
                timestampUtc = UtcNow(),
                stage = data.currentStage,
                action = action,
                path = assetPath.Replace('\\', '/'),
                sizeBytes = info != null ? info.Length : 0,
                sha256 = exists && info.Length <= 8 * 1024 * 1024 ? HashFile(full) : string.Empty
            });
            data.lastActivityUtc = UtcNow();
        }

        private static bool IsStudentProjectPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return false;
            path = path.Replace('\\', '/');
            return path.StartsWith("Assets/", StringComparison.Ordinal) &&
                   !path.EndsWith(".meta", StringComparison.OrdinalIgnoreCase) &&
                   !path.StartsWith("Assets/WiRR/Reports/", StringComparison.Ordinal);
        }

        private static void RefreshKnownAssets()
        {
            KnownAssets.Clear();
            if (!Directory.Exists(Application.dataPath)) return;
            var projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            foreach (var file in Directory.EnumerateFiles(Application.dataPath, "*", SearchOption.AllDirectories))
            {
                var relative = NormalizeRelative(projectRoot, file);
                if (IsStudentProjectPath(relative)) KnownAssets.Add(relative);
            }
        }

        private static void RecordEditorEvent(string type, string detail)
        {
            data.editorEvents.Add(new WiRREditorEvent { timestampUtc = UtcNow(), type = type, detail = detail ?? string.Empty });
            data.lastActivityUtc = UtcNow();
            Trim();
            Save();
        }

        private static void Trim()
        {
            const int maxSnapshots = 144;
            const int maxFileEvents = 4000;
            const int maxEditorEvents = 2000;
            if (data.snapshots.Count > maxSnapshots) data.snapshots.RemoveRange(0, data.snapshots.Count - maxSnapshots);
            if (data.fileEvents.Count > maxFileEvents) data.fileEvents.RemoveRange(0, data.fileEvents.Count - maxFileEvents);
            if (data.editorEvents.Count > maxEditorEvents) data.editorEvents.RemoveRange(0, data.editorEvents.Count - maxEditorEvents);
            data.snapshotCount = data.snapshots.Count;
        }

        private static WiRRTelemetryBundle Load()
        {
            try
            {
                if (File.Exists(TelemetryPath))
                    return JsonUtility.FromJson<WiRRTelemetryBundle>(File.ReadAllText(TelemetryPath)) ?? new WiRRTelemetryBundle();
            }
            catch (Exception exception) { Debug.LogWarning("[WiRR Reports] Telemetry load failed: " + exception.Message); }
            return new WiRRTelemetryBundle();
        }

        private static WiRRTelemetryBundle Ensure(WiRRTelemetryBundle bundle)
        {
            bundle = bundle ?? new WiRRTelemetryBundle();
            bundle.stageDurations = bundle.stageDurations ?? new List<WiRRStageDuration>();
            bundle.snapshots = bundle.snapshots ?? new List<WiRRTelemetrySnapshot>();
            bundle.fileEvents = bundle.fileEvents ?? new List<WiRRFileEvent>();
            bundle.editorEvents = bundle.editorEvents ?? new List<WiRREditorEvent>();
            return bundle;
        }

        private static void Save()
        {
            try
            {
                Directory.CreateDirectory(Root);
                File.WriteAllText(TelemetryPath, JsonUtility.ToJson(data, true));
            }
            catch (Exception exception) { Debug.LogWarning("[WiRR Reports] Telemetry save failed: " + exception.Message); }
        }

        private static string HashFile(string path)
        {
            using var sha = SHA256.Create();
            using var stream = File.OpenRead(path);
            return Hex(sha.ComputeHash(stream));
        }

        private static string Hex(byte[] bytes) => BitConverter.ToString(bytes ?? Array.Empty<byte>()).Replace("-", string.Empty).ToLowerInvariant();

        private static string NormalizeRelative(string root, string path)
        {
            root = Path.GetFullPath(root).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
            path = Path.GetFullPath(path);
            return path.StartsWith(root, StringComparison.OrdinalIgnoreCase)
                ? path.Substring(root.Length).Replace('\\', '/')
                : Path.GetFileName(path);
        }

        private static string UtcNow() => DateTime.UtcNow.ToString("O");
    }

    internal sealed class WiRRTelemetryAssetPostprocessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            WiRRTelemetryRecorder.RecordAssetEvents(importedAssets, deletedAssets, movedAssets, movedFromAssetPaths);
        }
    }
}
