using UnityEngine;

namespace KIA.WiRR
{
    /// <summary>
    /// Applies a normalized mobile-AR light estimate to a virtual light and selected
    /// renderers. AR Foundation light-estimation values can be mapped by an adapter
    /// to intensity 0..1, RGB light color and optional main-light direction.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class WiRRMobileARLightMatch : MonoBehaviour
    {
        [InspectorName("Wirtualne światło")]
        [SerializeField] private Light virtualLight;

        [InspectorName("Renderery obiektu")]
        [SerializeField] private Renderer[] targetRenderers = System.Array.Empty<Renderer>();

        [Min(0f)]
        [InspectorName("Minimalna intensywność światła")]
        [SerializeField] private float minLightIntensity = 0.25f;

        [Min(0.01f)]
        [InspectorName("Maksymalna intensywność światła")]
        [SerializeField] private float maxLightIntensity = 2.0f;

        [Min(0.1f)]
        [InspectorName("Wygładzanie")]
        [SerializeField] private float smoothing = 5f;

        [Min(0.1f)]
        [InspectorName("Timeout estymacji [s]")]
        [SerializeField] private float estimateTimeout = 0.75f;

        [InspectorName("Symuluj zmianę światła bez providera")]
        [SerializeField] private bool simulateWhenStale = true;

        private MaterialPropertyBlock propertyBlock;
        private Color[] baseColors = System.Array.Empty<Color>();
        private float targetIntensity01 = 0.65f;
        private Color targetColor = Color.white;
        private Vector3 targetDirection = new Vector3(0.3f, -1f, 0.25f);
        private float confidence = 1f;
        private float lastEstimate = -999f;
        private float currentIntensity01 = 0.65f;
        private Color currentColor = Color.white;

        public float Intensity01 => currentIntensity01;
        public float Confidence => confidence;

        public void Configure(Light lightSource, Renderer[] renderers)
        {
            virtualLight = lightSource;
            targetRenderers = renderers ?? System.Array.Empty<Renderer>();
            CaptureBaseColors();
        }

        public void SetLightEstimate(
            float normalizedIntensity,
            Color lightColor,
            Vector3 mainLightDirection,
            float estimateConfidence = 1f)
        {
            targetIntensity01 = Mathf.Clamp01(normalizedIntensity);
            targetColor = lightColor;
            if (mainLightDirection.sqrMagnitude > 0.0001f)
                targetDirection = mainLightDirection.normalized;
            confidence = Mathf.Clamp01(estimateConfidence);
            lastEstimate = Time.unscaledTime;
        }

        private void Awake()
        {
            propertyBlock = new MaterialPropertyBlock();
            CaptureBaseColors();
        }

        private void Update()
        {
            var stale = Time.unscaledTime - lastEstimate > estimateTimeout;
            if (stale && simulateWhenStale && Application.isEditor)
            {
                var cycle = Mathf.Sin(Time.time * 0.45f) * 0.5f + 0.5f;
                targetIntensity01 = Mathf.Lerp(0.22f, 0.92f, cycle);
                targetColor = Color.Lerp(new Color(1f, 0.78f, 0.58f), new Color(0.76f, 0.88f, 1f), cycle);
                targetDirection = Quaternion.Euler(0f, Time.time * 7f, 0f) * new Vector3(0.35f, -1f, 0.2f);
                confidence = 0.72f;
            }

            var t = 1f - Mathf.Exp(-smoothing * Time.deltaTime);
            currentIntensity01 = Mathf.Lerp(currentIntensity01, targetIntensity01, t);
            currentColor = Color.Lerp(currentColor, targetColor, t);

            ApplyLight(t);
            ApplyRendererTint();
        }

        private void ApplyLight(float t)
        {
            if (virtualLight == null)
                return;

            virtualLight.intensity = Mathf.Lerp(minLightIntensity, maxLightIntensity, currentIntensity01);
            virtualLight.color = currentColor;

            if (targetDirection.sqrMagnitude > 0.0001f)
            {
                var forward = -targetDirection.normalized;
                var up = Mathf.Abs(Vector3.Dot(forward, Vector3.up)) > 0.98f
                    ? Vector3.forward
                    : Vector3.up;
                var rotation = Quaternion.LookRotation(forward, up);
                virtualLight.transform.rotation = Quaternion.Slerp(virtualLight.transform.rotation, rotation, t);
            }
        }

        private void ApplyRendererTint()
        {
            if (propertyBlock == null)
                propertyBlock = new MaterialPropertyBlock();

            for (var i = 0; i < targetRenderers.Length; i++)
            {
                var renderer = targetRenderers[i];
                if (renderer == null)
                    continue;

                var baseColor = i < baseColors.Length ? baseColors[i] : Color.white;
                var intensityTint = Mathf.Lerp(0.72f, 1.08f, currentIntensity01);
                var tint = Multiply(baseColor, currentColor) * intensityTint;
                tint.a = baseColor.a;

                renderer.GetPropertyBlock(propertyBlock);
                if (renderer.sharedMaterial != null && renderer.sharedMaterial.HasProperty("_BaseColor"))
                    propertyBlock.SetColor("_BaseColor", tint);
                else if (renderer.sharedMaterial != null && renderer.sharedMaterial.HasProperty("_Color"))
                    propertyBlock.SetColor("_Color", tint);
                renderer.SetPropertyBlock(propertyBlock);
            }
        }

        private void CaptureBaseColors()
        {
            baseColors = new Color[targetRenderers?.Length ?? 0];
            for (var i = 0; i < baseColors.Length; i++)
            {
                var renderer = targetRenderers[i];
                var material = renderer != null ? renderer.sharedMaterial : null;
                if (material != null && material.HasProperty("_BaseColor"))
                    baseColors[i] = material.GetColor("_BaseColor");
                else if (material != null && material.HasProperty("_Color"))
                    baseColors[i] = material.GetColor("_Color");
                else
                    baseColors[i] = Color.white;
            }
        }

        private static Color Multiply(Color a, Color b)
        {
            return new Color(a.r * b.r, a.g * b.g, a.b * b.b, a.a * b.a);
        }
    }
}
