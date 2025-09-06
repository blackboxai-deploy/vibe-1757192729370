using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ColorPicker - Sistem pemilihan warna yang child-friendly dengan palette yang cerah dan menarik
/// Mendukung color wheel, preset colors, dan custom color mixing
/// </summary>
public class ColorPicker : MonoBehaviour
{
    [Header("Color Picker UI")]
    public GameObject colorPickerPanel;
    public Image colorPreview;
    public Button colorPickerButton;
    public Button closeButton;
    
    [Header("Color Wheel")]
    public RectTransform colorWheel;
    public Image colorWheelImage;
    public RectTransform colorSelector;
    public bool enableColorWheel = true;
    
    [Header("Preset Color Palette")]
    public RectTransform colorPaletteGrid;
    public GameObject colorButtonPrefab;
    public int paletteColumns = 6;
    public int paletteRows = 4;
    
    [Header("Brightness & Saturation")]
    public Slider brightnessSlider;
    public Slider saturationSlider;
    public Image brightnessPreview;
    public Image saturationPreview;
    
    [Header("Color Mixing")]
    public Button colorMixButton;
    public GameObject colorMixPanel;
    public Button[] mixColors = new Button[3];
    public Button mixResultButton;
    public Text mixInstructions;
    
    [Header("Favorite Colors")]
    public RectTransform favoriteColorsGrid;
    public Button addToFavoritesButton;
    public int maxFavoriteColors = 12;
    
    // Child-friendly color palette
    private Color[] childFriendlyColors = {
        // Bright primary colors
        new Color(1f, 0f, 0f),      // Bright Red
        new Color(0f, 1f, 0f),      // Bright Green  
        new Color(0f, 0f, 1f),      // Bright Blue
        new Color(1f, 1f, 0f),      // Bright Yellow
        new Color(1f, 0f, 1f),      // Bright Magenta
        new Color(0f, 1f, 1f),      // Bright Cyan
        
        // Pastel colors
        new Color(1f, 0.7f, 0.7f),  // Light Pink
        new Color(0.7f, 1f, 0.7f),  // Light Green
        new Color(0.7f, 0.7f, 1f),  // Light Blue
        new Color(1f, 1f, 0.7f),    // Light Yellow
        new Color(1f, 0.7f, 1f),    // Light Magenta
        new Color(0.7f, 1f, 1f),    // Light Cyan
        
        // Rainbow colors
        new Color(1f, 0.5f, 0f),    // Orange
        new Color(0.5f, 0f, 1f),    // Purple
        new Color(1f, 0.4f, 0.7f),  // Hot Pink
        new Color(0.2f, 0.8f, 0.2f),// Lime Green
        new Color(0.9f, 0.6f, 0.1f),// Gold
        new Color(0.6f, 0.3f, 0.0f),// Brown
        
        // Additional fun colors
        new Color(1f, 0.8f, 0.9f),  // Baby Pink
        new Color(0.8f, 1f, 0.9f),  // Mint Green
        new Color(0.9f, 0.9f, 1f),  // Lavender
        new Color(1f, 0.9f, 0.8f),  // Peach
        new Color(0.2f, 0.2f, 0.2f),// Dark Gray
        new Color(0.9f, 0.9f, 0.9f) // Light Gray
    };
    
    // Private variables
    private Color currentColor = Color.red;
    private Color[] favoriteColors = new Color[0];
    private List<Button> colorPaletteButtons = new List<Button>();
    private List<Button> favoriteColorButtons = new List<Button>();
    
    // Color wheel variables
    private bool isDraggingColorWheel = false;
    private RectTransform colorWheelRect;
    
    // Color mixing variables
    private List<Color> mixingColors = new List<Color>();
    private int mixingStep = 0;
    
    // Events
    public System.Action<Color> OnColorSelected;
    public System.Action OnColorPickerOpened;
    public System.Action OnColorPickerClosed;
    public System.Action<Color> OnColorAddedToFavorites;
    
    private void Start()
    {
        InitializeColorPicker();
        SetupUI();
        LoadFavoriteColors();
        SetDefaultColor();
    }
    
    private void Update()
    {
        HandleColorWheelInput();
    }
    
    /// <summary>
    /// Inisialisasi color picker system
    /// </summary>
    private void InitializeColorPicker()
    {
        // Setup default state
        if (colorPickerPanel != null)
        {
            colorPickerPanel.SetActive(false);
        }
        
        if (colorMixPanel != null)
        {
            colorMixPanel.SetActive(false);
        }
        
        // Get color wheel rect transform
        if (colorWheel != null)
        {
            colorWheelRect = colorWheel.GetComponent<RectTransform>();
        }
        
        Debug.Log("🎨 Color Picker Initialized");
    }
    
