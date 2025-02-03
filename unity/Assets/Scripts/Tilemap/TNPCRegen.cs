using UnityEngine;
using UnityEngine.Tilemaps;

public class TNPCRegen : MonoBehaviour
{
    [Header("Tilemap")]
    public Tilemap tilemap;

    public float npcSpeed = 5.0f;

    [Header("Spawner")]
    public GameObject[] npcPrefabs;
    public float lifespan = 12f;

    void Update()
    {
        if (GameTimeManager.Instance.CanNPCSpawn()) SpawnNPC();

    }

    void SpawnNPC()
    {
        if (npcPrefabs.Length <= 0 || tilemap == null) return;

        bool spawnLeft = Random.value < 0.5f;
        Vector3Int tilemapOrigin = tilemap.origin;
        Vector3Int tilemapSize = tilemap.size;

        int spawnX = spawnLeft ? tilemapOrigin.x + 1 : tilemapOrigin.x + tilemapSize.x - 1;
        int spawnY = tilemapOrigin.y + tilemapSize.y + 5;
        Vector3 spawnPos = tilemap.CellToWorld(new Vector3Int(spawnX, spawnY, -2));

        GameObject npc = Instantiate(npcPrefabs[Random.Range(0, npcPrefabs.Length - 1)], spawnPos, Quaternion.identity);

        Rigidbody2D rb = npc.GetComponent<Rigidbody2D>();

        npc.transform.parent = this.transform;

        Vector3 npcLocalPos = npc.transform.localPosition;
        npcLocalPos.z = -2;
        npc.transform.localPosition = npcLocalPos;

        Vector3 npcScale = npc.transform.localScale;
        npcScale.x = spawnLeft ? 1 : -1;
        npc.transform.localScale = npcScale;


        if (rb != null)
        {
            rb.linearVelocity = new Vector2(0, -npcSpeed);
        }

        Destroy(npc, lifespan);
    }
}