using System;
using System.Linq;
using KIA.WiRR;
using NUnit.Framework;
using UnityEngine;

public sealed class WiRRStudioRunTests
{
    [Test] public void CompletedGoalDoesNotFinishPresentationBeforeSixtySeconds()
    {
        var run = new WiRRStudioRun(10, true, false); run.Observe(13, true);
        Assert.IsTrue(run.GoalReached); Assert.IsFalse(run.CanFinish);
        run.Observe(70, true); Assert.IsTrue(run.CanFinish);
        Assert.AreEqual(3, run.Metrics().Single(m => m.name == "time_to_goal").value);
    }
    [Test] public void PresentationAcceptsNinetyAndRejectsOverrun()
    {
        var run = new WiRRStudioRun(0, true, false); run.Observe(5, true);
        run.Observe(90, true); Assert.IsTrue(run.CanFinish);
        run.Observe(90.001, true); Assert.IsTrue(run.Overdue); Assert.IsFalse(run.CanFinish);
    }
    [Test] public void ElapsedPresentationWindowCannotReplaceGoalAchievement()
    {
        var run = new WiRRStudioRun(0, true, false); run.Observe(65, false);
        Assert.IsTrue(run.InPresentationWindow); Assert.IsFalse(run.CanFinish);
        Assert.IsFalse(run.Metrics().Single(m => m.name == "time_to_goal").available);
    }
    [Test] public void OrdinaryTrialCanFinishImmediatelyAfterTheGoal()
    {
        var run = new WiRRStudioRun(0, false, false); run.Observe(3, true);
        Assert.IsTrue(run.CanFinish);
        Assert.AreEqual(0, run.Metrics().Single(m => m.name == "presentation_window_met").value);
    }
    [Test] public void SimulatedAssistanceDoesNotBecomeAReportedHumanHintOrTime()
    {
        var run = new WiRRStudioRun(0, false, true); run.Hint(1); run.Observe(3, true);
        Assert.AreEqual(1, run.SimulatedHints); Assert.AreEqual(0, run.ReportedHints);
        Assert.IsFalse(run.Metrics().Single(m => m.name == "author_reported_hints").available);
        Assert.IsFalse(run.Metrics().Single(m => m.name == "time_to_goal").available);
    }
    [Test] public void PartialHintReportSurvivesRestartWithoutInventingSuccess()
    {
        var run = new WiRRStudioRun(0, true, false); run.Hint(2);
        var session = new WiRRStudentSession { block = 4 };
        session.BeginTrial(8, "DESKTOP_SIM", "HUMAN", "desktop-studio/1"); session.pendingTrial.trial.metrics = run.Metrics();
        var restored = JsonUtility.FromJson<WiRRStudentSession>(JsonUtility.ToJson(session)); restored.RecoverInterruptedTrial();
        var trial = restored.Lab(8).trials.Single(); Assert.AreEqual("FAILED", trial.validity);
        Assert.AreEqual(1, trial.metrics.Single(m => m.name == "author_reported_hints").value);
        Assert.AreEqual(0, trial.metrics.Single(m => m.name == "studio_goal_achieved").value);
    }
    [Test] public void InvalidClockDoesNotAlterTheFirstGoalTime()
    {
        var run = new WiRRStudioRun(0, false, false); run.Observe(3, true);
        Assert.Throws<ArgumentOutOfRangeException>(() => run.Hint(2));
        Assert.Throws<ArgumentOutOfRangeException>(() => run.Observe(double.NaN, false));
        Assert.AreEqual(3, run.Metrics().Single(m => m.name == "time_to_goal").value);
        Assert.AreEqual(0, run.ReportedHints);
    }
}
