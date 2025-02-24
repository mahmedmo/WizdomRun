using UnityEngine;
using UnityEngine.Tilemaps;

public class RowMonitor : MonoBehaviour
{
    public TStructRegen parentRegen;
    public Vector3Int bottomRowPos;

    private Tilemap tilemap;

    void Start()
    {
        if (parentRegen != null)
            tilemap = parentRegen.tilemap;
    }

    void Update()
    {
        if (tilemap == null) return;

        // If row exceeds below the tilemap by a buffer of tilemap height + 11, recycle it
        if (transform.position.y <= bottomRowPos.y - tilemap.size.y - 11)
        {
            if (parentRegen != null)
                parentRegen.RecycleRow(gameObject);
        }
    }
}