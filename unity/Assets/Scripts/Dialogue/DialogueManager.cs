using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    public DialogueTyper dialogueTyper;
    public GameObject touchButtonOverlay;
    public GameObject portrait;
    public GameObject dialogueStatus;
    public Sprite dialogueProgress;
    public Sprite dialogueDone;
    public NPCDatabase npcDatabase;
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
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void ShowDialogue()
    {
        for (int i = 0; i < npcDialogue.Length; i++)
        {
            npcDialogue[i].gameObject.SetActive(true);
        }
        Debug.Log("SHOW");
    }
    public void HideDialogue()
    {
        for (int i = 0; i < npcDialogue.Length; i++)
        {
            npcDialogue[i].gameObject.SetActive(false);
        }
        Debug.Log("HIDE");
    }
    public void HideChoices()
    {
        for (int i = 0; i < playerChoices.Length; i++)
        {
            playerChoices[i].gameObject.SetActive(false);
        }

    }

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
    public void SetDialogue(Dialogue dlg, CutsceneType ct)
    {
        touchButtonOverlay.SetActive(false);
        dialogue = dlg;
        cutsceneType = ct;
        portrait.GetComponent<Image>().sprite = npcDatabase.npcList[dialogue.npcId].portrait;
        currDialogueIdx = 0;
        currChoiceIdx = 0;
        dialogueStatus.GetComponent<Image>().sprite = dialogueProgress;
        StartDialogue();
    }

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
            // Wait for continue interaction
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
    public void OnChoicePressed()
    {
        if (currDialogueIdx != dialogue.dialogueText.Length - 1)
        {
            HideChoices();
            NextDialogue();
            return;
        }
        switch (cutsceneType)
        {
            case CutsceneType.Start:
                LevelManager.Instance.StartLevel();
                break;
        }
    }

}