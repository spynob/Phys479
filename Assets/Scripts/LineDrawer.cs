using UnityEngine;

public class LineDrawer : MonoBehaviour {
    public Transform player; // First object
    public Transform anchor; // Second object
    private LineRenderer lineRenderer;
    public float width;
    Color lineColor = new Color(255, 255, 255);
    private float stress = 0f;
    public float maxStretch = 0;

    void Start() {
        lineRenderer = gameObject.AddComponent<LineRenderer>();

        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 2;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
    }

    void Update() {
        if (player != null && anchor != null) {
            lineRenderer.SetPosition(0, player.position);
            lineRenderer.SetPosition(1, anchor.position);

            lineColor = Color.HSVToRGB(0, stress, 1);
            lineRenderer.startColor = lineColor;
            lineRenderer.endColor = lineColor;
        }
    }

    public void setAnchor(Transform anchorTransform) {
        anchor = anchorTransform;
    }
    public void setStress(float displacement) {
        if (displacement > maxStretch) { maxStretch = displacement; }
        stress = Mathf.Min(1, Mathf.Abs(displacement) / maxStretch);
    }
}
