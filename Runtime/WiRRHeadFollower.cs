using UnityEngine;

namespace KIA.WiRR
{
    /// <summary>
    /// Comfortable diegetic/head-following panel. It follows the user's yaw with
    /// damping instead of being rigidly parented to the HMD. Pin/Unpin demonstrates
    /// the difference between body/head-referenced and world-locked UI.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class WiRRHeadFollower : MonoBehaviour
    {
        [Min(0.25f)]
        [InspectorName("Odległość od użytkownika [m]")]
        [SerializeField] private float distance = 1.15f;

        [InspectorName("Przesunięcie poziome [m]")]
        [SerializeField] private float horizontalOffset = 0.38f;

        [InspectorName("Przesunięcie pionowe [m]")]
        [SerializeField] private float verticalOffset = -0.08f;

        [Min(0.1f)]
        [InspectorName("Wygładzanie pozycji")]
        [SerializeField] private float positionSmoothing = 5f;

        [Min(0.1f)]
        [InspectorName("Wygładzanie obrotu")]
        [SerializeField] private float rotationSmoothing = 7f;

        [InspectorName("Przypięty do świata")]
        [SerializeField] private bool pinned;

        private Transform viewer;
        private int cameraLookupFrame;

        public bool IsPinned => pinned;

        public void Configure(float newDistance, float newHorizontalOffset, float newVerticalOffset)
        {
            distance = Mathf.Max(0.25f, newDistance);
            horizontalOffset = newHorizontalOffset;
            verticalOffset = newVerticalOffset;
        }

        public void Pin()
        {
            pinned = true;
        }

        public void Unpin()
        {
            pinned = false;
        }

        public void TogglePin()
        {
            pinned = !pinned;
        }

        private void OnEnable()
        {
            ResolveViewer(true);
        }

        private void LateUpdate()
        {
            ResolveViewer(false);
            if (viewer == null || pinned)
                return;

            var forward = Vector3.ProjectOnPlane(viewer.forward, Vector3.up);
            if (forward.sqrMagnitude < 0.001f)
                forward = Vector3.ProjectOnPlane(viewer.up, Vector3.up);
            forward.Normalize();

            var right = Vector3.Cross(Vector3.up, forward).normalized;
            var targetPosition =
                viewer.position +
                forward * distance +
                right * horizontalOffset +
                Vector3.up * verticalOffset;

            var targetRotation = Quaternion.LookRotation(transform.position - viewer.position, Vector3.up);
            var positionT = 1f - Mathf.Exp(-positionSmoothing * Time.deltaTime);
            var rotationT = 1f - Mathf.Exp(-rotationSmoothing * Time.deltaTime);

            transform.position = Vector3.Lerp(transform.position, targetPosition, positionT);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationT);
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
