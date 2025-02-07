using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour {

    public float widthCorridor;
    public float lengthCorridor;
    public float heightCorridor;
    public GameObject wall1;
    public GameObject wall2;
    public GameObject ground;

    public GameObject anchorPrefab;
    public int anchorCount;
    public static ObstacleSpawner Instance;
    public List<GameObject> anchors = new List<GameObject>();

    void Start() {
        spawnAnchors();
    }

    private void spawnAnchors() {
        float interval = lengthCorridor / (anchorCount + 1);
        float x = widthCorridor / 2;
        for (int i = 1; i <= anchorCount; i++) {
            anchors.Add(Instantiate(anchorPrefab, new Vector3(x, heightCorridor, interval * i), Quaternion.identity));
            anchors.Add(Instantiate(anchorPrefab, new Vector3(-x, heightCorridor, interval * i), Quaternion.identity));
        }
    }
    private void rescale() {
        float x = widthCorridor / 2f;
        float y = heightCorridor / 2;
        float z = lengthCorridor / 2;

        wall1.transform.position = new Vector3(x, y, z);
        wall2.transform.position = new Vector3(-x, y, z);
        ground.transform.position = new Vector3(0, 0, z - 2);

        wall1.transform.localScale = new Vector3(1, heightCorridor, lengthCorridor);
        wall2.transform.localScale = new Vector3(1, heightCorridor, lengthCorridor);
        ground.transform.localScale = new Vector3(widthCorridor / 10, 1, lengthCorridor / 10 + 5);

    }

    private void Update() {
        rescale();
    }
}
