using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// GameManager - Pusat kontrol untuk seluruh game
/// Mengelola state game, progress level, dan koordinasi sistem lainnya
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Game Configuration")]
    public int totalLevels = 20;
    public float levelProgressSaveInterval = 1f;
    
    [Header("Player Progress")]
    public int currentLevel = 1;
    public int totalStars = 0;
    public int totalCoins = 0;
    
    [Header("Game State")]
    public bool isGamePaused = false;
    public bool isLevelComplete = false;
    
    // Singleton pattern untuk akses mudah dari script lain
    public static GameManager Instance;
    
    // Events untuk komunikasi antar sistem
    public System.Action<int> OnLevelComplete;
    public System.Action<int> OnStarsEarned;
    public System.Action<int> OnCoinsEarned;
    public System.Action OnGamePaused;
    public System.Action OnGameResumed;
    
    private void Awake()
    {
        // Singleton setup - hanya satu GameManager yang boleh ada
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        LoadPlayerProgress();
    }
    
    /// <summary>
    /// Inisialisasi awal game - dipanggil saat game pertama kali dimulai
    /// </summary>
    private void InitializeGame()
    {
        // Setup konfigurasi dasar game
        Application.targetFrameRate = 60; // Limit FPS untuk mobile
        Screen.sleepTimeout = SleepTimeout.NeverSleep; // Cegah layar mati saat main
        
        Debug.Log("🎮 Game Manager Initialized - Welcome to Creative Puzzle Adventure!");
    }
    
    /// <summary>
    /// Load progress pemain dari PlayerPrefs
    /// </summary>
    private void LoadPlayerProgress()
    {
        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        totalStars = PlayerPrefs.GetInt("TotalStars", 0);
        totalCoins = PlayerPrefs.GetInt("TotalCoins", 0);
        
        Debug.Log($"📊 Player Progress Loaded - Level: {currentLevel}, Stars: {totalStars}, Coins: {totalCoins}");
    }
    
    /// <summary>
    /// Simpan progress pemain ke PlayerPrefs
    /// </summary>
    public void SavePlayerProgress()
    {
        PlayerPrefs.SetInt("CurrentLevel", currentLevel);
        PlayerPrefs.SetInt("TotalStars", totalStars);
        PlayerPrefs.SetInt("TotalCoins", totalCoins);
        PlayerPrefs.Save();
        
        Debug.Log("💾 Player Progress Saved!");
    }
    
    /// <summary>
    /// Panggil ketika level selesai dengan sukses
    /// </summary>
    /// <param name="starsEarned">Jumlah bintang yang didapat (1-3)</param>
    /// <param name="coinsEarned">Jumlah koin yang didapat</param>
    public void CompleteLevel(int starsEarned, int coinsEarned)
    {
        isLevelComplete = true;
        
        // Tambah stars dan coins
        totalStars += starsEarned;
        totalCoins += coinsEarned;
        
        // Unlock level selanjutnya jika belum
        if (currentLevel <= totalLevels)
        {
            currentLevel++;
        }
        
        // Trigger events untuk sistem lain
        OnLevelComplete?.Invoke(starsEarned);
        OnStarsEarned?.Invoke(starsEarned);
        OnCoinsEarned?.Invoke(coinsEarned);
        
        // Auto save progress
        SavePlayerProgress();
        
        Debug.log($"🌟 Level Complete! Stars: {starsEarned}, Coins: {coinsEarned}");
        
        // Tampilkan layar reward setelah delay singkat
        StartCoroutine(ShowRewardScreenAfterDelay(2f));
    }
    
    /// <summary>
    /// Tampilkan layar reward dengan delay untuk animasi
    /// </summary>
    private IEnumerator ShowRewardScreenAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("RewardScreen");
    }
    
    /// <summary>
    /// Load level spesifik
    /// </summary>
    public void LoadLevel(int levelNumber)
    {
        if (levelNumber <= totalLevels && levelNumber > 0)
        {
            currentLevel = levelNumber;
            isLevelComplete = false;
            SceneManager.LoadScene("GameplayPuzzle");
            Debug.Log($"🚀 Loading Level {levelNumber}");
        }
    }
    
    /// <summary>
    /// Kembali ke main menu
    /// </summary>
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Debug.Log("🏠 Returning to Main Menu");
    }
    
    /// <summary>
    /// Buka mode kreativitas
    /// </summary>
    public void OpenCreativityMode()
    {
        SceneManager.LoadScene("CreativityMode");
        Debug.Log("🎨 Opening Creativity Mode");
    }
    
    /// <summary>
    /// Pause game - untuk menu pause atau panggilan telepon
    /// </summary>
    public void PauseGame()
    {
        isGamePaused = true;
        Time.timeScale = 0f; // Stop semua animasi dan timer
        OnGamePaused?.Invoke();
        Debug.Log("⏸️ Game Paused");
    }
    
    /// <summary>
    /// Resume game setelah pause
    /// </summary>
    public void ResumeGame()
    {
        isGamePaused = false;
        Time.timeScale = 1f; // Resume normal speed
        OnGameResumed?.Invoke();
        Debug.Log("▶️ Game Resumed");
    }
    
    /// <summary>
    /// Kurangi koin - untuk pembelian item di shop
    /// </summary>
    public bool SpendCoins(int amount)
    {
        if (totalCoins >= amount)
        {
            totalCoins -= amount;
            SavePlayerProgress();
            Debug.Log($"💰 Spent {amount} coins. Remaining: {totalCoins}");
            return true;
        }
        else
        {
            Debug.Log("❌ Not enough coins!");
            return false;
        }
    }
    
    /// <summary>
    /// Tambah koin - untuk reward atau bonus
    /// </summary>
    public void AddCoins(int amount)
    {
        totalCoins += amount;
        SavePlayerProgress();
        OnCoinsEarned?.Invoke(amount);
        Debug.Log($"💰 Earned {amount} coins. Total: {totalCoins}");
    }
    
    /// <summary>
    /// Reset progress - untuk testing atau tombol reset
    /// </summary>
    public void ResetProgress()
    {
        currentLevel = 1;
        totalStars = 0;
        totalCoins = 0;
        SavePlayerProgress();
        Debug.Log("🔄 Progress Reset!");
    }
    
    /// <summary>
    /// Handle ketika aplikasi di-pause (panggilan telepon, home button, dll)
    /// </summary>
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            PauseGame();
            SavePlayerProgress(); // Auto save saat aplikasi di-pause
        }
        else
        {
            ResumeGame();
        }
    }
    
    /// <summary>
    /// Handle ketika aplikasi kehilangan focus
    /// </summary>
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            SavePlayerProgress(); // Auto save saat lose focus
        }
    }
}