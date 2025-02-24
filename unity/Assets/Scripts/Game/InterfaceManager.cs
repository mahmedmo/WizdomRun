using UnityEngine;
using DG.Tweening;

public class InterfaceManager : MonoBehaviour
{
    public static InterfaceManager Instance { get; private set; }
    public GameObject Interface;

    public GameObject healthBar;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Interface = GameObject.Find("UserInterface");
    }
    public void DrawHealth(int totalHealth, int currHealth)
    {
        Vector3 scale = healthBar.transform.localScale;
        float ratio = currHealth / (float)totalHealth;
        healthBar.transform.DOScaleX(ratio, 0.5f).SetEase(Ease.OutQuad);
    }
}
