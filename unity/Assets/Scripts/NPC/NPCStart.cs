using UnityEngine;
using UnityEngine.Tilemaps;

public class NPCStart : MonoBehaviour
{
    private Rigidbody2D rb;
    private Tilemap tilemap;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;
        tilemap = GameObject.Find("LStruct01").GetComponent<Tilemap>();
    }

    void Update()
    {
        if (GameManager.Instance == null || tilemap == null) return;

        if (transform.position.y <= tilemap.origin.y - 2)
        {
            Destroy(gameObject);
        }

        if (GameManager.Instance.isPaused)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        rb.linearVelocity = new Vector2(0, -5.0f);
    }
}