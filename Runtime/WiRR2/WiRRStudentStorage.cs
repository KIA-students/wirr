using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

namespace KIA.WiRR
{
    public static class WiRRStudentStorage
    {
        public static void Validate(WiRRStudentSession session)
        {
            if (session == null || session.schema != "wirr-block-session/2.0-draft.1"
                || !Guid.TryParseExact(session.id, "N", out _) || session.block < 1 || session.block > 4
                || !WiRRStudentEvidence.ValidIndices(session.studentIndices) || (session.group != "G1" && session.group != "G2")
                || session.labs == null) throw new ArgumentException("Niepoprawna sesja lub profil.");
            var labs = new HashSet<int>(); var trials = new HashSet<string>();
            foreach (var lab in session.labs)
            {
                if (lab == null || lab.labNumber < 1 || lab.labNumber > 8 || (lab.labNumber + 1) / 2 != session.block
                    || !labs.Add(lab.labNumber) || lab.trials == null) throw new ArgumentException("Niepoprawna lista laboratoriów.");
                foreach (var trial in lab.trials) ValidateTrial(trial, trials);
            }
            if (session.pendingTrial != null)
            {
                if (!labs.Contains(session.pendingTrial.labNumber)) throw new ArgumentException("Brak laboratorium rozpoczętej próby.");
                ValidateTrial(session.pendingTrial.trial, trials);
            }
        }
        private static void ValidateTrial(WiRRStudentTrial trial, HashSet<string> ids)
        {
            if (trial == null || string.IsNullOrWhiteSpace(trial.id) || !ids.Add(trial.id)
                || string.IsNullOrWhiteSpace(trial.condition) || string.IsNullOrWhiteSpace(trial.timestampUtc)
                || (trial.validity != "VALID" && trial.validity != "FAILED")
                || (trial.deviceMode != "DESKTOP_SIM" && trial.deviceMode != "SIM_AR" && trial.deviceMode != "ROS_REPLAY")
                || (trial.participantMode != "SOLO" && trial.participantMode != "HUMAN" && trial.participantMode != "GUEST_SIM")
                || trial.metrics == null) throw new ArgumentException("Niepoprawna lub powtórzona próba.");
            var names = new HashSet<string>();
            foreach (var metric in trial.metrics)
                if (metric == null || string.IsNullOrWhiteSpace(metric.name) || !names.Add(metric.name)
                    || string.IsNullOrWhiteSpace(metric.unit) || (metric.available && (double.IsNaN(metric.value) || double.IsInfinity(metric.value))))
                    throw new ArgumentException("Niepoprawna lub powtórzona metryka.");
        }
        private static WiRRStudentSession Read(string path, string[] indices, string group, int block)
        {
            var loaded = JsonUtility.FromJson<WiRRStudentSession>(File.ReadAllText(path));
            Validate(loaded);
            if (loaded.block != block || loaded.group != group
                || !loaded.studentIndices.OrderBy(x => x).SequenceEqual(indices.OrderBy(x => x)))
                throw new ArgumentException("Plik nie odpowiada profilowi i blokowi.");
            return loaded;
        }
        public static WiRRStudentSession Load(string path, string[] indices, string group, int block, out bool usedBackup)
        {
            usedBackup = false;
            if (!File.Exists(path) && !File.Exists(path + ".bak"))
            {
                var created = new WiRRStudentSession { studentIndices = indices, group = group, block = block };
                Validate(created); return created;
            }
            if (File.Exists(path))
            {
                try { return Read(path, indices, group, block); }
                catch (Exception error) when (error is IOException || error is ArgumentException)
                { if (!File.Exists(path + ".bak")) throw; }
            }
            // Validate the backup before moving anything. Keep the bad primary for diagnosis.
            var backup = Read(path + ".bak", indices, group, block);
            if (File.Exists(path)) File.Move(path, path + ".corrupt." + Guid.NewGuid().ToString("N"));
            usedBackup = true; return backup;
        }
        public static string ExportBlock(WiRRStudentSession session, string exportsRoot, string unityVersion, Action<string, string> writer = null)
        {
            Validate(session);
            if (session.pendingTrial != null) throw new InvalidOperationException("Próba nie jest zamknięta.");
            session.Lab(session.block * 2 - 1); session.Lab(session.block * 2);
            var contents = session.labs.OrderBy(l => l.labNumber).ToDictionary(l => "WiRR-lab" + l.labNumber.ToString("00") + ".json",
                l => WiRRStudentEvidence.Json(WiRRStudentEvidence.Report(session, l.labNumber, unityVersion)));
            var exportId = Guid.NewGuid().ToString("N");
            var folder = Path.Combine(exportsRoot, session.id, exportId);
            Directory.CreateDirectory(folder);
            Action<string, string> write = writer ?? WiRRStudentEvidence.AtomicWrite;
            var hashes = new Dictionary<string, object>();
            foreach (var item in contents)
            {
                var path = Path.Combine(folder, item.Key); write(path, item.Value);
                if (File.ReadAllText(path) != item.Value) throw new IOException("Eksport nie przeszedł odczytu kontrolnego.");
                using (var sha = SHA256.Create())
                    hashes[item.Key] = BitConverter.ToString(sha.ComputeHash(File.ReadAllBytes(path))).Replace("-", "").ToLowerInvariant();
            }
            // A directory is complete only when this final commit marker exists.
            var manifest = WiRRStudentEvidence.Json(new Dictionary<string, object> {
                { "schemaVersion", "wirr-block-export/2.0-draft.1" }, { "sessionId", session.id }, { "exportId", exportId },
                { "createdAtUtc", DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture) },
                { "reports", contents.Keys.ToArray() }, { "sha256", hashes }, { "deliveryStatus", "LOCAL_ONLY" } });
            var marker = Path.Combine(folder, "block.json");
            try
            {
                write(marker, manifest);
                if (File.ReadAllText(marker) != manifest) throw new IOException("Niepoprawny manifest eksportu.");
            }
            catch
            {
                if (File.Exists(marker)) File.Move(marker, marker + ".incomplete");
                throw;
            }
            return folder;
        }
    }
}
