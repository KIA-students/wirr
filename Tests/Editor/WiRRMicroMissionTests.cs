using System;
using System.Linq;
using KIA.WiRR;
using NUnit.Framework;
using UnityEngine;

public sealed class WiRRMicroMissionTests
{
    [Test] public void WrongSelectionRemainsInTheCompletedStep()
    {
        var task = new WiRRMicroMission(10, false, false);
        task.Select(2, 11); task.Select(-1, 12); task.Select(0, 13);
        Assert.AreEqual(1, task.Step); Assert.AreEqual(2, task.Errors);
        Assert.AreEqual(3, task.Metrics().Single(m => m.name == "mission_step_1_attempts").value);
        Assert.AreEqual(0, task.Metrics().Single(m => m.name == "mission_step_1_first_answer_correct").value);
        Assert.AreEqual(3, task.Metrics().Single(m => m.name == "mission_step_1_completed_at").value);
    }
    [Test] public void ClassificationRecordsWrongAnswersOnceWithoutRetryingThem()
    {
        var task = new WiRRMicroMission(0, true, false);
        task.Classify(false, 1); task.Classify(false, 2); task.Classify(true, 3);
        Assert.IsTrue(task.IsComplete); Assert.AreEqual(2, task.Successes); Assert.AreEqual(1, task.Errors);
        Assert.AreEqual(0, task.Metrics().Single(m => m.name == "mission_goal_achieved").value);
        Assert.AreEqual(1, task.Metrics().Single(m => m.name == "mission_step_1_attempts").value);
    }
    [Test] public void TimeoutBoundaryDoesNotInventCompletion()
    {
        var task = new WiRRMicroMission(10, false, false);
        task.Select(0, 99.9); task.Select(1, 100);
        Assert.IsTrue(task.TimedOut); Assert.AreEqual(1, task.Step);
        Assert.IsFalse(task.Metrics().Single(m => m.name == "mission_step_2_completed_at").available);
    }
    [Test] public void SimulationDoesNotReportHumanStepTimes()
    {
        var task = new WiRRMicroMission(0, false, true); task.Select(0, 1);
        Assert.AreEqual(1, task.Step);
        Assert.IsFalse(task.Metrics().Single(m => m.name == "mission_step_1_completed_at").available);
    }
    [Test] public void CompletedMissionCannotAccumulateExtraSteps()
    {
        var task = new WiRRMicroMission(0, false, false);
        task.Select(0, 1); task.Select(1, 2); task.Select(2, 3); task.Select(0, 4);
        Assert.AreEqual(3, task.Step); Assert.AreEqual(3, task.Successes); Assert.AreEqual(0, task.Errors);
    }
    [Test] public void InvalidClockAndWrongInputProtocolAreRejected()
    {
        var task = new WiRRMicroMission(5, false, false);
        Assert.Throws<ArgumentOutOfRangeException>(() => task.Select(0, 4));
        Assert.Throws<ArgumentOutOfRangeException>(() => task.Tick(double.NaN));
        Assert.Throws<InvalidOperationException>(() => task.Classify(true, 6));
    }
    [Test] public void PartialErrorsSurviveRestartAsFailedEvidence()
    {
        var task = new WiRRMicroMission(0, false, false); task.Select(2, 1); task.Select(0, 2);
        var session = new WiRRStudentSession { block = 4 };
        session.BeginTrial(7, "DESKTOP_SIM", "SOLO", "desktop-three-steps/1"); session.pendingTrial.trial.metrics = task.Metrics();
        var restored = JsonUtility.FromJson<WiRRStudentSession>(JsonUtility.ToJson(session)); restored.RecoverInterruptedTrial();
        var trial = restored.Lab(7).trials.Single(); Assert.AreEqual("FAILED", trial.validity);
        Assert.AreEqual(1, trial.metrics.Single(m => m.name == "mission_errors").value);
        Assert.IsFalse(trial.metrics.Single(m => m.name == "mission_step_2_completed_at").available);
    }
}
