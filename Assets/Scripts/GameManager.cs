using UnityEngine;

public class GameManager : MonoBehaviour {
    InputSubscription GetInput;
    public float DecayTime;
    private void Awake() {
        GetInput = GetComponent<InputSubscription>();
    }
}
