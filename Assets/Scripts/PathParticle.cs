using UnityEngine;

public class PathParticle : MonoBehaviour {
    private float TimeToLive;
    private Vector3 initalSize;
    private GameManager gameManager;
    private GameObject manager;
    float timer;

    void Start() {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        TimeToLive = gameManager.DecayTime;
        initalSize = transform.localScale;

    }

    // Update is called once per frame
    void Update() {
        timer += Time.deltaTime;
        if (TimeToLive <= 0) { return; }
        float shrinkFactor = Mathf.Max(0, 1 - (timer / TimeToLive));
        transform.localScale = initalSize * shrinkFactor;
        if (timer >= TimeToLive) {
            Destroy(gameObject);
        }
    }
}
