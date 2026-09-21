using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using KIA.WiRR;

namespace KIA.WiRR.Editor
{
    /// <summary>
    /// Oblicza wartości, które jednoznacznie wynikają z danych wpisanych przez studentów.
    /// Nie wyprowadza wniosków ani ocen merytorycznych.
    /// </summary>
    internal static class WiRRReportCalculator
    {
        private static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;

        public static void Recalculate(WiRRReportDocument document)
        {
            if (document == null || document.labNumber < 1 || document.labNumber > 7)
                return;

            foreach (var section in WiRRReportSchemaCatalog.Get(document.labNumber))
            foreach (var table in WiRRReportTableCatalog.Get(document.labNumber, section.Checkpoint))
                RecalculateTable(document, table);

            RecalculateSummaryFields(document);
        }

        public static bool IsDerivedField(string fieldId)
        {
            return fieldId == "cp30.baseline_fps" ||
                   fieldId == "cp30.baseline_frame_ms" ||
                   fieldId == "cp30.errors" ||
                   fieldId == "cp35.false_activations" ||
                   fieldId == "cp40.corrections" ||
                   fieldId == "cp45.scenario_median_s" ||
                   fieldId == "cp40.drift_median_mm" ||
                   fieldId == "cp45.e_median_mm" ||
                   fieldId == "cp45.e_max_mm" ||
                   fieldId == "cp50.error_after_fix_mm" ||
                   fieldId == "cp40.latency_ms" ||
                   fieldId == "cp40.jitter_ms" ||
                   fieldId == "cp45.stale_ms" ||
                   fieldId == "cp45.recovery_ms" ||
                   fieldId == "cp30.smoke_pass" ||
                   fieldId == "cp30.nv" ||
                   fieldId == "cp40.success_rate" ||
                   fieldId == "cp40.false_activations" ||
                   fieldId == "cp40.assistance" ||
                   fieldId == "cp50.max_risk";
        }

        public static string DerivedFieldDescription(string fieldId)
        {
            return fieldId switch
            {
                "cp30.baseline_fps" => "Mediana z trzech wartości FPS w tabeli pomiaru bazowego.",
                "cp30.baseline_frame_ms" => "Mediana z trzech czasów klatki w tabeli pomiaru bazowego.",
                "cp30.errors" => "Suma błędów ze wszystkich warunków A–C w etapie 3.0.",
                "cp35.false_activations" => "Suma błędnych i powtórzonych aktywacji ze wszystkich warunków A–C.",
                "cp40.corrections" => "Suma błędów i dodatkowych korekt ze wszystkich warunków A–C.",
                "cp45.scenario_median_s" => "Mediana czasu z pięciu prób scenariusza bazowego.",
                "cp40.drift_median_mm" => "Mediana błędu końcowego z trzech pomiarów po powrocie.",
                "cp45.e_median_mm" => "Mediana wartości e_med z trzech rejestracji dwupunktowych.",
                "cp45.e_max_mm" => "Największa wartość e_max z trzech rejestracji dwupunktowych.",
                "cp50.error_after_fix_mm" => "Mediana e_med z trzech powtórzeń wykonanych po naprawie.",
                "cp40.latency_ms" => "Mediana z dziesięciu próbek RTT.",
                "cp40.jitter_ms" => "Odchylenie standardowe z 30 odstępów między kolejnymi wiadomościami.",
                "cp45.stale_ms" => "Wartość pobierana z pola „Wykrycie STALE [ms]” w tabeli LIVE/STALE.",
                "cp45.recovery_ms" => "Wartość pobierana z pola „Powrót LIVE [ms]” w tabeli LIVE/STALE.",
                "cp30.smoke_pass" => "Liczba pozycji ze statusem PASS w tabeli testu podstawowego.",
                "cp30.nv" => "Liczba pozycji ze statusem NV w tabeli testu podstawowego.",
                "cp40.success_rate" => "Odsetek zadań U1–U3 oznaczonych jako wykonane poprawnie.",
                "cp40.false_activations" => "Suma błędnych aktywacji dla zadań U1–U3.",
                "cp40.assistance" => "Suma podpowiedzi lub przypadków pomocy dla zadań U1–U3.",
                "cp50.max_risk" => "Największa wartość R = P × S w macierzy ryzyka.",
                _ => "Wartość obliczana automatycznie na podstawie danych w tabelach."
            };
        }

