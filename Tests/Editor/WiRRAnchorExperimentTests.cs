using System;
using System.Linq;
using KIA.WiRR;
using NUnit.Framework;
using UnityEngine;

public sealed class WiRRAnchorExperimentTests
{
    private static WiRRAnchorDataset Data() => JsonUtility.FromJson<WiRRAnchorDataset>(Resources.Load<TextAsset>("WiRR2/anchor-tracking").text);
    private static void Before(WiRRAnchorExperiment experiment)
    { experiment.Place(0); experiment.Tick(1); experiment.Tick(2); experiment.Tick(3); }

    [Test] public void BaselineRequiresPlacementAndThreeSeparateReadings()
    {
        var experiment = new WiRRAnchorExperiment(Data(), 0, false, false);
        Assert.IsFalse(experiment.Tick(0));
        Assert.AreEqual(0, experiment.Before.Count);
        experiment.Place(0);
        Assert.IsFalse(experiment.Tick(0.5));
        experiment.Tick(1);
        Assert.IsFalse(experiment.Metrics().Single(m => m.name == "before_scatter_rms").available);
        experiment.Tick(2); experiment.Tick(3);
        Assert.AreEqual(WiRRAnchorPhase.Complete, experiment.Phase);
        Assert.AreEqual(3, experiment.Before.Count);
        Assert.AreEqual(0, experiment.After.Count);
        Assert.IsFalse(experiment.Metrics().Single(m => m.name == "relocalization_time").available);
        Assert.IsFalse(experiment.Tick(4));
        Assert.Throws<InvalidOperationException>(() => experiment.Place(4));
    }

    [TestCase(0)] [TestCase(1)]
    public void StableReadingsCanStillHaveNonzeroSystematicError(int profile)
    {
        var data = Data();
        var experiment = new WiRRAnchorExperiment(data, profile, false, false);
        Before(experiment);
        var metrics = experiment.Metrics();
        Assert.AreEqual(data.profiles[profile].bias_m.magnitude * 100,
            metrics.Single(m => m.name == "before_bias_norm").value, 1e-5);
        Assert.Greater(metrics.Single(m => m.name == "before_scatter_rms").value, 0);
        Assert.Greater(metrics.Single(m => m.name == "before_mean_error").value, 0);
    }

    [Test] public void RelocalizationSeparatesBeforeLossAndAfterRecovery()
    {
        var experiment = new WiRRAnchorExperiment(Data(), 0, true, false);
        Before(experiment);
        Assert.AreEqual(WiRRAnchorPhase.TrackingLost, experiment.Phase);
        Assert.IsNull(experiment.Latest);
        Assert.IsFalse(experiment.Metrics().Single(m => m.name == "pose_during_tracking_loss").available);
        experiment.Tick(4.5);
        Assert.AreEqual(WiRRAnchorPhase.AfterRecovery, experiment.Phase);
        Assert.IsNull(experiment.Latest, "Nie pokazujemy starego odczytu jako nowego po powrocie.");
        experiment.Tick(5.5); experiment.Tick(6.5); experiment.Tick(7.5);
        Assert.AreEqual(WiRRAnchorPhase.Complete, experiment.Phase);
        Assert.AreEqual(3, experiment.Before.Count); Assert.AreEqual(3, experiment.After.Count);
        var metrics = experiment.Metrics();
        Assert.AreEqual(1.5, metrics.Single(m => m.name == "relocalization_time").value);
        Assert.Greater(metrics.Single(m => m.name == "after_bias_norm").value, metrics.Single(m => m.name == "before_bias_norm").value);
    }

    [Test] public void FailedRecoveryDoesNotBecomeZeroSecondsOrZeroError()
    {
        var experiment = new WiRRAnchorExperiment(Data(), 1, true, true);
        Before(experiment); experiment.Tick(8);
        Assert.AreEqual(WiRRAnchorPhase.Failed, experiment.Phase);
        Assert.AreEqual(3, experiment.Before.Count); Assert.AreEqual(0, experiment.After.Count);
        Assert.IsNull(experiment.Latest);
        Assert.IsFalse(experiment.Metrics().Single(m => m.name == "relocalization_time").available);
        Assert.IsFalse(experiment.Metrics().Single(m => m.name == "after_mean_error").available);
    }

    [Test] public void StalledFramesNeverBackfillUnobservedReadings()
    {
        var experiment = new WiRRAnchorExperiment(Data(), 0, false, false);
        experiment.Place(0); experiment.Tick(100);
        Assert.AreEqual(1, experiment.Before.Count);
        Assert.AreEqual(100, experiment.Before[0].elapsed_s);
        Assert.IsFalse(experiment.Tick(100));
        experiment.Tick(101); Assert.AreEqual(2, experiment.Before.Count);
    }

    [Test] public void RecoveryObservedAfterTimeoutCannotClaimSuccess()
    {
        var experiment = new WiRRAnchorExperiment(Data(), 0, true, false);
        Before(experiment); experiment.Tick(10);
        Assert.AreEqual(WiRRAnchorPhase.Failed, experiment.Phase);
        Assert.IsFalse(experiment.Metrics().Single(m => m.name == "relocalization_time").available);
    }

    [Test] public void InvalidModelsAndNonmonotonicClocksAreRejected()
    {
        var data = Data(); data.source = "REAL_AR";
        Assert.Throws<ArgumentException>(() => new WiRRAnchorExperiment(data, 0, false, false));
        var experiment = new WiRRAnchorExperiment(Data(), 0, false, false);
        Assert.Throws<ArgumentOutOfRangeException>(() => experiment.Place(double.NaN));
        experiment.Place(10);
        Assert.Throws<ArgumentOutOfRangeException>(() => experiment.Tick(9));
    }

    [Test] public void PartialReadingsSurviveSessionRecoveryWithoutCompletingTheExperiment()
    {
        var experiment = new WiRRAnchorExperiment(Data(), 0, true, false);
        experiment.Place(0); experiment.Tick(1); experiment.Tick(2);
        var session = new WiRRStudentSession { block = 2 };
        session.BeginTrial(3, "SIM_AR", "SOLO", "anchor-model/1");
        session.pendingTrial.trial.metrics = experiment.Metrics();
        var restored = JsonUtility.FromJson<WiRRStudentSession>(JsonUtility.ToJson(session));
        restored.RecoverInterruptedTrial();
        var trial = restored.Lab(3).trials.Single();
        Assert.AreEqual("FAILED", trial.validity);
        Assert.AreEqual(2, trial.metrics.Single(m => m.name == "before_sample_count").value);
        Assert.IsFalse(trial.metrics.Single(m => m.name == "before_sample_3_dx").available);
        Assert.IsFalse(trial.metrics.Single(m => m.name == "elapsed").available);
    }
}
