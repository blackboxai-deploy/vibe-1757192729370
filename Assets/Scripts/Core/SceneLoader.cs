using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// SceneLoader - Mengelola transisi antar scene dengan loading screen dan animasi
/// Dirancang untuk memberikan pengalaman smooth untuk anak-anak
/// </summary>
public class SceneLoader : MonoBehaviour
{
    [Header("Loading Screen")]
    public GameObject loadingScreen;
    public Slider progressBar;
    public Text loadingText;
    public Image loadingIcon;
    
    [Header("Loading Messages")]
    public string[] loadingMessages = {
        "Mempersiapkan puzzle seru...",
        "Menyiapkan warna-warna cerah...",
        "Mengatur mainan...",
        "Hampir siap bermain!",
        "Sebentar lagi..."
    };
    
    [Header("Animation Settings")]
    public float minLoadingTime = 1.5f; // Minimal waktu loading untuk animasi
    public float iconRotationSpeed = 180f; // Kecepatan rotasi icon loading
    public AnimationCurve progressCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    // Singleton pattern
    public static SceneLoader Instance;
    
    private bool isLoading = false;
    private Coroutine loadingIconRotation;
    
    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Pastikan loading screen hidden di start
            if (loadingScreen != null)
            {
                loadingScreen.SetActive(false);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// Load scene dengan loading screen dan animasi smooth
    /// </summary>
    /// <param name="sceneName">Nama scene yang akan di-load</param>
    public void LoadScene(string sceneName)
    {
        if (!isLoading)
        {
            StartCoroutine(LoadSceneAsync(sceneName));
        }
    }
    
    /// <summary>
    /// Load scene berdasarkan index
    /// </summary>
    /// <param name="sceneIndex">Index scene dalam build settings</param>
    public void LoadScene(int sceneIndex)
    {
        if (!isLoading)
        {
            StartCoroutine(LoadSceneAsync(sceneIndex));
        }
    }
    
    /// <summary>
    /// Async loading dengan progress bar dan animasi
    /// </summary>
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        isLoading = true;
        
        // Tampilkan loading screen
        ShowLoadingScreen();
        
        // Putar audio loading jika ada
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }
        
        // Mulai loading scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;
        
        float elapsedTime = 0f;
        int currentMessageIndex = 0;
        
        // Loading loop
        while (elapsedTime < minLoadingTime || asyncLoad.progress < 0.9f)
        {
            elapsedTime += Time.deltaTime;
            
            // Update progress bar dengan curve yang smooth
            float normalizedTime = elapsedTime / minLoadingTime;
            float actualProgress = Mathf.Max(asyncLoad.progress / 0.9f, normalizedTime);
            float smoothProgress = progressCurve.Evaluate(actualProgress);
            
            UpdateProgressBar(smoothProgress);
            
            // Update loading message secara periodik
            int messageIndex = Mathf.FloorToInt((elapsedTime / minLoadingTime) * loadingMessages.Length);
            messageIndex = Mathf.Clamp(messageIndex, 0, loadingMessages.Length - 1);
            
            if (messageIndex != currentMessageIndex)
            {
                currentMessageIndex = messageIndex;
                UpdateLoadingMessage(loadingMessages[currentMessageIndex]);
            }
            
            yield return null;
        }
        
        // Pastikan progress bar full
        UpdateProgressBar(1f);
        UpdateLoadingMessage("Siap bermain!");
        
        yield return new WaitForSeconds(0.5f); // Delay kecil untuk user experience
        
        // Activate scene
        asyncLoad.allowSceneActivation = true;
        
        // Tunggu scene benar-benar loaded
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        
        // Hide loading screen
        HideLoadingScreen();
        
        isLoading = false;
        
