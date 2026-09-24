using UnityEngine;
using UnityEngine.InputSystem;

namespace KIA.WiRR
{
    /// <summary>
    /// Phone-AR placement controller. An AR Foundation adapter can feed the current
    /// plane/raycast hit through SetSurfaceHit. Without a provider the component
    /// raycasts ordinary scene colliders so it can be demonstrated in the Editor.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class WiRRMobileARTapPlacement : MonoBehaviour
    {
        [InspectorName("Reticle podglądu")]
        [SerializeField] private Transform reticle;

        [InspectorName("Umieszczany obiekt")]
        [SerializeField] private Transform placedContent;

        [Min(0.01f)]
        [InspectorName("Odsunięcie od powierzchni [m]")]
        [SerializeField] private float surfaceOffset = 0.01f;

        [Min(0.1f)]
        [InspectorName("Wygładzanie reticle")]
        [SerializeField] private float smoothing = 18f;

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

        private Pose candidatePose;
        private bool candidateValid;
        private float candidateConfidence;
        private float lastExternalHit = -999f;
        private bool hasPlacedObject;
        private Vector3 baseContentScale = Vector3.one;

        public bool HasCandidate => candidateValid;
        public bool HasPlacedObject => hasPlacedObject;
        public float CandidateConfidence => candidateConfidence;

        public void Configure(Transform previewReticle, Transform content)
        {
            reticle = previewReticle;
            placedContent = content;
            if (placedContent != null)
                baseContentScale = placedContent.localScale;
            RefreshVisibility();
        }

        public void SetSurfaceHit(Vector3 worldPoint, Vector3 worldNormal, bool valid, float confidence = 1f)
        {
            lastExternalHit = Time.unscaledTime;
            candidateValid = valid;
            candidateConfidence = Mathf.Clamp01(confidence);
            if (!valid)
                return;

            candidatePose = new Pose(
                worldPoint + worldNormal.normalized * surfaceOffset,
                RotationFromNormal(worldNormal));
        }

        public void SetSurfacePose(Pose pose, bool valid, float confidence = 1f)
        {
            lastExternalHit = Time.unscaledTime;
            candidateValid = valid;
            candidateConfidence = Mathf.Clamp01(confidence);
            if (valid)
                candidatePose = pose;
        }

        public bool ConfirmPlacement()
        {
            if (!candidateValid || placedContent == null)
                return false;

            placedContent.position = candidatePose.position;
            placedContent.rotation = candidatePose.rotation;
            placedContent.localScale = baseContentScale;
            placedContent.gameObject.SetActive(true);
            hasPlacedObject = true;
            return true;
        }

        public void ClearPlacement()
        {
            hasPlacedObject = false;
            if (placedContent != null)
                placedContent.gameObject.SetActive(false);
        }

        private void Awake()
        {
            if (placedContent != null)
            {
                baseContentScale = placedContent.localScale;
                placedContent.gameObject.SetActive(false);
            }

            RefreshVisibility();
        }

        private void Update()
        {
            var externalFresh = Time.unscaledTime - lastExternalHit <= externalHitTimeout;
            if (!externalFresh && fallbackRaycast && Application.isEditor)
                UpdateFallbackHit();
            else if (!externalFresh && !Application.isEditor)
                candidateValid = false;

            UpdateReticle();

            if (PointerPressedThisFrame())
                ConfirmPlacement();
        }

        private void UpdateReticle()
        {
            if (reticle == null)
                return;

            reticle.gameObject.SetActive(candidateValid);
            if (!candidateValid)
                return;

            var t = 1f - Mathf.Exp(-smoothing * Time.deltaTime);
            reticle.position = Vector3.Lerp(reticle.position, candidatePose.position, t);
            reticle.rotation = Quaternion.Slerp(reticle.rotation, candidatePose.rotation, t);
            var scale = Mathf.Lerp(0.72f, 1f, candidateConfidence);
            reticle.localScale = Vector3.one * scale;
        }

        private void UpdateFallbackHit()
        {
            var camera = Camera.main;
            if (camera == null)
            {
                candidateValid = false;
                return;
            }

            var screenPosition = PointerPosition();
            var ray = camera.ScreenPointToRay(screenPosition);
            if (Physics.Raycast(ray, out var hit, fallbackDistance, fallbackLayers, QueryTriggerInteraction.Ignore))
            {
                candidateValid = true;
                candidateConfidence = 0.75f;
                candidatePose = new Pose(
                    hit.point + hit.normal * surfaceOffset,
                    RotationFromNormal(hit.normal));
            }
            else
            {
                candidateValid = false;
            }
        }

        private Quaternion RotationFromNormal(Vector3 normal)
        {
            var safeNormal = normal.sqrMagnitude > 0.0001f ? normal.normalized : Vector3.up;
            var camera = Camera.main;
            var forward = camera != null
                ? Vector3.ProjectOnPlane(camera.transform.forward, safeNormal)
                : Vector3.ProjectOnPlane(Vector3.forward, safeNormal);

            if (forward.sqrMagnitude < 0.0001f)
                forward = Vector3.ProjectOnPlane(Vector3.right, safeNormal);

            return Quaternion.LookRotation(forward.normalized, safeNormal);
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

        private void RefreshVisibility()
        {
            if (reticle != null)
                reticle.gameObject.SetActive(candidateValid);
            if (placedContent != null)
                placedContent.gameObject.SetActive(hasPlacedObject);
        }
    }
}
