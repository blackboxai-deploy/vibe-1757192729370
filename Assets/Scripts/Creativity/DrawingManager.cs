using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// DrawingManager - Mengelola mode kreativitas untuk menggambar dan membuat karya seni
/// Dirancang untuk anak-anak dengan tools yang mudah digunakan dan interface yang intuitif
/// </summary>
public class DrawingManager : MonoBehaviour
{
    [Header("Drawing Canvas")]
    public RenderTexture drawingCanvas;
    public Camera drawingCamera;
    public LayerMask drawingLayer = -1;
    public int canvasWidth = 1024;
    public int canvasHeight = 768;
    
    [Header("Drawing Tools")]
    public DrawingTool currentTool = DrawingTool.Brush;
    public float brushSize = 10f;
    public Color currentColor = Color.black;
    public Material drawingMaterial;
    public LineRenderer lineRenderer;
    
    [Header("Drawing Settings")]
    [Range(1f, 50f)]
    public float minBrushSize = 5f;
    [Range(1f, 50f)]
    public float maxBrushSize = 30f;
    public bool smoothDrawing = true;
    public float smoothingFactor = 0.1f;
    
    [Header("UI References")]
    public Slider brushSizeSlider;
    public Button[] colorButtons;
    public Button brushButton;
    public Button eraserButton;
    public Button clearButton;
    public Button saveButton;
    public Button undoButton;
    public Button redoButton;
    
    [Header("Shape Tools")]
    public Button circleButton;
    public Button squareButton;
    public Button triangleButton;
    public Button starButton;
    public Button heartButton;
    
    [Header("Stamps and Stickers")]
    public Button stampsButton;
    public GameObject stampsPanel;
    public Button[] stampButtons;
    public List<Sprite> stampSprites = new List<Sprite>();
    
    // Private variables
    private bool isDrawing = false;
    private Vector3 lastDrawPosition;
    private List<Vector3> currentStroke = new List<Vector3>();
    private List<List<Vector3>> strokeHistory = new List<List<Vector3>>();
    private List<List<Vector3>> redoHistory = new List<List<Vector3>>();
    private int maxHistorySteps = 20;
    
    // Drawing objects
    private GameObject currentDrawingObject;
    private List<GameObject> drawnObjects = new List<GameObject>();
    
    // Touch/Mouse input
    private bool isTouchSupported;
    private Vector2 lastTouchPosition;
    
    // Events
    public System.Action<DrawingTool> OnToolChanged;
    public System.Action<Color> OnColorChanged;
    public System.Action<float> OnBrushSizeChanged;
    public System.Action OnDrawingStarted;
    public System.Action OnDrawingFinished;
    public System.Action OnCanvasCleared;
    
    private void Start()
    {
        InitializeDrawingSystem();
        SetupUIEvents();
        SetupDrawingCanvas();
    }
    
    private void Update()
    {
        HandleInput();
        UpdateDrawing();
    }
    
    /// <summary>
    /// Inisialisasi drawing system
    /// </summary>
    private void InitializeDrawingSystem()
    {
        // Detect touch support
        isTouchSupported = Input.touchSupported;
        
        // Setup default values
        SetBrushSize(15f);
        SetDrawingColor(Color.blue); // Default to child-friendly blue
        SetTool(DrawingTool.Brush);
        
        // Setup camera untuk drawing
        if (drawingCamera == null)
        {
            drawingCamera = Camera.main;
        }
        
        Debug.Log("🎨 Drawing Manager Initialized");
    }
    
    /// <summary>
    /// Setup drawing canvas
    /// </summary>
    private void SetupDrawingCanvas()
    {
        // Create render texture untuk drawing canvas
        if (drawingCanvas == null)
        {
            drawingCanvas = new RenderTexture(canvasWidth, canvasHeight, 0);
            drawingCanvas.Create();
        }
        
        // Setup line renderer untuk smooth drawing
        if (lineRenderer == null)
        {
            GameObject lineObj = new GameObject("LineRenderer");
            lineRenderer = lineObj.AddComponent<LineRenderer>();
        }
        
        lineRenderer.material = drawingMaterial;
        lineRenderer.startWidth = brushSize / 100f;
        lineRenderer.endWidth = brushSize / 100f;
        lineRenderer.useWorldSpace = false;
        lineRenderer.sortingLayerName = "Drawing";
        
        Debug.log("🖼️ Drawing Canvas Setup Complete");
    }
    
