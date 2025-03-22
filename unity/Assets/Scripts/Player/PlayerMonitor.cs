using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System;
public class PlayerMonitor : MonoBehaviour
{
    public static PlayerMonitor Instance { get; private set; }
    public AffinityDatabase affinityDatabase;
    public SpriteRenderer wizardHat;
    public SpriteRenderer wizardStaff;

    private PlayerState currentState = PlayerState.None;
    private bool isAttacking = false;
    private float attackStateTimer = 0f;
    private string staffAfterState = "";

    private List<Animator> wizardAnimators = new List<Animator>();
    private Animator wizardStaffAnimator;
    private Animator wizardShadowAnimator;

    private List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();
    private Dictionary<SpriteRenderer, Color> originalColors = new Dictionary<SpriteRenderer, Color>();

    public bool playerDead = false;
    private int currGold;
    private float currHealth;
    private int currMana;
    private StatsService.PlayerStats staticPlayerStats;
    private List<int> staticSpellSlots;
    public PlayerStats currPlayerStats;

    public SpellDatabase spellDatabase;
    private float attackDuration = 0.5f;

    private SpriteRenderer wizardStaffRenderer;
    private float staffGlowIntensity = 5f;
    private float staffGlowDuration = 0.5f;
    private float particleGlowIntensity = 5f;

    public int GetGold()
    {
        return currGold;
    }
    public void SpendGold(int amount)
    {
        currGold -= amount;
    }
    public void AddGold(int amount)
    {
        currGold += amount;
    }

    // Reset the player monitor state to its initial values
    public void ResetPlayerState()
    {
        // Get player stats from the backend as a reference point for players stats
        staticPlayerStats = CampaignManager.Instance.currPlayerStats;
        staticSpellSlots = CampaignManager.Instance.currSpellSlots;
        currPlayerStats = new PlayerStats()
        {
            HP = 100,
            Mana = 100,
            Gold = staticPlayerStats.mana,
            Affinity = MapAffinity(staticPlayerStats.affinity),
            Slots = staticSpellSlots,
        };
        isAttacking = false;
        attackStateTimer = 0f;
        staffAfterState = "";
        playerDead = false;
        currHealth = currPlayerStats.HP;
        currMana = currPlayerStats.Mana;
        currGold = currPlayerStats.Gold;
        currentState = PlayerState.None;
        SetAffinity(currPlayerStats.Affinity);
        foreach (Animator anim in wizardAnimators)
        {
            anim.Rebind();
        }
        wizardStaffAnimator.Rebind();
        wizardShadowAnimator.Rebind();
    }
    // Maps the backend affinity to an enmy PlayerClass
    private static PlayerClass MapAffinity(string affinity)
    {
        if (string.IsNullOrEmpty(affinity))
        {
            return PlayerClass.None;
        }

        if (Enum.TryParse<PlayerClass>(affinity, true, out var result))
        {
            return result;
        }

        return PlayerClass.None;
    }
    public PlayerClass? GetAffinity()
    {
        if (currPlayerStats.Affinity != null)
            return currPlayerStats.Affinity;
        return PlayerClass.None;

    }
    public void SetAffinity(PlayerClass? playerClass)
    {
        if (playerClass == null) playerClass = PlayerClass.None;
        currPlayerStats.Affinity = playerClass;
        Affinity affinity = affinityDatabase.affinityList.Find(a => a.playerClass == playerClass);
        wizardHat.sprite = affinity.hat;
        wizardStaff.sprite = affinity.staff;
        UpdateSpellSlotsForAffinity();
        InterfaceManager.Instance?.DrawSlots(currPlayerStats.Slots);
    }

