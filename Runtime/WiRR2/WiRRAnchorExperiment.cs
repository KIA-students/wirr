using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KIA.WiRR
{
    [Serializable] public sealed class WiRRAnchorProfile
    {
        public string name;
        public Vector3 bias_m, relocalization_shift_m;
        public Vector3[] jitter_m;
    }

    [Serializable] public sealed class WiRRAnchorDataset
    {
        public string version, source;
        public int seed;
        public float cadence_s, recovery_delay_s, recovery_timeout_s;
        public WiRRAnchorProfile[] profiles;
    }

    [Serializable] public sealed class WiRRAnchorReading
    {
        public Vector3 offset_m;
        public double elapsed_s;
        public int observation;
    }

    public enum WiRRAnchorPhase { WaitingForPlacement, BeforeLoss, TrackingLost, AfterRecovery, Complete, Failed }

    /// <summary>Explicit synthetic anchor model. No AR provider or physical accuracy is inferred.</summary>
    public sealed class WiRRAnchorExperiment
    {
        private readonly WiRRAnchorDataset data;
        private readonly WiRRAnchorProfile profile;
        private readonly bool relocalize, failRecovery;
        private double placedAt, nextReadingAt, lostAt, lastTick;
        private double? recoveredAfter;
        private readonly List<WiRRAnchorReading> before = new List<WiRRAnchorReading>();
        private readonly List<WiRRAnchorReading> after = new List<WiRRAnchorReading>();
        public WiRRAnchorPhase Phase { get; private set; } = WiRRAnchorPhase.WaitingForPlacement;
        public IReadOnlyList<WiRRAnchorReading> Before => before.AsReadOnly();
        public IReadOnlyList<WiRRAnchorReading> After => after.AsReadOnly();
        public bool HasLostTracking { get; private set; }
        public int Observation => Phase == WiRRAnchorPhase.AfterRecovery ? after.Count : before.Count;
        public WiRRAnchorReading Latest => Phase == WiRRAnchorPhase.TrackingLost || Phase == WiRRAnchorPhase.Failed ? null :
            Phase == WiRRAnchorPhase.AfterRecovery || (Phase == WiRRAnchorPhase.Complete && HasLostTracking) ?
                (after.Count > 0 ? after[after.Count - 1] : null) : (before.Count > 0 ? before[before.Count - 1] : null);

        public WiRRAnchorExperiment(WiRRAnchorDataset dataset, int profileIndex, bool relocalize, bool failRecovery)
        {
            if (dataset == null || dataset.source != "SYNTHETIC_MODEL_NOT_DEVICE" || string.IsNullOrWhiteSpace(dataset.version)
                || dataset.profiles == null || profileIndex < 0 || profileIndex >= dataset.profiles.Length
                || !PositiveFinite(dataset.cadence_s) || !PositiveFinite(dataset.recovery_delay_s)
                || !PositiveFinite(dataset.recovery_timeout_s) || dataset.recovery_timeout_s <= dataset.recovery_delay_s)
                throw new ArgumentException("Nieprawidłowy model SIM_AR.");
            profile = dataset.profiles[profileIndex];
            if (profile == null || profile.jitter_m == null || profile.jitter_m.Length != 3 || !Finite(profile.bias_m)
                || !Finite(profile.relocalization_shift_m) || profile.jitter_m.Any(v => !Finite(v)))
                throw new ArgumentException("Profil musi zawierać trzy skończone odczyty modelu.");
            data = dataset; this.relocalize = relocalize; this.failRecovery = failRecovery;
        }

        private static bool PositiveFinite(float value) => value > 0 && !float.IsInfinity(value);
        private static bool Finite(Vector3 value) => new[] { value.x, value.y, value.z }.All(v => !float.IsNaN(v) && !float.IsInfinity(v));
        private static void CheckTime(double time)
        {
            if (double.IsNaN(time) || double.IsInfinity(time) || time < 0) throw new ArgumentOutOfRangeException(nameof(time));
        }

        public void Place(double now)
        {
            CheckTime(now);
            if (Phase != WiRRAnchorPhase.WaitingForPlacement) throw new InvalidOperationException("Kotwica została już umieszczona.");
            placedAt = lastTick = now; nextReadingAt = now + data.cadence_s; Phase = WiRRAnchorPhase.BeforeLoss;
        }

        /// <returns>True when a real update changes the synthetic state or records one reading.</returns>
        public bool Tick(double now)
        {
            CheckTime(now);
            if (now < lastTick) throw new ArgumentOutOfRangeException(nameof(now), "Zegar cofnął się.");
            lastTick = now;
            if (Phase == WiRRAnchorPhase.TrackingLost)
            {
                var delay = now - lostAt;
                if (delay >= data.recovery_timeout_s) { Phase = WiRRAnchorPhase.Failed; return true; }
                if (!failRecovery && delay >= data.recovery_delay_s)
                {
                    recoveredAfter = delay; Phase = WiRRAnchorPhase.AfterRecovery; nextReadingAt = now + data.cadence_s;
                    return true;
                }
                return false;
            }
            if ((Phase != WiRRAnchorPhase.BeforeLoss && Phase != WiRRAnchorPhase.AfterRecovery) || now < nextReadingAt) return false;
            var readings = Phase == WiRRAnchorPhase.BeforeLoss ? before : after;
            readings.Add(new WiRRAnchorReading { observation = readings.Count,
                offset_m = profile.bias_m + profile.jitter_m[readings.Count] + (Phase == WiRRAnchorPhase.AfterRecovery ? profile.relocalization_shift_m : Vector3.zero),
                elapsed_s = now - placedAt });
            // Never backfill several readings after a stalled frame or a tracking gap.
            nextReadingAt = now + data.cadence_s;
            if (readings.Count == 3)
            {
                if (Phase == WiRRAnchorPhase.BeforeLoss && relocalize)
                { Phase = WiRRAnchorPhase.TrackingLost; HasLostTracking = true; lostAt = now; }
                else Phase = WiRRAnchorPhase.Complete;
            }
            return true;
        }

        public List<WiRRStudentMetric> Metrics()
        {
            var metrics = new List<WiRRStudentMetric> {
                WiRRStudentMetric.Measured("tracking_losses", HasLostTracking ? 1 : 0, "count"),
                recoveredAfter.HasValue ? WiRRStudentMetric.Measured("relocalization_time", recoveredAfter.Value, "s") :
                    new WiRRStudentMetric { name = "relocalization_time", unit = "s", unavailableReason = relocalize ?
                        "Nie zarejestrowano odzyskania śledzenia." : "Nie uruchomiono kontrpróby relokalizacji." } };
            AddReadings(metrics, "before", before); AddReadings(metrics, "after", after);
            if (HasLostTracking) metrics.Add(new WiRRStudentMetric { name = "pose_during_tracking_loss", unit = "cm",
                unavailableReason = "Brak poprawnego odczytu podczas utraty śledzenia SIM_AR." });
            return metrics;
        }

        private static void AddReadings(List<WiRRStudentMetric> metrics, string phase, List<WiRRAnchorReading> readings)
        {
            metrics.Add(WiRRStudentMetric.Measured(phase + "_sample_count", readings.Count, "count"));
            var mean = readings.Count > 0 ? readings.Aggregate(Vector3.zero, (sum, r) => sum + r.offset_m) / readings.Count : Vector3.zero;
            AddMetric(metrics, phase + "_mean_error", readings.Count > 0 ? (double?)readings.Average(r => r.offset_m.magnitude) * 100 : null, "cm");
            AddMetric(metrics, phase + "_bias_norm", readings.Count > 0 ? (double?)mean.magnitude * 100 : null, "cm");
            AddMetric(metrics, phase + "_scatter_rms", readings.Count > 1 ? (double?)Math.Sqrt(readings.Average(r => (r.offset_m - mean).sqrMagnitude)) * 100 : null, "cm");
            for (var i = 0; i < 3; i++)
            {
                var reading = i < readings.Count ? readings[i] : null;
                var prefix = phase + "_sample_" + (i + 1);
                AddMetric(metrics, prefix + "_dx", reading == null ? null : (double?)reading.offset_m.x * 100, "cm");
                AddMetric(metrics, prefix + "_dy", reading == null ? null : (double?)reading.offset_m.y * 100, "cm");
                AddMetric(metrics, prefix + "_dz", reading == null ? null : (double?)reading.offset_m.z * 100, "cm");
                AddMetric(metrics, prefix + "_elapsed", reading == null ? null : (double?)reading.elapsed_s, "s");
            }
        }

        private static void AddMetric(List<WiRRStudentMetric> metrics, string name, double? value, string unit) =>
            metrics.Add(value.HasValue ? WiRRStudentMetric.Measured(name, value.Value, unit) : new WiRRStudentMetric {
                name = name, unit = unit, unavailableReason = "Brak wystarczających poprawnych odczytów w tej fazie." });
    }
}
