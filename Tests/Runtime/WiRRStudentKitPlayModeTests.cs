using System.Collections;
using System.Linq;
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

    [UnityTest] public IEnumerator AnchorViewChangesPreserveReferenceAndHideMissingPose()
    {
        var parent = new GameObject("SIM_AR reference test");
        var world = new WiRRStudentWorld(parent.transform, 3, 10, 0, true, false, false, 0);
        try
        {
            var reference = world.anchorReference.position;
            world.SetAnchorObservation(0); var firstView = world.camera.transform.position;
            world.SetAnchorPose(new Vector3(0.02f, 0, 0), true); yield return null;
            Assert.Less(Vector3.Distance(reference + new Vector3(0.02f, 0, 0), world.targets[0].transform.position), 1e-6f);
            world.SetAnchorObservation(2); world.SetAnchorPose(Vector3.zero, false); yield return null;
            Assert.AreNotEqual(firstView, world.camera.transform.position);
            Assert.AreEqual(reference, world.anchorReference.position);
            Assert.IsFalse(world.targets[0].activeSelf, "Nie rysujemy nieaktualnej kotwicy jako poprawnej pozycji.");
            world.SetAnchorPose(new Vector3(0.03f, 0, 0), true); yield return null;
            Assert.IsTrue(world.targets[0].activeSelf);
            Assert.AreEqual(reference, world.anchorReference.position);
        }
        finally { world.Dispose(); Object.Destroy(parent); }
        yield return null;
    }

    [UnityTest] public IEnumerator DepthReferenceIsIndependentOfRenderingLabelsAndLight()
    {
        for (var view = 0; view < 2; view++)
        for (var variant = 0; variant < 2; variant++)
        {
            var parent = new GameObject("Depth geometry test");
            var world = new WiRRStudentWorld(parent.transform, 4, 10, variant, true, false, false, view);
            try
            {
                var baseline = world.ConfigureDepth(0, false, true, true);
                yield return null;
                Assert.IsTrue(baseline.All(o => o.available));
                CollectionAssert.AreEqual(new[] { view == 1, true, true }, baseline.Select(o => o.referenceVisible));
                Assert.IsFalse(baseline[2].reportedFirstHit, "Błędny komunikat nie jest referencją geometrii.");
                Assert.AreEqual("3", world.targets[1].GetComponentInChildren<TextMesh>().text);
                Assert.AreEqual(variant == 0 || view == 1, baseline[0].modelVisible);
                var dimmed = world.ConfigureDepth(0, true, false, false);
                CollectionAssert.AreEqual(baseline.Select(o => o.referenceVisible), dimmed.Select(o => o.referenceVisible));
                Assert.AreEqual("2", world.targets[1].GetComponentInChildren<TextMesh>().text);
                Assert.IsTrue(dimmed[2].reportedFirstHit);
                var loose = world.ConfigureDepth(2, true, false, false);
                Assert.IsTrue(loose[0].modelVisible);
                Assert.AreEqual(view == 1, loose[0].referenceVisible);
                world.targets[0].GetComponent<Collider>().enabled = false;
                Physics.SyncTransforms();
                Assert.IsFalse(world.ProbeDepth(0).available);
            }
            finally { world.Dispose(); Object.Destroy(parent); }
            yield return null;
        }
    }
    [UnityTest] public IEnumerator CadMaterialsPreserveMeshAndReferenceScale()
    {
        var parent = new GameObject("CAD material test");
        var world = new WiRRStudentWorld(parent.transform, 5, 10, 0, true, false, false, 0);
        try
        {
            var vertices = world.activeMesh.vertices;
            var triangles = world.activeMesh.triangles.OrderBy(x => x).ToArray();
            var bounds = world.activeMesh.bounds;
            world.ConfigureCad(4, true);
            yield return null;
            Assert.AreEqual(4, world.CadMaterialCount);
            Assert.AreEqual(4, world.activeMesh.subMeshCount);
            CollectionAssert.AreEqual(vertices, world.activeMesh.vertices);
            CollectionAssert.AreEqual(triangles, world.activeMesh.triangles.OrderBy(x => x).ToArray());
            Assert.AreEqual(bounds, world.activeMesh.bounds);
            Assert.AreEqual(2, Vector3.Distance(world.camera.transform.position, world.lod.transform.position), 1e-5);
            world.SetLod(2); Assert.AreEqual(4, world.activeMesh.subMeshCount);
            world.ConfigureCad(1, false); world.SetLod(0);
            Assert.AreEqual(1, world.activeMesh.subMeshCount);
            CollectionAssert.AreEqual(triangles, world.activeMesh.triangles.OrderBy(x => x).ToArray());
            foreach (var level in world.lod.GetLODs())
                Assert.AreEqual(1, level.renderers[0].sharedMaterials.Length);
        }
        finally { world.Dispose(); Object.Destroy(parent); }
        yield return null;
    }

    [UnityTest] public IEnumerator ReplayStaleReactionPreservesPoseAndRestoresVisibility()
    {
        var parent = new GameObject("REPLAY status test");
        var world = new WiRRStudentWorld(parent.transform, 6, 10, 0, true, false, false, 0);
        try
        {
            world.robotJoint1.localRotation = Quaternion.Euler(0, 0, 35);
            var pose = world.robotJoint1.localRotation;
            world.SetReplayStatus(false, false, false); yield return null;
            Assert.IsFalse(world.robotJoint1.gameObject.activeSelf);
            world.SetReplayStatus(true, true, false); yield return null;
            Assert.IsTrue(world.robotJoint1.gameObject.activeSelf);
            Assert.AreEqual(pose, world.robotJoint1.localRotation);
            world.SetReplayStatus(true, true, true); yield return null;
            Assert.IsFalse(world.robotJoint1.gameObject.activeSelf);
            world.SetReplayStatus(true, false, true); yield return null;
            Assert.IsTrue(world.robotJoint1.gameObject.activeSelf);
            Assert.AreEqual(pose, world.robotJoint1.localRotation);
        }
        finally { world.Dispose(); Object.Destroy(parent); }
        yield return null;
    }

    [UnityTest] public IEnumerator MissionTransferChangesDistanceWithoutMovingGoals()
    {
        var parent = new GameObject("Mission transfer test");
        var world = new WiRRStudentWorld(parent.transform, 7, 10, 0, true, false, false, 0);
        try
        {
            world.ConfigureMission(0, 0, true);
            var positions = world.targets.Select(t => t.transform.position).ToArray();
            var camera = world.camera.transform.position;
            world.targets[0].transform.localScale *= 1.5f;
            var scale = world.targets[0].transform.localScale;
            world.ConfigureMission(1, 0, true); yield return null;
            CollectionAssert.AreEqual(positions, world.targets.Select(t => t.transform.position).ToArray());
            Assert.AreEqual(4, camera.z - world.camera.transform.position.z, 1e-6);
            Assert.AreEqual(scale, world.targets[0].transform.localScale);
            world.HighlightMissionTarget(1, true);
            Assert.AreEqual(Color.yellow, world.targets[1].GetComponent<Renderer>().sharedMaterial.color);
            Assert.AreNotEqual(Color.yellow, world.targets[0].GetComponent<Renderer>().sharedMaterial.color);
        }
        finally { world.Dispose(); Object.Destroy(parent); }
        yield return null;
    }

}
