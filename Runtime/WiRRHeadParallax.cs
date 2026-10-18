using UnityEngine;

namespace KIA.WiRR
{
    /// <summary>
    /// Head-coupled parallax for layered XR dioramas and portal windows.
    /// The effect uses the main camera pose, so 6DoF head translation changes the
    /// apparent depth of layers without requiring an XR SDK dependency.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class WiRRHeadParallax : MonoBehaviour
    {
        [InspectorName("Warstwy paralaksy")]
        [SerializeField] private Transform[] layers = System.Array.Empty<Transform>();

        [InspectorName("Współczynniki głębokości")]
        [SerializeField] private float[] depthFactors = System.Array.Empty<float>();

        [Min(0.01f)]
        [InspectorName("Maksymalne przesunięcie [m]")]
        [SerializeField] private float maxShift = 0.24f;

        [Min(0.1f)]
        [InspectorName("Wygładzanie")]
        [SerializeField] private float smoothing = 12f;

        [InspectorName("Uwzględniaj ruch pionowy")]
        [SerializeField] private bool verticalParallax = true;

        private Transform viewer;
        private Vector3[] baseLocalPositions = System.Array.Empty<Vector3>();
        private Vector3 viewerReferenceLocal;
        private bool hasViewerReference;
        private int cameraLookupFrame;

        public void Configure(Transform[] newLayers, float[] newDepthFactors, float newMaxShift = 0.24f)
        {
            layers = newLayers ?? System.Array.Empty<Transform>();
            depthFactors = newDepthFactors ?? System.Array.Empty<float>();
            maxShift = Mathf.Max(0.01f, newMaxShift);
            CaptureBasePositions();
        }

        private void Awake()
        {
            CaptureBasePositions();
            ResolveViewer(true);
        }

        private void OnEnable()
        {
            if (baseLocalPositions.Length != layers.Length)
                CaptureBasePositions();
            ResolveViewer(true);
        }

        private void LateUpdate()
        {
            ResolveViewer(false);
            if (viewer == null || layers == null || layers.Length == 0)
                return;

            if (baseLocalPositions.Length != layers.Length)
                CaptureBasePositions();

            var viewerLocal = transform.InverseTransformPoint(viewer.position);
            if (!hasViewerReference)
            {
                viewerReferenceLocal = viewerLocal;
                hasViewerReference = true;
            }

            var delta = viewerLocal - viewerReferenceLocal;
            var lateral = new Vector2(
                Mathf.Clamp(delta.x, -maxShift, maxShift),
                verticalParallax ? Mathf.Clamp(delta.y, -maxShift, maxShift) : 0f);

            var t = 1f - Mathf.Exp(-smoothing * Time.deltaTime);
            for (var i = 0; i < layers.Length; i++)
            {
                var layer = layers[i];
                if (layer == null)
                    continue;

                var factor = i < depthFactors.Length ? depthFactors[i] : 0.1f;
                var desired = baseLocalPositions[i] + new Vector3(
                    -lateral.x * factor,
                    -lateral.y * factor,
                    0f);
                layer.localPosition = Vector3.Lerp(layer.localPosition, desired, t);
            }
        }

        private void CaptureBasePositions()
        {
            if (layers == null)
                layers = System.Array.Empty<Transform>();

            baseLocalPositions = new Vector3[layers.Length];
            for (var i = 0; i < layers.Length; i++)
                baseLocalPositions[i] = layers[i] != null ? layers[i].localPosition : Vector3.zero;
        }

        public void Recenter()
        {
            if (viewer == null)
                ResolveViewer(true);

            if (viewer != null)
            {
                viewerReferenceLocal = transform.InverseTransformPoint(viewer.position);
                hasViewerReference = true;
            }
        }

        private void ResolveViewer(bool force)
        {
            if (!force && viewer != null)
                return;
            if (!force && Time.frameCount < cameraLookupFrame)
                return;

            var camera = Camera.main;
            var nextViewer = camera != null ? camera.transform : null;
            if (nextViewer != viewer)
                hasViewerReference = false;
            viewer = nextViewer;
            cameraLookupFrame = Time.frameCount + 30;
        }
    }
}
