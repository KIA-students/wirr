using UnityEngine;

namespace KIA.WiRR
{
    /// <summary>
    /// Gaze/dwell-ready visual response. The object opens when the user looks at it,
    /// while SetExternalActivation can be driven by XRI hover, hand proximity or UI.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class WiRRGazeBloom : MonoBehaviour
    {
        [InspectorName("Rdzeń")]
        [SerializeField] private Transform core;

        [InspectorName("Płatki / satelity")]
        [SerializeField] private Transform[] petals = System.Array.Empty<Transform>();

        [Range(2f, 45f)]
        [InspectorName("Kąt aktywacji spojrzenia [°]")]
        [SerializeField] private float gazeAngle = 13f;

        [Min(0.2f)]
        [InspectorName("Maksymalna odległość [m]")]
        [SerializeField] private float maxDistance = 5f;

        [Range(0f, 1.5f)]
        [InspectorName("Rozwarcie")]
        [SerializeField] private float opening = 0.55f;

        [Min(0.1f)]
        [InspectorName("Szybkość reakcji")]
        [SerializeField] private float responseSpeed = 5f;

        [Min(0f)]
        [InspectorName("Prędkość obrotu [°/s]")]
        [SerializeField] private float rotationSpeed = 50f;

        private Transform viewer;
        private Vector3[] basePetalPositions = System.Array.Empty<Vector3>();
        private Vector3 baseCoreScale = Vector3.one;
        private float activation;
        private float externalActivation;
        private int cameraLookupFrame;

        public float Activation => activation;

        public void Configure(Transform newCore, Transform[] newPetals, float newOpening = 0.55f)
        {
            core = newCore;
            petals = newPetals ?? System.Array.Empty<Transform>();
            opening = Mathf.Max(0f, newOpening);
            CaptureBaseState();
        }

        public void SetExternalActivation(float value)
        {
            externalActivation = Mathf.Clamp01(value);
        }

        public void ClearExternalActivation()
        {
            externalActivation = 0f;
        }

        private void Awake()
        {
            CaptureBaseState();
            ResolveViewer(true);
        }

        private void Update()
        {
            ResolveViewer(false);
            var gazeActivation = ComputeGazeActivation();
            var target = Mathf.Max(gazeActivation, externalActivation);
            activation = Mathf.MoveTowards(
                activation,
                target,
                responseSpeed * Time.deltaTime);

            var pulse = 1f + Mathf.Sin(Time.time * 5f) * 0.035f * activation;
            if (core != null)
                core.localScale = baseCoreScale * (1f + activation * 0.28f) * pulse;

            for (var i = 0; i < petals.Length; i++)
            {
                var petal = petals[i];
                if (petal == null)
                    continue;

                var basePosition = i < basePetalPositions.Length
                    ? basePetalPositions[i]
                    : petal.localPosition;
                var planar = new Vector3(basePosition.x, basePosition.y, 0f);
                var direction = planar.sqrMagnitude > 0.0001f ? planar.normalized : Vector3.up;
                petal.localPosition = basePosition + direction * opening * activation;
                petal.localRotation *= Quaternion.Euler(0f, 0f, rotationSpeed * Time.deltaTime * activation);
            }

            if (activation > 0.001f)
                transform.Rotate(Vector3.up, rotationSpeed * 0.18f * activation * Time.deltaTime, Space.Self);
        }

        private float ComputeGazeActivation()
        {
            if (viewer == null)
                return 0f;

            var delta = transform.position - viewer.position;
            var distance = delta.magnitude;
            if (distance < 0.05f || distance > maxDistance)
                return 0f;

            var dot = Vector3.Dot(viewer.forward, delta / distance);
            var threshold = Mathf.Cos(gazeAngle * Mathf.Deg2Rad);
            var angular = Mathf.InverseLerp(threshold, 1f, dot);
            var distanceWeight = 1f - Mathf.InverseLerp(maxDistance * 0.65f, maxDistance, distance);
            return Mathf.Clamp01(angular * distanceWeight);
        }

        private void CaptureBaseState()
        {
            if (petals == null)
                petals = System.Array.Empty<Transform>();

            basePetalPositions = new Vector3[petals.Length];
            for (var i = 0; i < petals.Length; i++)
                basePetalPositions[i] = petals[i] != null ? petals[i].localPosition : Vector3.zero;

            if (core != null)
                baseCoreScale = core.localScale;
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
