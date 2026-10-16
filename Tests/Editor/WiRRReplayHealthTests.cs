using System;
using System.Linq;
using KIA.WiRR;
using NUnit.Framework;
using UnityEngine;

public sealed class WiRRReplayHealthTests
{
    private static double Value(WiRRReplayHealth model, string name) => model.Metrics().Single(m => m.name == name).value;
    [Test] public void MissingSourceDoesNotBecomeFreshOrZeroAge()
    {
        var model = new WiRRReplayHealth(0.5); model.Observe(10);
        Assert.IsFalse(model.HasSource); Assert.IsFalse(model.IsStale); Assert.IsNull(model.Age);
        Assert.IsFalse(model.Metrics().Single(m => m.name == "receiver_age_max").available);
    }
    [Test] public void WatchdogUsesStrictThresholdAndRecordsPollingOvershoot()
    {
        var model = new WiRRReplayHealth(0.5); model.Receive(1); model.Observe(1.5);
        Assert.IsFalse(model.IsStale); model.Observe(1.6);
        Assert.IsTrue(model.IsStale);
        Assert.AreEqual(0.6, Value(model, "last_message_to_stale"), 1e-9);
        Assert.AreEqual(0.1, Value(model, "stale_detection_overshoot"), 1e-9);
    }
    [Test] public void GapLengthRecoveryAndStaleDurationAreDifferentMeasurements()
    {
        var model = new WiRRReplayHealth(0.5); model.Receive(0); model.BeginGap(0.1);
        model.Observe(0.6); model.EndGap(1.6); model.Receive(1.7);
        Assert.AreEqual(1.5, Value(model, "simulated_gap_duration"), 1e-9);
        Assert.AreEqual(0.1, Value(model, "resume_to_first_message"), 1e-9);
        Assert.AreEqual(1.1, Value(model, "first_stale_duration"), 1e-9);
        Assert.AreEqual(1, model.Recoveries); Assert.IsFalse(model.IsStale);
    }
    [Test] public void SecondEpisodeIsCountedWithoutOverwritingFirstEvent()
    {
        var model = new WiRRReplayHealth(0.5); model.Receive(0); model.Observe(0.6); model.Receive(1);
        model.Observe(2); model.Receive(3);
        Assert.AreEqual(2, model.StaleEpisodes); Assert.AreEqual(2, model.Recoveries);
        Assert.AreEqual(0.6, Value(model, "last_message_to_stale"), 1e-9);
        Assert.AreEqual(0.4, Value(model, "first_stale_duration"), 1e-9);
    }
    [Test] public void ResumeWithoutMessageDoesNotInventRecovery()
    {
        var model = new WiRRReplayHealth(0.5); model.Receive(0); model.BeginGap(0.1); model.EndGap(2);
        Assert.IsTrue(model.IsStale); Assert.AreEqual(0, model.Recoveries);
        Assert.IsFalse(model.Metrics().Single(m => m.name == "resume_to_first_message").available);
        Assert.IsFalse(model.Metrics().Single(m => m.name == "rtt").available);
        Assert.IsFalse(model.Metrics().Single(m => m.name == "source_to_receiver_latency").available);
    }
    [Test] public void InterarrivalIncludesTheGapAndDoesNotResetAtReplayLoop()
    {
        var model = new WiRRReplayHealth(0.5); model.Receive(0); model.Receive(0.1); model.Receive(1.6);
        Assert.AreEqual(800, Value(model, "interarrival_median"), 1e-8);
        Assert.AreEqual(3, model.Received);
    }
    [Test] public void ClockAndGapProtocolRejectInvalidSequences()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new WiRRReplayHealth(0));
        var model = new WiRRReplayHealth(0.5); model.Observe(1);
        Assert.Throws<ArgumentOutOfRangeException>(() => model.Receive(0.9));
        Assert.Throws<ArgumentOutOfRangeException>(() => model.Observe(double.NaN));
        Assert.Throws<InvalidOperationException>(() => model.EndGap(2));
        model.BeginGap(2);
        Assert.Throws<InvalidOperationException>(() => model.BeginGap(2));
        model.EndGap(3); Assert.Throws<InvalidOperationException>(() => model.EndGap(3));
    }
    [Test] public void RestartRetainsObservedStaleWithoutFabricatingARecovery()
    {
        var model = new WiRRReplayHealth(0.5); model.Receive(0); model.BeginGap(0.1); model.Observe(0.6);
        var session = new WiRRStudentSession { block = 3 };
        session.BeginTrial(6, "ROS_REPLAY", "SOLO", "paused-source/1"); session.pendingTrial.trial.metrics = model.Metrics();
        var restored = JsonUtility.FromJson<WiRRStudentSession>(JsonUtility.ToJson(session)); restored.RecoverInterruptedTrial();
        var trial = restored.Lab(6).trials.Single();
        Assert.AreEqual("FAILED", trial.validity);
        Assert.AreEqual(1, trial.metrics.Single(m => m.name == "stale_episodes").value);
        Assert.IsFalse(trial.metrics.Single(m => m.name == "resume_to_first_message").available);
    }
}
