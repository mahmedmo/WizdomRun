using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MenuMusicManager : MonoBehaviour
{
    public static MenuMusicManager Instance { get; private set; }
    public AudioClip mainMenuMusic;
    public AudioClip buttonClickClip;
    private AudioSource musicAudioSource;
    private AudioSource sfxAudioSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            musicAudioSource = GetComponent<AudioSource>();
            if (musicAudioSource == null)
            {
                musicAudioSource = gameObject.AddComponent<AudioSource>();
            }
            musicAudioSource.loop = true;

            sfxAudioSource = gameObject.AddComponent<AudioSource>();
            sfxAudioSource.playOnAwake = false;
            sfxAudioSource.loop = false;
            SceneManager.sceneLoaded += OnSceneLoaded;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        AddButtonListeners();
        musicAudioSource.clip = mainMenuMusic;
        musicAudioSource.Play();
    }
    void Update()
    {
        string activeSceneName = SceneManager.GetActiveScene().name;
        if (activeSceneName != "MainMenu" && activeSceneName != "UserMenu")
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Destroy(gameObject);
        }
    }

    private void AddButtonListeners()
    {
        Button[] buttons = Object.FindObjectsByType<Button>(FindObjectsSortMode.None);
        foreach (Button button in buttons)
        {
            button.onClick.RemoveListener(PlayButtonClickSound);
            button.onClick.AddListener(PlayButtonClickSound);
        }
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AddButtonListeners();
    }

    public void PlayButtonClickSound()
    {
        if (buttonClickClip != null)
        {
            sfxAudioSource.PlayOneShot(buttonClickClip);
        }
    }
}