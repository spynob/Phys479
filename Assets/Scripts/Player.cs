using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour {

    // Input Movement
    InputSubscription GetInput;
    Vector2 playerMovement;
    Rigidbody rb;
    bool grounded = false;
    private Vector3 Velocity;
    private Vector3 Acceleration;

    // Properties
    public float mass = 100;
    public float gravity = 9.81f;
    public float damping = 0.1f;
    private float length = 1;
    public GameObject Anchor;

    // Pendulum stuff
    private float epsilon = 0.0001f;
    public Vector3 angularVelocity;
    private Quaternion orientationPendulum;

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
            //UpdateSwing();
        }
    }

    private void GetAnchor() { }

    private void Grapple() {
        Vector3 relativePos = transform.position - Anchor.transform.position;
        length = relativePos.magnitude;
        orientationPendulum = Quaternion.FromToRotation(Vector3.down, relativePos);
    }

    private Vector3 TorqueGravity(Quaternion q) {
        Vector3 up = q * Vector3.up;
        Vector3 gravityForce = -gravity * up;

        return Vector3.Cross(up, gravityForce) / length;
    }

    void FixedUpdate() {
        //Vector3 relativePos = transform.position - Anchor.transform.position;

        float dt = Time.fixedDeltaTime;

        Debug.Log(orientationPendulum);
        (Quaternion newRotation, Vector3 newAngularVelocity) = RK4Step(orientationPendulum, angularVelocity, dt);
        orientationPendulum = newRotation;
        angularVelocity = newAngularVelocity;
        Debug.Log(angularVelocity);
        transform.position = Anchor.transform.position + orientationPendulum * (Vector3.down * length);
    }

    private (Quaternion, Vector3) RK4Step(Quaternion q, Vector3 w, float dt) {
        (Vector3 k1_w, Quaternion k1_q) = Derivatives(q, w);
        (Vector3 k2_w, Quaternion k2_q) = Derivatives(q * ScaleQuaternion(k1_q, 0.5f * dt), w + 0.5f * dt * k1_w);
        (Vector3 k3_w, Quaternion k3_q) = Derivatives(q * ScaleQuaternion(k2_q, 0.5f * dt), w + 0.5f * dt * k2_w);
        (Vector3 k4_w, Quaternion k4_q) = Derivatives(q * ScaleQuaternion(k3_q, dt), w + dt * k3_w);

        Vector3 newAngularVelocity = w + dt / 6.0f * (k1_w + 2 * k2_w + 2 * k3_w + k4_w);
        Quaternion delta_q = ScaleQuaternion(k1_q * ScaleQuaternion(k2_q, 2) * ScaleQuaternion(k3_q, 2) * k4_q, dt / 6.0f);

        Quaternion newRotation = q * delta_q;

        return (Quaternion.Normalize(newRotation), newAngularVelocity);
    }

    private (Vector3, Quaternion) Derivatives(Quaternion q, Vector3 v) {
        Vector3 r = q * (Vector3.down * length);
        Vector3 torque = Vector3.Cross(r, Vector3.down * gravity) - damping * v;
        Quaternion qDot = ScaleQuaternion(new Quaternion(0, v.x, v.y, v.z) * q, 0.5f);

        return (torque, qDot);
    }

    private Quaternion ScaleQuaternion(Quaternion q, float scale) {
        return new Quaternion(q.x * scale, q.y * scale, q.z * scale, q.w * scale);
    }
}

