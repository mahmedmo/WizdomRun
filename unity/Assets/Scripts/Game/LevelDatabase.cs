using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LevelDatabase", menuName = "ScriptableObjects/LevelDatabase", order = 6)]
public class LevelDatabase : ScriptableObject
{
    public List<Level> levels;
}

[System.Serializable]
public class Level
{
    public int levelId;
    public int maxEnemyCount;
    public float enemySpawnDelay;
    public float npcSpawnDelay;
    public float enemyDmgMultiplier;
    public GameObject intro;
    public float introDelay;
}
