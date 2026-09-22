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
    /// Generates optional dynamic teaching prefabs: moving platforms, a controllable
    /// rigidbody vehicle, a physics impulse launcher and an audio-kinetic beacon.
    /// The prefabs run without XRI/AR/ROS and expose clear placeholders for student work.
    /// </summary>
    internal static class WiRRDynamicPrefabTools
    {
        private const string TeachingRootName = "WiRR_TeachingAssets";
        private const string DynamicRootName = "DynamicDemos";
        private const string PrefabRoot = "Assets/WiRR/Common/Prefabs/Dynamic";

        private sealed class DynamicDefinition
        {
            public string Name { get; }
            public Vector3 Position { get; }
            public Func<GameObject> Builder { get; }

            public DynamicDefinition(string name, Vector3 position, Func<GameObject> builder)
            {
                Name = name;
                Position = position;
                Builder = builder;
            }
        }

        public static void CreateAll(int labNumber)
        {
            foreach (var definition in Definitions())
                CreateOne(labNumber, definition);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void CreateFloatingTeleportPlatform(int labNumber) =>
            CreateOne(labNumber, Find("DYN_FloatingTeleportPlatform"));

        public static void CreateShuttlePlatform(int labNumber) =>
            CreateOne(labNumber, Find("DYN_ShuttlePlatform"));

        public static void CreateDriveableCart(int labNumber) =>
            CreateOne(labNumber, Find("DYN_DriveableCart"));

        public static void CreatePhysicsLauncher(int labNumber) =>
            CreateOne(labNumber, Find("DYN_PhysicsImpulseLauncher"));

        public static void CreateKineticAudioBeacon(int labNumber) =>
            CreateOne(labNumber, Find("DYN_KineticAudioBeacon"));

        public static bool DynamicDemosExist(int labNumber)
        {
            if (labNumber < 1 || labNumber > 7)
                return false;

            var scene = SceneManager.GetActiveScene();
            if (!scene.IsValid())
                return false;

            var labRoot = FindInScene(scene, $"WiRR_Lab{labNumber:00}");
            var dynamicRoot = labRoot?.transform.Find($"{TeachingRootName}/{DynamicRootName}");
            return dynamicRoot != null && dynamicRoot.childCount > 0;
        }

        public static void RemoveDynamicDemos(int labNumber)
        {
            ValidateLabNumber(labNumber);
            var scene = SceneManager.GetActiveScene();
            if (!scene.IsValid())
                return;

            var labRoot = FindInScene(scene, $"WiRR_Lab{labNumber:00}");
            var dynamicRoot = labRoot?.transform.Find($"{TeachingRootName}/{DynamicRootName}");
            if (dynamicRoot == null)
                return;

            Undo.DestroyObjectImmediate(dynamicRoot.gameObject);
            EditorSceneManager.MarkSceneDirty(scene);
            Debug.Log($"[WiRR] Usunięto dynamiczne demonstratory z Lab {labNumber:00}.");
        }

        private static IEnumerable<DynamicDefinition> Definitions()
        {
            return new[]
            {
                new DynamicDefinition(
                    "DYN_FloatingTeleportPlatform",
                    new Vector3(-2.1f, 0.25f, -1.6f),
                    BuildFloatingTeleportPlatform),
                new DynamicDefinition(
                    "DYN_ShuttlePlatform",
                    new Vector3(0f, 0.35f, -2.1f),
                    BuildShuttlePlatform),
                new DynamicDefinition(
                    "DYN_DriveableCart",
                    new Vector3(2.0f, 0.28f, -1.5f),
                    BuildDriveableCart),
                new DynamicDefinition(
                    "DYN_PhysicsImpulseLauncher",
                    new Vector3(-1.9f, 0f, 1.3f),
                    BuildPhysicsLauncher),
                new DynamicDefinition(
                    "DYN_KineticAudioBeacon",
                    new Vector3(2.0f, 0f, 1.25f),
                    BuildKineticAudioBeacon)
            };
        }

        private static DynamicDefinition Find(string name)
        {
            foreach (var definition in Definitions())
                if (definition.Name == name)
                    return definition;

            throw new ArgumentException($"Nieznany demonstrator: {name}", nameof(name));
        }

        private static GameObject BuildFloatingTeleportPlatform()
        {
            var root = new GameObject("DYN_FloatingTeleportPlatform");

            var body = root.AddComponent<Rigidbody>();
            body.isKinematic = true;
            body.useGravity = false;
            body.interpolation = RigidbodyInterpolation.Interpolate;

            var motion = root.AddComponent<WiRRLoopMotion>();
            motion.Configure(
                WiRRLoopMotionMode.Bob,
                new Vector3(0f, 0.65f, 0f),
                4.5f,
                0.1f,
                true);

            var audio = root.AddComponent<WiRRProceduralAudio>();
            audio.Configure(WiRRProceduralAudioProfile.Hum, 72f, 0.07f, 1f);

            var deckMaterial = WiRRSurfaceTextureLibrary.GetMaterial(WiRRSurfaceFamily.DiamondPlate);
            var frameMaterial = WiRRSurfaceTextureLibrary.GetMaterial(WiRRSurfaceFamily.Metal004);
            var padMaterial = WiRRPrefabTextureLibrary.GetMaterial(
                WiRRGridTextureSet.Grid2,
                WiRRGridMaterialRole.Accent);

            Box(
                root.transform,
                "PlatformDeck",
                new Vector3(0f, 0f, 0f),
                new Vector3(1.8f, 0.16f, 1.8f),
                deckMaterial,
                true);

            foreach (var x in new[] { -0.83f, 0.83f })
            foreach (var z in new[] { -0.83f, 0.83f })
                Box(
                    root.transform,
                    "CornerThruster",
                    new Vector3(x, -0.16f, z),
                    new Vector3(0.15f, 0.22f, 0.15f),
                    frameMaterial,
                    false);

            var teleportArea = Box(
                root.transform,
                "TeleportAreaPlaceholder",
                new Vector3(0f, 0.095f, 0f),
                new Vector3(1.45f, 0.025f, 1.45f),
                padMaterial,
                true);

            Note(
                teleportArea.transform,
                "Student_adds_XRI_TeleportationArea_here");
            Note(
                root.transform,
                "Observe_motion_sickness_risk_when_destination_moves_vertically");

            return root;
        }

        private static GameObject BuildShuttlePlatform()
        {
            var root = new GameObject("DYN_ShuttlePlatform");

            var body = root.AddComponent<Rigidbody>();
            body.isKinematic = true;
            body.useGravity = false;
            body.interpolation = RigidbodyInterpolation.Interpolate;

            var motion = root.AddComponent<WiRRLoopMotion>();
            motion.Configure(
                WiRRLoopMotionMode.PingPong,
                new Vector3(3.2f, 0f, 0f),
                7f,
                0f,
                true);

            var audio = root.AddComponent<WiRRProceduralAudio>();
            audio.Configure(WiRRProceduralAudioProfile.Wind, 55f, 0.055f, 1f);

            var floorMaterial = WiRRSurfaceTextureLibrary.GetMaterial(WiRRSurfaceFamily.WoodFloor040);
            var railMaterial = WiRRSurfaceTextureLibrary.GetMaterial(WiRRSurfaceFamily.Metal044A);
            var padMaterial = WiRRPrefabTextureLibrary.GetMaterial(
                WiRRGridTextureSet.Grid1,
                WiRRGridMaterialRole.Accent);

            Box(
                root.transform,
                "ShuttleDeck",
                Vector3.zero,
                new Vector3(2.2f, 0.14f, 1.1f),
                floorMaterial,
                true);

            foreach (var z in new[] { -0.51f, 0.51f })
                Box(
                    root.transform,
                    "SafetyRail",
                    new Vector3(0f, 0.38f, z),
                    new Vector3(2.1f, 0.07f, 0.07f),
                    railMaterial,
                    true);

            foreach (var x in new[] { -1.0f, 1.0f })
            foreach (var z in new[] { -0.51f, 0.51f })
                Box(
                    root.transform,
                    "RailPost",
                    new Vector3(x, 0.20f, z),
                    new Vector3(0.07f, 0.40f, 0.07f),
                    railMaterial,
                    true);

            var teleportArea = Box(
                root.transform,
                "TeleportAreaPlaceholder",
                new Vector3(0f, 0.085f, 0f),
                new Vector3(1.75f, 0.02f, 0.78f),
                padMaterial,
                true);

            Note(teleportArea.transform, "Student_adds_XRI_TeleportationArea_here");
            Note(root.transform, "Compare_static_and_moving_teleport_destinations");
            return root;
        }

        private static GameObject BuildDriveableCart()
        {
            var root = new GameObject("DYN_DriveableCart");

            var body = root.AddComponent<Rigidbody>();
            body.mass = 85f;
            body.useGravity = true;
            body.interpolation = RigidbodyInterpolation.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            body.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            var collider = root.AddComponent<BoxCollider>();
            collider.center = new Vector3(0f, 0.28f, 0f);
            collider.size = new Vector3(1.25f, 0.42f, 1.75f);

            var audio = root.AddComponent<WiRRProceduralAudio>();
            audio.Configure(WiRRProceduralAudioProfile.Engine, 68f, 0.12f, 1f);

            var controller = root.AddComponent<WiRRVehicleController>();
            controller.Configure(8.5f, 7f, 95f);

            var chassisMaterial = WiRRSurfaceTextureLibrary.GetMaterial(WiRRSurfaceFamily.PaintedWood);
            var metalMaterial = WiRRSurfaceTextureLibrary.GetMaterial(WiRRSurfaceFamily.DiamondPlate);
            var tyreMaterial = WiRRSurfaceTextureLibrary.GetMaterial(WiRRSurfaceFamily.Metal044A);
            var seatMaterial = WiRRSurfaceTextureLibrary.GetMaterial(WiRRSurfaceFamily.Fabric066);

            Box(
                root.transform,
                "Chassis",
                new Vector3(0f, 0.30f, 0f),
                new Vector3(1.20f, 0.32f, 1.68f),
                chassisMaterial,
                false);

            Box(
                root.transform,
                "FloorPlate",
                new Vector3(0f, 0.51f, 0.08f),
                new Vector3(0.98f, 0.05f, 1.10f),
                metalMaterial,
                false);

            Box(
                root.transform,
                "Seat",
                new Vector3(0f, 0.69f, 0.28f),
                new Vector3(0.72f, 0.28f, 0.52f),
                seatMaterial,
                false);

            var wheelPositions = new[]
            {
                new Vector3(-0.68f, 0.20f, 0.58f),
                new Vector3(0.68f, 0.20f, 0.58f),
                new Vector3(-0.68f, 0.20f, -0.58f),
                new Vector3(0.68f, 0.20f, -0.58f)
            };

            for (var i = 0; i < wheelPositions.Length; i++)
            {
                var wheel = Primitive(
                    root.transform,
                    $"Wheel_{i + 1}",
                    PrimitiveType.Cylinder,
                    wheelPositions[i],
                    new Vector3(0.30f, 0.12f, 0.30f),
                    tyreMaterial,
                    false);
                wheel.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
            }

            var steeringWheel = Primitive(
                root.transform,
                "SteeringWheelVisual",
                PrimitiveType.Cylinder,
                new Vector3(0f, 0.92f, -0.35f),
                new Vector3(0.19f, 0.035f, 0.19f),
                metalMaterial,
                false);
            steeringWheel.transform.localRotation = Quaternion.Euler(65f, 0f, 0f);

            Note(root.transform, "Controls_WASD_or_arrows_space_brake_gamepad_supported");
            Note(root.transform, "XR_option_call_WiRRVehicleController_SetExternalInput");
            Note(root.transform, "Keep_vehicle_on_WiRR_Ground_or_build_a_track");

            return root;
        }

        private static GameObject BuildPhysicsLauncher()
        {
            var root = new GameObject("DYN_PhysicsImpulseLauncher");

            var baseMaterial = WiRRSurfaceTextureLibrary.GetMaterial(WiRRSurfaceFamily.Concrete);
            var rampMaterial = WiRRSurfaceTextureLibrary.GetMaterial(WiRRSurfaceFamily.DiamondPlate);
            var ballMaterials = new[]
            {
                WiRRSurfaceTextureLibrary.GetMaterial(WiRRSurfaceFamily.Metal004),
                WiRRSurfaceTextureLibrary.GetMaterial(WiRRSurfaceFamily.PaintedWood),
                WiRRSurfaceTextureLibrary.GetMaterial(WiRRSurfaceFamily.Fabric023)
            };

            Box(
                root.transform,
                "Base",
                new Vector3(0f, 0.08f, 0f),
                new Vector3(1.6f, 0.16f, 1.8f),
                baseMaterial,
                true);

            var trigger = new GameObject("ImpulseField");
            trigger.transform.SetParent(root.transform, false);
            trigger.transform.localPosition = new Vector3(0f, 0.36f, 0.15f);
            var triggerCollider = trigger.AddComponent<BoxCollider>();
            triggerCollider.isTrigger = true;
            triggerCollider.size = new Vector3(1.15f, 0.52f, 0.72f);

            var impulse = trigger.AddComponent<WiRRPhysicsImpulsePad>();
            impulse.Configure(new Vector3(0f, 1f, 0.42f), 5.8f, 0.45f);

            var triggerVisual = Box(
                trigger.transform,
                "FieldVisual",
                Vector3.zero,
                new Vector3(1.12f, 0.04f, 0.70f),
                rampMaterial,
                false);

            Note(triggerVisual.transform, "Trigger_volume_applies_impulse_to_dynamic_Rigidbody");

            for (var i = 0; i < 6; i++)
            {
                var x = -0.42f + (i % 3) * 0.42f;
                var z = -0.54f - (i / 3) * 0.28f;
                var ball = Primitive(
                    root.transform,
                    $"PhysicsBall_{i + 1}",
                    i % 2 == 0 ? PrimitiveType.Sphere : PrimitiveType.Cube,
                    new Vector3(x, 0.34f, z),
                    Vector3.one * 0.22f,
                    ballMaterials[i % ballMaterials.Length],
                    true);

                var body = ball.AddComponent<Rigidbody>();
                body.mass = 0.22f + i * 0.08f;
                body.interpolation = RigidbodyInterpolation.Interpolate;
            }

            Note(root.transform, "Compare_mass_trajectory_collision_and_force_mode");
            return root;
        }

        private static GameObject BuildKineticAudioBeacon()
        {
            var root = new GameObject("DYN_KineticAudioBeacon");

            var concrete = WiRRSurfaceTextureLibrary.GetMaterial(WiRRSurfaceFamily.Concrete);
            var metal = WiRRSurfaceTextureLibrary.GetMaterial(WiRRSurfaceFamily.Metal004);
            var accent = WiRRPrefabTextureLibrary.GetMaterial(
                WiRRGridTextureSet.Grid3,
                WiRRGridMaterialRole.Accent);

            Box(
                root.transform,
                "Pedestal",
                new Vector3(0f, 0.12f, 0f),
                new Vector3(0.82f, 0.24f, 0.82f),
                concrete,
                true);

            var rotor = new GameObject("Rotor");
            rotor.transform.SetParent(root.transform, false);
            rotor.transform.localPosition = new Vector3(0f, 0.82f, 0f);

            var motion = rotor.AddComponent<WiRRLoopMotion>();
            motion.Configure(
                WiRRLoopMotionMode.Rotate,
                Vector3.up,
                5.5f,
                0f,
                true);

            var audio = rotor.AddComponent<WiRRProceduralAudio>();
            audio.Configure(WiRRProceduralAudioProfile.Beacon, 150f, 0.10f, 1f);

            for (var i = 0; i < 4; i++)
            {
                var angle = i * Mathf.PI * 0.5f;
                var direction = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));

                Box(
                    rotor.transform,
                    $"Arm_{i + 1}",
                    direction * 0.42f,
                    new Vector3(
                        Mathf.Abs(direction.x) > 0.5f ? 0.78f : 0.06f,
                        0.06f,
                        Mathf.Abs(direction.z) > 0.5f ? 0.78f : 0.06f),
                    metal,
                    false);

                Primitive(
                    rotor.transform,
                    $"Emitter_{i + 1}",
                    PrimitiveType.Sphere,
                    direction * 0.78f,
                    Vector3.one * 0.18f,
                    accent,
                    false);
            }

            var orbitingProbe = new GameObject("OrbitingProbe");
            orbitingProbe.transform.SetParent(root.transform, false);
            orbitingProbe.transform.localPosition = new Vector3(0f, 1.12f, 0f);
            var orbit = orbitingProbe.AddComponent<WiRRLoopMotion>();
            orbit.Configure(
                WiRRLoopMotionMode.Orbit,
                new Vector3(0.62f, 0.10f, 0.62f),
                3.6f,
                0.25f,
                true);
            Primitive(
                orbitingProbe.transform,
                "Probe",
                PrimitiveType.Sphere,
                Vector3.zero,
                Vector3.one * 0.15f,
                accent,
                false);

            Note(root.transform, "Observe_spatial_audio_while_sources_and_visuals_move");
            Note(root.transform, "Compare_rotation_orbit_and_period_phase");

            return root;
        }

        private static GameObject Primitive(
            Transform parent,
            string name,
            PrimitiveType type,
            Vector3 localPosition,
            Vector3 localScale,
            Material material,
            bool keepCollider)
        {
            var go = GameObject.CreatePrimitive(type);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPosition;
            go.transform.localScale = localScale;

            var renderer = go.GetComponent<Renderer>();
            if (renderer != null)
                renderer.sharedMaterial = material;

            if (!keepCollider)
            {
                var collider = go.GetComponent<Collider>();
                if (collider != null)
                    UnityEngine.Object.DestroyImmediate(collider);
            }

            return go;
        }

        private static GameObject Box(
            Transform parent,
            string name,
            Vector3 localPosition,
            Vector3 localScale,
            Material material,
            bool keepCollider)
        {
            return Primitive(
                parent,
                name,
                PrimitiveType.Cube,
                localPosition,
                localScale,
                material,
                keepCollider);
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

        private static void CreateOne(int labNumber, DynamicDefinition definition)
        {
            ValidateLabNumber(labNumber);
            WiRRSceneTools.PrepareBaseScene(labNumber);
            EnsureFolder(PrefabRoot);

            var prefab = SavePrefab(
                $"{PrefabRoot}/{definition.Name}.prefab",
                definition.Builder);
            PlaceInScene(labNumber, prefab, definition.Position);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"[WiRR] Dodano dynamiczny demonstrator {definition.Name}.");
        }

        private static void PlaceInScene(int labNumber, GameObject prefab, Vector3 localPosition)
        {
            if (prefab == null)
                throw new InvalidOperationException("[WiRR] Wygenerowany prefab dynamiczny nie jest dostępny.");

            var scene = SceneManager.GetActiveScene();
            if (!scene.IsValid())
                throw new InvalidOperationException("[WiRR] Brak aktywnej sceny.");

            var labRoot = FindInScene(scene, $"WiRR_Lab{labNumber:00}");
            if (labRoot == null)
                throw new InvalidOperationException("[WiRR] Brak głównego obiektu laboratorium.");

            var teaching = EnsureChild(labRoot.transform, TeachingRootName);
            var dynamicRoot = EnsureChild(teaching.transform, DynamicRootName);

            var existing = dynamicRoot.transform.Find(prefab.name);
            if (existing != null)
                Undo.DestroyObjectImmediate(existing.gameObject);

            var instance = PrefabUtility.InstantiatePrefab(prefab, scene) as GameObject;
            if (instance == null)
                throw new InvalidOperationException($"[WiRR] Nie udało się umieścić {prefab.name} w scenie.");

            Undo.RegisterCreatedObjectUndo(instance, $"Add {prefab.name}");
            instance.transform.SetParent(dynamicRoot.transform, false);
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
            foreach (var root in scene.GetRootGameObjects())
            foreach (var transform in root.GetComponentsInChildren<Transform>(true))
                if (transform.name == name)
                    return transform.gameObject;
            return null;
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
                throw new ArgumentOutOfRangeException(
                    nameof(labNumber),
                    "Numer laboratorium musi należeć do zakresu 1–7.");
        }
    }
}