    /// <summary>
    /// Setup UI event listeners
    /// </summary>
    private void SetupUIEvents()
    {
        // Brush size slider
        if (brushSizeSlider != null)
        {
            brushSizeSlider.minValue = minBrushSize;
            brushSizeSlider.maxValue = maxBrushSize;
            brushSizeSlider.value = brushSize;
            brushSizeSlider.onValueChanged.AddListener(OnBrushSizeSliderChanged);
        }
        
        // Tool buttons
        if (brushButton != null)
            brushButton.onClick.AddListener(() => SetTool(DrawingTool.Brush));
        
        if (eraserButton != null)
            eraserButton.onClick.AddListener(() => SetTool(DrawingTool.Eraser));
        
        // Action buttons
        if (clearButton != null)
            clearButton.onClick.AddListener(ClearCanvas);
        
        if (saveButton != null)
            saveButton.onClick.AddListener(SaveDrawing);
        
        if (undoButton != null)
            undoButton.onClick.AddListener(UndoLastStroke);
        
        if (redoButton != null)
            redoButton.onClick.AddListener(RedoLastStroke);
        
        // Color buttons
        SetupColorButtons();
        
        // Shape buttons
        SetupShapeButtons();
        
        // Stamp buttons
        SetupStampButtons();
        
        Debug.log("🎛️ UI Events Setup Complete");
    }
    
    /// <summary>
    /// Setup color palette buttons
    /// </summary>
    private void SetupColorButtons()
    {
        Color[] defaultColors = {
            Color.red, Color.blue, Color.green, Color.yellow,
            Color.magenta, Color.cyan, Color.orange, Color.purple,
            Color.black, Color.white, Color.brown, Color.pink
        };
        
        for (int i = 0; i < colorButtons.Length && i < defaultColors.Length; i++)
        {
            Color color = defaultColors[i];
            colorButtons[i].GetComponent<Image>().color = color;
            colorButtons[i].onClick.AddListener(() => SetDrawingColor(color));
            
            // Add visual feedback
            AddButtonClickAnimation(colorButtons[i]);
        }
    }
    
    /// <summary>
    /// Setup shape tool buttons
    /// </summary>
    private void SetupShapeButtons()
    {
        if (circleButton != null)
            circleButton.onClick.AddListener(() => SetTool(DrawingTool.Circle));
        
        if (squareButton != null)
            squareButton.onClick.AddListener(() => SetTool(DrawingTool.Square));
        
        if (triangleButton != null)
            triangleButton.onClick.AddListener(() => SetTool(DrawingTool.Triangle));
        
        if (starButton != null)
            starButton.onClick.AddListener(() => SetTool(DrawingTool.Star));
        
        if (heartButton != null)
            heartButton.onClick.AddListener(() => SetTool(DrawingTool.Heart));
    }
    
    /// <summary>
    /// Setup stamp/sticker buttons
    /// </summary>
    private void SetupStampButtons()
    {
        if (stampsButton != null)
        {
            stampsButton.onClick.AddListener(ToggleStampsPanel);
        }
        
        for (int i = 0; i < stampButtons.Length && i < stampSprites.Count; i++)
        {
            int stampIndex = i; // Capture for closure
            stampButtons[i].GetComponent<Image>().sprite = stampSprites[i];
            stampButtons[i].onClick.AddListener(() => SelectStamp(stampIndex));
        }
    }
    
