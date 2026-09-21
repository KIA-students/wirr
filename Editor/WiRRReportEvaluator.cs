using System.Collections.Generic;
using System.Linq;
using KIA.WiRR;

namespace KIA.WiRR.Editor
{
    internal sealed class WiRRCheckpointEvaluation
    {
        public string Checkpoint;
        public bool Complete;
        public readonly List<string> Reasons = new List<string>();
    }

    internal sealed class WiRRReportEvaluation
    {
        public string SuggestedGrade;
        public readonly List<string> BlockingIssues = new List<string>();
        public readonly List<WiRRCheckpointEvaluation> Checkpoints = new List<WiRRCheckpointEvaluation>();

        public bool CanSubmit =>
            BlockingIssues.Count == 0 &&
            Checkpoints.Any(cp => cp.Checkpoint == "3.0" && cp.Complete);
    }

    internal static class WiRRReportEvaluator
    {
        private static readonly string[] Grades = { "3.0", "3.5", "4.0", "4.5", "5.0" };

        public static WiRRReportEvaluation Evaluate(WiRRReportDocument document)
        {
            var result = new WiRRReportEvaluation();
            result.BlockingIssues.AddRange(WiRRReportStore.ValidateIdentity(document));
            if (document == null || document.labNumber < 1 || document.labNumber > 7) return result;

            var chainOpen = result.BlockingIssues.Count == 0;
            foreach (var grade in Grades)
            {
                var cp = new WiRRCheckpointEvaluation { Checkpoint = grade };
                var section = WiRRReportSchemaCatalog.Get(document.labNumber).FirstOrDefault(s => s.Checkpoint == grade);
                if (section == null)
                {
                    cp.Reasons.Add("Brak definicji etapu.");
                    chainOpen = false;
                    result.Checkpoints.Add(cp);
                    continue;
                }

                foreach (var field in section.Fields.Where(f => f.Required))
                    if (string.IsNullOrWhiteSpace(WiRRReportStore.GetValue(document, field.Id)))
                        cp.Reasons.Add("Brak: " + field.Label + ".");

                foreach (var table in WiRRReportTableCatalog.Get(document.labNumber, grade))
                {
                    var hasData = table.Rows.Any(row => table.Columns.Any(column =>
                        !string.IsNullOrWhiteSpace(WiRRReportStore.GetValue(document, WiRRReportTableCatalog.CellKey(table, row, column)))));
                    if (!hasData) cp.Reasons.Add("Brak danych pomiarowych: " + table.Label + ".");
                }

                if (!chainOpen && cp.Reasons.Count == 0) cp.Reasons.Add("Najpierw uzupełnij wcześniejszy etap.");
                cp.Complete = chainOpen && cp.Reasons.Count == 0;
                if (cp.Complete) result.SuggestedGrade = grade;
                else chainOpen = false;
                result.Checkpoints.Add(cp);
            }
            return result;
        }
    }
}
