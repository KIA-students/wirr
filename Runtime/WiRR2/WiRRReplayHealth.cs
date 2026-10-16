using System;
using System.Collections.Generic;

namespace KIA.WiRR
{
    /// <summary>Receiver-clock observations for local replay, never a network latency measurement.</summary>
    public sealed class WiRRReplayHealth
    {
        private readonly double threshold;
        private readonly List<double> intervals = new List<double>();
        private double clock = -1, lastMessage, maxAge, staleStarted;
        private double? firstStaleAge, firstOvershoot, firstStaleDuration, gapStarted, gapEnded, firstRecovery;
        public bool HasSource { get; private set; }
        public bool IsStale { get; private set; }
        public bool HasGap => gapStarted.HasValue;
        public int Received { get; private set; }
        public int StaleEpisodes { get; private set; }
        public int Recoveries { get; private set; }
        public double? Age => HasSource ? (double?)(clock - lastMessage) : null;
        public WiRRReplayHealth(double watchdogSeconds)
        {
            if (!Finite(watchdogSeconds) || watchdogSeconds <= 0) throw new ArgumentOutOfRangeException(nameof(watchdogSeconds));
            threshold = watchdogSeconds;
        }
        public void Observe(double now)
        {
            if (!Finite(now) || now < 0 || now < clock) throw new ArgumentOutOfRangeException(nameof(now));
            clock = now;
            if (!HasSource) return;
            var age = now - lastMessage; maxAge = Math.Max(maxAge, age);
            if (!IsStale && age > threshold)
            {
                IsStale = true; StaleEpisodes++; staleStarted = now;
                if (!firstStaleAge.HasValue) { firstStaleAge = age; firstOvershoot = age - threshold; }
            }
        }
        public void Receive(double now)
        {
            Observe(now);
            if (HasSource) intervals.Add((now - lastMessage) * 1000);
            if (IsStale)
            {
                Recoveries++;
                if (!firstStaleDuration.HasValue) firstStaleDuration = now - staleStarted;
            }
            if (gapEnded.HasValue && !firstRecovery.HasValue) firstRecovery = now - gapEnded.Value;
            HasSource = true; IsStale = false; lastMessage = now; Received++;
        }
        public void BeginGap(double now)
        {
            if (HasGap) throw new InvalidOperationException("Jedna kontrolowana przerwa na próbę.");
            Observe(now); gapStarted = now;
        }
        public void EndGap(double now)
        {
            if (!HasGap || gapEnded.HasValue) throw new InvalidOperationException("Brak otwartej przerwy.");
            Observe(now); gapEnded = now;
        }
        public List<WiRRStudentMetric> Metrics()
        {
            var metrics = new List<WiRRStudentMetric> {
                WiRRStudentMetric.Measured("messages_received", Received, "count"),
                WiRRStudentMetric.Measured("stale_episodes", StaleEpisodes, "count"),
                WiRRStudentMetric.Measured("fresh_recoveries", Recoveries, "count") };
            Add(metrics, "interarrival_median", intervals.Count > 0 ? (double?)WiRRStudentEvidence.Percentile(intervals, 0.5) : null, "ms");
            Add(metrics, "interarrival_p95", intervals.Count > 0 ? (double?)WiRRStudentEvidence.Percentile(intervals, 0.95) : null, "ms");
            Add(metrics, "receiver_age_max", HasSource ? (double?)maxAge : null, "s");
            Add(metrics, "last_message_to_stale", firstStaleAge, "s");
            Add(metrics, "stale_detection_overshoot", firstOvershoot, "s");
            Add(metrics, "first_stale_duration", firstStaleDuration, "s");
            Add(metrics, "simulated_gap_duration", gapEnded.HasValue ? gapEnded - gapStarted : null, "s");
            Add(metrics, "resume_to_first_message", firstRecovery, "s");
            metrics.Add(new WiRRStudentMetric { name = "rtt", unit = "ms", unavailableReason = "Jednokierunkowy lokalny REPLAY nie mierzy RTT." });
            metrics.Add(new WiRRStudentMetric { name = "source_to_receiver_latency", unit = "ms", unavailableReason = "Czas pliku jest wirtualny i zamrażany podczas przerwy; brak pomiaru transportu." });
            return metrics;
        }
        private static bool Finite(double value) => !double.IsNaN(value) && !double.IsInfinity(value);
        private static void Add(List<WiRRStudentMetric> output, string name, double? value, string unit) =>
            output.Add(value.HasValue ? WiRRStudentMetric.Measured(name, value.Value, unit) : new WiRRStudentMetric {
                name = name, unit = unit, unavailableReason = "Nie zaobserwowano wymaganego zdarzenia REPLAY." });
    }
}
