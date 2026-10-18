using UnityEngine;
using UnityEngine.InputSystem;

namespace KIA.WiRR
{
    /// <summary>
    /// Two-point mobile AR ruler. A plane/depth adapter updates the candidate point;
    /// screen taps confirm A and B. The component exposes DistanceMeters so UI or a
    /// report layer can format the measurement without depending on TextMeshPro.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class WiRRMobileARRuler : MonoBehaviour
    {
        [InspectorName("Punkt A")]
        [SerializeField] private Transform pointA;

        [InspectorName("Punkt B")]
        [SerializeField] private Transform pointB;

        [InspectorName("Wskaźnik kandydata")]
        [SerializeField] private Transform candidateMarker;

        [InspectorName("Linia pomiarowa")]
        [SerializeField] private LineRenderer measurementLine;

        [InspectorName("Znaczniki podziałki")]
        [SerializeField] private Transform[] ticks = System.Array.Empty<Transform>();

        [Min(0.01f)]
        [InspectorName("Odstęp podziałki [m]")]
        [SerializeField] private float tickSpacing = 0.10f;

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

        private Vector3 candidatePoint;
        private Vector3 candidateNormal = Vector3.up;
        private bool candidateValid;
        private float lastExternalHit = -999f;
        private int confirmedPoints;
        private Vector3 a;
        private Vector3 b;

        public int ConfirmedPoints => confirmedPoints;
        public float DistanceMeters => confirmedPoints >= 2 ? Vector3.Distance(a, b) : 0f;

        public void Configure(
            Transform first,
            Transform second,
            Transform preview,
            LineRenderer line,
            Transform[] tickMarkers)
        {
            pointA = first;
            pointB = second;
            candidateMarker = preview;
            measurementLine = line;
            ticks = tickMarkers ?? System.Array.Empty<Transform>();
            RefreshVisuals();
        }

        public void SetCandidatePoint(Vector3 worldPoint, Vector3 worldNormal, bool valid)
        {
            candidatePoint = worldPoint;
            candidateNormal = worldNormal.sqrMagnitude > 0.0001f ? worldNormal.normalized : Vector3.up;
            candidateValid = valid;
            lastExternalHit = Time.unscaledTime;
        }

        public void SetPoint(int index, Vector3 worldPoint)
        {
            if (index == 0)
            {
                a = worldPoint;
                confirmedPoints = Mathf.Max(confirmedPoints, 1);
            }
            else if (index == 1)
            {
                b = worldPoint;
                confirmedPoints = 2;
            }

            RefreshVisuals();
        }

        public bool ConfirmCandidate()
        {
            if (!candidateValid)
                return false;

            if (confirmedPoints >= 2)
                ResetMeasurement();

            if (confirmedPoints == 0)
            {
                a = candidatePoint;
                confirmedPoints = 1;
            }
            else
            {
                b = candidatePoint;
                confirmedPoints = 2;
            }

            RefreshVisuals();
            return true;
        }

        public void ResetMeasurement()
        {
            confirmedPoints = 0;
            RefreshVisuals();
        }

        private void Awake()
        {
            RefreshVisuals();
        }

        private void Update()
        {
            var externalFresh = Time.unscaledTime - lastExternalHit <= externalHitTimeout;
            if (!externalFresh && fallbackRaycast && Application.isEditor)
                UpdateFallbackHit();
            else if (!externalFresh && !Application.isEditor)
                candidateValid = false;

            UpdateCandidateVisual();

            if (PointerPressedThisFrame())
                ConfirmCandidate();
        }

        private void UpdateFallbackHit()
        {
            var camera = Camera.main;
            if (camera == null)
            {
                candidateValid = false;
                return;
            }

            var ray = camera.ScreenPointToRay(PointerPosition());
            if (Physics.Raycast(ray, out var hit, fallbackDistance, fallbackLayers, QueryTriggerInteraction.Ignore))
            {
                candidatePoint = hit.point;
                candidateNormal = hit.normal;
                candidateValid = true;
            }
            else
            {
                candidateValid = false;
            }
        }

        private void UpdateCandidateVisual()
        {
            if (candidateMarker == null)
                return;

            candidateMarker.gameObject.SetActive(candidateValid);
            if (!candidateValid)
                return;

            candidateMarker.position = candidatePoint + candidateNormal * 0.006f;
            candidateMarker.rotation = Quaternion.FromToRotation(Vector3.up, candidateNormal);
        }

        private void RefreshVisuals()
        {
            if (pointA != null)
            {
                pointA.gameObject.SetActive(confirmedPoints >= 1);
                if (confirmedPoints >= 1)
                    pointA.position = a;
            }

            if (pointB != null)
            {
                pointB.gameObject.SetActive(confirmedPoints >= 2);
                if (confirmedPoints >= 2)
                    pointB.position = b;
            }

            if (measurementLine != null)
            {
                measurementLine.enabled = confirmedPoints >= 2;
                if (confirmedPoints >= 2)
                {
                    measurementLine.positionCount = 2;
                    measurementLine.SetPosition(0, a);
                    measurementLine.SetPosition(1, b);
                }
            }

            UpdateTicks();
        }

        private void UpdateTicks()
        {
            var distance = DistanceMeters;
            var needed = confirmedPoints >= 2
                ? Mathf.Min(ticks.Length, Mathf.Max(0, Mathf.FloorToInt(distance / Mathf.Max(0.01f, tickSpacing)) - 1))
                : 0;

            for (var i = 0; i < ticks.Length; i++)
            {
                var tick = ticks[i];
                if (tick == null)
                    continue;

                var active = i < needed;
                tick.gameObject.SetActive(active);
                if (!active)
                    continue;

                var t = ((i + 1) * tickSpacing) / Mathf.Max(0.0001f, distance);
                tick.position = Vector3.Lerp(a, b, Mathf.Clamp01(t));
                tick.rotation = Quaternion.LookRotation(
                    Vector3.Cross((b - a).normalized, Vector3.up).sqrMagnitude > 0.0001f
                        ? Vector3.Cross((b - a).normalized, Vector3.up).normalized
                        : Vector3.forward,
                    Vector3.up);
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

        private static bool PointerPressedThisFrame()
        {
            if (Touchscreen.current != null &&
                Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
                return true;

            return Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
        }
    }
}
