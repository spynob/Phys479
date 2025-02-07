using UnityEngine;

public class GameManager : MonoBehaviour {
    InputSubscription GetInput;

    private void Awake() {
        GetInput = GetComponent<InputSubscription>();
    }
}
