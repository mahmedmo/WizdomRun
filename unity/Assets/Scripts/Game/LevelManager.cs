using UnityEngine;
using UnityEngine.Tilemaps;
using DG.Tweening;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    public LevelDatabase levelDatabase;
    public BossDatabase bossDatabase;
    public NPCDatabase savedNPCDatabase;

    public int campaignLength => GetCampaignLength();
    private int _campaignLength = 0;
    public int maxEnemyCount => levelDatabase.levels[campaignLevelIdx].maxEnemyCount;
    public float enemySpawnDelay => levelDatabase.levels[campaignLevelIdx].enemySpawnDelay;
    public float npcSpawnDelay => levelDatabase.levels[campaignLevelIdx].npcSpawnDelay;
    public float enemyDmgMultiplier => levelDatabase.levels[campaignLevelIdx].enemyDmgMultiplier;
    public GameObject intro => levelDatabase.levels[campaignLevelIdx].intro;
    public float introDelay => levelDatabase.levels[campaignLevelIdx].introDelay;
    public int enemyCount { get; set; } = 0;
    public float levelProgress { get; set; } = 0;
    public int campaignLevelIdx => CampaignManager.Instance != null ? CampaignManager.Instance.GetLevel() - 1 : 0;
    public bool firstEncounter { get; set; } = true;
    public bool levelStart { get; set; } = false;
    public bool bossCSFlag { get; set; } = false;
    public bool elemCSFlag { get; set; } = false;
    public bool inCutscene { get; set; } = false;
    public bool isPlayerTurn { get; set; } = true;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            firstEncounter = true;
            inCutscene = false;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        ResetLevel();
    }

    // Checks for Cutscenes at specified progress points
    void Update()
    {
        if (inCutscene) return;

        if (levelProgress >= CutsceneManager.Instance.allocatedCutscene.spawnProgress && !elemCSFlag)
        {
            inCutscene = true;
            elemCSFlag = true;
            Debug.Log("ELEMENTALIST SPAWN");
            CutsceneManager.Instance.StartCutscene(CutsceneManager.Instance.allocatedCutscene.cutscene);
        }
        if (levelProgress >= 1000 && !bossCSFlag)
        {
            inCutscene = true;
            bossCSFlag = true;
            Debug.Log("BOSS SPAWN");
            CutsceneManager.Instance.StartCutscene(CutsceneManager.Instance.FindBossScene());
        }
    }

    public void DecrementEnemyCount()
    {
        enemyCount--;
    }
    public void IncrementEnemyCount()
    {
        enemyCount++;
    }
    public void ResetLevel()
    {
        enemyCount = 0;
        levelStart = false;
        bossCSFlag = false;
        elemCSFlag = false;
        inCutscene = false;
        isPlayerTurn = true;
        levelProgress = 0f;
        StartCS();
    }

    // Cutscene Starters
    public void StartCS()
    {
        DialogueManager.Instance.ShowDialogue();
        DialogueManager.Instance.HideChoices();
        CutsceneManager.Instance.StartCutscene(CutsceneManager.Instance.FindStartScene(firstEncounter));
    }
    public void SavedCS()
    {
        DialogueManager.Instance.ShowDialogue();
        DialogueManager.Instance.HideChoices();
        CutsceneManager.Instance.StartCutscene(CutsceneManager.Instance.FindSavedScene());
    }
    public void EShopCS()
    {
        DialogueManager.Instance.ShowDialogue();
        DialogueManager.Instance.HideChoices();
        Cutscene eShopCutscene = CutsceneManager.Instance.FindEShopScene();
        DialogueManager.Instance.SetDialogue(eShopCutscene.dialogue, eShopCutscene.cutsceneType);
    }
    public void StartLevel()
    {
        InterfaceManager.Instance.ShowInterface();
        InterfaceManager.Instance.AnimateStartPopup();
        DialogueManager.Instance.HideDialogue();
        DialogueManager.Instance.HideChoices();

        levelStart = true;
    }

    // Gold Handlers
    public void AddGold(int amount)
    {
        PlayerMonitor.Instance.AddGold(amount);

        InterfaceManager.Instance.AddGold(amount);
    }
    public void SpendGold(int amount)
    {
        PlayerMonitor.Instance.SpendGold(amount);

        InterfaceManager.Instance.SpendGold(amount);
    }
    public void LoadNextLevel()
    {
        CampaignManager.Instance.NextCampaignLevel();
    }

    public void StartBoss()
    {
        InterfaceManager.Instance.AnimateBossPopup();
        GameManager.Instance.PauseMovement();
        string bossName = bossDatabase.bossList.Find(b => b.campaignLevel == CampaignManager.Instance.GetLevel()).name;
        StartCoroutine(ShowBossDataDelayed(bossName));
    }

    public void StartEShop()
    {
        DialogueManager.Instance.HideDialogue();
        DialogueManager.Instance.HideChoices();
        ShopManager.Instance.AnimateShopPopup();
    }
    public void PlayerTurn()
    {
        isPlayerTurn = true;
        InterfaceManager.Instance.PlayerTurn();

    }
    public void EnemyTurn()
    {
        isPlayerTurn = false;
        InterfaceManager.Instance.EnemyTurn();

    }

    // Function to spawn the saved NPC from the bosses defeated position
    public void OnBossDeath()
    {
        InterfaceManager.Instance.HideInterface();
        int npcIndex = CampaignManager.Instance.GetLevel() - 1;
        NPC npcToSpawn = savedNPCDatabase.npcList[npcIndex];

        Vector3 localSpawnPos = new Vector3(-6f, -6.5f, -3f);

        GameObject npc = Instantiate(npcToSpawn.prefab, this.transform);
        npc.transform.localPosition = localSpawnPos;

        npc.AddComponent<BoxCollider2D>();
        npc.AddComponent<NPCSavedMonitor>();

        npc.transform.localScale = Vector3.zero;
        npc.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
    }

    private IEnumerator ShowBossDataDelayed(string bossName)
    {
        yield return new WaitForSeconds(2f);
        InterfaceManager.Instance.ShowBossDataPopIn(bossName);
    }
    private int GetCampaignLength()
    {
        if (_campaignLength != 0) return _campaignLength;
        CampaignLength cl = CampaignLength.SAGA;
        switch (cl)
        {
            case CampaignLength.SAGA:
                _campaignLength = 10;
                break;
            case CampaignLength.ODYSSEY:
                _campaignLength = 5;
                break;
            case CampaignLength.QUEST:
                _campaignLength = 3;
                break;
            default:
                _campaignLength = 10;
                break;
        }
        return _campaignLength;
    }

}
