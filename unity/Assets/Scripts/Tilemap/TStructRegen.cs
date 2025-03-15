using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Collections;

public class TStructRegen : MonoBehaviour
{
    [Header("Tilemap")]
    public Tilemap tilemap;
    public float scrollSpeed = 5f;
    public int rows = 20;
    public int cols = 12;

    [Header("Tiles")]
    public TileBase[] wideStructTiles;
    public TileBase[] skinnyStructTiles;
    public TileBase[] rareStructTilesLeft;
    public TileBase[] rareStructTilesRight;

    [Header("Spawner")]
    [Range(0.0f, 1.0f)]
    public float spawnRate;
    public int maxSpawn = 20;
    public int rareBuffer = 3; // Buffer for empty rows in between a rare spawn

    [Header("Camera")]
    public Camera mainCamera;

    private Vector3Int topRowPos;
    private Vector3Int bottomRowPos;
    private HashSet<int> busyRows = new HashSet<int>();
    private Queue<int> freeRows = new Queue<int>();
    private int skipLeft = 0;
    private int skipRight = 0;

    void Start()
    {
        bottomRowPos = tilemap.origin;

        // Initialize free rows (row IDs) based on maxSpawn
        for (int i = 0; i < maxSpawn; i++)
        {
            freeRows.Enqueue(bottomRowPos.y + i);
        }

        topRowPos = bottomRowPos + new Vector3Int(0, rows - 1 + 3, 0);
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isPaused)
            return;

        if (!GameManager.Instance.RunStart()) return;

        // Scroll the entire tilemap downward
        transform.position += Vector3.down * scrollSpeed * Time.deltaTime;

