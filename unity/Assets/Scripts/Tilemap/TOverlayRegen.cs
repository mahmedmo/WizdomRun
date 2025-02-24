using UnityEngine;
using UnityEngine.Tilemaps;

public class TOverlayRegen : MonoBehaviour
{
    [Header("Tilemap")]
    public Tilemap tilemap;
    public float scrollSpeed = 5f;
    public int rows = 20;
    public int cols = 12;

    [Header("Tiles")]
    public TileBase[] overlayBaseTiles;
    public TileBase[] overlayPathTiles;

    [Header("Spawner")]
    [Range(0.0f, 1.0f)]
    public float spawnRate;
    public int maxBaseSpawn = 4;
    public int maxPathSpawn = 1;

    [Header("Camera")]
    public Camera mainCamera;

    private Vector3Int topRowPos;
    private Vector3Int bottomRowPos;

    void Start()
    {
        bottomRowPos = tilemap.origin;

        topRowPos = bottomRowPos + new Vector3Int(0, rows - 1, 0);
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsPaused)
            return;
        // Check GameTime
        if (!GameManager.Instance.RunStart()) return;

        // Tilemap scroll
        transform.position += Vector3.down * scrollSpeed * Time.deltaTime;

        // Checks tilemap bounds and updates tilemap accordingly
        CheckBounds();
    }

    void CheckBounds()
    {
        float topOfMapWorldY = tilemap.CellToWorld(new Vector3Int(0, topRowPos.y, 0)).y;
        float bottomOfMapWorldY = tilemap.CellToWorld(new Vector3Int(0, bottomRowPos.y, 0)).y;

        float cameraTopY = mainCamera.transform.position.y + mainCamera.orthographicSize;
        float cameraBottomY = mainCamera.transform.position.y - mainCamera.orthographicSize;

        if (topOfMapWorldY < cameraTopY)
        {
            AddRow(topRowPos.y + 1);
        }

        if (bottomOfMapWorldY + tilemap.cellSize.y < cameraBottomY)
        {
            ClearRow(bottomRowPos.y);
        }
    }

    void AddRow(int rowY)
    {
        topRowPos = new Vector3Int(0, rowY, 0);

        if (Random.value < spawnRate)
        {
            AddBaseOverlay(rowY);
        }

        if (Random.value < spawnRate)
        {
            AddPathOverlay(rowY);
        }
    }

    void ClearRow(int rowY)
    {
        for (int x = tilemap.origin.x; x < tilemap.origin.x + cols; x++)
        {
            tilemap.SetTile(new Vector3Int(x, rowY, 0), null);
        }

        bottomRowPos = new Vector3Int(0, rowY + 1, 0);
    }

    void AddBaseOverlay(int rowY)
    {
        for (int i = 0; i < maxBaseSpawn; i++)
        {
            int randomCol = Random.Range(0, 2) == 0
                ? Random.Range(tilemap.origin.x, tilemap.origin.x + 4) // Left bounds
                : Random.Range(tilemap.origin.x + cols - 4, tilemap.origin.x + cols); // Right bounds

            TileBase randomTile = overlayBaseTiles[Random.Range(0, overlayBaseTiles.Length)];

            tilemap.SetTile(new Vector3Int(randomCol, rowY, 0), randomTile);
        }
    }

    void AddPathOverlay(int rowY)
    {
        for (int i = 0; i < maxPathSpawn; i++)
        {
            // In between x + 5 and cols -5 (path tiles)
            int randomCol = Random.Range(tilemap.origin.x + 5, tilemap.origin.x + cols - 5);

            TileBase randomTile = overlayPathTiles[Random.Range(0, overlayPathTiles.Length)];

            tilemap.SetTile(new Vector3Int(randomCol, rowY, 0), randomTile);
        }
    }
}