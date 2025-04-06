using UnityEngine;

public class LineDrawer : MonoBehaviour {
    public Transform player; // First object
    public Transform anchor; // Second object
    private LineRenderer lineRenderer;
    public float width;
    Color lineColor = new Color(0, 0, 0);
    private float stress = 0f;
    public float maxStretch = 0;
    private bool Pendulum = true;

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

            if (Pendulum) { lineColor = Color.HSVToRGB(1, 1, stress); }
            else { lineColor = Color.green; }
            lineRenderer.startColor = lineColor;
            lineRenderer.endColor = lineColor;
        }
    }

    public void SetAnchor(Transform anchorTransform) {
        anchor = anchorTransform;
    }
    public void SetStress(float displacement) {
        if (displacement > maxStretch) { maxStretch = displacement; }
        stress = Mathf.Min(1, Mathf.Abs(displacement) / maxStretch);
    }

    public void SetToFreefall() {
        Pendulum = false;
    }

    public void SetToPendulum() {
        Pendulum = true;
    }
}
