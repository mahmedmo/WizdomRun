using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public float gameTime { get; private set; } = 0f;
    public float rareSpawnDelay = 15.0f;

    private float rareSpawnTimer = 0.0f;
    private float npcSpawnTimer = 0.0f;
    private float enemySpawnTimer = 0.0f;

    public bool IsFrozen { get; private set; }
    public bool isPaused { get; set; } = true;
    public bool lastPauseState { get; set; } = true;

    public bool shownIntro;


    public void ResetGameState()
    {
        gameTime = 0f;
        rareSpawnTimer = 0f;
        npcSpawnTimer = 0f;
        enemySpawnTimer = 0f;
        isPaused = true;
        shownIntro = false;
        InterfaceManager.Instance.HideInterface();
        DialogueManager.Instance.HideChoices();
        CutsceneManager.Instance.AllocateCutscenes();
        Debug.Log("GameManager state reset!");
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        ResetGameState();
    }
    public void FreezeGame()
    {
        lastPauseState = isPaused;
        isPaused = true;
        IsFrozen = true;

    }
    public void UnFreezeGame()
    {
        isPaused = lastPauseState;
        IsFrozen = false;
    }
    void Update()
    {
        if (IsFrozen || LevelManager.Instance.inCutscene) return;
        if (!isPaused)
        {
            gameTime += Time.deltaTime;
            LevelManager.Instance.levelProgress = (gameTime * 10);
            if (!shownIntro && gameTime >= LevelManager.Instance.introDelay)
            {
                shownIntro = true;
                InterfaceManager.Instance.FadeIntro();
            }

        }
        if (LevelManager.Instance.enemyCount == 0 && isPaused && LevelManager.Instance.levelStart) StartCoroutine(ResumeAfterDelay(1f));

    }
    private IEnumerator ResumeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ResumeMovement();
    }
    public bool RunStart()
    {
        if (!isPaused)
        {
            PlayerMonitor.Instance.SetState(PlayerState.Run);
            return true;
        }

        PlayerMonitor.Instance.SetState(PlayerState.Idle);
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
        if (gameTime - npcSpawnTimer < LevelManager.Instance.npcSpawnDelay) return false;
        Debug.Log("NPC Spawned! Time:" + gameTime);
        npcSpawnTimer = gameTime;
        return true;
    }

    public bool CanEnemySpawn()
    {
        if (gameTime - enemySpawnTimer < LevelManager.Instance.enemySpawnDelay) return false;
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
    public void TryAgain()
    {

        // Fade to black over 2 seconds
        InterfaceManager.Instance.FadeToBlack(2f, () =>
        {
            if (CampaignManager.Instance.GetRemainingTries() <= 0)
            {
                CampaignManager.Instance.RestartCampaign();
            }
            // When fade is complete, load the scene
            CampaignManager.Instance.LoadMappedLevel();
        });
    }
    public static void DestroyPersistentObjectsMM()
    {
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.scene.name == "DontDestroyOnLoad")
            {
                if (obj.GetComponent<AuthManager>() != null || obj.name == "Firebase Services")
                    continue;

                Destroy(obj);
            }
        }
    }
    public static void DestroyPersistentObjectsLevel()
    {
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.scene.name == "DontDestroyOnLoad")
            {
                if (obj.GetComponent<AuthManager>() != null || obj.name == "Firebase Services")
                    continue;

                Destroy(obj);
            }
        }
    }
    // Called when returning to main menu
    public void Cleanup()
    {
        DestroyPersistentObjectsMM();
    }
    public void OnMainMenu()
    {
        Cleanup();
        SceneManager.LoadScene("UserMenu");
    }


}