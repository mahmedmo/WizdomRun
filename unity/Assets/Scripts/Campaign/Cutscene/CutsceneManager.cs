using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager Instance { get; private set; }
    public CutsceneDatabase cutsceneDatabase;
    public CSTilemapDatabase csTilemapDatabase;
    public BossDatabase bossDatabase;
    public AllocatedCutscene allocatedCutscene;
    public Tilemap spawnTilemap;
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

    // Cutscene router (based on type)
    public void StartCutscene(Cutscene cutscene)
    {
        switch (cutscene.cutsceneType)
        {
            case CutsceneType.Start:
                DialogueManager.Instance.SetDialogue(cutscene.dialogue, cutscene.cutsceneType);
                break;
            case CutsceneType.Boss:
            case CutsceneType.Elementalist:
                SpawnCSTilemapForCutscene(cutscene);
                if (cutscene.cutsceneType == CutsceneType.Boss) SpawnBossFromCutscene(cutscene);
                break;
            case CutsceneType.Saved:
                DialogueManager.Instance.SetDialogue(cutscene.dialogue, cutscene.cutsceneType);
                break;
            default:
                Debug.Log("Not a valid cutscene.");
                break;
        }
    }
    // Destroys all spawned tilemaps
    public void DestroyAllChildren()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
    // Boss/Elementalist cutscenes have a tilemap assosciated to each of them,
    // this function instantiates them by associating the tilemap in csTilemapDatabase
    private void SpawnCSTilemapForCutscene(Cutscene cutscene)
    {
        // Calculate spawn position.
        spawnTilemap = GameObject.Find("LOverlay01").GetComponent<Tilemap>();
        int spawnX = spawnTilemap.origin.x + spawnTilemap.size.x / 2;
        int spawnY = spawnTilemap.origin.y + spawnTilemap.size.y + 10;
        Vector3 spawnPos = spawnTilemap.CellToWorld(new Vector3Int(spawnX, spawnY, 0));
        spawnPos.z = -4;
        // Find the appropriate CSTilemap.
        CSTilemap csTilemap = FindCSTilemap(cutscene.cutsceneId);

        // Instantiate the tilemap and set its parent.
        GameObject csSpawn = Instantiate(csTilemap.tilemap, spawnPos, Quaternion.identity);
        csSpawn.transform.parent = this.transform;

        // Attach a CSTilemapMonitor and assign its type.
        CSTilemapMonitor csMonitor = csSpawn.AddComponent<CSTilemapMonitor>();
        csMonitor.csType = cutscene.cutsceneType;
    }

    // Boss cutscenes have a boss enemy/prefab associated with them
    // this function instantiates that prefab, coordinating is position
    // to the center of the tilemap
    public void SpawnBossFromCutscene(Cutscene cutscene)
    {
        spawnTilemap = GameObject.Find("LOverlay01").GetComponent<Tilemap>();
        int campaignLevel = cutscene.campaignLevel;

        // Find the boss in the bossDatabase that matches the campaign level.
        Boss bossToSpawn = bossDatabase.bossList.Find(b => b.campaignLevel == campaignLevel);

        // Determine the center of the spawnTilemap.
        int centerX = spawnTilemap.origin.x + spawnTilemap.size.x / 2;
        int centerY = spawnTilemap.origin.y + spawnTilemap.size.y + 14;
        Vector3 centerPos = spawnTilemap.CellToWorld(new Vector3Int(centerX, centerY, 0));

        // Z value that ensures the boss is visible on top of the tilemap
        centerPos.z = -6;

        // Instantiate the boss prefab at the center position.
        GameObject bossInstance = Instantiate(bossToSpawn.prefab, centerPos, Quaternion.identity);
        bossInstance.transform.parent = this.transform;

        // Attach the BossMonitor script (tracks boss-specific functions)
        BossMonitor bossMonitor = bossInstance.AddComponent<BossMonitor>();
        bossMonitor.Damage = bossToSpawn.damage;
        bossMonitor.Health = bossToSpawn.health;
        bossMonitor.GoldDrop = bossToSpawn.goldDrop;
        bossMonitor.Airborne = bossToSpawn.airborne;
        bossMonitor.tilemap = spawnTilemap;

    }

    // Returns the associated cutscene tilemap
    public CSTilemap FindCSTilemap(int cutsceneId)
    {
        return csTilemapDatabase.cutsceneAreas.Find(csT => csT.cutsceneId == cutsceneId);
    }

    // Finds the Start cutscene for the current campaign level in cutsceneDatabase
    // Checks for a tutorial encounter for when the user is playing
    // for the first time
    public Cutscene FindStartScene(bool firstEnc)
    {
        int campaignLevel = CampaignManager.Instance.GetLevel();
        Cutscene startScene;
        bool tutorialEncounter = CampaignManager.Instance.GetRemainingTries() <= 2 && campaignLevel == 1;
        if (tutorialEncounter)
        {
            startScene = cutsceneDatabase.cutsceneList.Find(cs =>
           cs.cutsceneType == CutsceneType.Start && cs.campaignLevel == campaignLevel && cs.firstEncounter);
            return startScene;
        }

        startScene = cutsceneDatabase.cutsceneList.Find(cs =>
        cs.cutsceneType == CutsceneType.Start && cs.campaignLevel == campaignLevel && !cs.firstEncounter);
        return startScene;
    }

    // Finds the Elementalist cutscene in the cutsceneDatabase
    public Cutscene FindEShopScene()
    {
        Cutscene eScene;
        eScene = cutsceneDatabase.cutsceneList.Find(cs =>
        cs.cutsceneType == CutsceneType.Elementalist);
        return eScene;

    }

    // Finds the Saved cutscene in the cutsceneDatabase for the current level
    public Cutscene FindSavedScene()
    {
        int campaignLevel = CampaignManager.Instance.GetLevel();
        Cutscene savedScene = cutsceneDatabase.cutsceneList.Find(cs =>
            cs.cutsceneType == CutsceneType.Saved && cs.campaignLevel == campaignLevel);
        return savedScene;
    }

    // Finds the Boss cutscene in the cutsceneDatabase for the current level
    public Cutscene FindBossScene()
    {
        int campaignLevel = CampaignManager.Instance.GetLevel();
        Cutscene bossScene = cutsceneDatabase.cutsceneList.Find(cs =>
            cs.cutsceneType == CutsceneType.Boss && cs.campaignLevel == campaignLevel);
        return bossScene;
    }

    // "Allocates" an Elementalist cutscene from the database and attributes a randomized spawn point for it
    public void AllocateCutscenes()
    {
        List<Cutscene> elementalistCutscenes = cutsceneDatabase.cutsceneList.FindAll(cs => cs.cutsceneType == CutsceneType.Elementalist);
        Cutscene chosenCutscene = elementalistCutscenes[Random.Range(0, elementalistCutscenes.Count)];
        // Generate a random spawn progress between 300 and 700 (30 - 70% level progress)
        float spawnProgress = Random.Range(300f, 700f);

        AllocatedCutscene allocated = new AllocatedCutscene() { cutscene = chosenCutscene, spawnProgress = spawnProgress };
        allocatedCutscene = allocated;
        Debug.Log("cutsceneID: " + allocatedCutscene.cutscene.cutsceneId);

    }

}