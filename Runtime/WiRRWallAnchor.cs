using UnityEngine;

namespace KIA.WiRR
{
    /// <summary>
    /// Places virtual content on a detected real wall. Scene-understanding providers
    /// can call SetWallPlane; in Editor the same logic can be tested against colliders
    /// with TryAnchorFromViewerRay.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class WiRRWallAnchor : MonoBehaviour
    {
        [InspectorName("Zawartość kotwicy")]
        [SerializeField] private Transform contentRoot;

        [InspectorName("Nominalny rozmiar zawartości [m]")]
        [SerializeField] private Vector2 nominalSize = new Vector2(1.6f, 1.2f);

        [Min(0f)]
        [InspectorName("Odsunięcie od ściany [m]")]
        [SerializeField] private float surfaceOffset = 0.012f;

        [Min(0.5f)]
        [InspectorName("Maksymalny raycast [m]")]
        [SerializeField] private float raycastDistance = 5f;

        [InspectorName("Warstwy ścian fallback")]
        [SerializeField] private LayerMask wallLayers = ~0;

        private Vector3 initialPosition;
        private Quaternion initialRotation;
        private Vector3 initialContentScale = Vector3.one;
        private bool anchored;

        public bool IsAnchored => anchored;

        public void Configure(Transform content, Vector2 referenceSize)
        {
            contentRoot = content;
            nominalSize = new Vector2(
                Mathf.Max(0.1f, referenceSize.x),
                Mathf.Max(0.1f, referenceSize.y));
            if (contentRoot != null)
                initialContentScale = contentRoot.localScale;
        }

        public void SetWallPlane(Vector3 center, Vector3 normal, Vector2 wallSize)
        {
            var safeNormal = normal.sqrMagnitude > 0.0001f ? normal.normalized : Vector3.forward;
            var up = Mathf.Abs(Vector3.Dot(safeNormal, Vector3.up)) > 0.95f
                ? Vector3.forward
                : Vector3.up;

            transform.position = center + safeNormal * surfaceOffset;
            transform.rotation = Quaternion.LookRotation(safeNormal, up);

            if (contentRoot != null)
            {
                var widthScale = wallSize.x > 0.01f ? wallSize.x / nominalSize.x : 1f;
                var heightScale = wallSize.y > 0.01f ? wallSize.y / nominalSize.y : 1f;
                var uniform = Mathf.Clamp(Mathf.Min(widthScale, heightScale), 0.35f, 2.5f);
                contentRoot.localScale = initialContentScale * uniform;
            }

            var parallax = GetComponentInChildren<WiRRHeadParallax>();
            if (parallax != null)
                parallax.Recenter();

            anchored = true;
        }

        public bool TryAnchorFromViewerRay()
        {
            var camera = Camera.main;
            if (camera == null)
                return false;

            var ray = new Ray(camera.transform.position, camera.transform.forward);
            if (!Physics.Raycast(ray, out var hit, raycastDistance, wallLayers, QueryTriggerInteraction.Ignore))
                return false;

            SetWallPlane(hit.point, hit.normal, nominalSize);
            return true;
        }

        public void ClearAnchor()
        {
            transform.position = initialPosition;
            transform.rotation = initialRotation;
            if (contentRoot != null)
                contentRoot.localScale = initialContentScale;
            anchored = false;
        }

        private void Awake()
        {
            initialPosition = transform.position;
            initialRotation = transform.rotation;
            if (contentRoot != null)
                initialContentScale = contentRoot.localScale;
        }
    }
}
