using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEngine.Rendering;
public class EnemySpawner : MonoBehaviour
{
    public EnemyDatabase enemyDatabase;
    public Tilemap tilemap;

    private bool sortAlt = false;
    public int enemySpacing = 2;

    void Start()
    {
        tilemap = GameObject.Find("Overlay01").GetComponent<Tilemap>();
    }
    void Update()
    {
        if (LevelManager.Instance.enemyCount > LevelManager.Instance.maxEnemyCount || GameManager.Instance.isPaused)
            return;

        if (GameManager.Instance.CanEnemySpawn())
            SpawnEnemy();
    }

    // Filters the enemy database list to return an
    // enemy weighted according to their spawnRates
    Enemy SelectEnemy(List<Enemy> enemies)
    {
        float totalRate = 0f;
        foreach (Enemy enemy in enemies)
        {
            totalRate += enemy.spawnRate;
        }

        float randomValue = Random.Range(0f, totalRate);
        foreach (Enemy enemy in enemies)
        {
            randomValue -= enemy.spawnRate;
            if (randomValue <= 0f)
            {
                return enemy;
            }
        }
        // Fallback: return last enemy if none selected.
        return enemies[enemies.Count - 1];
    }
    void SpawnEnemy()
    {
        if (enemyDatabase.enemyList == null || enemyDatabase.enemyList.Count == 0 || tilemap == null)
            return;

        List<Enemy> enemyPool = enemyDatabase.enemyList.FindAll(e =>
            e.levelId == LevelManager.Instance.campaignLevel
        );

        // Determine group spawn: by default, spawn one enemy.
        int groupCount = 1;
        // For example, a 40% chance to spawn a group of 2 to 4 enemies:
        if (Random.value < 0.4f)
        {
            groupCount = Random.Range(2, LevelManager.Instance.maxEnemyCount);
        }
        int remainingSpawnCount = LevelManager.Instance.maxEnemyCount - LevelManager.Instance.enemyCount;
        groupCount = Mathf.Clamp(groupCount, 1, remainingSpawnCount);

        for (int i = 0; i < groupCount; i++)
        {
            Enemy selectedEnemy = SelectEnemy(enemyPool);

            // Base spawn position: center-top of the tilemap.
            int spawnX = tilemap.origin.x + tilemap.size.x / 2;
            int spawnY = tilemap.origin.y + tilemap.size.y + 5 + (enemySpacing * i);
            Vector3 spawnPos = tilemap.CellToWorld(new Vector3Int(spawnX, spawnY, -2));

            GameObject enemy = Instantiate(selectedEnemy.prefab, spawnPos, Quaternion.identity);
            enemy.transform.parent = this.transform;

            // Add physics components.
            Rigidbody2D rb = enemy.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            enemy.AddComponent<BoxCollider2D>();

            // Attach and configure the EnemyMonitor script.
            EnemyMonitor monitor = enemy.AddComponent<EnemyMonitor>();
            monitor.Speed = -selectedEnemy.speed; // Negative for downward movement.
            monitor.Health = selectedEnemy.health;
            monitor.Damage = selectedEnemy.damage;
            monitor.Airborne = selectedEnemy.airborne;

            LevelManager.Instance.IncrementEnemyCount();
        }
    }

}