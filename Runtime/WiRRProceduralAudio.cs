using UnityEngine;

namespace KIA.WiRR
{
    public enum WiRRProceduralAudioProfile
    {
        Hum,
        Beacon,
        Engine,
        Wind
    }

    /// <summary>
    /// Generuje prosty zapętlony sygnał audio bez zewnętrznych plików dźwiękowych.
    /// Nadaje się do demonstracji częstotliwości, modulacji, spatial blend i powiązania
    /// parametrów audio ze stanem obiektu.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    [DisallowMultipleComponent]
    public sealed class WiRRProceduralAudio : MonoBehaviour
    {
        [InspectorName("Profil")]
        [SerializeField] private WiRRProceduralAudioProfile profile = WiRRProceduralAudioProfile.Hum;

        [Range(20f, 1000f)]
        [InspectorName("Częstotliwość bazowa [Hz]")]
        [SerializeField] private float baseFrequency = 90f;

        [Range(0f, 1f)]
        [InspectorName("Głośność")]
        [SerializeField] private float volume = 0.18f;

        [Range(0f, 1f)]
        [InspectorName("Dźwięk przestrzenny")]
        [SerializeField] private float spatialBlend = 1f;

        [Range(0f, 1f)]
        [InspectorName("Głębokość modulacji")]
        [SerializeField] private float modulationDepth = 0.15f;

        [Min(8000)]
        [InspectorName("Częstotliwość próbkowania")]
        [SerializeField] private int sampleRate = 22050;

        private AudioSource source;
        private AudioClip generatedClip;
        private float normalizedLoad;

        public WiRRProceduralAudioProfile Profile => profile;
        public float NormalizedLoad => normalizedLoad;

        private void Awake()
        {
            EnsureSource();
            BuildClip();
        }

        private void OnEnable()
        {
            EnsureSource();
            if (Application.isPlaying && source.clip != null && !source.isPlaying)
                source.Play();
        }

        private void OnDisable()
        {
            if (source != null && source.isPlaying)
                source.Stop();
        }

        private void OnDestroy()
        {
            if (generatedClip != null)
                Destroy(generatedClip);
        }

        public void Configure(
            WiRRProceduralAudioProfile newProfile,
            float frequency,
            float newVolume,
            float newSpatialBlend = 1f)
        {
            profile = newProfile;
            baseFrequency = Mathf.Clamp(frequency, 20f, 1000f);
            volume = Mathf.Clamp01(newVolume);
            spatialBlend = Mathf.Clamp01(newSpatialBlend);

            if (Application.isPlaying)
            {
                EnsureSource();
                BuildClip();
            }
        }

        public void SetNormalizedLoad(float value)
        {
            normalizedLoad = Mathf.Clamp01(value);
            if (source == null)
                return;

            if (profile == WiRRProceduralAudioProfile.Engine)
            {
                source.pitch = Mathf.Lerp(0.70f, 1.65f, normalizedLoad);
                source.volume = volume * Mathf.Lerp(0.55f, 1f, normalizedLoad);
            }
            else if (profile == WiRRProceduralAudioProfile.Wind)
            {
                source.pitch = Mathf.Lerp(0.85f, 1.20f, normalizedLoad);
                source.volume = volume * Mathf.Lerp(0.30f, 1f, normalizedLoad);
            }
        }

        private void EnsureSource()
        {
            if (source == null)
                source = GetComponent<AudioSource>();

            source.loop = true;
            source.playOnAwake = true;
            source.spatialBlend = spatialBlend;
            source.volume = volume;
            source.dopplerLevel = 0.2f;
            source.rolloffMode = AudioRolloffMode.Logarithmic;
            source.minDistance = 0.5f;
            source.maxDistance = 12f;
        }

        private void BuildClip()
        {
            if (generatedClip != null)
            {
                if (source != null && source.clip == generatedClip)
                    source.clip = null;
                Destroy(generatedClip);
            }

            var duration = profile == WiRRProceduralAudioProfile.Beacon ? 2f : 1f;
            var samples = Mathf.Max(256, Mathf.RoundToInt(sampleRate * duration));
            var data = new float[samples];

            var noiseState = 0x12345678u;
            var filteredNoise = 0f;

            for (var i = 0; i < samples; i++)
            {
                var t = i / (float)sampleRate;
                var phase = t * baseFrequency * Mathf.PI * 2f;
                var sample = 0f;

                switch (profile)
                {
                    case WiRRProceduralAudioProfile.Hum:
                        sample =
                            Mathf.Sin(phase) * 0.72f +
                            Mathf.Sin(phase * 2f) * 0.18f +
                            Mathf.Sin(phase * 0.5f) * modulationDepth * 0.10f;
                        break;

                    case WiRRProceduralAudioProfile.Beacon:
                    {
                        var cycle = Mathf.Repeat(t, 1f);
                        var gate = cycle < 0.16f || (cycle > 0.28f && cycle < 0.38f) ? 1f : 0f;
                        sample =
                            (Mathf.Sin(phase * 4f) * 0.75f +
                             Mathf.Sin(phase * 8f) * 0.18f) * gate;
                        break;
                    }

                    case WiRRProceduralAudioProfile.Engine:
                        sample =
                            Mathf.Sin(phase) * 0.60f +
                            Mathf.Sin(phase * 2f) * 0.22f +
                            Mathf.Sin(phase * 3f) * 0.10f;
                        break;

                    case WiRRProceduralAudioProfile.Wind:
                        noiseState = noiseState * 1664525u + 1013904223u;
                        var white = ((noiseState >> 8) / 16777215f) * 2f - 1f;
                        filteredNoise = Mathf.Lerp(filteredNoise, white, 0.035f);
                        sample = filteredNoise * 0.85f;
                        break;
                }

                var tremolo = 1f - modulationDepth * 0.25f +
                              Mathf.Sin(t * Mathf.PI * 2f * 0.7f) * modulationDepth * 0.25f;
                data[i] = Mathf.Clamp(sample * tremolo * 0.6f, -1f, 1f);
            }

            generatedClip = AudioClip.Create(
                $"WiRR_{profile}_{baseFrequency:0}Hz",
                samples,
                1,
                sampleRate,
                false);
            generatedClip.SetData(data, 0);

            source.clip = generatedClip;
            source.volume = volume;
            source.spatialBlend = spatialBlend;

            if (isActiveAndEnabled && Application.isPlaying)
                source.Play();
        }
    }
}
