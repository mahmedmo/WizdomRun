using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SpellDatabase", menuName = "ScriptableObjects/SpellDatabase", order = 1)]
public class SpellDatabase : ScriptableObject
{
    public List<SpellData> spells;
}

[System.Serializable]
public class SpellData
{
    public int spellID;
    public string spellName;
    public int spellDamage;

    public Sprite spellSprite;
    public GameObject spellPrefab;


}