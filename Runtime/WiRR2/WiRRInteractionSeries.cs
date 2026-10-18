using System;
using System.Collections.Generic;
using System.Linq;

namespace KIA.WiRR
{
    [Serializable] public sealed class WiRRInteractionCycle
    {
        public int number, errors;
        public double seconds;
        public bool success;
    }

    /// <summary>Five actual manipulation cycles; abort never manufactures the remaining cycles.</summary>
    public sealed class WiRRInteractionSeries
    {
        private readonly List<WiRRInteractionCycle> cycles = new List<WiRRInteractionCycle>();
        private readonly int targetCount;
        private double cycleStarted;
        private int currentErrors;
        public IReadOnlyList<WiRRInteractionCycle> Cycles => cycles.AsReadOnly();
        public int CurrentNumber => cycles.Count + 1;
        public int Successes => cycles.Count(c => c.success);
        public int TotalErrors => cycles.Sum(c => c.errors) + currentErrors;
        public bool IsComplete => cycles.Count == targetCount;
        public bool IsAborted { get; private set; }

        public WiRRInteractionSeries(double startedAt, int count = 5)
        {
            if (count < 1 || count > 100) throw new ArgumentOutOfRangeException(nameof(count));
            CheckTime(startedAt);
            cycleStarted = startedAt; targetCount = count;
        }

        private static void CheckTime(double time)
        {
            if (double.IsNaN(time) || double.IsInfinity(time) || time < 0)
                throw new ArgumentOutOfRangeException(nameof(time));
        }

        public double CurrentSeconds(double now)
        {
            CheckTime(now);
            if (now < cycleStarted) throw new ArgumentOutOfRangeException(nameof(now), "Zegar cofnął się.");
            return now - cycleStarted;
        }

        public void Error()
        {
            if (IsComplete || IsAborted) throw new InvalidOperationException("Seria jest zamknięta.");
            currentErrors++;
        }

        public void CompleteCycle(double now, bool success)
        {
            if (IsComplete || IsAborted) throw new InvalidOperationException("Seria jest zamknięta.");
            var seconds = CurrentSeconds(now);
            cycles.Add(new WiRRInteractionCycle { number = cycles.Count + 1, errors = currentErrors,
                seconds = seconds, success = success });
            currentErrors = 0; cycleStarted = now;
        }

        public void Abort(double now)
        {
            if (IsComplete || IsAborted) return;
            CompleteCycle(now, false);
            IsAborted = true;
        }

        public List<WiRRStudentMetric> Metrics()
        {
            var result = new List<WiRRStudentMetric> {
                WiRRStudentMetric.Measured("cycles_recorded", cycles.Count, "count"),
                WiRRStudentMetric.Measured("cycles_required", targetCount, "count") };
            for (var i = 0; i < targetCount; i++)
            {
                var prefix = "cycle_" + (i + 1);
                if (i < cycles.Count)
                {
                    result.Add(WiRRStudentMetric.Measured(prefix + "_time", cycles[i].seconds, "s"));
                    result.Add(WiRRStudentMetric.Measured(prefix + "_errors", cycles[i].errors, "count"));
                    result.Add(WiRRStudentMetric.Measured(prefix + "_success", cycles[i].success ? 1 : 0, "boolean"));
                }
                else
                {
                    result.Add(new WiRRStudentMetric { name = prefix + "_time", unit = "s", unavailableReason = "Nie wykonano cyklu." });
                    result.Add(new WiRRStudentMetric { name = prefix + "_errors", unit = "count", unavailableReason = "Nie wykonano cyklu." });
                    result.Add(new WiRRStudentMetric { name = prefix + "_success", unit = "boolean", unavailableReason = "Nie wykonano cyklu." });
                }
            }
            var successful = cycles.Where(c => c.success).Select(c => c.seconds).ToArray();
            result.Add(successful.Length > 0 ? WiRRStudentMetric.Measured("cycle_time_median_successful",
                WiRRStudentEvidence.Percentile(successful, 0.5), "s") : new WiRRStudentMetric {
                    name = "cycle_time_median_successful", unit = "s", unavailableReason = "Brak udanych cykli." });
            return result;
        }
    }
}
