using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
[CreateAssetMenu(fileName = "CSTilemapDatabase", menuName = "ScriptableObjects/CSTilemapDatabase", order = 5)]
public class CSTilemapDatabase : ScriptableObject
{
    public List<CSTilemap> csTilemaps;
}

[System.Serializable]
public class CSTilemap
{
    public int cutsceneId;
    public int levelId;
    public Tilemap tilemap;
}
