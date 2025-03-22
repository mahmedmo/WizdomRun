using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Music Database")]
    public MusicDatabase musicDatabase;
    public AudioClip buttonClickClip;
    private AudioSource musicAudioSource;
    private AudioSource sfxAudioSource;
    private int currentLevel = 0;
    private bool isBossMusicPlaying = false;

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
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (LevelManager.Instance.levelStart)
            UpdateMusic();
        Button[] buttons = Object.FindObjectsByType<Button>(FindObjectsSortMode.None);
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() => PlayButtonClickSound());
        }
    }

    void Update()
    {
        // Check if the campaign level has changed.
        int level = CampaignManager.Instance.GetLevel();
        if (level != currentLevel)
        {
            currentLevel = level;
            UpdateMusic();
        }
        // Check if the boss flag has changed.
        bool bossFlag = LevelManager.Instance.bossCSFlag;
        if (bossFlag != isBossMusicPlaying)
        {
            isBossMusicPlaying = bossFlag;
            UpdateMusic();
        }
    }

    private void UpdateMusic()
    {
        if (musicDatabase == null || musicDatabase.levelMusics == null)
        {
            return;
        }

        LevelMusic levelMusic = musicDatabase.levelMusics.Find(lm => lm.level == currentLevel);
        if (levelMusic == null)
        {
            return;
        }

        AudioClip targetClip = !isBossMusicPlaying ? levelMusic.bossMusic : levelMusic.normalMusic;

        if (targetClip == null)
        {
            return;
        }

        if (musicAudioSource.clip != targetClip)
        {
            musicAudioSource.clip = targetClip;
            musicAudioSource.Play();
        }
    }

    public void PlayButtonClickSound()
    {
        if (buttonClickClip != null)
        {
            sfxAudioSource.PlayOneShot(buttonClickClip);
        }
    }
}