        // Check bounds to spawn new rows as they come into view
        CheckBounds();
    }

    void CheckBounds()
    {
        float topOfMapWorldY = tilemap.CellToWorld(new Vector3Int(0, topRowPos.y, 0)).y;
        float cameraTopY = mainCamera.transform.position.y + mainCamera.orthographicSize;

        // If the top of our map is in view, add a new row if available
        if (topOfMapWorldY < cameraTopY && freeRows.Count > 0)
        {
            AddRow(freeRows.Dequeue());
        }
    }

    void AddRow(int rowY)
    {
        // Prevent duplicate rows
        if (busyRows.Contains(rowY)) return;

        if (skipLeft > 0) skipLeft--;
        if (skipRight > 0) skipRight--;

        int offsetY = 24;  // Pushes rows 24 higher from the bottom of the tilemap (pad of 4)
        int rowYOffset = rowY + offsetY;

        // Create a new row GameObject with its own Tilemap for Z-sorting
        GameObject row = new GameObject($"Row_{rowY}");
        row.transform.parent = this.transform;

        Tilemap rowTilemap = row.AddComponent<Tilemap>();
        TilemapRenderer rowRenderer = row.AddComponent<TilemapRenderer>();

        rowRenderer.sortingLayerName = "Default";
        rowRenderer.sortingOrder = -rowYOffset;

        int currZ = 0;
        bool multiSpawn = Random.value < 0.25f;

        // Determine if rare spawn
        bool leftRare = (Random.value < 0.5f) && GameManager.Instance.CanRareSpawn();
        bool rightRare = (!leftRare) && GameManager.Instance.CanRareSpawn();

        // Left Bound Handler
        if (skipLeft <= 0 && Random.value < spawnRate)
        {
            if (multiSpawn)
            {
                int spawnCase = Random.value < 0.33f ? 0 : (Random.value < 0.66f ? 1 : 2);
                int firstCol, secondCol;
                TileBase firstTile, secondTile;

                switch (spawnCase)
                {
                    case 0: // Wide-Wide Pair
                        firstCol = tilemap.origin.x - 1;
                        secondCol = firstCol + 3;
                        firstTile = PickStructTile(true, false);
                        secondTile = PickStructTile(true, false);
                        if (firstTile != null) rowTilemap.SetTile(new Vector3Int(firstCol, rowYOffset, currZ), firstTile);
                        if (secondTile != null) rowTilemap.SetTile(new Vector3Int(secondCol, rowYOffset, currZ), secondTile);
                        break;
                    case 1: // Skinny-Skinny Pair
                        firstCol = tilemap.origin.x;
                        secondCol = Mathf.Min(tilemap.origin.x + 3, firstCol + Random.Range(2, 4));
                        firstTile = PickStructTile(false, false);
                        secondTile = PickStructTile(false, false);
                        if (firstTile != null) rowTilemap.SetTile(new Vector3Int(firstCol, rowYOffset, currZ), firstTile);
                        if (secondTile != null) rowTilemap.SetTile(new Vector3Int(secondCol, rowYOffset, currZ), secondTile);
                        break;
                    case 2: // Skinny-Wide Pair
                        firstCol = Random.Range(tilemap.origin.x - 1, tilemap.origin.x + 1);
                        secondCol = Mathf.Min(tilemap.origin.x + 3, firstCol + Random.Range(2, 4));
                        firstTile = PickStructTile(true, false);
                        secondTile = PickStructTile(false, false);
                        if (firstTile != null) rowTilemap.SetTile(new Vector3Int(firstCol, rowYOffset, currZ), firstTile);
                        if (secondTile != null) rowTilemap.SetTile(new Vector3Int(secondCol, rowYOffset, currZ), secondTile);
                        break;
                }
            }
            else
            {
                bool isWide = Random.value < 0.5f;
                int col = isWide
                    ? Random.Range(tilemap.origin.x, tilemap.origin.x + 3)
                    : Random.Range(tilemap.origin.x, tilemap.origin.x + 4);
                TileBase tileToPlace = PickStructTile(isWide, false);
                if (tileToPlace != null)
                    rowTilemap.SetTile(new Vector3Int(col, rowYOffset, currZ), tileToPlace);
            }
        }

        // Left Bound Rare Hander
        if (skipLeft == rareBuffer / 2)
        {
            int col = tilemap.origin.x - 1;
            TileBase tileToPlace = PickStructTile(false, true);
            if (tileToPlace != null)
                rowTilemap.SetTile(new Vector3Int(col, rowYOffset, currZ), tileToPlace);
        }

        // Right Bound Handler
        if (skipRight <= 0 && Random.value < spawnRate)
        {
            if (multiSpawn)
            {
                int spawnCase = Random.value < 0.33f ? 0 : (Random.value < 0.66f ? 1 : 2);
                int firstCol, secondCol;
                TileBase firstTile, secondTile;

                switch (spawnCase)
                {
                    case 0:
                        firstCol = tilemap.origin.x + cols;
                        secondCol = firstCol - 3;
                        firstTile = PickStructTile(true, false);
                        secondTile = PickStructTile(true, false);
                        if (firstTile != null) rowTilemap.SetTile(new Vector3Int(firstCol, rowYOffset, currZ), firstTile);
                        if (secondTile != null) rowTilemap.SetTile(new Vector3Int(secondCol, rowYOffset, currZ), secondTile);
                        break;
                    case 1:
                        firstCol = tilemap.origin.x + cols - 1;
                        secondCol = Mathf.Max(tilemap.origin.x + cols - 4, firstCol - Random.Range(2, 4));
                        firstTile = PickStructTile(false, false);
                        secondTile = PickStructTile(false, false);
                        if (firstTile != null) rowTilemap.SetTile(new Vector3Int(firstCol, rowYOffset, currZ), firstTile);
                        if (secondTile != null) rowTilemap.SetTile(new Vector3Int(secondCol, rowYOffset, currZ), secondTile);
                        break;
                    case 2:
                        firstCol = Random.Range(tilemap.origin.x + cols, tilemap.origin.x + cols - 2);
                        secondCol = Mathf.Max(tilemap.origin.x + cols - 4, firstCol - Random.Range(2, 4));
                        firstTile = PickStructTile(true, false);
                        secondTile = PickStructTile(false, false);
                        if (firstTile != null) rowTilemap.SetTile(new Vector3Int(firstCol, rowYOffset, currZ), firstTile);
                        if (secondTile != null) rowTilemap.SetTile(new Vector3Int(secondCol, rowYOffset, currZ), secondTile);
                        break;
                }
            }
            else
            {
                bool isWide = Random.value < 0.5f;
                int col = isWide
                    ? Random.Range(tilemap.origin.x + cols - 3, tilemap.origin.x + cols)
                    : Random.Range(tilemap.origin.x + cols - 4, tilemap.origin.x + cols);
                TileBase tileToPlace = PickStructTile(isWide, false);
                if (tileToPlace != null)
                    rowTilemap.SetTile(new Vector3Int(col, rowYOffset, currZ), tileToPlace);
            }
        }

        // Right Bound Rare Hander
        if (skipRight == rareBuffer / 2)
        {
            int col = tilemap.origin.x + cols;
            TileBase tileToPlace = PickStructTile(false, true);
            if (tileToPlace != null)
                rowTilemap.SetTile(new Vector3Int(col, rowYOffset, currZ), tileToPlace);
        }

        if (leftRare) skipLeft = rareBuffer;
        if (rightRare) skipRight = rareBuffer;

        topRowPos = new Vector3Int(0, rowYOffset, 0);
        busyRows.Add(rowY);

        // Attach RowMonitor script to each row to
        // recycle row when off screen + recycle buffer
        RowMonitor monitor = row.AddComponent<RowMonitor>();
        monitor.parentRegen = this;
        monitor.bottomRowPos = bottomRowPos;
    }

    TileBase PickStructTile(bool isWide, bool isRare)
    {
        TileBase[] tiles = isRare ? (skipLeft != 0 ? rareStructTilesLeft : rareStructTilesRight)
                                    : (isWide ? wideStructTiles : skinnyStructTiles);
        if (tiles.Length > 0)
        {
            int randomIndex = Random.Range(0, tiles.Length);
            return tiles[randomIndex];
        }
        return null;
    }

    public void RecycleRow(GameObject row)
    {
        // Find row number
        int rowY;
        string[] split = row.name.Split('_');
        if (split.Length > 1 && int.TryParse(split[1], out rowY))
        {
            busyRows.Remove(rowY);
            freeRows.Enqueue(rowY);
        }
        else
        {
            Debug.LogWarning("RecycleRow: Could not parse rowY from " + row.name);
        }

        Destroy(row);

        if (freeRows.Count > 0)
        {
            AddRow(freeRows.Dequeue());
        }
    }
}