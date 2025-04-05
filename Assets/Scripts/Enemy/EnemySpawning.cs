using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // Assign your enemy prefab in Inspector
    public float spawnInterval = 10f;

    private void Start()
    {
        InvokeRepeating(nameof(SpawnEnemy), 0f, spawnInterval);
    }

    private void SpawnEnemy()
    {
        Instantiate(enemyPrefab, transform.position, Quaternion.identity);
    }
}

