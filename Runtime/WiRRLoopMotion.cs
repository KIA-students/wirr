using UnityEngine;

namespace KIA.WiRR
{
    public enum WiRRLoopMotionMode
    {
        PingPong,
        Bob,
        Orbit,
        Rotate
    }

    /// <summary>
    /// Prosty, deterministyczny ruch zapętlony do demonstracji transformacji,
    /// ruchu kinematycznego oraz zależności pozycja-czas.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class WiRRLoopMotion : MonoBehaviour
    {
        [InspectorName("Tryb ruchu")]
        [SerializeField] private WiRRLoopMotionMode mode = WiRRLoopMotionMode.PingPong;

        [InspectorName("Wektor ruchu / oś")]
        [Tooltip("PingPong/Bob: przesunięcie od pozycji początkowej. Orbit: promień w osiach XZ. Rotate: oś obrotu.")]
        [SerializeField] private Vector3 motionVector = new Vector3(0f, 0.5f, 1.5f);

        [Min(0.1f)]
        [InspectorName("Okres [s]")]
        [SerializeField] private float periodSeconds = 4f;

        [Range(0f, 1f)]
        [InspectorName("Faza początkowa")]
        [SerializeField] private float phase01;

        [InspectorName("Przestrzeń lokalna")]
        [SerializeField] private bool localSpace = true;

        [InspectorName("Użyj Rigidbody, jeśli istnieje")]
        [SerializeField] private bool useRigidbody = true;

        private Rigidbody body;
        private Vector3 baseLocalPosition;
        private Vector3 baseWorldPosition;
        private Quaternion baseLocalRotation;
        private Quaternion baseWorldRotation;
        private float startTime;

        public WiRRLoopMotionMode Mode => mode;
        public float PeriodSeconds => periodSeconds;

        private void Awake()
        {
            CaptureOrigin();
            body = GetComponent<Rigidbody>();
            startTime = Time.time;
        }

        private void OnEnable()
        {
            if (!Application.isPlaying)
                return;
            CaptureOrigin();
            startTime = Time.time;
        }

        private void Update()
        {
            if (body != null && body.isKinematic && useRigidbody)
                return;
            ApplyAt(Time.time - startTime);
        }

        private void FixedUpdate()
        {
            if (body == null || !body.isKinematic || !useRigidbody)
                return;
            ApplyAt(Time.fixedTime - startTime);
        }

        public void Configure(
            WiRRLoopMotionMode newMode,
            Vector3 vector,
            float period,
            float phase = 0f,
            bool useLocalSpace = true)
        {
            mode = newMode;
            motionVector = vector;
            periodSeconds = Mathf.Max(0.1f, period);
            phase01 = Mathf.Repeat(phase, 1f);
            localSpace = useLocalSpace;
        }

        public void CaptureOrigin()
        {
            baseLocalPosition = transform.localPosition;
            baseWorldPosition = transform.position;
            baseLocalRotation = transform.localRotation;
            baseWorldRotation = transform.rotation;
        }

        private void ApplyAt(float time)
        {
            var t = Mathf.Repeat(time / Mathf.Max(0.1f, periodSeconds) + phase01, 1f);
            var angle = t * Mathf.PI * 2f;

            switch (mode)
            {
                case WiRRLoopMotionMode.PingPong:
                {
                    var alpha = 0.5f - 0.5f * Mathf.Cos(angle);
                    SetPosition((localSpace ? baseLocalPosition : baseWorldPosition) + motionVector * alpha);
                    break;
                }
                case WiRRLoopMotionMode.Bob:
                {
                    var alpha = 0.5f + 0.5f * Mathf.Sin(angle);
                    SetPosition((localSpace ? baseLocalPosition : baseWorldPosition) + motionVector * alpha);
                    break;
                }
                case WiRRLoopMotionMode.Orbit:
                {
                    var offset = new Vector3(
                        Mathf.Cos(angle) * motionVector.x,
                        Mathf.Sin(angle * 2f) * motionVector.y,
                        Mathf.Sin(angle) * motionVector.z);
                    SetPosition((localSpace ? baseLocalPosition : baseWorldPosition) + offset);
                    break;
                }
                case WiRRLoopMotionMode.Rotate:
                {
                    var axis = motionVector.sqrMagnitude > 0.0001f ? motionVector.normalized : Vector3.up;
                    var rotation = Quaternion.AngleAxis(t * 360f, axis);
                    SetRotation((localSpace ? baseLocalRotation : baseWorldRotation) * rotation);
                    break;
                }
            }
        }

        private void SetPosition(Vector3 position)
        {
            if (body != null && body.isKinematic && useRigidbody)
            {
                var world = localSpace && transform.parent != null
                    ? transform.parent.TransformPoint(position)
                    : position;
                body.MovePosition(world);
                return;
            }

            if (localSpace)
                transform.localPosition = position;
            else
                transform.position = position;
        }

        private void SetRotation(Quaternion rotation)
        {
            if (body != null && body.isKinematic && useRigidbody)
            {
                var world = localSpace && transform.parent != null
                    ? transform.parent.rotation * rotation
                    : rotation;
                body.MoveRotation(world);
                return;
            }

            if (localSpace)
                transform.localRotation = rotation;
            else
                transform.rotation = rotation;
        }
    }
}
