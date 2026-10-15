using UnityEngine;
using UnityEngine.InputSystem;

namespace KIA.WiRR
{
    /// <summary>
    /// Phone-AR surface painter. An AR raycast/depth adapter updates the point under
    /// the finger; dragging paints pooled virtual marks that follow the real surface.
    /// The same logic can be tested on ordinary scene colliders in the Editor.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class WiRRMobileARSurfacePainter : MonoBehaviour
    {
        [InspectorName("Pula znaczników")]
        [SerializeField] private Transform[] paintMarkers = System.Array.Empty<Transform>();

        [Min(0.005f)]
        [InspectorName("Minimalny odstęp punktów [m]")]
        [SerializeField] private float minSpacing = 0.035f;

        [Min(0f)]
        [InspectorName("Odsunięcie od powierzchni [m]")]
        [SerializeField] private float surfaceOffset = 0.006f;

        [Min(0.1f)]
        [InspectorName("Timeout zewnętrznego hitu [s]")]
        [SerializeField] private float externalHitTimeout = 0.35f;

        [InspectorName("Fallback raycast w Editorze")]
        [SerializeField] private bool fallbackRaycast = true;

        [Min(0.5f)]
        [InspectorName("Zasięg fallback [m]")]
        [SerializeField] private float fallbackDistance = 8f;

        [InspectorName("Warstwy fallback")]
        [SerializeField] private LayerMask fallbackLayers = ~0;

        private Vector3 hitPoint;
        private Vector3 hitNormal = Vector3.up;
        private bool hitValid;
        private float lastExternalHit = -999f;
        private Vector3 lastPaintPoint;
        private bool hasLastPoint;
        private int writeIndex;
        private int paintedCount;

        public int PaintedCount => paintedCount;

        public void Configure(Transform[] markers)
        {
            paintMarkers = markers ?? System.Array.Empty<Transform>();
            ClearPaint();
        }

        public void SetSurfaceHit(Vector3 worldPoint, Vector3 worldNormal, bool valid)
        {
            hitPoint = worldPoint;
            hitNormal = worldNormal.sqrMagnitude > 0.0001f ? worldNormal.normalized : Vector3.up;
            hitValid = valid;
            lastExternalHit = Time.unscaledTime;
        }

        public bool PaintAtCurrentHit()
        {
            if (!hitValid || paintMarkers.Length == 0)
                return false;

            var point = hitPoint + hitNormal * surfaceOffset;
            if (hasLastPoint && Vector3.Distance(lastPaintPoint, point) < minSpacing)
                return false;

            var marker = paintMarkers[writeIndex];
            if (marker == null)
                return false;

            marker.gameObject.SetActive(true);
            marker.position = point;
            marker.rotation = Quaternion.FromToRotation(Vector3.forward, hitNormal);
            marker.localScale = Vector3.one * Mathf.Lerp(0.025f, 0.055f, Mathf.PingPong(paintedCount * 0.13f, 1f));

            lastPaintPoint = point;
            hasLastPoint = true;
            writeIndex = (writeIndex + 1) % paintMarkers.Length;
            paintedCount = Mathf.Min(paintedCount + 1, paintMarkers.Length);
            return true;
        }

        public void BeginStroke()
        {
            hasLastPoint = false;
        }

        public void EndStroke()
        {
            hasLastPoint = false;
        }

        public void ClearPaint()
        {
            foreach (var marker in paintMarkers)
                if (marker != null)
                    marker.gameObject.SetActive(false);

            writeIndex = 0;
            paintedCount = 0;
            hasLastPoint = false;
        }

        private void Awake()
        {
            ClearPaint();
        }

        private void Update()
        {
            var externalFresh = Time.unscaledTime - lastExternalHit <= externalHitTimeout;
            if (!externalFresh && fallbackRaycast && Application.isEditor)
                UpdateFallbackHit();
            else if (!externalFresh && !Application.isEditor)
                hitValid = false;

            if (PointerPressedThisFrame())
                BeginStroke();

            if (PointerIsPressed())
                PaintAtCurrentHit();

            if (PointerReleasedThisFrame())
                EndStroke();
        }

        private void UpdateFallbackHit()
        {
            var camera = Camera.main;
            if (camera == null)
            {
                hitValid = false;
                return;
            }

            var ray = camera.ScreenPointToRay(PointerPosition());
            if (Physics.Raycast(ray, out var hit, fallbackDistance, fallbackLayers, QueryTriggerInteraction.Ignore))
            {
                hitPoint = hit.point;
                hitNormal = hit.normal;
                hitValid = true;
            }
            else
            {
                hitValid = false;
            }
        }

        private static Vector2 PointerPosition()
        {
            if (Touchscreen.current != null)
                return Touchscreen.current.primaryTouch.position.ReadValue();
            if (Mouse.current != null)
                return Mouse.current.position.ReadValue();
            return new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        }

        private static bool PointerIsPressed()
        {
            if (Touchscreen.current != null)
                return Touchscreen.current.primaryTouch.press.isPressed;
            return Mouse.current != null && Mouse.current.leftButton.isPressed;
        }

        private static bool PointerPressedThisFrame()
        {
            if (Touchscreen.current != null)
                return Touchscreen.current.primaryTouch.press.wasPressedThisFrame;
            return Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
        }

        private static bool PointerReleasedThisFrame()
        {
            if (Touchscreen.current != null)
                return Touchscreen.current.primaryTouch.press.wasReleasedThisFrame;
            return Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame;
        }
    }
}
