using UnityEngine;

public class Wall : MonoBehaviour {
    void OnTriggerEnter(Collider other) {
        Debug.Log("Fail");
    }
}