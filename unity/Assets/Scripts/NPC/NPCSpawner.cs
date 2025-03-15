using UnityEngine;
using UnityEngine.Tilemaps;

public class NPCSpawner : MonoBehaviour
{
    [Header("Tilemap")]
    public Tilemap tilemap;

    [Header("NPC Speed")]
    public float npcSpeed = 5.0f;

    [Header("NPC Sprites")]
    public GameObject[] npcPrefabs;

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isPaused)
            return;

        if (GameManager.Instance != null && GameManager.Instance.CanNPCSpawn())
            SpawnNPC();
    }

    void SpawnNPC()
    {
        if (npcPrefabs.Length <= 0 || tilemap == null) return;

        // Spawn either left or right
        bool spawnLeft = Random.value < 0.5f;

        // Spawn position logic
        int spawnX = spawnLeft ? tilemap.origin.x + 1 : tilemap.origin.x + tilemap.size.x - 1;
        int spawnY = tilemap.origin.y + tilemap.size.y + 5;
        Vector3 spawnPos = tilemap.CellToWorld(new Vector3Int(spawnX, spawnY, -2));

        GameObject npc = Instantiate(npcPrefabs[Random.Range(0, npcPrefabs.Length)], spawnPos, Quaternion.identity);
        npc.transform.parent = this.transform;

        // Z level positioning
        Vector3 npcLocalPos = npc.transform.localPosition;
        npcLocalPos.z = -2;
        npc.transform.localPosition = npcLocalPos;

        // Sprite faces towards tilemap's middle
        Vector3 npcScale = npc.transform.localScale;
        npcScale.x = spawnLeft ? 1 : -1;
        npc.transform.localScale = npcScale;

        // Spite movement enabler
        Rigidbody2D rb = npc.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(0, -npcSpeed);
        }

        // Add NPCMonitor script to created sprite to handle
        // sprite specific functions and pass needed vals
        if (npc.GetComponent<NPCMonitor>() == null)
        {
            npc.AddComponent<NPCMonitor>();
        }
        npc.GetComponent<NPCMonitor>().NPCSpeed = -npcSpeed;
    }
}