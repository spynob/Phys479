using UnityEngine;

public class AverageFPSLogger : MonoBehaviour {
    public float loggingInterval = 5f;

    private int frameCount = 0;
    private float timeElapsed = 0f;

    void Update() {
        frameCount++;
        timeElapsed += Time.unscaledDeltaTime;

        if (timeElapsed >= loggingInterval) {
            float averageFPS = frameCount / timeElapsed;
            Debug.Log($"[AverageFPSLogger] Average FPS over last {loggingInterval} seconds: {averageFPS:F2}");
            frameCount = 0;
            timeElapsed = 0f;
        }
    }
}
