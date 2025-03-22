using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using TMPro;
using System.Collections.Generic;
public class InterfaceManager : MonoBehaviour
{
    public static InterfaceManager Instance { get; private set; }
    public GameObject Interface;
    public TextMeshProUGUI playerGoldText;
    public GameObject healthBar;
    public GameObject manaBar;
    public GameObject bossBar;
    public GameObject bossInfo;
    public TextMeshProUGUI bossNameText;
    public GameObject progressBar;
    public GameObject progressPlayer;
    public GameObject progressEnd;
    public GameObject continueButton;
    public GameObject gameOverOverlay;
    public GameObject gameOver;
    public TextMeshProUGUI remainingTriesText;
    public TextMeshProUGUI tryAgainText;
    public GameObject introBannerPrefab;
    public GameObject[] spellSlots = new GameObject[4];
    public GameObject questionSpell;
    public CanvasGroup oom;
    public GameObject startPopup;
    public GameObject bossPopup;

    public GameObject playerTurn;
    public GameObject enemyTurn;

    public GameObject settingsMenu;

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

    public void SettingsMenuPopIn()
    {
        settingsMenu.transform.localScale = Vector3.zero;
        settingsMenu.SetActive(true);
        settingsMenu.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
    }

    public void OnNoReturn()
    {
        settingsMenu.transform.DOScale(Vector3.zero, 0.5f)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                settingsMenu.SetActive(false);
            });
    }

    public void OnYesReturn()
    {
        GameManager.Instance.OnMainMenu();
    }
    // Sets UI objects back to original states
    public void ResetInterface()
    {
        resetBar(healthBar);
        resetBar(bossBar);
        resetBar(manaBar);
        progressBar.GetComponent<Image>().fillAmount = 0f;
        progressPlayer.GetComponent<RectTransform>().anchoredPosition = playerProgressStartPos;
        gameOverOverlay.GetComponent<Image>().color = new Color(1f, 0f, 0f, 0f);
        gameOver.GetComponent<CanvasGroup>().alpha = 0f;
        gameOver.SetActive(false);
        continueButton.SetActive(false);
        oom.gameObject.SetActive(false);
        bossInfo.SetActive(false);
        Debug.Log("InterfaceManager state reset!");
    }

    private void assignObjs()
    {
        Interface = GameObject.Find("LevelInterface");
        healthBar = GameObject.Find("Health");
        manaBar = GameObject.Find("Mana");
        bossBar = GameObject.Find("BossHealth");
        bossInfo = GameObject.Find("BossInfo");
        progressBar = GameObject.Find("Progress");
        progressPlayer = GameObject.Find("PlayerIcon");
        progressEnd = GameObject.Find("EndIcon");
        gameOverOverlay = GameObject.Find("GameOverOverlay");
        gameOver = GameObject.Find("GameOver");
        spellSlots[0] = GameObject.Find("Spell01");
        spellSlots[1] = GameObject.Find("Spell02");
        spellSlots[2] = GameObject.Find("Spell03");
        spellSlots[3] = GameObject.Find("Spell04");
        playerGoldText = GameObject.Find("GoldText").GetComponent<TextMeshProUGUI>();
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
        }
        else
        {
            Destroy(gameObject);
        }

    }

    void Start()
    {
        GetGold();

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
    public void OnContinuePress()
    {
        continueButton.SetActive(false);
        LevelManager.Instance.inCutscene = false;
        AnimateStartPopup();
        GameManager.Instance.ResumeMovement();
    }

    public void AnimateContinueButton()
    {
        continueButton.SetActive(true);

        CanvasGroup cg = continueButton.GetComponent<CanvasGroup>();
        if (cg == null)
        {
            cg = continueButton.AddComponent<CanvasGroup>();
        }

        cg.alpha = 0f;
        cg.DOFade(1f, 1f).SetLoops(-1, LoopType.Yoyo);
    }
    // Gold Handlers
    public void GetGold()
    {
        playerGoldText.text = PlayerMonitor.Instance.GetGold().ToString();
    }
    public void SetGold(int amount)
    {
        playerGoldText.text = amount.ToString();
    }
    public void AddGold(int amount, float duration = 0.5f)
    {
        int startGold = int.Parse(playerGoldText.text);
        int targetGold = startGold + amount;

        DOTween.To(() => startGold, x =>
        {
            startGold = x;
            playerGoldText.text = startGold.ToString();
        }, targetGold, duration);
    }

    public void SpendGold(int amount, float duration = 0.5f)
    {
        int startGold = int.Parse(playerGoldText.text);
        int targetGold = startGold - amount;

        DOTween.To(() => startGold, x =>
        {
            startGold = x;
            playerGoldText.text = startGold.ToString();
        }, targetGold, duration);
    }

    public void PlayerTurn()
    {
        HideSpells();
        enemyTurn.SetActive(false);

        playerTurn.transform.localScale = Vector3.zero;
        playerTurn.SetActive(true);

        Sequence playerTurnSequence = DOTween.Sequence();
        playerTurnSequence.Append(playerTurn.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack));
        playerTurnSequence.AppendInterval(1f);
        playerTurnSequence.Append(playerTurn.transform.DOScale(0f, 0.5f).SetEase(Ease.InBack));
        playerTurnSequence.OnComplete(() =>
        {
            ShowSpells();
            playerTurn.SetActive(false);
        });

    }
    public void ShowSpells()
    {
        foreach (GameObject slot in spellSlots)
        {
            slot.GetComponent<Button>().interactable = true;
        }
        questionSpell.GetComponent<Button>().interactable = true;
    }
    public void HideSpells()
    {
        foreach (GameObject slot in spellSlots)
        {
            slot.GetComponent<Button>().interactable = false;
        }
        questionSpell.GetComponent<Button>().interactable = false;
    }


    public void EnemyTurn()
    {
        playerTurn.SetActive(false);

        enemyTurn.transform.localScale = Vector3.zero;
        enemyTurn.SetActive(true);

        Sequence enemyTurnSequence = DOTween.Sequence();
        enemyTurnSequence.Append(enemyTurn.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack));
        enemyTurnSequence.AppendInterval(1f);
        enemyTurnSequence.Append(enemyTurn.transform.DOScale(0f, 0.5f).SetEase(Ease.InBack));
        enemyTurnSequence.OnComplete(() => enemyTurn.SetActive(false));

        HideSpells();
    }
    public void UpdateProgressBar()
    {
        float fillAmount = Mathf.Clamp01(LevelManager.Instance.levelProgress / maxProgress);

        Image progressImage = progressBar.GetComponent<Image>();
        progressImage.fillAmount = fillAmount;

        RectTransform playerRect = progressPlayer.GetComponent<RectTransform>();
        RectTransform endRect = progressEnd.GetComponent<RectTransform>();

        float maxOffset = endRect.anchoredPosition.x - playerProgressStartPos.x;

        float newX = playerProgressStartPos.x + (maxOffset * fillAmount);

        playerRect.anchoredPosition = new Vector2(newX, playerRect.anchoredPosition.y);
    }
    public void DrawSlots(List<int> slots)
    {
        for (int i = 0; i < slots.Count; i++)
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

    // Bar Handlers
    // Updates bar with new value with animation
    public void DrawHealth(float totalHealth, float currHealth)
    {
        Vector3 scale = healthBar.transform.localScale;
        float ratio = currHealth / totalHealth;
        healthBar.transform.DOScaleX(ratio, 0.5f).SetEase(Ease.OutQuad);
    }
    public void DrawBossHealth(float totalBossHealth, float currBossHealth)
    {
        Vector3 scale = bossBar.transform.localScale;
        float ratio = currBossHealth / totalBossHealth;
        bossBar.transform.DOScaleX(ratio, 0.5f).SetEase(Ease.OutQuad);
    }
    public void DrawMana(int totalMana, int currMana)
    {
        Vector3 scale = manaBar.transform.localScale;
        float ratio = currMana / (float)totalMana;
        manaBar.transform.DOScaleX(ratio, 0.5f).SetEase(Ease.OutQuad);
    }
    private void InstantiateIntroBanner()
    {
        Transform parent = introBanner.gameObject.transform;
        introBannerPrefab = Instantiate(LevelManager.Instance.intro, parent, false);
    }
    public void FadeIntro()
    {
        InstantiateIntroBanner();
        introBanner.gameObject.SetActive(true);

        Sequence introSequence = DOTween.Sequence();

        introSequence.Append(introBanner.DOFade(1f, 1f).SetEase(Ease.Linear));

        introSequence.AppendInterval(1.5f);

        introSequence.Append(introBanner.DOFade(0f, 1f).SetEase(Ease.Linear));

        introSequence.OnComplete(() =>
        {
            Destroy(introBannerPrefab);
        });
    }
    public void AnimateStartPopup()
    {
        HideSpells();
        startPopup.transform.localScale = Vector3.zero;
        startPopup.SetActive(true);

        Sequence popupSequence = DOTween.Sequence();

        popupSequence.Append(startPopup.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack));

        popupSequence.AppendInterval(3f);

        popupSequence.Append(startPopup.transform.DOScale(0f, 0.5f).SetEase(Ease.InBack));

        popupSequence.OnComplete(() =>
        {
            startPopup.SetActive(false);
            LevelManager.Instance.inCutscene = false;
            ShowSpells();
        });

    }

    public void AnimateBossPopup()
    {
        HideSpells();
        bossPopup.transform.localScale = Vector3.zero;
        bossPopup.SetActive(true);

        Sequence popupSequence = DOTween.Sequence();

        popupSequence.Append(bossPopup.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack));

        popupSequence.AppendInterval(1f);

        popupSequence.Append(bossPopup.transform.DOScale(0f, 0.5f).SetEase(Ease.InBack));

        popupSequence.OnComplete(() =>
        {
            PlayerTurn();
            bossPopup.SetActive(false);
        });
    }
    public void ShowBossDataPopIn(string bossName)
    {
        bossNameText.text = bossName;
        bossInfo.transform.localScale = Vector3.zero;
        bossInfo.SetActive(true);
        Sequence popupSequence = DOTween.Sequence();

        popupSequence.Append(bossInfo.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack));

    }
    public void ShowBossDataPopOut()
    {
        Sequence popupSequence = DOTween.Sequence();
        popupSequence.Append(bossInfo.transform.DOScale(0f, 0.5f).SetEase(Ease.InBack));

        popupSequence.OnComplete(() => bossInfo.SetActive(false));

    }

    // Flashes screen with "Out of Mana" message
    public void FlashOOM()
    {
        if (oomFlashFlag) return;

        oomFlashFlag = true;

        oom.gameObject.SetActive(true);

        CanvasGroup cg = oom.GetComponent<CanvasGroup>();

        cg.alpha = 0f;

        Sequence flashSequence = DOTween.Sequence();
        for (int i = 0; i < 5; i++)
        {
            flashSequence.Append(cg.DOFade(1f, 0.2f));
            flashSequence.Append(cg.DOFade(0f, 0.2f));
        }

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
        Transform overlayTransform = spellSlots[slotIndex].transform.Find("CooldownOverlay");
        Image overlayImage = overlayTransform.GetComponent<Image>();
        overlayImage.gameObject.SetActive(true);

        Button slotButton = spellSlots[slotIndex].GetComponent<Button>();

        slotButton.interactable = false;

        overlayImage.fillAmount = 1f;

        overlayImage.DOFillAmount(0f, duration)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                overlayImage.gameObject.SetActive(false);
                if (!LevelManager.Instance.bossCSFlag) slotButton.interactable = true;
            });
    }

    // Overlays the GameOver screen
    public void DrawGameOver()
    {
        QuestionManager.Instance.gameObject.SetActive(false);
        remainingTriesText.text = CampaignManager.Instance.currCampaign.RemainingTries.ToString();
        if (CampaignManager.Instance.currCampaign.RemainingTries == 0)
        {
            tryAgainText.text = "Restart";

        }
        else
        {
            tryAgainText.text = "Try Again";
        }
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

        if (!levelLoadOverlay.gameObject.activeSelf)
            levelLoadOverlay.gameObject.SetActive(true);

        levelLoadOverlay.alpha = 1f;

        levelLoadOverlay.DOFade(0f, duration)
        .SetEase(Ease.Linear)
        .OnComplete(() =>
        {
            levelLoadOverlay.gameObject.SetActive(false);
            isFadingFlag = false;
            onComplete?.Invoke();
        });
    }

    public Tween FadeToBlack(float duration, System.Action onComplete = null)
    {
        isFadingFlag = true;

        if (!levelLoadOverlay.gameObject.activeSelf)
            levelLoadOverlay.gameObject.SetActive(true);

        levelLoadOverlay.alpha = 0f;

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