        public static bool IsDerivedCell(WiRRReportTable table, WiRRTableAxis column)
        {
            if (table == null)
                return false;

            var id = column.Id;

            if (id == "δfps" || id == "delta_fps")
                return HasColumn(table, "mediana_fps");

            if (table.Id == "lab01_phy_c" && id == "kroki_s")
                return true;

            if ((id == "e_med_mm" || id == "e_max_mm") &&
                HasColumns(table, "e1_mm", "e2_mm", "e3_mm"))
                return true;

            if (table.Id == "lab05_benchmark" && id == "fps" && HasColumn(table, "median_ms"))
                return true;

            if (table.Id == "lab07_risk" && id == "r" && HasColumns(table, "p", "s"))
                return true;

            return GetMedianSources(table, column).Count >= 2 ||
                   (id == "mediana_ms" && HasColumn(table, "mediana_fps"));
        }

        public static string DerivedDescription(WiRRReportTable table, WiRRTableAxis column)
        {
            if (table == null)
                return "Obliczane automatycznie.";

            if (column.Id == "δfps" || column.Id == "delta_fps")
                return "Obliczane automatycznie względem pierwszego wiersza tabeli.";
            if (table.Id == "lab01_phy_c" && column.Id == "kroki_s")
                return "Obliczane automatycznie jako 1 / Fixed Timestep.";
            if (column.Id == "e_med_mm")
                return "Mediana wartości e1, e2 i e3 — obliczana automatycznie.";
            if (column.Id == "e_max_mm")
                return "Największa z wartości e1, e2 i e3 — obliczana automatycznie.";
            if (table.Id == "lab05_benchmark" && column.Id == "fps")
                return "Obliczane automatycznie jako 1000 / mediana czasu klatki [ms].";
            if (table.Id == "lab07_risk" && column.Id == "r")
                return "Priorytet ryzyka R = P × S — obliczany automatycznie.";
            if (column.Id == "mediana_ms" && HasColumn(table, "mediana_fps") &&
                GetMedianSources(table, column).Count < 2)
                return "Obliczane automatycznie jako 1000 / mediana FPS.";
            if (GetMedianSources(table, column).Count >= 2)
                return "Mediana z wartości pomiarowych w tym wierszu — obliczana automatycznie.";

            return "Obliczane automatycznie.";
        }

        private static void RecalculateTable(WiRRReportDocument document, WiRRReportTable table)
        {
            foreach (var row in table.Rows)
            {
                foreach (var column in table.Columns)
                {
                    if (!IsDerivedCell(table, column))
                        continue;

                    var value = CalculateCell(document, table, row, column);
                    SetCell(document, table, row, column, value);
                }
            }
        }

