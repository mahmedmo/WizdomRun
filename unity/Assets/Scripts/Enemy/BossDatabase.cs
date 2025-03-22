using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "BossDatabase", menuName = "ScriptableObjects/BossDatabase", order = 6)]
public class BossDatabase : ScriptableObject
{
    public List<Boss> bossList;
}

[System.Serializable]
public class Boss
{
    public int id;
    public int campaignLevel;
    public string name;
    public int damage;
    public int health;
    public int goldDrop;
    public bool airborne;
    public GameObject prefab;

}