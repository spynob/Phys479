using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour {

    InputSubscription GetInput;
    Vector2 playerMovement;
    Rigidbody rb;
    bool grounded = false;
    private Vector3 Velocity;
    private Vector3 Acceleration;
    public float mass = 100;
    public float gravity = 9.81f;

    private float theta; // Polar angle
    private float phi; // Azimuthal angle
    private float thetaDot = 0;
    private float phiDot = 0.1f;
    private float thetaDdot = 0;
    private float phiDdot = 0;
    private float length = 1;
    private float damping = 0f;
    private float epsilon = 0.05f;

    public GameObject Anchor;

    private void Awake() {
        GetInput = GetComponent<InputSubscription>();
        rb = GetComponent<Rigidbody>();
        Grapple();

    }

    private void Update() {
        if (grounded) {
            playerMovement = new Vector2(GetInput.MoveInput.x, GetInput.MoveInput.y);
        }
        if (!grounded) {
            GetAnchor();
            UpdateSwing();
        }
    }

    private void GetAnchor() { }

    private void Grapple() {
        Vector3 relativePos = transform.position - Anchor.transform.position;
        length = relativePos.magnitude;
        if (length < epsilon) { theta = 0; phi = 0; return; }
        theta = Mathf.Acos(relativePos.y / length);
        phi = Mathf.Atan2(relativePos.z, relativePos.x);
    }



    private void UpdateSwing() {
        if (length < epsilon) {
            thetaDdot = 0;
        }
        else {
            thetaDdot = -gravity / length * Mathf.Sin(theta) + Mathf.Sin(theta) * Mathf.Cos(theta) * phiDot * phiDot - (damping * thetaDot);
        }
        if (Mathf.Abs(theta) < epsilon || 180 - Mathf.Abs(theta) < epsilon) { phiDdot = 0; }
        else { phiDdot = -(2 * thetaDot * phiDot) / Mathf.Tan(theta) - (damping * phiDot); }

        float dt = Time.deltaTime;
        thetaDot += thetaDdot * dt;
        phiDot += phiDdot * dt;
        theta += thetaDot * dt;
        phi += phiDot * dt;

        Debug.Log("\ntheta: " + theta + ", ThetaDot: " + thetaDot + ", ThetaDotDot" + thetaDdot + "\tPhi: " + phi + ", PhiDot: " + phiDot + ", PhiDotDot: " + phiDdot);

        transform.position = Anchor.transform.position + SphericalToCartesian(theta, phi, length);
    }

    Vector3 SphericalToCartesian(float t, float p, float r) {
        float x = r * Mathf.Sin(t) * Mathf.Cos(p);
        float y = -r * Mathf.Cos(t);
        float z = r * Mathf.Sin(t) * Mathf.Sin(p);
        return new Vector3(x, y, z);
    }

    private Vector3 CartesianToSpherical(float xBob, float yBob, float zBob, float xPivot, float yPivot, float zPivot) {
        Vector3 relativePos = new Vector3(xBob - xPivot, yBob - yPivot, zBob - zPivot);
        float r = relativePos.magnitude;
        if (r < epsilon) { return new Vector3(0, 0, 0); }
        float t = Mathf.Acos(-relativePos.y / r);
        float p = Mathf.Atan2(relativePos.z, relativePos.x);
        return new Vector3(t, p, r);
    }
}

