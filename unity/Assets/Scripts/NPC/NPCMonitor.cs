using UnityEngine;
using UnityEngine.Tilemaps;

public class NPCMonitor : MonoBehaviour
{
    private Tilemap tilemap;
    private Rigidbody2D npcRb;

    private float npcSpeed = -5.0f;
    public float NPCSpeed { get { return npcSpeed; } set { npcSpeed = value; } }

    void Start()
    {
        npcRb = GetComponent<Rigidbody2D>();
        npcRb.linearVelocity = new Vector2(0, npcSpeed);
    }
    void Update()
    {
        if (GameManager.Instance != null || tilemap == null) return;

        // Destroy NPC object if it passes tilemaps height with a pad of 2
        if (transform.position.y <= tilemap.origin.y - 2)
        {
            Destroy(gameObject);
        }

        if (GameManager.Instance.isPaused)
        {
            npcRb.linearVelocity = Vector2.zero;
            return;
        }

        npcRb.linearVelocity = new Vector2(0, npcSpeed);
    }
}
