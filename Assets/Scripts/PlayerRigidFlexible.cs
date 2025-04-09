using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerRigidFlexible : MonoBehaviour {

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
    public Vector2 InitialSphericalVelocity = new Vector2(0, 0); // (omega, alpha)

    // Anchor stuff
    private GameObject[] Anchors;
    private int anchorIndex = 0;

    // Pendulum stuff
    private Vector2 SphericalCoords; // (theta [polar], phi [azimuthal])
    float length;
    private Vector2 SphericalVelocity; // (omega [thetaDot], alpha [phiDot])
    private Vector2 SphericalAcc; // (omegaDot [thetaDotDot], alphaDot [phiDOtDot])
    private Vector3 CartesianVelocity; // (xDot, yDot, zDot)
    bool Switching = false;
    bool FreeFalling = false;

    private void Awake() {
        GetInput = GetComponent<InputSubscription>();

        // Very bad, but saves multiple divisions per frame. In the acceleration formulas (see RungeKutta.cs for more info), k and damping are supposed to be divided by mass, since mass is constant here and there is only one player, I optimized it by doing the division beforehand
        // DO NOT DO THIS IF YOU HAVE MULTIPLE OBJECTS OF VARYING MASS USING THE RUNGEKUTTA APPROX AND REMOVE THE NEXT LINE
        GameManager.Instance.UpdateDamping(mass);
        Anchors = GameManager.Instance.Anchors;
        InvokeRepeating(nameof(SpawnParticle), 0f, ParticleInterval);
        lineDrawer = GameObject.Find("LineDrawer").GetComponent<LineDrawer>();

        SphericalVelocity = new Vector2(InitialSphericalVelocity.x, InitialSphericalVelocity.y);
        Grapple();
    }

    private void Update() {
        if (GetInput.Swing && !Switching) {
            Debug.Log("SWITCH");
            //SphericalCoords = Utils.RelativeCartesianToSphericalCoords(transform.position - Anchors[anchorIndex].transform.position);
            if (!FreeFalling) { CartesianVelocity = Utils.SphericalToCartesianVelocity(SphericalVelocity, SphericalCoords, length); }
            lineDrawer.SetAnchor(null);
            Switching = true;
            FreeFalling = true;
            return;
        }
        else if (!GetInput.Swing && Switching) {
            Switching = false;
            SwitchAnchor();
            Grapple();
            SphericalVelocity = Utils.CartesianToSphericalVelocity(CartesianVelocity, SphericalCoords, length, GameManager.Instance.epsilon);
        }
        if (!Utils.IsRadialMovementOutwardRigid(SphericalCoords, SphericalVelocity) && !FreeFalling && !Switching) {
            //SphericalCoords = Utils.RelativeCartesianToSphericalCoords(transform.position - Anchors[anchorIndex].transform.position);
            CartesianVelocity = Utils.SphericalToCartesianVelocity(SphericalVelocity, SphericalCoords, length);
            lineDrawer.SetToFreefall();
            FreeFalling = true;
        }
        else if (Vector3.Distance(transform.position, Anchors[anchorIndex].transform.position) >= length + GameManager.Instance.epsilonLength && FreeFalling && !Switching) {
            SphericalCoords = Utils.RelativeCartesianToSphericalCoords(transform.position - Anchors[anchorIndex].transform.position);
            SphericalVelocity = Utils.CartesianToSphericalVelocity(CartesianVelocity, SphericalCoords, length, GameManager.Instance.epsilon);
            lineDrawer.SetToPendulum();
            FreeFalling = false;
        }
    }

    void FixedUpdate() {
        if (!FreeFalling) {
            float[] state = { SphericalCoords.x, SphericalVelocity.x, SphericalCoords.y, SphericalVelocity.y };
            state = RungeKutta.Step(Time.fixedDeltaTime, state, length);
            ParseState(state);
            transform.position = Anchors[anchorIndex].transform.position + Utils.SphericalToCartesianCoords(SphericalCoords, length);
        }
        else {
            CartesianVelocity = Utils.FreefallDisplacement(CartesianVelocity, Time.fixedDeltaTime);
            transform.position += CartesianVelocity * Time.fixedDeltaTime;
        }
    }

    void SwitchAnchor() {
        anchorIndex = Mathf.Min(Anchors.Length - 1, anchorIndex + 1);
    }

    private void Grapple() {
        Vector3 relativePos = transform.position - Anchors[anchorIndex].transform.position;
        length = Mathf.Max(relativePos.magnitude, GameManager.Instance.epsilonLength);
        SphericalCoords = Utils.RelativeCartesianToSphericalCoords(relativePos);
        lineDrawer.SetAnchor(Anchors[anchorIndex].transform);
        lineDrawer.SetToPendulum();
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
    }
}