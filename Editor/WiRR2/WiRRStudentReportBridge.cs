#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Text;
using KIA.WiRR;
using UnityEditor;
using UnityEngine;

namespace KIA.WiRR.Editor
{
    /// <summary>
    /// Transfers observed WiRR2 block 01–04 trials to the EXISTING WiRR report form.
    /// The form retains its normal validation and Supabase submission transport.
    /// No automatic submission, invented checkpoint measurements or second endpoint.
    /// </summary>
    internal static class WiRRStudentReportBridge
    {
        // The pilot supports 4 blocks, two distinct reports per block; never silently
        // accept a different source mode or infer an unmeasured network RTT.
        internal static int[] BlockLabs(int block)
        {
            if (block < 1 || block > 4) throw new ArgumentOutOfRangeException(nameof(block));
            return new[] { block * 2 - 1, block * 2 };
        }

        internal static string ExpectedMode(int labNumber)
        {
            if (labNumber < 1 || labNumber > 8) throw new ArgumentOutOfRangeException(nameof(labNumber));
            return labNumber == 3 || labNumber == 4 ? "SIM_AR" :
                labNumber == 6 ? "ROS_REPLAY" : "DESKTOP_SIM";
        }

        // Validate the complete selected lab before loading or modifying any v1 report draft.
        internal static void ValidateLabForImport(WiRRStudentSession session, int labNo)
        {
            if (session == null) throw new ArgumentException("Brak sesji WiRR2.");
            if (!BlockLabs(session.block).Contains(labNo))
                throw new ArgumentException("Laboratorium nie należy do wybranego bloku.");
            var lab = session.labs == null ? null : session.labs.Find(x => x != null && x.labNumber == labNo);
            if (lab == null || lab.trials == null || lab.trials.Count == 0)
                throw new ArgumentException("Brak zakończonych prób laboratorium " + labNo + ".");
            var expected = ExpectedMode(labNo);
            if (lab.trials.Any(t => t == null || t.deviceMode != expected))
                throw new ArgumentException("Próby laboratorium " + labNo +
                    " nie odpowiadają oczekiwanemu źródłu " + expected + ".");
        }

        // Read before ApplyStudentProfile: a draft must never be relabelled as another student's work.
        internal static void ValidateExistingOwner(WiRRReportDocument document, string[] indices, string group)
        {
            if (document == null || indices == null) throw new ArgumentException("Brak profilu raportu.");
            if (document.studentIndices != null && document.studentIndices.Any(x =>
                !string.IsNullOrWhiteSpace(x) && !indices.Contains(x)))
                throw new ArgumentException("W formularzu istnieje raport innych studentów. Nie nadpisano go.");
            if (!string.IsNullOrWhiteSpace(document.group) && document.group != group)
                throw new ArgumentException("Grupa istniejącego raportu różni się od grupy sesji.");
        }

