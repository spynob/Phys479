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
    private float theta; // Polar angle
    private float phi; // Azimuthal angle
    private float thetaDot = 0;
    private float phiDot = 0.5f;
    private float thetaDdot = 0;
    private float phiDdot = 0;
    private float epsilon = 0.0001f;
    public Vector3 angularVelocity;
    private Quaternion orientationPendulum;

    private float omega = 0; // Theta (angle) and omega (angular velocity)
    private float alpha = 0;   // Phi (azimuthal) and alpha (angular velocity)


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
        if (length < epsilon) { theta = 0; phi = 0; return; }
        theta = Mathf.Acos(relativePos.y / length);
        phi = Mathf.Atan2(relativePos.z, relativePos.x);
    }

    private Vector3 TorqueGravity(Quaternion q) {
        Vector3 up = q * Vector3.up;
        Vector3 gravityForce = -gravity * up;

        return Vector3.Cross(up, gravityForce) / length;
    }

    void FixedUpdate() {
        Debug.Log("Theta: " + theta + ", omega: " + omega + ", phi: " + phi + ", alpha: " + alpha);
        float[] state = { theta, omega, phi, alpha };
        RungeKuttaStep(state, Time.fixedDeltaTime);
        theta = state[0];
        omega = state[1];
        phi = state[2];
        alpha = state[3];
        /*
        //Vector3 relativePos = transform.position - Anchor.transform.position;

        float dt = Time.fixedDeltaTime;

        // Apply RK4 integration
        Debug.Log(orientationPendulum);
        (Quaternion newRotation, Vector3 newAngularVelocity) = RK4Step(orientationPendulum, angularVelocity, dt);
        orientationPendulum = newRotation;
        angularVelocity = newAngularVelocity;
        Debug.Log(angularVelocity);
        transform.position = Anchor.transform.position + orientationPendulum * (Vector3.down * length);*/

        /*
                Vector3 torque = Vector3.Cross(relativePos, Vector3.down * gravity) - damping * angularVel;

                angularVel += torque * Time.fixedDeltaTime;

                Quaternion deltaRotation = new Quaternion(0, angularVel.x, angularVel.y, angularVel.z) * rotation;

                rotation.w += 0.5f * deltaRotation.w * Time.fixedDeltaTime;
                rotation.x += 0.5f * deltaRotation.x * Time.fixedDeltaTime;
                rotation.y += 0.5f * deltaRotation.y * Time.fixedDeltaTime;
                rotation.z += 0.5f * deltaRotation.z * Time.fixedDeltaTime;*/
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

    private void UpdateSwing() {
        if (length < epsilon) {
            thetaDdot = 0;
        } //else if(theta < epsilon) {theta = epsilon;}
        else {
            thetaDdot = -gravity / length * Mathf.Sin(theta) + Mathf.Sin(theta) * Mathf.Cos(theta) * phiDot * phiDot - (damping * thetaDot);
        }
        if (Mathf.Abs(theta) < epsilon || 180 - Mathf.Abs(theta) < epsilon) { phiDdot = -(damping * phiDot); }
        else { phiDdot = -(2 * thetaDot * phiDot) / Mathf.Tan(theta) - (damping * phiDot); }



        //Debug.Log("\ntheta: " + theta + ", ThetaDot: " + thetaDot + ", ThetaDotDot" + thetaDdot + "\tPhi: " + phi + ", PhiDot: " + phiDot + ", PhiDotDot: " + phiDdot);

        transform.position = Anchor.transform.position + SphericalToCartesian(theta, phi, length);
    }

    void RungeKuttaStep(float[] state, float h) {
        float[] k1 = Derivatives(state);
        float[] k2 = Derivatives(AddVectors(state, MultiplyVector(k1, h / 2)));
        float[] k3 = Derivatives(AddVectors(state, MultiplyVector(k2, h / 2)));
        float[] k4 = Derivatives(AddVectors(state, MultiplyVector(k3, h)));

        for (int i = 0; i < state.Length; i++) {
            state[i] += h / 6 * (k1[i] + 2 * k2[i] + 2 * k3[i] + k4[i]);
        }
    }

    float[] Derivatives(float[] state) {
        float theta = state[0];
        float omega = state[1];
        float alpha = state[3];

        float omegaDot = -gravity / length * Mathf.Sin(theta) * Mathf.Cos(theta) * alpha * alpha - damping * omega;
        float alphaDot = -2 * omega * alpha / Mathf.Tan(theta) - damping * alpha;

        return new float[] { omega, omegaDot, alpha, alphaDot };
    }

    float[] AddVectors(float[] a, float[] b) {
        float[] result = new float[a.Length];
        for (int i = 0; i < a.Length; i++) {
            result[i] = a[i] + b[i];
        }
        return result;
    }

    float[] MultiplyVector(float[] a, float scalar) {
        float[] result = new float[a.Length];
        for (int i = 0; i < a.Length; i++) {
            result[i] = a[i] * scalar;
        }
        return result;
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

