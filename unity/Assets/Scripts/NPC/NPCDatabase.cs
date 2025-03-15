using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NPCDatabase", menuName = "ScriptableObjects/NPCDatabase", order = 3)]
public class NPCDatabase : ScriptableObject
{
    public List<NPC> npcList;
}

[System.Serializable]
public class NPC
{
    public string name;

    public Sprite portrait;

    public GameObject prefab;
}