        Debug.Log($"🚀 Scene '{sceneName}' loaded successfully!");
    }
    
    /// <summary>
    /// Async loading dengan scene index
    /// </summary>
    private IEnumerator LoadSceneAsync(int sceneIndex)
    {
        string sceneName = SceneManager.GetSceneByBuildIndex(sceneIndex).name;
        if (string.IsNullOrEmpty(sceneName))
        {
            sceneName = $"Scene {sceneIndex}";
        }
        
        yield return StartCoroutine(LoadSceneAsync(sceneName));
    }
    
    /// <summary>
    /// Tampilkan loading screen dengan animasi
    /// </summary>
    private void ShowLoadingScreen()
    {
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(true);
            
            // Reset progress bar
            if (progressBar != null)
            {
                progressBar.value = 0f;
            }
            
            // Set loading message pertama
            if (loadingMessages.Length > 0)
            {
                UpdateLoadingMessage(loadingMessages[0]);
            }
            
            // Mulai animasi rotasi icon
            if (loadingIcon != null)
            {
                loadingIconRotation = StartCoroutine(RotateLoadingIcon());
            }
            
            Debug.Log("📱 Loading Screen Shown");
        }
    }
    
    /// <summary>
    /// Sembunyikan loading screen
    /// </summary>
    private void HideLoadingScreen()
    {
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(false);
            
            // Stop rotasi icon
            if (loadingIconRotation != null)
            {
                StopCoroutine(loadingIconRotation);
                loadingIconRotation = null;
            }
            
            Debug.Log("📱 Loading Screen Hidden");
        }
    }
    
    /// <summary>
    /// Update progress bar dengan animasi smooth
    /// </summary>
    private void UpdateProgressBar(float progress)
    {
        if (progressBar != null)
        {
            progressBar.value = progress;
        }
    }
    
    /// <summary>
    /// Update loading message text
    /// </summary>
    private void UpdateLoadingMessage(string message)
    {
        if (loadingText != null)
        {
            loadingText.text = message;
            Debug.Log($"📝 Loading: {message}");
        }
    }
    
    /// <summary>
    /// Animasi rotasi untuk loading icon
    /// </summary>
    private IEnumerator RotateLoadingIcon()
    {
        while (loadingIcon != null)
        {
            loadingIcon.transform.Rotate(0, 0, -iconRotationSpeed * Time.deltaTime);
            yield return null;
        }
    }
    
    // === QUICK ACCESS METHODS UNTUK SCENES UTAMA ===
    
    /// <summary>
    /// Kembali ke main menu
    /// </summary>
    public void LoadMainMenu()
    {
        LoadScene("MainMenu");
        
        // Update musik
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMainMenuMusic();
        }
    }
    
    /// <summary>
    /// Load gameplay scene
    /// </summary>
    public void LoadGameplay()
    {
        LoadScene("GameplayPuzzle");
        
        // Update musik
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayGameplayMusic();
        }
    }
    
    /// <summary>
    /// Load creativity mode
    /// </summary>
    public void LoadCreativityMode()
    {
        LoadScene("CreativityMode");
        
        // Update musik
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayCreativityMusic();
        }
    }
    
    /// <summary>
    /// Load reward screen
    /// </summary>
    public void LoadRewardScreen()
    {
        LoadScene("RewardScreen");
        
        // Update musik
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayRewardMusic();
        }
    }
    
    /// <summary>
    /// Reload scene saat ini (untuk restart level)
    /// </summary>
    public void ReloadCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        LoadScene(currentSceneName);
        Debug.Log($"🔄 Reloading scene: {currentSceneName}");
    }
    
    /// <summary>
    /// Load scene selanjutnya dalam build order
    /// </summary>
    public void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("⚠️ No next scene available");
        }
    }
    
    /// <summary>
    /// Load scene sebelumnya dalam build order
    /// </summary>
    public void LoadPreviousScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int previousSceneIndex = currentSceneIndex - 1;
        
        if (previousSceneIndex >= 0)
        {
            LoadScene(previousSceneIndex);
        }
        else
        {
            Debug.Log("⚠️ No previous scene available");
        }
    }
    
    /// <summary>
    /// Check apakah sedang dalam proses loading
    /// </summary>
    public bool IsLoading()
    {
        return isLoading;
    }
    
    /// <summary>
    /// Get nama scene saat ini
    /// </summary>
    public string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }
}