using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KIA.WiRR.Editor
{
    /// <summary>
    /// Generates lightweight teaching prefabs in the student's Assets folder.
    /// The source package stays text-only and dependency-light, while students
    /// receive ordinary editable prefabs that can be inspected in the Hierarchy.
    /// </summary>
    internal static class WiRRTeachingAssetTools
    {
        private const string CommonRoot = "Assets/WiRR/Common";
        private const string TeachingRootName = "WiRR_TeachingAssets";

        private enum MaterialKind
        {
            Structure,
            Dark,
            Accent,
            Secondary,
            Warning,
            Error,
            Reference,
            Metal,
            Matte
        }

        private sealed class GeneratedAsset
        {
            public string Name;
            public Vector3 Position;
            public Quaternion Rotation;
            public Func<GameObject> Builder;

            public GeneratedAsset(string name, Vector3 position, Func<GameObject> builder)
            {
                Name = name;
                Position = position;
                Rotation = Quaternion.identity;
                Builder = builder;
            }

            public GeneratedAsset(string name, Vector3 position, Quaternion rotation, Func<GameObject> builder)
            {
                Name = name;
                Position = position;
                Rotation = rotation;
                Builder = builder;
            }
        }

        public static void CreateOrRepairEnvironment(int labNumber)
        {
            ValidateLabNumber(labNumber);
            WiRRSceneTools.PrepareBaseScene(labNumber);
            EnsureGeneratedFolders(labNumber);
            EnsureMaterials();

            var useArReferenceKit = labNumber == 3 || labNumber == 4;
            var prefabName = useArReferenceKit ? "WiRR_ARReferenceKit" : "WiRR_LabRoom";
            var prefab = EnsurePrefab(
                $"{CommonRoot}/Prefabs/Generated/{prefabName}.prefab",
                useArReferenceKit ? (Func<GameObject>)BuildARReferenceKit : BuildLabRoom);

            PlacePrefabInScene(labNumber, prefab, "Environment", Vector3.zero, Quaternion.identity);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"[WiRR] Przygotowano środowisko dydaktyczne Lab {labNumber:00}.");
        }

        public static void CreateOrRepairExperimentSet(int labNumber)
        {
            ValidateLabNumber(labNumber);
            WiRRSceneTools.PrepareBaseScene(labNumber);
            EnsureGeneratedFolders(labNumber);
            EnsureMaterials();

            var folder = GeneratedPrefabFolder(labNumber);
            foreach (var asset in GetLabAssets(labNumber))
            {
                var prefab = EnsurePrefab($"{folder}/{asset.Name}.prefab", asset.Builder);
                PlacePrefabInScene(labNumber, prefab, "Experiment", asset.Position, asset.Rotation);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"[WiRR] Przygotowano zestaw eksperymentalny Lab {labNumber:00}.");
        }

        public static void RemoveTeachingObjectsFromScene(int labNumber)
        {
            ValidateLabNumber(labNumber);
            var scene = SceneManager.GetActiveScene();
            if (!scene.IsValid())
                return;

            var labRoot = FindInScene(scene, $"WiRR_Lab{labNumber:00}");
            if (labRoot == null)
                return;

            var teaching = labRoot.transform.Find(TeachingRootName);
            if (teaching == null)
                return;

            Undo.DestroyObjectImmediate(teaching.gameObject);
            EditorSceneManager.MarkSceneDirty(scene);
            Debug.Log($"[WiRR] Usunięto obiekty dydaktyczne ze sceny Lab {labNumber:00}. Wygenerowane prefaby pozostają w Assets/WiRR.");
        }

        public static bool GeneratedPrefabsExist(int labNumber)
        {
            if (labNumber < 1 || labNumber > 7)
                return false;

            var folder = GeneratedPrefabFolder(labNumber);
            return AssetDatabase.IsValidFolder(folder) &&
                   AssetDatabase.FindAssets("t:Prefab", new[] { folder }).Length > 0;
        }

        private static IEnumerable<GeneratedAsset> GetLabAssets(int labNumber)
        {
            switch (labNumber)
            {
                case 1:
                    return new[]
                    {
                        new GeneratedAsset("L01_GeometryStressWall", new Vector3(-1.45f, 0f, 1.35f), BuildGeometryStressWall),
                        new GeneratedAsset("L01_PhysicsDropper", new Vector3(1.65f, 0f, 1.25f), BuildPhysicsDropper)
                    };
                case 2:
                    return new[]
                    {
                        new GeneratedAsset("L02_EnergySocket_Starter", new Vector3(0f, 0.8f, 1.4f), BuildEnergySocket),
                        new GeneratedAsset("L02_EnergyCell_Starter", new Vector3(-0.55f, 0.88f, 0.75f), () => BuildEnergyCell(false)),
                        new GeneratedAsset("L02_EnergyCell_Faulty", new Vector3(0.55f, 0.88f, 0.75f), () => BuildEnergyCell(true))
                    };
                case 3:
                    return new[]
                    {
                        new GeneratedAsset("L03_ARArtifact_Starter", new Vector3(0f, 0f, 1.1f), BuildARArtifact),
                        new GeneratedAsset("L03_GhostReference", new Vector3(0.65f, 0f, 1.1f), BuildGhostReference),
                        new GeneratedAsset("L03_CalibrationFrame_AB", new Vector3(-0.8f, 0f, 1.0f), BuildCalibrationFrame)
                    };
                case 4:
                    return new[]
                    {
                        new GeneratedAsset("L04_OcclusionBot", new Vector3(0f, 0f, 1.25f), BuildOcclusionBot),
                        new GeneratedAsset("L04_DepthProbe", new Vector3(-0.75f, 0f, 1.0f), BuildDepthProbe),
                        new GeneratedAsset("L04_LightingReference", new Vector3(0.85f, 0f, 1.0f), BuildLightingReference)
                    };
                case 5:
                    return new[]
                    {
                        new GeneratedAsset("L05_Gripper_Source", new Vector3(-1.35f, 0.8f, 1.2f), () => BuildGripper(0, false)),
                        new GeneratedAsset("L05_Gripper_LOD1Candidate", new Vector3(-0.45f, 0.8f, 1.2f), () => BuildGripper(1, false)),
                        new GeneratedAsset("L05_Gripper_LOD2Candidate", new Vector3(0.45f, 0.8f, 1.2f), () => BuildGripper(2, false)),
                        new GeneratedAsset("L05_Gripper_Faulty", new Vector3(1.35f, 0.8f, 1.2f), () => BuildGripper(0, true))
                    };
                case 6:
                    return new[]
                    {
                        new GeneratedAsset("L06_WiRRRobotArm_Visual", new Vector3(0f, 0f, 1.25f), BuildRobotArmVisual),
                        new GeneratedAsset("L06_JointAxisVisualizer", new Vector3(-1.0f, 0.8f, 1.25f), BuildJointAxisVisualizer),
                        new GeneratedAsset("L06_StateBeacon", new Vector3(1.0f, 0f, 1.25f), BuildStateBeacon)
                    };
                case 7:
                    return new[]
                    {
                        new GeneratedAsset("L07_XRTestBench", new Vector3(-0.65f, 0f, 1.25f), BuildXRTestBench),
                        new GeneratedAsset("L07_FaultInjectionConsole", new Vector3(0.85f, 0.8f, 1.25f), BuildFaultInjectionConsole)
                    };
                default:
                    throw new ArgumentOutOfRangeException(nameof(labNumber));
            }
        }

        private static GameObject BuildLabRoom()
        {
            var root = new GameObject("WiRR_LabRoom");

            Primitive(root.transform, "Floor", PrimitiveType.Cube, new Vector3(0f, -0.05f, 0f), new Vector3(6f, 0.10f, 8f), MaterialKind.Dark, true);
            Primitive(root.transform, "BackWall", PrimitiveType.Cube, new Vector3(0f, 1.5f, 3.95f), new Vector3(6f, 3f, 0.10f), MaterialKind.Structure, true);
            Primitive(root.transform, "SideWall", PrimitiveType.Cube, new Vector3(-2.95f, 1.5f, 1.0f), new Vector3(0.10f, 3f, 6f), MaterialKind.Structure, true);

            var bench = new GameObject("WiRR_Workbench_075m");
            bench.transform.SetParent(root.transform, false);
            Primitive(bench.transform, "Top", PrimitiveType.Cube, new Vector3(0f, 0.75f, 1.8f), new Vector3(2.2f, 0.08f, 0.8f), MaterialKind.Structure, true);
            foreach (var x in new[] { -0.95f, 0.95f })
            foreach (var z in new[] { 1.5f, 2.1f })
                Primitive(bench.transform, "Leg", PrimitiveType.Cube, new Vector3(x, 0.37f, z), new Vector3(0.08f, 0.74f, 0.08f), MaterialKind.Dark, true);

            var scale = new GameObject("ScaleReferences");
            scale.transform.SetParent(root.transform, false);
            Primitive(scale.transform, "MetricCube_1m", PrimitiveType.Cube, new Vector3(2.1f, 0.5f, 2.2f), Vector3.one, MaterialKind.Reference, false);
            BuildHumanReference(scale.transform, new Vector3(-2.0f, 0f, 2.1f));

            var grid = new GameObject("MetricGrid_0_5m");
            grid.transform.SetParent(root.transform, false);
            for (var i = -4; i <= 4; i++)
            {
                Primitive(grid.transform, $"GridX_{i}", PrimitiveType.Cube, new Vector3(i * 0.5f, 0.006f, 0f), new Vector3(0.008f, 0.008f, 4f), MaterialKind.Reference, false);
                Primitive(grid.transform, $"GridZ_{i}", PrimitiveType.Cube, new Vector3(0f, 0.006f, i * 0.5f), new Vector3(4f, 0.008f, 0.008f), MaterialKind.Reference, false);
            }

            return root;
        }

        private static GameObject BuildARReferenceKit()
        {
            var root = new GameObject("WiRR_ARReferenceKit");
            Primitive(root.transform, "MetricCube_10cm", PrimitiveType.Cube, new Vector3(0f, 0.05f, 0f), new Vector3(0.1f, 0.1f, 0.1f), MaterialKind.Reference, false);
            Primitive(root.transform, "Reference_A", PrimitiveType.Sphere, new Vector3(-0.25f, 0.025f, 0f), Vector3.one * 0.05f, MaterialKind.Accent, false);
            Primitive(root.transform, "Reference_B", PrimitiveType.Cube, new Vector3(0.25f, 0.025f, 0f), Vector3.one * 0.05f, MaterialKind.Secondary, false);
            Primitive(root.transform, "Baseline_AB_0_5m", PrimitiveType.Cube, new Vector3(0f, 0.01f, 0f), new Vector3(0.5f, 0.01f, 0.01f), MaterialKind.Structure, false);
            return root;
        }

        private static GameObject BuildGeometryStressWall()
        {
            var root = new GameObject("L01_GeometryStressWall");
            Primitive(root.transform, "Panel", PrimitiveType.Cube, new Vector3(0f, 1.15f, 0f), new Vector3(2.2f, 1.3f, 0.12f), MaterialKind.Dark, true);

            for (var y = 0; y < 6; y++)
            for (var x = 0; x < 8; x++)
            {
                var p = new Vector3(-0.84f + x * 0.24f, 0.72f + y * 0.18f, -0.09f);
                var type = (x + y) % 3 == 0 ? PrimitiveType.Cylinder : (x + y) % 3 == 1 ? PrimitiveType.Cube : PrimitiveType.Sphere;
                var part = Primitive(root.transform, $"Part_{y:00}_{x:00}", type, p, Vector3.one * 0.10f, (x + y) % 2 == 0 ? MaterialKind.Accent : MaterialKind.Secondary, false);
                if (type == PrimitiveType.Cylinder)
                    part.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            }

            var hint = new GameObject("Duplicate_this_prefab_for_controlled_object-count_tests");
            hint.transform.SetParent(root.transform, false);
            return root;
        }

        private static GameObject BuildPhysicsDropper()
        {
            var root = new GameObject("L01_PhysicsDropper");
            Primitive(root.transform, "Base", PrimitiveType.Cube, new Vector3(0f, 0.05f, 0f), new Vector3(1.4f, 0.1f, 1.1f), MaterialKind.Dark, true);
            Primitive(root.transform, "PostLeft", PrimitiveType.Cube, new Vector3(-0.6f, 1.0f, 0f), new Vector3(0.08f, 2f, 0.08f), MaterialKind.Structure, true);
            Primitive(root.transform, "PostRight", PrimitiveType.Cube, new Vector3(0.6f, 1.0f, 0f), new Vector3(0.08f, 2f, 0.08f), MaterialKind.Structure, true);
            Primitive(root.transform, "Header", PrimitiveType.Cube, new Vector3(0f, 1.95f, 0f), new Vector3(1.28f, 0.08f, 0.08f), MaterialKind.Structure, true);

            var bodyIndex = 0;
            for (var y = 0; y < 4; y++)
            for (var x = 0; x < 6; x++)
            {
                var type = bodyIndex % 2 == 0 ? PrimitiveType.Cube : PrimitiveType.Sphere;
                var body = Primitive(
                    root.transform,
                    $"DynamicBody_{bodyIndex:00}",
                    type,
                    new Vector3(-0.45f + x * 0.18f, 0.35f + y * 0.19f, 0f),
                    Vector3.one * 0.14f,
                    bodyIndex % 3 == 0 ? MaterialKind.Warning : MaterialKind.Accent,
                    true);
                var rb = body.AddComponent<Rigidbody>();
                rb.mass = 0.2f;
                bodyIndex++;
            }

            return root;
        }

        private static GameObject BuildEnergyCell(bool faulty)
        {
            var root = new GameObject(faulty ? "L02_EnergyCell_Faulty" : "L02_EnergyCell_Starter");
            var bodyKind = faulty ? MaterialKind.Error : MaterialKind.Accent;

            Primitive(root.transform, "Body", faulty ? PrimitiveType.Sphere : PrimitiveType.Cube, new Vector3(0f, 0f, 0f), faulty ? new Vector3(0.17f, 0.17f, 0.17f) : new Vector3(0.12f, 0.18f, 0.08f), bodyKind, true);
            Primitive(root.transform, "TerminalPlus", PrimitiveType.Cylinder, new Vector3(0f, 0.11f, 0f), new Vector3(0.035f, 0.025f, 0.035f), MaterialKind.Metal, false);

            var attach = new GameObject("AttachPoint");
            attach.transform.SetParent(root.transform, false);
            attach.transform.localPosition = new Vector3(0f, 0f, 0f);

            var rb = root.AddComponent<Rigidbody>();
            rb.mass = faulty ? 0.5f : 0.25f;
            rb.interpolation = RigidbodyInterpolation.Interpolate;

            var note = new GameObject(faulty ? "FAULT_hint_wrong_shape_or_interaction_layer" : "Student_adds_XRGrabInteractable");
            note.transform.SetParent(root.transform, false);
            return root;
        }

        private static GameObject BuildEnergySocket()
        {
            var root = new GameObject("L02_EnergySocket_Starter");
            Primitive(root.transform, "Backplate", PrimitiveType.Cube, Vector3.zero, new Vector3(0.45f, 0.35f, 0.08f), MaterialKind.Dark, true);
            Primitive(root.transform, "LeftGuide", PrimitiveType.Cube, new Vector3(-0.105f, 0f, -0.065f), new Vector3(0.035f, 0.22f, 0.08f), MaterialKind.Accent, false);
            Primitive(root.transform, "RightGuide", PrimitiveType.Cube, new Vector3(0.105f, 0f, -0.065f), new Vector3(0.035f, 0.22f, 0.08f), MaterialKind.Accent, false);
            Primitive(root.transform, "BottomGuide", PrimitiveType.Cube, new Vector3(0f, -0.095f, -0.065f), new Vector3(0.24f, 0.03f, 0.08f), MaterialKind.Accent, false);

            var socketPoint = new GameObject("SocketPoint");
            socketPoint.transform.SetParent(root.transform, false);
            socketPoint.transform.localPosition = new Vector3(0f, 0f, -0.10f);

            var note = new GameObject("Student_adds_XRSocketInteractor_and_filtering");
            note.transform.SetParent(root.transform, false);
            return root;
        }

        private static GameObject BuildARArtifact()
        {
            var root = new GameObject("L03_ARArtifact_Starter");
            BuildMiniRobot(root.transform, false, MaterialKind.Accent);
            var anchorPoint = new GameObject("AnchorPoint");
            anchorPoint.transform.SetParent(root.transform, false);
            anchorPoint.transform.localPosition = Vector3.zero;
            return root;
        }

        private static GameObject BuildGhostReference()
        {
            var root = new GameObject("L03_GhostReference");
            Primitive(root.transform, "BodyFrameTop", PrimitiveType.Cube, new Vector3(0f, 0.24f, 0f), new Vector3(0.28f, 0.015f, 0.18f), MaterialKind.Reference, false);
            Primitive(root.transform, "BodyFrameBottom", PrimitiveType.Cube, new Vector3(0f, 0.04f, 0f), new Vector3(0.28f, 0.015f, 0.18f), MaterialKind.Reference, false);
            Primitive(root.transform, "BodyFrameLeft", PrimitiveType.Cube, new Vector3(-0.135f, 0.14f, 0f), new Vector3(0.015f, 0.20f, 0.18f), MaterialKind.Reference, false);
            Primitive(root.transform, "BodyFrameRight", PrimitiveType.Cube, new Vector3(0.135f, 0.14f, 0f), new Vector3(0.015f, 0.20f, 0.18f), MaterialKind.Reference, false);
            Primitive(root.transform, "HeadReference", PrimitiveType.Sphere, new Vector3(0f, 0.35f, 0f), Vector3.one * 0.12f, MaterialKind.Reference, false);
            var note = new GameObject("Use_as_reference_for_drift_and_registration_error");
            note.transform.SetParent(root.transform, false);
            return root;
        }

        private static GameObject BuildCalibrationFrame()
        {
            var root = new GameObject("L03_CalibrationFrame_AB");
            Primitive(root.transform, "Baseline_0_5m", PrimitiveType.Cube, new Vector3(0f, 0.02f, 0f), new Vector3(0.50f, 0.015f, 0.015f), MaterialKind.Structure, false);
            Primitive(root.transform, "Point_A", PrimitiveType.Sphere, new Vector3(-0.25f, 0.02f, 0f), Vector3.one * 0.055f, MaterialKind.Accent, false);
            Primitive(root.transform, "Point_B", PrimitiveType.Cube, new Vector3(0.25f, 0.02f, 0f), Vector3.one * 0.055f, MaterialKind.Secondary, false);
            return root;
        }

        private static GameObject BuildOcclusionBot()
        {
            var root = new GameObject("L04_OcclusionBot");
            BuildMiniRobot(root.transform, true, MaterialKind.Secondary);
            var note = new GameObject("Move_behind_real_objects_to_test_environment_occlusion");
            note.transform.SetParent(root.transform, false);
            return root;
        }

        private static GameObject BuildDepthProbe()
        {
            var root = new GameObject("L04_DepthProbe");
            Primitive(root.transform, "ProbeBody", PrimitiveType.Cylinder, new Vector3(0f, 0.16f, 0f), new Vector3(0.05f, 0.16f, 0.05f), MaterialKind.Accent, true);
            var beam = Primitive(root.transform, "RayDirection", PrimitiveType.Cylinder, new Vector3(0f, 0.22f, 0.22f), new Vector3(0.012f, 0.22f, 0.012f), MaterialKind.Reference, false);
            beam.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            Primitive(root.transform, "DepthTarget", PrimitiveType.Cube, new Vector3(0f, 0.22f, 0.48f), new Vector3(0.22f, 0.22f, 0.02f), MaterialKind.Warning, false);
            var note = new GameObject("Student_connects_depth_raycast_and_logs_distance_error");
            note.transform.SetParent(root.transform, false);
            return root;
        }

        private static GameObject BuildLightingReference()
        {
            var root = new GameObject("L04_LightingReference");
            Primitive(root.transform, "MatteReference", PrimitiveType.Sphere, new Vector3(-0.22f, 0.14f, 0f), Vector3.one * 0.24f, MaterialKind.Matte, false);
            Primitive(root.transform, "NeutralReference", PrimitiveType.Cube, new Vector3(0f, 0.12f, 0f), Vector3.one * 0.22f, MaterialKind.Reference, false);
            Primitive(root.transform, "MetalReference", PrimitiveType.Sphere, new Vector3(0.24f, 0.14f, 0f), Vector3.one * 0.24f, MaterialKind.Metal, false);
            return root;
        }

        private static GameObject BuildGripper(int detailLevel, bool faulty)
        {
            var root = new GameObject(faulty ? "L05_Gripper_Faulty" : detailLevel == 0 ? "L05_Gripper_Source" : detailLevel == 1 ? "L05_Gripper_LOD1Candidate" : "L05_Gripper_LOD2Candidate");

            Primitive(root.transform, "Palm", PrimitiveType.Cube, new Vector3(0f, 0f, 0f), new Vector3(0.48f, 0.16f, 0.28f), faulty ? MaterialKind.Error : MaterialKind.Structure, true);
            Primitive(root.transform, "JawLeft", PrimitiveType.Cube, new Vector3(-0.18f, 0.20f, 0f), new Vector3(0.10f, 0.38f, 0.16f), faulty ? MaterialKind.Warning : MaterialKind.Accent, true);
            Primitive(root.transform, "JawRight", PrimitiveType.Cube, new Vector3(0.18f, 0.20f, 0f), new Vector3(0.10f, 0.38f, 0.16f), faulty ? MaterialKind.Secondary : MaterialKind.Accent, true);
            Primitive(root.transform, "Motor", PrimitiveType.Cylinder, new Vector3(0f, -0.14f, 0f), new Vector3(0.12f, 0.12f, 0.12f), MaterialKind.Metal, true);

            if (detailLevel <= 1)
            {
                Primitive(root.transform, "RailLeft", PrimitiveType.Cylinder, new Vector3(-0.11f, 0.05f, 0f), new Vector3(0.025f, 0.20f, 0.025f), MaterialKind.Metal, false);
                Primitive(root.transform, "RailRight", PrimitiveType.Cylinder, new Vector3(0.11f, 0.05f, 0f), new Vector3(0.025f, 0.20f, 0.025f), MaterialKind.Metal, false);
            }

            if (detailLevel == 0)
            {
                for (var i = 0; i < 8; i++)
                {
                    var x = i < 4 ? -0.18f : 0.18f;
                    var z = -0.09f + (i % 4) * 0.06f;
                    Primitive(root.transform, $"Bolt_{i:00}", PrimitiveType.Cylinder, new Vector3(x, 0.08f, z), new Vector3(0.018f, 0.012f, 0.018f), faulty && i % 2 == 0 ? MaterialKind.Warning : MaterialKind.Metal, false);
                }
            }

            if (faulty)
            {
                root.transform.localScale = Vector3.one * 0.01f;
                var note = new GameObject("FAULTS_wrong_scale_many_materials_expensive_colliders");
                note.transform.SetParent(root.transform, false);

                foreach (var meshFilter in root.GetComponentsInChildren<MeshFilter>())
                {
                    var existingCollider = meshFilter.GetComponent<Collider>();
                    if (existingCollider != null)
                        UnityEngine.Object.DestroyImmediate(existingCollider);
                    var meshCollider = meshFilter.gameObject.AddComponent<MeshCollider>();
                    meshCollider.sharedMesh = meshFilter.sharedMesh;
                    meshCollider.convex = false;
                }
            }
            else
            {
                var note = new GameObject(detailLevel == 0 ? "Reference_high_detail" : $"Candidate_for_LOD{detailLevel}");
                note.transform.SetParent(root.transform, false);
            }

            return root;
        }

        private static GameObject BuildRobotArmVisual()
        {
            var root = new GameObject("L06_WiRRRobotArm_Visual");
            Primitive(root.transform, "base_link", PrimitiveType.Cylinder, new Vector3(0f, 0.10f, 0f), new Vector3(0.22f, 0.10f, 0.22f), MaterialKind.Dark, true);

            var joint1 = new GameObject("joint1");
            joint1.transform.SetParent(root.transform, false);
            joint1.transform.localPosition = new Vector3(0f, 0.20f, 0f);
            Primitive(joint1.transform, "link1", PrimitiveType.Cube, new Vector3(0f, 0.28f, 0f), new Vector3(0.12f, 0.56f, 0.12f), MaterialKind.Accent, true);

            var joint2 = new GameObject("joint2");
            joint2.transform.SetParent(joint1.transform, false);
            joint2.transform.localPosition = new Vector3(0f, 0.56f, 0f);
            Primitive(joint2.transform, "link2", PrimitiveType.Cube, new Vector3(0.28f, 0f, 0f), new Vector3(0.56f, 0.10f, 0.10f), MaterialKind.Secondary, true);

            var joint3 = new GameObject("joint3");
            joint3.transform.SetParent(joint2.transform, false);
            joint3.transform.localPosition = new Vector3(0.56f, 0f, 0f);
            Primitive(joint3.transform, "tool", PrimitiveType.Cylinder, new Vector3(0.12f, 0f, 0f), new Vector3(0.08f, 0.12f, 0.08f), MaterialKind.Warning, true);

            var note = new GameObject("Student_maps_joint1_joint2_joint3_to_RobotState");
            note.transform.SetParent(root.transform, false);
            return root;
        }

        private static GameObject BuildJointAxisVisualizer()
        {
            var root = new GameObject("L06_JointAxisVisualizer");
            BuildAxis(root.transform, "X_Axis", Vector3.right, new Color(0.85f, 0.20f, 0.20f), Quaternion.Euler(0f, 0f, -90f));
            BuildAxis(root.transform, "Y_Axis", Vector3.up, new Color(0.20f, 0.75f, 0.25f), Quaternion.identity);
            BuildAxis(root.transform, "Z_Axis", Vector3.forward, new Color(0.20f, 0.45f, 0.90f), Quaternion.Euler(90f, 0f, 0f));
            var note = new GameObject("Use_to_diagnose_axis_sign_and_offset");
            note.transform.SetParent(root.transform, false);
            return root;
        }

        private static GameObject BuildStateBeacon()
        {
            var root = new GameObject("L06_StateBeacon");
            Primitive(root.transform, "Pole", PrimitiveType.Cylinder, new Vector3(0f, 0.45f, 0f), new Vector3(0.035f, 0.45f, 0.035f), MaterialKind.Dark, true);
            Primitive(root.transform, "LIVE_Cylinder", PrimitiveType.Cylinder, new Vector3(0f, 0.95f, 0f), new Vector3(0.10f, 0.07f, 0.10f), MaterialKind.Accent, false);
            Primitive(root.transform, "STALE_Sphere", PrimitiveType.Sphere, new Vector3(0f, 1.18f, 0f), Vector3.one * 0.18f, MaterialKind.Warning, false);
            Primitive(root.transform, "DISCONNECTED_Cube", PrimitiveType.Cube, new Vector3(0f, 1.42f, 0f), Vector3.one * 0.16f, MaterialKind.Error, false);
            var note = new GameObject("Shapes_and_names_duplicate_colour_information");
            note.transform.SetParent(root.transform, false);
            return root;
        }

        private static GameObject BuildXRTestBench()
        {
            var root = new GameObject("L07_XRTestBench");
            Primitive(root.transform, "Bench", PrimitiveType.Cube, new Vector3(0f, 0.72f, 0f), new Vector3(1.3f, 0.08f, 0.65f), MaterialKind.Structure, true);
            Primitive(root.transform, "Target_A", PrimitiveType.Sphere, new Vector3(-0.38f, 0.88f, 0f), Vector3.one * 0.18f, MaterialKind.Accent, true);
            Primitive(root.transform, "Target_B", PrimitiveType.Cube, new Vector3(0f, 0.86f, 0f), Vector3.one * 0.16f, MaterialKind.Secondary, true);
            Primitive(root.transform, "Target_C", PrimitiveType.Cylinder, new Vector3(0.38f, 0.86f, 0f), new Vector3(0.10f, 0.16f, 0.10f), MaterialKind.Warning, true);
            var note = new GameObject("Use_for_smoke_performance_usability_and_regression_tests");
            note.transform.SetParent(root.transform, false);
            return root;
        }

        private static GameObject BuildFaultInjectionConsole()
        {
            var root = new GameObject("L07_FaultInjectionConsole");
            Primitive(root.transform, "Console", PrimitiveType.Cube, Vector3.zero, new Vector3(0.90f, 0.58f, 0.10f), MaterialKind.Dark, true);

            var faultNames = new[]
            {
                "WrongScale",
                "WrongInteractionLayer",
                "TrackingDegraded",
                "Latency",
                "PacketLoss",
                "StaleState",
                "WrongJointSign"
            };

            for (var i = 0; i < faultNames.Length; i++)
            {
                var row = i / 4;
                var col = i % 4;
                var x = -0.30f + col * 0.20f;
                var y = 0.15f - row * 0.25f;
                Primitive(root.transform, $"FAULT_{faultNames[i]}", i % 2 == 0 ? PrimitiveType.Cube : PrimitiveType.Cylinder, new Vector3(x, y, -0.08f), Vector3.one * 0.11f, i < 3 ? MaterialKind.Warning : MaterialKind.Error, true);
            }

            var note = new GameObject("Visual_console_only_student_or_test_harness_controls_faults");
            note.transform.SetParent(root.transform, false);
            return root;
        }

        private static void BuildMiniRobot(Transform parent, bool wheels, MaterialKind bodyMaterial)
        {
            Primitive(parent, "Body", PrimitiveType.Cube, new Vector3(0f, 0.18f, 0f), new Vector3(0.28f, 0.24f, 0.18f), bodyMaterial, true);
            Primitive(parent, "Head", PrimitiveType.Sphere, new Vector3(0f, 0.36f, 0f), Vector3.one * 0.15f, MaterialKind.Reference, true);
            Primitive(parent, "EyeLeft", PrimitiveType.Sphere, new Vector3(-0.04f, 0.38f, -0.065f), Vector3.one * 0.025f, MaterialKind.Dark, false);
            Primitive(parent, "EyeRight", PrimitiveType.Sphere, new Vector3(0.04f, 0.38f, -0.065f), Vector3.one * 0.025f, MaterialKind.Dark, false);

            if (wheels)
            {
                var left = Primitive(parent, "WheelLeft", PrimitiveType.Cylinder, new Vector3(-0.16f, 0.08f, 0f), new Vector3(0.07f, 0.035f, 0.07f), MaterialKind.Dark, true);
                var right = Primitive(parent, "WheelRight", PrimitiveType.Cylinder, new Vector3(0.16f, 0.08f, 0f), new Vector3(0.07f, 0.035f, 0.07f), MaterialKind.Dark, true);
                left.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
                right.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
            }
        }

        private static void BuildHumanReference(Transform parent, Vector3 position)
        {
            var root = new GameObject("HumanReference_1_75m");
            root.transform.SetParent(parent, false);
            root.transform.localPosition = position;
            Primitive(root.transform, "Torso", PrimitiveType.Capsule, new Vector3(0f, 1.05f, 0f), new Vector3(0.28f, 0.55f, 0.20f), MaterialKind.Reference, false);
            Primitive(root.transform, "Head", PrimitiveType.Sphere, new Vector3(0f, 1.62f, 0f), Vector3.one * 0.25f, MaterialKind.Reference, false);
            Primitive(root.transform, "LegLeft", PrimitiveType.Cube, new Vector3(-0.10f, 0.42f, 0f), new Vector3(0.12f, 0.84f, 0.12f), MaterialKind.Reference, false);
            Primitive(root.transform, "LegRight", PrimitiveType.Cube, new Vector3(0.10f, 0.42f, 0f), new Vector3(0.12f, 0.84f, 0.12f), MaterialKind.Reference, false);
        }

        private static void BuildAxis(Transform parent, string name, Vector3 direction, Color color, Quaternion rotation)
        {
            var material = EnsureMaterial($"Axis_{name}", color, 0f, 0.35f);
            var axis = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            axis.name = name;
            axis.transform.SetParent(parent, false);
            axis.transform.localPosition = direction * 0.18f;
            axis.transform.localScale = new Vector3(0.018f, 0.18f, 0.018f);
            axis.transform.localRotation = rotation;
            SetMaterial(axis, material);
            RemoveCollider(axis);

            var tip = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            tip.name = $"{name}_Positive";
            tip.transform.SetParent(parent, false);
            tip.transform.localPosition = direction * 0.38f;
            tip.transform.localScale = Vector3.one * 0.055f;
            SetMaterial(tip, material);
            RemoveCollider(tip);
        }

        private static GameObject Primitive(
            Transform parent,
            string name,
            PrimitiveType type,
            Vector3 localPosition,
            Vector3 localScale,
            MaterialKind materialKind,
            bool keepCollider)
        {
            var go = GameObject.CreatePrimitive(type);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPosition;
            go.transform.localScale = localScale;
            SetMaterial(go, GetMaterial(materialKind));
            if (!keepCollider)
                RemoveCollider(go);
            return go;
        }

        private static void SetMaterial(GameObject go, Material material)
        {
            var renderer = go.GetComponent<Renderer>();
            if (renderer != null)
                renderer.sharedMaterial = material;
        }

        private static void RemoveCollider(GameObject go)
        {
            var collider = go.GetComponent<Collider>();
            if (collider != null)
                UnityEngine.Object.DestroyImmediate(collider);
        }

        private static Material GetMaterial(MaterialKind kind)
        {
            switch (kind)
            {
                case MaterialKind.Structure:
                    return EnsureMaterial("WiRR_Structure", new Color(0.47f, 0.52f, 0.58f), 0.05f, 0.35f);
                case MaterialKind.Dark:
                    return EnsureMaterial("WiRR_Dark", new Color(0.10f, 0.13f, 0.17f), 0.10f, 0.40f);
                case MaterialKind.Accent:
                    return EnsureMaterial("WiRR_Accent", new Color(0.10f, 0.63f, 0.86f), 0.05f, 0.45f);
                case MaterialKind.Secondary:
                    return EnsureMaterial("WiRR_Secondary", new Color(0.50f, 0.40f, 0.85f), 0.05f, 0.45f);
                case MaterialKind.Warning:
                    return EnsureMaterial("WiRR_Warning", new Color(0.95f, 0.58f, 0.18f), 0.05f, 0.40f);
                case MaterialKind.Error:
                    return EnsureMaterial("WiRR_Error", new Color(0.88f, 0.25f, 0.24f), 0.05f, 0.40f);
                case MaterialKind.Reference:
                    return EnsureMaterial("WiRR_Reference", new Color(0.90f, 0.92f, 0.95f), 0.0f, 0.25f);
                case MaterialKind.Metal:
                    return EnsureMaterial("WiRR_Metal", new Color(0.50f, 0.54f, 0.60f), 0.85f, 0.65f);
                case MaterialKind.Matte:
                    return EnsureMaterial("WiRR_Matte", new Color(0.72f, 0.72f, 0.72f), 0.0f, 0.08f);
                default:
                    throw new ArgumentOutOfRangeException(nameof(kind));
            }
        }

        private static void EnsureMaterials()
        {
            foreach (MaterialKind kind in Enum.GetValues(typeof(MaterialKind)))
                GetMaterial(kind);
        }

        private static Material EnsureMaterial(string name, Color color, float metallic, float smoothness)
        {
            EnsureFolder($"{CommonRoot}/Materials/Generated");
            var path = $"{CommonRoot}/Materials/Generated/{name}.mat";
            var existing = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (existing != null)
                return existing;

            var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            if (shader == null)
                throw new InvalidOperationException("[WiRR] Nie znaleziono zgodnego shadera do wygenerowania materiałów dydaktycznych.");

            var material = new Material(shader) { name = name, color = color };
            if (material.HasProperty("_BaseColor"))
                material.SetColor("_BaseColor", color);
            if (material.HasProperty("_Metallic"))
                material.SetFloat("_Metallic", metallic);
            if (material.HasProperty("_Smoothness"))
                material.SetFloat("_Smoothness", smoothness);
            if (material.HasProperty("_Glossiness"))
                material.SetFloat("_Glossiness", smoothness);

            AssetDatabase.CreateAsset(material, path);
            return material;
        }

        private static GameObject EnsurePrefab(string path, Func<GameObject> builder)
        {
            EnsureFolder(Path.GetDirectoryName(path)?.Replace('\\', '/'));
            var temporary = builder();
            try
            {
                temporary.transform.position = Vector3.zero;
                temporary.transform.rotation = Quaternion.identity;
                PrefabUtility.SaveAsPrefabAsset(temporary, path, out var success);
                if (!success)
                    throw new InvalidOperationException($"[WiRR] Nie udało się zapisać prefabu: {path}");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(temporary);
            }

            return AssetDatabase.LoadAssetAtPath<GameObject>(path);
        }

        private static void PlacePrefabInScene(
            int labNumber,
            GameObject prefab,
            string category,
            Vector3 localPosition,
            Quaternion localRotation)
        {
            if (prefab == null)
                throw new InvalidOperationException("[WiRR] Nie udało się odczytać wygenerowanego prefabu.");

            var scene = SceneManager.GetActiveScene();
            if (!scene.IsValid())
                throw new InvalidOperationException("[WiRR] Brak aktywnej sceny.");

            var labRoot = FindInScene(scene, $"WiRR_Lab{labNumber:00}");
            if (labRoot == null)
                throw new InvalidOperationException("[WiRR] Brak głównego obiektu laboratorium. Najpierw przygotuj scenę bazową.");

            var teaching = EnsureChild(labRoot.transform, TeachingRootName);
            var categoryRoot = EnsureChild(teaching.transform, category);
            var existing = categoryRoot.transform.Find(prefab.name);
            if (existing != null)
                Undo.DestroyObjectImmediate(existing.gameObject);

            var instance = PrefabUtility.InstantiatePrefab(prefab, scene) as GameObject;
            if (instance == null)
                throw new InvalidOperationException($"[WiRR] Nie udało się umieścić prefabu {prefab.name} w scenie.");

            Undo.RegisterCreatedObjectUndo(instance, $"Add {prefab.name}");
            instance.transform.SetParent(categoryRoot.transform, false);
            instance.transform.localPosition = localPosition;
            instance.transform.localRotation = localRotation;
            EditorSceneManager.MarkSceneDirty(scene);
        }

        private static GameObject EnsureChild(Transform parent, string name)
        {
            var existing = parent.Find(name);
            if (existing != null)
                return existing.gameObject;

            var child = new GameObject(name);
            Undo.RegisterCreatedObjectUndo(child, $"Create {name}");
            child.transform.SetParent(parent, false);
            return child;
        }

        private static GameObject FindInScene(Scene scene, string name)
        {
            foreach (var root in scene.GetRootGameObjects())
            foreach (var transform in root.GetComponentsInChildren<Transform>(true))
                if (transform.name == name)
                    return transform.gameObject;
            return null;
        }

        private static string GeneratedPrefabFolder(int labNumber) =>
            $"{WiRRSceneTools.GetLabRootPath(labNumber)}/Prefabs/Generated";

        private static void EnsureGeneratedFolders(int labNumber)
        {
            WiRRSceneTools.EnsureFolders(labNumber);
            EnsureFolder(CommonRoot);
            EnsureFolder($"{CommonRoot}/Materials");
            EnsureFolder($"{CommonRoot}/Materials/Generated");
            EnsureFolder($"{CommonRoot}/Prefabs");
            EnsureFolder($"{CommonRoot}/Prefabs/Generated");
            EnsureFolder(GeneratedPrefabFolder(labNumber));
        }

        private static void EnsureFolder(string assetPath)
        {
            if (string.IsNullOrWhiteSpace(assetPath) || AssetDatabase.IsValidFolder(assetPath))
                return;

            var parent = Path.GetDirectoryName(assetPath)?.Replace('\\', '/');
            var name = Path.GetFileName(assetPath);
            if (string.IsNullOrWhiteSpace(parent) || string.IsNullOrWhiteSpace(name))
                return;

            if (!AssetDatabase.IsValidFolder(parent))
                EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, name);
        }

        private static void ValidateLabNumber(int labNumber)
        {
            if (labNumber < 1 || labNumber > 7)
                throw new ArgumentOutOfRangeException(nameof(labNumber), "Numer laboratorium musi należeć do zakresu 1–7.");
        }
    }
}
