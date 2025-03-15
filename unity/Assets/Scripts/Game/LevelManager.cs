using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    public LevelDatabase levelDatabase;
    public int campaignLength => GetCampaignLength();
    private int _campaignLength = 0;
    public int maxEnemyCount => levelDatabase.levels[levelId].maxEnemyCount;
    public float enemySpawnDelay => levelDatabase.levels[levelId].enemySpawnDelay;
    public float npcSpawnDelay => levelDatabase.levels[levelId].npcSpawnDelay;
    public float enemyDmgMultiplier => levelDatabase.levels[levelId].enemyDmgMultiplier;
    public GameObject intro => levelDatabase.levels[levelId].intro;
    public float introDelay => levelDatabase.levels[levelId].introDelay;
    public int enemyCount { get; set; } = 0;

    public float levelProgress { get; set; } = 0;
    public int campaignLevel { get; set; } = 1;
    public bool firstEncounter { get; set; } = true;
    public bool levelStart { get; set; } = false;
    public int levelId { get; set; } = 0;


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
        levelProgress = 0f;
    }
    public void StartCS()
    {
        DialogueManager.Instance.ShowDialogue();
        DialogueManager.Instance.HideChoices();
        CutsceneManager.Instance.StartCutscene(CutsceneManager.Instance.FindStartScene(firstEncounter));
    }
    public void StartLevel()
    {
        InterfaceManager.Instance.ShowInterface();
        DialogueManager.Instance.HideDialogue();
        DialogueManager.Instance.HideChoices();
        levelStart = true;
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
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            firstEncounter = true;
            DontDestroyOnLoad(this);
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
    void Update()
    {

    }

}
