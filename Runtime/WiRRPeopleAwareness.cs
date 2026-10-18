using UnityEngine;

namespace KIA.WiRR
{
    /// <summary>
    /// Privacy-preserving people-awareness visualizer. It needs only world positions,
    /// not identity or face data. A detector/provider can call SetPeople/SetPerson.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class WiRRPeopleAwareness : MonoBehaviour
    {
        [InspectorName("Markery osób")]
        [SerializeField] private Transform[] personMarkers = System.Array.Empty<Transform>();

        [Min(0.1f)]
        [InspectorName("Wygładzanie")]
        [SerializeField] private float smoothing = 9f;

        [Min(0.1f)]
        [InspectorName("Strefa bliska [m]")]
        [SerializeField] private float nearDistance = 1.0f;

        [Min(0.2f)]
        [InspectorName("Strefa daleka [m]")]
        [SerializeField] private float farDistance = 3.0f;

        [Min(0.1f)]
        [InspectorName("Timeout detekcji [s]")]
        [SerializeField] private float trackingTimeout = 1.0f;

        [InspectorName("Symuluj ludzi bez detektora")]
        [SerializeField] private bool simulateWhenStale = true;

        private Vector3[] targets = System.Array.Empty<Vector3>();
        private bool[] targetVisible = System.Array.Empty<bool>();
        private float lastExternalUpdate = -999f;
        private Transform viewer;
        private int cameraLookupFrame;

        public int Capacity => personMarkers?.Length ?? 0;

        public void Configure(Transform[] markers)
        {
            personMarkers = markers ?? System.Array.Empty<Transform>();
            EnsureBuffers();
        }

        public void SetPeople(Vector3[] worldPositions)
        {
            EnsureBuffers();
            var count = worldPositions?.Length ?? 0;
            for (var i = 0; i < personMarkers.Length; i++)
            {
                targetVisible[i] = i < count;
                if (i < count)
                    targets[i] = worldPositions[i];
            }

            lastExternalUpdate = Time.unscaledTime;
        }

        public void SetPerson(int slot, Vector3 worldPosition, bool visible = true)
        {
            EnsureBuffers();
            if (slot < 0 || slot >= personMarkers.Length)
                return;

            targets[slot] = worldPosition;
            targetVisible[slot] = visible;
            lastExternalUpdate = Time.unscaledTime;
        }

        public void ClearPeople()
        {
            EnsureBuffers();
            for (var i = 0; i < targetVisible.Length; i++)
                targetVisible[i] = false;
            lastExternalUpdate = Time.unscaledTime;
        }

        private void Awake()
        {
            EnsureBuffers();
            ResolveViewer(true);
        }

        private void Update()
        {
            ResolveViewer(false);
            var stale = Time.unscaledTime - lastExternalUpdate > trackingTimeout;
            if (stale && simulateWhenStale)
                SimulatePeople();

            ApplyMarkers();
        }

        private void SimulatePeople()
        {
            EnsureBuffers();
            var count = Mathf.Min(2, personMarkers.Length);
            for (var i = 0; i < personMarkers.Length; i++)
                targetVisible[i] = i < count;

            for (var i = 0; i < count; i++)
            {
                var phase = Time.time * (0.25f + i * 0.08f) + i * 2.4f;
                targets[i] = transform.position + new Vector3(
                    Mathf.Cos(phase) * (1.6f + i * 0.45f),
                    0f,
                    Mathf.Sin(phase) * (1.25f + i * 0.4f));
            }
        }

        private void ApplyMarkers()
        {
            var t = 1f - Mathf.Exp(-smoothing * Time.deltaTime);
            for (var i = 0; i < personMarkers.Length; i++)
            {
                var marker = personMarkers[i];
                if (marker == null)
                    continue;

                var active = i < targetVisible.Length && targetVisible[i];
                marker.gameObject.SetActive(active);
                if (!active)
                    continue;

                marker.position = Vector3.Lerp(marker.position, targets[i], t);

                var distance = viewer != null
                    ? Vector3.Distance(viewer.position, marker.position)
                    : farDistance;
                var proximity = 1f - Mathf.InverseLerp(nearDistance, farDistance, distance);
                proximity = Mathf.Clamp01(proximity);
                var pulse = 1f + Mathf.Sin(Time.time * 6f + i) * 0.08f * proximity;
                marker.localScale = Vector3.one * Mathf.Lerp(0.78f, 1.18f, proximity) * pulse;
            }
        }

        private void EnsureBuffers()
        {
            var count = personMarkers?.Length ?? 0;
            if (targets.Length != count)
                targets = new Vector3[count];
            if (targetVisible.Length != count)
                targetVisible = new bool[count];
        }

        private void ResolveViewer(bool force)
        {
            if (!force && viewer != null)
                return;
            if (!force && Time.frameCount < cameraLookupFrame)
                return;

            var camera = Camera.main;
            viewer = camera != null ? camera.transform : null;
            cameraLookupFrame = Time.frameCount + 30;
        }
    }
}
