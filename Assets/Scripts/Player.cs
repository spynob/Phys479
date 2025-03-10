using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Player : MonoBehaviour {

    // Particles
    public bool MovementParticles = false;
    public GameObject particle;
    public float ParticleInterval = 0.5f;

    // Input Movement
    [SerializeField] InputSubscription GetInput;
    bool Switching = false;

    // Properties
    public float mass = 100;
    public float gravity = 9.81f;
    public float damping = 0.1f;
    private float length = 1;

    // Anchor stuff
    public GameObject[] Anchors;
    private int anchorIndex = 0;

    // Pendulum stuff
    private float theta; // Polar angle
    private float phi; // Azimuthal angle
    private float omega = 0;
    private float alpha = 0f;
    private float thetaDdot = 0;
    private float phiDdot = 0;
    private float epsilon = 0.001f;
    private float epsilonLength = 0.1f;

    // Momentum Transfer
    private bool Swinging = true;

    //thetaDdot = -gravity / length * Mathf.Sin(theta) + Mathf.Sin(theta) * Mathf.Cos(theta) * alpha * alpha - (damping * omega);
    //phiDdot = -(2 * omega * alpha) / Mathf.Tan(theta) - (damping * alpha);

    private void Awake() {
        GetInput = GetComponent<InputSubscription>();
        SaveLength();
        Grapple();
        InvokeRepeating(nameof(SpawnParticle), 0f, ParticleInterval);
    }

    private void Update() {
        if (GetInput.Swing && !Switching) {
            Debug.Log("SWITCH");
            SwitchAnchor();
            Swinging = false;
            Switching = true;
        }
        if (!GetInput.Swing) { Switching = false; }
        if (!Swinging) {
            FreeFall();
            CheckIsOutRadius();
        }
    }

    void FreeFall() {
        yMom -= (gravity + damping * yMom) * Time.deltaTime;
        transform.position = new Vector3(transform.position.x + xMom * (1 - damping) * Time.deltaTime, transform.position.y + yMom * Time.deltaTime, transform.position.z + zMom * (1 - damping) * Time.deltaTime);
    }

    void CheckIsOutRadius() {
        Vector3 relativePos = transform.position - Anchors[anchorIndex].transform.position;
        float distance = relativePos.magnitude;
        if (distance >= length) { // update angles and set swinging to true
            Grapple();
            Swinging = true;
            (omega, alpha) = GetSphericalMomentum(new Vector3(xMom, yMom, zMom), theta, phi, length);
        }
    }

    void SaveLength() {
        Vector3 relativePos = transform.position - Anchors[anchorIndex].transform.position;
        length = relativePos.magnitude;
    }

    void SwitchAnchor() {
        Vector3 cartesianMomentum = GetCartesianMomentum(theta, omega, phi, alpha, length);
        anchorIndex = Mathf.Min(anchorIndex + 1, Anchors.Length - 1);
        Grapple();
        (omega, alpha) = GetSphericalMomentum(cartesianMomentum, theta, phi, length);
        anchorIndex = Mathf.Min(2, anchorIndex + 1);
    }

    // Not sure about this one
    private (float, float) GetSphericalMomentum(Vector3 cartesianMomentum, float pTheta, float pPhi, float pLength) {
        float o = (Mathf.Cos(pTheta) * (cartesianMomentum.x * Mathf.Cos(pPhi) + cartesianMomentum.z * Mathf.Sin(pPhi)) + cartesianMomentum.y * Mathf.Sin(pTheta)) / pLength;

        float dividant = pLength * Mathf.Sin(pTheta);
        float a;
        if (dividant < epsilon) { a = 0; }
        else { a = (cartesianMomentum.z * Mathf.Cos(pPhi) - cartesianMomentum.x * Mathf.Sin(pPhi)) / (pLength * Mathf.Sin(pTheta)); }
        return (o, a);
    }

    private Vector3 GetCartesianMomentum(float pTheta, float pOmega, float pPhi, float pAlpha, float pLength) {
        float xDot = pLength * (pOmega * Mathf.Cos(pTheta) * Mathf.Cos(pPhi) - pAlpha * Mathf.Sin(pTheta) * Mathf.Sin(pPhi));
        float yDot = pLength * pOmega * Mathf.Sin(pTheta);
        float zDot = pLength * (pOmega * Mathf.Cos(pTheta) * Mathf.Sin(pPhi) + pAlpha * Mathf.Sin(pTheta) * Mathf.Cos(pPhi));
        return new Vector3(xDot, yDot, zDot);
    }

    private void Grapple() {
        Vector3 relativePos = transform.position - Anchors[anchorIndex].transform.position;
        length = relativePos.magnitude;
        if (length < epsilonLength) { theta = 0; phi = 0; length = epsilonLength; return; }
        theta = Mathf.Acos(-relativePos.y / length);
        phi = Mathf.Atan2(relativePos.z, relativePos.x);
    }

    void FixedUpdate() {
        if (Swinging) {
            float[] state = { theta, omega, phi, alpha };
            RungeKuttaStep(Time.fixedDeltaTime, state);
            theta = state[0];
            omega = state[1];
            phi = state[2];
            alpha = state[3];
            //Debug.Log("Theta: " + theta + ", Omega: " + omega + ", Phi: " + phi + ", Alpha: " + alpha);
            transform.position = Anchors[anchorIndex].transform.position + SphericalToCartesian(theta, phi, length);
        }
    }

    void RungeKuttaStep(float h, float[] state) {
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

        thetaDdot = -gravity / length * Mathf.Sin(theta) + Mathf.Sin(theta) * Mathf.Cos(theta) * alpha * alpha - damping * omega;

        float dividant = Mathf.Tan(theta);
        if (Mathf.Abs(dividant) > epsilon) { phiDdot = -2 * omega * alpha / dividant - damping * alpha; }
        else { phiDdot = 0; }
        return new float[] { omega, thetaDdot, alpha, phiDdot };
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

    private void SpawnParticle() {
        if (MovementParticles) {
            Instantiate(particle, transform.position, Quaternion.identity);
        }
    }
}