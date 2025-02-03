using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

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
    public int rareBuffer = 3;
    public float structLifespan = 10f;

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

        // Initialize maxSpawn rows
        for (int i = 0; i < maxSpawn; i++)
        {
            freeRows.Enqueue(bottomRowPos.y + i);
        }

        topRowPos = bottomRowPos + new Vector3Int(0, rows - 1 + 3, 0);
    }

    void Update()
    {
        // Check GameTime
        if (!GameTimeManager.Instance.RunStart()) return;

        // Tilemap scroll
        transform.position += Vector3.down * scrollSpeed * Time.deltaTime;

        // Check tilemap bounds and update accordingly
        CheckBounds();
    }

    void CheckBounds()
    {
        float topOfMapWorldY = tilemap.CellToWorld(new Vector3Int(0, topRowPos.y, 0)).y;
        float bottomOfMapWorldY = tilemap.CellToWorld(new Vector3Int(0, bottomRowPos.y, 0)).y;

        float cameraTopY = mainCamera.transform.position.y + mainCamera.orthographicSize;
        float cameraBottomY = mainCamera.transform.position.y - mainCamera.orthographicSize;

        // As rows enter view render them busy
        if (topOfMapWorldY < cameraTopY)
        {
            if (freeRows.Count > 0)
            {
                AddRow(freeRows.Dequeue());
            }
        }

    }

    void AddRow(int rowY)
    {
        // Ensures no new row (outside of the initial 20) do not get created
        if (busyRows.Contains(rowY)) return;

        if (skipLeft > 0) skipLeft--;
        if (skipRight > 0) skipRight--;

        // Offset rows to above the height of tilemap
        int offsetY = 24;
        int rowYOffset = rowY + offsetY;

        // Create a new Tilemap for each row to implement Z index sorting
        GameObject row = new GameObject($"Row_{rowY}");
        row.transform.parent = this.transform;

        Tilemap rowTilemap = row.AddComponent<Tilemap>();
        TilemapRenderer rowRenderer = row.AddComponent<TilemapRenderer>();

        rowRenderer.sortingLayerName = "Default";

        // Ensures rows with smaller Y order are in front
        rowRenderer.sortingOrder = -rowYOffset;

        int currZ = 0;

        bool multiSpawn = Random.value < 0.25f;

        // Rare Spawn handler
        bool leftRare = false;
        bool rightRare = false;
        // Allows either the left or right to take the rare spawn
        if (Random.value < 0.5f)
        {
            leftRare = GameTimeManager.Instance.CanRareSpawn();
        }
        else
        {
            rightRare = GameTimeManager.Instance.CanRareSpawn();
        }

        // Left Bound Handler
        if (skipLeft <= 0 && Random.value < spawnRate)
        {
            if (multiSpawn)
            {
                // 0: wide-wide, 1: skinny-skinny, 2: skinny-wide
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
                        ? Random.Range(tilemap.origin.x, tilemap.origin.x + 3) // Wide struct bound
                        : Random.Range(tilemap.origin.x, tilemap.origin.x + 4); // Skinny struct bound
                TileBase tileToPlace = PickStructTile(isWide, false);
                if (tileToPlace != null) rowTilemap.SetTile(new Vector3Int(col, rowYOffset, currZ), tileToPlace);

            }
        }
        // After rareBuffer/2 above and rareBuffer/2 below spawn rare struct
        if (skipLeft == rareBuffer / 2)
        {
            int col = tilemap.origin.x - 1; // Rare struct bound
            TileBase tileToPlace = PickStructTile(false, true);
            if (tileToPlace != null) rowTilemap.SetTile(new Vector3Int(col, rowYOffset, currZ), tileToPlace);

        }

        // Right Bound Handler
        if (skipRight <= 0 && Random.value < spawnRate)
        {
            if (multiSpawn)
            {
                // 0: wide-wide, 1: skinny-skinny, 2: skinny-wide
                int spawnCase = Random.value < 0.33f ? 0 : (Random.value < 0.66f ? 1 : 2);
                int firstCol, secondCol;
                TileBase firstTile, secondTile;

                switch (spawnCase)
                {
                    case 0: // Wide-Wide Pair
                        firstCol = tilemap.origin.x + cols;
                        secondCol = firstCol - 3;
                        firstTile = PickStructTile(true, false);
                        secondTile = PickStructTile(true, false);
                        if (firstTile != null) rowTilemap.SetTile(new Vector3Int(firstCol, rowYOffset, currZ), firstTile);
                        if (secondTile != null) rowTilemap.SetTile(new Vector3Int(secondCol, rowYOffset, currZ), secondTile);
                        break;

                    case 1: // Skinny-Skinny Pair
                        firstCol = tilemap.origin.x + cols - 1;
                        secondCol = Mathf.Max(tilemap.origin.x + cols - 4, firstCol - Random.Range(2, 4));
                        firstTile = PickStructTile(false, false);
                        secondTile = PickStructTile(false, false);
                        if (firstTile != null) rowTilemap.SetTile(new Vector3Int(firstCol, rowYOffset, currZ), firstTile);
                        if (secondTile != null) rowTilemap.SetTile(new Vector3Int(secondCol, rowYOffset, currZ), secondTile);
                        break;

                    case 2: // Skinny-Wide Pair
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
                    ? Random.Range(tilemap.origin.x + cols - 3, tilemap.origin.x + cols) // Wide struct bound
                    : Random.Range(tilemap.origin.x + cols - 4, tilemap.origin.x + cols); // Skinny struct bound
                TileBase tileToPlace = PickStructTile(isWide, false);
                if (tileToPlace != null) rowTilemap.SetTile(new Vector3Int(col, rowYOffset, currZ), tileToPlace);
            }
        }

        // After rareBuffer/2 above and rareBuffer/2 below spawn rare struct
        if (skipRight == rareBuffer / 2)
        {
            int col = tilemap.origin.x + cols; // Rare struct bound
            TileBase tileToPlace = PickStructTile(false, true);
            if (tileToPlace != null) rowTilemap.SetTile(new Vector3Int(col, rowYOffset, currZ), tileToPlace);

        }

        if (leftRare) skipLeft = rareBuffer;
        if (rightRare) skipRight = rareBuffer;

        topRowPos = new Vector3Int(0, rowYOffset, 0);

        busyRows.Add(rowY);

        DestroyTimer(row, structLifespan, rowY);
    }

    TileBase PickStructTile(bool isWide, bool isRare)
    {
        TileBase[] tiles = isRare ? (skipLeft != 0 ? rareStructTilesLeft : rareStructTilesRight) : (isWide ? wideStructTiles : skinnyStructTiles);
        if (tiles.Length > 0)
        {
            int randomIndex = Random.Range(0, tiles.Length);
            return tiles[randomIndex];
        }
        return null;
    }

    void DestroyTimer(GameObject row, float delay, int rowY)
    {
        // Destroy row after delay
        Destroy(row, delay);

        // Destroy row after half a delay with a 1.50 second pad
        StartCoroutine(RecycleTimer(rowY, delay / 2 + 1.50f));
    }

    System.Collections.IEnumerator RecycleTimer(int rowY, float delay)
    {
        // Delay before removing row from busy queue
        yield return new WaitForSeconds(delay);
        busyRows.Remove(rowY);

        // Pad for 2 seconds before freeing row
        yield return new WaitForSeconds(2f);
        freeRows.Enqueue(rowY);
    }
}


