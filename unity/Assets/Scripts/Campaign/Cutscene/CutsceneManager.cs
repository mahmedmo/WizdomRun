using UnityEngine;
using System.Collections.Generic;
public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager Instance { get; private set; }
    // Assign the CutsceneDatabase via the Inspector.
    public CutsceneDatabase cutsceneDatabase;
    public List<AllocatedCutscene> allocatedCutscenes = new List<AllocatedCutscene>();
    public Dictionary<CutsceneType, int> allocatedTypeCounts = new Dictionary<CutsceneType, int>();

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
    /// <summary>
    /// Finds the Start cutscene for the current campaign level.
    /// </summary>
    /// <returns>The Start cutscene if found; otherwise, null.</returns>
    public Cutscene FindStartScene(bool firstEnc)
    {
        int campaignLevel = LevelManager.Instance.campaignLevel;
        Cutscene startScene;
        if (firstEnc)
        {
            startScene = cutsceneDatabase.cutsceneList.Find(cs =>
        cs.cutsceneType == CutsceneType.Start && cs.campaignLevel == campaignLevel && cs.firstEncounter);
            return startScene;
        }

        startScene = cutsceneDatabase.cutsceneList.Find(cs =>
        cs.cutsceneType == CutsceneType.Start && cs.campaignLevel == campaignLevel && !cs.firstEncounter);
        return startScene;

    }

    public void StartCutscene(Cutscene cutscene)
    {
        switch (cutscene.cutsceneType)
        {
            case CutsceneType.Start:
                DialogueManager.Instance.SetDialogue(cutscene.dialogue, cutscene.cutsceneType);
                break;
            default:
                Debug.Log("Not a valid cutscene.");
                break;
        }
    }
    /// <summary>
    /// Finds the Boss cutscene for the current campaign level.
    /// </summary>
    /// <returns>The Boss cutscene if found; otherwise, null.</returns>
    public Cutscene FindBossScene()
    {
        int campaignLevel = LevelManager.Instance.campaignLevel;
        Cutscene bossScene = cutsceneDatabase.cutsceneList.Find(cs =>
            cs.cutsceneType == CutsceneType.Boss && cs.campaignLevel == campaignLevel);
        if (bossScene == null)
        {
            Debug.LogError($"No Boss cutscene found for campaign level {campaignLevel}.");
        }
        return bossScene;
    }

    public void AllocateCutscenes()
    {
        List<AllocatedCutscene> result = new List<AllocatedCutscene>();

        // 1. Filter cutscenes for the current campaign level and exclude Start/Boss types.
        List<Cutscene> availableCutscenes = cutsceneDatabase.cutsceneList.FindAll(cs =>
            cs.campaignLevel <= LevelManager.Instance.campaignLevel &&
            cs.cutsceneType != CutsceneType.Start &&
            cs.cutsceneType != CutsceneType.Boss
        );

        // 2. Determine distinct available types.
        List<CutsceneType> availableTypes = new List<CutsceneType>();
        foreach (Cutscene cs in availableCutscenes)
        {
            if (!availableTypes.Contains(cs.cutsceneType))
                availableTypes.Add(cs.cutsceneType);
        }

        if (availableTypes.Count < 3)
        {
            Debug.LogWarning("Not enough distinct cutscene types available for allocation.");
            return;
        }

        // 3. Weighted random selection of 3 distinct types.
        List<CutsceneType> selectedTypes = new List<CutsceneType>();
        // Make a temporary list copy.
        List<CutsceneType> tempTypes = new List<CutsceneType>(availableTypes);
        for (int i = 0; i < 3; i++)
        {
            float totalWeight = 0f;
            Dictionary<CutsceneType, float> weights = new Dictionary<CutsceneType, float>();
            foreach (CutsceneType t in tempTypes)
            {
                int count = 0;
                if (allocatedTypeCounts.ContainsKey(t))
                    count = allocatedTypeCounts[t];
                float weight = 1f / (count + 1f);
                weights[t] = weight;
                totalWeight += weight;
            }
            float r = Random.Range(0f, totalWeight);
            float cumulative = 0f;
            CutsceneType chosenType = tempTypes[0];
            foreach (CutsceneType t in tempTypes)
            {
                cumulative += weights[t];
                if (r <= cumulative)
                {
                    chosenType = t;
                    break;
                }
            }
            selectedTypes.Add(chosenType);
            tempTypes.Remove(chosenType);
            // Update count.
            if (!allocatedTypeCounts.ContainsKey(chosenType))
                allocatedTypeCounts[chosenType] = 1;
            else
                allocatedTypeCounts[chosenType]++;
        }

        // 4. For each selected type, choose a cutscene.
        foreach (CutsceneType t in selectedTypes)
        {
            List<Cutscene> typeCutscenes = availableCutscenes.FindAll(cs => cs.cutsceneType == t);
            // Determine if this is a first encounter for this type.
            bool wantFirstEncounter = (allocatedTypeCounts[t] == 1);
            List<Cutscene> candidates = typeCutscenes.FindAll(cs => cs.firstEncounter == wantFirstEncounter);
            Cutscene chosenCutscene = null;
            if (candidates.Count > 0)
            {
                chosenCutscene = candidates[Random.Range(0, candidates.Count)];
            }
            else
            {
                // Fallback if no candidate matches.
                chosenCutscene = typeCutscenes[Random.Range(0, typeCutscenes.Count)];
            }
            // 5. Assign a spawn progress for this cutscene.
            // For example, choose a random progress value between 200 and 900.
            float spawnProgress = Random.Range(200f, 900f);
            AllocatedCutscene allocated = new AllocatedCutscene() { cutscene = chosenCutscene, spawnProgress = spawnProgress };
            result.Add(allocated);
        }

        // Optionally, sort the allocated cutscenes by spawnProgress so they are in order.
        result.Sort((a, b) => a.spawnProgress.CompareTo(b.spawnProgress));

        allocatedCutscenes = result;
    }
}