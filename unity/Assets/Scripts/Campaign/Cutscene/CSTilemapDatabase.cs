using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CSTilemapDatabase", menuName = "ScriptableObjects/CSTilemapDatabase", order = 5)]
public class CSTilemapDatabase : ScriptableObject
{
    public List<CSTilemap> cutsceneAreas;
}

[System.Serializable]
public class CSTilemap
{
    public int cutsceneId;
    public CutsceneType cutsceneType;
    public GameObject tilemap;
}