    /// <summary>
    /// Setup UI elements dan event listeners
    /// </summary>
    private void SetupUI()
    {
        // Main buttons
        if (colorPickerButton != null)
            colorPickerButton.onClick.AddListener(OpenColorPicker);
        
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseColorPicker);
        
        // Color mixing
        if (colorMixButton != null)
            colorMixButton.onClick.AddListener(OpenColorMixing);
        
        // Favorites
        if (addToFavoritesButton != null)
            addToFavoritesButton.onClick.AddListener(AddCurrentColorToFavorites);
        
        // Sliders
        SetupColorSliders();
        
        // Create color palette
        CreateColorPalette();
        
        // Create favorite colors grid
        CreateFavoriteColorsGrid();
        
        Debug.Log("🎛️ Color Picker UI Setup Complete");
    }
    
    /// <summary>
    /// Setup color adjustment sliders
    /// </summary>
    private void SetupColorSliders()
    {
        if (brightnessSlider != null)
        {
            brightnessSlider.value = 1f; // Default full brightness
            brightnessSlider.onValueChanged.AddListener(OnBrightnessChanged);
        }
        
        if (saturationSlider != null)
        {
            saturationSlider.value = 1f; // Default full saturation
            saturationSlider.onValueChanged.AddListener(OnSaturationChanged);
        }
    }
    
    /// <summary>
    /// Create color palette dengan child-friendly colors
    /// </summary>
    private void CreateColorPalette()
    {
        if (colorPaletteGrid == null || colorButtonPrefab == null) return;
        
        // Clear existing buttons
        foreach (Button button in colorPaletteButtons)
        {
            if (button != null) Destroy(button.gameObject);
        }
        colorPaletteButtons.Clear();
        
        // Create grid layout
        GridLayoutGroup gridLayout = colorPaletteGrid.GetComponent<GridLayoutGroup>();
        if (gridLayout == null)
        {
            gridLayout = colorPaletteGrid.gameObject.AddComponent<GridLayoutGroup>();
        }
        
        gridLayout.cellSize = new Vector2(60, 60); // Large buttons untuk anak-anak
        gridLayout.spacing = new Vector2(5, 5);
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = paletteColumns;
        
        // Create color buttons
        for (int i = 0; i < childFriendlyColors.Length; i++)
        {
            Color color = childFriendlyColors[i];
            CreateColorButton(color, colorPaletteGrid);
        }
    }
    
    /// <summary>
    /// Create individual color button
    /// </summary>
    private Button CreateColorButton(Color color, Transform parent)
    {
        GameObject buttonObj = Instantiate(colorButtonPrefab, parent);
        Button button = buttonObj.GetComponent<Button>();
        Image buttonImage = buttonObj.GetComponent<Image>();
        
        if (buttonImage != null)
        {
            buttonImage.color = color;
        }
        
        // Add click event
        button.onClick.AddListener(() => SelectColor(color));
        
        // Add hover animation
        AddButtonHoverEffect(button);
        
        colorPaletteButtons.Add(button);
        
        return button;
    }
    
    /// <summary>
    /// Create favorite colors grid
    /// </summary>
    private void CreateFavoriteColorsGrid()
    {
        if (favoriteColorsGrid == null) return;
        
        // Setup grid layout
        GridLayoutGroup gridLayout = favoriteColorsGrid.GetComponent<GridLayoutGroup>();
        if (gridLayout == null)
        {
            gridLayout = favoriteColorsGrid.gameObject.AddComponent<GridLayoutGroup>();
        }
        
        gridLayout.cellSize = new Vector2(50, 50);
        gridLayout.spacing = new Vector2(3, 3);
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = 6;
    }
    
    /// <summary>
    /// Add hover effect ke color button
    /// </summary>
    private void AddButtonHoverEffect(Button button)
    {
        // Scale animation on hover
        UnityEngine.EventSystems.EventTrigger eventTrigger = button.gameObject.GetComponent<UnityEngine.EventSystems.EventTrigger>();
        if (eventTrigger == null)
        {
            eventTrigger = button.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
        }
        
        // Hover start
        UnityEngine.EventSystems.EventTrigger.Entry pointerEnter = new UnityEngine.EventSystems.EventTrigger.Entry
        {
            eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter
        };
        pointerEnter.callback.AddListener((eventData) => {
            LeanTween.scale(button.gameObject, Vector3.one * 1.1f, 0.1f);
        });
        eventTrigger.triggers.Add(pointerEnter);
        
        // Hover end
        UnityEngine.EventSystems.EventTrigger.Entry pointerExit = new UnityEngine.EventSystems.EventTrigger.Entry
        {
            eventID = UnityEngine.EventSystems.EventTriggerType.PointerExit
        };
        pointerExit.callback.AddListener((eventData) => {
            LeanTween.scale(button.gameObject, Vector3.one, 0.1f);
        });
        eventTrigger.triggers.Add(pointerExit);
        
        // Click feedback
        button.onClick.AddListener(() => {
            LeanTween.scale(button.gameObject, Vector3.one * 0.9f, 0.05f)
                .setOnComplete(() => {
                    LeanTween.scale(button.gameObject, Vector3.one, 0.1f);
                });
            
            // Play click sound
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayButtonClick();
            }
        });
    }
    
    /// <summary>
    /// Handle color wheel input
    /// </summary>
    private void HandleColorWheelInput()
    {
        if (!enableColorWheel || colorWheel == null) return;
        
        bool isInputActive = false;
        Vector2 inputPosition = Vector2.zero;
        
        // Handle touch input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            inputPosition = touch.position;
            isInputActive = touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved;
        }
        // Handle mouse input
        else if (Input.GetMouseButton(0))
        {
            inputPosition = Input.mousePosition;
            isInputActive = true;
        }
        
        if (isInputActive)
        {
            // Check if input is over color wheel
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                colorWheelRect, inputPosition, null, out localPoint);
            
            // Check if point is within circle
            float distance = localPoint.magnitude;
            float wheelRadius = colorWheelRect.rect.width * 0.5f;
            
            if (distance <= wheelRadius)
            {
                // Calculate color based on position
                Color wheelColor = GetColorFromWheelPosition(localPoint, wheelRadius);
                SelectColor(wheelColor);
                
                // Update selector position
                UpdateColorWheelSelector(localPoint);
            }
        }
    }
    
    /// <summary>
    /// Get color dari posisi di color wheel
    /// </summary>
    private Color GetColorFromWheelPosition(Vector2 localPosition, float wheelRadius)
    {
        // Calculate angle dan distance
        float angle = Mathf.Atan2(localPosition.y, localPosition.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360f;
        
        float distance = localPosition.magnitude;
        float normalizedDistance = Mathf.Clamp01(distance / wheelRadius);
        
        // Convert angle to hue (0-1)
        float hue = angle / 360f;
        
        // Use distance for saturation
        float saturation = normalizedDistance;
        
        // Use slider for brightness atau default 1
        float brightness = brightnessSlider != null ? brightnessSlider.value : 1f;
        
        // Create color dari HSV
        return Color.HSVToRGB(hue, saturation, brightness);
    }
    
    /// <summary>
    /// Update color wheel selector position
    /// </summary>
    private void UpdateColorWheelSelector(Vector2 localPosition)
    {
        if (colorSelector != null)
        {
            colorSelector.localPosition = localPosition;
        }
    }
    
    /// <summary>
    /// Select color dan update UI
    /// </summary>
    public void SelectColor(Color color)
    {
        currentColor = color;
        
        // Update color preview
        if (colorPreview != null)
        {
            colorPreview.color = color;
        }
        
        // Update brightness/saturation previews
        UpdateColorPreviews();
        
        // Trigger event
        OnColorSelected?.Invoke(color);
        
        Debug.Log($"🎨 Color Selected: {color}");
    }
    
    /// <summary>
    /// Update color previews untuk brightness/saturation
    /// </summary>
    private void UpdateColorPreviews()
    {
        if (brightnessPreview != null)
        {
            float brightness = brightnessSlider != null ? brightnessSlider.value : 1f;
            Color.RGBToHSV(currentColor, out float h, out float s, out float v);
            brightnessPreview.color = Color.HSVToRGB(h, s, brightness);
        }
        
        if (saturationPreview != null)
        {
            float saturation = saturationSlider != null ? saturationSlider.value : 1f;
            Color.RGBToHSV(currentColor, out float h, out float s, out float v);
            saturationPreview.color = Color.HSVToRGB(h, saturation, v);
        }
    }
    
    /// <summary>
    /// Set default color
    /// </summary>
    private void SetDefaultColor()
    {
        SelectColor(childFriendlyColors[0]); // Start dengan bright red
    }
    
    /// <summary>
    /// Open color picker panel
    /// </summary>
    public void OpenColorPicker()
    {
        if (colorPickerPanel != null)
        {
            colorPickerPanel.SetActive(true);
            
            // Animate panel entrance
            AnimatePanelOpen(colorPickerPanel);
        }
        
        OnColorPickerOpened?.Invoke();
        
        // Play open sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }
    }
    
    /// <summary>
    /// Close color picker panel
    /// </summary>
    public void CloseColorPicker()
    {
        if (colorPickerPanel != null)
        {
            AnimatePanelClose(colorPickerPanel, () => {
                colorPickerPanel.SetActive(false);
            });
        }
        
        OnColorPickerClosed?.Invoke();
    }
    
    /// <summary>
    /// Open color mixing panel
    /// </summary>
    public void OpenColorMixing()
    {
        if (colorMixPanel != null)
        {
            colorMixPanel.SetActive(true);
            SetupColorMixing();
            
            // Animate panel entrance
            AnimatePanelOpen(colorMixPanel);
        }
    }
    
    /// <summary>
    /// Setup color mixing UI
    /// </summary>
    private void SetupColorMixing()
    {
        mixingColors.Clear();
        mixingStep = 0;
        
        // Reset mixing buttons
        for (int i = 0; i < mixColors.Length; i++)
        {
            if (mixColors[i] != null)
            {
                int index = i; // Capture untuk closure
                mixColors[i].GetComponent<Image>().color = Color.white;
                mixColors[i].onClick.RemoveAllListeners();
                mixColors[i].onClick.AddListener(() => AddColorToMix(index));
            }
        }
        
        // Reset result button
        if (mixResultButton != null)
        {
            mixResultButton.GetComponent<Image>().color = Color.white;
            mixResultButton.onClick.RemoveAllListeners();
            mixResultButton.onClick.AddListener(UseMixedColor);
        }
        
        // Update instructions
        UpdateMixingInstructions();
    }
    
    /// <summary>
    /// Add color ke mixing
    /// </summary>
    public void AddColorToMix(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < mixColors.Length)
        {
            mixColors[slotIndex].GetComponent<Image>().color = currentColor;
            
            // Update mixing colors list
            while (mixingColors.Count <= slotIndex)
            {
                mixingColors.Add(Color.white);
            }
            mixingColors[slotIndex] = currentColor;
            
            // Calculate mixed color
            CalculateMixedColor();
        }
    }
    
    /// <summary>
    /// Calculate mixed color dari selected colors
    /// </summary>
    private void CalculateMixedColor()
    {
        if (mixingColors.Count == 0) return;
        
        // Simple color mixing - average RGB values
        float r = 0, g = 0, b = 0;
        int validColors = 0;
        
        foreach (Color color in mixingColors)
        {
            if (color != Color.white) // Skip default white
            {
                r += color.r;
                g += color.g;
                b += color.b;
                validColors++;
            }
        }
        
        if (validColors > 0)
        {
            Color mixedColor = new Color(r / validColors, g / validColors, b / validColors, 1f);
            
            // Update result button
            if (mixResultButton != null)
            {
                mixResultButton.GetComponent<Image>().color = mixedColor;
            }
        }
        
        UpdateMixingInstructions();
    }
    
    /// <summary>
    /// Use mixed color
    /// </summary>
    public void UseMixedColor()
    {
        if (mixResultButton != null)
        {
            Color mixedColor = mixResultButton.GetComponent<Image>().color;
            if (mixedColor != Color.white)
            {
                SelectColor(mixedColor);
                CloseColorMixing();
            }
        }
    }
    
    /// <summary>
    /// Close color mixing panel
    /// </summary>
    public void CloseColorMixing()
    {
        if (colorMixPanel != null)
        {
            AnimatePanelClose(colorMixPanel, () => {
                colorMixPanel.SetActive(false);
            });
        }
    }
    
    /// <summary>
    /// Update mixing instructions text
    /// </summary>
    private void UpdateMixingInstructions()
    {
        if (mixInstructions != null)
        {
            int colorsSelected = 0;
            foreach (Color color in mixingColors)
            {
                if (color != Color.white) colorsSelected++;
            }
            
            if (colorsSelected == 0)
            {
                mixInstructions.text = "Pilih warna untuk dicampur!";
            }
            else if (colorsSelected == 1)
            {
                mixInstructions.text = "Pilih warna lain untuk dicampur!";
            }
            else
            {
                mixInstructions.text = "Tekan hasil untuk menggunakan warna campuran!";
            }
        }
    }
    
    /// <summary>
    /// Add current color ke favorites
    /// </summary>
    public void AddCurrentColorToFavorites()
    {
        if (favoriteColors.Length >= maxFavoriteColors)
        {
            Debug.LogWarning("⚠️ Maximum favorite colors reached!");
            return;
        }
        
        // Check if color already exists
        foreach (Color existingColor in favoriteColors)
        {
            if (ColorApproximatelyEqual(existingColor, currentColor))
            {
                Debug.LogWarning("⚠️ Color already in favorites!");
                return;
            }
        }
        
        // Add color ke favorites
        System.Array.Resize(ref favoriteColors, favoriteColors.Length + 1);
        favoriteColors[favoriteColors.Length - 1] = currentColor;
        
        // Create favorite color button
        Button favoriteButton = CreateColorButton(currentColor, favoriteColorsGrid);
        favoriteColorButtons.Add(favoriteButton);
        
        // Save favorites
        SaveFavoriteColors();
        
        OnColorAddedToFavorites?.Invoke(currentColor);
        
        Debug.Log($"💕 Color added to favorites: {currentColor}");
    }
    
    /// <summary>
    /// Check if two colors are approximately equal
    /// </summary>
    private bool ColorApproximatelyEqual(Color a, Color b, float threshold = 0.01f)
    {
        return Mathf.Abs(a.r - b.r) < threshold &&
               Mathf.Abs(a.g - b.g) < threshold &&
               Mathf.Abs(a.b - b.b) < threshold;
    }
    
    /// <summary>
    /// Load favorite colors dari PlayerPrefs
    /// </summary>
    private void LoadFavoriteColors()
    {
        int favoriteCount = PlayerPrefs.GetInt("FavoriteColorsCount", 0);
        favoriteColors = new Color[favoriteCount];
        
        for (int i = 0; i < favoriteCount; i++)
        {
            float r = PlayerPrefs.GetFloat($"FavoriteColor_{i}_R", 1f);
            float g = PlayerPrefs.GetFloat($"FavoriteColor_{i}_G", 1f);
            float b = PlayerPrefs.GetFloat($"FavoriteColor_{i}_B", 1f);
            favoriteColors[i] = new Color(r, g, b, 1f);
            
            // Create favorite button
            Button favoriteButton = CreateColorButton(favoriteColors[i], favoriteColorsGrid);
            favoriteColorButtons.Add(favoriteButton);
        }
        
        Debug.Log($"💾 Loaded {favoriteCount} favorite colors");
    }
    
    /// <summary>
    /// Save favorite colors ke PlayerPrefs
    /// </summary>
    private void SaveFavoriteColors()
    {
        PlayerPrefs.SetInt("FavoriteColorsCount", favoriteColors.Length);
        
        for (int i = 0; i < favoriteColors.Length; i++)
        {
            PlayerPrefs.SetFloat($"FavoriteColor_{i}_R", favoriteColors[i].r);
            PlayerPrefs.SetFloat($"FavoriteColor_{i}_G", favoriteColors[i].g);
            PlayerPrefs.SetFloat($"FavoriteColor_{i}_B", favoriteColors[i].b);
        }
        
        PlayerPrefs.Save();
    }
    
    // === SLIDER EVENT HANDLERS ===
    
    private void OnBrightnessChanged(float brightness)
    {
        // Apply brightness ke current color
        Color.RGBToHSV(currentColor, out float h, out float s, out float v);
        Color adjustedColor = Color.HSVToRGB(h, s, brightness);
        SelectColor(adjustedColor);
    }
    
    private void OnSaturationChanged(float saturation)
    {
        // Apply saturation ke current color
        Color.RGBToHSV(currentColor, out float h, out float s, out float v);
        Color adjustedColor = Color.HSVToRGB(h, saturation, v);
        SelectColor(adjustedColor);
    }
    
    // === ANIMATION HELPERS ===
    
    /// <summary>
    /// Animate panel opening
    /// </summary>
    private void AnimatePanelOpen(GameObject panel)
    {
        panel.transform.localScale = Vector3.zero;
        LeanTween.scale(panel, Vector3.one, 0.3f)
            .setEase(LeanTweenType.easeOutBack);
    }
    
    /// <summary>
    /// Animate panel closing
    /// </summary>
    private void AnimatePanelClose(GameObject panel, System.Action onComplete = null)
    {
        LeanTween.scale(panel, Vector3.zero, 0.2f)
            .setEase(LeanTweenType.easeInBack)
            .setOnComplete(() => onComplete?.Invoke());
    }
    
    // === PUBLIC GETTERS ===
    
    public Color GetCurrentColor() { return currentColor; }
    public Color[] GetChildFriendlyColors() { return childFriendlyColors; }
    public Color[] GetFavoriteColors() { return favoriteColors; }
    public bool IsColorPickerOpen() { return colorPickerPanel != null && colorPickerPanel.activeInHierarchy; }
}