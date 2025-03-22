using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MusicDatabase", menuName = "ScriptableObjects/MusicDatabase", order = 10)]
public class MusicDatabase : ScriptableObject
{
    public List<LevelMusic> levelMusics;
}
[System.Serializable]
public class LevelMusic
{
    public int level;
    public AudioClip normalMusic;
    public AudioClip bossMusic;
}