    // Spell slots for affinities goes like: 0-3 base 4-7 fire base,
    // 8-11 fire advanced, 12-15 water base, 16-19 water advanced,
    // 20-23 earth base, 24-27 earth base, 28-31 air, 32-35 air advanced
    public void UpdateSpellSlotsForAffinity()
    {
        // No affinity check (just 0, 1, 2, 3, 4)
        if (currPlayerStats.Affinity == null || currPlayerStats.Affinity == PlayerClass.None)
        {
            for (int i = 0; i < 4; i++)
            {
                currPlayerStats.Slots[i] = i;
            }
            return;
        }

        // Determine the base values for the current affinity.
        int basicStart = 0, advancedStart = 0;
        switch (currPlayerStats.Affinity ?? PlayerClass.None)
        {
            case PlayerClass.Fire:
                basicStart = 4; advancedStart = 8;
                break;
            case PlayerClass.Water:
                basicStart = 12; advancedStart = 16;
                break;
            case PlayerClass.Earth:
                basicStart = 20; advancedStart = 24;
                break;
            case PlayerClass.Air:
                basicStart = 28; advancedStart = 32;
                break;
            default:
                break;
        }

        for (int i = 0; i < 4; i++)
        {
            bool potency = false;
            switch (i)
            {
                case 0:
                    potency = HasPotencyOne();
                    break;
                case 1:
                    potency = HasPotencyTwo();
                    break;
                case 2:
                    potency = HasPotencyThree();
                    break;
                case 3:
                    potency = HasPotencyFour();
                    break;
            }

            if (potency)
            {
                currPlayerStats.Slots[i] = advancedStart + i;
            }
            else
            {
                currPlayerStats.Slots[i] = basicStart + i;
            }
        }
    }
    // Adds four to get the upgraded spell (position in database + backend)
    public void SetPotencyOne()
    {
        currPlayerStats.Slots[0] += 4;
        InterfaceManager.Instance?.DrawSlots(currPlayerStats.Slots);

    }
    public void SetPotencyTwo()
    {
        currPlayerStats.Slots[1] += 4;
        InterfaceManager.Instance?.DrawSlots(currPlayerStats.Slots);
    }
    public void SetPotencyThree()
    {
        currPlayerStats.Slots[2] += 4;
        InterfaceManager.Instance?.DrawSlots(currPlayerStats.Slots);
    }
    public void SetPotencyFour()
    {
        currPlayerStats.Slots[3] += 4;
        InterfaceManager.Instance?.DrawSlots(currPlayerStats.Slots);
    }
    public bool IsAdvancedSpell(int spellId)
    {
        bool fireAdvanced = spellId >= 8 && spellId < 12;
        bool waterAdvanced = spellId >= 16 && spellId < 20;
        bool earthAdvanced = spellId >= 24 && spellId < 28;
        bool airAdvanced = spellId >= 32 && spellId < 36;
        return fireAdvanced || waterAdvanced || earthAdvanced || airAdvanced;
    }
    public bool HasPotencyOne()
    {

        return IsAdvancedSpell(currPlayerStats.Slots[0]);

    }
    public bool HasPotencyTwo()
    {
        return IsAdvancedSpell(currPlayerStats.Slots[1]);

    }
    public bool HasPotencyThree()
    {
        return IsAdvancedSpell(currPlayerStats.Slots[2]);

    }
    public bool HasPotencyFour()
    {
        return IsAdvancedSpell(currPlayerStats.Slots[3]);

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
        // Respectively assign to animator and sprite renderer
        foreach (Transform child in transform)
        {
            GameObject go = child.gameObject;
            SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                spriteRenderers.Add(sr);
                originalColors[sr] = sr.color;
            }

            Animator anim = go.GetComponent<Animator>();
            if (go.name == "WizardStaff")
            {
                wizardStaffAnimator = anim;
                wizardStaffRenderer = sr;
            }
            else if (go.name == "WizardShadow")
            {
                wizardShadowAnimator = anim;
            }
            else if (anim != null)
            {
                wizardAnimators.Add(anim);
            }
        }
        ResetPlayerState();
        InterfaceManager.Instance?.DrawSlots(currPlayerStats.Slots);
    }

    public List<int> GetPlayerSlots()
    {
        return currPlayerStats.Slots;
    }
    public void BoostMana(int amount)
    {
        currMana += amount;
        currMana = Mathf.Clamp(currMana, 0, currPlayerStats.Mana);
        InterfaceManager.Instance.DrawMana(currPlayerStats.Mana, currMana);
    }
    public void CastSpell(int slotId)
    {
        if (playerDead || GameManager.Instance.IsFrozen) return;

        // Find spell
        SpellData spellData = spellDatabase.spellList.Find(s => s.id == currPlayerStats.Slots[slotId]);
        if (spellData == null)
        {
            Debug.LogError("Spell with ID " + slotId + " not found in the database!");
            return;
        }

        // Check if sufficient mana
        if (currMana - spellData.manaCost < 0)
        {
            InterfaceManager.Instance.FlashOOM();
            return;
        }

        currMana -= spellData.manaCost;
        currMana = Mathf.Clamp(currMana, 0, currPlayerStats.Mana);
        InterfaceManager.Instance.DrawMana(currPlayerStats.Mana, currMana);

        InterfaceManager.Instance.SpellCooldown(slotId, 2f);

        // Fire the attack animation
        isAttacking = true;
        attackStateTimer = 0;
        SetStaffAnim("IsAttack");
        // If in boss fight hide spells
        if (LevelManager.Instance.bossCSFlag)
        {
            InterfaceManager.Instance.HideSpells();
        }
        // Spawn the spell
        Quaternion prefabRotation = spellData.prefab.transform.rotation;
        Vector3 prefabPosition = spellData.prefab.transform.position;
        Vector3 spawnPosition = new Vector3(
            transform.position.x,
            prefabPosition.y,
            prefabPosition.z
        );
        GameObject spellObject = Instantiate(
            spellData.prefab,
            spawnPosition,
            prefabRotation
        );

        ParticleSystemRenderer psRenderer = spellObject.GetComponent<ParticleSystemRenderer>();
        if (psRenderer != null)
        {
            // Clone the material so only this spell instance is affected
            psRenderer.material = new Material(psRenderer.material);

            // Enable emission (if the shader requires it)
            psRenderer.material.EnableKeyword("_EMISSION");

            // Compute the final color based on Affinity and intensity
            Color finalParticleColor = GetAffinityColor(currPlayerStats.Affinity) * particleGlowIntensity;
            psRenderer.material.SetColor("_EmissionColor", finalParticleColor);
        }

        // Pass spell's attributes
        SpellMonitor spellMonitor = spellObject.AddComponent<SpellMonitor>();
        spellMonitor.Damage = spellData.damage;
        spellMonitor.Speed = spellData.speed;
        spellMonitor.Airborne = spellData.airborne;

    }

    public void OnHit(float damage)
    {
        if (playerDead) return;
        StartCoroutine(FlashCoroutine());

        currHealth -= damage;
        currHealth = Mathf.Clamp(currHealth, 0, currPlayerStats.HP);
        InterfaceManager.Instance.DrawHealth(currPlayerStats.HP, currHealth);
        if (currHealth <= 0 && !playerDead)
        {
            WizardDeath();
        }
    }

    private IEnumerator FlashCoroutine()
    {
        foreach (SpriteRenderer sr in spriteRenderers)
        {
            sr.color = Color.red;
        }
        yield return new WaitForSeconds(0.2f);
        foreach (SpriteRenderer sr in spriteRenderers)
        {
            sr.color = originalColors[sr];
        }
    }

    public void SetState(PlayerState newState)
    {
        if (currentState == newState) return;
        currentState = newState;
        switch (currentState)
        {
            case PlayerState.Idle:
                Debug.Log("Player is Idle");
                SetWizardAnim("IsIdle");
                if (!isAttacking) SetStaffAnim("IsIdle");
                staffAfterState = "IsIdle";
                break;
            case PlayerState.Run:
                Debug.Log("Player is Running");
                SetWizardAnim("IsRun");
                if (!isAttacking) SetStaffAnim("IsRun");
                staffAfterState = "IsRun";
                break;

        }
    }
    void Update()
    {
        if (GameManager.Instance.IsFrozen)
        {
            return;
        }
        // Attacking duration.
        if (isAttacking)
        {
            attackStateTimer += Time.deltaTime;
            if (attackStateTimer >= attackDuration)
            {
                Debug.Log("Attack finished, returning to: " + staffAfterState);
                SetStaffAnim(staffAfterState);
                isAttacking = false;
            }
        }
    }
    void SetStaffAnim(string anim)
    {
        wizardStaffAnimator.SetBool("IsIdle", false);
        wizardStaffAnimator.SetBool("IsRun", false);
        wizardStaffAnimator.SetBool("IsAttack", false);
        wizardStaffAnimator.SetBool(anim, true);

        // If attacking, animate the staff's glow
        if (anim == "IsAttack" && wizardStaffRenderer != null)
        {
            // Ensure the staff's material supports emission
            wizardStaffRenderer.material.EnableKeyword("_EMISSION");

            // Calculate the emission color from the player's Affinity
            Color targetEmission = GetAffinityColor(currPlayerStats.Affinity) * staffGlowIntensity;

            // Animate to the target color, then back to black
            wizardStaffRenderer.material.DOColor(targetEmission, "_EmissionColor", staffGlowDuration)
                .OnComplete(() =>
                {
                    wizardStaffRenderer.material.DOColor(Color.black, "_EmissionColor", staffGlowDuration);
                });
        }
    }
    void SetWizardAnim(string anim)
    {
        foreach (Animator wizAnim in wizardAnimators)
        {
            wizAnim.SetBool("IsIdle", false);
            wizAnim.SetBool("IsRun", false);
            wizAnim.SetBool(anim, true);
        }
    }
    void WizardDeath()
    {
        CampaignManager.Instance.GameOver();
        InterfaceManager.Instance.DrawGameOver();
        foreach (Animator wizAnim in wizardAnimators)
        {
            wizAnim.SetTrigger("Death");
        }
        wizardStaffAnimator.SetTrigger("Death");
        wizardShadowAnimator.SetTrigger("Death");
        playerDead = true;
    }
    private Color GetAffinityColor(PlayerClass? affinity)
    {
        switch (affinity)
        {
            case PlayerClass.Fire:
                // Fiery red/orange
                return new Color(1f, 0, 0f);
            case PlayerClass.Water:
                // Sky blue
                return new Color(0.53f, 0.81f, 0.98f);
            case PlayerClass.Earth:
                // Green light
                return new Color(0.5f, 1f, 0.5f);
            case PlayerClass.Air:
                // Lightning purple
                return new Color(0.7f, 0.3f, 1f);
            default:
                return Color.white;
        }
    }
}