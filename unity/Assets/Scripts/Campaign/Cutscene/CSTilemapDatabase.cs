using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
[CreateAssetMenu(fileName = "CSTilemapDatabase", menuName = "ScriptableObjects/CSTilemapDatabase", order = 5)]
public class CSTilemapDatabase : ScriptableObject
{
    public List<CSTilemap> cutsceneAreas;
}

[System.Serializable]
public class CSTilemap
{
    public int cutsceneId;
    public int levelId = -1;
    public CutsceneType cutsceneType;
    public GameObject tilemap;
}
