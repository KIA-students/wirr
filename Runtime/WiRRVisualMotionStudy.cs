using UnityEngine;
using UnityEngine.InputSystem;

namespace KIA.WiRR
{
    /// <summary>
    /// Safe, voluntary visual-reference demonstration. It NEVER moves the XR
    /// camera, changes IPD or runs without explicit start. This is not designed
    /// to induce nausea. Use with a seated participant and observer.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class WiRRVisualMotionStudy : MonoBehaviour
    {
        [SerializeField] private Transform movingReference;
        [SerializeField, Range(0.02f, 0.16f)] private float movementMeters = 0.12f;
        [SerializeField, Range(4f, 8f)] private float durationSeconds = 8f;
        [SerializeField] private bool showDesktopControls = true;

        private Vector3 referencePosition;
        private float started;
        private bool running;
        private bool completed;
        public bool IsRunning => running;

        public void Configure(Transform panel)
        {
            movingReference = panel;
            if (panel != null) referencePosition = panel.localPosition;
        }

        private void Awake()
        {
            if (movingReference != null) referencePosition = movingReference.localPosition;
        }

        [ContextMenu("Rozpocznij jedną krótką demonstrację")]
        public void Begin()
        {
            if (!isActiveAndEnabled || movingReference == null || running || completed) return;
            referencePosition = movingReference.localPosition;
            started = Time.unscaledTime;
            running = true;
            Debug.Log("[WiRR][Comfort] Rozpoczęto dobrowolną, łagodną prezentację ruchu otoczenia. Kamera pozostaje nieruchoma.");
        }

        [ContextMenu("Przerwij demonstrację")]
        public void StopStudy()
        {
            if (movingReference != null) movingReference.localPosition = referencePosition;
            running = false;
            completed = true;
            Debug.Log("[WiRR][Comfort] Zakończono ekspozycję. Każda oznaka dyskomfortu wymaga natychmiastowego przerwania.");
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

            if (!running || movingReference == null) return;
            var elapsed = Time.unscaledTime - started;
            if (elapsed >= durationSeconds)
            {
                StopStudy();
                return;
            }
            var phase = Mathf.Clamp01(elapsed / durationSeconds);
            // One slow, reversible translation of SCENE REFERENCE only.
            var offset = Mathf.Sin(phase * Mathf.PI) * movementMeters;
            movingReference.localPosition = referencePosition + Vector3.right * offset;
        }

        private void OnDisable()
        {
            if (movingReference != null) movingReference.localPosition = referencePosition;
            running = false;
        }

        private void OnGUI()
        {
            if (!showDesktopControls) return;
            GUILayout.BeginArea(new Rect(18, 18, 415, 130), GUI.skin.box);
            GUILayout.Label("WiRR: ruch punktów odniesienia (nie ruch kamery)");
            GUILayout.Label("Nie uruchamia się automatycznie. Maksymalnie 8 s. Przerwij przy dyskomforcie.");
            if (!running && !completed && GUILayout.Button("Rozpocznij łagodny pokaz"))
                Begin();
            if (running && GUILayout.Button("PRZERWIJ NATYCHMIAST"))
                StopStudy();
            if (completed) GUILayout.Label("Zakończono. Odpocznij. Nie powtarzaj dla wyniku.");
            GUILayout.EndArea();
        }

        private void OnValidate()
        {
            movementMeters = Mathf.Clamp(movementMeters, 0.02f, 0.16f);
            durationSeconds = Mathf.Clamp(durationSeconds, 4f, 8f);
        }
    }
}
