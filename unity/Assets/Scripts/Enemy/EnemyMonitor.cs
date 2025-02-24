using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class EnemyMonitor : MonoBehaviour
{
    public EnemySpawner parentSpawner;

    [Header("Enemy Behavior")]
    private float enemySpeed = -5.0f;
    public float EnemySpeed { get { return enemySpeed; } set { enemySpeed = value; } }

    [Header("Animation Durations")]
    public float idleDuration = 1f;
    public float attackDuration = 0.5f;

    [Header("Damage Scaling")]
    private int attackDamage = 10; // Damage dealt to the player when attacking

    [Header("Health Settings")]
    public int health = 20; // Public health variable

    private Tilemap tilemap;
    private Rigidbody2D enemyRb;
    private Animator animator;
    private SpriteRenderer enemySprite;
    private EnemyState currentState = EnemyState.Moving;
    private float stateTimer = 0f;

    void Start()
    {
        if (parentSpawner != null)
            tilemap = parentSpawner.tilemap;

        enemyRb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        enemySprite = GetComponent<SpriteRenderer>(); // Assuming a single SpriteRenderer on the enemy

        enemyRb.linearVelocity = new Vector2(0, enemySpeed);

        SetState(EnemyState.Moving);
    }

    void Update()
    {
        if (tilemap == null) return;

        // Destroy Enemy object if it passes tilemap's height with a pad of 2
        if (transform.position.y <= tilemap.origin.y - 2)
        {
            Destroy(gameObject);
        }

        switch (currentState)
        {
            case EnemyState.Moving:
                enemyRb.linearVelocity = new Vector2(0, enemySpeed);

                // Position Enemy object in front of the player in the tilemap
                if (transform.position.y <= tilemap.origin.y + 4.5f)
                {
                    enemyRb.linearVelocity = Vector2.zero;
                    GameManager.Instance.PauseMovement();
                    SetState(EnemyState.Idle);
                }
                break;

            case EnemyState.Idle:
                stateTimer += Time.deltaTime;
                if (stateTimer >= idleDuration)
                {
                    SetState(EnemyState.Attacking);
                }
                break;

            case EnemyState.Attacking:
                stateTimer += Time.deltaTime;
                if (stateTimer >= attackDuration)
                {
                    PlayerMonitor.Instance.OnHit(attackDamage);
                    SetState(EnemyState.Idle);
                }
                break;
        }
    }

    // Handle collisions (using 2D triggers)
    private void OnTriggerEnter2D(Collider2D other)
    {
        // If colliding with a spell (tagged "Magic")
        if (other.CompareTag("Magic"))
        {
            TakeSpellDamage(5);
        }
    }

    /// <summary>
    /// Called when the enemy is hit by a spell.
    /// Subtracts health, flashes the enemy, and destroys it if health is 0 or below.
    /// </summary>
    /// <param name="damageAmount">Amount of damage taken (5 in this case).</param>
    public void TakeSpellDamage(int damageAmount)
    {
        health -= damageAmount;
        StartCoroutine(FlashCoroutine());

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    // A simple coroutine to flash the enemy (changes its color briefly)
    private IEnumerator FlashCoroutine()
    {
        if (enemySprite != null)
        {
            Color originalColor = enemySprite.color;
            enemySprite.color = Color.red;
            yield return new WaitForSeconds(0.2f);
            enemySprite.color = originalColor;
        }
    }

    // Helper method to change states and trigger corresponding animations
    void SetState(EnemyState newState)
    {
        currentState = newState;
        stateTimer = 0f;

        switch (newState)
        {
            case EnemyState.Moving:
                animator.SetTrigger("MoveTrigger");
                break;
            case EnemyState.Idle:
                animator.SetTrigger("IdleTrigger");
                break;
            case EnemyState.Attacking:
                animator.SetTrigger("MeleeTrigger");
                break;
        }
    }
}