using UnityEngine;
using UnityEngine.Tilemaps;

public class NPCSavedMonitor : MonoBehaviour
{
    void Start()
    {
        LevelManager.Instance.SavedCS();
    }

}