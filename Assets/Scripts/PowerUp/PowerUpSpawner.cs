using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    
    public Transform spawnLocationsParent;

    public GameObject powerUpPrefab;

    private Transform[] spawnPoints;

    void Awake()
    {
        // Get all child transforms under the parent (excluding the parent itself)
        spawnPoints = new Transform[spawnLocationsParent.childCount];
        for (int i = 0; i < spawnLocationsParent.childCount; i++)
        {
            spawnPoints[i] = spawnLocationsParent.GetChild(i);
        }
    }
    public void SpawnPowerUp()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("No spawn points available.");
            return;
        }

        int randomIndex = Random.Range(0, spawnPoints.Length);
        Vector3 spawnPos = spawnPoints[randomIndex].position;

        Instantiate(powerUpPrefab, spawnPos, Quaternion.identity);
    }
}
