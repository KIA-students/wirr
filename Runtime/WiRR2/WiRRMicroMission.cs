using System;
using System.Collections.Generic;

namespace KIA.WiRR
{
    /// <summary>Three-step desktop mission; wrong selections remain recorded, classification is single-answer.</summary>
    public sealed class WiRRMicroMission
    {
        private readonly bool classification, simulated;
        private readonly double start;
        private double clock;
        private readonly int[] attempts = new int[3], errors = new int[3];
        private readonly double?[] completedAt = new double?[3];
        private readonly bool?[] correct = new bool?[3];
        public int Step { get; private set; }
        public int Errors { get; private set; }
        public int Successes { get; private set; }
        public bool IsComplete => Step == 3;
        public bool TimedOut { get; private set; }
        public WiRRMicroMission(double now, bool stateClassification, bool guestSim)
        {
            if (!Finite(now) || now < 0) throw new ArgumentOutOfRangeException(nameof(now));
            start = clock = now; classification = stateClassification; simulated = guestSim;
        }
        public void Tick(double now)
        {
            if (!Finite(now) || now < clock) throw new ArgumentOutOfRangeException(nameof(now));
            clock = now;
            if (!IsComplete && now - start >= 90) TimedOut = true;
        }
        public void Select(int target, double now)
        {
            if (classification) throw new InvalidOperationException("To zadanie klasyfikacji.");
            Tick(now); if (IsComplete || TimedOut) return;
            attempts[Step]++;
            if (target != Step) { errors[Step]++; Errors++; return; }
            correct[Step] = errors[Step] == 0; completedAt[Step] = now - start; Successes++; Step++;
        }
        public void Classify(bool fresh, double now)
        {
            if (!classification) throw new InvalidOperationException("To zadanie wyboru celu.");
            Tick(now); if (IsComplete || TimedOut) return;
            attempts[Step]++;
            var expectedFresh = Step != 1;
            correct[Step] = fresh == expectedFresh;
            if (correct[Step].Value) Successes++; else { Errors++; errors[Step]++; }
            completedAt[Step] = now - start; Step++;
        }
        public List<WiRRStudentMetric> Metrics()
        {
            var result = new List<WiRRStudentMetric> {
                WiRRStudentMetric.Measured("mission_steps_completed", Step, "count"),
                WiRRStudentMetric.Measured("mission_errors", Errors, "count"),
                WiRRStudentMetric.Measured("mission_successes", Successes, "count"),
                WiRRStudentMetric.Measured("mission_timed_out", TimedOut ? 1 : 0, "boolean"),
                WiRRStudentMetric.Measured("mission_goal_achieved", IsComplete && Successes == 3 ? 1 : 0, "boolean") };
            for (var i = 0; i < 3; i++)
            {
                var prefix = "mission_step_" + (i + 1);
                result.Add(WiRRStudentMetric.Measured(prefix + "_attempts", attempts[i], "count"));
                result.Add(WiRRStudentMetric.Measured(prefix + "_errors", errors[i], "count"));
                Add(result, prefix + "_first_answer_correct", correct[i].HasValue ? (double?)(correct[i].Value ? 1 : 0) : null, "boolean", "Brak zakończonego kroku.");
                Add(result, prefix + "_completed_at", simulated ? null : completedAt[i], "s", simulated ? "GUEST_SIM nie mierzy czasu wykonania człowieka." : "Brak zakończonego kroku.");
            }
            return result;
        }
        private static bool Finite(double value) => !double.IsNaN(value) && !double.IsInfinity(value);
        private static void Add(List<WiRRStudentMetric> output, string name, double? value, string unit, string reason) =>
            output.Add(value.HasValue ? WiRRStudentMetric.Measured(name, value.Value, unit) : new WiRRStudentMetric { name = name, unit = unit, unavailableReason = reason });
    }
}
