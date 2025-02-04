using UnityEngine;

public class ObstacleSpawner : MonoBehaviour {

    public GameObject wallPrefab;
    public float corridorWidth;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        float x = corridorWidth / 2f;
        float y = wallPrefab.transform.localScale.y / 2;
        float z = wallPrefab.transform.localScale.z / 2;

        Instantiate(wallPrefab, new Vector3(x, y, z), Quaternion.identity);
        Instantiate(wallPrefab, new Vector3(-x, y, z), Quaternion.identity);
    }
}
