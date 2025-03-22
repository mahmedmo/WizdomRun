using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEngine.Rendering;
public class EnemySpawner : MonoBehaviour
{
    public EnemyDatabase enemyDatabase;
    public Tilemap tilemap;

    public int enemySpacing = 1;

    void Start()
    {
        tilemap = GameObject.Find("LOverlay01").GetComponent<Tilemap>();
    }
    void Update()
    {
        if (LevelManager.Instance.inCutscene && GameManager.Instance.isPaused) DestroyEnemies();
        if (LevelManager.Instance.enemyCount > LevelManager.Instance.maxEnemyCount || GameManager.Instance.isPaused)
            return;

        if (GameManager.Instance.CanEnemySpawn())
            SpawnEnemy();

    }
    void DestroyEnemies()
    {
        if (LevelManager.Instance.enemyCount == 0) return;
        Transform[] children = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            children[i] = transform.GetChild(i);
        }

        foreach (Transform child in children)
        {

            Destroy(child.gameObject);
            LevelManager.Instance.DecrementEnemyCount();
        }
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
        return enemies[enemies.Count - 1];
    }
    void SpawnEnemy()
    {
        if (enemyDatabase.enemyList == null || enemyDatabase.enemyList.Count == 0 || tilemap == null)
            return;

        List<Enemy> enemyPool = enemyDatabase.enemyList.FindAll(e =>
            e.campaignLevel == CampaignManager.Instance.GetLevel()
        );

        // Determine group spawn, defaults to one enemy
        int groupCount = 1;

        // 40% chance for multi enemy spawns
        if (Random.value < 0.4f)
        {
            groupCount = Random.Range(2, LevelManager.Instance.maxEnemyCount);
        }

        int remainingSpawnCount = LevelManager.Instance.maxEnemyCount - LevelManager.Instance.enemyCount;
        groupCount = Mathf.Clamp(groupCount, 1, remainingSpawnCount);

        for (int i = 0; i < groupCount; i++)
        {
            Enemy selectedEnemy = SelectEnemy(enemyPool);

            int spawnX = tilemap.origin.x + tilemap.size.x / 2;

            // Place enemies in reverse order to fix Z value overlaps
            int spawnY = tilemap.origin.y + tilemap.size.y + 5 - (enemySpacing * i);

            Vector3 spawnPos = tilemap.CellToWorld(new Vector3Int(spawnX, spawnY, 0));
            spawnPos.z = -3;
            GameObject enemy = Instantiate(selectedEnemy.prefab, spawnPos, Quaternion.identity);
            enemy.transform.parent = this.transform;

            Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = enemy.AddComponent<Rigidbody2D>();

            }
            rb.bodyType = RigidbodyType2D.Kinematic;

            // Attack EnemyMonitor script (internal checks for Enemy)
            EnemyMonitor monitor = enemy.AddComponent<EnemyMonitor>();
            monitor.Speed = -selectedEnemy.speed;
            monitor.Health = selectedEnemy.health;
            monitor.Damage = selectedEnemy.damage;
            monitor.Airborne = selectedEnemy.airborne;
            monitor.GoldDrop = selectedEnemy.goldDrop;

            LevelManager.Instance.IncrementEnemyCount();
        }
    }

}