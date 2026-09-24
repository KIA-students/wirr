using UnityEngine;

namespace KIA.WiRR
{
    /// <summary>
    /// Visualizes samples from depth/spatial-mesh providers. Without a provider it
    /// scans ordinary scene colliders with camera rays, which makes the same prefab
    /// usable in Editor and later with real environment depth.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class WiRRSpatialSurfaceScanner : MonoBehaviour
    {
        [InspectorName("Markery próbek")]
        [SerializeField] private Transform[] sampleMarkers = System.Array.Empty<Transform>();

        [Min(0.5f)]
        [InspectorName("Zasięg [m]")]
        [SerializeField] private float maxDistance = 6f;

        [Min(0.1f)]
        [InspectorName("Czas życia próbki [s]")]
        [SerializeField] private float sampleLifetime = 2.2f;

        [Min(1)]
        [InspectorName("Promienie na klatkę")]
        [SerializeField] private int raysPerFrame = 3;

        [InspectorName("Warstwy fallback raycast")]
        [SerializeField] private LayerMask fallbackLayers = ~0;

        [InspectorName("Raycast fallback bez depth API")]
        [SerializeField] private bool raycastWhenNoExternalDepth = true;

        [Min(0.1f)]
        [InspectorName("Timeout zewnętrznego depth [s]")]
        [SerializeField] private float externalDepthTimeout = 0.75f;

        private float[] sampleTimes = System.Array.Empty<float>();
        private int writeIndex;
        private int scanIndex;
        private float lastExternalDepth = -999f;
        private Transform viewer;

        public void Configure(Transform[] markers, float range = 6f)
        {
            sampleMarkers = markers ?? System.Array.Empty<Transform>();
            maxDistance = Mathf.Max(0.5f, range);
            EnsureBuffers();
        }

        public void SubmitSurfaceSample(Vector3 worldPoint, Vector3 worldNormal, float confidence = 1f)
        {
            lastExternalDepth = Time.unscaledTime;
            AddSample(worldPoint, worldNormal, Mathf.Clamp01(confidence));
        }

        public void SubmitSurfaceSamples(Vector3[] worldPoints, Vector3[] worldNormals)
        {
            var count = Mathf.Min(worldPoints?.Length ?? 0, worldNormals?.Length ?? 0);
            lastExternalDepth = Time.unscaledTime;
            for (var i = 0; i < count; i++)
                AddSample(worldPoints[i], worldNormals[i], 1f);
        }

        public void ClearSamples()
        {
            EnsureBuffers();
            for (var i = 0; i < sampleMarkers.Length; i++)
            {
                if (sampleMarkers[i] != null)
                    sampleMarkers[i].gameObject.SetActive(false);
                sampleTimes[i] = -999f;
            }
        }

        private void Awake()
        {
            EnsureBuffers();
            ResolveViewer();
        }

        private void Update()
        {
            ResolveViewer();

            var externalFresh = Time.unscaledTime - lastExternalDepth <= externalDepthTimeout;
            if (!externalFresh && raycastWhenNoExternalDepth && viewer != null)
                ScanFallback();

            for (var i = 0; i < sampleMarkers.Length; i++)
            {
                var marker = sampleMarkers[i];
                if (marker == null || !marker.gameObject.activeSelf)
                    continue;

                var age = Time.unscaledTime - sampleTimes[i];
                if (age > sampleLifetime)
                {
                    marker.gameObject.SetActive(false);
                    continue;
                }

                var life = 1f - Mathf.Clamp01(age / sampleLifetime);
                marker.localScale = Vector3.one * Mathf.Lerp(0.012f, 0.055f, life);
            }
        }

        private void ScanFallback()
        {
            var camera = Camera.main;
            if (camera == null)
                return;

            for (var rayIndex = 0; rayIndex < raysPerFrame; rayIndex++)
            {
                scanIndex++;
                var u = Mathf.Repeat(scanIndex * 0.61803398875f, 1f);
                var v = Mathf.Repeat(scanIndex * 0.41421356237f, 1f);
                u = Mathf.Lerp(0.08f, 0.92f, u);
                v = Mathf.Lerp(0.12f, 0.88f, v);

                var ray = camera.ViewportPointToRay(new Vector3(u, v, 0f));
                if (Physics.Raycast(ray, out var hit, maxDistance, fallbackLayers, QueryTriggerInteraction.Ignore))
                    AddSample(hit.point, hit.normal, 0.72f);
            }
        }

        private void AddSample(Vector3 point, Vector3 normal, float confidence)
        {
            EnsureBuffers();
            if (sampleMarkers.Length == 0)
                return;

            var marker = sampleMarkers[writeIndex];
            if (marker != null)
            {
                marker.gameObject.SetActive(true);
                marker.position = point + normal.normalized * 0.008f;

                var safeNormal = normal.sqrMagnitude > 0.0001f ? normal.normalized : Vector3.up;
                var up = Mathf.Abs(Vector3.Dot(safeNormal, Vector3.up)) > 0.94f
                    ? Vector3.forward
                    : Vector3.up;
                marker.rotation = Quaternion.LookRotation(safeNormal, up);
                marker.localScale = Vector3.one * Mathf.Lerp(0.018f, 0.060f, confidence);
                sampleTimes[writeIndex] = Time.unscaledTime;
            }

            writeIndex = (writeIndex + 1) % sampleMarkers.Length;
        }

        private void EnsureBuffers()
        {
            var count = sampleMarkers?.Length ?? 0;
            if (sampleTimes.Length != count)
            {
                sampleTimes = new float[count];
                for (var i = 0; i < count; i++)
                    sampleTimes[i] = -999f;
            }
        }

        private void ResolveViewer()
        {
            var camera = Camera.main;
            viewer = camera != null ? camera.transform : null;
        }
    }
}
