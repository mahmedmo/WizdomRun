using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class PlayerMonitor : MonoBehaviour
{
    public static PlayerMonitor Instance { get; private set; }
    private PlayerState prevState = PlayerState.Idle;
    private PlayerState currentState = PlayerState.Idle;
    private float stateTimer = 0f;
    private Animator[] spriteAnimators;
    private SpriteRenderer[] spriteRenderers;
    private Dictionary<SpriteRenderer, Color> originalColors = new Dictionary<SpriteRenderer, Color>();

    // Player Observations
    private PlayerStats playerStats;

    private int currHealth = 100;

    public SpellDatabase spellDatabase;
    private float attackDuration = 1f;

    void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            spriteAnimators = GetComponentsInChildren<Animator>();
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

            // Cache the original colors for each SpriteRenderer
            foreach (SpriteRenderer sr in spriteRenderers)
            {
                originalColors[sr] = sr.color;
            }
            playerStats = new PlayerStats() { HP = 100 };
            currentState = PlayerState.Moving;
            SetIdle();

        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CastSpell(int spellId)
    {
        SpellData spellData = spellDatabase.spells.Find(s => s.spellID == spellId);
        if (spellData == null)
        {
            Debug.LogError("Spell with ID " + spellId + " not found in the database!");
            return;
        }

        // Use the prefab's rotation (or Quaternion.identity if you prefer)
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

        // If the SpellMonitor needs the damage value, set it
        SpellMonitor spellMonitor = spellObject.GetComponent<SpellMonitor>();
        if (spellMonitor != null)
        {
            spellMonitor.Damage = spellData.spellDamage;
            // Optionally set any other fields, like SpellSpeed, if you want
            // spellMonitor.SpellSpeed = someValue;
        }
    }
    public void OnHit(int damage)
    {
        prevState = currentState;
        currentState = PlayerState.Hit;
        stateTimer = 0f;

        StartCoroutine(FlashCoroutine());

        currHealth -= damage;
        currHealth = Mathf.Clamp(currHealth, 0, playerStats.HP);
        InterfaceManager.Instance.DrawHealth(playerStats.HP, currHealth);
    }
    private IEnumerator FlashCoroutine()
    {
        // Flash red on all SpriteRenderers
        foreach (SpriteRenderer sr in spriteRenderers)
        {
            sr.color = Color.red;
        }
        yield return new WaitForSeconds(0.2f);

        // Return to original colors
        foreach (SpriteRenderer sr in spriteRenderers)
        {
            sr.color = originalColors[sr];
        }
    }
    public void SetRun()
    {
        if (currentState == PlayerState.Moving) return;
        prevState = currentState;
        currentState = PlayerState.Moving;
        stateTimer = 0f;
        foreach (Animator animator in spriteAnimators)
        {
            animator.SetTrigger("RunTrigger");
        }
    }

    public void SetIdle()
    {
        if (currentState == PlayerState.Idle) return;
        prevState = currentState;
        currentState = PlayerState.Idle;
        stateTimer = 0f;
        foreach (Animator animator in spriteAnimators)
        {
            animator.SetTrigger("IdleTrigger");
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case PlayerState.Moving:
                break;

            case PlayerState.Idle:
                break;

            case PlayerState.Attacking:
                stateTimer += Time.deltaTime;
                if (stateTimer >= attackDuration)
                {
                    currentState = prevState;
                }
                break;
            case PlayerState.Hit:
                break;
        }
    }
}
