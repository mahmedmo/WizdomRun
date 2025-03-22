using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EnemyDatabase", menuName = "ScriptableObjects/EnemyDatabase", order = 4)]
public class EnemyDatabase : ScriptableObject
{
    public List<Enemy> enemyList = new List<Enemy>()
    {
        // Level 1
        new Enemy() { id = 0, campaignLevel = 1, name = "Leafer", damage = 3,  health = 8,  speed = 4, goldDrop = 10,  spawnRate = 0.5f, airborne = false, prefab = null },
        new Enemy() { id = 1, campaignLevel = 1, name = "Eagle", damage = 4,  health = 9,  speed = 4, goldDrop = 16,  spawnRate = 0.5f, airborne = true, prefab = null },
        new Enemy() { id = 2, campaignLevel = 1, name = "Mushroom", damage = 5,  health = 10, speed = 5, goldDrop = 17,  spawnRate = 0.5f, airborne = false, prefab = null },

        // Level 2
        new Enemy() { id = 3, campaignLevel = 2, name = "Seagull", damage = 6,  health = 12, speed = 5, goldDrop = 18,  spawnRate = 0.5f, airborne = true, prefab = null },
        new Enemy() { id = 4, campaignLevel = 2, name = "Viper", damage = 7,  health = 13, speed = 5, goldDrop = 19,  spawnRate = 0.5f, airborne = false, prefab = null },
        new Enemy() { id = 5, campaignLevel = 2, name = "PirateL", damage = 8,  health = 15, speed = 5, goldDrop = 10, spawnRate = 0.5f, airborne = false, prefab = null },

        // Level 3
        new Enemy() { id = 6, campaignLevel = 3, name = "PuppetC", damage = 9,  health = 18, speed = 6, goldDrop = 11, spawnRate = 0.5f, airborne = false, prefab = null },
        new Enemy() { id = 7, campaignLevel = 3, name = "PuppetR", damage = 10, health = 20, speed = 6, goldDrop = 12, spawnRate = 0.5f, airborne = false, prefab = null },
        new Enemy() { id = 8, campaignLevel = 3, name = "PuppetH", damage = 11, health = 22, speed = 6, goldDrop = 13, spawnRate = 0.5f, airborne = false, prefab = null },

        // Level 4
        new Enemy() { id = 9, campaignLevel = 4, name = "Bat", damage = 9, health = 15, speed = 6, goldDrop = 14, spawnRate = 0.5f, airborne = true, prefab = null },
        new Enemy() { id = 10, campaignLevel = 4, name = "Rat", damage = 11, health = 18, speed = 6, goldDrop = 15, spawnRate = 0.5f, airborne = false, prefab = null },
        new Enemy() { id = 11, campaignLevel = 4, name = "Spider", damage = 12, health = 20, speed = 6, goldDrop = 16, spawnRate = 0.5f, airborne = false, prefab = null },

        // Level 5
        new Enemy() { id = 12, campaignLevel = 5, name = "Snowman", damage = 10, health = 22, speed = 7, goldDrop = 17, spawnRate = 0.5f, airborne = false, prefab = null },
        new Enemy() { id = 13, campaignLevel = 5, name = "FrozenWidow", damage = 11, health = 18, speed = 7, goldDrop = 18, spawnRate = 0.5f, airborne = true, prefab = null },
        new Enemy() { id = 14, campaignLevel = 5, name = "FrozenScorpion", damage = 14, health = 20, speed = 6, goldDrop = 16, spawnRate = 0.5f, airborne = false, prefab = null },

        // Level 6
        new Enemy() { id = 15, campaignLevel = 6, name = "Crow", damage = 12, health = 28, speed = 7, goldDrop = 20, spawnRate = 0.5f, airborne = true, prefab = null },
        new Enemy() { id = 16, campaignLevel = 6, name = "Rat", damage = 11, health = 20, speed = 7, goldDrop = 21, spawnRate = 0.5f, airborne = false, prefab = null },
        new Enemy() { id = 17, campaignLevel = 6, name = "SkullTriple", damage = 13, health = 22, speed = 7, goldDrop = 22, spawnRate = 0.5f, airborne = false, prefab = null },

        // Level 7
        new Enemy() { id = 18, campaignLevel = 7, name = "Bat", damage = 16, health = 23, speed = 8, goldDrop = 23, spawnRate = 0.5f, airborne = true, prefab = null },
        new Enemy() { id = 19, campaignLevel = 7, name = "WolfB", damage = 18, health = 24, speed = 8, goldDrop = 24, spawnRate = 0.5f, airborne = false, prefab = null },

        // Level 8
        new Enemy() { id = 21, campaignLevel = 8, name = "Seagull", damage = 14, health = 26, speed = 8, goldDrop = 26, spawnRate = 0.5f, airborne = true, prefab = null },
        new Enemy() { id = 22, campaignLevel = 8, name = "Snowman", damage = 18, health = 27, speed = 8, goldDrop = 27, spawnRate = 0.5f, airborne = false, prefab = null },
        new Enemy() { id = 23, campaignLevel = 8, name = "ElementalAir", damage = 14, health = 20, speed = 9, goldDrop = 31, spawnRate = 0.5f, airborne = true, prefab = null },

        // Level 9
        new Enemy() { id = 24, campaignLevel = 9, name = "Boar", damage = 27, health = 28, speed = 9, goldDrop = 29, spawnRate = 0.5f, airborne = false, prefab = null },
        new Enemy() { id = 25, campaignLevel = 9, name = "Ant", damage = 28, health = 29, speed = 9, goldDrop = 30, spawnRate = 0.5f, airborne = false, prefab = null },

        // Level 10
        new Enemy() { id = 26, campaignLevel = 10, name = "Crow", damage = 30, health = 30, speed = 10, goldDrop = 35, spawnRate = 0.5f, airborne = true, prefab = null },
        new Enemy() { id = 27, campaignLevel = 10, name = "SkullTriple", damage = 32, health = 30, speed = 10, goldDrop = 36, spawnRate = 0.5f, airborne = false, prefab = null },
        new Enemy() { id = 27, campaignLevel = 10, name = "WolfB", damage = 35, health = 30, speed = 10, goldDrop = 40, spawnRate = 0.5f, airborne = false, prefab = null }
    };
}

[System.Serializable]
public class Enemy
{
    public int id;
    public int campaignLevel;
    public string name;
    public int damage;
    public int health;
    public int speed;
    public int goldDrop;
    public float spawnRate;
    public bool airborne;
    public GameObject prefab;

}