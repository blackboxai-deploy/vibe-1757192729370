using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// UIManager - Mengelola semua UI elements dan navigasi dalam game
/// Dirancang dengan UI yang child-friendly: button besar, warna cerah, navigasi sederhana
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("Main UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject gameplayPanel;
    public GameObject pausePanel;
    public GameObject settingsPanel;
    public GameObject rewardPanel;
    public GameObject creativityPanel;
    
    [Header("Gameplay UI Elements")]
    public Text levelText;
    public Text timeText;
    public Text starsText;
    public Text coinsText;
    public Slider progressBar;
    public Button hintButton;
    public Button pauseButton;
    public Text progressText; // "2/5 pieces"
    
    [Header("Main Menu UI")]
    public Button playButton;
    public Button creativityButton;
    public Button settingsButton;
    public Text welcomeText;
    public Text playerStatsText;
    
    [Header("Settings UI")]
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Toggle musicToggle;
    public Toggle sfxToggle;
    public Button backButton;
    
    [Header("Reward UI")]
    public Text rewardTitleText;
    public Text starsEarnedText;
    public Text coinsEarnedText;
    public Button nextLevelButton;
    public Button replayButton;
    public Button menuButton;
    public GameObject[] starIcons; // Array of star GameObjects for visual feedback
    
    [Header("Pause UI")]
    public Button resumeButton;
    public Button restartButton;
    public Button mainMenuButton;
    
    [Header("UI Animations")]
    public float panelTransitionDuration = 0.3f;
    public LeanTweenType transitionEase = LeanTweenType.easeOutBack;
    public AnimationCurve buttonScaleCurve = AnimationCurve.EaseInOut(0, 1, 1, 1.2f);
    
    // Private variables
    private GameObject currentPanel;
    private bool isTransitioning = false;
    private Dictionary<string, GameObject> panels = new Dictionary<string, GameObject>();
    
    // Singleton pattern
    public static UIManager Instance;
    
    // Events
    public System.Action OnPlayButtonClicked;
    public System.Action OnCreativityButtonClicked;
    public System.Action OnSettingsOpened;
    public System.Action OnGamePaused;
    public System.Action OnGameResumed;
    
    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            SetupPanelReferences();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        InitializeUI();
        SetupEventListeners();
        ShowMainMenu(); // Default to main menu
    }
    
    private void Update()
    {
        // Handle back button untuk Android
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandleBackButton();
        }
        
        UpdateGameplayUI();
    }
    
    /// <summary>
    /// Setup references ke semua UI panels
    /// </summary>
    private void SetupPanelReferences()
    {
        panels["MainMenu"] = mainMenuPanel;
        panels["Gameplay"] = gameplayPanel;
        panels["Pause"] = pausePanel;
        panels["Settings"] = settingsPanel;
        panels["Reward"] = rewardPanel;
        panels["Creativity"] = creativityPanel;
        
        Debug.Log("📋 UI Panels Setup Complete");
    }
    
    /// <summary>
    /// Inisialisasi UI dengan pengaturan child-friendly
    /// </summary>
    private void InitializeUI()
    {
        // Setup button colors dan sizes yang child-friendly
        SetupChildFriendlyButtons();
        
        // Hide semua panel kecuali main menu
        HideAllPanels();
        
        // Load player stats untuk main menu
        UpdatePlayerStats();
        
        // Setup welcome message
        if (welcomeText != null)
        {
            welcomeText.text = "Selamat Datang di\nPuzzle Adventure!";
        }
        
        Debug.Log("🎨 Child-Friendly UI Initialized");
    }
    
    /// <summary>
    /// Setup button dengan style child-friendly
    /// </summary>
    private void SetupChildFriendlyButtons()
    {
        // Get semua buttons dalam scene
        Button[] allButtons = FindObjectsOfType<Button>();
        
        foreach (Button button in allButtons)
        {
            // Add hover effects
            AddButtonHoverEffect(button);
            
            // Add click sound
            AddButtonClickSound(button);
            
            // Ensure button sizes minimal 80px untuk touch target
            RectTransform buttonRect = button.GetComponent<RectTransform>();
            if (buttonRect != null)
            {
                Vector2 minSize = new Vector2(120f, 80f); // Minimum size untuk anak-anak
                if (buttonRect.sizeDelta.x < minSize.x || buttonRect.sizeDelta.y < minSize.y)
                {
                    buttonRect.sizeDelta = Vector2.Max(buttonRect.sizeDelta, minSize);
                }
            }
        }
    }
    
    /// <summary>
    /// Tambah hover effect ke button
    /// </summary>
    private void AddButtonHoverEffect(Button button)
    {
        // Add EventTrigger untuk hover effects
        UnityEngine.EventSystems.EventTrigger eventTrigger = button.gameObject.GetComponent<UnityEngine.EventSystems.EventTrigger>();
        if (eventTrigger == null)
        {
            eventTrigger = button.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
        }
        
        // Pointer Enter (hover start)
        UnityEngine.EventSystems.EventTrigger.Entry pointerEnter = new UnityEngine.EventSystems.EventTrigger.Entry
        {
            eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter
        };
        pointerEnter.callback.AddListener((eventData) => { AnimateButtonHover(button, true); });
        eventTrigger.triggers.Add(pointerEnter);
        
        // Pointer Exit (hover end)
        UnityEngine.EventSystems.EventTrigger.Entry pointerExit = new UnityEngine.EventSystems.EventTrigger.Entry
        {
            eventID = UnityEngine.EventSystems.EventTriggerType.PointerExit
        };
        pointerExit.callback.AddListener((eventData) => { AnimateButtonHover(button, false); });
        eventTrigger.triggers.Add(pointerExit);
        
        // Pointer Down (click feedback)
        UnityEngine.EventSystems.EventTrigger.Entry pointerDown = new UnityEngine.EventSystems.EventTrigger.Entry
        {
            eventID = UnityEngine.EventSystems.EventTriggerType.PointerDown
        };
        pointerDown.callback.AddListener((eventData) => { AnimateButtonClick(button); });
        eventTrigger.triggers.Add(pointerDown);
    }
    
    /// <summary>
    /// Add click sound ke button
    /// </summary>
    private void AddButtonClickSound(Button button)
    {
        button.onClick.AddListener(() => {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayButtonClick();
            }
        });
    }
    
    /// <summary>
    /// Animate button hover effect
    /// </summary>
    private void AnimateButtonHover(Button button, bool isHovering)
    {
        float targetScale = isHovering ? 1.1f : 1f;
        LeanTween.scale(button.gameObject, Vector3.one * targetScale, 0.1f)
            .setEase(LeanTweenType.easeOutQuad);
    }
    
    /// <summary>
    /// Animate button click effect
    /// </summary>
    private void AnimateButtonClick(Button button)
    {
        // Quick scale down then back up
        LeanTween.scale(button.gameObject, Vector3.one * 0.95f, 0.05f)
            .setEase(LeanTweenType.easeOutQuad)
            .setOnComplete(() => {
                LeanTween.scale(button.gameObject, Vector3.one, 0.1f)
                    .setEase(LeanTweenType.easeOutBack);
            });
    }
    
    /// <summary>
    /// Setup event listeners untuk semua buttons
    /// </summary>
    private void SetupEventListeners()
    {
        // Main Menu Buttons
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayButtonPressed);
        
        if (creativityButton != null)
            creativityButton.onClick.AddListener(OnCreativityButtonPressed);
        
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OnSettingsButtonPressed);
        
        // Gameplay Buttons
        if (hintButton != null)
            hintButton.onClick.AddListener(OnHintButtonPressed);
        
        if (pauseButton != null)
            pauseButton.onClick.AddListener(OnPauseButtonPressed);
        
        // Pause Menu Buttons
        if (resumeButton != null)
            resumeButton.onClick.AddListener(OnResumeButtonPressed);
        
        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestartButtonPressed);
        
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(OnMainMenuButtonPressed);
        
        // Reward Screen Buttons
        if (nextLevelButton != null)
            nextLevelButton.onClick.AddListener(OnNextLevelButtonPressed);
        
        if (replayButton != null)
            replayButton.onClick.AddListener(OnReplayButtonPressed);
        
        if (menuButton != null)
            menuButton.onClick.AddListener(OnMainMenuButtonPressed);
        
        // Settings
        if (backButton != null)
            backButton.onClick.AddListener(OnBackButtonPressed);
        
        // Setup sliders
        SetupSettingsSliders();
        
        // Subscribe to game events
        SubscribeToGameEvents();
        
        Debug.Log("🔗 Event Listeners Setup Complete");
    }
    
    /// <summary>
    /// Setup sliders untuk settings
    /// </summary>
    private void SetupSettingsSliders()
    {
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            musicVolumeSlider.value = AudioManager.Instance != null ? AudioManager.Instance.GetMusicVolume() : 0.7f;
        }
        
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
            sfxVolumeSlider.value = AudioManager.Instance != null ? AudioManager.Instance.GetSFXVolume() : 0.8f;
        }
        
        if (musicToggle != null)
        {
            musicToggle.onValueChanged.AddListener(OnMusicToggled);
            musicToggle.isOn = AudioManager.Instance != null ? AudioManager.Instance.IsMusicEnabled() : true;
        }
        
        if (sfxToggle != null)
        {
            sfxToggle.onValueChanged.AddListener(OnSFXToggled);
            sfxToggle.isOn = AudioManager.Instance != null ? AudioManager.Instance.IsSFXEnabled() : true;
        }
    }
    
    /// <summary>
    /// Subscribe ke game events
    /// </summary>
    private void SubscribeToGameEvents()
    {
        // GameManager events
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnLevelComplete += OnLevelCompleted;
            GameManager.Instance.OnStarsEarned += OnStarsEarned;
            GameManager.Instance.OnCoinsEarned += OnCoinsEarned;
        }
        
        // PuzzleManager events jika ada
        PuzzleManager puzzleManager = FindObjectOfType<PuzzleManager>();
        if (puzzleManager != null)
        {
            puzzleManager.OnProgressUpdate += OnPuzzleProgressUpdated;
            puzzleManager.OnTimeUpdate += OnTimeUpdated;
            puzzleManager.OnPuzzleComplete += OnPuzzleCompleted;
        }
    }
    
    /// <summary>
    /// Update UI elements selama gameplay
    /// </summary>
    private void UpdateGameplayUI()
    {
        if (currentPanel != gameplayPanel) return;
        
        // Update level info
        if (levelText != null && GameManager.Instance != null)
        {
            levelText.text = $"Level {GameManager.Instance.currentLevel}";
        }
        
        // Update coins dan stars
        if (coinsText != null && GameManager.Instance != null)
        {
            coinsText.text = GameManager.Instance.totalCoins.ToString();
        }
        
        if (starsText != null && GameManager.Instance != null)
        {
            starsText.text = GameManager.Instance.totalStars.ToString();
        }
        
        // Update hint button
        PuzzleManager puzzleManager = FindObjectOfType<PuzzleManager>();
        if (hintButton != null && puzzleManager != null)
        {
            hintButton.interactable = puzzleManager.CanUseHint();
            
            // Update hint button text
            Text hintText = hintButton.GetComponentInChildren<Text>();
            if (hintText != null)
            {
                int hintsLeft = puzzleManager.GetMaxHints() - puzzleManager.GetHintsUsed();
                hintText.text = $"Petunjuk ({hintsLeft})";
            }
        }
    }
    
    /// <summary>
    /// Hide semua panels
    /// </summary>
    private void HideAllPanels()
    {
        foreach (var panel in panels.Values)
        {
            if (panel != null)
            {
                panel.SetActive(false);
            }
        }
    }
    
    /// <summary>
    /// Show panel dengan animasi
    /// </summary>
    public void ShowPanel(string panelName)
    {
        if (isTransitioning) return;
        
        if (panels.ContainsKey(panelName))
        {
            StartCoroutine(TransitionToPanel(panels[panelName]));
        }
        else
        {
            Debug.LogWarning($"⚠️ Panel '{panelName}' not found!");
        }
    }
    
    /// <summary>
    /// Transition ke panel dengan animasi smooth
    /// </summary>
    private IEnumerator TransitionToPanel(GameObject newPanel)
    {
        isTransitioning = true;
        
        // Hide current panel dengan slide out animation
        if (currentPanel != null)
        {
            yield return StartCoroutine(SlideOutPanel(currentPanel));
            currentPanel.SetActive(false);
        }
        
        // Show new panel dengan slide in animation
        if (newPanel != null)
        {
            newPanel.SetActive(true);
            yield return StartCoroutine(SlideInPanel(newPanel));
            currentPanel = newPanel;
        }
        
        isTransitioning = false;
    }
    
    /// <summary>
    /// Slide out animation untuk panel
    /// </summary>
    private IEnumerator SlideOutPanel(GameObject panel)
    {
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        if (panelRect != null)
        {
            Vector3 startPos = panelRect.localPosition;
            Vector3 endPos = new Vector3(-Screen.width, startPos.y, startPos.z);
            
            LeanTween.moveLocal(panel, endPos, panelTransitionDuration)
                .setEase(LeanTweenType.easeInBack);
            
            yield return new WaitForSeconds(panelTransitionDuration);
            
            // Reset position
            panelRect.localPosition = startPos;
        }
    }
    
    /// <summary>
    /// Slide in animation untuk panel
    /// </summary>
    private IEnumerator SlideInPanel(GameObject panel)
    {
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        if (panelRect != null)
        {
            Vector3 finalPos = panelRect.localPosition;
            Vector3 startPos = new Vector3(Screen.width, finalPos.y, finalPos.z);
            
            panelRect.localPosition = startPos;
            
            LeanTween.moveLocal(panel, finalPos, panelTransitionDuration)
                .setEase(transitionEase);
            
            yield return new WaitForSeconds(panelTransitionDuration);
        }
    }
    
    // === PUBLIC PANEL METHODS ===
    
    public void ShowMainMenu()
    {
        ShowPanel("MainMenu");
        UpdatePlayerStats();
    }
    
    public void ShowGameplay()
    {
        ShowPanel("Gameplay");
    }
    
    public void ShowPauseMenu()
    {
        ShowPanel("Pause");
    }
    
    public void ShowSettings()
    {
        ShowPanel("Settings");
    }
    
    public void ShowRewardScreen()
    {
        ShowPanel("Reward");
    }
    
    public void ShowCreativityMode()
    {
        ShowPanel("Creativity");
    }
    
    // === BUTTON EVENT HANDLERS ===
    
    private void OnPlayButtonPressed()
    {
        OnPlayButtonClicked?.Invoke();
        
        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadGameplay();
        }
        else
        {
            SceneManager.LoadScene("GameplayPuzzle");
        }
    }
    
    private void OnCreativityButtonPressed()
    {
        OnCreativityButtonClicked?.Invoke();
        
        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadCreativityMode();
        }
        else
        {
            SceneManager.LoadScene("CreativityMode");
        }
    }
    
    private void OnSettingsButtonPressed()
    {
        OnSettingsOpened?.Invoke();
        ShowSettings();
    }
    
    private void OnHintButtonPressed()
    {
        PuzzleManager puzzleManager = FindObjectOfType<PuzzleManager>();
        if (puzzleManager != null)
        {
            puzzleManager.UseHint();
        }
    }
    
    private void OnPauseButtonPressed()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PauseGame();
        }
        
        OnGamePaused?.Invoke();
        ShowPauseMenu();
    }
    
    private void OnResumeButtonPressed()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResumeGame();
        }
        
        OnGameResumed?.Invoke();
        ShowGameplay();
    }
    
    private void OnRestartButtonPressed()
    {
        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.ReloadCurrentScene();
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
    
    private void OnMainMenuButtonPressed()
    {
        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadMainMenu();
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
    
    private void OnNextLevelButtonPressed()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadLevel(GameManager.Instance.currentLevel + 1);
        }
    }
    
    private void OnReplayButtonPressed()
    {
        OnRestartButtonPressed(); // Same as restart
    }
    
    private void OnBackButtonPressed()
    {
        ShowMainMenu();
    }
    
    // === SETTINGS EVENT HANDLERS ===
    
    private void OnMusicVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(value);
        }
    }
    
    private void OnSFXVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSFXVolume(value);
        }
    }
    
    private void OnMusicToggled(bool isOn)
    {
        if (AudioManager.Instance != null)
        {
            if (AudioManager.Instance.IsMusicEnabled() != isOn)
            {
                AudioManager.Instance.ToggleMusic();
            }
        }
    }
    
    private void OnSFXToggled(bool isOn)
    {
        if (AudioManager.Instance != null)
        {
            if (AudioManager.Instance.IsSFXEnabled() != isOn)
            {
                AudioManager.Instance.ToggleSFX();
            }
        }
    }
    
    // === GAME EVENT HANDLERS ===
    
    private void OnLevelCompleted(int starsEarned)
    {
        Debug.Log($"🏆 Level completed with {starsEarned} stars!");
    }
    
    private void OnStarsEarned(int stars)
    {
        // Animate stars in reward screen
        if (rewardPanel != null && rewardPanel.activeInHierarchy)
        {
            AnimateStarsReward(stars);
        }
    }
    
    private void OnCoinsEarned(int coins)
    {
        // Update coin display dengan animasi
        StartCoroutine(AnimateCoinIncrease(coins));
    }
    
    private void OnPuzzleProgressUpdated(int correct, int total)
    {
        if (progressBar != null)
        {
            float progress = (float)correct / total;
            LeanTween.value(progressBar.gameObject, progressBar.value, progress, 0.3f)
                .setOnUpdate((float val) => progressBar.value = val);
        }
        
        if (progressText != null)
        {
            progressText.text = $"{correct}/{total}";
        }
    }
    
    private void OnTimeUpdated(float currentTime)
    {
        if (timeText != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            timeText.text = $"{minutes:00}:{seconds:00}";
        }
    }
    
    private void OnPuzzleCompleted(PuzzleCompletionData completionData)
    {
        // Update reward screen dengan completion data
        UpdateRewardScreen(completionData);
        
        // Auto show reward screen setelah delay
        StartCoroutine(ShowRewardScreenAfterDelay(1f));
    }
    
    // === HELPER METHODS ===
    
    /// <summary>
    /// Update player stats di main menu
    /// </summary>
    private void UpdatePlayerStats()
    {
        if (playerStatsText != null && GameManager.Instance != null)
        {
            playerStatsText.text = $"Level: {GameManager.Instance.currentLevel}\n" +
                                 $"⭐ {GameManager.Instance.totalStars}\n" +
                                 $"💰 {GameManager.Instance.totalCoins}";
        }
    }
    
    /// <summary>
    /// Update reward screen dengan completion data
    /// </summary>
    private void UpdateRewardScreen(PuzzleCompletionData data)
    {
        if (rewardTitleText != null)
        {
            rewardTitleText.text = "Level Selesai!";
        }
        
        if (starsEarnedText != null)
        {
            starsEarnedText.text = data.starsEarned.ToString();
        }
        
        if (coinsEarnedText != null)
        {
            coinsEarnedText.text = $"+{data.coinsEarned}";
        }
    }
    
    /// <summary>
    /// Animate stars reward
    /// </summary>
    private void AnimateStarsReward(int starsCount)
    {
        for (int i = 0; i < starIcons.Length && i < starsCount; i++)
        {
            if (starIcons[i] != null)
            {
                StartCoroutine(AnimateStarIcon(starIcons[i], i * 0.2f));
            }
        }
    }
    
    /// <summary>
    /// Animate individual star icon
    /// </summary>
    private IEnumerator AnimateStarIcon(GameObject starIcon, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        starIcon.transform.localScale = Vector3.zero;
        starIcon.SetActive(true);
        
        LeanTween.scale(starIcon, Vector3.one, 0.5f)
            .setEase(LeanTweenType.easeOutBack);
        
        // Play star sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayStarEarned();
        }
    }
    
    /// <summary>
    /// Animate coin increase
    /// </summary>
    private IEnumerator AnimateCoinIncrease(int coinsAdded)
    {
        if (coinsText == null) yield break;
        
        int startCoins = GameManager.Instance != null ? GameManager.Instance.totalCoins - coinsAdded : 0;
        int endCoins = startCoins + coinsAdded;
        
        float duration = 1f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            int currentCoins = Mathf.RoundToInt(Mathf.Lerp(startCoins, endCoins, t));
            coinsText.text = currentCoins.ToString();
            yield return null;
        }
        
        coinsText.text = endCoins.ToString();
    }
    
    /// <summary>
    /// Show reward screen setelah delay
    /// </summary>
    private IEnumerator ShowRewardScreenAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowRewardScreen();
    }
    
    /// <summary>
    /// Handle back button (Android)
    /// </summary>
    private void HandleBackButton()
    {
        if (currentPanel == gameplayPanel)
        {
            OnPauseButtonPressed();
        }
        else if (currentPanel == pausePanel)
        {
            OnResumeButtonPressed();
        }
        else if (currentPanel == settingsPanel || currentPanel == rewardPanel)
        {
            ShowMainMenu();
        }
        else if (currentPanel == mainMenuPanel)
        {
            // Quit application (Android)
            #if UNITY_ANDROID && !UNITY_EDITOR
            Application.Quit();
            #endif
        }
    }
}