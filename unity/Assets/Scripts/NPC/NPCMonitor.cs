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
        tilemap = GameObject.Find("LOverlay01").GetComponent<Tilemap>();
    }
    void Update()
    {
        if (LevelManager.Instance.inCutscene) Destroy(gameObject);
        if (GameManager.Instance.IsFrozen || GameManager.Instance.isPaused || PlayerMonitor.Instance.playerDead)
        {
            npcRb.linearVelocity = Vector2.zero;
            return;
        }
        if (tilemap == null) return;

        // Destroy NPC object if it passes tilemaps height with a pad of 2
        if (transform.position.y <= tilemap.origin.y - 2)
        {
            Destroy(gameObject);
        }

        npcRb.linearVelocity = new Vector2(0, npcSpeed);
    }
}
