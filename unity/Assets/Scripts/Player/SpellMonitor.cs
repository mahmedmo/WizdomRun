using UnityEngine;
using DG.Tweening;

public class SpellMonitor : MonoBehaviour
{
    [Header("Spell Behavior")]
    [SerializeField] private float spellSpeed = 50.0f;   // Speed for continuous upward movement (after initial tween)
    public float SpellSpeed
    {
        get { return spellSpeed; }
        set { spellSpeed = value; }
    }

    [Header("Damage Scaling")]
    [SerializeField] private int damage = 1;
    public int Damage
    {
        get { return damage; }
        set { damage = value; }
    }

    [Header("Initial Animation Settings")]
    [SerializeField] private float scaleDuration = 0.5f;
    [SerializeField] private float moveDistance = 5f;
    [SerializeField] private float moveDuration = 1f;

    [SerializeField] private float maxTravelDistance = 18f; // Destroy spell when it travels this far

    private Vector3 originalScale;
    private Vector3 startPosition;

    void Start()
    {
        // Capture the original scale (whatever is set on the prefab)
        originalScale = transform.localScale;

        // Start tiny
        transform.localScale = Vector3.zero;

        // We'll tween from the current position
        startPosition = transform.position;

        // 1) Scale up from zero
        transform.DOScale(originalScale, scaleDuration).SetEase(Ease.OutBack);

        // 2) Move up a bit initially
        Vector3 targetPos = startPosition + new Vector3(0, moveDistance, 0);
        transform.DOMove(targetPos, moveDuration).SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                // Optionally do something when this initial animation finishes.
                // For instance, you could start a different movement or effect.
                // If you want it to keep moving up afterwards, you can let Update() handle that:
                // e.g., keep a flag that says "hasAnimated = true" if needed.
            });
    }

    void Update()
    {
        // If you want continuous upward movement (beyond the initial tween),
        // simply keep moving up every frame:
        transform.Translate(Vector3.up * spellSpeed * Time.deltaTime, Space.World);
        // Check the travel distance from the spawn point
        if (Vector3.Distance(startPosition, transform.position) >= maxTravelDistance)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("TOUCHED!!");

        // If you want to damage enemies:
        // if (other.CompareTag("Enemy"))
        // {
        //     // do damage logic
        //     Destroy(gameObject);
        // }
    }
}