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
        originalScale = transform.localScale;

        // Start tiny
        transform.localScale = Vector3.zero;

        startPosition = transform.position;

        // Scale up from zero
        transform.DOScale(originalScale, scaleDuration).SetEase(Ease.OutBack);

        Vector3 targetPos = startPosition + new Vector3(0, moveDistance, 0);
        transform.DOMove(targetPos, moveDuration).SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                // WIP: MAY COME BACK HERE TO ADD A MAGIC HIT EFFECT
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