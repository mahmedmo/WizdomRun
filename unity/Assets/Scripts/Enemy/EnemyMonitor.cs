using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using DG.Tweening;
public class EnemyMonitor : MonoBehaviour
{
    public float Speed { get; set; }
    public int Damage { get; set; }
    public int Health { get; set; }
    public int GoldDrop { get; set; }
    public bool Airborne { get; set; }

    [Header("Animation Durations")]
    public float idleDuration = 1f;
    public float attackDuration = 0.5f;
    public float deathDuration = 0.5f;

    private Tilemap tilemap;
    private Rigidbody2D enemyRb;
    private Animator animator;
    private SpriteRenderer enemySprite;
    private EnemyState currentState = EnemyState.Run;
    private float stateTimer = 0f;
    private bool isWaiting = false;

    void Start()
    {
        tilemap = GameObject.Find("LStruct01").GetComponent<Tilemap>();

        enemyRb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        enemySprite = GetComponent<SpriteRenderer>();
        enemyRb.linearVelocity = new Vector2(0, Speed);

        SetState(EnemyState.Run);
    }

    void Update()
    {
        if (tilemap == null) return;

        // Destroy Enemy object if it passes tilemap's height with a pad of 2
        if (transform.position.y <= tilemap.origin.y - 2)
        {
            Destroy(gameObject);
        }
        if (GameManager.Instance.IsFrozen)
        {
            enemyRb.linearVelocity = Vector2.zero;
            return;
        }
        if (PlayerMonitor.Instance!.playerDead)
        {
            SetState(EnemyState.Idle);
        }
        // State machine for Enemy, checks for other enemies to indicate pause, if no enemy, attacks.
        switch (currentState)
        {
            case EnemyState.Run:
                if (IsEnemyBelow())
                {
                    enemyRb.linearVelocity = Vector2.zero;
                    isWaiting = true;
                    SetState(EnemyState.Idle);
                    break;
                }
                else
                {
                    enemyRb.linearVelocity = new Vector2(0, Speed);
                }
                if (transform.position.y <= tilemap.origin.y + 4.5f)
                {
                    enemyRb.linearVelocity = Vector2.zero;
                    GameManager.Instance.PauseMovement();
                    SetState(EnemyState.Idle);

                }

                break;

            case EnemyState.Idle:
                if (isWaiting && !IsEnemyBelow())
                {
                    isWaiting = false;
                    SetState(EnemyState.Run);
                    break;
                }
                else if (isWaiting)
                {
                    enemyRb.linearVelocity = Vector2.zero;
                    break;
                }
                stateTimer += Time.deltaTime;
                if (stateTimer >= idleDuration && !isWaiting)
                {
                    SetState(EnemyState.Attack);
                }
                break;

            case EnemyState.Attack:
                stateTimer += Time.deltaTime;
                if (stateTimer >= attackDuration)
                {
                    PlayerMonitor.Instance?.OnHit(Damage * LevelManager.Instance.enemyDmgMultiplier);
                    SetState(EnemyState.Idle);
                }
                break;
            case EnemyState.Death:
                stateTimer += Time.deltaTime;
                if (stateTimer >= deathDuration)
                {
                    LevelManager.Instance.DecrementEnemyCount();
                    Destroy(gameObject);
                }
                break;
        }
    }

    // Helper method to check for nearby enemies
    private bool IsEnemyBelow(float rayDistance = 2f, float safeSeparation = 1f)
    {
        Vector2 origin = enemyRb.position;
        Vector2 direction = Vector2.down;

        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, direction, rayDistance);

        Debug.DrawRay(origin, direction * rayDistance, Color.red, 0.1f);

        float closestDistance = float.MaxValue;
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider.gameObject != gameObject && hit.collider.CompareTag("Enemy"))
            {
                if (hit.distance < closestDistance)
                {
                    closestDistance = hit.distance;
                }
            }
        }

        if (closestDistance != float.MaxValue && closestDistance <= safeSeparation)
        {
            return true;
        }

        return false;
    }

    // Enemy collision detector (For player spells)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Magic"))
        {
            if (Airborne && !other.gameObject.GetComponent<SpellMonitor>().Airborne)
            {
                DodgeSpellDamage();
                return;
            }
            int damageTaken = other.gameObject.GetComponent<SpellMonitor>().Damage;

            Debug.Log("HIT " + damageTaken);
            TakeSpellDamage(damageTaken);
        }
    }
    public void DodgeSpellDamage()
    {
        StartCoroutine(FlashBlueCoroutine());
    }

    public void TakeSpellDamage(int damageAmount)
    {
        Health -= damageAmount;
        StartCoroutine(FlashRedCoroutine());

        if (Health <= 0)
        {
            LevelManager.Instance.AddGold(GoldDrop);
            SetState(EnemyState.Death);
        }
    }


    // Flashes the sprite with red to indicate damage
    private IEnumerator FlashRedCoroutine()
    {
        Color originalColor = enemySprite.color;
        Color flashColor = Color.red;

        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        enemySprite.GetPropertyBlock(mpb);

        mpb.SetColor("_Color", flashColor);
        enemySprite.SetPropertyBlock(mpb);
        enemySprite.color = flashColor;

        yield return new WaitForSeconds(0.2f);

        float flashDuration = 0.4f;
        float t = 0f;
        while (t < flashDuration)
        {
            t += Time.deltaTime;
            Color lerpedColor = Color.Lerp(flashColor, originalColor, t / flashDuration);
            mpb.SetColor("_Color", lerpedColor);
            enemySprite.SetPropertyBlock(mpb);
            enemySprite.color = lerpedColor;
            yield return null;
        }

        mpb.SetColor("_Color", originalColor);
        enemySprite.SetPropertyBlock(mpb);
        enemySprite.color = originalColor;
    }
    // Flashes the sprite with blue to indicate dodge
    private IEnumerator FlashBlueCoroutine()
    {
        Color flashColor = new Color(0f, 248f / 255f, 255f / 255f);
        Color originalColor = enemySprite.color;

        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        enemySprite.GetPropertyBlock(mpb);

        mpb.SetColor("_Color", flashColor);
        enemySprite.SetPropertyBlock(mpb);
        enemySprite.color = flashColor;

        yield return new WaitForSeconds(0.5f);

        float flashDuration = 0.4f;
        float t = 0f;
        while (t < flashDuration)
        {
            t += Time.deltaTime;
            Color lerpedColor = Color.Lerp(flashColor, originalColor, t / flashDuration);
            mpb.SetColor("_Color", lerpedColor);
            enemySprite.SetPropertyBlock(mpb);
            enemySprite.color = lerpedColor;
            yield return null;
        }
        mpb.SetColor("_Color", originalColor);
        enemySprite.SetPropertyBlock(mpb);
        enemySprite.color = originalColor;
    }

    // Enemy state machine
    void SetState(EnemyState newState)
    {
        currentState = newState;
        stateTimer = 0f;

        switch (newState)
        {
            case EnemyState.Run:
                animator.SetTrigger("IsRun");
                break;
            case EnemyState.Idle:
                animator.SetTrigger("IsIdle");
                break;
            case EnemyState.Attack:
                animator.SetTrigger("IsAttack");
                break;
            case EnemyState.Death:
                animator.SetTrigger("Death");
                break;
        }
    }
}