using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EnemyDatabase", menuName = "ScriptableObjects/EnemyDatabase", order = 4)]
public class EnemyDatabase : ScriptableObject
{
    public List<Enemy> enemyList;
}

[System.Serializable]
public class Enemy
{
    public int id;
    public string name;
    public int damage;
    public int health;
    public int speed;
    public int goldDrop;
    public int levelId;
    public float spawnRate;
    public bool airborne;
    public GameObject prefab;

}