using System.Collections;
using KIA.WiRR;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public sealed class WiRRStudentKitPlayModeTests
{
    [UnityTest] public IEnumerator AllEightLessonsCreateADesktopWorld()
    {
        for (var lab = 1; lab <= 8; lab++)
        {
            var root = new GameObject("Student kit smoke " + lab);
            var controller = root.AddComponent<WiRRStudentLab>();
            controller.labNumber = lab;
            yield return null;
            Assert.AreEqual(1, root.GetComponentsInChildren<Camera>().Length, "Lab " + lab);
            Assert.IsNotNull(Resources.Load<TextAsset>("WiRR2/lab" + lab.ToString("00")));
            Assert.IsFalse(controller.IsTrialActive);
            Assert.AreEqual(0, controller.RecordedTrials, "Nie tworzymy fikcyjnych prób przy otwarciu");
            Object.Destroy(root);
            yield return null;
        }
    }
}
