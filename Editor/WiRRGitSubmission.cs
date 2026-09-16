using System;
using System.Diagnostics;
using System.IO;
using KIA.WiRR;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace KIA.WiRR.Editor
{
    internal sealed class WiRRGitSubmissionResult
    {
        public bool Success;
        public bool PullRequestCreated;
        public string Branch;
        public string RepositoryPath;
        public string Message;
    }

    internal static class WiRRGitSubmission
    {
        public const string DefaultRepositoryUrl = "https://github.com/KIA-students/wirr.git";
        public const string DefaultRepositorySlug = "KIA-students/wirr";
        public const string DefaultBaseBranch = "main";
        public const string DefaultReportsPath = "students/reports";

        public static WiRRGitSubmissionResult Submit(WiRRReportDocument document, string repositoryUrl,
            string repositorySlug, string baseBranch, string reportsPath)
        {
            var errors = WiRRReportStore.Validate(document);
            if (errors.Count > 0)
                return new WiRRGitSubmissionResult { Success = false, Message = "Raport niekompletny: " + errors[0] };

            var exported = WiRRReportStore.ExportFinal(document, true);
            var projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            var cacheRoot = Path.Combine(projectRoot, "Library", "WiRRReports", "submission-repo");
            Directory.CreateDirectory(Path.GetDirectoryName(cacheRoot) ?? projectRoot);

            var team = WiRRReportStore.Sanitize(document.teamId, "team");
            var stamp = DateTime.UtcNow.ToString("yyyyMMdd-HHmmss");
            var branch = $"report/{team}/lab{document.labNumber:00}-{stamp}";
            reportsPath = (reportsPath ?? DefaultReportsPath).Trim().Trim('/').Replace('\\', '/');
            baseBranch = string.IsNullOrWhiteSpace(baseBranch) ? DefaultBaseBranch : baseBranch.Trim();
            repositoryUrl = string.IsNullOrWhiteSpace(repositoryUrl) ? DefaultRepositoryUrl : repositoryUrl.Trim();
            var relative = $"{reportsPath}/{team}/lab-{document.labNumber:00}/{document.submissionId}.json";

            try
            {
                if (!Directory.Exists(Path.Combine(cacheRoot, ".git")))
                {
                    if (Directory.Exists(cacheRoot)) Directory.Delete(cacheRoot, true);
                    var clone = Run(projectRoot, "git", $"clone {Q(repositoryUrl)} {Q(cacheRoot)}", 120000);
                    if (clone.code != 0) return Fail("Nie udało się sklonować repozytorium. " + clone.error);
                }
                else
                {
                    Run(cacheRoot, "git", $"remote set-url origin {Q(repositoryUrl)}", 10000);
                }

                var fetch = Run(cacheRoot, "git", $"fetch origin {Q(baseBranch)}", 60000);
                if (fetch.code != 0) return Fail("git fetch nie powiódł się. " + fetch.error);
                var checkout = Run(cacheRoot, "git", $"checkout -B {Q(branch)} {Q("origin/" + baseBranch)}", 30000);
                if (checkout.code != 0) return Fail("Nie udało się utworzyć gałęzi zgłoszenia. " + checkout.error);

                // Local-only identity makes submission independent of the student's global git config.
                Run(cacheRoot, "git", "config user.name \"WiRR Course Toolkit\"", 10000);
                Run(cacheRoot, "git", "config user.email \"wirr-reports@users.noreply.github.com\"", 10000);

                var destination = Path.Combine(cacheRoot, relative.Replace('/', Path.DirectorySeparatorChar));
                Directory.CreateDirectory(Path.GetDirectoryName(destination) ?? cacheRoot);
                File.Copy(exported, destination, true);

                var add = Run(cacheRoot, "git", $"add -- {Q(relative)}", 10000);
                if (add.code != 0) return Fail("git add nie powiódł się. " + add.error);
                var commit = Run(cacheRoot, "git", $"commit -m {Q($"report(wirr): {team} lab {document.labNumber:00}")}", 30000);
                if (commit.code != 0) return Fail("git commit nie powiódł się. " + commit.error);
                var push = Run(cacheRoot, "git", $"push -u origin {Q(branch)}", 120000);
                if (push.code != 0) return Fail("git push nie powiódł się. Zaloguj Git/Git Credential Manager/SSH i upewnij się, że masz prawo tworzyć gałęzie w repozytorium. " + push.error);

                var prCreated = false;
                var ghCheck = Run(cacheRoot, "gh", "--version", 5000, false);
                if (ghCheck.code == 0 && !string.IsNullOrWhiteSpace(repositorySlug))
                {
                    var title = $"WiRR report: {team} — Lab {document.labNumber:00}";
                    var body = "Automatyczne zgłoszenie z WiRR Course Toolkit. CI przygotowuje wyłącznie propozycję oceny; wynik wymaga zatwierdzenia przez prowadzącego.";
                    var pr = Run(cacheRoot, "gh", $"pr create --repo {Q(repositorySlug)} --base {Q(baseBranch)} --head {Q(branch)} --title {Q(title)} --body {Q(body)}", 60000, false);
                    prCreated = pr.code == 0;
                }

                return new WiRRGitSubmissionResult
                {
                    Success = true,
                    PullRequestCreated = prCreated,
                    Branch = branch,
                    RepositoryPath = relative,
                    Message = prCreated
                        ? "Raport wysłany; utworzono Pull Request do oceny."
                        : "Raport wysłany na osobną gałąź. GitHub CLI nie utworzył PR — gałąź pozostaje gotowa do otwarcia PR."
                };
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                return Fail(exception.Message);
            }
        }

        private static WiRRGitSubmissionResult Fail(string message) => new WiRRGitSubmissionResult { Success = false, Message = message };

        private static (int code, string output, string error) Run(string workingDirectory, string executable,
            string arguments, int timeoutMs, bool logErrors = true)
        {
            try
            {
                var start = new ProcessStartInfo(executable, arguments)
                {
                    WorkingDirectory = workingDirectory,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using var process = Process.Start(start);
                if (process == null) return (-1, "", $"Nie można uruchomić {executable}.");
                if (!process.WaitForExit(timeoutMs))
                {
                    try { process.Kill(); } catch { }
                    return (-2, "", $"Przekroczono czas operacji {executable}.");
                }
                var output = process.StandardOutput.ReadToEnd().Trim();
                var error = process.StandardError.ReadToEnd().Trim();
                if (logErrors && process.ExitCode != 0) Debug.LogWarning($"[WiRR Reports] {executable}: {error}");
                return (process.ExitCode, output, error);
            }
            catch (Exception exception)
            {
                return (-3, "", exception.Message);
            }
        }

        private static string Q(string value) => "\"" + (value ?? string.Empty).Replace("\"", "\\\"") + "\"";
    }
}
