using UnityEngine;

public class PerformanceBenchmark : MonoBehaviour
{
    [SerializeField] private int sampleFrames = 300;

    private float accumulatedTime;
    private int frameCount;

    private void Update()
    {
        accumulatedTime += Time.unscaledDeltaTime;
        frameCount++;

        if (frameCount < sampleFrames)
            return;

        float averageFrameTime = accumulatedTime / frameCount;
        float fps = 1f / averageFrameTime;
        float frameTimeMs = averageFrameTime * 1000f;

        Debug.Log(
            $"LAB01_BENCHMARK | FPS={fps:F1} | FrameTime={frameTimeMs:F2} ms"
        );

        accumulatedTime = 0f;
        frameCount = 0;
    }
}
