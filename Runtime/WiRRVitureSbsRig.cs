using UnityEngine;

namespace KIA.WiRR
{
    /// <summary>
    /// Prosty rig stereoskopowy Full SBS dla okularów VITURE XR.
    /// Lewa kamera renderuje lewą połowę ekranu, prawa prawą połowę.
    /// Dla natywnego trybu 3D VITURE docelowy framebuffer powinien mieć 3840x1080.
    /// </summary>
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public sealed class WiRRVitureSbsRig : MonoBehaviour
    {
        [SerializeField] private Camera leftEye;
        [SerializeField] private Camera rightEye;

        [Range(0.050f, 0.075f)]
        [Tooltip("Rozstaw kamer w metrach. 0.064 m jest bezpiecznym punktem startowym do demonstracji stereoskopii.")]
        [SerializeField] private float ipdMeters = 0.064f;

        [Tooltip("Jeżeli włączone, rig wymusza prostą geometrię parallel-axis bez toe-in.")]
        [SerializeField] private bool enforceParallelAxes = true;

        public Camera LeftEye => leftEye;
        public Camera RightEye => rightEye;
        public float IpdMeters => ipdMeters;

        public void Configure(Camera left, Camera right, float ipd = 0.064f)
        {
            leftEye = left;
            rightEye = right;
            ipdMeters = Mathf.Clamp(ipd, 0.050f, 0.075f);
            Apply();
        }

        public void Apply()
        {
            if (leftEye == null || rightEye == null)
                return;

            leftEye.rect = new Rect(0f, 0f, 0.5f, 1f);
            rightEye.rect = new Rect(0.5f, 0f, 0.5f, 1f);

            var half = ipdMeters * 0.5f;
            leftEye.transform.localPosition = new Vector3(-half, 0f, 0f);
            rightEye.transform.localPosition = new Vector3(half, 0f, 0f);

            if (enforceParallelAxes)
            {
                leftEye.transform.localRotation = Quaternion.identity;
                rightEye.transform.localRotation = Quaternion.identity;
            }

            leftEye.stereoTargetEye = StereoTargetEyeMask.None;
            rightEye.stereoTargetEye = StereoTargetEyeMask.None;
        }

        private void Awake() => Apply();
        private void OnValidate() => Apply();
    }
}
