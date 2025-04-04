using System;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class GameManager : MonoBehaviour {

    public static GameManager Instance { get; private set; }
    InputSubscription GetInput;

    [Header("Environement Variables")]
    public float gravity = 9.81f;
    public float damping = 0.1f;
    public float k = 0;
    public float DecayTime;
    public float epsilon = 0.05f;
    public float epsilonLength = 1f;
    public GameObject[] Anchors;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            GetInput = GetComponent<InputSubscription>();
        }
        else {
            Destroy(gameObject);
        }
    }

    public void UpdateDampingAndK(float mass) {
        try {
            damping = damping / mass;
            k = k / mass;

        }
        catch (DivideByZeroException ex) {
            Debug.LogError("Division by zero: player mass cannot be zero");
            throw ex;
        }
    }

    public void UpdateDamping(float mass) {
        try {
            damping = damping / mass;
        }
        catch (DivideByZeroException ex) {
            Debug.LogError("Division by zero: player mass cannot be zero");
            throw ex;
        }
    }
}