        [MenuItem("WiRR/Nowe instrukcje/Przenieś wyniki do formularza raportu")]
        internal static void Import()
        {
            if (!WiRRStudentProfile.IsRegistered)
            {
                EditorUtility.DisplayDialog("WiRR", "Najpierw uzupełnij profil studenta WiRR.", "OK");
                WiRRStudentProfileWindow.Open();
                return;
            }
            var root = Path.Combine(Application.persistentDataPath, "WiRR", "sessions");
            var path = EditorUtility.OpenFilePanel("Wybierz zapisaną sesję WiRR2 (blok 01–04)", root, "json");
            if (string.IsNullOrEmpty(path)) return;
            try
            {
                // Never take arbitrary report JSON: only a validated native WiRR2 session.
                if (new FileInfo(path).Length > 4 * 1024 * 1024)
                    throw new ArgumentException("Sesja przekracza limit 4 MB.");
                var session = JsonUtility.FromJson<WiRRStudentSession>(File.ReadAllText(path, Encoding.UTF8));
                WiRRStudentStorage.Validate(session);
                if (session.block < 1 || session.block > 4 || session.pendingTrial != null)
                    throw new ArgumentException("Wymagana zakończona sesja bloku 01–04 (dwa laboratoria).");
                if (session.studentIndices == null || !session.studentIndices.Contains(WiRRStudentProfile.Index))
                    throw new ArgumentException("Indeks zalogowanego studenta nie należy do tej sesji.");
                if (session.group != (WiRRStudentProfile.GroupLabel))
                    throw new ArgumentException("Grupa profilu WiRR różni się od grupy zapisanej sesji.");

                var labs = BlockLabs(session.block);
                var choice = EditorUtility.DisplayDialogComplex("WiRR: import wyników",
                    "Sesja bloku " + session.block.ToString("00") + ". Wybrać raport " +
                    labs[0].ToString("00") + " albo " + labs[1].ToString("00") + "? " +
                    "Zostaną zachowane próby i jawne braki danych. " +
                    "Import NIE wysyła raportu do Supabase.",
                    "Laboratorium " + labs[0].ToString("00"), "Anuluj",
                    "Laboratorium " + labs[1].ToString("00"));
                if (choice == 1) return;
                var labNo = choice == 0 ? labs[0] : labs[1];
                ValidateLabForImport(session, labNo);
                var lab = session.Lab(labNo);

                var document = WiRRReportStore.LoadOrCreate(labNo, applyStudentProfile: false);
                var hasExistingContent = document.answers.Any(answer =>
                    !string.IsNullOrWhiteSpace(answer.key) && !answer.key.StartsWith("variant.", StringComparison.Ordinal) &&
                    answer.key != "basic.mode" && !string.IsNullOrWhiteSpace(answer.value));
                ValidateExistingOwner(document, session.studentIndices, session.group);
                if (hasExistingContent &&
                    !EditorUtility.DisplayDialog("WiRR: istniejąca treść",
                        "Formularz zawiera wcześniejsze obserwacje. Import zastąpi pola krótkiego raportu. " +
                        "Zachowaj kopię poprzedniego raportu, jeśli jest potrzebna.", "Zastąp pola", "Anuluj"))
                    return;
                // Reuse the existing v1 report and normal Supabase transport.
                // Source modes are lab-specific: 05 is DESKTOP_SIM, 06 ROS_REPLAY.
                var sourceMode = ExpectedMode(labNo);
                // Save applies the active profile to element zero: keep the partner exactly once.
                document.studentIndices = new[] { WiRRStudentProfile.Index }
                    .Concat(session.studentIndices.Where(x => x != WiRRStudentProfile.Index)).ToList();
                document.submittedAtUtc = null;
                document.group = session.group;
                document.teamId = "S" + WiRRStudentProfile.Identifier;
                WiRRReportStore.SetValue(document, "basic.mode", "true");
                WiRRReportStore.SetValue(document, "basic.source", "Unity 6, WiRR2, " + sourceMode + " (lokalna symulacja lub replay; brak pomiaru sprzętowego)");
                var observations = new StringBuilder();
                foreach (var trial in lab.trials)
                {
                    observations.Append("Próba ").Append(trial.id).Append("; ")
                        .Append(trial.timestampUtc).Append("; ")
                        .Append(trial.validity).Append("; tryb: ").Append(trial.deviceMode)
                        .Append("; uczestnictwo: ").Append(trial.participantMode).Append("; warunki: ")
                        .Append(trial.condition).Append("; ");
                    if (trial.validity != "VALID" && !string.IsNullOrEmpty(trial.reason))
                        observations.Append("przyczyna: ").Append(trial.reason).Append("; ");
                    foreach (var metric in trial.metrics)
                        observations.Append(metric.name).Append("=")
                            .Append(metric.available ? metric.value.ToString("R", System.Globalization.CultureInfo.InvariantCulture) : "BRAK")
                            .Append(" ").Append(metric.unit).Append("; ")
                            .Append(metric.available ? "" : "przyczyna braku: " + metric.unavailableReason + "; ");
                    observations.AppendLine();
                }
                WiRRReportStore.SetValue(document, "basic.observation", observations.ToString());
                WiRRReportStore.SetValue(document, "basic.change",
                    "Porównane konfiguracje (według zapisu prób): " +
                    string.Join(" | ", lab.trials.Select(t => t.condition).Distinct().ToArray()));
                WiRRReportStore.SetValue(document, "basic.conclusion",
                    string.IsNullOrWhiteSpace(lab.conclusion) ? "" : lab.conclusion);
                WiRRReportStore.SetValue(document, "basic.limits",
                    "Tryb " + sourceMode + "; brak potwierdzonego pomiaru urządzenia/łącza. " +
                    (sourceMode == "ROS_REPLAY" ? "Lokalny zapis nie mierzy RTT ani ROS_LIVE. " : "") +
                    "Próby FAILED i metryki BRAK zachowano jawnie. " +
                    "Identyfikator sesji: " + session.id + ". Student powinien zweryfikować opis przed wysłaniem.");
                WiRRReportStore.Save(document);
                WiRRReportWindow.OpenForLab(labNo);
                EditorUtility.DisplayDialog("WiRR: wyniki w formularzu",
                    "Zaimportowano rzeczywiste wyniki sesji. Sprawdź pola, uzupełnij własny wniosek, " +
                    "kliknij „Sprawdź kompletność raportu”, a następnie „Zapisz raport lokalnie (bez logowania)”. " +
                    "Dopiero potwierdzenie serwera oznacza odbiór.", "OK");
            }
            catch (Exception error) when (error is IOException || error is UnauthorizedAccessException ||
                error is ArgumentException || error is InvalidOperationException)
            {
                EditorUtility.DisplayDialog("WiRR: nie zaimportowano", error.Message, "OK");
            }
        }
    }
}
#endif
