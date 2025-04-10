using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerSpringDirect : MonoBehaviour {

    // Particles
    public bool MovementParticles = false;
    public GameObject particle;
    public GameObject TransitionParticle;
    public float ParticleInterval = 0.5f;

    // Tether
    LineDrawer lineDrawer;

    // Input Movement
    [SerializeField] InputSubscription GetInput;

    // Properties
    public float mass = 100;
    public Vector3 InitialSphericalVelocity = new Vector3(0, 0, 0); // (omega, alpha, lengthDot)
    public float InitialStretch = 0;

    // Anchor stuff
    private GameObject[] Anchors;
    private int anchorIndex = 0;

    // Pendulum stuff
    private float naturalLength;
    private Vector3 SphericalCoords; // (theta [polar], phi [azimuthal], length [radial])
    private Vector3 SphericalVelocity; // (omega [thetaDot], alpha [phiDot], lengthDot)
    private Vector3 SphericalAcc; // (omegaDot [thetaDotDot], alphaDot [phiDOtDot], lengthDotDot)
    private Vector3 CartesianVelocity; // (xDot, yDot, zDot)
    bool Switching = false;

    private void Awake() {
        GetInput = GetComponent<InputSubscription>();

        // Very bad, but saves multiple divisions per frame. In the acceleration formulas (see RungeKutta.cs for more info), k and damping are supposed to be divided by mass, since mass is constant here and there is only one player, I optimized it by doing the division beforehand
        // DO NOT DO THIS IF YOU HAVE MULTIPLE OBJECTS OF VARYING MASS USING THE RUNGEKUTTA APPROX AND REMOVE THE NEXT LINE
        GameManager.Instance.UpdateDampingAndK(mass);
        Anchors = GameManager.Instance.Anchors;
        InvokeRepeating(nameof(SpawnParticle), 0f, ParticleInterval);
        lineDrawer = GameObject.Find("LineDrawer").GetComponent<LineDrawer>();

        SphericalVelocity = new Vector3(InitialSphericalVelocity.x, InitialSphericalVelocity.y, InitialSphericalVelocity.z);
        Grapple();
        naturalLength = Mathf.Max(naturalLength - InitialStretch, GameManager.Instance.epsilonLength * 1.1f);
    }

    private void Update() {
        if (GetInput.Swing && !Switching) {
            Debug.Log("SWITCH");
            CartesianVelocity = Utils.SphericalToCartesianVelocity(SphericalVelocity, SphericalCoords);
            SwitchAnchor();
            Grapple();
            SpawnTransitionParticle();
            SphericalVelocity = Utils.CartesianToSphericalVelocity(CartesianVelocity, SphericalCoords, GameManager.Instance.epsilon);
            Switching = true;
            return;
        }
        else if (!GetInput.Swing && Switching) {
            Switching = false;
        }
        lineDrawer.SetStress(SphericalCoords.z - naturalLength);
    }

    void FixedUpdate() {
        if (!Switching) {
            float[] state = { SphericalCoords.x, SphericalVelocity.x, SphericalCoords.y, SphericalVelocity.y, SphericalCoords.z, SphericalVelocity.z };
            state = RungeKutta.StepSpring(Time.fixedDeltaTime, state, naturalLength);
            ParseState(state);
            transform.position = Anchors[anchorIndex].transform.position + Utils.SphericalToCartesianCoords(SphericalCoords);
        }
    }

    void SwitchAnchor() {
        anchorIndex = Mathf.Min(Anchors.Length - 1, anchorIndex + 1);
    }

    private void Grapple() {
        Vector3 relativePos = transform.position - Anchors[anchorIndex].transform.position;
        naturalLength = Mathf.Max(relativePos.magnitude, GameManager.Instance.epsilonLength);
        SphericalCoords = Utils.RelativeCartesianToSphericalCoords(relativePos);
        lineDrawer.SetAnchor(Anchors[anchorIndex].transform);
    }

    private void SpawnParticle() {
        if (MovementParticles) {
            Instantiate(particle, transform.position, Quaternion.identity);
        }
    }

    private void SpawnTransitionParticle() {
        if (MovementParticles) {
            Instantiate(TransitionParticle, transform.position, Quaternion.identity);
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