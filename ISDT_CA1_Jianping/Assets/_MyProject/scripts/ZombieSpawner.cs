using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    [Header("Zombie Prefab")]
    [SerializeField] private GameObject zombiePrefab;

    [Header("Spawn Settings")]
    public float initialSpawnDelay = 2f;   // Time before first wave spawns
    public float spawnInterval = 10f;      // Time between spawns
    public float spawnRadius = 10f;        // Radius for where zombies appear

    [Header("Zombie Movement Settings")]
    public float zombieSpeed = 2f;         // Fixed speed for newly spawned zombies

    void Start()
    {
        // Schedule repeated spawns

        InvokeRepeating(nameof(SpawnZombies), initialSpawnDelay, spawnInterval);
    }

    void SpawnZombies()
    {
        int spawnCount = Random.Range(1, 2);

        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 spawnPos = transform.position + Random.insideUnitSphere * spawnRadius;
            spawnPos.y = transform.position.y;

            GameObject zombie = Instantiate(zombiePrefab, spawnPos, Quaternion.identity);

            // Set enemy speed in the EnemyAI script
            EnemyAI ai = zombie.GetComponent<EnemyAI>();
            if (ai != null)
            {
                ai.moveSpeed = zombieSpeed;
                ai.InitializeAnimations(); // Add this call
            }
        }
    }

    
}