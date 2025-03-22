using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ShopDatabase", menuName = "ScriptableObjects/ShopDatabase", order = 7)]
public class ShopDatabase : ScriptableObject
{
    public List<ShopItem> shopItems;
}

[System.Serializable]
public class ShopItem
{
    public int id;
    public string name;
    public string description;
    public int cost;
    public Sprite icon;

}