using UnityEngine;
using DG.Tweening;

public class SpellMonitor : MonoBehaviour
{
    [Header("Spell Behavior")]
    public float Speed { get; set; }
    public int Damage { get; set; }
    public bool Airborne { get; set; }

    [Header("Initial Animation Settings")]
    [SerializeField] private float scaleDuration = 0.5f;
    [SerializeField] private float moveDistance = 5f;
    [SerializeField] private float moveDuration = 1f;
    [SerializeField] private float maxTravelDistance = 18f;

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

        transform.Translate(Vector3.up * Speed * Time.deltaTime, Space.World);
        if (Vector3.Distance(startPosition, transform.position) >= maxTravelDistance)
        {
            Destroy(gameObject);
        }
    }

}