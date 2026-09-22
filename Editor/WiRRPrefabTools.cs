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
    /// Generates a second, texture-aware family of teaching prefabs.
    /// These objects provide visual context and measurement targets, but deliberately
    /// avoid adding XRI, AR Foundation or ROS components that students are expected to configure.
    /// </summary>
    internal static class WiRRPrefabTools
    {
        private const string TeachingRootName = "WiRR_TeachingAssets";
        private const string GridCategoryName = "GridPrefabs";

        private sealed class PrefabDefinition
        {
            public string Name { get; }
            public Vector3 Position { get; }
            public Func<GameObject> Builder { get; }

            public PrefabDefinition(string name, Vector3 position, Func<GameObject> builder)
            {
                Name = name;
                Position = position;
                Builder = builder;
            }
        }

        [MenuItem("WiRR/Prefaby/Odśwież materiały grid", priority = 40)]
        private static void RefreshMaterialsFromMenu() =>
            WiRRPrefabTextureLibrary.RefreshAll();

        [MenuItem("WiRR/Prefaby/Wygeneruj zestaw grid dla wybranego laboratorium", priority = 41)]
        private static void GenerateFromMenu()
        {
            var lab = EditorPrefs.GetInt("KIA.WiRR.SelectedLab", 1);
            CreateOrRepairGridSet(lab);
        }

        [MenuItem("WiRR/Prefaby/Usuń zestaw grid z aktywnej sceny", priority = 42)]
        private static void RemoveFromMenu()
        {
            var lab = EditorPrefs.GetInt("KIA.WiRR.SelectedLab", 1);
            RemoveGridObjectsFromScene(lab);
        }

        public static void CreateOrRepairGridSet(int labNumber)
        {
            ValidateLabNumber(labNumber);
            WiRRSceneTools.PrepareBaseScene(labNumber);
            EnsureFolder(GridPrefabFolder(labNumber));

            var definitions = DefinitionsFor(labNumber);
            foreach (var definition in definitions)
            {
                var prefab = SavePrefab(
                    $"{GridPrefabFolder(labNumber)}/{definition.Name}.prefab",
                    definition.Builder);
                PlaceInScene(labNumber, prefab, definition.Position);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"[WiRR] Wygenerowano teksturowany zestaw prefabów Grid dla Lab {labNumber:00}.");
        }

        public static void RefreshGridMaterials() =>
            WiRRPrefabTextureLibrary.RefreshAll();

        public static void RemoveGridObjectsFromScene(int labNumber)
        {
            ValidateLabNumber(labNumber);
            var scene = SceneManager.GetActiveScene();
            if (!scene.IsValid())
                return;

            var labRoot = FindInScene(scene, $"WiRR_Lab{labNumber:00}");
            var teachingRoot = labRoot != null ? labRoot.transform.Find(TeachingRootName) : null;
            var gridRoot = teachingRoot != null ? teachingRoot.Find(GridCategoryName) : null;
            if (gridRoot == null)
                return;

            Undo.DestroyObjectImmediate(gridRoot.gameObject);
            EditorSceneManager.MarkSceneDirty(scene);
            Debug.Log($"[WiRR] Usunięto instancje prefabów Grid z aktywnej sceny Lab {labNumber:00}.");
        }

        public static bool GridPrefabsExist(int labNumber)
        {
            if (labNumber < 1 || labNumber > 7)
                return false;

            var folder = GridPrefabFolder(labNumber);
            return AssetDatabase.IsValidFolder(folder) &&
                   AssetDatabase.FindAssets("t:Prefab", new[] { folder }).Length > 0;
        }

        private static IEnumerable<PrefabDefinition> DefinitionsFor(int labNumber)
        {
            return labNumber switch
            {
                1 => new[]
                {
                    new PrefabDefinition("L01_MetricArena_Grid", new Vector3(-1.4f, 0f, 2.5f), BuildMetricArena),
                    new PrefabDefinition("L01_RenderGallery_Grid", new Vector3(1.5f, 0f, 2.7f), BuildRenderGallery)
                },
                2 => new[]
                {
                    new PrefabDefinition("L02_AssemblyWorkbench_Grid", new Vector3(-1.1f, 0f, 2.4f), BuildAssemblyWorkbench),
                    new PrefabDefinition("L02_ControlPanel_Grid", new Vector3(1.2f, 0.85f, 2.7f), BuildControlPanel)
                },
                3 => new[]
                {
                    new PrefabDefinition("L03_RegistrationBoard_Grid", new Vector3(-0.65f, 0f, 1.6f), BuildRegistrationBoard),
                    new PrefabDefinition("L03_AnchorTotem_Grid", new Vector3(0.75f, 0f, 1.6f), BuildAnchorTotem)
                },
                4 => new[]
                {
                    new PrefabDefinition("L04_OcclusionCorridor_Grid", new Vector3(-0.8f, 0f, 2.0f), BuildOcclusionCorridor),
                    new PrefabDefinition("L04_LightingBay_Grid", new Vector3(1.0f, 0f, 2.1f), BuildLightingBay)
                },
                5 => new[]
                {
                    new PrefabDefinition("L05_LODGallery_Grid", new Vector3(-0.8f, 0f, 2.2f), BuildLodGallery),
                    new PrefabDefinition("L05_ColliderLab_Grid", new Vector3(1.2f, 0f, 2.2f), BuildColliderLab)
                },
                6 => new[]
                {
                    new PrefabDefinition("L06_DigitalTwinCell_Grid", new Vector3(-0.9f, 0f, 2.4f), BuildDigitalTwinCell),
                    new PrefabDefinition("L06_NetworkConsole_Grid", new Vector3(1.3f, 0.85f, 2.5f), BuildNetworkConsole)
                },
                7 => new[]
                {
                    new PrefabDefinition("L07_QAArena_Grid", new Vector3(-0.9f, 0f, 2.4f), BuildQaArena),
                    new PrefabDefinition("L07_RiskMatrix_Grid", new Vector3(1.35f, 0.8f, 2.6f), BuildRiskMatrix)
                },
                _ => throw new ArgumentOutOfRangeException(nameof(labNumber))
            };
        }

        private static GameObject BuildMetricArena()
        {
            var root = NewRoot("L01_MetricArena_Grid");
            Box(root.transform, "Floor_2m", new Vector3(0f, 0.02f, 0f), new Vector3(2f, 0.04f, 2f), WiRRGridTextureSet.Grid1, WiRRGridMaterialRole.Floor, true);
            Box(root.transform, "Wall_X", new Vector3(0f, 0.5f, 1f), new Vector3(2f, 1f, 0.05f), WiRRGridTextureSet.Grid2, WiRRGridMaterialRole.Wall, true);
            Box(root.transform, "Wall_Z", new Vector3(-1f, 0.5f, 0f), new Vector3(0.05f, 1f, 2f), WiRRGridTextureSet.Grid3, WiRRGridMaterialRole.Wall, true);

            for (var i = 0; i < 4; i++)
            {
                var height = 0.25f * (i + 1);
                Box(
                    root.transform,
                    $"ScaleBlock_{height:0.00}m",
                    new Vector3(-0.6f + i * 0.4f, height / 2f, 0.25f),
                    new Vector3(0.22f, height, 0.22f),
                    (WiRRGridTextureSet)((i % 3) + 1),
                    WiRRGridMaterialRole.Panel,
                    true);
            }

            Marker(root.transform, "Origin_X", new Vector3(0.35f, 0.02f, -0.55f), Vector3.right, WiRRGridTextureSet.Grid1);
            Marker(root.transform, "Origin_Z", new Vector3(0.35f, 0.02f, -0.55f), Vector3.forward, WiRRGridTextureSet.Grid2);
            return root;
        }

        private static GameObject BuildRenderGallery()
        {
            var root = NewRoot("L01_RenderGallery_Grid");
            Box(root.transform, "Backplate", new Vector3(0f, 0.75f, 0f), new Vector3(1.8f, 1.5f, 0.08f), WiRRGridTextureSet.Grid1, WiRRGridMaterialRole.Wall, true);

            var sets = new[] { WiRRGridTextureSet.Grid1, WiRRGridTextureSet.Grid2, WiRRGridTextureSet.Grid3 };
            for (var row = 0; row < 2; row++)
            for (var col = 0; col < 3; col++)
            {
                var set = sets[col];
                var type = row == 0 ? PrimitiveType.Sphere : PrimitiveType.Cube;
                Primitive(
                    root.transform,
                    $"Sample_{set}_{type}",
                    type,
                    new Vector3(-0.55f + col * 0.55f, 0.45f + row * 0.62f, -0.14f),
                    Vector3.one * 0.34f,
                    set,
                    WiRRGridMaterialRole.Panel,
                    false);
            }

            Note(root.transform, "Compare_material_count_batches_and_visual_quality");
            return root;
        }

        private static GameObject BuildAssemblyWorkbench()
        {
            var root = NewRoot("L02_AssemblyWorkbench_Grid");
            Box(root.transform, "Workbench", new Vector3(0f, 0.72f, 0f), new Vector3(1.6f, 0.08f, 0.72f), WiRRGridTextureSet.Grid1, WiRRGridMaterialRole.Panel, true);
            foreach (var x in new[] { -0.68f, 0.68f })
            foreach (var z in new[] { -0.25f, 0.25f })
                Box(root.transform, "Leg", new Vector3(x, 0.35f, z), new Vector3(0.06f, 0.70f, 0.06f), WiRRGridTextureSet.Grid3, WiRRGridMaterialRole.Wall, true);

            for (var i = 0; i < 3; i++)
            {
                var set = (WiRRGridTextureSet)(i + 1);
                var x = -0.48f + i * 0.48f;
                Box(root.transform, $"Socket_{i + 1}", new Vector3(x, 0.79f, 0.12f), new Vector3(0.28f, 0.035f, 0.24f), set, WiRRGridMaterialRole.Accent, true);

                var module = Box(root.transform, $"Module_{i + 1}", new Vector3(x, 0.90f, -0.18f), new Vector3(0.16f, 0.20f, 0.12f), set, WiRRGridMaterialRole.Panel, true);
                var rb = module.AddComponent<Rigidbody>();
                rb.mass = 0.25f;
                rb.interpolation = RigidbodyInterpolation.Interpolate;
                Note(module.transform, "Student_adds_XRGrabInteractable");
            }

            Note(root.transform, "Student_configures_socket_filters_attach_points_and_haptics");
            return root;
        }

        private static GameObject BuildControlPanel()
        {
            var root = NewRoot("L02_ControlPanel_Grid");
            Box(root.transform, "Panel", Vector3.zero, new Vector3(1.1f, 0.65f, 0.08f), WiRRGridTextureSet.Grid2, WiRRGridMaterialRole.Panel, true);

            for (var i = 0; i < 3; i++)
            {
                var knob = Primitive(
                    root.transform,
                    $"RotaryKnob_{i + 1}",
                    PrimitiveType.Cylinder,
                    new Vector3(-0.32f + i * 0.32f, 0.12f, -0.10f),
                    new Vector3(0.10f, 0.05f, 0.10f),
                    (WiRRGridTextureSet)((i % 3) + 1),
                    WiRRGridMaterialRole.Accent,
                    true);
                knob.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
                Note(knob.transform, "Student_configures_rotation_constraint");
            }

            for (var i = 0; i < 3; i++)
                Primitive(
                    root.transform,
                    $"PushButton_{i + 1}",
                    PrimitiveType.Sphere,
                    new Vector3(-0.32f + i * 0.32f, -0.16f, -0.10f),
                    Vector3.one * 0.13f,
                    (WiRRGridTextureSet)(((i + 1) % 3) + 1),
                    WiRRGridMaterialRole.Accent,
                    true);

            return root;
        }

        private static GameObject BuildRegistrationBoard()
        {
            var root = NewRoot("L03_RegistrationBoard_Grid");
            Box(root.transform, "Board_0_8x0_5m", new Vector3(0f, 0.015f, 0f), new Vector3(0.8f, 0.03f, 0.5f), WiRRGridTextureSet.Grid1, WiRRGridMaterialRole.Panel, false);
            Fiducial(root.transform, "Point_A", new Vector3(-0.30f, 0.045f, 0f), WiRRGridTextureSet.Grid1, PrimitiveType.Sphere);
            Fiducial(root.transform, "Point_B", new Vector3(0.30f, 0.045f, 0f), WiRRGridTextureSet.Grid2, PrimitiveType.Cube);
            Fiducial(root.transform, "Point_C", new Vector3(0f, 0.045f, 0.18f), WiRRGridTextureSet.Grid3, PrimitiveType.Cylinder);
            Box(root.transform, "Baseline_AB_0_6m", new Vector3(0f, 0.035f, 0f), new Vector3(0.6f, 0.012f, 0.012f), WiRRGridTextureSet.Grid2, WiRRGridMaterialRole.Accent, false);
            Note(root.transform, "Measure_registration_error_in_millimetres");
            return root;
        }

        private static GameObject BuildAnchorTotem()
        {
            var root = NewRoot("L03_AnchorTotem_Grid");
            Primitive(root.transform, "Base", PrimitiveType.Cylinder, new Vector3(0f, 0.05f, 0f), new Vector3(0.22f, 0.05f, 0.22f), WiRRGridTextureSet.Grid1, WiRRGridMaterialRole.Floor, false);
            Box(root.transform, "Stem", new Vector3(0f, 0.30f, 0f), new Vector3(0.10f, 0.50f, 0.10f), WiRRGridTextureSet.Grid2, WiRRGridMaterialRole.Panel, false);
            Primitive(root.transform, "Target", PrimitiveType.Sphere, new Vector3(0f, 0.62f, 0f), Vector3.one * 0.22f, WiRRGridTextureSet.Grid3, WiRRGridMaterialRole.Accent, false);
            Note(root.transform, "AnchorPoint_student_adds_ARAnchor");
            return root;
        }

        private static GameObject BuildOcclusionCorridor()
        {
            var root = NewRoot("L04_OcclusionCorridor_Grid");
            Box(root.transform, "ReferenceFloor", new Vector3(0f, 0.015f, 0.35f), new Vector3(1.4f, 0.03f, 1.8f), WiRRGridTextureSet.Grid1, WiRRGridMaterialRole.Floor, false);

            for (var i = 0; i < 3; i++)
                Box(
                    root.transform,
                    $"Occluder_{i + 1}",
                    new Vector3(-0.45f + i * 0.45f, 0.45f, 0.15f + i * 0.32f),
                    new Vector3(0.22f, 0.90f, 0.08f),
                    (WiRRGridTextureSet)(i + 1),
                    WiRRGridMaterialRole.Wall,
                    false);

            Primitive(root.transform, "VirtualTarget", PrimitiveType.Sphere, new Vector3(0f, 0.30f, 1.05f), Vector3.one * 0.26f, WiRRGridTextureSet.Grid3, WiRRGridMaterialRole.Accent, false);
            Note(root.transform, "Use_real_geometry_or_depth_occlusion_to_hide_VirtualTarget");
            return root;
        }

        private static GameObject BuildLightingBay()
        {
            var root = NewRoot("L04_LightingBay_Grid");
            Box(root.transform, "Bay", new Vector3(0f, 0.02f, 0f), new Vector3(1.1f, 0.04f, 0.72f), WiRRGridTextureSet.Grid2, WiRRGridMaterialRole.Floor, false);
            Primitive(root.transform, "Grid1_Sphere", PrimitiveType.Sphere, new Vector3(-0.32f, 0.18f, 0f), Vector3.one * 0.28f, WiRRGridTextureSet.Grid1, WiRRGridMaterialRole.Panel, false);
            Primitive(root.transform, "Grid2_Cube", PrimitiveType.Cube, new Vector3(0f, 0.16f, 0f), Vector3.one * 0.26f, WiRRGridTextureSet.Grid2, WiRRGridMaterialRole.Panel, false);
            Primitive(root.transform, "Grid3_Cylinder", PrimitiveType.Cylinder, new Vector3(0.34f, 0.16f, 0f), new Vector3(0.14f, 0.16f, 0.14f), WiRRGridTextureSet.Grid3, WiRRGridMaterialRole.Panel, false);
            Note(root.transform, "Compare_lighting_estimation_across_three_material_families");
            return root;
        }

        private static GameObject BuildLodGallery()
        {
            var root = NewRoot("L05_LODGallery_Grid");
            BuildLodSpecimen(root.transform, "LOD0_Detailed", new Vector3(-0.55f, 0f, 0f), WiRRGridTextureSet.Grid1, 10);
            BuildLodSpecimen(root.transform, "LOD1_Medium", Vector3.zero, WiRRGridTextureSet.Grid2, 4);
            BuildLodSpecimen(root.transform, "LOD2_Low", new Vector3(0.55f, 0f, 0f), WiRRGridTextureSet.Grid3, 0);
            Note(root.transform, "Student_builds_LODGroup_and_measures_screen_relative_transition");
            return root;
        }

        private static GameObject BuildColliderLab()
        {
            var root = NewRoot("L05_ColliderLab_Grid");
            var box = Primitive(root.transform, "BoxColliderCandidate", PrimitiveType.Cube, new Vector3(-0.45f, 0.22f, 0f), new Vector3(0.32f, 0.44f, 0.32f), WiRRGridTextureSet.Grid1, WiRRGridMaterialRole.Panel, true);
            var compound = NewRoot("CompoundColliderCandidate");
            compound.transform.SetParent(root.transform, false);
            compound.transform.localPosition = Vector3.zero;
            Box(compound.transform, "PartA", new Vector3(0f, 0.16f, 0f), new Vector3(0.34f, 0.22f, 0.30f), WiRRGridTextureSet.Grid2, WiRRGridMaterialRole.Panel, true);
            Box(compound.transform, "PartB", new Vector3(0f, 0.42f, 0f), new Vector3(0.16f, 0.30f, 0.16f), WiRRGridTextureSet.Grid2, WiRRGridMaterialRole.Accent, true);
            var mesh = Primitive(root.transform, "MeshColliderCandidate", PrimitiveType.Sphere, new Vector3(0.45f, 0.24f, 0f), Vector3.one * 0.42f, WiRRGridTextureSet.Grid3, WiRRGridMaterialRole.Panel, false);
            mesh.AddComponent<MeshCollider>().sharedMesh = mesh.GetComponent<MeshFilter>().sharedMesh;
            Note(root.transform, "Compare_collider_cost_and_accuracy");
            _ = box;
            return root;
        }

        private static GameObject BuildDigitalTwinCell()
        {
            var root = NewRoot("L06_DigitalTwinCell_Grid");
            Box(root.transform, "CellFloor", new Vector3(0f, 0.02f, 0f), new Vector3(1.8f, 0.04f, 1.2f), WiRRGridTextureSet.Grid1, WiRRGridMaterialRole.Floor, true);
            Box(root.transform, "RobotPad", new Vector3(-0.42f, 0.06f, 0f), new Vector3(0.52f, 0.08f, 0.52f), WiRRGridTextureSet.Grid2, WiRRGridMaterialRole.Panel, true);
            Box(root.transform, "Conveyor", new Vector3(0.40f, 0.18f, 0f), new Vector3(0.82f, 0.18f, 0.40f), WiRRGridTextureSet.Grid3, WiRRGridMaterialRole.Panel, true);

            for (var i = 0; i < 3; i++)
                Primitive(
                    root.transform,
                    $"Payload_{i + 1}",
                    PrimitiveType.Cube,
                    new Vector3(0.14f + i * 0.25f, 0.34f, 0f),
                    Vector3.one * 0.16f,
                    (WiRRGridTextureSet)(i + 1),
                    WiRRGridMaterialRole.Accent,
                    true);

            Marker(root.transform, "RobotBase_X", new Vector3(-0.42f, 0.12f, 0f), Vector3.right, WiRRGridTextureSet.Grid1);
            Marker(root.transform, "RobotBase_Z", new Vector3(-0.42f, 0.12f, 0f), Vector3.forward, WiRRGridTextureSet.Grid2);
            Note(root.transform, "Place_or_map_WiRRRobotRig_on_RobotPad");
            return root;
        }

        private static GameObject BuildNetworkConsole()
        {
            var root = NewRoot("L06_NetworkConsole_Grid");
            Box(root.transform, "Console", Vector3.zero, new Vector3(1.0f, 0.62f, 0.08f), WiRRGridTextureSet.Grid3, WiRRGridMaterialRole.Panel, true);
            Primitive(root.transform, "LIVE", PrimitiveType.Sphere, new Vector3(-0.30f, 0.12f, -0.10f), Vector3.one * 0.16f, WiRRGridTextureSet.Grid1, WiRRGridMaterialRole.Accent, false);
            Primitive(root.transform, "STALE", PrimitiveType.Cylinder, new Vector3(0f, 0.12f, -0.10f), new Vector3(0.09f, 0.08f, 0.09f), WiRRGridTextureSet.Grid2, WiRRGridMaterialRole.Accent, false);
            Primitive(root.transform, "DISCONNECTED", PrimitiveType.Cube, new Vector3(0.30f, 0.12f, -0.10f), Vector3.one * 0.15f, WiRRGridTextureSet.Grid3, WiRRGridMaterialRole.Accent, false);

            for (var i = 0; i < 8; i++)
                Box(
                    root.transform,
                    $"LatencyBar_{i + 1}",
                    new Vector3(-0.35f + i * 0.10f, -0.16f, -0.10f),
                    new Vector3(0.05f, 0.08f + i * 0.025f, 0.05f),
                    (WiRRGridTextureSet)((i % 3) + 1),
                    WiRRGridMaterialRole.Accent,
                    false);

            Note(root.transform, "Student_binds_state_latency_jitter_or_packet_loss");
            return root;
        }

        private static GameObject BuildQaArena()
        {
            var root = NewRoot("L07_QAArena_Grid");
            Box(root.transform, "Floor", new Vector3(0f, 0.02f, 0f), new Vector3(1.8f, 0.04f, 1.6f), WiRRGridTextureSet.Grid1, WiRRGridMaterialRole.Floor, true);

            for (var i = 0; i < 4; i++)
            {
                var set = (WiRRGridTextureSet)((i % 3) + 1);
                Primitive(
                    root.transform,
                    $"AcceptanceTarget_{i + 1}",
                    i % 2 == 0 ? PrimitiveType.Sphere : PrimitiveType.Cube,
                    new Vector3(-0.60f + i * 0.40f, 0.20f + i * 0.10f, 0.35f),
                    Vector3.one * (0.16f + i * 0.03f),
                    set,
                    WiRRGridMaterialRole.Accent,
                    true);
            }

            Box(root.transform, "NarrowPassageLeft", new Vector3(-0.35f, 0.55f, -0.35f), new Vector3(0.10f, 1.1f, 0.50f), WiRRGridTextureSet.Grid2, WiRRGridMaterialRole.Wall, true);
            Box(root.transform, "NarrowPassageRight", new Vector3(0.35f, 0.55f, -0.35f), new Vector3(0.10f, 1.1f, 0.50f), WiRRGridTextureSet.Grid3, WiRRGridMaterialRole.Wall, true);
            Note(root.transform, "Define_acceptance_criteria_before_running_tests");
            return root;
        }

        private static GameObject BuildRiskMatrix()
        {
            var root = NewRoot("L07_RiskMatrix_Grid");
            Box(root.transform, "Board", Vector3.zero, new Vector3(0.95f, 0.95f, 0.06f), WiRRGridTextureSet.Grid1, WiRRGridMaterialRole.Panel, true);

            for (var y = 0; y < 5; y++)
            for (var x = 0; x < 5; x++)
            {
                var risk = (x + 1) * (y + 1);
                var set = risk <= 5
                    ? WiRRGridTextureSet.Grid1
                    : risk <= 12
                        ? WiRRGridTextureSet.Grid2
                        : WiRRGridTextureSet.Grid3;

                Box(
                    root.transform,
                    $"Risk_P{x + 1}_S{y + 1}_R{risk}",
                    new Vector3(-0.32f + x * 0.16f, -0.32f + y * 0.16f, -0.055f),
                    new Vector3(0.13f, 0.13f, 0.025f),
                    set,
                    WiRRGridMaterialRole.Accent,
                    false);
            }

            Note(root.transform, "R_equals_probability_times_severity");
            return root;
        }

        private static void BuildLodSpecimen(
            Transform parent,
            string name,
            Vector3 position,
            WiRRGridTextureSet set,
            int detailParts)
        {
            var specimen = NewRoot(name);
            specimen.transform.SetParent(parent, false);
            specimen.transform.localPosition = position;

            Box(specimen.transform, "Body", new Vector3(0f, 0.22f, 0f), new Vector3(0.34f, 0.34f, 0.28f), set, WiRRGridMaterialRole.Panel, true);
            Box(specimen.transform, "Neck", new Vector3(0f, 0.46f, 0f), new Vector3(0.14f, 0.18f, 0.14f), set, WiRRGridMaterialRole.Accent, true);

            for (var i = 0; i < detailParts; i++)
            {
                var angle = i * Mathf.PI * 2f / Mathf.Max(1, detailParts);
                Primitive(
                    specimen.transform,
                    $"Detail_{i + 1:00}",
                    PrimitiveType.Sphere,
                    new Vector3(Mathf.Cos(angle) * 0.18f, 0.24f, Mathf.Sin(angle) * 0.14f),
                    Vector3.one * 0.055f,
                    set,
                    WiRRGridMaterialRole.Accent,
                    false);
            }
        }

        private static void Fiducial(
            Transform parent,
            string name,
            Vector3 position,
            WiRRGridTextureSet set,
            PrimitiveType primitiveType)
        {
            Primitive(parent, name, primitiveType, position, Vector3.one * 0.10f, set, WiRRGridMaterialRole.Accent, false);
        }

        private static void Marker(
            Transform parent,
            string name,
            Vector3 origin,
            Vector3 direction,
            WiRRGridTextureSet set)
        {
            var marker = Primitive(
                parent,
                name,
                PrimitiveType.Cylinder,
                origin + direction * 0.18f,
                new Vector3(0.018f, 0.18f, 0.018f),
                set,
                WiRRGridMaterialRole.Accent,
                false);

            marker.transform.localRotation = direction == Vector3.right
                ? Quaternion.Euler(0f, 0f, -90f)
                : Quaternion.Euler(90f, 0f, 0f);
        }

        private static GameObject NewRoot(string name) => new(name);

        private static GameObject Box(
            Transform parent,
            string name,
            Vector3 localPosition,
            Vector3 localScale,
            WiRRGridTextureSet set,
            WiRRGridMaterialRole role,
            bool keepCollider)
        {
            return Primitive(parent, name, PrimitiveType.Cube, localPosition, localScale, set, role, keepCollider);
        }

        private static GameObject Primitive(
            Transform parent,
            string name,
            PrimitiveType type,
            Vector3 localPosition,
            Vector3 localScale,
            WiRRGridTextureSet set,
            WiRRGridMaterialRole role,
            bool keepCollider)
        {
            var go = GameObject.CreatePrimitive(type);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPosition;
            go.transform.localScale = localScale;

            var renderer = go.GetComponent<Renderer>();
            if (renderer != null)
                renderer.sharedMaterial = WiRRPrefabTextureLibrary.GetMaterial(set, role);

            if (!keepCollider)
            {
                var collider = go.GetComponent<Collider>();
                if (collider != null)
                    UnityEngine.Object.DestroyImmediate(collider);
            }

            return go;
        }

        private static void Note(Transform parent, string name)
        {
            var note = new GameObject(name);
            note.transform.SetParent(parent, false);
        }

        private static GameObject SavePrefab(string path, Func<GameObject> builder)
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

        private static void PlaceInScene(int labNumber, GameObject prefab, Vector3 localPosition)
        {
            if (prefab == null)
                throw new InvalidOperationException("[WiRR] Wygenerowany prefab nie jest dostępny.");

            var scene = SceneManager.GetActiveScene();
            var labRoot = FindInScene(scene, $"WiRR_Lab{labNumber:00}");
            if (labRoot == null)
                throw new InvalidOperationException("[WiRR] Brak głównego obiektu laboratorium.");

            var teachingRoot = EnsureChild(labRoot.transform, TeachingRootName);
            var gridRoot = EnsureChild(teachingRoot.transform, GridCategoryName);
            var existing = gridRoot.transform.Find(prefab.name);
            if (existing != null)
                Undo.DestroyObjectImmediate(existing.gameObject);

            var instance = PrefabUtility.InstantiatePrefab(prefab, scene) as GameObject;
            if (instance == null)
                throw new InvalidOperationException($"[WiRR] Nie udało się umieścić {prefab.name} w scenie.");

            Undo.RegisterCreatedObjectUndo(instance, $"Add {prefab.name}");
            instance.transform.SetParent(gridRoot.transform, false);
            instance.transform.localPosition = localPosition;
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
            if (!scene.IsValid())
                return null;

            foreach (var root in scene.GetRootGameObjects())
            foreach (var transform in root.GetComponentsInChildren<Transform>(true))
                if (transform.name == name)
                    return transform.gameObject;

            return null;
        }

        private static string GridPrefabFolder(int labNumber) =>
            $"{WiRRSceneTools.GetLabRootPath(labNumber)}/Prefabs/GridGenerated";

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
