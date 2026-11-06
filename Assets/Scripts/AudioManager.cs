using UnityEngine;

/// <summary>
/// Manages global background music and sound effects across the game.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip gameplayBGM;
    [SerializeField] private AudioClip menuBGM;
    [SerializeField] private AudioClip scrapPickupSFX;
    [SerializeField] private AudioClip laserShootSFX;
    [SerializeField] private AudioClip playerDeathSFX;
    [SerializeField] private AudioClip asteroidDeathSFX;
    [SerializeField] private AudioClip enemyDeathSFX;

    [Header("Volume Settings")]
    [Range(0f, 1f)][SerializeField] private float musicVolume = 1.0f;
    [Range(0f, 1f)][SerializeField] private float sfxVolume = 1.0f;

    private void Awake()
    {
        // Singleton pattern to ensure only one AudioManager exists and persists across scenes
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // Apply initial volume settings
        ApplyVolumes();
    }

    public void ApplyVolumes()
    {
        if (musicSource != null) musicSource.volume = musicVolume;
        if (sfxSource != null) sfxSource.volume = sfxVolume;
    }

    // Music Controls 
    public void PlayGameplayMusic()
    {
        PlayMusic(gameplayBGM, true);
    }

    public void PlayMenuMusic()
    {
        PlayMusic(menuBGM, true);
    }

    private void PlayMusic(AudioClip clip, bool loop)
    {
        if (musicSource == null || clip == null) return;

        if (musicSource.clip == clip && musicSource.isPlaying) return; // Don't restart if already playing

        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }

    // Sound Effects Controls 
    public void PlayScrapPickup() => PlaySFX(scrapPickupSFX);
    public void PlayLaserShoot() => PlaySFX(laserShootSFX);
    public void PlayPlayerDeath() => PlaySFX(playerDeathSFX);
    public void PlayAsteroidDeath() => PlaySFX(asteroidDeathSFX);
    public void PlayEnemyDeath() => PlaySFX(enemyDeathSFX);

    private void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip, sfxVolume);
        }
    }
}