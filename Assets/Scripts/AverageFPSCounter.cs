using UnityEngine;

public class AverageFPSCounter : MonoBehaviour {
    public int sampleSize = 100; // Number of frames to average over
    private float[] frameTimes;
    private int frameIndex = 0;
    private bool filled = false;

    void Start() {
        frameTimes = new float[sampleSize];
    }

    void Update() {
        float deltaTime = Time.unscaledDeltaTime;
        frameTimes[frameIndex] = deltaTime;
        frameIndex = (frameIndex + 1) % sampleSize;
        if (frameIndex == 0) filled = true;

        float averageDeltaTime = 0f;
        int count = filled ? sampleSize : frameIndex;
        for (int i = 0; i < count; i++) {
            averageDeltaTime += frameTimes[i];
        }
        averageDeltaTime /= count;

        float avgFPS = 1f / averageDeltaTime;
        Debug.Log($"Avg FPS: {avgFPS:F2}");
    }
}