        private static string CalculateCell(
            WiRRReportDocument document,
            WiRRReportTable table,
            WiRRTableAxis row,
            WiRRTableAxis column)
        {
            if (column.Id == "δfps" || column.Id == "delta_fps")
            {
                var medianColumn = FindColumn(table, "mediana_fps");
                if (!medianColumn.HasValue || table.Rows.Count == 0)
                    return string.Empty;

                var baseline = ReadCellNumber(document, table, table.Rows[0], medianColumn.Value);
                var current = ReadCellNumber(document, table, row, medianColumn.Value);
                if (!baseline.HasValue || !current.HasValue || Math.Abs(baseline.Value) < 1e-12)
                    return string.Empty;

                return Format(100.0 * (current.Value - baseline.Value) / baseline.Value);
            }

            if (table.Id == "lab01_phy_c" && column.Id == "kroki_s")
            {
                if (!TryParse(row.Label, out var timestep) || timestep <= 0)
                    return string.Empty;
                return Math.Round(1.0 / timestep).ToString(Invariant);
            }

            if (column.Id == "e_med_mm" || column.Id == "e_max_mm")
            {
                var values = ReadColumns(document, table, row, "e1_mm", "e2_mm", "e3_mm");
                if (values.Count < 3)
                    return string.Empty;
                return column.Id == "e_med_mm" ? Format(Median(values)) : Format(values.Max());
            }

            if (table.Id == "lab05_benchmark" && column.Id == "fps")
            {
                var medianMs = ReadCellNumber(document, table, row, FindColumn(table, "median_ms"));
                if (!medianMs.HasValue || medianMs.Value <= 0)
                    return string.Empty;
                return Format(1000.0 / medianMs.Value);
            }

            if (table.Id == "lab07_risk" && column.Id == "r")
            {
                var p = ReadCellNumber(document, table, row, FindColumn(table, "p"));
                var s = ReadCellNumber(document, table, row, FindColumn(table, "s"));
                if (!p.HasValue || !s.HasValue)
                    return string.Empty;
                return Format(p.Value * s.Value);
            }

            var sources = GetMedianSources(table, column);
            if (sources.Count >= 2)
            {
                var values = new List<double>();
                foreach (var source in sources)
                {
                    var value = ReadCellNumber(document, table, row, source);
                    if (value.HasValue)
                        values.Add(value.Value);
                }

                // Mediana jest pokazywana dopiero po uzupełnieniu wszystkich przewidzianych powtórzeń.
                if (values.Count != sources.Count)
                    return string.Empty;
                return Format(Median(values));
            }

            if (column.Id == "mediana_ms" && HasColumn(table, "mediana_fps"))
            {
                var medianFps = ReadCellNumber(document, table, row, FindColumn(table, "mediana_fps"));
                if (!medianFps.HasValue || medianFps.Value <= 0)
                    return string.Empty;
                return Format(1000.0 / medianFps.Value);
            }

            return string.Empty;
        }

        private static void RecalculateSummaryFields(WiRRReportDocument document)
        {
            switch (document.labNumber)
            {
                case 1:
                    SetFieldFromColumnMedian(document, "cp30.baseline_fps", "3.0", "lab01_cp30_baseline", "fps");
                    SetFieldFromColumnMedian(document, "cp30.baseline_frame_ms", "3.0", "lab01_cp30_baseline", "ms_klatka");
                    break;

                case 2:
                    SetFieldFromColumnSum(document, "cp30.errors", "3.0", "lab02_cp30_conditions", "błędy");
                    SetFieldFromColumnSum(document, "cp35.false_activations", "3.5", "lab02_cp35_conditions", "błędne_aktywacje");
                    SetFieldFromColumnSum(document, "cp40.corrections", "4.0", "lab02_cp40_conditions", "błędy_korekty");
                    SetFieldFromColumnMedian(document, "cp45.scenario_median_s", "4.5", "lab02_scenario", "czas_s");
                    break;

                case 3:
                    SetFieldFromColumnMedian(document, "cp40.drift_median_mm", "4.0", "lab03_drift", "błąd_końcowy_mm");
                    SetFieldFromColumnMedian(document, "cp45.e_median_mm", "4.5", "lab03_registration", "e_med_mm");
                    SetFieldFromColumnMax(document, "cp45.e_max_mm", "4.5", "lab03_registration", "e_max_mm");
                    SetFieldFromRowGroupMedian(document, "cp50.error_after_fix_mm", "5.0", "lab03_fault", "e_med_mm", "naprawa");
                    break;

                case 6:
                    SetFieldFromColumnMedian(document, "cp40.latency_ms", "4.0", "lab06_rtt", "rtt_ms");
                    SetFieldFromColumnStdDev(document, "cp40.jitter_ms", "4.0", "lab06_interarrival", "inter_arrival_ms");
                    SetFieldFromSingleCell(document, "cp45.stale_ms", "4.5", "lab06_stale", "pomiar", "detekcja_stale_ms");
                    SetFieldFromSingleCell(document, "cp45.recovery_ms", "4.5", "lab06_stale", "pomiar", "odzyskanie_live_ms");
                    break;

                case 7:
                    SetFieldFromStatusCount(document, "cp30.smoke_pass", "3.0", "lab07_smoke", "status_pass_fail_nv", "PASS");
                    SetFieldFromStatusCount(document, "cp30.nv", "3.0", "lab07_smoke", "status_pass_fail_nv", "NV");
                    SetFieldFromBooleanRate(document, "cp40.success_rate", "4.0", "lab07_usability", "sukces");
                    SetFieldFromColumnSum(document, "cp40.false_activations", "4.0", "lab07_usability", "błędne_aktywacje");
                    SetFieldFromColumnSum(document, "cp40.assistance", "4.0", "lab07_usability", "pomoc");
                    SetFieldFromColumnMax(document, "cp50.max_risk", "5.0", "lab07_risk", "r");
                    break;
            }
        }

