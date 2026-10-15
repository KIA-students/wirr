using UnityEngine;

namespace KIA.WiRR
{
    /// <summary>
    /// Provider-neutral image-tracking anchor for mobile AR. AR Foundation tracked
    /// image data can be forwarded through SetTrackedImagePose. A simulation mode
    /// keeps the prefab useful in the Editor before a reference-image library exists.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class WiRRMobileARImageAnchor : MonoBehaviour
    {
        [InspectorName("Zawartość nad markerem")]
        [SerializeField] private Transform contentRoot;

        [Min(0.01f)]
        [InspectorName("Referencyjna szerokość obrazu [m]")]
        [SerializeField] private float referenceWidth = 0.20f;

        [Min(0.1f)]
        [InspectorName("Wygładzanie pozycji")]
        [SerializeField] private float positionSmoothing = 18f;

        [Min(0.1f)]
        [InspectorName("Wygładzanie obrotu")]
        [SerializeField] private float rotationSmoothing = 18f;

        [Min(0.1f)]
        [InspectorName("Timeout trackingu [s]")]
        [SerializeField] private float trackingTimeout = 0.65f;

        [InspectorName("Symuluj marker w Editorze")]
        [SerializeField] private bool simulateWhenStale = true;

        private Vector3 targetPosition;
        private Quaternion targetRotation = Quaternion.identity;
        private Vector2 physicalSize = new Vector2(0.20f, 0.20f);
        private float lastTrackingUpdate = -999f;
        private bool tracked;
        private float confidence;
        private Vector3 baseScale = Vector3.one;
        private Vector3 fallbackPosition;
        private Quaternion fallbackRotation;
        private string trackedImageName = string.Empty;

        public bool IsTracked => tracked && Time.unscaledTime - lastTrackingUpdate <= trackingTimeout;
        public float Confidence => confidence;
        public string TrackedImageName => trackedImageName;

        public void Configure(Transform content, float expectedImageWidth = 0.20f)
        {
            contentRoot = content;
            referenceWidth = Mathf.Max(0.01f, expectedImageWidth);
            CaptureBaseState();
        }

        public void SetTrackedImagePose(
            string imageName,
            Vector3 worldPosition,
            Quaternion worldRotation,
            Vector2 physicalSizeMeters,
            bool isTracked,
            float trackingConfidence = 1f)
        {
            trackedImageName = imageName ?? string.Empty;
            targetPosition = worldPosition;
            targetRotation = worldRotation;
            physicalSize = new Vector2(
                Mathf.Max(0.01f, physicalSizeMeters.x),
                Mathf.Max(0.01f, physicalSizeMeters.y));
            tracked = isTracked;
            confidence = Mathf.Clamp01(trackingConfidence);
            lastTrackingUpdate = Time.unscaledTime;
        }

        public void LostTracking()
        {
            tracked = false;
            confidence = 0f;
            lastTrackingUpdate = Time.unscaledTime;
        }

        private void Awake()
        {
            CaptureBaseState();
        }

        private void Update()
        {
            var live = IsTracked;
            if (!live && simulateWhenStale && Application.isEditor)
            {
                var bob = Mathf.Sin(Time.time * 1.7f) * 0.015f;
                targetPosition = fallbackPosition + Vector3.up * bob;
                targetRotation = fallbackRotation * Quaternion.Euler(0f, Mathf.Sin(Time.time * 0.5f) * 4f, 0f);
                physicalSize = new Vector2(referenceWidth, referenceWidth);
                confidence = 0.72f;
                live = true;
            }

            if (contentRoot != null)
                contentRoot.gameObject.SetActive(live);

            if (!live)
                return;

            var positionT = 1f - Mathf.Exp(-positionSmoothing * Time.deltaTime);
            var rotationT = 1f - Mathf.Exp(-rotationSmoothing * Time.deltaTime);
            transform.position = Vector3.Lerp(transform.position, targetPosition, positionT);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationT);

            if (contentRoot != null)
            {
                var scaleFactor = Mathf.Clamp(physicalSize.x / referenceWidth, 0.25f, 4f);
                var confidencePulse = 1f + Mathf.Sin(Time.time * 6f) * 0.025f * (1f - confidence);
                contentRoot.localScale = baseScale * scaleFactor * confidencePulse;
            }
        }

        private void CaptureBaseState()
        {
            fallbackPosition = transform.position;
            fallbackRotation = transform.rotation;
            if (contentRoot != null)
                baseScale = contentRoot.localScale;
        }
    }
}
