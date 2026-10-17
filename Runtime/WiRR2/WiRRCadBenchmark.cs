using System;
using System.Collections.Generic;
using System.Linq;

namespace KIA.WiRR
{
    /// <summary>Observed rendered-frame costs; material slots are not GPU draw calls.</summary>
    public sealed class WiRRCadBenchmark
    {
        private readonly List<double> frames = new List<double>();
        private readonly List<int> triangles = new List<int>(), materials = new List<int>();
        private double missingHoleMs;
        public void Sample(double frameMs, int triangleCount, int materialSlots, bool holePreserved)
        {
            if (double.IsNaN(frameMs) || double.IsInfinity(frameMs) || frameMs <= 0 || triangleCount <= 0 || materialSlots <= 0)
                throw new ArgumentOutOfRangeException(nameof(frameMs));
            frames.Add(frameMs); triangles.Add(triangleCount); materials.Add(materialSlots);
            if (!holePreserved) missingHoleMs += frameMs;
        }

        public List<WiRRStudentMetric> Metrics()
        {
            var result = new List<WiRRStudentMetric> { WiRRStudentMetric.Measured("cad_frame_samples", frames.Count, "count") };
            Add(result, "cad_frame_median", frames.Count > 0 ? (double?)WiRRStudentEvidence.Percentile(frames, 0.5) : null, "ms");
            Add(result, "cad_frame_p95", frames.Count > 0 ? (double?)WiRRStudentEvidence.Percentile(frames, 0.95) : null, "ms");
            Add(result, "triangles_min", triangles.Count > 0 ? (double?)triangles.Min() : null, "count");
            Add(result, "triangles_max", triangles.Count > 0 ? (double?)triangles.Max() : null, "count");
            Add(result, "triangles_median", triangles.Count > 0 ? (double?)WiRRStudentEvidence.Percentile(triangles.Select(x => (double)x), 0.5) : null, "count");
            Add(result, "material_slots_min", materials.Count > 0 ? (double?)materials.Min() : null, "count");
            Add(result, "material_slots_max", materials.Count > 0 ? (double?)materials.Max() : null, "count");
            Add(result, "hole_missing_observed", frames.Count > 0 ? (double?)(missingHoleMs / 1000) : null, "s");
            Add(result, "hole_missing_time_fraction", frames.Count > 0 ? (double?)(missingHoleMs / frames.Sum()) : null, "ratio");
            result.Add(new WiRRStudentMetric { name = "gpu_draw_calls", unit = "count", unavailableReason = "Sloty materiałów nie mierzą wywołań GPU; brak licznika renderera." });
            return result;
        }
        private static void Add(List<WiRRStudentMetric> output, string name, double? value, string unit) =>
            output.Add(value.HasValue ? WiRRStudentMetric.Measured(name, value.Value, unit) :
                new WiRRStudentMetric { name = name, unit = unit, unavailableReason = "Brak próbek benchmarku CAD." });
    }
}
