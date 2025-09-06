using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AudioManager - Mengelola semua audio dalam game (musik dan sound effect)
/// Dirancang khusus untuk game anak-anak dengan suara yang menyenangkan
/// </summary>
public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    
    [Header("Background Music")]
    public AudioClip mainMenuMusic;
    public AudioClip gameplayMusic;
    public AudioClip creativityModeMusic;
    public AudioClip rewardScreenMusic;
    
    [Header("Sound Effects")]
    public AudioClip buttonClickSFX;
    public AudioClip puzzleCompleteSFX;
    public AudioClip starEarnedSFX;
    public AudioClip coinEarnedSFX;
    public AudioClip dragStartSFX;
    public AudioClip dragDropSFX;
    public AudioClip wrongAnswerSFX;
    public AudioClip levelCompleteSFX;
    public AudioClip celebrationSFX;
    public AudioClip drawingSFX;
    
    [Header("Volume Settings")]
    [Range(0f, 1f)]
    public float masterVolume = 1f;
    [Range(0f, 1f)]
    public float musicVolume = 0.7f;
    [Range(0f, 1f)]
    public float sfxVolume = 0.8f;
    
    // Singleton pattern
    public static AudioManager Instance;
    
    private bool musicEnabled = true;
    private bool sfxEnabled = true;
    private AudioClip currentlyPlayingMusic;
    
    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetupAudioSources();
            LoadAudioSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        // Mulai dengan musik main menu
        PlayMusic(mainMenuMusic);
    }
    
    /// <summary>
    /// Setup audio sources dengan konfigurasi optimal untuk game anak
    /// </summary>
    private void SetupAudioSources()
    {
        // Setup Music Source
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
        }
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.volume = musicVolume;
        
        // Setup SFX Source
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
        }
        sfxSource.loop = false;
        sfxSource.playOnAwake = false;
        sfxSource.volume = sfxVolume;
        
        Debug.Log("🔊 Audio Manager Setup Complete");
    }
    
    /// <summary>
    /// Load pengaturan audio dari PlayerPrefs
    /// </summary>
    private void LoadAudioSettings()
    {
        musicEnabled = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;
        sfxEnabled = PlayerPrefs.GetInt("SFXEnabled", 1) == 1;
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.8f);
        
        UpdateVolumeSettings();
        
        Debug.Log($"🎵 Audio Settings Loaded - Music: {musicEnabled}, SFX: {sfxEnabled}");
    }
    
    /// <summary>
    /// Simpan pengaturan audio ke PlayerPrefs
    /// </summary>
    public void SaveAudioSettings()
    {
        PlayerPrefs.SetInt("MusicEnabled", musicEnabled ? 1 : 0);
        PlayerPrefs.SetInt("SFXEnabled", sfxEnabled ? 1 : 0);
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.Save();
        
        Debug.Log("💾 Audio Settings Saved");
    }
    
    /// <summary>
    /// Update semua pengaturan volume
    /// </summary>
    private void UpdateVolumeSettings()
    {
        if (musicSource != null)
        {
            musicSource.volume = musicEnabled ? musicVolume * masterVolume : 0f;
        }
        
        if (sfxSource != null)
        {
            sfxSource.volume = sfxEnabled ? sfxVolume * masterVolume : 0f;
        }
    }
    
    /// <summary>
    /// Putar musik background
    /// </summary>
    /// <param name="musicClip">Audio clip musik yang akan dimainkan</param>
    public void PlayMusic(AudioClip musicClip)
    {
        if (musicClip == null || musicSource == null) return;
        
        // Jangan putar ulang musik yang sama
        if (currentlyPlayingMusic == musicClip && musicSource.isPlaying) return;
        
        currentlyPlayingMusic = musicClip;
        musicSource.clip = musicClip;
        
        if (musicEnabled)
        {
            musicSource.Play();
            Debug.Log($"🎵 Playing Music: {musicClip.name}");
        }
    }
    
    /// <summary>
    /// Stop musik background
    /// </summary>
    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
            currentlyPlayingMusic = null;
            Debug.Log("🔇 Music Stopped");
        }
    }
    
    /// <summary>
    /// Putar sound effect
    /// </summary>
    /// <param name="sfxClip">Audio clip SFX yang akan dimainkan</param>
    public void PlaySFX(AudioClip sfxClip)
    {
        if (sfxClip == null || sfxSource == null || !sfxEnabled) return;
        
        sfxSource.PlayOneShot(sfxClip, sfxVolume * masterVolume);
        Debug.Log($"🔊 Playing SFX: {sfxClip.name}");
    }
    
    /// <summary>
    /// Putar sound effect dengan volume custom
    /// </summary>
    /// <param name="sfxClip">Audio clip SFX</param>
    /// <param name="customVolume">Volume custom (0-1)</param>
    public void PlaySFX(AudioClip sfxClip, float customVolume)
    {
        if (sfxClip == null || sfxSource == null || !sfxEnabled) return;
        
        float finalVolume = Mathf.Clamp01(customVolume) * masterVolume;
        sfxSource.PlayOneShot(sfxClip, finalVolume);
        Debug.Log($"🔊 Playing SFX: {sfxClip.name} at volume {finalVolume}");
    }
    
    // === QUICK ACCESS METHODS UNTUK SOUND EFFECTS GAME ===
    
    /// <summary>
    /// Putar suara klik tombol
    /// </summary>
    public void PlayButtonClick()
    {
        PlaySFX(buttonClickSFX);
    }
    
    /// <summary>
    /// Putar suara puzzle selesai
    /// </summary>
    public void PlayPuzzleComplete()
    {
        PlaySFX(puzzleCompleteSFX);
    }
    
    /// <summary>
    /// Putar suara mendapat bintang
    /// </summary>
    public void PlayStarEarned()
    {
        PlaySFX(starEarnedSFX);
    }
    
    /// <summary>
    /// Putar suara mendapat koin
    /// </summary>
    public void PlayCoinEarned()
    {
        PlaySFX(coinEarnedSFX);
    }
    
    /// <summary>
    /// Putar suara mulai drag
    /// </summary>
    public void PlayDragStart()
    {
        PlaySFX(dragStartSFX);
    }
    
    /// <summary>
    /// Putar suara drop objek
    /// </summary>
    public void PlayDragDrop()
    {
        PlaySFX(dragDropSFX);
    }
    
    /// <summary>
    /// Putar suara jawaban salah (lembut untuk anak-anak)
    /// </summary>
    public void PlayWrongAnswer()
    {
        PlaySFX(wrongAnswerSFX);
    }
    
    /// <summary>
    /// Putar suara level complete dengan celebrasi
    /// </summary>
    public void PlayLevelComplete()
    {
        PlaySFX(levelCompleteSFX);
        // Delay untuk celebrasi
        StartCoroutine(PlayCelebrationAfterDelay(1f));
    }
    
    /// <summary>
    /// Putar suara celebrasi setelah delay
    /// </summary>
    private IEnumerator PlayCelebrationAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        PlaySFX(celebrationSFX);
    }
    
    /// <summary>
    /// Putar suara drawing untuk mode kreativitas
    /// </summary>
    public void PlayDrawing()
    {
        PlaySFX(drawingSFX);
    }
    
    // === MUSIK SWITCHING BERDASARKAN SCENE ===
    
    /// <summary>
    /// Switch ke musik main menu
    /// </summary>
    public void PlayMainMenuMusic()
    {
        PlayMusic(mainMenuMusic);
    }
    
    /// <summary>
    /// Switch ke musik gameplay
    /// </summary>
    public void PlayGameplayMusic()
    {
        PlayMusic(gameplayMusic);
    }
    
    /// <summary>
    /// Switch ke musik mode kreativitas
    /// </summary>
    public void PlayCreativityMusic()
    {
        PlayMusic(creativityModeMusic);
    }
    
    /// <summary>
    /// Switch ke musik reward screen
    /// </summary>
    public void PlayRewardMusic()
    {
        PlayMusic(rewardScreenMusic);
    }
    
    // === TOGGLE CONTROLS UNTUK UI ===
    
    /// <summary>
    /// Toggle musik on/off
    /// </summary>
    public void ToggleMusic()
    {
        musicEnabled = !musicEnabled;
        UpdateVolumeSettings();
        SaveAudioSettings();
        
        if (musicEnabled && currentlyPlayingMusic != null)
        {
            musicSource.Play();
        }
        else if (!musicEnabled)
        {
            musicSource.Pause();
        }
        
        Debug.Log($"🎵 Music {(musicEnabled ? "Enabled" : "Disabled")}");
    }
    
    /// <summary>
    /// Toggle SFX on/off
    /// </summary>
    public void ToggleSFX()
    {
        sfxEnabled = !sfxEnabled;
        UpdateVolumeSettings();
        SaveAudioSettings();
        
        Debug.Log($"🔊 SFX {(sfxEnabled ? "Enabled" : "Disabled")}");
    }
    
    /// <summary>
    /// Set master volume (0-1)
    /// </summary>
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        UpdateVolumeSettings();
        SaveAudioSettings();
        Debug.Log($"🔊 Master Volume: {masterVolume}");
    }
    
    /// <summary>
    /// Set music volume (0-1)
    /// </summary>
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        UpdateVolumeSettings();
        SaveAudioSettings();
        Debug.Log($"🎵 Music Volume: {musicVolume}");
    }
    
    /// <summary>
    /// Set SFX volume (0-1)
    /// </summary>
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        UpdateVolumeSettings();
        SaveAudioSettings();
        Debug.Log($"🔊 SFX Volume: {sfxVolume}");
    }
    
    // === GETTERS ===
    
    public bool IsMusicEnabled() { return musicEnabled; }
    public bool IsSFXEnabled() { return sfxEnabled; }
    public float GetMasterVolume() { return masterVolume; }
    public float GetMusicVolume() { return musicVolume; }
    public float GetSFXVolume() { return sfxVolume; }
}