    /// <summary>
    /// Add click animation ke button
    /// </summary>
    private void AddButtonClickAnimation(Button button)
    {
        button.onClick.AddListener(() => {
            // Scale animation
            LeanTween.scale(button.gameObject, Vector3.one * 0.9f, 0.1f)
                .setOnComplete(() => {
                    LeanTween.scale(button.gameObject, Vector3.one, 0.1f);
                });
            
            // Play sound
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayButtonClick();
            }
        });
    }
    
    /// <summary>
    /// Handle input untuk drawing
    /// </summary>
    private void HandleInput()
    {
        if (isTouchSupported && Input.touchCount > 0)
        {
            HandleTouchInput();
        }
        else
        {
            HandleMouseInput();
        }
    }
    
    /// <summary>
    /// Handle touch input
    /// </summary>
    private void HandleTouchInput()
    {
        Touch touch = Input.GetTouch(0);
        Vector3 touchPosition = GetWorldPositionFromTouch(touch.position);
        
        switch (touch.phase)
        {
            case TouchPhase.Began:
                StartDrawing(touchPosition);
                break;
                
            case TouchPhase.Moved:
                if (isDrawing)
                {
                    ContinueDrawing(touchPosition);
                }
                break;
                
            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                EndDrawing();
                break;
        }
    }
    
    /// <summary>
    /// Handle mouse input (untuk testing di editor)
    /// </summary>
    private void HandleMouseInput()
    {
        Vector3 mousePosition = GetWorldPositionFromMouse();
        
        if (Input.GetMouseButtonDown(0))
        {
            StartDrawing(mousePosition);
        }
        else if (Input.GetMouseButton(0) && isDrawing)
        {
            ContinueDrawing(mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            EndDrawing();
        }
    }
    
    /// <summary>
    /// Start drawing stroke
    /// </summary>
    private void StartDrawing(Vector3 position)
    {
        if (currentTool == DrawingTool.Eraser)
        {
            EraseAtPosition(position);
            return;
        }
        
        isDrawing = true;
        lastDrawPosition = position;
        currentStroke.Clear();
        currentStroke.Add(position);
        
        // Create new drawing object
        CreateDrawingObject(position);
        
        OnDrawingStarted?.Invoke();
        
        // Play drawing sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayDrawing();
        }
    }
    
    /// <summary>
    /// Continue drawing stroke
    /// </summary>
    private void ContinueDrawing(Vector3 position)
    {
        if (!isDrawing) return;
        
        float distance = Vector3.Distance(lastDrawPosition, position);
        
        // Minimum distance untuk smooth drawing
        if (distance < 0.1f) return;
        
        currentStroke.Add(position);
        
        if (currentTool == DrawingTool.Brush)
        {
            DrawLine(lastDrawPosition, position);
        }
        
        lastDrawPosition = position;
        UpdateCurrentDrawingObject();
    }
    
    /// <summary>
    /// End drawing stroke
    /// </summary>
    private void EndDrawing()
    {
        if (!isDrawing) return;
        
        isDrawing = false;
        
        // Add stroke to history untuk undo functionality
        if (currentStroke.Count > 0)
        {
            strokeHistory.Add(new List<Vector3>(currentStroke));
            
            // Limit history size
            if (strokeHistory.Count > maxHistorySteps)
            {
                strokeHistory.RemoveAt(0);
            }
            
            // Clear redo history
            redoHistory.Clear();
        }
        
        // Finalize drawing object
        FinalizeDrawingObject();
        
        OnDrawingFinished?.Invoke();
    }
    
    /// <summary>
    /// Draw line between two points
    /// </summary>
    private void DrawLine(Vector3 start, Vector3 end)
    {
        // Create line segment
        GameObject lineSegment = new GameObject("LineSegment");
        lineSegment.transform.SetParent(currentDrawingObject.transform);
        
        LineRenderer segmentRenderer = lineSegment.AddComponent<LineRenderer>();
        segmentRenderer.material = drawingMaterial;
        segmentRenderer.color = currentColor;
        segmentRenderer.startWidth = brushSize / 100f;
        segmentRenderer.endWidth = brushSize / 100f;
        segmentRenderer.positionCount = 2;
        segmentRenderer.useWorldSpace = false;
        segmentRenderer.sortingLayerName = "Drawing";
        
        // Set positions
        segmentRenderer.SetPosition(0, start);
        segmentRenderer.SetPosition(1, end);
    }
    
    /// <summary>
    /// Create drawing object untuk current stroke
    /// </summary>
    private void CreateDrawingObject(Vector3 startPosition)
    {
        currentDrawingObject = new GameObject($"Drawing_{System.DateTime.Now.Ticks}");
        currentDrawingObject.transform.position = Vector3.zero;
        
        // Add ke drawing objects list
        drawnObjects.Add(currentDrawingObject);
        
        // Handle different tools
        switch (currentTool)
        {
            case DrawingTool.Brush:
                // Brush handled dalam ContinueDrawing
                break;
                
            case DrawingTool.Circle:
                CreateShape(ShapeType.Circle, startPosition);
                break;
                
            case DrawingTool.Square:
                CreateShape(ShapeType.Square, startPosition);
                break;
                
            case DrawingTool.Triangle:
                CreateShape(ShapeType.Triangle, startPosition);
                break;
                
            case DrawingTool.Star:
                CreateShape(ShapeType.Star, startPosition);
                break;
                
            case DrawingTool.Heart:
                CreateShape(ShapeType.Heart, startPosition);
                break;
        }
    }
    
    /// <summary>
    /// Update current drawing object
    /// </summary>
    private void UpdateCurrentDrawingObject()
    {
        if (currentDrawingObject == null) return;
        
        // Update shape objects berdasarkan drag
        if (IsShapeTool(currentTool))
        {
            UpdateShapeSize();
        }
    }
    
    /// <summary>
    /// Finalize drawing object
    /// </summary>
    private void FinalizeDrawingObject()
    {
        if (currentDrawingObject == null) return;
        
        // Add collider untuk interaction (opsional)
        if (currentDrawingObject.GetComponent<Collider2D>() == null)
        {
            currentDrawingObject.AddComponent<PolygonCollider2D>();
        }
        
        currentDrawingObject = null;
    }
    
    /// <summary>
    /// Create shape di posisi tertentu
    /// </summary>
    private void CreateShape(ShapeType shapeType, Vector3 position)
    {
        GameObject shape = CreateShapeObject(shapeType);
        shape.transform.SetParent(currentDrawingObject.transform);
        shape.transform.position = position;
        
        // Setup visual
        SpriteRenderer renderer = shape.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.color = currentColor;
            renderer.sortingLayerName = "Drawing";
        }
    }
    
    /// <summary>
    /// Create shape object berdasarkan type
    /// </summary>
    private GameObject CreateShapeObject(ShapeType shapeType)
    {
        GameObject shape = new GameObject(shapeType.ToString());
        SpriteRenderer renderer = shape.AddComponent<SpriteRenderer>();
        
        // Create sprite berdasarkan shape type
        // Ini bisa diganti dengan pre-made sprites
        Sprite shapeSprite = CreateShapeSprite(shapeType);
        renderer.sprite = shapeSprite;
        
        return shape;
    }
    
    /// <summary>
    /// Create sprite untuk shape (simplified implementation)
    /// </summary>
    private Sprite CreateShapeSprite(ShapeType shapeType)
    {
        // Simplified - dalam implementasi nyata, gunakan pre-made sprites
        // Atau generate procedural shapes
        int textureSize = 64;
        Texture2D texture = new Texture2D(textureSize, textureSize);
        
        // Fill dengan basic shape (simplified)
        for (int x = 0; x < textureSize; x++)
        {
            for (int y = 0; y < textureSize; y++)
            {
                float centerX = textureSize / 2f;
                float centerY = textureSize / 2f;
                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(centerX, centerY));
                
                bool isInside = false;
                
                switch (shapeType)
                {
                    case ShapeType.Circle:
                        isInside = distance < textureSize / 3f;
                        break;
                    case ShapeType.Square:
                        isInside = Mathf.Abs(x - centerX) < textureSize / 3f && Mathf.Abs(y - centerY) < textureSize / 3f;
                        break;
                    default:
                        isInside = distance < textureSize / 3f; // Default to circle
                        break;
                }
                
                texture.SetPixel(x, y, isInside ? Color.white : Color.clear);
            }
        }
        
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, textureSize, textureSize), Vector2.one * 0.5f);
    }
    
    /// <summary>
    /// Update shape size berdasarkan drag
    /// </summary>
    private void UpdateShapeSize()
    {
        if (currentDrawingObject == null || currentStroke.Count < 2) return;
        
        Vector3 startPos = currentStroke[0];
        Vector3 currentPos = currentStroke[currentStroke.Count - 1];
        float distance = Vector3.Distance(startPos, currentPos);
        
        // Scale shape berdasarkan drag distance
        float scale = Mathf.Clamp(distance * 2f, 0.5f, 3f);
        Transform shapeTransform = currentDrawingObject.transform.GetChild(0);
        if (shapeTransform != null)
        {
            shapeTransform.localScale = Vector3.one * scale;
        }
    }
    
    /// <summary>
    /// Check if current tool adalah shape tool
    /// </summary>
    private bool IsShapeTool(DrawingTool tool)
    {
        return tool == DrawingTool.Circle || tool == DrawingTool.Square || 
               tool == DrawingTool.Triangle || tool == DrawingTool.Star || 
               tool == DrawingTool.Heart;
    }
    
    /// <summary>
    /// Erase at position
    /// </summary>
    private void EraseAtPosition(Vector3 position)
    {
        float eraseRadius = brushSize / 50f; // Convert to world units
        
        // Find objects to erase
        for (int i = drawnObjects.Count - 1; i >= 0; i--)
        {
            if (drawnObjects[i] != null)
            {
                float distance = Vector3.Distance(drawnObjects[i].transform.position, position);
                if (distance < eraseRadius)
                {
                    Destroy(drawnObjects[i]);
                    drawnObjects.RemoveAt(i);
                }
            }
        }
    }
    
    /// <summary>
    /// Clear entire canvas
    /// </summary>
    public void ClearCanvas()
    {
        // Clear all drawn objects
        foreach (GameObject obj in drawnObjects)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        
        drawnObjects.Clear();
        strokeHistory.Clear();
        redoHistory.Clear();
        
        OnCanvasCleared?.Invoke();
        
        Debug.Log("🗑️ Canvas Cleared");
    }
    
    /// <summary>
    /// Undo last stroke
    /// </summary>
    public void UndoLastStroke()
    {
        if (strokeHistory.Count > 0 && drawnObjects.Count > 0)
        {
            // Move last stroke to redo history
            List<Vector3> lastStroke = strokeHistory[strokeHistory.Count - 1];
            redoHistory.Add(lastStroke);
            strokeHistory.RemoveAt(strokeHistory.Count - 1);
            
            // Remove last drawn object
            GameObject lastObject = drawnObjects[drawnObjects.Count - 1];
            drawnObjects.RemoveAt(drawnObjects.Count - 1);
            Destroy(lastObject);
            
            Debug.Log("↶ Undo Last Stroke");
        }
    }
    
    /// <summary>
    /// Redo last undone stroke
    /// </summary>
    public void RedoLastStroke()
    {
        if (redoHistory.Count > 0)
        {
            // Move stroke back dari redo ke main history
            List<Vector3> strokeToRedo = redoHistory[redoHistory.Count - 1];
            strokeHistory.Add(strokeToRedo);
            redoHistory.RemoveAt(redoHistory.Count - 1);
            
            // Recreate the stroke (simplified implementation)
            RedrawStroke(strokeToRedo);
            
            Debug.Log("↷ Redo Last Stroke");
        }
    }
    
    /// <summary>
    /// Redraw stroke dari history
    /// </summary>
    private void RedrawStroke(List<Vector3> stroke)
    {
        if (stroke.Count < 2) return;
        
        GameObject redrawObject = new GameObject($"Redrawn_{System.DateTime.Now.Ticks}");
        drawnObjects.Add(redrawObject);
        
        for (int i = 1; i < stroke.Count; i++)
        {
            DrawLineBetween(stroke[i-1], stroke[i], redrawObject);
        }
    }
    
    /// <summary>
    /// Draw line between points untuk redraw
    /// </summary>
    private void DrawLineBetween(Vector3 start, Vector3 end, GameObject parent)
    {
        GameObject lineSegment = new GameObject("RedrawSegment");
        lineSegment.transform.SetParent(parent.transform);
        
        LineRenderer renderer = lineSegment.AddComponent<LineRenderer>();
        renderer.material = drawingMaterial;
        renderer.color = currentColor;
        renderer.startWidth = brushSize / 100f;
        renderer.endWidth = brushSize / 100f;
        renderer.positionCount = 2;
        renderer.useWorldSpace = false;
        renderer.sortingLayerName = "Drawing";
        
        renderer.SetPosition(0, start);
        renderer.SetPosition(1, end);
    }
    
    /// <summary>
    /// Save drawing sebagai screenshot
    /// </summary>
    public void SaveDrawing()
    {
        StartCoroutine(CaptureAndSaveDrawing());
    }
    
    /// <summary>
    /// Capture dan save drawing
    /// </summary>
    private IEnumerator CaptureAndSaveDrawing()
    {
        yield return new WaitForEndOfFrame();
        
        // Capture screen
        Texture2D screenshot = ScreenCapture.CaptureScreenAsTexture();
        
        // Save to persistent data path (mobile compatible)
        string filename = $"Drawing_{System.DateTime.Now:yyyyMMdd_HHmmss}.png";
        string filepath = System.IO.Path.Combine(Application.persistentDataPath, filename);
        
        byte[] data = screenshot.EncodeToPNG();
        System.IO.File.WriteAllBytes(filepath, data);
        
        Destroy(screenshot);
        
        // Show success message
        Debug.Log($"🖼️ Drawing saved: {filepath}");
        
        // Play save sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayCoinEarned(); // Use coin sound for save success
        }
    }
    
    // === PUBLIC SETTERS ===
    
    /// <summary>
    /// Set drawing tool
    /// </summary>
    public void SetTool(DrawingTool tool)
    {
        currentTool = tool;
        OnToolChanged?.Invoke(tool);
        Debug.Log($"🛠️ Tool changed to: {tool}");
    }
    
    /// <summary>
    /// Set drawing color
    /// </summary>
    public void SetDrawingColor(Color color)
    {
        currentColor = color;
        OnColorChanged?.Invoke(color);
        Debug.Log($"🎨 Color changed to: {color}");
    }
    
    /// <summary>
    /// Set brush size
    /// </summary>
    public void SetBrushSize(float size)
    {
        brushSize = Mathf.Clamp(size, minBrushSize, maxBrushSize);
        OnBrushSizeChanged?.Invoke(brushSize);
        
        // Update line renderer width
        if (lineRenderer != null)
        {
            lineRenderer.startWidth = brushSize / 100f;
            lineRenderer.endWidth = brushSize / 100f;
        }
    }
    
    /// <summary>
    /// Toggle stamps panel
    /// </summary>
    public void ToggleStampsPanel()
    {
        if (stampsPanel != null)
        {
            stampsPanel.SetActive(!stampsPanel.activeInHierarchy);
        }
    }
    
    /// <summary>
    /// Select stamp
    /// </summary>
    public void SelectStamp(int stampIndex)
    {
        if (stampIndex >= 0 && stampIndex < stampSprites.Count)
        {
            SetTool(DrawingTool.Stamp);
            // Store selected stamp index untuk placement
            PlayerPrefs.SetInt("SelectedStamp", stampIndex);
        }
    }
    
    // === UI EVENT HANDLERS ===
    
    private void OnBrushSizeSliderChanged(float value)
    {
        SetBrushSize(value);
    }
    
    // === UTILITY METHODS ===
    
    /// <summary>
    /// Get world position dari touch
    /// </summary>
    private Vector3 GetWorldPositionFromTouch(Vector2 touchPosition)
    {
        Vector3 worldPosition = drawingCamera.ScreenToWorldPoint(new Vector3(touchPosition.x, touchPosition.y, drawingCamera.nearClipPlane));
        worldPosition.z = 0; // 2D drawing
        return worldPosition;
    }
    
    /// <summary>
    /// Get world position dari mouse
    /// </summary>
    private Vector3 GetWorldPositionFromMouse()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldPosition = drawingCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, drawingCamera.nearClipPlane));
        worldPosition.z = 0; // 2D drawing
        return worldPosition;
    }
    
    // === PUBLIC GETTERS ===
    
    public DrawingTool GetCurrentTool() { return currentTool; }
    public Color GetCurrentColor() { return currentColor; }
    public float GetBrushSize() { return brushSize; }
    public bool IsDrawing() { return isDrawing; }
    public int GetDrawnObjectsCount() { return drawnObjects.Count; }
    public bool CanUndo() { return strokeHistory.Count > 0; }
    public bool CanRedo() { return redoHistory.Count > 0; }
}

// === ENUMS ===

public enum DrawingTool
{
    Brush,
    Eraser,
    Circle,
    Square,
    Triangle,
    Star,
    Heart,
    Stamp
}

public enum ShapeType
{
    Circle,
    Square,
    Triangle,
    Star,
    Heart
}