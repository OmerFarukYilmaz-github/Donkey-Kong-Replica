using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject barrelPrefab;
    public float minTime = 2f;
    public float maxTime = 4f;

    private void Start()
    {
        Spawn();
    }

    private void Spawn()
    {
        Instantiate(barrelPrefab, transform.position, Quaternion.identity);
        Invoke(nameof(Spawn), Random.Range(minTime, maxTime));
    }

}
