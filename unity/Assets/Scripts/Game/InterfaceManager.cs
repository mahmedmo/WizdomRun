using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class InterfaceManager : MonoBehaviour
{
    public static InterfaceManager Instance { get; private set; }
    public GameObject Interface;

    public GameObject healthBar;

    public GameObject gameOverOverlay;

    public GameObject gameOver;


    public GameObject[] spellSlots = new GameObject[4];

    public SpellDatabase spellDatabase;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Interface = GameObject.Find("UserInterface");
    }
    public void DrawSlots(int[] slots)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == -1)
            {
                spellSlots[i].SetActive(false);
                continue;
            }
            spellSlots[i].SetActive(true);
            spellSlots[i].GetComponent<Image>().sprite = spellDatabase.spells.Find(s => s.spellID == slots[i]).spellSprite;

        }
    }
    public void DrawHealth(int totalHealth, int currHealth)
    {
        Vector3 scale = healthBar.transform.localScale;
        float ratio = currHealth / (float)totalHealth;
        healthBar.transform.DOScaleX(ratio, 0.5f).SetEase(Ease.OutQuad);
    }

    public void DrawGameOver()
    {
        Image overlayImage = gameOverOverlay.GetComponent<Image>();

        overlayImage.color = new Color(1f, 0f, 0f, 0f);

        overlayImage.DOColor(new Color(1f, 0f, 0f, 1f), 4f)
        .SetEase(Ease.Linear)
        .OnComplete(() =>
        {
            // Once the overlay fade completes, fade in the "gameOver" object.
            // Make sure your "gameOver" object has a CanvasGroup component!
            CanvasGroup cg = gameOver.GetComponent<CanvasGroup>();

            // Ensure the gameOver object is active so we can see it.
            gameOver.SetActive(true);

            // Start it fully transparent
            cg.alpha = 0f;

            // Tween to fully visible over 2 seconds (adjust to taste)
            cg.DOFade(1f, 2f).SetEase(Ease.OutQuad);
        });
    }
}
