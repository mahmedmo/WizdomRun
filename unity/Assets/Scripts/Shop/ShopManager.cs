using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Linq;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    [Header("References")]
    public ShopDatabase shopDatabase;
    public GameObject shopInterface;
    public GameObject[] shopItems = new GameObject[6];
    public Button buyShopButton;

    [Header("Lock Sprite")]
    public Sprite lockSprite;
    public TextMeshProUGUI buyFeedback;
    private ShopItem[] displayedItems;
    private int selectedShopItemId = -1;

    public bool shopOpen { get; set; } = false;
    private bool openedFlag = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    void Start()
    {
        openedFlag = false;
        shopInterface.SetActive(false);
        buyShopButton.gameObject.SetActive(false);
    }

    // Picks 6 random items from the database, populates the shop UI,
    // locks items  (if bought/not met the affinity requirement)
    public void PopulateShopItems()
    {
        List<ShopItem> allItems = shopDatabase.shopItems;

        List<ShopItem> randomSelection = allItems
            .OrderBy(x => Random.value)
            .Take(6)
            .ToList();

        displayedItems = new ShopItem[6];

        for (int i = 0; i < shopItems.Length; i++)
        {
            ShopItem itemData = randomSelection[i];
            displayedItems[i] = itemData;

            Transform iconTf = shopItems[i].transform.Find("ItemIcon");
            Transform nameTf = shopItems[i].transform.Find("ItemName");
            Transform descrTf = shopItems[i].transform.Find("ItemDescr");
            Transform costTf = shopItems[i].transform.Find("ItemCost");
            Transform clickableTf = shopItems[i].transform.Find("Clickable");

            Image iconImg = iconTf.GetComponent<Image>();
            TMP_Text nameText = nameTf.GetComponent<TMP_Text>();
            TMP_Text descrText = descrTf.GetComponent<TMP_Text>();
            TMP_Text costText = costTf.GetComponent<TMP_Text>();
            Button clickableBtn = clickableTf.GetComponent<Button>();

            // Populate UI fields
            iconImg.sprite = itemData.icon;
            nameText.text = itemData.name;
            descrText.text = itemData.description;
            costText.text = itemData.cost.ToString();

            bool shouldLock = ShouldLockItem(itemData);

            clickableBtn.interactable = !shouldLock;
            if (shouldLock)
            {

                Transform lockIcon = shopItems[i].transform.Find("LockIcon");
                if (lockIcon != null)
                {
                    lockIcon.gameObject.SetActive(true);
                    Image lockIconImg = lockIcon.GetComponent<Image>();
                    if (lockIconImg && lockSprite) lockIconImg.sprite = lockSprite;
                }
            }

            clickableBtn.onClick.RemoveAllListeners();
            int indexCopy = i;
            clickableBtn.onClick.AddListener(() => OnSelectItem(indexCopy));
        }
    }

    // Lock item logic
    private bool ShouldLockItem(ShopItem item)
    {
        PlayerClass? playerAffinity = PlayerMonitor.Instance.GetAffinity();

        // IDs 0..3 => check wizard classes
        if (item.id == 0 && playerAffinity == PlayerClass.Fire) return true;
        if (item.id == 1 && playerAffinity == PlayerClass.Water) return true;
        if (item.id == 2 && playerAffinity == PlayerClass.Air) return true;
        if (item.id == 3 && playerAffinity == PlayerClass.Earth) return true;

        // IDs 4..7 => check potencies
        if (item.id == 4 && (PlayerMonitor.Instance.HasPotencyOne() || playerAffinity == PlayerClass.None)) return true;
        if (item.id == 5 && (PlayerMonitor.Instance.HasPotencyTwo() || playerAffinity == PlayerClass.None)) return true;
        if (item.id == 6 && (PlayerMonitor.Instance.HasPotencyThree() || playerAffinity == PlayerClass.None)) return true;
        if (item.id == 7 && (PlayerMonitor.Instance.HasPotencyFour() || playerAffinity == PlayerClass.None)) return true;

        return false;
    }

    private void OnSelectItem(int index)
    {
        // If we click the same item, deselect it
        if (selectedShopItemId == index)
        {
            DeselectItem(index);
            selectedShopItemId = -1;
            buyShopButton.gameObject.SetActive(false);
        }
        else
        {
            if (selectedShopItemId >= 0)
            {
                DeselectItem(selectedShopItemId);
            }

            selectedShopItemId = index;
            HighlightItem(index, true);

            buyShopButton.gameObject.SetActive(true);
            buyShopButton.transform.localScale = Vector3.zero;
            buyShopButton.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        }
    }

    private void HighlightItem(int index, bool highlight)
    {
        Transform frameTf = shopItems[index].transform.Find("ItemFrame");
        if (frameTf)
        {
            Image frameImg = frameTf.GetComponent<Image>();
            if (frameImg)
            {
                frameImg.color = highlight ? Color.yellow : Color.white;
            }
        }
    }

    private void DeselectItem(int index)
    {
        HighlightItem(index, false);
    }
    private void ResetSelection()
    {
        for (int i = 0; i < shopItems.Length; i++)
        {
            HighlightItem(i, false);
        }
        selectedShopItemId = -1;
    }

    // Buy Button Handler
    public void OnBuyButtonClicked()
    {
        if (selectedShopItemId < 0) return;

        ShopItem selectedItem = displayedItems[selectedShopItemId];
        int playerGold = PlayerMonitor.Instance.GetGold();

        if (playerGold >= selectedItem.cost)
        {
            LevelManager.Instance.SpendGold(selectedItem.cost);

            // Set player's affinity for items 0-3
            if (selectedItem.id >= 0 && selectedItem.id <= 3)
            {
                switch (selectedItem.id)
                {
                    case 0:
                        PlayerMonitor.Instance.SetAffinity(PlayerClass.Fire);
                        break;
                    case 1:
                        PlayerMonitor.Instance.SetAffinity(PlayerClass.Water);
                        break;
                    case 2:
                        PlayerMonitor.Instance.SetAffinity(PlayerClass.Air);
                        break;
                    case 3:
                        PlayerMonitor.Instance.SetAffinity(PlayerClass.Earth);
                        break;
                }
            }
            // Set player's potency for items 4-7
            else if (selectedItem.id >= 4 && selectedItem.id <= 7)
            {
                switch (selectedItem.id)
                {
                    case 4:
                        PlayerMonitor.Instance.SetPotencyOne();
                        break;
                    case 5:
                        PlayerMonitor.Instance.SetPotencyTwo();
                        break;
                    case 6:
                        PlayerMonitor.Instance.SetPotencyThree();
                        break;
                    case 7:
                        PlayerMonitor.Instance.SetPotencyFour();
                        break;
                }
            }
            // Lock the purchased item so it can't be bought again
            Transform clickableTf = shopItems[selectedShopItemId].transform.Find("Clickable");
            if (clickableTf != null)
            {
                Button clickableBtn = clickableTf.GetComponent<Button>();
                if (clickableBtn != null) clickableBtn.interactable = false;
            }

            // Show lock icon
            Transform lockIcon = shopItems[selectedShopItemId].transform.Find("LockIcon");
            if (lockIcon != null)
            {
                lockIcon.gameObject.SetActive(true);
                Image lockIconImg = lockIcon.GetComponent<Image>();
                if (lockIconImg && lockSprite) lockIconImg.sprite = lockSprite;
            }

            // Marrowlin's dialogyue upon purchase
            buyFeedback.text = "<color=green>Thanks for the bones!</color>";
            buyFeedback.gameObject.SetActive(true);
            buyFeedback.color = Color.green;
            buyFeedback.transform.localScale = Vector3.one;

            buyFeedback.DOKill();

            DOTween.Sequence()
                .AppendInterval(1.5f)
                .Append(buyFeedback.DOFade(0f, 0.5f))
                .OnComplete(() =>
                {
                    buyFeedback.gameObject.SetActive(false);
                    Color tempColor = buyFeedback.color;
                    tempColor.a = 1f;
                    buyFeedback.color = tempColor;
                });
            ResetSelection();

        }
        else
        {
            // Error buying feedback
            buyFeedback.gameObject.SetActive(true);
            buyFeedback.text = "<color=red>Not enough gold!</color>";

            Color feedbackColor = buyFeedback.color;
            feedbackColor.a = 1f;
            buyFeedback.color = feedbackColor;
            buyFeedback.transform.localScale = Vector3.one;

            buyFeedback.DOKill();

            DOTween.Sequence()
                .AppendInterval(1.5f)
                .Append(buyFeedback.DOFade(0f, 0.5f))
                .OnComplete(() =>
                {
                    buyFeedback.gameObject.SetActive(false);
                    feedbackColor.a = 1f;
                    buyFeedback.color = feedbackColor;
                });
            ResetSelection();

        }
    }

    public void AnimateShopPopup()
    {
        InterfaceManager.Instance.HideInterface();
        if (!openedFlag) PopulateShopItems();
        shopOpen = true;
        openedFlag = true;
        shopInterface.SetActive(true);
        shopInterface.transform.localScale = Vector3.zero;

        // Adjusted scale to inspector (manually tested values)
        shopInterface.transform.DOScale(new Vector3(0.035f, 0.05f, 1f), 0.4f)
            .SetEase(Ease.OutBack);
        ResetSelection();
        buyFeedback.gameObject.SetActive(false);
        buyShopButton.gameObject.SetActive(false);
        buyShopButton.transform.localScale = Vector3.zero;
    }
    public void CloseShopPopup()
    {
        InterfaceManager.Instance.ShowInterface();
        shopOpen = false;
        ResetSelection();
        shopInterface.transform.DOScale(0f, 0.3f)
            .SetEase(Ease.InBack)
            .OnComplete(() => shopInterface.SetActive(false));
    }
}