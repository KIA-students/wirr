using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace KIA.WiRR
{
    /// <summary>
    /// Lekki pomiar mediany czasu klatki, FPS i pamięci.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class WiRRFrameMetrics : MonoBehaviour
    {
        [Min(30)]
        [SerializeField] private int sampleFrames = 180;

        [SerializeField] private bool showOverlay = true;

        [Min(0.1f)]
        [SerializeField] private float refreshSeconds = 0.5f;

        private readonly Queue<float> frameTimesMs = new Queue<float>();
        private float nextRefresh;
        private float medianFrameMs;
        private float medianFps;
        private float allocatedMemoryMb;

        public float MedianFrameMs => medianFrameMs;
        public float MedianFps => medianFps;
        public float AllocatedMemoryMb => allocatedMemoryMb;

        private void Update()
        {
            var dt = Time.unscaledDeltaTime;
            if (dt > 0f)
            {
                frameTimesMs.Enqueue(dt * 1000f);
                while (frameTimesMs.Count > sampleFrames)
                    frameTimesMs.Dequeue();
            }

            if (Time.unscaledTime < nextRefresh)
                return;

            nextRefresh = Time.unscaledTime + refreshSeconds;
            Recalculate();
        }

        private void Recalculate()
        {
            if (frameTimesMs.Count == 0)
                return;

            var values = frameTimesMs.ToArray();
            Array.Sort(values);
            var middle = values.Length / 2;
            medianFrameMs = values.Length % 2 == 0
                ? (values[middle - 1] + values[middle]) * 0.5f
                : values[middle];

            medianFps = medianFrameMs > 0.0001f ? 1000f / medianFrameMs : 0f;
            allocatedMemoryMb = Profiler.GetTotalAllocatedMemoryLong() / (1024f * 1024f);
        }

        private void OnGUI()
        {
            if (!showOverlay)
                return;

            GUI.Box(
                new Rect(12, 12, 300, 78),
                $"Metryki WiRR\nMediana czasu klatki: {medianFrameMs:F2} ms  ({medianFps:F1} FPS)\nPrzydzielona pamięć: {allocatedMemoryMb:F1} MB");
        }
    }
}
