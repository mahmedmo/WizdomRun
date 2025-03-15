using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class PlayerMonitor : MonoBehaviour
{
    public static PlayerMonitor Instance { get; private set; }
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
    // Player Observations
    private PlayerStats playerStats = new PlayerStats()
    {
        HP = 100,
        Mana = 100,
        Affinity = PlayerClass.Fire,
        UnlockedList = new HashSet<int> { 0, 1, 2, 3 },
        Slots = new int[] { 2, 3, -1, -1 }
    };
    private float currHealth;
    private int currMana;

    public SpellDatabase spellDatabase;
    private float attackDuration = 0.5f;

    private SpriteRenderer wizardStaffRenderer;
    private float staffGlowIntensity = 5f;
    private float staffGlowDuration = 0.5f;
    private float particleGlowIntensity = 5f;

    // Reset the player monitor state to its initial values
    public void ResetPlayerState()
    {
        currentState = PlayerState.None;
        isAttacking = false;
        attackStateTimer = 0f;
        staffAfterState = "";
        playerDead = false;
        currHealth = playerStats.HP;
        currMana = playerStats.Mana;
        foreach (Animator anim in wizardAnimators)
        {
            anim.Rebind();
        }
        wizardStaffAnimator.Rebind();
        wizardShadowAnimator.Rebind();
        Debug.Log("PlayerMonitor state reset!");
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Respectively assign to animator and sprite renderer
            foreach (Transform child in transform)
            {
                GameObject go = child.gameObject;
                SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
                spriteRenderers.Add(sr);
                originalColors[sr] = sr.color;

                Animator anim = go.GetComponent<Animator>();
                if (go.name == "WizardStaff")
                {
                    wizardStaffAnimator = anim;
                    wizardStaffRenderer = sr;

                }
                else if (go.name == "WizardShadow")
                {
                    wizardShadowAnimator = anim;
                    continue;
                }
                else
                {
                    wizardAnimators.Add(anim);
                }
            }
            ResetPlayerState();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InterfaceManager.Instance?.DrawSlots(playerStats.Slots);
    }

    public int[] GetPlayerSlots()
    {
        return playerStats.Slots;
    }
    public void CastSpell(int slotId)
    {
        if (playerDead) return;

        // Find spell
        SpellData spellData = spellDatabase.spellList.Find(s => s.id == playerStats.Slots[slotId]);
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
        currMana = Mathf.Clamp(currMana, 0, playerStats.Mana);
        InterfaceManager.Instance.DrawMana(playerStats.Mana, currMana);

        InterfaceManager.Instance.SpellCooldown(slotId, 2f);

        // Fire the attack animation
        isAttacking = true;
        attackStateTimer = 0;
        SetStaffAnim("IsAttack");

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
            Color finalParticleColor = GetAffinityColor(playerStats.Affinity) * particleGlowIntensity;
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
        currHealth = Mathf.Clamp(currHealth, 0, playerStats.HP);
        InterfaceManager.Instance.DrawHealth(playerStats.HP, currHealth);
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
        // Attacking  duration.
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
            Color targetEmission = GetAffinityColor(playerStats.Affinity) * staffGlowIntensity;

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
        InterfaceManager.Instance?.DrawGameOver();
        foreach (Animator wizAnim in wizardAnimators)
        {
            wizAnim.SetTrigger("Death");
        }
        wizardStaffAnimator.SetTrigger("Death");
        wizardShadowAnimator.SetTrigger("Death");
        playerDead = true;
    }
    private Color GetAffinityColor(PlayerClass affinity)
    {
        switch (affinity)
        {
            case PlayerClass.Fire:
                // Fiery red/orange
                return new Color(1f, 0.45f, 0f);
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