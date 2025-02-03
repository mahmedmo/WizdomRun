using UnityEngine;

public class GameTimeManager : MonoBehaviour
{
    public static GameTimeManager Instance { get; private set; }

    public float gameTime { get; private set; } = 0f;
    public float runStartDelay = 5.0f;

    public float rareSpawnDelay = 15.0f;
    public float npcSpawnDelay = 10.0f;

    private float rareSpawnTimer = 0.0f;
    private float npcSpawnTimer = 0.0f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        gameTime += Time.deltaTime;
    }

    public bool RunStart()
    {
        return gameTime >= runStartDelay;
    }

    public bool CanRareSpawn()
    {
        if (gameTime - rareSpawnTimer < rareSpawnDelay) return false;
        Debug.Log("Rare Structure Spawned! Time: " + gameTime);
        rareSpawnTimer = gameTime;
        return true;
    }

    public bool CanNPCSpawn()
    {
        if (gameTime - npcSpawnTimer < npcSpawnDelay) return false;
        Debug.Log("NPC Spawned! Time:" + gameTime);
        npcSpawnTimer = gameTime;
        return true;
    }
}