using UnityEngine;

namespace KIA.WiRR
{
    /// <summary>
    /// Virtual aura over a tracked real hand. A provider can send wrist and five
    /// fingertip positions. If no provider is connected, a procedural hand near the
    /// main camera demonstrates the behaviour.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class WiRRHandAura : MonoBehaviour
    {
        [InspectorName("Marker nadgarstka")]
        [SerializeField] private Transform wristMarker;

        [InspectorName("Markery opuszków")]
        [SerializeField] private Transform[] fingertipMarkers = System.Array.Empty<Transform>();

        [InspectorName("Linie palców")]
        [SerializeField] private LineRenderer[] fingerLines = System.Array.Empty<LineRenderer>();

        [InspectorName("Rdzeń pinch")]
        [SerializeField] private Transform pinchCore;

        [Min(0.01f)]
        [InspectorName("Próg pinch [m]")]
        [SerializeField] private float pinchThreshold = 0.035f;

        [Min(0.1f)]
        [InspectorName("Wygładzanie")]
        [SerializeField] private float smoothing = 18f;

        [Min(0.1f)]
        [InspectorName("Timeout trackingu [s]")]
        [SerializeField] private float trackingTimeout = 0.6f;

        [InspectorName("Symuluj bez hand trackingu")]
        [SerializeField] private bool simulateWhenStale = true;

        private readonly Vector3[] targetTips = new Vector3[5];
        private Vector3 targetWrist;
        private Quaternion targetWristRotation = Quaternion.identity;
        private float lastTrackingUpdate = -999f;
        private bool externallyTracked;
        private Transform viewer;
        private int cameraLookupFrame;
        private float pinch01;

        public float Pinch01 => pinch01;
        public bool IsExternallyTracked => externallyTracked && Time.unscaledTime - lastTrackingUpdate <= trackingTimeout;

        public void Configure(
            Transform wrist,
            Transform[] fingertips,
            LineRenderer[] lines,
            Transform pinchVisual)
        {
            wristMarker = wrist;
            fingertipMarkers = fingertips ?? System.Array.Empty<Transform>();
            fingerLines = lines ?? System.Array.Empty<LineRenderer>();
            pinchCore = pinchVisual;
        }

        public void SetHandPose(
            Vector3 wristWorld,
            Quaternion wristRotation,
            Vector3 thumbTip,
            Vector3 indexTip,
            Vector3 middleTip,
            Vector3 ringTip,
            Vector3 littleTip,
            bool tracked = true)
        {
            targetWrist = wristWorld;
            targetWristRotation = wristRotation;
            targetTips[0] = thumbTip;
            targetTips[1] = indexTip;
            targetTips[2] = middleTip;
            targetTips[3] = ringTip;
            targetTips[4] = littleTip;
            externallyTracked = tracked;
            lastTrackingUpdate = Time.unscaledTime;
        }

        public void SetTracked(bool tracked)
        {
            externallyTracked = tracked;
            lastTrackingUpdate = Time.unscaledTime;
        }

        private void Awake()
        {
            ResolveViewer(true);
        }

        private void Update()
        {
            ResolveViewer(false);

            if (!IsExternallyTracked && simulateWhenStale)
                SimulateHand();
            else if (!IsExternallyTracked)
                SetVisualsActive(false);

            if (IsExternallyTracked || simulateWhenStale)
            {
                SetVisualsActive(true);
                ApplyPose();
            }
        }

        private void SimulateHand()
        {
            if (viewer == null)
                return;

            var right = viewer.right;
            var up = viewer.up;
            var forward = viewer.forward;
            targetWrist = viewer.position + forward * 0.62f + right * 0.24f - up * 0.19f;
            targetWristRotation = Quaternion.LookRotation(forward, up);

            var spread = new[] { -0.15f, -0.075f, 0f, 0.075f, 0.145f };
            var lengths = new[] { 0.14f, 0.22f, 0.235f, 0.215f, 0.185f };
            var wave = Mathf.Sin(Time.time * 1.7f) * 0.012f;
            for (var i = 0; i < 5; i++)
            {
                targetTips[i] =
                    targetWrist +
                    right * spread[i] +
                    up * (0.035f + lengths[i]) +
                    forward * (0.035f + wave * (i + 1));
            }

            if (Mathf.Repeat(Time.time, 4f) > 2.7f)
                targetTips[0] = Vector3.Lerp(targetTips[0], targetTips[1], 0.86f);
        }

        private void ApplyPose()
        {
            var t = 1f - Mathf.Exp(-smoothing * Time.deltaTime);

            if (wristMarker != null)
            {
                wristMarker.position = Vector3.Lerp(wristMarker.position, targetWrist, t);
                wristMarker.rotation = Quaternion.Slerp(wristMarker.rotation, targetWristRotation, t);
            }

            for (var i = 0; i < fingertipMarkers.Length && i < targetTips.Length; i++)
            {
                if (fingertipMarkers[i] != null)
                    fingertipMarkers[i].position = Vector3.Lerp(fingertipMarkers[i].position, targetTips[i], t);
            }

            for (var i = 0; i < fingerLines.Length && i < targetTips.Length; i++)
            {
                var line = fingerLines[i];
                if (line == null)
                    continue;
                line.positionCount = 2;
                line.SetPosition(0, wristMarker != null ? wristMarker.position : targetWrist);
                line.SetPosition(1, fingertipMarkers.Length > i && fingertipMarkers[i] != null
                    ? fingertipMarkers[i].position
                    : targetTips[i]);
            }

            var thumb = fingertipMarkers.Length > 0 && fingertipMarkers[0] != null
                ? fingertipMarkers[0].position
                : targetTips[0];
            var index = fingertipMarkers.Length > 1 && fingertipMarkers[1] != null
                ? fingertipMarkers[1].position
                : targetTips[1];

            var pinchDistance = Vector3.Distance(thumb, index);
            pinch01 = 1f - Mathf.InverseLerp(pinchThreshold, pinchThreshold * 2.5f, pinchDistance);
            pinch01 = Mathf.Clamp01(pinch01);

            if (pinchCore != null)
            {
                pinchCore.position = Vector3.Lerp(thumb, index, 0.5f);
                var scale = Mathf.Lerp(0.035f, 0.11f, pinch01);
                pinchCore.localScale = Vector3.one * scale;
            }
        }

        private void SetVisualsActive(bool active)
        {
            if (wristMarker != null)
                wristMarker.gameObject.SetActive(active);
            foreach (var marker in fingertipMarkers)
                if (marker != null)
                    marker.gameObject.SetActive(active);
            foreach (var line in fingerLines)
                if (line != null)
                    line.enabled = active;
            if (pinchCore != null)
                pinchCore.gameObject.SetActive(active);
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
