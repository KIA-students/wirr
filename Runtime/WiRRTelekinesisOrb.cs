using UnityEngine;

namespace KIA.WiRR
{
    /// <summary>
    /// Distant-interaction demonstrator. Head gaze dwell pulls an object to a hover
    /// point in front of the user. Public BeginHold/Release methods can be connected
    /// to XRI Select/Hover events or hand-tracking gestures.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class WiRRTelekinesisOrb : MonoBehaviour
    {
        [InspectorName("Przenoszony obiekt")]
        [SerializeField] private Transform movable;

        [InspectorName("Wiązka")]
        [SerializeField] private LineRenderer beam;

        [Range(2f, 30f)]
        [InspectorName("Kąt aktywacji spojrzenia [°]")]
        [SerializeField] private float gazeAngle = 9f;

        [Min(0.1f)]
        [InspectorName("Czas zatrzymania spojrzenia [s]")]
        [SerializeField] private float dwellSeconds = 0.85f;

        [Min(0.2f)]
        [InspectorName("Odległość trzymania [m]")]
        [SerializeField] private float holdDistance = 0.8f;

        [InspectorName("Przesunięcie pionowe [m]")]
        [SerializeField] private float verticalOffset = -0.12f;

        [Min(0.1f)]
        [InspectorName("Czas do zwolnienia po utracie spojrzenia [s]")]
        [SerializeField] private float releaseDelay = 1.25f;

        [Min(0.1f)]
        [InspectorName("Szybkość ruchu")]
        [SerializeField] private float followSpeed = 10f;

        private Transform viewer;
        private Vector3 homePosition;
        private Quaternion homeRotation;
        private Vector3 baseScale = Vector3.one;
        private float dwell;
        private float lostGaze;
        private bool held;
        private bool returning;
        private int cameraLookupFrame;

        public bool IsHeld => held;
        public float DwellProgress => dwellSeconds <= 0f ? 1f : Mathf.Clamp01(dwell / dwellSeconds);

        public void Configure(
            Transform newMovable,
            LineRenderer newBeam,
            float newHoldDistance = 0.8f,
            float newDwellSeconds = 0.85f)
        {
            movable = newMovable;
            beam = newBeam;
            holdDistance = Mathf.Max(0.2f, newHoldDistance);
            dwellSeconds = Mathf.Max(0.1f, newDwellSeconds);
            CaptureHome();
        }

        public void BeginHold()
        {
            if (movable == null)
                return;

            held = true;
            returning = false;
            dwell = dwellSeconds;
            lostGaze = 0f;
            if (beam != null)
                beam.enabled = true;
        }

        public void Release()
        {
            held = false;
            returning = true;
            dwell = 0f;
            lostGaze = 0f;
        }

        public void ToggleHold()
        {
            if (held) Release();
            else BeginHold();
        }

        private void Awake()
        {
            CaptureHome();
            ResolveViewer(true);
            if (beam != null)
                beam.enabled = false;
        }

        private void Update()
        {
            ResolveViewer(false);
            if (movable == null)
                return;

            var gazed = IsGazedAt();
            if (!held && !returning)
            {
                dwell = gazed ? Mathf.Min(dwellSeconds, dwell + Time.deltaTime) : Mathf.Max(0f, dwell - Time.deltaTime * 1.6f);
                if (dwell >= dwellSeconds)
                    BeginHold();
            }
            else if (held)
            {
                if (gazed)
                    lostGaze = 0f;
                else
                    lostGaze += Time.deltaTime;

                if (lostGaze >= releaseDelay)
                    Release();
            }

            var t = 1f - Mathf.Exp(-followSpeed * Time.deltaTime);
            if (held && viewer != null)
            {
                var target = viewer.position + viewer.forward * holdDistance + viewer.up * verticalOffset;
                movable.position = Vector3.Lerp(movable.position, target, t);
                movable.rotation = Quaternion.Slerp(
                    movable.rotation,
                    Quaternion.LookRotation(viewer.forward, viewer.up),
                    t);
            }
            else if (returning)
            {
                movable.position = Vector3.Lerp(movable.position, homePosition, t);
                movable.rotation = Quaternion.Slerp(movable.rotation, homeRotation, t);
                if ((movable.position - homePosition).sqrMagnitude < 0.0001f)
                {
                    movable.position = homePosition;
                    movable.rotation = homeRotation;
                    returning = false;
                    if (beam != null)
                        beam.enabled = false;
                }
            }

            var charge = held ? 1f : DwellProgress;
            movable.localScale = baseScale * (1f + 0.14f * charge + Mathf.Sin(Time.time * 8f) * 0.025f * charge);
            UpdateBeam(charge);
        }

        private bool IsGazedAt()
        {
            if (viewer == null || movable == null)
                return false;

            var delta = movable.position - viewer.position;
            if (delta.sqrMagnitude < 0.0025f)
                return false;

            var direction = delta.normalized;
            var dot = Vector3.Dot(viewer.forward, direction);
            return dot >= Mathf.Cos(gazeAngle * Mathf.Deg2Rad);
        }

        private void UpdateBeam(float charge)
        {
            if (beam == null || movable == null)
                return;

            beam.enabled = held || charge > 0.02f || returning;
            if (!beam.enabled)
                return;

            beam.positionCount = 2;
            beam.SetPosition(0, transform.position + Vector3.up * 0.48f);
            beam.SetPosition(1, movable.position);
            var width = Mathf.Lerp(0.006f, 0.028f, Mathf.Clamp01(charge));
            beam.startWidth = width;
            beam.endWidth = width * 0.35f;
        }

        private void CaptureHome()
        {
            if (movable == null)
                return;
            homePosition = movable.position;
            homeRotation = movable.rotation;
            baseScale = movable.localScale;
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
