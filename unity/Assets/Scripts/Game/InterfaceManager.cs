using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class InterfaceManager : MonoBehaviour
{
    public static InterfaceManager Instance { get; private set; }
    public GameObject Interface;
    public GameObject healthBar;
    public GameObject manaBar;
    public GameObject progressBar;
    public GameObject progressPlayer;
    public GameObject progressEnd;
    public GameObject gameOverOverlay;
    public GameObject gameOver;
    public GameObject introBannerPrefab;
    public GameObject[] spellSlots = new GameObject[4];
    public CanvasGroup oom;
    public CanvasGroup levelLoadOverlay;
    public CanvasGroup introBanner;

    public SpellDatabase spellDatabase;
    private Vector2 playerProgressStartPos;

    private float maxProgress = 1000f;

    public bool oomFlashFlag { get; set; } = false;
    public bool isFadingFlag { get; set; } = false;

    private void resetBar(GameObject bar)
    {
        bar.transform.localScale = new Vector3(1, bar.transform.localScale.y, bar.transform.localScale.z);
    }
    public void HideInterface()
    {
        if (Interface != null)
        {
            Interface.SetActive(false);
        }
    }
    public void ShowInterface()
    {
        if (Interface != null)
        {
            Interface.SetActive(true);
        }
    }
    public void ResetInterface()
    {
        resetBar(healthBar);
        resetBar(manaBar);
        progressBar.GetComponent<Image>().fillAmount = 0f;
        progressPlayer.GetComponent<RectTransform>().anchoredPosition = playerProgressStartPos;

        // Set Game Over back to red and invisible
        gameOverOverlay.GetComponent<Image>().color = new Color(1f, 0f, 0f, 0f);
        gameOver.GetComponent<CanvasGroup>().alpha = 0f;
        gameOver.SetActive(false);

        oom.gameObject.SetActive(false);

        DrawSlots(PlayerMonitor.Instance!.GetPlayerSlots());
        Debug.Log("InterfaceManager state reset!");
    }

    private void assignObjs()
    {
        DontDestroyOnLoad(gameObject);
        Interface = GameObject.Find("LevelInterface");
        healthBar = GameObject.Find("Health");
        manaBar = GameObject.Find("Mana");
        progressBar = GameObject.Find("Progress");
        progressPlayer = GameObject.Find("PlayerIcon");
        progressEnd = GameObject.Find("EndIcon");
        gameOverOverlay = GameObject.Find("GameOverOverlay");
        gameOver = GameObject.Find("GameOver");
        spellSlots[0] = GameObject.Find("Spell01");
        spellSlots[1] = GameObject.Find("Spell02");
        spellSlots[2] = GameObject.Find("Spell03");
        spellSlots[3] = GameObject.Find("Spell04");
        oom = GameObject.Find("OOM").GetComponent<CanvasGroup>();
        levelLoadOverlay = GameObject.Find("LevelLoadOverlay").GetComponent<CanvasGroup>();
        introBanner = GameObject.Find("Intro").GetComponent<CanvasGroup>();
        introBannerPrefab = null;
        playerProgressStartPos = progressPlayer.GetComponent<RectTransform>().anchoredPosition;
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            assignObjs();
            ResetInterface();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    void Start()
    {
        // Optionally, if the overlay is already active (alpha == 1), then start a gradual fade-in:
        if (levelLoadOverlay != null && levelLoadOverlay.alpha >= 1f)
        {
            // Gradually fade from black to clear over 2 seconds.
            levelLoadOverlay.DOFade(0f, 2f)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    levelLoadOverlay.gameObject.SetActive(false);
                });
        }
    }
    void Update()
    {
        if (!GameManager.Instance.isPaused)
        {
            UpdateProgressBar();
        }
    }
    public void UpdateProgressBar()
    {
        // Calculate fillAmount (0 to 1) based on gameTime and maxProgress.
        float fillAmount = Mathf.Clamp01(LevelManager.Instance.levelProgress / maxProgress);

        // Update the progress bar's Image fill amount.
        Image progressImage = progressBar.GetComponent<Image>();
        progressImage.fillAmount = fillAmount;

        // Get the RectTransforms.
        RectTransform playerRect = progressPlayer.GetComponent<RectTransform>();
        RectTransform endRect = progressEnd.GetComponent<RectTransform>();

        // Calculate the maximum offset from the starting position.
        // This is the distance from the starting position (as set in the Inspector)
        // to the progressEnd's anchored position.
        float maxOffset = endRect.anchoredPosition.x - playerProgressStartPos.x;

        // Compute the new X by adding a fraction of the max offset to the starting X.
        float newX = playerProgressStartPos.x + (maxOffset * fillAmount);

        // Update the player's anchored position, keeping the original Y.
        playerRect.anchoredPosition = new Vector2(newX, playerRect.anchoredPosition.y);
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
            spellSlots[i].GetComponent<Image>().sprite = spellDatabase.spellList.Find(s => s.id == slots[i]).icon;

        }
    }

    // Updates health bar with new value
    public void DrawHealth(float totalHealth, float currHealth)
    {
        Vector3 scale = healthBar.transform.localScale;
        float ratio = currHealth / totalHealth;
        healthBar.transform.DOScaleX(ratio, 0.5f).SetEase(Ease.OutQuad);
    }

    // Updates mana bar with new value
    public void DrawMana(int totalMana, int currMana)
    {
        Vector3 scale = manaBar.transform.localScale;
        float ratio = currMana / (float)totalMana;
        manaBar.transform.DOScaleX(ratio, 0.5f).SetEase(Ease.OutQuad);
    }
    private void InstantiateIntroBanner()
    {
        // Optionally, choose a parent for organization (e.g., your Canvas).
        Transform parent = introBanner.gameObject.transform;
        introBannerPrefab = Instantiate(LevelManager.Instance.intro, parent, false);
    }
    public void FadeIntro()
    {
        InstantiateIntroBanner();
        // Ensure the intro banner is active.
        introBanner.gameObject.SetActive(true);

        // Create a DOTween sequence.
        Sequence introSequence = DOTween.Sequence();

        // Fade in over 1 second.
        introSequence.Append(introBanner.DOFade(1f, 1f).SetEase(Ease.Linear));

        // Hold for 2 seconds.
        introSequence.AppendInterval(2f);

        // Fade out over 1 second.
        introSequence.Append(introBanner.DOFade(0f, 1f).SetEase(Ease.Linear));

        // Once complete, optionally disable the canvas group.
        introSequence.OnComplete(() =>
        {
            Destroy(introBannerPrefab);
        });
    }

    // Flashes screen with "Out of Mana" message
    public void FlashOOM()
    {
        if (oomFlashFlag) return;

        // Signals that OOM animation is in progress
        oomFlashFlag = true;

        // Activate the OOM object
        oom.gameObject.SetActive(true);

        CanvasGroup cg = oom.GetComponent<CanvasGroup>();

        // Ensure the OOM object starts fully transparent.
        cg.alpha = 0f;

        Sequence flashSequence = DOTween.Sequence();
        // 5 cycles of fading in and fading out.
        for (int i = 0; i < 5; i++)
        {
            flashSequence.Append(cg.DOFade(1f, 0.2f));  // Fade in.
            flashSequence.Append(cg.DOFade(0f, 0.2f));  // Fade out.
        }

        // When the sequence completes, ensure the object is fully faded out and deactivate it.
        flashSequence.OnComplete(() =>
        {
            cg.alpha = 0f;
            oom.gameObject.SetActive(false);
            oomFlashFlag = false;
        });
    }

    // Disables a spell for a specified duration
    public void SpellCooldown(int slotIndex, float duration)
    {
        // Find the CooldownOverlay child on the spell slot.
        Transform overlayTransform = spellSlots[slotIndex].transform.Find("CooldownOverlay");
        Image overlayImage = overlayTransform.GetComponent<Image>();
        overlayImage.gameObject.SetActive(true);

        Button slotButton = spellSlots[slotIndex].GetComponent<Button>();

        // Disable the button
        slotButton.interactable = false;

        // Reset fill amount to full.
        overlayImage.fillAmount = 1f;

        // Animate the fill amount from full to empty over 'duration' seconds.
        overlayImage.DOFillAmount(0f, duration)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                overlayImage.gameObject.SetActive(false);
                slotButton.interactable = true;
            });
    }
    public void DrawGameOver()
    {
        Image overlayImage = gameOverOverlay.GetComponent<Image>();

        overlayImage.color = new Color(1f, 0f, 0f, 0f);

        overlayImage.DOColor(new Color(1f, 0f, 0f, 1f), 2f)
        .SetEase(Ease.Linear)
        .OnComplete(() =>
        {
            CanvasGroup cg = gameOver.GetComponent<CanvasGroup>();

            gameOver.SetActive(true);

            cg.alpha = 0f;

            cg.DOFade(1f, 2f).SetEase(Ease.OutQuad);
        });
    }

    public void FadeFromBlack(float duration, System.Action onComplete = null)
    {
        isFadingFlag = true;

        // Ensure the overlay is active
        if (!levelLoadOverlay.gameObject.activeSelf)
            levelLoadOverlay.gameObject.SetActive(true);

        // Start fully opaque.
        levelLoadOverlay.alpha = 1f;

        // Animate to transparent.
        levelLoadOverlay.DOFade(0f, duration)
        .SetEase(Ease.Linear)
        .OnComplete(() =>
        {
            levelLoadOverlay.gameObject.SetActive(false);
            isFadingFlag = false;
            onComplete?.Invoke();
        });
    }

    // Call this function to fade to black (for example, on game over).
    public Tween FadeToBlack(float duration, System.Action onComplete = null)
    {
        isFadingFlag = true;

        // Activate the overlay
        if (!levelLoadOverlay.gameObject.activeSelf)
            levelLoadOverlay.gameObject.SetActive(true);

        // Start fully transparent.
        levelLoadOverlay.alpha = 0f;

        // Create the tween that fades to opaque.
        Tween fadeTween = levelLoadOverlay.DOFade(1f, duration)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                isFadingFlag = false;
                onComplete?.Invoke();
            });

        return fadeTween;
    }
}
