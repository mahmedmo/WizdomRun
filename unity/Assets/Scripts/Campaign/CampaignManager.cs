using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class CampaignManager : MonoBehaviour
{
    public static CampaignManager Instance { get; private set; }

    // Global access to the campaign centric fields to track player progress
    public Campaign currCampaign;
    public StatsService.PlayerStats currPlayerStats;
    public List<int> currSpellSlots;

    private CampaignService campaignService;
    private StatsService statsService;
    private QuestionService questionService;
    private bool gameOverProcessed = false;
    private bool nextLevelProcessing = false;
    private GameObject AppLoad;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            campaignService = GetComponent<CampaignService>();
            statsService = GetComponent<StatsService>();
            questionService = GetComponent<QuestionService>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Calls an async function that updates the remaining tries left for the player.
    public void GameOver()
    {
        if (gameOverProcessed)
        {
            return;
        }
        gameOverProcessed = true;
        currCampaign.RemainingTries -= 1;
        StartCoroutine(GameOverRoutine());
    }

    private IEnumerator GameOverRoutine()
    {
        bool updateSuccess = false;
        yield return StartCoroutine(campaignService.UpdateCampaign(
            currCampaign.CampaignID,
            currCampaign.CurrLevel,
            currCampaign.RemainingTries,
            AuthManager.Instance.AuthToken,
            () => { updateSuccess = true; },
            (error) => { Debug.LogError(error); }
        ));
        if (updateSuccess)
        {
            Debug.Log("Campaign updated with new remaining tries: " + currCampaign.RemainingTries);
            gameOverProcessed = false;
        }
    }

    public int GetLevel()
    {
        int currLevel = currCampaign.CurrLevel;
        if (currCampaign.CampaignLength == CampaignLength.QUEST && currLevel == 3)
        {
            currLevel = 10;
        }
        else if (currCampaign.CampaignLength == CampaignLength.ODYSSEY && currLevel == 5)
        {
            currLevel = 10;
        }
        return currLevel;
    }

    public int GetRemainingTries()
    {
        return currCampaign.RemainingTries;
    }

    private void SetLoad(bool visible)
    {
        AppLoad = null;
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject go in allObjects)
        {
            if (go.name == "AppLoad")
            {
                AppLoad = go;
                break;
            }
        }
        if (AppLoad != null)
        {
            AppLoad.SetActive(visible);
        }
    }
    // Calls an async function that retrieves player's progress
    // and saves it to the backend, as well as updating the curr campaign level
    // lastly calls the loading of the next campaign level.
    public void NextCampaignLevel()
    {
        if (nextLevelProcessing)
        {
            return;
        }
        SetLoad(true);
        nextLevelProcessing = true;
        StartCoroutine(NextCampaignLevelRoutine());
    }

    private IEnumerator NextCampaignLevelRoutine()
    {

        PlayerStats monitorStats = PlayerMonitor.Instance.currPlayerStats;
        List<int> monitorSlots = monitorStats.Slots;

        currPlayerStats = new StatsService.PlayerStats
        {
            mana = monitorStats.Gold,

            // Matching enum affinity to backend's expected value
            affinity = monitorStats.Affinity.HasValue && monitorStats.Affinity.Value != PlayerClass.None
                         ? monitorStats.Affinity.Value.ToString().ToLower()
                         : null,
        };

        currSpellSlots = new List<int>(monitorSlots);

        currCampaign.CurrLevel += 1;

        bool updateStatsSuccess = false;
        yield return StartCoroutine(statsService.UpdatePlayerStats(
            currCampaign.CampaignID,
            currPlayerStats.mana,
            currPlayerStats.affinity,
            AuthManager.Instance.AuthToken,
            () => { updateStatsSuccess = true; },
            (error) => { Debug.LogError(error); }
        ));
        if (!updateStatsSuccess) yield break;

        bool assignSpellsSuccess = false;
        yield return StartCoroutine(statsService.AssignSpells(
            currCampaign.CampaignID,
            monitorSlots,
            AuthManager.Instance.AuthToken,
            () => { assignSpellsSuccess = true; },
            (error) => { Debug.LogError(error); }
        ));
        if (!assignSpellsSuccess) yield break;

        bool updateCampaignSuccess = false;
        yield return StartCoroutine(campaignService.UpdateCampaign(
            currCampaign.CampaignID,
            currCampaign.CurrLevel,
            currCampaign.RemainingTries,
            AuthManager.Instance.AuthToken,
            () => { updateCampaignSuccess = true; },
            (error) => { Debug.LogError(error); }
        ));
        if (!updateCampaignSuccess) yield break;

        foreach (var question in currCampaign.QuestionList)
        {
            if (question.GotCorrect == 1)
            {
                yield return StartCoroutine(questionService.AnswerQuestion(
                    question.QuestionID,
                    true,
                    AuthManager.Instance.AuthToken,
                    () => { Debug.Log("Question " + question.QuestionID + " updated in backend."); },
                    (error) => { Debug.LogError("Error updating question " + question.QuestionID + ": " + error); }
                ));
            }
        }
        nextLevelProcessing = false;
        LoadMappedLevel();
    }

    // Maps level integers to respective Unity scenes
    public void LoadMappedLevel()
    {
        // Length handler for smaller campaigns
        int currLevel = currCampaign.CurrLevel;
        if (currCampaign.CampaignLength == CampaignLength.QUEST && currLevel == 3)
        {
            currLevel = 10;
        }
        else if (currCampaign.CampaignLength == CampaignLength.ODYSSEY && currLevel == 5)
        {
            currLevel = 10;
        }

        if (currCampaign.CampaignLength == CampaignLength.QUEST && currLevel == 4)
        {
            currLevel = 1;
        }
        else if (currCampaign.CampaignLength == CampaignLength.ODYSSEY && currLevel == 6)
        {
            currLevel = 1;
        }
        if (currLevel == 11) currLevel = 1;

        switch (currLevel)
        {
            case 1:
                SceneManager.LoadScene("HFLevel");
                break;
            case 2:
                SceneManager.LoadScene("BeachLevel");
                break;
            case 3:
                SceneManager.LoadScene("DesertLevel");
                break;
            case 4:
                SceneManager.LoadScene("CaveLevel");
                break;
            case 5:
                SceneManager.LoadScene("FrozenLevel");
                break;
            case 6:
                SceneManager.LoadScene("GraveLevel");
                break;
            case 7:
                SceneManager.LoadScene("LavaLevel");
                break;
            case 8:
                SceneManager.LoadScene("SkyLevel");
                break;
            case 9:
                SceneManager.LoadScene("EnchantedLevel");
                break;
            case 10:
                SceneManager.LoadScene("CorruptionLevel");
                break;
            default:
                GameManager.Instance.OnMainMenu();
                break;
        }
    }

    // Creates a new campaign and subsequent player progress fields (PlayerStats and PlayerSpells)
    public IEnumerator CreateNewCampaign(string userID, string title, string campaignLength, int startingLevel)
    {
        yield return campaignService.CreateCampaign(
            userID,
            title,
            campaignLength,
            startingLevel,
            AuthManager.Instance.AuthToken,
            (CampaignService.Campaign csCampaign) =>
            {
                currCampaign = CampaignAdapter(csCampaign);
                Debug.Log("Campaign created successfully");
            },
            (string error) =>
            {
                Debug.LogError(error);
            }
        );

        if (currCampaign == null) yield break;

        bool createStatsSuccess = false;
        yield return StartCoroutine(statsService.CreatePlayerStats(
            currCampaign.CampaignID,
            1.0f,
            100,
            500,
            null,
            AuthManager.Instance.AuthToken,
            () =>
            {
                createStatsSuccess = true;
                Debug.Log("Player stats created successfully");
            },
            (string error) =>
            {
                Debug.LogError(error);
            }
        ));
        if (!createStatsSuccess) yield break;

        bool assignSpellsSuccess = false;
        List<int> defaultSpellIDs = new List<int> { 0, 1, 2, 3 };
        yield return StartCoroutine(statsService.AssignSpells(
            currCampaign.CampaignID,
            defaultSpellIDs,
            AuthManager.Instance.AuthToken,
            () =>
            {
                assignSpellsSuccess = true;
                Debug.Log("Spells assigned successfully.");
            },
            (string error) =>
            {
                Debug.LogError(error);
            }
        ));
        if (!assignSpellsSuccess) yield break;


        yield return StartCoroutine(LoadPlayerStatsRoutine(currCampaign.CampaignID, AuthManager.Instance.AuthToken));
        yield return StartCoroutine(LoadSpellSlotsRoutine(currCampaign.CampaignID, AuthManager.Instance.AuthToken));

    }

    // Gets and sets the campaign to currCampaign
    public IEnumerator GetCampaignAndSetCurrent(int campaignID)
    {
        yield return StartCoroutine(campaignService.GetCampaign(
            campaignID,
            AuthManager.Instance.AuthToken,
            (CampaignService.Campaign csCampaign) =>
            {
                currCampaign = CampaignAdapter(csCampaign);
                Debug.Log("Campaign loaded successfully");
            },
            (string error) =>
            {
                Debug.LogError("Error retrieving campaign: " + error);
            }
        ));
        yield return StartCoroutine(LoadPlayerStatsRoutine(campaignID, AuthManager.Instance.AuthToken));
        yield return StartCoroutine(LoadSpellSlotsRoutine(campaignID, AuthManager.Instance.AuthToken));
    }

    // Calls an async function to restart the campaign from scratch (on no more tries left)
    public void RestartCampaign()
    {
        StartCoroutine(RestartCampaignRoutine());
    }

    private IEnumerator RestartCampaignRoutine()
    {
        int campaignID = currCampaign.CampaignID;
        string token = AuthManager.Instance.AuthToken;

        bool updateStatsSuccess = false;
        yield return StartCoroutine(statsService.UpdatePlayerStats(
            campaignID,
            0,
            null,
            token,
            () => { updateStatsSuccess = true; },
            (error) => { Debug.LogError(error); }
        ));
        if (!updateStatsSuccess) yield break;

        bool assignSpellsSuccess = false;
        List<int> spellIDs = new List<int> { 0, 1, 2, 3 };
        yield return StartCoroutine(statsService.AssignSpells(
            campaignID,
            spellIDs,
            token,
            () => { assignSpellsSuccess = true; },
            (error) => { Debug.LogError(error); }
        ));
        if (!assignSpellsSuccess) yield break;

        bool updateCampaignSuccess = false;
        yield return StartCoroutine(campaignService.UpdateCampaign(
            campaignID,
            1,
            3,
            token,
            () => { updateCampaignSuccess = true; },
            (error) => { Debug.LogError(error); }
        ));
        if (!updateCampaignSuccess) yield break;

        bool getCampaignSuccess = false;
        yield return StartCoroutine(campaignService.GetCampaign(
            campaignID,
            token,
            (CampaignService.Campaign csCampaign) =>
            {
                currCampaign = CampaignAdapter(csCampaign);
                getCampaignSuccess = true;
            },
            (error) => { Debug.LogError(error); }
        ));
        if (!getCampaignSuccess) yield break;

        yield return StartCoroutine(LoadPlayerStatsRoutine(campaignID, token));
        yield return StartCoroutine(LoadSpellSlotsRoutine(campaignID, token));

        LoadMappedLevel();
    }

    // Retrieves the PlayerStats from the current campaign
    public void LoadPlayerStats()
    {
        if (currCampaign == null)
        {
            Debug.LogError("Current campaign is null. Cannot load player stats.");
            return;
        }

        StartCoroutine(LoadPlayerStatsRoutine(currCampaign.CampaignID, AuthManager.Instance.AuthToken));
    }

    private IEnumerator LoadPlayerStatsRoutine(int campaignID, string token)
    {
        bool getStatsSuccess = false;
        yield return StartCoroutine(statsService.GetPlayerStats(
            campaignID,
            token,
            (StatsService.PlayerStats stats) =>
            {
                currPlayerStats = stats;
                getStatsSuccess = true;
                Debug.Log("Player stats loaded successfully.");
            },
            (string error) =>
            {
                Debug.LogError(error);
            }
        ));
        if (!getStatsSuccess) yield break;
    }
    public void LoadSpellSlots()
    {
        if (currCampaign == null)
        {
            Debug.LogError("Current campaign is null. Cannot load spell slots.");
            return;
        }
        StartCoroutine(LoadSpellSlotsRoutine(currCampaign.CampaignID, AuthManager.Instance.AuthToken));
    }

    private IEnumerator LoadSpellSlotsRoutine(int campaignID, string token)
    {
        bool getSpellsSuccess = false;
        yield return StartCoroutine(statsService.GetPlayerSpells(
            campaignID,
            token,
            (List<int> spells) =>
            {
                currSpellSlots = spells;
                getSpellsSuccess = true;
                Debug.Log("Spell slots loaded successfully.");
            },
            (string error) =>
            {
                Debug.LogError(error);
            }
        ));
        if (!getSpellsSuccess) yield break;
    }
    public void ResetCampaign()
    {
        currCampaign = null;
    }

    // Adapter method that converts backend entity to frontend's Campaign
    private Campaign CampaignAdapter(CampaignService.Campaign csCampaign)
    {
        Campaign campaign = new Campaign();
        campaign.CampaignID = csCampaign.campaignID;
        campaign.LastUpdated = csCampaign.lastUpdated;
        campaign.UserID = csCampaign.userID;
        campaign.Title = csCampaign.title;
        campaign.CampaignLength = (CampaignLength)System.Enum.Parse(typeof(CampaignLength), csCampaign.campaignLength, true);
        campaign.CurrLevel = csCampaign.currLevel;
        campaign.RemainingTries = csCampaign.remainingTries;
        campaign.AchievementList = new List<Achievement>();
        campaign.QuestionList = new List<Question>();
        return campaign;
    }
}