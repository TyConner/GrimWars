using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;

    [Header("Music Tracks")]
    public AudioClip titleMusic;
    public AudioClip grimoireMusic;
    public AudioClip tutorialMusic;

    public AudioClip firedungeonMusic;
    public AudioClip icedungeonMusic;
    public AudioClip aerodungeonMusic;
    public AudioClip lightdungeonMusic;
    public AudioClip darkdungeonMusic;
    public AudioClip timedRoomMusic;

    public AudioClip bossMusic;
    public AudioClip finalBossMusic;

    public AudioClip winMusic;
    public AudioClip loseMusic;

    private AudioClip currentClip;
    private AudioClip previousMusic;
    private Stack<AudioClip> musicStack = new Stack<AudioClip>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "TitleScreen")
        {
            PlayMusic(titleMusic);
        }
        else if (scene.name == "Grimoire Selection")
        {
            PlayMusic(grimoireMusic);
        }
        else if (scene.name == "FireDungeon")
        {
            PlayMusic(firedungeonMusic);
        }
        else if (scene.name == "IceDungeon")
        {
            PlayMusic(icedungeonMusic);
        }
        else if (scene.name == "AeroDungeon")
        {
            PlayMusic(aerodungeonMusic);
        }
        else if (scene.name == "LightDungeon")
        {
            PlayMusic(lightdungeonMusic);
        }
        else if (scene.name == "DarkDungeon")
        {
            PlayMusic(darkdungeonMusic);
        }
        else if (scene.name == "Credits")
        {
            PlayMusic(titleMusic);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;

        if (currentClip != null)
        {
            musicStack.Push(currentClip);
        }

        audioSource.Stop();

        currentClip = clip;
        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void PopMusic()
    {
        if (musicStack.Count == 0) return;

        AudioClip previous = musicStack.Pop();

        audioSource.Stop();

        currentClip = previous;
        audioSource.clip = previous;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void StopMusic()
    {
        audioSource.Stop();
    }

    public void PauseMusic()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
        }
    }

    public void ResumeMusic()
    {
        if (audioSource.clip != null)
        {
            audioSource.UnPause();
        }
    }

    public void PlayChallengeMusic()
    {
        if (audioSource.clip == timedRoomMusic) return;

        previousMusic = audioSource.clip;

        audioSource.clip = timedRoomMusic;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void StopChallengeMusicAndRestorePrevious()
    {
        audioSource.Stop();

        if (previousMusic != null)
        {
            audioSource.clip = previousMusic;
            audioSource.loop = true;
            audioSource.Play();
        }
    }
}
