using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CutsceneDatabase", menuName = "ScriptableObjects/CutsceneDatabase", order = 2)]
public class CutsceneDatabase : ScriptableObject
{
    public List<Cutscene> cutsceneList;
}

[System.Serializable]
public class Cutscene
{
    public int cutsceneId;
    public CutsceneType cutsceneType;
    public bool firstEncounter;
    public int campaignLevel;
    public Dialogue dialogue;
}

[System.Serializable]
public class Dialogue
{
    public int npcId;
    public string[] dialogueText;

    public ChoiceSet[] choiceSets;

}

[System.Serializable]
public class ChoiceSet
{
    public int choiceIdx;
    public Choice[] choices;
}

[System.Serializable]
public class Choice
{
    public string text;
    public Sprite icon;
}