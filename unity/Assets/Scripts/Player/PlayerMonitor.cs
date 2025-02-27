using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

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

    private bool playerDead = false;
    // Player Observations
    private PlayerStats playerStats = new PlayerStats()
    {
        HP = 100,
        Affinity = PlayerClass.Air,
        UnlockedList = new HashSet<int> { 0, 1, 2, 3 },
        Slots = new int[] { 2, 3, -1, -1 }
    };
    private int currHealth = 100;

    public SpellDatabase spellDatabase;
    private float attackDuration = 0.5f;

    private SpriteRenderer wizardStaffRenderer;
    [SerializeField] private float staffGlowIntensity = 5f;
    [SerializeField] private float staffGlowDuration = 0.5f;
    [SerializeField] private float particleGlowIntensity = 5f;


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
    public void CastSpell(int slotId)
    {
        if (playerDead) return;
        // Find spell
        SpellData spellData = spellDatabase.spells.Find(s => s.spellID == playerStats.Slots[slotId]);
        if (spellData == null)
        {
            Debug.LogError("Spell with ID " + slotId + " not found in the database!");
            return;
        }

        // Fire the attack animation
        isAttacking = true;
        attackStateTimer = 0;
        setStaffAnim("IsAttack");

        // Spawn the spell
        Quaternion prefabRotation = spellData.spellPrefab.transform.rotation;
        Vector3 prefabPosition = spellData.spellPrefab.transform.position;
        Vector3 spawnPosition = new Vector3(
            transform.position.x,
            prefabPosition.y,
            prefabPosition.z
        );
        GameObject spellObject = Instantiate(
            spellData.spellPrefab,
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

        // Pass damage values
        SpellMonitor spellMonitor = spellObject.GetComponent<SpellMonitor>();
        if (spellMonitor != null)
        {
            spellMonitor.Damage = spellData.spellDamage;
        }
    }

    public void OnHit(int damage)
    {
        if (playerDead) return;
        StartCoroutine(FlashCoroutine());

        currHealth -= damage;
        currHealth = Mathf.Clamp(currHealth, 0, playerStats.HP);
        InterfaceManager.Instance.DrawHealth(playerStats.HP, currHealth);
        if (currHealth <= 0 && !playerDead)
        {
            wizardDeath();
            playerDead = true;
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
                setWizardAnim("IsIdle");
                if (!isAttacking) setStaffAnim("IsIdle");
                staffAfterState = "IsIdle";
                break;
            case PlayerState.Run:
                Debug.Log("Player is Running");
                setWizardAnim("IsRun");
                if (!isAttacking) setStaffAnim("IsRun");
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
                setStaffAnim(staffAfterState);
                isAttacking = false;
            }
        }
    }

    void setStaffAnim(string anim)
    {

        wizardStaffAnimator.SetBool("IsIdle", false);
        wizardStaffAnimator.SetBool("IsRun", false);
        wizardStaffAnimator.SetBool("IsAttack", false);
        wizardStaffAnimator.SetBool(anim, true);
        Debug.Log(anim + " was set to true.");

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

    void setWizardAnim(string anim)
    {
        foreach (Animator wizAnim in wizardAnimators)
        {
            wizAnim.SetBool("IsIdle", false);
            wizAnim.SetBool("IsRun", false);
            wizAnim.SetBool(anim, true);
        }
    }

    void wizardDeath()
    {
        InterfaceManager.Instance?.DrawGameOver();
        foreach (Animator wizAnim in wizardAnimators)
        {
            wizAnim.SetTrigger("Death");
        }
        wizardStaffAnimator.SetTrigger("Death");
        wizardShadowAnimator.SetTrigger("Death");
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