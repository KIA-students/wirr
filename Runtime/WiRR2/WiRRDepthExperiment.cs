using System;
using System.Collections.Generic;
using System.Linq;

namespace KIA.WiRR
{
    public sealed class WiRRDepthObservation
    {
        public bool available, referenceVisible, modelVisible, reportedFirstHit;
        public double targetDistance;
        public double? otherSurfaceDistance;
    }

    /// <summary>Independent reference geometry, rendering policy, reported ray and participant answer.</summary>
    public sealed class WiRRDepthExperiment
    {
        private readonly WiRRDepthObservation[] observations;
        private readonly bool?[] answers;
        public int Answered => answers.Count(a => a.HasValue);
        public bool IsComplete => Answered == answers.Length;
        public bool HasMissingSource => observations.Any(o => !o.available);
        public int Correct => Enumerable.Range(0, answers.Length).Count(i => answers[i].HasValue && observations[i].available && answers[i].Value == observations[i].referenceVisible);
        public int FalseAlarms => Enumerable.Range(0, answers.Length).Count(i => answers[i] == true && observations[i].available && !observations[i].referenceVisible);
        public int MissedVisible => Enumerable.Range(0, answers.Length).Count(i => answers[i] == false && observations[i].available && observations[i].referenceVisible);

        public WiRRDepthExperiment(WiRRDepthObservation[] source)
        {
            if (source == null || source.Length == 0 || source.Any(o => o == null)) throw new ArgumentException("Brak obserwacji głębi.");
            // A trial owns its reference snapshot even if the scene is subsequently edited.
            observations = source.Select(o => new WiRRDepthObservation { available = o.available,
                referenceVisible = o.referenceVisible, modelVisible = o.modelVisible, reportedFirstHit = o.reportedFirstHit,
                targetDistance = o.targetDistance, otherSurfaceDistance = o.otherSurfaceDistance }).ToArray();
            foreach (var observation in observations.Where(o => o.available))
                if (PredictVisible(observation.targetDistance, observation.otherSurfaceDistance, 0) != observation.referenceVisible)
                    throw new ArgumentException("Widoczność referencyjna nie zgadza się z geometrią promienia.");
            answers = new bool?[source.Length];
        }

        public bool HasAnswer(int index) => index >= 0 && index < answers.Length && answers[index].HasValue;

        public void InvalidateReference(int index)
        {
            if (index < 0 || index >= observations.Length) throw new ArgumentOutOfRangeException(nameof(index));
            observations[index].available = false;
        }

        public void Answer(int index, bool visible)
        {
            if (index < 0 || index >= answers.Length) throw new ArgumentOutOfRangeException(nameof(index));
            if (answers[index].HasValue) throw new InvalidOperationException("Odpowiedź została już zapisana.");
            answers[index] = visible;
        }

        public static bool PredictVisible(double targetDistance, double? otherDistance, double tolerance)
        {
            if (!FiniteNonnegative(targetDistance) || !FiniteNonnegative(tolerance)
                || (otherDistance.HasValue && !FiniteNonnegative(otherDistance.Value)))
                throw new ArgumentOutOfRangeException(nameof(targetDistance));
            return !otherDistance.HasValue || otherDistance.Value + tolerance >= targetDistance;
        }

        private static bool FiniteNonnegative(double value) => value >= 0 && !double.IsNaN(value) && !double.IsInfinity(value);

        public List<WiRRStudentMetric> Metrics()
        {
            var metrics = new List<WiRRStudentMetric> {
                WiRRStudentMetric.Measured("answers_recorded", Answered, "count"),
                WiRRStudentMetric.Measured("correct_indications", Correct, "count"),
                WiRRStudentMetric.Measured("false_alarms", FalseAlarms, "count"),
                WiRRStudentMetric.Measured("missed_visible", MissedVisible, "count"),
                WiRRStudentMetric.Measured("unknown_reference", observations.Count(o => !o.available), "count"),
                WiRRStudentMetric.Measured("model_visibility_errors", observations.Count(o => o.available && o.modelVisible != o.referenceVisible), "count"),
                WiRRStudentMetric.Measured("reported_ray_errors", observations.Count(o => o.available && o.reportedFirstHit != o.referenceVisible), "count") };
            for (var i = 0; i < observations.Length; i++)
            {
                var o = observations[i]; var prefix = "object_" + (i + 1);
                Add(metrics, prefix + "_answer_visible", answers[i].HasValue ? (double?)(answers[i].Value ? 1 : 0) : null, "boolean", "Brak odpowiedzi.");
                Add(metrics, prefix + "_reference_visible", o.available ? (double?)(o.referenceVisible ? 1 : 0) : null, "boolean", "Brak poprawnego trafienia referencyjnego.");
                Add(metrics, prefix + "_model_visible", o.available ? (double?)(o.modelVisible ? 1 : 0) : null, "boolean", "Brak danych dla modelu okluzji.");
                Add(metrics, prefix + "_reported_first_hit", o.available ? (double?)(o.reportedFirstHit ? 1 : 0) : null, "boolean", "Brak danych promienia.");
                Add(metrics, prefix + "_target_distance", o.available ? (double?)o.targetDistance : null, "m", "Brak trafienia w obiekt.");
                Add(metrics, prefix + "_other_surface_distance", o.available ? o.otherSurfaceDistance : null, "m", o.available ? "Brak innej powierzchni na promieniu." : "Brak danych promienia.");
            }
            return metrics;
        }

        private static void Add(List<WiRRStudentMetric> metrics, string name, double? value, string unit, string reason) =>
            metrics.Add(value.HasValue ? WiRRStudentMetric.Measured(name, value.Value, unit) : new WiRRStudentMetric {
                name = name, unit = unit, unavailableReason = reason });
    }
}
