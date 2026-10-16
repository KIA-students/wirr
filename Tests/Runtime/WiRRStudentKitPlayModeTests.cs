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

    [UnityTest] public IEnumerator CadLodSwitchesActualMeshAndReturnsToHigh()
    {
        var parent = new GameObject("LOD test root");
        var world = new WiRRStudentWorld(parent.transform, 5, 10, 0, true, false, false, 0);
        try
        {
            foreach (var distance in new[] { 3f, 7f, 12f, 3f })
            {
                world.SetLod(WiRRStudentWorld.DistanceLod(distance, 6, 10));
                yield return null;
                var expected = distance < 6 ? 0 : distance < 10 ? 1 : 2;
                Assert.AreEqual(expected, world.ActiveLod);
                Assert.AreEqual(expected == 0 ? 96 * 8 : expected == 1 ? 24 * 8 : 6 * 4, world.activeMesh.triangles.Length / 3);
                Assert.AreEqual(world.activeMesh.triangles.Length / 3, world.ActiveTriangleCount);
            }
        }
        finally { world.Dispose(); Object.Destroy(parent); }
        yield return null;
    }

    [UnityTest] public IEnumerator InteractionTeleportHasStableHomeAndDoesNotMoveTheTask()
    {
        var parent = new GameObject("Teleport preview test");
        var world = new WiRRStudentWorld(parent.transform, 2, 10, 0, true, false, false, 0);
        try
        {
            var home = world.camera.transform.position;
            var target = world.targets[0].transform.position;
            var socket = world.socket.position;
            world.TeleportPreview(0); yield return null;
            Assert.AreNotEqual(home, world.camera.transform.position);
            world.TeleportPreview(1); yield return null;
            Assert.AreEqual(target, world.targets[0].transform.position);
            Assert.AreEqual(socket, world.socket.position);
            world.TeleportPreview(2); yield return null;
            Assert.AreEqual(home, world.camera.transform.position);
        }
        finally { world.Dispose(); Object.Destroy(parent); }
        yield return null;
    }
}