        private static void SetFieldFromColumnMedian(
            WiRRReportDocument document, string fieldId, string checkpoint, string tableId, string columnId)
        {
            var values = ReadCompleteColumn(document, checkpoint, tableId, columnId);
            WiRRReportStore.SetValue(document, fieldId, values == null ? string.Empty : Format(Median(values)));
        }

        private static void SetFieldFromColumnMax(
            WiRRReportDocument document, string fieldId, string checkpoint, string tableId, string columnId)
        {
            var values = ReadCompleteColumn(document, checkpoint, tableId, columnId);
            WiRRReportStore.SetValue(document, fieldId, values == null ? string.Empty : Format(values.Max()));
        }

        private static void SetFieldFromColumnSum(
            WiRRReportDocument document, string fieldId, string checkpoint, string tableId, string columnId)
        {
            var values = ReadCompleteColumn(document, checkpoint, tableId, columnId);
            WiRRReportStore.SetValue(document, fieldId, values == null ? string.Empty : Format(values.Sum()));
        }

        private static void SetFieldFromColumnStdDev(
            WiRRReportDocument document, string fieldId, string checkpoint, string tableId, string columnId)
        {
            var values = ReadCompleteColumn(document, checkpoint, tableId, columnId);
            if (values == null || values.Count < 2)
            {
                WiRRReportStore.SetValue(document, fieldId, string.Empty);
                return;
            }

            var mean = values.Average();
            var variance = values.Sum(v => (v - mean) * (v - mean)) / (values.Count - 1);
            WiRRReportStore.SetValue(document, fieldId, Format(Math.Sqrt(variance)));
        }

        private static void SetFieldFromSingleCell(
            WiRRReportDocument document,
            string fieldId,
            string checkpoint,
            string tableId,
            string rowId,
            string columnId)
        {
            var table = WiRRReportTableCatalog.Get(document.labNumber, checkpoint).FirstOrDefault(t => t.Id == tableId);
            if (table == null)
            {
                WiRRReportStore.SetValue(document, fieldId, string.Empty);
                return;
            }

            var row = table.Rows.FirstOrDefault(r => r.Id == rowId);
            var column = FindColumn(table, columnId);
            if (!column.HasValue || string.IsNullOrEmpty(row.Id))
            {
                WiRRReportStore.SetValue(document, fieldId, string.Empty);
                return;
            }

            var raw = WiRRReportStore.GetValue(document, WiRRReportTableCatalog.CellKey(table, row, column.Value));
            WiRRReportStore.SetValue(document, fieldId, raw);
        }

