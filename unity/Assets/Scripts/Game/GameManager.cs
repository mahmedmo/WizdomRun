using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public float gameTime { get; private set; } = 0f;

    public float runStartDelay = 5.0f;
    public float rareSpawnDelay = 15.0f;
    public float npcSpawnDelay = 10.0f;
    public float enemySpawnDelay = 10.0f;

    private float rareSpawnTimer = 0.0f;
    private float npcSpawnTimer = 0.0f;
    private float enemySpawnTimer = 0.0f;

    private bool isPaused = false;
    public bool IsPaused { get { return isPaused; } }

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
        if (!isPaused)
        {
            gameTime += Time.deltaTime;
        }
    }

    public bool RunStart()
    {
        if (gameTime >= runStartDelay)
        {
            PlayerMonitor.Instance.SetState(PlayerState.Run);
            return true;
        }else{
            PlayerMonitor.Instance.SetState(PlayerState.Idle);
        }

        return false;
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

    public bool CanEnemySpawn()
    {
        if (gameTime - enemySpawnTimer < enemySpawnDelay) return false;
        Debug.Log("Enemy Spawned! Time:" + gameTime);
        enemySpawnTimer = gameTime;
        return true;
    }


    public void PauseMovement()
    {
        if (!isPaused)
        {
            isPaused = true;
            // Set the player's state to Idle while paused.
            PlayerMonitor.Instance?.SetState(PlayerState.Idle);
            Debug.Log("Game Paused");
        }
    }

    public void ResumeMovement()
    {
        if (isPaused)
        {
            isPaused = false;
            // When resuming, switch the player's state back to Run.
            PlayerMonitor.Instance?.SetState(PlayerState.Run);
            Debug.Log("Game Resumed");
        }
    }
}