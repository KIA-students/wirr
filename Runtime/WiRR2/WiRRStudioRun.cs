using System;
using System.Collections.Generic;

namespace KIA.WiRR
{
    /// <summary>Presentation window and explicitly reported assistance, separate from task validity.</summary>
    public sealed class WiRRStudioRun
    {
        private readonly double start;
        private readonly bool demonstration, simulated;
        private double clock;
        private double? goalAt;
        public int ReportedHints { get; private set; }
        public int SimulatedHints { get; private set; }
        public bool GoalReached => goalAt.HasValue;
        public bool Overdue => clock - start > 90;
        public bool InPresentationWindow => clock - start >= 60 && clock - start <= 90;
        public bool CanFinish => GoalReached && !Overdue && (!demonstration || InPresentationWindow);
        public WiRRStudioRun(double now, bool presentation, bool guestSim)
        {
            if (!Finite(now) || now < 0) throw new ArgumentOutOfRangeException(nameof(now));
            start = clock = now; demonstration = presentation; simulated = guestSim;
        }
        public void Observe(double now, bool goalReached)
        {
            if (!Finite(now) || now < clock) throw new ArgumentOutOfRangeException(nameof(now));
            clock = now;
            if (goalReached && !goalAt.HasValue) goalAt = now - start;
        }
        public void Hint(double now)
        {
            Observe(now, false);
            if (simulated) SimulatedHints++; else ReportedHints++;
        }
        public List<WiRRStudentMetric> Metrics()
        {
            var result = new List<WiRRStudentMetric> {
                WiRRStudentMetric.Measured("studio_goal_achieved", GoalReached ? 1 : 0, "boolean"),
                WiRRStudentMetric.Measured("presentation_requested", demonstration ? 1 : 0, "boolean"),
                WiRRStudentMetric.Measured("presentation_window_met", demonstration && InPresentationWindow ? 1 : 0, "boolean"),
                WiRRStudentMetric.Measured("simulated_hints", SimulatedHints, "count") };
            result.Add(!simulated ? WiRRStudentMetric.Measured("author_reported_hints", ReportedHints, "count") : new WiRRStudentMetric {
                name = "author_reported_hints", unit = "count", unavailableReason = "GUEST_SIM nie jest obserwacją podpowiedzi dla człowieka." });
            result.Add(!simulated && goalAt.HasValue ? WiRRStudentMetric.Measured("time_to_goal", goalAt.Value, "s") : new WiRRStudentMetric {
                name = "time_to_goal", unit = "s", unavailableReason = simulated ? "GUEST_SIM nie mierzy czasu człowieka." : "Nie osiągnięto celu." });
            return result;
        }
        private static bool Finite(double value) => !double.IsNaN(value) && !double.IsInfinity(value);
    }
}
