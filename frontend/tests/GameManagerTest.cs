using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

// shortened version of enum to testing
public enum PlayerState { Idle, Run }

// Stubs for the other manager clasess GameManager communicates with
public class DummyLevelManager : MonoBehaviour
{
    public static DummyLevelManager Instance;
    public float npcSpawnDelay = 5f;
    public float enemySpawnDelay = 5f;
    public float introDelay = 1f;
    public float levelProgress = 0f;
    public int enemyCount = 1;
    public bool levelStart = true;
    public bool inCutscene = false;
    void Awake() { Instance = this; }
}

public class DummyInterfaceManager : MonoBehaviour
{
    public static DummyInterfaceManager Instance;
    void Awake() { Instance = this; }
    public void HideInterface() { }
    public void FadeIntro() { }
    public void FadeToBlack(float duration, System.Action onComplete) { onComplete(); }
}

public class DummyDialogueManager : MonoBehaviour
{
    public static DummyDialogueManager Instance;
    void Awake() { Instance = this; }
    public void HideChoices() { }
}

public class DummyCutsceneManager : MonoBehaviour
{
    public static DummyCutsceneManager Instance;
    void Awake() { Instance = this; }
    public void AllocateCutscenes() { }
}

public class DummyPlayerMonitor : MonoBehaviour
{
    public static DummyPlayerMonitor Instance;
    public PlayerState state;
    void Awake() { Instance = this; }
    public void SetState(PlayerState newState) { state = newState; }
}

public class DummyCampaignManager : MonoBehaviour
{
    public static DummyCampaignManager Instance;
    void Awake() { Instance = this; }
    public int GetRemainingTries() { return 1; }
    public void RestartCampaign() { }
    public void LoadMappedLevel() { }
}

// UNIT Tests
public class GameManagerTests
{
    GameObject gmObject;
    GameManager gameManager;
    GameObject dummyManagers;

    [SetUp]
    public void Setup()
    {
        dummyManagers = new GameObject("DummyManagers");
        dummyManagers.AddComponent<DummyInterfaceManager>();
        dummyManagers.AddComponent<DummyDialogueManager>();
        dummyManagers.AddComponent<DummyCutsceneManager>();
        dummyManagers.AddComponent<DummyPlayerMonitor>();
        dummyManagers.AddComponent<DummyCampaignManager>();
        dummyManagers.AddComponent<DummyLevelManager>();

        gmObject = new GameObject("GameManager");
        gameManager = gmObject.AddComponent<GameManager>();
        gameManager.ResetGameState();
    }

    [TearDown]
    public void Teardown()
    {
        Object.Destroy(gmObject);
        Object.Destroy(dummyManagers);
    }

    [UnityTest]
    public IEnumerator ResetGameState_ResetsTimersAndStates()
    {
        gameManager.gameTime = 10f;
        gameManager.shownIntro = true;
        gameManager.isPaused = false;
        gameManager.ResetGameState();
        Assert.AreEqual(0f, gameManager.gameTime);
        Assert.IsTrue(gameManager.isPaused);
        Assert.IsFalse(gameManager.shownIntro);
        yield return null;
    }

    [UnityTest]
    public IEnumerator FreezeAndUnFreezeGame_TogglesIsFrozen()
    {
        gameManager.isPaused = false;
        gameManager.FreezeGame();
        Assert.IsTrue(gameManager.isPaused);
        Assert.IsTrue(gameManager.IsFrozen);
        gameManager.UnFreezeGame();
        Assert.IsFalse(gameManager.IsFrozen);
        yield return null;
    }

    [UnityTest]
    public IEnumerator PauseAndResumeMovement_ChangesPauseState()
    {
        gameManager.isPaused = false;
        gameManager.PauseMovement();
        Assert.IsTrue(gameManager.isPaused);
        gameManager.ResumeMovement();
        Assert.IsFalse(gameManager.isPaused);
        yield return null;
    }

    [UnityTest]
    public IEnumerator CanRareSpawn_ReturnsTrueAfterDelay()
    {
        gameManager.ResetGameState();
        bool result = gameManager.CanRareSpawn();
        Assert.IsTrue(result);
        result = gameManager.CanRareSpawn();
        Assert.IsFalse(result);
        gameManager.gameTime += gameManager.rareSpawnDelay;
        result = gameManager.CanRareSpawn();
        Assert.IsTrue(result);
        yield return null;
    }

    [UnityTest]
    public IEnumerator CanNPCSpawn_ReturnsTrueAfterDelay()
    {
        DummyLevelManager.Instance.npcSpawnDelay = 5f;
        gameManager.ResetGameState();
        bool result = gameManager.CanNPCSpawn();
        Assert.IsTrue(result);
        result = gameManager.CanNPCSpawn();
        Assert.IsFalse(result);
        gameManager.gameTime += DummyLevelManager.Instance.npcSpawnDelay;
        result = gameManager.CanNPCSpawn();
        Assert.IsTrue(result);
        yield return null;
    }

    [UnityTest]
    public IEnumerator CanEnemySpawn_ReturnsTrueAfterDelay()
    {
        DummyLevelManager.Instance.enemySpawnDelay = 5f;
        gameManager.ResetGameState();
        bool result = gameManager.CanEnemySpawn();
        Assert.IsTrue(result);
        result = gameManager.CanEnemySpawn();
        Assert.IsFalse(result);
        gameManager.gameTime += DummyLevelManager.Instance.enemySpawnDelay;
        result = gameManager.CanEnemySpawn();
        Assert.IsTrue(result);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ResumeAfterDelay_ResumesMovement()
    {
        gameManager.PauseMovement();
        DummyLevelManager.Instance.enemyCount = 0;
        gameManager.StartCoroutine(gameManager.ResumeAfterDelay(0.1f));
        yield return new WaitForSeconds(0.2f);
        Assert.IsFalse(gameManager.isPaused);
    }
}