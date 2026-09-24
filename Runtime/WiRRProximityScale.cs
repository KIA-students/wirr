using UnityEngine;

namespace KIA.WiRR
{
    /// <summary>
    /// Embodied scale demonstrator. A miniature grows toward room scale as the user
    /// approaches. Public SetExternalFactor supports controller sliders or gestures.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class WiRRProximityScale : MonoBehaviour
    {
        [InspectorName("Skalowana zawartość")]
        [SerializeField] private Transform content;

        [Min(0.1f)]
        [InspectorName("Odległość bliska [m]")]
        [SerializeField] private float nearDistance = 0.7f;

        [Min(0.2f)]
        [InspectorName("Odległość daleka [m]")]
        [SerializeField] private float farDistance = 3f;

        [Range(0.02f, 1f)]
        [InspectorName("Skala miniatury")]
        [SerializeField] private float miniatureScale = 0.18f;

        [Range(1f, 4f)]
        [InspectorName("Skala maksymalna")]
        [SerializeField] private float roomScale = 1.45f;

        [Min(0.1f)]
        [InspectorName("Wygładzanie")]
        [SerializeField] private float smoothing = 4f;

        private Transform viewer;
        private Vector3 baseScale = Vector3.one;
        private float externalFactor = -1f;
        private float factor;
        private int cameraLookupFrame;

        public float Factor => factor;

        public void Configure(
            Transform newContent,
            float newMiniatureScale = 0.18f,
            float newRoomScale = 1.45f)
        {
            content = newContent;
            miniatureScale = Mathf.Clamp(newMiniatureScale, 0.02f, 1f);
            roomScale = Mathf.Max(1f, newRoomScale);
            if (content != null)
                baseScale = content.localScale;
        }

        public void SetExternalFactor(float value)
        {
            externalFactor = Mathf.Clamp01(value);
        }

        public void UseProximity()
        {
            externalFactor = -1f;
        }

        private void Awake()
        {
            if (content != null)
                baseScale = content.localScale;
            ResolveViewer(true);
        }

        private void Update()
        {
            ResolveViewer(false);
            var targetFactor = externalFactor >= 0f ? externalFactor : ComputeProximityFactor();
            factor = Mathf.MoveTowards(factor, targetFactor, smoothing * Time.deltaTime);

            if (content != null)
            {
                var scale = Mathf.Lerp(miniatureScale, roomScale, factor);
                content.localScale = baseScale * scale;
            }
        }

        private float ComputeProximityFactor()
        {
            if (viewer == null)
                return 0f;

            var distance = Vector3.Distance(viewer.position, transform.position);
            if (farDistance <= nearDistance)
                return distance <= nearDistance ? 1f : 0f;

            return 1f - Mathf.InverseLerp(nearDistance, farDistance, distance);
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
