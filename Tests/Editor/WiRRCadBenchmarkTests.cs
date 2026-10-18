using System;
using System.Linq;
using KIA.WiRR;
using NUnit.Framework;
using UnityEngine;

public sealed class WiRRCadBenchmarkTests
{
    [Test] public void EmptyMeasurementHasUnknownCostsInsteadOfZeros()
    {
        var metrics = new WiRRCadBenchmark().Metrics();
        Assert.AreEqual(0, metrics.Single(m => m.name == "cad_frame_samples").value);
        Assert.IsFalse(metrics.Single(m => m.name == "triangles_min").available);
        Assert.IsFalse(metrics.Single(m => m.name == "hole_missing_time_fraction").available);
    }
    [Test] public void FullWindowRetainsHighCostAndTemporaryLossOfHole()
    {
        var benchmark = new WiRRCadBenchmark();
        benchmark.Sample(10, 1536, 4, true);
        benchmark.Sample(30, 24, 4, false);
        benchmark.Sample(10, 384, 4, true);
        var metrics = benchmark.Metrics();
        Assert.AreEqual(24, metrics.Single(m => m.name == "triangles_min").value);
        Assert.AreEqual(1536, metrics.Single(m => m.name == "triangles_max").value);
        Assert.AreEqual(384, metrics.Single(m => m.name == "triangles_median").value);
        Assert.AreEqual(0.6, metrics.Single(m => m.name == "hole_missing_time_fraction").value, 1e-9);
        Assert.AreEqual(0.03, metrics.Single(m => m.name == "hole_missing_observed").value, 1e-9);
    }
    [Test] public void MaterialSlotsDoNotBecomeMeasuredDrawCalls()
    {
        var benchmark = new WiRRCadBenchmark(); benchmark.Sample(12, 384, 4, true);
        var metrics = benchmark.Metrics();
        Assert.AreEqual(4, metrics.Single(m => m.name == "material_slots_max").value);
        Assert.IsFalse(metrics.Single(m => m.name == "gpu_draw_calls").available);
    }
    [Test] public void InvalidFrameCannotContaminatePartialEvidence()
    {
        var benchmark = new WiRRCadBenchmark(); benchmark.Sample(12, 384, 1, true);
        foreach (var value in new[] { double.NaN, double.PositiveInfinity, 0, -1 })
            Assert.Throws<ArgumentOutOfRangeException>(() => benchmark.Sample(value, 384, 1, true));
        Assert.Throws<ArgumentOutOfRangeException>(() => benchmark.Sample(12, 0, 1, true));
        Assert.AreEqual(1, benchmark.Metrics().Single(m => m.name == "cad_frame_samples").value);
    }
    [Test] public void ExplicitlySavedPartialSnapshotRetainsCostsAndFailedStatus()
    {
        var benchmark = new WiRRCadBenchmark(); benchmark.Sample(12, 384, 1, true);
        var session = new WiRRStudentSession { block = 3 };
        session.BeginTrial(5, "DESKTOP_SIM", "SOLO", "procedural-ring/v1");
        session.pendingTrial.trial.metrics = benchmark.Metrics();
        var restored = JsonUtility.FromJson<WiRRStudentSession>(JsonUtility.ToJson(session));
        restored.RecoverInterruptedTrial();
        var trial = restored.Lab(5).trials.Single();
        Assert.AreEqual("FAILED", trial.validity);
        Assert.AreEqual(384, trial.metrics.Single(m => m.name == "triangles_max").value);
        Assert.IsFalse(trial.metrics.Single(m => m.name == "gpu_draw_calls").available);
    }
}
