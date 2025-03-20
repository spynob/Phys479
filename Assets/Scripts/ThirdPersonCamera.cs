using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour {
    public Transform target;
    public Vector3 offset = new Vector3(0, 2, -4);
    public float rotationSpeed = 3.0f;
    public float smoothSpeed = 5.0f;

    // Zoom variables
    public float minZoom = 2f;
    public float maxZoom = 10f;
    public float zoomSpeed = 5f;
    private float currentZoom;

    private float yaw = 0f;
    private float pitch = 0f;

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentZoom = offset.magnitude;
    }

    void LateUpdate() {
        if (target == null) return;

        yaw += Input.GetAxis("Mouse X") * rotationSpeed;
        pitch -= Input.GetAxis("Mouse Y") * rotationSpeed;
        pitch = Mathf.Clamp(pitch, -30f, 60f);

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        currentZoom -= scroll * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 zoomedOffset = offset.normalized * currentZoom;
        Vector3 desiredPosition = target.position + rotation * zoomedOffset;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.LookAt(target.position);
    }
}
