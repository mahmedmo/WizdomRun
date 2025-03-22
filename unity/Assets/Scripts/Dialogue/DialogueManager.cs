using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    public DialogueTyper dialogueTyper;
    public GameObject touchButtonOverlay;
    public GameObject portrait;
    public GameObject npcInfo;

    public TextMeshProUGUI npcName;
    public GameObject dialogueStatus;
    public Sprite dialogueProgress;
    public Sprite dialogueDone;
    public GameObject skipBtn;
    public NPCDatabase npcDatabase;
    public NPCDatabase savedNPCDatabase;

    public Transform[] npcDialogue;
    public Transform[] playerChoices;

    private Dialogue dialogue;
    private CutsceneType cutsceneType;

    private int currChoiceIdx = -1;
    private int currDialogueIdx = -1;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void ShowDialogue()
    {
        InterfaceManager.Instance.HideInterface();
        for (int i = 0; i < npcDialogue.Length; i++)
        {
            npcDialogue[i].gameObject.SetActive(true);
        }
        npcInfo.SetActive(true);
        skipBtn.SetActive(true);
    }
    public void HideDialogue()
    {
        InterfaceManager.Instance.ShowInterface();
        for (int i = 0; i < npcDialogue.Length; i++)
        {
            npcDialogue[i].gameObject.SetActive(false);
        }
        npcInfo.SetActive(false);
        skipBtn.SetActive(false);
    }
    public void HideChoices()
    {
        for (int i = 0; i < playerChoices.Length; i++)
        {
            playerChoices[i].gameObject.SetActive(false);
        }
    }

    // Finds the choice for the associated choice index
    public void ShowChoices()
    {
        for (int i = 0; i < dialogue.choiceSets[currChoiceIdx].choices.Length; i++)
        {
            TextMeshProUGUI choiceText = playerChoices[i].Find("ChoiceText").gameObject.GetComponent<TextMeshProUGUI>();
            Image choiceIcon = playerChoices[i].Find("ChoiceIcon").gameObject.GetComponent<Image>();
            choiceText.text = dialogue.choiceSets[currChoiceIdx].choices[i].text;
            choiceIcon.sprite = dialogue.choiceSets[currChoiceIdx].choices[i].icon;
            playerChoices[i].gameObject.SetActive(true);
        }
    }

    // Assigns the dialogue, the portrait for the npc speaking as well as resetting indexes/GameObjects
    public void SetDialogue(Dialogue dlg, CutsceneType ct)
    {
        touchButtonOverlay.SetActive(false);
        dialogue = dlg;
        cutsceneType = ct;
        if (ct == CutsceneType.Saved)
        {
            portrait.GetComponent<Image>().sprite = savedNPCDatabase.npcList[CampaignManager.Instance.GetLevel() - 1].portrait;
            npcName.text = savedNPCDatabase.npcList[CampaignManager.Instance.GetLevel() - 1].name;
        }
        else
        {
            portrait.GetComponent<Image>().sprite = npcDatabase.npcList[dialogue.npcId].portrait;
            npcName.text = npcDatabase.npcList[dialogue.npcId].name;
        }

        currDialogueIdx = 0;
        currChoiceIdx = 0;
        dialogueStatus.GetComponent<Image>().sprite = dialogueProgress;
        StartDialogue();
    }

    // Outputs the indexed dialogue as it were typed
    public void StartDialogue()
    {

        string dialogueText = dialogue.dialogueText[currDialogueIdx];
        dialogueTyper.DisplayDialogue(dialogueText, () =>
        {
            // When dialogue is finished typing change the DialogueProgress sprite to complete
            dialogueStatus.GetComponent<Image>().sprite = dialogueDone;
            if (currDialogueIdx == dialogue.choiceSets[currChoiceIdx].choiceIdx)
            {
                ShowChoices();
                FindNextChoice();
                return;
            }
            // Wait for user's tap
            touchButtonOverlay.SetActive(true);
        });
    }

    public void FindNextChoice()
    {
        if (currChoiceIdx + 1 >= dialogue.choiceSets.Length)
        {
            currChoiceIdx = 0;
            return;
        }
        currChoiceIdx += 1;
    }

    public void NextDialogue()
    {
        currDialogueIdx++;
        dialogueStatus.GetComponent<Image>().sprite = dialogueProgress;
        touchButtonOverlay.SetActive(false);
        StartDialogue();
    }

    // Skips to the final dialogue index
    public void OnSkipDialogue()
    {
        currDialogueIdx = dialogue.dialogueText.Length - 2;
        currChoiceIdx = dialogue.choiceSets.Length - 1;
        HideChoices();
        NextDialogue();
        skipBtn.SetActive(false);
    }

    public void OnChoicePressed()
    {
        if (currDialogueIdx != dialogue.dialogueText.Length - 1)
        {
            HideChoices();
            NextDialogue();
            return;
        }
        // Route handler for final choices in a dialogue sequence
        switch (cutsceneType)
        {
            case CutsceneType.Start:
                LevelManager.Instance.StartLevel();
                break;
            case CutsceneType.Saved:
                LevelManager.Instance.LoadNextLevel();
                break;
            case CutsceneType.Elementalist:
                LevelManager.Instance.StartEShop();
                break;
        }
    }

}