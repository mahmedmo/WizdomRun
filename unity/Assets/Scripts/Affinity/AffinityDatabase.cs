using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "AffinityDatabase", menuName = "ScriptableObjects/AffinityDatabase", order = 7)]
public class AffinityDatabase : ScriptableObject
{
    public List<Affinity> affinityList;
}

[System.Serializable]
public class Affinity
{
    public int id;
    public PlayerClass playerClass;
    public Sprite staff;
    public Sprite hat;

}