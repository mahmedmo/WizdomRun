using UnityEngine;
using UnityEngine.Tilemaps;

public class CSTilemapMonitor : MonoBehaviour
{
    public CutsceneType csType;

    private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Tilemap tilemap;
    private Tilemap refTilemap;
    private Tilemap childTilemap;
    private bool stopFlag = false;
    private bool endFlag = false;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Start moving tilemap into view
        rb.linearVelocity = Vector2.up * -moveSpeed;
        tilemap = GameObject.Find("LOverlay01").GetComponent<Tilemap>();
        refTilemap = GameObject.Find("LStruct01").GetComponent<Tilemap>();
        childTilemap = GetComponentInChildren<Tilemap>();
    }

    void Update()
    {
        // Destination check (stop when fully in view)
        if (transform.position.y <= 0 && !stopFlag)
        {
            Debug.Log("Stoped moving");
            rb.linearVelocity = Vector2.zero;
            stopFlag = true;
            HandleCSType();
        }
        if (!LevelManager.Instance.inCutscene && LevelManager.Instance.elemCSFlag && !LevelManager.Instance.bossCSFlag)
        {
            Debug.Log("Moving away");
            rb.linearVelocity = Vector2.up * -moveSpeed;
        }
        if (transform.position.y <= -10 && endFlag) Destroy(gameObject);

    }

    // Cutscene type handler
    void HandleCSType()
    {
        switch (csType)
        {
            case CutsceneType.Boss:
                LevelManager.Instance.StartBoss();
                break;
            case CutsceneType.Elementalist:
                GameManager.Instance.PauseMovement();
                InterfaceManager.Instance.AnimateContinueButton();
                break;
        }
    }
}