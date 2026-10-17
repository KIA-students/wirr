using System;
using System.Linq;
using KIA.WiRR;
using NUnit.Framework;
using UnityEngine;

public sealed class WiRRDepthExperimentTests
{
    private static WiRRDepthObservation Visible() => new WiRRDepthObservation { available = true,
        targetDistance = 3, otherSurfaceDistance = null, referenceVisible = true, modelVisible = true, reportedFirstHit = true };
    private static WiRRDepthObservation Hidden() => new WiRRDepthObservation { available = true,
        targetDistance = 5, otherSurfaceDistance = 3, referenceVisible = false, modelVisible = true, reportedFirstHit = false };

    [Test] public void FalseAlarmsAndMissedVisibleAreDistinct()
    {
        var experiment = new WiRRDepthExperiment(new[] { Hidden(), Visible(), Visible() });
        experiment.Answer(0, true); experiment.Answer(1, false); experiment.Answer(2, true);
        Assert.AreEqual(1, experiment.FalseAlarms);
        Assert.AreEqual(1, experiment.MissedVisible);
        Assert.AreEqual(1, experiment.Correct);
        Assert.IsTrue(experiment.IsComplete);
        Assert.AreEqual(1, experiment.Metrics().Single(m => m.name == "model_visibility_errors").value);
    }

    [Test] public void FaultyRayClaimCannotChangeTheReference()
    {
        var observation = Visible(); observation.reportedFirstHit = false;
        var experiment = new WiRRDepthExperiment(new[] { observation });
        observation.referenceVisible = false; // mutation of the caller must not alter the snapshot
        experiment.Answer(0, true);
        Assert.AreEqual(1, experiment.Correct);
        Assert.AreEqual(1, experiment.Metrics().Single(m => m.name == "reported_ray_errors").value);
    }

    [Test] public void MissingRayIsUnknownInsteadOfVisibleOrZeroDistance()
    {
        var experiment = new WiRRDepthExperiment(new[] { new WiRRDepthObservation() });
        experiment.Answer(0, true);
        Assert.IsTrue(experiment.HasMissingSource);
        Assert.AreEqual(0, experiment.Correct);
        Assert.AreEqual(0, experiment.FalseAlarms);
        Assert.IsFalse(experiment.Metrics().Single(m => m.name == "object_1_target_distance").available);
        Assert.IsFalse(experiment.Metrics().Single(m => m.name == "object_1_reference_visible").available);
    }

    [Test] public void NoOtherSurfaceIsDifferentFromMissingTarget()
    {
        Assert.IsTrue(WiRRDepthExperiment.PredictVisible(3, null, 0));
        var experiment = new WiRRDepthExperiment(new[] { Visible() });
        var metrics = experiment.Metrics();
        Assert.IsTrue(metrics.Single(m => m.name == "object_1_target_distance").available);
        Assert.IsFalse(metrics.Single(m => m.name == "object_1_other_surface_distance").available);
    }

    [TestCase(0, false)] [TestCase(1.99, false)] [TestCase(2, true)]
    public void ToleranceChangesOnlyTheModelDecision(double tolerance, bool expected)
    {
        Assert.AreEqual(expected, WiRRDepthExperiment.PredictVisible(5, 3, tolerance));
        Assert.Throws<ArgumentOutOfRangeException>(() => WiRRDepthExperiment.PredictVisible(double.NaN, 3, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => WiRRDepthExperiment.PredictVisible(5, 3, -1));
    }

    [Test] public void DuplicateResponsesAreNotCountedTwice()
    {
        var experiment = new WiRRDepthExperiment(new[] { Visible() });
        experiment.Answer(0, true);
        Assert.Throws<InvalidOperationException>(() => experiment.Answer(0, false));
        Assert.AreEqual(1, experiment.Answered);
        Assert.AreEqual(1, experiment.Correct);
    }

    [Test] public void InvalidatedReferenceRetainsAnswerButRemovesItsScore()
    {
        var experiment = new WiRRDepthExperiment(new[] { Visible() });
        experiment.Answer(0, true); experiment.InvalidateReference(0);
        Assert.AreEqual(1, experiment.Answered);
        Assert.AreEqual(0, experiment.Correct);
        Assert.IsTrue(experiment.HasMissingSource);
    }

    [Test] public void PartialResponsesSurviveRestartWithoutAnInventedAnswer()
    {
        var experiment = new WiRRDepthExperiment(new[] { Hidden(), Visible(), Visible() });
        experiment.Answer(0, false);
        var session = new WiRRStudentSession { block = 2 };
        session.BeginTrial(4, "SIM_AR", "SOLO", "depth-center-ray/1");
        session.pendingTrial.trial.metrics = experiment.Metrics();
        var restored = JsonUtility.FromJson<WiRRStudentSession>(JsonUtility.ToJson(session));
        restored.RecoverInterruptedTrial();
        var trial = restored.Lab(4).trials.Single();
        Assert.AreEqual("FAILED", trial.validity);
        Assert.AreEqual(1, trial.metrics.Single(m => m.name == "answers_recorded").value);
        Assert.IsFalse(trial.metrics.Single(m => m.name == "object_2_answer_visible").available);
    }
}
