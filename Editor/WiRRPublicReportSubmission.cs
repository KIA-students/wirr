#if UNITY_EDITOR
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

// Anonymous course-key transport, separate from offline report saving.
// PublicCourseKey is not a GitHub credential.
public static class WiRRPublicReportSubmission
{
    [Serializable]
    private sealed class Envelope
    {
        public string submission_id;
        public int lab_no;
        public string report;
    }

    [Serializable]
    private sealed class SubmissionReceipt
    {
        public bool accepted;
        public string submission_id;
    }

    private static readonly HttpClient Client = new HttpClient { Timeout = TimeSpan.FromSeconds(25) };
    public static async void SubmitFile(string path, int lab)
    {
        if (lab < 1 || lab > 8 || string.IsNullOrEmpty(path) || !File.Exists(path))
        {
            EditorUtility.DisplayDialog("WiRR", "Najpierw zapisz raport lokalnie.", "OK");
            return;
        }
        try
        {
            var report = File.ReadAllText(path, Encoding.UTF8).Trim();
            if (!report.StartsWith("{") || !report.EndsWith("}") || Encoding.UTF8.GetByteCount(report) > 120000)
                throw new IOException("Raport nie jest poprawnym obiektem JSON lub przekracza 120 KB.");
            // Use a sidecar identifier for idempotent retries. Never overwrite the report.
            var idPath = path + ".submission-id";
            var submissionId = File.Exists(idPath) ? File.ReadAllText(idPath).Trim() : Guid.NewGuid().ToString();
            Guid parsed;
            if (!Guid.TryParse(submissionId, out parsed)) throw new IOException("Niepoprawny identyfikator zgłoszenia.");
            if (!File.Exists(idPath)) File.WriteAllText(idPath, submissionId, Encoding.UTF8);
            var payload = "{\"submission_id\":\"" + submissionId + "\",\"lab_no\":" + lab + ",\"report\":" + report + "}";
            using (var request = new HttpRequestMessage(HttpMethod.Post, WiRRCourseSubmissionConfig.SubmissionEndpoint))
            {
                request.Headers.Add("X-WiRR-Course-Key", WiRRCourseSubmissionConfig.PublicCourseKey);
                request.Content = new StringContent(payload, Encoding.UTF8, "application/json");
                using (var response = await Client.SendAsync(request))
                {
                    var body = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode || !body.Contains("\"accepted\":true"))
                        throw new IOException("Brak potwierdzenia odbioru. HTTP " + (int)response.StatusCode + ". " + body);
                    File.WriteAllText(path + ".receipt.json", body, Encoding.UTF8);
                    EditorUtility.DisplayDialog("WiRR: raport przyjęty",
                        "Serwer KIA potwierdził odbiór raportu. Identyfikator: " + submissionId +
                        "\nRaport oczekuje w prywatnej kolejce. Publikacja na GitHub jest odrębnym procesem.", "OK");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning("WiRR: przesłanie raportu nie powiodło się: " + ex.Message);
            EditorUtility.DisplayDialog("WiRR: raport nadal lokalnie",
                "Nie uzyskano potwierdzenia odbioru. Zachowano plik. Możesz ponowić przesłanie.\n" + ex.Message, "OK");
        }
    }

    // Parse the receipt rather than matching whitespace-sensitive JSON text.
    // Where the service echoes a submission identifier, it must match this request.
    private static bool IsAcceptedResponse(string json, string submissionId)
    {
        if (string.IsNullOrWhiteSpace(json)) return false;
        try
        {
            var receipt = JsonUtility.FromJson<SubmissionReceipt>(json);
            if (receipt == null || !receipt.accepted) return false;
            return string.IsNullOrEmpty(receipt.submission_id) ||
                   string.Equals(receipt.submission_id, submissionId, StringComparison.OrdinalIgnoreCase);
        }
        catch (ArgumentException)
        {
            return false;
        }
    }

    [MenuItem("WiRR/Student/Wyślij raport bez logowania")]
    public static void OpenSavedReport()
    {
        var projectRoot = Directory.GetParent(Application.dataPath).FullName;
        var folder = Path.Combine(projectRoot, "Library", "WiRRReports", "exports");
        var path = EditorUtility.OpenFilePanel("Wybierz zapisany raport WiRR", folder, "json");
        if (string.IsNullOrEmpty(path)) return;
        var name = Path.GetFileNameWithoutExtension(path).ToLowerInvariant();
        var lab = 0;
        for (int i = 1; i <= 8; i++)
            if (name.Contains("lab" + i.ToString("00"))) { lab = i; break; }
        if (lab == 0)
        {
            EditorUtility.DisplayDialog("WiRR", "Nazwa eksportu powinna zawierać lab01...lab08. Sprawdź plik.", "OK");
            return;
        }
        SubmitFile(path, lab);
    }
}
#endif