        private static void SetFieldFromRowGroupMedian(
            WiRRReportDocument document,
            string fieldId,
            string checkpoint,
            string tableId,
            string columnId,
            string rowPrefix)
        {
            var table = WiRRReportTableCatalog.Get(document.labNumber, checkpoint).FirstOrDefault(t => t.Id == tableId);
            var column = table == null ? null : FindColumn(table, columnId);
            if (table == null || !column.HasValue)
            {
                WiRRReportStore.SetValue(document, fieldId, string.Empty);
                return;
            }

            var rows = table.Rows.Where(r => r.Id.StartsWith(rowPrefix, StringComparison.OrdinalIgnoreCase)).ToArray();
            var values = new List<double>();
            foreach (var row in rows)
            {
                var value = ReadCellNumber(document, table, row, column.Value);
                if (value.HasValue)
                    values.Add(value.Value);
            }

            WiRRReportStore.SetValue(
                document,
                fieldId,
                rows.Length > 0 && values.Count == rows.Length ? Format(Median(values)) : string.Empty);
        }

        private static void SetFieldFromStatusCount(
            WiRRReportDocument document,
            string fieldId,
            string checkpoint,
            string tableId,
            string columnId,
            string expectedStatus)
        {
            var table = WiRRReportTableCatalog.Get(document.labNumber, checkpoint).FirstOrDefault(t => t.Id == tableId);
            var column = table == null ? null : FindColumn(table, columnId);
            if (table == null || !column.HasValue)
            {
                WiRRReportStore.SetValue(document, fieldId, string.Empty);
                return;
            }

            var values = new List<string>();
            foreach (var row in table.Rows)
            {
                var raw = WiRRReportStore.GetValue(document, WiRRReportTableCatalog.CellKey(table, row, column.Value));
                if (string.IsNullOrWhiteSpace(raw))
                {
                    WiRRReportStore.SetValue(document, fieldId, string.Empty);
                    return;
                }
                values.Add(raw.Trim());
            }

            var count = values.Count(value => value.StartsWith(expectedStatus, StringComparison.OrdinalIgnoreCase));
            WiRRReportStore.SetValue(document, fieldId, count.ToString(Invariant));
        }

        private static void SetFieldFromBooleanRate(
            WiRRReportDocument document,
            string fieldId,
            string checkpoint,
            string tableId,
            string columnId)
        {
            var table = WiRRReportTableCatalog.Get(document.labNumber, checkpoint).FirstOrDefault(t => t.Id == tableId);
            var column = table == null ? null : FindColumn(table, columnId);
            if (table == null || !column.HasValue)
            {
                WiRRReportStore.SetValue(document, fieldId, string.Empty);
                return;
            }

            var successCount = 0;
            foreach (var row in table.Rows)
            {
                var raw = WiRRReportStore.GetValue(document, WiRRReportTableCatalog.CellKey(table, row, column.Value));
                if (string.IsNullOrWhiteSpace(raw))
                {
                    WiRRReportStore.SetValue(document, fieldId, string.Empty);
                    return;
                }

                if (raw.Equals("tak", StringComparison.OrdinalIgnoreCase) ||
                    raw.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                    raw.Equals("PASS", StringComparison.OrdinalIgnoreCase))
                    successCount++;
            }

            WiRRReportStore.SetValue(document, fieldId, Format(100.0 * successCount / table.Rows.Count));
        }

        private static List<double> ReadCompleteColumn(
            WiRRReportDocument document, string checkpoint, string tableId, string columnId)
        {
            var table = WiRRReportTableCatalog.Get(document.labNumber, checkpoint).FirstOrDefault(t => t.Id == tableId);
            if (table == null)
                return null;

            var column = FindColumn(table, columnId);
            if (!column.HasValue)
                return null;

            var values = new List<double>();
            foreach (var row in table.Rows)
            {
                var value = ReadCellNumber(document, table, row, column.Value);
                if (!value.HasValue)
                    return null;
                values.Add(value.Value);
            }
            return values;
        }

