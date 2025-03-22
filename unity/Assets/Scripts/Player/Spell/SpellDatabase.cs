using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SpellDatabase", menuName = "ScriptableObjects/SpellDatabase", order = 1)]
public class SpellDatabase : ScriptableObject
{
    public List<SpellData> spellList;
}

[System.Serializable]
public class SpellData
{
    public int id;
    public string name;
    public int damage;
    public float speed;
    public int manaCost;
    public bool airborne;
    public Sprite icon;
    public GameObject prefab;

}