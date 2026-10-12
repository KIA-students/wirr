using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace KIA.WiRR.Editor
{
    /// <summary>Optional GitHub CLI transport: runs only after explicit student activation.</summary>
    [InitializeOnLoad]
    internal static class WiRRStudentGitHubSync
    {
        private const string Repo = "KIA-students/wirr";
        private static readonly string Root = Path.GetFullPath(Path.Combine(Application.dataPath, "../Library/WiRRProgress"));
        private static double nextTry;
        private static bool busy;

        static WiRRStudentGitHubSync() => EditorApplication.update += Tick;

        [MenuItem("WiRR/Student/Włącz automatyczne wysyłanie na GitHub", priority = 35)]
        public static void Enable()
        {
            if (!WiRRStudentProfile.IsRegistered)
            {
                WiRRStudentProfileWindow.Open();
                return;
            }
            if (!Run("gh", "auth status", out _))
            {
                EditorUtility.DisplayDialog("WiRR GitHub",
                    "Zainstaluj GitHub CLI (gh) i wykonaj jednorazowo w terminalu: gh auth login. Następnie ponów włączenie.",
                    "OK");
                return;
            }
            if (!EditorUtility.DisplayDialog("WiRR GitHub",
                "Do publicznego repozytorium trafi wyłącznie techniczny identyfikator i informacja o scenach/eksportach, bez indeksu ani surowego raportu. GitHub pokaże konto autora PR. Kontynuować?",
                "Włącz", "Anuluj")) return;
            EditorPrefs.SetBool("KIA.WiRR.GitHubSync." + WiRRStudentProfile.Identifier, true);
            nextTry = 0;
        }

        private static void Tick()
        {
            if (EditorApplication.timeSinceStartup < nextTry) return;
            nextTry = EditorApplication.timeSinceStartup + 300;
            if (busy || Application.isBatchMode || EditorApplication.isCompiling || !WiRRStudentProfile.IsRegistered ||
                !EditorPrefs.GetBool("KIA.WiRR.GitHubSync." + WiRRStudentProfile.Identifier, false)) return;
            var source = Path.Combine(Root, "outbox/progress.json");
            if (!File.Exists(source)) return;
            busy = true;
            try { Upload(source); }
            catch (Exception exception) { UnityEngine.Debug.LogWarning("[WiRR] Synchronizacja: " + exception.Message); }
            finally { busy = false; }
        }

        private static void Upload(string source)
        {
            if (!Run("gh", "api user --jq .login", out var login))
                throw new IOException("Zaloguj GitHub CLI przez gh auth login.");
            login = login.Trim();
            if (!System.Text.RegularExpressions.Regex.IsMatch(login, "^[A-Za-z0-9-]{1,39}$"))
                throw new IOException("Niepoprawna nazwa konta GitHub.");
            var identity = WiRRStudentProfile.Identifier;
            var branch = "student/progress-" + identity;
            var temp = Path.Combine(Path.GetTempPath(), "wirr-github-" + identity);
            Directory.CreateDirectory(temp);
            if (!Directory.Exists(Path.Combine(temp, ".git")))
            {
                if (!Run("gh", "repo fork " + Repo + " --clone=false", out _) &&
                    !Run("gh", "repo view " + login + "/wirr --json name", out _))
                    throw new IOException("Nie udało się utworzyć forka repozytorium.");
                Must("git", "-C " + Quote(temp) + " init");
                Must("git", "-C " + Quote(temp) + " config user.name " + Quote(login));
                Must("git", "-C " + Quote(temp) + " config user.email " + Quote("wirr+" + identity + "@users.noreply.github.com"));
                Must("git", "-C " + Quote(temp) + " remote add origin https://github.com/" + login + "/wirr.git");
            }
            // Each submission branches from the public main: valid PR ancestry.
            Must("git", "-C " + Quote(temp) + " fetch --depth=1 https://github.com/" + Repo + ".git main");
            Must("git", "-C " + Quote(temp) + " checkout -B " + branch + " FETCH_HEAD");
            var dest = Path.Combine(temp, "students", "progress", identity + ".json");
            Directory.CreateDirectory(Path.GetDirectoryName(dest));
            File.Copy(source, dest, true);
            Must("git", "-C " + Quote(temp) + " add students/progress/" + identity + ".json");
            if (Run("git", "-C " + Quote(temp) + " diff --cached --quiet", out _)) return;
            if (!Run("git", "-C " + Quote(temp) + " diff --cached --quiet", out _))
                Must("git", "-C " + Quote(temp) + " commit -m " + Quote("WiRR: progress snapshot " + identity));
            Must("gh", "auth setup-git");
            Must("git", "-C " + Quote(temp) + " push -u origin HEAD:refs/heads/" + branch + " --force-with-lease");
            if (!Run("gh", "pr list --repo " + Repo + " --head " + login + ":" + branch + " --state open --json number --jq length", out var count))
                throw new IOException("Nie można odczytać stanu istniejącego PR.");
            if (count.Trim() != "0") return;
            Must("gh", "pr create --repo " + Repo + " --base main --head " + login + ":" + branch +
                " --title " + Quote("WiRR: postęp " + identity) +
                " --body " + Quote("Automatyczne zgłoszenie stanu technicznego. Dane nie są oceną i wymagają kontroli."));
        }

        private static string Quote(string value) => "\"" + value.Replace("\"", "\\\"") + "\"";
        private static void Must(string program, string args)
        {
            if (!Run(program, args, out var log)) throw new IOException(program + ": " + log);
        }
        private static bool Run(string program, string args, out string output)
        {
            try
            {
                using (var p = new Process())
                {
                    p.StartInfo = new ProcessStartInfo(program, args)
                    {
                        UseShellExecute = false, CreateNoWindow = true,
                        RedirectStandardOutput = true, RedirectStandardError = true
                    };
                    p.Start();
                    var stdout = p.StandardOutput.ReadToEnd();
                    var stderr = p.StandardError.ReadToEnd();
                    if (!p.WaitForExit(30000)) { try { p.Kill(); } catch { } output = "Timeout"; return false; }
                    output = p.ExitCode == 0 ? stdout : stderr;
                    return p.ExitCode == 0;
                }
            }
            catch (Exception e) { output = e.Message; return false; }
        }
    }
}
