using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemySpawner : MonoBehaviour
{
    [Header("Tilemap")]
    public Tilemap tilemap;

    [Header("Enemy Speed")]
    public float enemySpeed = 5.0f;

    [Header("Enemy Sprites")]
    public GameObject[] enemyPrefabs;
    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsPaused)
            return;

        if (GameManager.Instance != null && GameManager.Instance.CanEnemySpawn())
            SpawnEnemy();
    }

    void SpawnEnemy()
    {
        if (enemyPrefabs.Length <= 0 || tilemap == null)
            return;

        // Spawn position logic (center-top of tilemap)
        int spawnX = tilemap.origin.x + tilemap.size.x / 2;
        int spawnY = tilemap.origin.y + tilemap.size.y + 5;
        Vector3 spawnPos = tilemap.CellToWorld(new Vector3Int(spawnX, spawnY, -2));

        GameObject enemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)], spawnPos, Quaternion.identity);
        enemy.transform.parent = this.transform;

        // Z level positioning
        Vector3 enemyLocalPos = enemy.transform.localPosition;
        enemyLocalPos.z = -2;
        enemy.transform.localPosition = enemyLocalPos;

        // Spite movement enabler
        Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(0, -enemySpeed);
        }

        // Add EnemyMonitor script to created sprite to handle
        // sprite specific functions and pass needed vals
        if (enemy.GetComponent<EnemyMonitor>() == null)
        {
            enemy.AddComponent<EnemyMonitor>();
        }
        
        enemy.GetComponent<EnemyMonitor>().EnemySpeed = -enemySpeed;
        enemy.GetComponent<EnemyMonitor>().parentSpawner = this;
    }
}