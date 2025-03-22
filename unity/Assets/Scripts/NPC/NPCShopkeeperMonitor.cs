using UnityEngine;
using UnityEngine.Tilemaps;

public class NPCShopkeeperMonitor : MonoBehaviour
{
    void OnMouseDown()
    {
        if (!ShopManager.Instance.shopOpen && GameManager.Instance.isPaused)
            LevelManager.Instance.EShopCS();
    }

}