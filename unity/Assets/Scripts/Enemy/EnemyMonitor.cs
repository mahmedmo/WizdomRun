using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using DG.Tweening;
public class EnemyMonitor : MonoBehaviour
{
    public float Speed { get; set; }
    public int Damage { get; set; }
    public int Health { get; set; }
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
        tilemap = GameObject.Find("Struct01").GetComponent<Tilemap>();

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
        if (GameManager.Instance.PauseEnemyMovement && GameManager.Instance.isPaused)
        {
            enemyRb.linearVelocity = Vector2.zero;
            return;
        }
        if (PlayerMonitor.Instance!.playerDead)
        {
            SetState(EnemyState.Idle);
        }

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
                // Position Enemy object in front of the player in the tilemap
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
    private bool IsEnemyBelow(float rayDistance = 2f, float safeSeparation = 0.5f)
    {
        // Using enemyRb.position ensures we use the Rigidbody2D position
        Vector2 origin = enemyRb.position;
        Vector2 direction = Vector2.down;

        // Create a layer mask for layer 6 (Enemies layer)
        int enemyLayerMask = 1 << 6;

        // Get all hits along the ray
        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, direction, rayDistance, enemyLayerMask);

        // Debug draw the ray (visible in Scene view)
        Debug.DrawRay(origin, direction * rayDistance, Color.red, 0.1f);

        // Find the closest enemy (if any)
        float closestDistance = float.MaxValue;
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider.gameObject != gameObject && hit.collider.CompareTag("Enemy"))
            {
                // Only update if this hit is closer than any we've seen before
                if (hit.distance < closestDistance)
                {
                    closestDistance = hit.distance;
                }
            }
        }

        // If we found an enemy and it's within the safe separation distance, return true
        if (closestDistance != float.MaxValue && closestDistance <= safeSeparation)
        {
            return true;
        }

        return false;
    }

    // Handle collisions (using 2D triggers)
    private void OnTriggerEnter2D(Collider2D other)
    {
        // If colliding with a spell (tagged "Magic")
        if (other.CompareTag("Magic"))
        {
            if (Airborne && !other.gameObject.GetComponent<SpellMonitor>().Airborne) return;
            int damageTaken = other.gameObject.GetComponent<SpellMonitor>().Damage;

            Debug.Log("HIT " + damageTaken);
            TakeSpellDamage(damageTaken);
        }
    }

    public void TakeSpellDamage(int damageAmount)
    {
        Health -= damageAmount;
        StartCoroutine(FlashCoroutine());

        if (Health <= 0)
        {
            SetState(EnemyState.Death);
        }
    }


    // A simple coroutine to flash the enemy (changes its color briefly)
    private IEnumerator FlashCoroutine()
    {
        // Use the SpriteRenderer's current color as the original color.
        Color originalColor = enemySprite.color;

        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        enemySprite.GetPropertyBlock(mpb);

        // Immediately set the color to red.
        mpb.SetColor("_Color", Color.red);
        enemySprite.SetPropertyBlock(mpb);

        // Wait for 0.2 seconds.
        yield return new WaitForSeconds(0.2f);

        // Lerp back to original color over 0.4 seconds.
        float flashDuration = 0.4f;
        float t = 0f;
        while (t < flashDuration)
        {
            t += Time.deltaTime;
            Color lerpedColor = Color.Lerp(Color.red, originalColor, t / flashDuration);
            mpb.SetColor("_Color", lerpedColor);
            enemySprite.SetPropertyBlock(mpb);
            yield return null;
        }
        // Ensure final color is restored.
        mpb.SetColor("_Color", originalColor);
        enemySprite.SetPropertyBlock(mpb);
    }

    // Helper method to change states and trigger corresponding animations
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