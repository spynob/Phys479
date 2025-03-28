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

    // Tether
    LineDrawer lineDrawer;

    // Input Movement
    [SerializeField] InputSubscription GetInput;

    // Properties
    public float mass = 100;
    public float InitialStretching = 0;
    public Vector2 IntialSphericalVelocity = new Vector2(0, 0); // (omega, alpha)

    // Anchor stuff
    public GameObject[] Anchors;
    private int anchorIndex = 0;

    // Pendulum stuff
    private float naturalLength;
    private Vector3 SphericalCoords; // (theta [polar], phi [azimuthal], length [radial])
    private Vector3 SphericalVelocity; // (omega [thetaDot], alpha [phiDot], lengthDot)
    private Vector3 SphericalAcc; // (omegaDot [thetaDotDot], alphaDot [phiDOtDot], lengthDotDot)
    private Vector3 CartesianVelocity; // (xDot, yDot, zDot)
    private bool InsideRadius = false;
    bool Switching = false;

    private void Awake() {
        GetInput = GetComponent<InputSubscription>();

        // Very bad, but saves multiple divisions per frame. In the acceleration formulas (see RungeKutta.cs for more info), k and damping are supposed to be divided by mass, since mass is constant here and there is only one player, I optimized it by doing the division beforehand
        // DO NOT DO THIS IF YOU HAVE MULTIPLE OBJECTS OF VARYING MASS USING THE RUNGEKUTTA APPROX AND REMOVE THE NEXT LINE
        GameManager.Instance.UpdateDampingAndK(mass);

        SphericalVelocity = new Vector3(IntialSphericalVelocity.x, IntialSphericalVelocity.y, 0);
        Grapple();
        naturalLength = Mathf.Max(naturalLength - InitialStretching, GameManager.Instance.epsilonLength * 1.1f);

        InvokeRepeating(nameof(SpawnParticle), 0f, ParticleInterval);
        lineDrawer = GameObject.Find("LineDrawer").GetComponent<LineDrawer>();
    }

    private void Update() {
        if (GetInput.Swing && !Switching) {
            Debug.Log("SWITCH");
            SwitchAnchor();
            CartesianVelocity = Utils.SphericalToCartesianVelocity(SphericalVelocity, SphericalCoords);
            lineDrawer.setAnchor(null);
            Switching = true;
            return;
        }
        else if (!GetInput.Swing && Switching) {
            Switching = false;
            lineDrawer.setAnchor(Anchors[anchorIndex].transform);
            SaveLength();
        }
        lineDrawer.setStress(SphericalCoords.z - naturalLength);
        CheckRadius();
    }

    void FixedUpdate() {
        if (!Switching && !InsideRadius) {
            float[] state = { SphericalCoords.x, SphericalVelocity.x, SphericalCoords.y, SphericalVelocity.y, SphericalCoords.z, SphericalVelocity.z };
            state = RungeKutta.Step(Time.fixedDeltaTime, state, naturalLength);
            ParseState(state);
            //Debug.Log("Theta: " + theta + ", Omega: " + omega + ", Phi: " + phi + ", Alpha: " + alpha + ", Length: " + length + ", LengthDot: " + lengthDot + ", Natural Length: " + naturalLength);
            transform.position = Anchors[anchorIndex].transform.position + Utils.SphericalToCartesianCoords(SphericalCoords);
        }
        else {
            transform.position += Utils.FreefallDisplacement(CartesianVelocity, Time.fixedDeltaTime);
        }
    }

    void CheckRadius() {
        Vector3 relativePos = transform.position - Anchors[anchorIndex].transform.position;
        float distance = relativePos.magnitude;

        if (distance - naturalLength >= -GameManager.Instance.epsilonLength) // outside Radius
        {
            if (!InsideRadius) { return; }
            SphericalCoords = Utils.RelativeCartesianToSphericalCoords(relativePos);
            SphericalVelocity = Utils.CartesianToSphericalVelocitySpring(CartesianVelocity, SphericalCoords, GameManager.Instance.epsilon);
            InsideRadius = false;
        }
        else { // inside radius
            if (InsideRadius) { return; }
            CartesianVelocity = Utils.SphericalToCartesianVelocity(SphericalVelocity, SphericalCoords);
            InsideRadius = true;
        }
    }

    void SwitchAnchor() {
        anchorIndex = Mathf.Min(Anchors.Length - 1, anchorIndex + 1);
    }

    void SaveLength() {
        Vector3 relativePos = transform.position - Anchors[anchorIndex].transform.position;
        naturalLength = relativePos.magnitude;
    }

    private void Grapple() {
        Vector3 relativePos = transform.position - Anchors[anchorIndex].transform.position;
        naturalLength = Mathf.Max(relativePos.magnitude, GameManager.Instance.epsilonLength);
        SphericalCoords = Utils.RelativeCartesianToSphericalCoords(relativePos);
    }

    private void SpawnParticle() {
        if (MovementParticles) {
            Instantiate(particle, transform.position, Quaternion.identity);
        }
    }

    private void ParseState(float[] state) {
        SphericalCoords.x = state[0];
        SphericalVelocity.x = state[1];
        SphericalCoords.y = state[2];
        SphericalVelocity.y = state[3];
        SphericalCoords.z = state[4];
        SphericalVelocity.z = state[5];
    }
}