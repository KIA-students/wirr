using UnityEngine;

namespace KIA.WiRR
{
    public enum WiRRVitureDemoPreset
    {
        StereoDepthLayers = 0,
        StereoComfort = 1,
        Immersive2DSource = 2
    }

    /// <summary>
    /// Samowystarczalny demonstrator VITURE XR. Buduje lekką scenę bez zewnętrznych
    /// assetów i bez zależności od XRI. Presety 0-1 używają Full SBS, preset 2
    /// pozostaje zwykłym obrazem 2D do porównania z VITURE Immersive 3D.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class WiRRVitureDemoBootstrap : MonoBehaviour
    {
        [SerializeField] private WiRRVitureDemoPreset preset = WiRRVitureDemoPreset.StereoDepthLayers;
        [SerializeField] private float ipdMeters = 0.064f;
        [SerializeField] private bool rebuildOnStart = true;

        private Transform generatedRoot;

        public WiRRVitureDemoPreset Preset => preset;

        public void Configure(WiRRVitureDemoPreset demoPreset, float ipd = 0.064f)
        {
            preset = demoPreset;
            ipdMeters = Mathf.Clamp(ipd, 0.050f, 0.075f);
        }

        private void Start()
        {
            if (rebuildOnStart)
                Build();
        }

        [ContextMenu("Zbuduj demonstrator")]
        public void Build()
        {
            ClearGenerated();

            var root = new GameObject("VITURE_Demo_Generated");
            root.transform.SetParent(transform, false);
            generatedRoot = root.transform;

            CreateLight(root.transform);
            CreateGround(root.transform);

            switch (preset)
            {
                case WiRRVitureDemoPreset.StereoComfort:
                    BuildStereoComfort(root.transform);
                    CreateStereoRig(root.transform);
                    break;
                case WiRRVitureDemoPreset.Immersive2DSource:
                    BuildImmersive2D(root.transform);
                    CreateMonoCamera(root.transform);
                    break;
                default:
                    BuildDepthLayers(root.transform);
                    CreateStereoRig(root.transform);
                    break;
            }
        }

        private void BuildDepthLayers(Transform parent)
        {
            CreateBox(parent, "Near_Red_1.2m", new Vector3(-0.55f, 1.25f, 1.2f), new Vector3(0.42f, 0.42f, 0.42f), new Color(0.90f, 0.18f, 0.16f));
            CreateBox(parent, "Near_Red_1.2m_B", new Vector3(0.55f, 0.85f, 1.2f), new Vector3(0.30f, 0.30f, 0.30f), new Color(0.95f, 0.30f, 0.18f));

            CreateBox(parent, "Mid_Green_2.5m", new Vector3(0.65f, 1.30f, 2.5f), new Vector3(0.70f, 0.70f, 0.70f), new Color(0.15f, 0.72f, 0.34f));
            CreateBox(parent, "Mid_Green_2.5m_B", new Vector3(-0.75f, 0.70f, 2.5f), new Vector3(0.46f, 0.46f, 0.46f), new Color(0.20f, 0.82f, 0.45f));

            CreateBox(parent, "Far_Blue_5m", new Vector3(0f, 1.4f, 5f), new Vector3(1.25f, 1.25f, 1.25f), new Color(0.14f, 0.43f, 0.92f));
            CreateBox(parent, "Far_Blue_7m", new Vector3(-1.6f, 1.0f, 7f), new Vector3(1.0f, 1.0f, 1.0f), new Color(0.28f, 0.52f, 0.95f));

            for (var i = 0; i < 9; i++)
            {
                var z = 1.5f + i * 0.7f;
                var x = (i % 2 == 0 ? -1f : 1f) * (1.25f + i * 0.07f);
                CreateSphere(parent, $"DepthMarker_{i + 1:00}", new Vector3(x, 0.22f, z), 0.16f, Color.Lerp(new Color(1f, 0.65f, 0.08f), new Color(0.55f, 0.18f, 0.95f), i / 8f));
            }
        }

        private void BuildStereoComfort(Transform parent)
        {
            CreateBox(parent, "Reference_Plane_2m", new Vector3(0f, 1.25f, 2f), new Vector3(2.8f, 1.7f, 0.05f), new Color(0.18f, 0.20f, 0.24f));

            for (var row = 0; row < 3; row++)
            {
                for (var col = 0; col < 5; col++)
                {
                    var z = 1.55f + row * 0.45f;
                    var x = (col - 2) * 0.48f;
                    var y = 0.72f + row * 0.42f;
                    var color = Color.Lerp(new Color(0.12f, 0.80f, 0.95f), new Color(0.95f, 0.36f, 0.22f), row / 2f);
                    CreateSphere(parent, $"Comfort_Target_R{row + 1}_C{col + 1}", new Vector3(x, y, z), 0.14f, color);
                }
            }

            CreateBox(parent, "Far_Reference", new Vector3(0f, 1.0f, 5.5f), new Vector3(2.4f, 1.2f, 0.12f), new Color(0.24f, 0.62f, 0.32f));
        }

        private void BuildImmersive2D(Transform parent)
        {
            CreateBox(parent, "Foreground_Frame_Left", new Vector3(-1.25f, 1.0f, 1.4f), new Vector3(0.18f, 1.9f, 0.18f), new Color(0.92f, 0.44f, 0.12f));
            CreateBox(parent, "Foreground_Frame_Right", new Vector3(1.25f, 1.0f, 1.4f), new Vector3(0.18f, 1.9f, 0.18f), new Color(0.92f, 0.44f, 0.12f));

            for (var i = 0; i < 7; i++)
            {
                var z = 1.8f + i * 0.62f;
                var scale = Mathf.Lerp(0.62f, 0.26f, i / 6f);
                var x = Mathf.Sin(i * 1.3f) * 0.75f;
                CreateBox(parent, $"Perspective_Block_{i + 1:00}", new Vector3(x, scale, z), Vector3.one * scale, Color.Lerp(new Color(0.93f, 0.16f, 0.31f), new Color(0.15f, 0.56f, 0.94f), i / 6f));
            }

            CreateBox(parent, "Background_Wall", new Vector3(0f, 1.4f, 6.4f), new Vector3(4.0f, 2.6f, 0.08f), new Color(0.16f, 0.18f, 0.22f));
        }

        private void CreateStereoRig(Transform parent)
        {
            var rigObject = new GameObject("VITURE_FullSBS_Rig");
            rigObject.transform.SetParent(parent, false);
            rigObject.transform.localPosition = new Vector3(0f, 1.25f, 0f);

            var leftObject = new GameObject("LeftEye");
            leftObject.transform.SetParent(rigObject.transform, false);
            var left = leftObject.AddComponent<Camera>();

            var rightObject = new GameObject("RightEye");
            rightObject.transform.SetParent(rigObject.transform, false);
            var right = rightObject.AddComponent<Camera>();

            ConfigureCamera(left);
            ConfigureCamera(right);

            var rig = rigObject.AddComponent<WiRRVitureSbsRig>();
            rig.Configure(left, right, ipdMeters);
        }

        private void CreateMonoCamera(Transform parent)
        {
            var cameraObject = new GameObject("VITURE_2D_Source_Camera");
            cameraObject.transform.SetParent(parent, false);
            cameraObject.transform.localPosition = new Vector3(0f, 1.25f, 0f);
            var camera = cameraObject.AddComponent<Camera>();
            ConfigureCamera(camera);
            camera.tag = "MainCamera";
        }

        private static void ConfigureCamera(Camera camera)
        {
            camera.fieldOfView = 60f;
            camera.nearClipPlane = 0.03f;
            camera.farClipPlane = 100f;
            camera.clearFlags = CameraClearFlags.Skybox;
        }

        private static void CreateLight(Transform parent)
        {
            var lightObject = new GameObject("Directional Light");
            lightObject.transform.SetParent(parent, false);
            lightObject.transform.rotation = Quaternion.Euler(48f, -32f, 0f);
            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.2f;
        }

        private static void CreateGround(Transform parent)
        {
            var ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ground.name = "Ground";
            ground.transform.SetParent(parent, false);
            ground.transform.localPosition = new Vector3(0f, -0.08f, 3.5f);
            ground.transform.localScale = new Vector3(8f, 0.12f, 9f);
            SetColor(ground, new Color(0.20f, 0.22f, 0.24f));
        }

        private static void CreateBox(Transform parent, string name, Vector3 position, Vector3 scale, Color color)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = position;
            go.transform.localScale = scale;
            SetColor(go, color);
        }

        private static void CreateSphere(Transform parent, string name, Vector3 position, float diameter, Color color)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = position;
            go.transform.localScale = Vector3.one * diameter;
            SetColor(go, color);
        }

        private static void SetColor(GameObject go, Color color)
        {
            var renderer = go.GetComponent<Renderer>();
            if (renderer == null)
                return;

            var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            if (shader == null)
                return;

            var material = new Material(shader) { color = color };
            renderer.material = material;
        }

        private void ClearGenerated()
        {
            if (generatedRoot != null)
            {
                if (Application.isPlaying)
                    Destroy(generatedRoot.gameObject);
                else
                    DestroyImmediate(generatedRoot.gameObject);
                generatedRoot = null;
                return;
            }

            var stale = transform.Find("VITURE_Demo_Generated");
            if (stale != null)
            {
                if (Application.isPlaying)
                    Destroy(stale.gameObject);
                else
                    DestroyImmediate(stale.gameObject);
            }
        }
    }
}
