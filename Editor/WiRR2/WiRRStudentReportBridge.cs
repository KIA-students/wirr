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
    /// Transfers observed SIM_AR trials to the EXISTING WiRR report form.
    /// The form retains its normal validation and Supabase submission transport.
    /// No automatic submission, invented checkpoint measurements or second endpoint.
    /// </summary>
    internal static class WiRRStudentReportBridge
    {
        // Read before ApplyStudentProfile: a draft must never be relabelled as another student's work.
        internal static void ValidateExistingOwner(WiRRReportDocument document, string[] indices, string group)
        {
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
            var path = EditorUtility.OpenFilePanel("Wybierz zapisaną sesję bloku 02 WiRR2", root, "json");
            if (string.IsNullOrEmpty(path)) return;
            try
            {
                // Never take arbitrary report JSON: only a validated native WiRR2 session.
                if (new FileInfo(path).Length > 4 * 1024 * 1024)
                    throw new ArgumentException("Sesja przekracza limit 4 MB.");
                var session = JsonUtility.FromJson<WiRRStudentSession>(File.ReadAllText(path, Encoding.UTF8));
                WiRRStudentStorage.Validate(session);
                if (session.block != 2 || session.pendingTrial != null)
                    throw new ArgumentException("Wymagana zakończona sesja bloku 02 (laboratoria 03 i 04).");
                if (!session.studentIndices.Contains(WiRRStudentProfile.Index))
                    throw new ArgumentException("Indeks zalogowanego studenta nie należy do tej sesji.");
                if (session.group != (WiRRStudentProfile.GroupLabel))
                    throw new ArgumentException("Grupa profilu WiRR różni się od grupy zapisanej sesji.");

                var choice = EditorUtility.DisplayDialogComplex("WiRR: import wyników",
                    "Zaimportować wyniki z sesji do formularza laboratorium 03 czy 04? " +
                    "Zostaną zachowane wszystkie próby i braki danych w opisie. " +
                    "Import NIE wysyła raportu do Supabase.",
                    "Laboratorium 03", "Anuluj", "Laboratorium 04");
                if (choice == 1) return;
                var labNo = choice == 0 ? 3 : 4;
                var lab = session.Lab(labNo);
                if (lab.trials.Count == 0)
                    throw new ArgumentException("Brak zakończonych prób laboratorium " + labNo + ".");

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
                if (lab.trials.Any(t => t.deviceMode != "SIM_AR"))
                    throw new ArgumentException("Raport SIM_AR nie może zawierać prób z innego źródła.");
                // Save applies the active profile to element zero: keep the partner exactly once.
                document.studentIndices = new[] { WiRRStudentProfile.Index }
                    .Concat(session.studentIndices.Where(x => x != WiRRStudentProfile.Index)).ToList();
                document.submittedAtUtc = null;
                document.group = session.group;
                document.teamId = "S" + WiRRStudentProfile.Identifier;
                WiRRReportStore.SetValue(document, "basic.mode", "true");
                WiRRReportStore.SetValue(document, "basic.source", "Unity 6, WiRR2, SIM_AR (dane syntetyczne, nie pomiar sprzętowy)");
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
                    "Tryb SIM_AR; brak pomiarów sprzętowych. Próby oznaczone FAILED i metryki BRAK zachowano jawnie. " +
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
