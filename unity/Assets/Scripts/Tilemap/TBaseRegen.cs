using UnityEngine;
using UnityEngine.Tilemaps;

public class TBaseRegen : MonoBehaviour
{
    [Header("Tilemap")]
    public Tilemap tilemap;
    public float scrollSpeed = 5f;
    public int rows = 20;
    public int cols = 12;

    [Header("Tiles")]
    public TileBase pathTile;
    public TileBase pathLETile;
    public TileBase pathRETile;
    public TileBase baseTile;

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
        for (int x = tilemap.origin.x; x < tilemap.origin.x + cols; x++)
        {
            // Tilemap relative X pos
            int tileX = x - tilemap.origin.x;

            if (tileX < 4 || tileX > cols - 5)
            {
                tilemap.SetTile(new Vector3Int(x, rowY, 0), baseTile);
            }
            else if (tileX == 4)
            {
                tilemap.SetTile(new Vector3Int(x, rowY, 0), pathLETile);
            }
            else if (tileX == cols - 5)
            {
                tilemap.SetTile(new Vector3Int(x, rowY, 0), pathRETile);
            }
            else
            {
                tilemap.SetTile(new Vector3Int(x, rowY, 0), pathTile);
            }
        }

        topRowPos = new Vector3Int(0, rowY, 0);
    }

    void ClearRow(int rowY)
    {
        for (int x = tilemap.origin.x; x < tilemap.origin.x + cols; x++)
        {
            tilemap.SetTile(new Vector3Int(x, rowY, 0), null);
        }

        bottomRowPos = new Vector3Int(0, rowY + 1, 0);
    }
}