        private static List<WiRRTableAxis> GetMedianSources(WiRRReportTable table, WiRRTableAxis target)
        {
            var result = new List<WiRRTableAxis>();
            var targetId = target.Id;

            foreach (var column in table.Columns)
            {
                var id = column.Id;

                if (targetId == "mediana_fps" && IsNumbered(id, "fps_"))
                    result.Add(column);
                else if (targetId == "mediana_ms" && IsNumbered(id, "ms_"))
                    result.Add(column);
                else if ((targetId == "mediana_s" || targetId == "mediana") &&
                         (IsNumbered(id, "próba_") || IsNumbered(id, "powt_") || IsNumbered(id, "powtórzenie_")))
                    result.Add(column);
                else if (targetId == "mediana_mm" &&
                         (IsNumbered(id, "powt_") || IsNumbered(id, "powtórzenie_")))
                    result.Add(column);
                else if (targetId == "mediana" && id.Contains("successrate", StringComparison.OrdinalIgnoreCase))
                    result.Add(column);
            }

            // Specjalne nagłówki, których slug zawiera jednostkę.
            if (targetId.Contains("mediana", StringComparison.OrdinalIgnoreCase) &&
                targetId.Contains("s", StringComparison.OrdinalIgnoreCase) &&
                result.Count == 0)
            {
                foreach (var column in table.Columns)
                    if ((column.Id.StartsWith("próba_", StringComparison.Ordinal) ||
                         column.Id.StartsWith("powt_", StringComparison.Ordinal) ||
                         column.Id.StartsWith("powtórzenie_", StringComparison.Ordinal)) &&
                        !column.Id.Contains("mediana", StringComparison.OrdinalIgnoreCase))
                        result.Add(column);
            }

            return result;
        }

        private static bool IsNumbered(string id, string prefix)
        {
            if (!id.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                return false;

            var rest = id.Substring(prefix.Length);
            return rest.Length > 0 && char.IsDigit(rest[0]);
        }

        private static List<double> ReadColumns(
            WiRRReportDocument document,
            WiRRReportTable table,
            WiRRTableAxis row,
            params string[] columnIds)
        {
            var values = new List<double>();
            foreach (var id in columnIds)
            {
                var column = FindColumn(table, id);
                var value = ReadCellNumber(document, table, row, column);
                if (value.HasValue)
                    values.Add(value.Value);
            }
            return values;
        }

        private static double? ReadCellNumber(
            WiRRReportDocument document,
            WiRRReportTable table,
            WiRRTableAxis row,
            WiRRTableAxis? column)
        {
            if (!column.HasValue)
                return null;

            var raw = WiRRReportStore.GetValue(document, WiRRReportTableCatalog.CellKey(table, row, column.Value));
            return TryParse(raw, out var value) ? value : null;
        }

        private static void SetCell(
            WiRRReportDocument document,
            WiRRReportTable table,
            WiRRTableAxis row,
            WiRRTableAxis column,
            string value)
        {
            WiRRReportStore.SetValue(document, WiRRReportTableCatalog.CellKey(table, row, column), value);
        }

        private static WiRRTableAxis? FindColumn(WiRRReportTable table, string id)
        {
            foreach (var column in table.Columns)
                if (column.Id == id)
                    return column;
            return null;
        }

        private static bool HasColumn(WiRRReportTable table, string id) => FindColumn(table, id).HasValue;

        private static bool HasColumns(WiRRReportTable table, params string[] ids)
        {
            foreach (var id in ids)
                if (!HasColumn(table, id))
                    return false;
            return true;
        }

        private static double Median(IReadOnlyList<double> values)
        {
            var sorted = values.OrderBy(v => v).ToArray();
            var middle = sorted.Length / 2;
            return sorted.Length % 2 == 1
                ? sorted[middle]
                : (sorted[middle - 1] + sorted[middle]) / 2.0;
        }

        private static bool TryParse(string text, out double value)
        {
            if (double.TryParse(text, NumberStyles.Float, Invariant, out value))
                return true;

            return double.TryParse(
                (text ?? string.Empty).Trim().Replace(',', '.'),
                NumberStyles.Float,
                Invariant,
                out value);
        }

        private static string Format(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                return string.Empty;
            return value.ToString("0.###", Invariant);
        }
    }
}
