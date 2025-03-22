using UnityEngine;
using UnityEngine.Tilemaps;
using DG.Tweening;
using System.Collections;
public class BossMonitor : MonoBehaviour
{
    public int Damage { get; set; }
    public int Health { get; set; }
    public int GoldDrop { get; set; }
    public bool Airborne { get; set; }
    public Tilemap tilemap { get; set; }

    [Header("Animation Durations")]
    public float idleDuration = 1f;
    public float attackDuration = 0.5f;
    public float deathDuration = 0.5f;

    private Rigidbody2D enemyRb;
    private Animator animator;
    private SpriteRenderer enemySprite;
    private EnemyState currentState = EnemyState.Idle;
    private float stateTimer = 0f;
    private bool stopFlag = false;
    private int currHealth;
    private bool isAttacking = false;
    private Vector3 originalPosition;

    void Start()
    {
        enemyRb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        enemySprite = GetComponent<SpriteRenderer>();
        SetState(EnemyState.Idle);
        stopFlag = false;
        currHealth = Health;
        const int speed = 5;
        enemyRb.linearVelocity = Vector2.up * -speed;
        originalPosition = transform.position;
    }

    void Update()
    {
        // Boss reaches its destination (in view and on boss tilemap)
        if (transform.position.y < tilemap.origin.y + 14 && !stopFlag)
        {
            enemyRb.linearVelocity = Vector2.zero;
            stopFlag = true;
            originalPosition = transform.position;
            LevelManager.Instance.bossCSFlag = true;
            return;
        }

        if (PlayerMonitor.Instance!.playerDead)
        {
            SetState(EnemyState.Idle);
        }

        if (!isAttacking)
        {
            switch (currentState)
            {
                case EnemyState.Idle:
                    stateTimer += Time.deltaTime;
                    break;

                case EnemyState.Death:
                    stateTimer += Time.deltaTime;
                    if (stateTimer >= deathDuration)
                    {
                        InterfaceManager.Instance.ShowBossDataPopOut();
                        Destroy(gameObject);
                    }
                    break;
            }
        }
        // If it’s enemy turn (isPlayerTurn false) and boss has not started attacking, start attack sequence.
        if (!LevelManager.Instance.isPlayerTurn && !isAttacking && stopFlag && currentState != EnemyState.Death)
        {
            isAttacking = true;
            StartCoroutine(EnemyAttackSequence());
        }
    }
    // Async method that transitions the boss to the player to attack then hands over turn privelage
    private IEnumerator EnemyAttackSequence()
    {
        Vector3 targetPos = new Vector3(transform.position.x, tilemap.origin.y + 4.5f, transform.position.z);
        float moveDuration = 0.5f;

        yield return transform.DOMove(targetPos, moveDuration).SetEase(Ease.OutSine).WaitForCompletion();

        SetState(EnemyState.Attack);
        yield return new WaitForSeconds(attackDuration);

        // Apply damage
        PlayerMonitor.Instance?.OnHit(Damage * LevelManager.Instance.enemyDmgMultiplier);

        yield return transform.DOMove(originalPosition, moveDuration).SetEase(Ease.InSine).WaitForCompletion();

        SetState(EnemyState.Idle);

        yield return new WaitForSeconds(1.5f);

        LevelManager.Instance.PlayerTurn();

        isAttacking = false;
    }

    // Boss state machine
    void SetState(EnemyState newState)
    {
        currentState = newState;
        stateTimer = 0f;

        switch (newState)
        {
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

    // Boss collision detector
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
            if (LevelManager.Instance.bossCSFlag) StartCoroutine(DelayedTurnSwitch());
        }
    }

    private IEnumerator DelayedTurnSwitch()
    {
        yield return new WaitForSeconds(1.5f);
        LevelManager.Instance.EnemyTurn();
    }
    public void DodgeSpellDamage()
    {
        StartCoroutine(FlashBlueCoroutine());

    }
    public void TakeSpellDamage(int damageAmount)
    {
        // If we are still moving then we haven't started the boss fight yet.
        if (!GameManager.Instance.isPaused) return;
        currHealth -= damageAmount;
        currHealth = Mathf.Clamp(currHealth, 0, Health);
        InterfaceManager.Instance.DrawBossHealth(Health, currHealth);
        StartCoroutine(FlashRedCoroutine());
        if (currHealth <= 0)
        {
            LevelManager.Instance.bossCSFlag = false;
            LevelManager.Instance.OnBossDeath();
            isAttacking = false;
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

}