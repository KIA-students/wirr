using UnityEngine;
using UnityEngine.InputSystem;

namespace KIA.WiRR
{
    /// <summary>
    /// One short, comfortable near/far stereoscopy comparison.
    /// Moves an object only: does NOT modify XR camera, IPD, focus or projection.
    /// It cannot measure accommodation and should not be used to provoke symptoms.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class WiRRDepthCueStudy : MonoBehaviour
    {
        [SerializeField] private Transform observationTarget;
        [SerializeField, Range(2.0f, 2.8f)] private float nearDistanceMeters = 2.4f;
        [SerializeField, Range(3.0f, 4.0f)] private float farDistanceMeters = 3.3f;
        [SerializeField, Range(6f, 10f)] private float durationSeconds = 10f;
        [SerializeField] private bool showDesktopControls = true;

        private Vector3 originalPosition;
        private float started;
        private bool active;
        private bool completed;

        public void Configure(Transform target)
        {
            observationTarget = target;
            if (target != null) originalPosition = target.localPosition;
        }

        private void Awake()
        {
            if (observationTarget != null) originalPosition = observationTarget.localPosition;
        }

        public void Begin()
        {
            if (!isActiveAndEnabled || observationTarget == null || active || completed) return;
            originalPosition = observationTarget.localPosition;
            started = Time.unscaledTime;
            active = true;
            Debug.Log("[WiRR][Comfort] Łagodna zmiana wirtualnej odległości. To nie jest pomiar akomodacji.");
        }

        public void StopStudy()
        {
            if (observationTarget != null) observationTarget.localPosition = originalPosition;
            active = false;
            completed = true;
        }

        private void Update()
        {
            if ((Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame) ||
                (Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame))
            {
                StopStudy();
                return;
            }
            if ((Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame) ||
                (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame))
                Begin();

            if (!active || observationTarget == null) return;
            var elapsed = Time.unscaledTime - started;
            if (elapsed >= durationSeconds)
            {
                StopStudy();
                return;
            }
            // Gradual near → far → near, bounded for comfortable viewing.
            var t = Mathf.PingPong(elapsed / (durationSeconds * 0.5f), 1f);
            var smooth = t * t * (3f - 2f * t);
            var position = originalPosition;
            position.z = Mathf.Lerp(nearDistanceMeters, farDistanceMeters, smooth);
            observationTarget.localPosition = position;
        }

        private void OnDisable()
        {
            if (observationTarget != null) observationTarget.localPosition = originalPosition;
            active = false;
        }

        private void OnGUI()
        {
            if (!showDesktopControls) return;
            GUILayout.BeginArea(new Rect(18, 160, 440, 135), GUI.skin.box);
            GUILayout.Label("WiRR: wergencja / akomodacja: porównanie wskazówek głębi");
            GUILayout.Label("Kamera i optyczna ogniskowa gogli nie są zmieniane.");
            if (!active && !completed && GUILayout.Button("Porównaj głębię (maks. 10 s)"))
                Begin();
            if (active && GUILayout.Button("PRZERWIJ NATYCHMIAST"))
                StopStudy();
            if (completed) GUILayout.Label("Koniec. Oceń czytelność, nie odporność na zmęczenie.");
            GUILayout.EndArea();
        }

        private void OnValidate()
        {
            nearDistanceMeters = Mathf.Clamp(nearDistanceMeters, 2.0f, 2.8f);
            farDistanceMeters = Mathf.Clamp(farDistanceMeters, 3.0f, 4.0f);
            durationSeconds = Mathf.Clamp(durationSeconds, 6f, 10f);
        }
    }
}
