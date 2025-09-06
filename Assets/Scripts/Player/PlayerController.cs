using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// PlayerController - Mengelola input pemain untuk drag and drop dan touch interaction
/// Dioptimalkan untuk anak-anak dengan area touch yang besar dan feedback visual yang jelas
/// </summary>
public class PlayerController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("Touch Settings")]
    public float dragThreshold = 10f; // Minimum jarak untuk dianggap drag
    public LayerMask interactableLayer = -1; // Layer objek yang bisa di-interact
    public float touchRadius = 50f; // Area touch yang diperbesar untuk anak-anak
    
    [Header("Visual Feedback")]
    public GameObject dragIndicator; // Visual indicator saat drag
    public Color highlightColor = Color.yellow;
    public float highlightIntensity = 1.5f;
    public ParticleSystem touchParticles; // Efek partikel saat touch
    
    [Header("Haptic Feedback")]
    public bool enableHaptic = true;
    public float hapticStrength = 0.5f;
    
    // Private variables
    private Camera playerCamera;
    private GameObject currentDragObject;
    private Vector3 dragOffset;
    private Vector3 lastTouchPosition;
    private bool isDragging = false;
    private bool isInteracting = false;
    
    // Touch tracking untuk multi-touch (jika diperlukan)
    private Dictionary<int, Vector2> activeTouches = new Dictionary<int, Vector2>();
    
    // Events untuk komunikasi dengan sistem lain
    public System.Action<GameObject> OnObjectSelected;
    public System.Action<GameObject> OnObjectReleased;
    public System.Action<GameObject, Vector3> OnObjectDragged;
    public System.Action<Vector3> OnTouchStart;
    public System.Action<Vector3> OnTouchEnd;
    
    private void Start()
    {
        InitializeController();
    }
    
    private void Update()
    {
        HandleInput();
    }
    
    /// <summary>
    /// Inisialisasi player controller
    /// </summary>
    private void InitializeController()
    {
        // Setup camera reference
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
            if (playerCamera == null)
            {
                playerCamera = FindObjectOfType<Camera>();
            }
        }
        
        // Setup drag indicator
        if (dragIndicator != null)
        {
            dragIndicator.SetActive(false);
        }
        
        Debug.Log("🎮 Player Controller Initialized");
    }
    
    /// <summary>
    /// Handle input berdasarkan platform (mobile vs desktop)
    /// </summary>
    private void HandleInput()
    {
        // Handle mobile touch input
        if (Application.isMobilePlatform || Input.touchCount > 0)
        {
            HandleTouchInput();
        }
        // Handle desktop mouse input untuk testing
        else
        {
            HandleMouseInput();
        }
    }
    
    /// <summary>
    /// Handle touch input untuk mobile device
    /// </summary>
    private void HandleTouchInput()
    {
        // Clear touches yang tidak aktif
        List<int> touchesToRemove = new List<int>();
        foreach (var touch in activeTouches)
        {
            bool foundActiveTouch = false;
            for (int i = 0; i < Input.touchCount; i++)
            {
                if (Input.GetTouch(i).fingerId == touch.Key)
                {
                    foundActiveTouch = true;
                    break;
                }
            }
            if (!foundActiveTouch)
            {
                touchesToRemove.Add(touch.Key);
            }
        }
        
        foreach (int touchId in touchesToRemove)
        {
            activeTouches.Remove(touchId);
        }
        
        // Process active touches
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            Vector3 worldPosition = GetWorldPositionFromTouch(touch.position);
            
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    HandleTouchStart(worldPosition, touch.fingerId);
                    activeTouches[touch.fingerId] = touch.position;
                    break;
                    
                case TouchPhase.Moved:
                    if (activeTouches.ContainsKey(touch.fingerId))
                    {
                        HandleTouchDrag(worldPosition, touch.position);
                        activeTouches[touch.fingerId] = touch.position;
                    }
                    break;
                    
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    HandleTouchEnd(worldPosition);
                    activeTouches.Remove(touch.fingerId);
                    break;
            }
        }
    }
    
    /// <summary>
    /// Handle mouse input untuk testing di editor
    /// </summary>
    private void HandleMouseInput()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldPosition = GetWorldPositionFromScreenPoint(mousePosition);
        
        if (Input.GetMouseButtonDown(0))
        {
            HandleTouchStart(worldPosition, 0);
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            HandleTouchDrag(worldPosition, mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            HandleTouchEnd(worldPosition);
        }
    }
    
    /// <summary>
    /// Handle touch start - deteksi objek yang bisa di-interact
    /// </summary>
    private void HandleTouchStart(Vector3 worldPosition, int fingerId)
    {
        lastTouchPosition = worldPosition;
        
        // Trigger touch start event
        OnTouchStart?.Invoke(worldPosition);
        
        // Play touch particles effect
        if (touchParticles != null)
        {
            touchParticles.transform.position = worldPosition;
            touchParticles.Play();
        }
        
        // Haptic feedback
        if (enableHaptic && Application.isMobilePlatform)
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
            // Android haptic feedback
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
            vibrator.Call("vibrate", (long)(hapticStrength * 100));
            #elif UNITY_IOS && !UNITY_EDITOR
            // iOS haptic feedback bisa ditambahkan dengan plugin
            #endif
        }
        
        // Cari objek yang bisa di-interact
        GameObject hitObject = GetInteractableObject(worldPosition);
        if (hitObject != null)
        {
            StartInteraction(hitObject, worldPosition);
        }
        
        // Play audio feedback
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayDragStart();
        }
    }
    
    /// <summary>
    /// Handle touch drag - update posisi objek yang sedang di-drag
    /// </summary>
    private void HandleTouchDrag(Vector3 worldPosition, Vector2 screenPosition)
    {
        if (currentDragObject != null && isInteracting)
        {
            float dragDistance = Vector3.Distance(worldPosition, lastTouchPosition);
            
            // Mulai drag jika threshold tercapai
            if (!isDragging && dragDistance > dragThreshold / 100f) // Convert pixel to world units
            {
                StartDragging();
            }
            
            if (isDragging)
            {
                // Update posisi objek
                Vector3 newPosition = worldPosition + dragOffset;
                currentDragObject.transform.position = newPosition;
                
                // Update drag indicator
                if (dragIndicator != null)
                {
                    dragIndicator.transform.position = newPosition;
                }
                
                // Trigger drag event
                OnObjectDragged?.Invoke(currentDragObject, newPosition);
            }
        }
    }
    
    /// <summary>
    /// Handle touch end - selesaikan drag dan check drop target
    /// </summary>
    private void HandleTouchEnd(Vector3 worldPosition)
    {
        // Trigger touch end event
        OnTouchEnd?.Invoke(worldPosition);
        
        if (currentDragObject != null)
        {
            if (isDragging)
            {
                // Check drop target
                CheckDropTarget(worldPosition);
                
                // Play drop sound
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayDragDrop();
                }
            }
            
            // Stop interaction
            StopInteraction();
        }
        
        // Reset drag state
        isDragging = false;
        isInteracting = false;
    }
    
    /// <summary>
    /// Mulai interaksi dengan objek
    /// </summary>
    private void StartInteraction(GameObject target, Vector3 touchPosition)
    {
        currentDragObject = target;
        isInteracting = true;
        
        // Calculate drag offset
        dragOffset = target.transform.position - touchPosition;
        
        // Visual feedback - highlight object
        AddHighlightToObject(target);
        
        // Trigger selection event
        OnObjectSelected?.Invoke(target);
        
        Debug.Log($"🎯 Started interaction with: {target.name}");
    }
    
    /// <summary>
    /// Mulai proses dragging
    /// </summary>
    private void StartDragging()
    {
        isDragging = true;
        
        // Show drag indicator
        if (dragIndicator != null)
        {
            dragIndicator.SetActive(true);
        }
        
        // Bring object to front
        if (currentDragObject != null)
        {
            // Temporarily increase sorting order or z-position
            SpriteRenderer renderer = currentDragObject.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.sortingOrder += 10;
            }
        }
        
        Debug.Log($"🚀 Started dragging: {currentDragObject?.name}");
    }
    
    /// <summary>
    /// Stop interaksi dan reset state
    /// </summary>
    private void StopInteraction()
    {
        if (currentDragObject != null)
        {
            // Remove highlight
            RemoveHighlightFromObject(currentDragObject);
            
            // Reset sorting order
            SpriteRenderer renderer = currentDragObject.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.sortingOrder -= 10;
            }
            
            // Trigger release event
            OnObjectReleased?.Invoke(currentDragObject);
            
            Debug.Log($"🏁 Stopped interaction with: {currentDragObject.name}");
            
            currentDragObject = null;
        }
        
        // Hide drag indicator
        if (dragIndicator != null)
        {
            dragIndicator.SetActive(false);
        }
    }
    
    /// <summary>
    /// Check apakah objek di-drop ke target yang valid
    /// </summary>
    private void CheckDropTarget(Vector3 dropPosition)
    {
        if (currentDragObject == null) return;
        
        // Cari drop target di posisi drop
        Collider2D dropTarget = Physics2D.OverlapCircle(dropPosition, touchRadius / 100f, interactableLayer);
        
        if (dropTarget != null && dropTarget.gameObject != currentDragObject)
        {
            // Check apakah drop target valid
            IDropTarget dropTargetComponent = dropTarget.GetComponent<IDropTarget>();
            if (dropTargetComponent != null)
            {
                bool dropAccepted = dropTargetComponent.OnObjectDropped(currentDragObject);
                
                if (dropAccepted)
                {
                    Debug.Log($"✅ Object dropped successfully on: {dropTarget.name}");
                }
                else
                {
                    Debug.Log($"❌ Drop rejected by: {dropTarget.name}");
                    // Return object to original position jika diperlukan
                    ReturnObjectToOriginalPosition();
                }
            }
        }
        else
        {
            // No valid drop target - return to original position
            ReturnObjectToOriginalPosition();
        }
    }
    
    /// <summary>
    /// Return objek ke posisi original jika drop tidak valid
    /// </summary>
    private void ReturnObjectToOriginalPosition()
    {
        if (currentDragObject != null)
        {
            // Implementasi return ke posisi asal bisa ditambahkan di sini
            // Misalnya dengan menyimpan original position di start
            Debug.Log($"🔄 Returning {currentDragObject.name} to original position");
        }
    }
    
    /// <summary>
    /// Get objek yang bisa di-interact dari posisi touch
    /// </summary>
    private GameObject GetInteractableObject(Vector3 worldPosition)
    {
        // Use overlap circle dengan radius yang diperbesar untuk anak-anak
        Collider2D hit = Physics2D.OverlapCircle(worldPosition, touchRadius / 100f, interactableLayer);
        
        if (hit != null && hit.GetComponent<IDraggable>() != null)
        {
            return hit.gameObject;
        }
        
        return null;
    }
    
    /// <summary>
    /// Convert touch position ke world position
    /// </summary>
    private Vector3 GetWorldPositionFromTouch(Vector2 touchPosition)
    {
        Vector3 worldPosition = playerCamera.ScreenToWorldPoint(new Vector3(touchPosition.x, touchPosition.y, playerCamera.nearClipPlane));
        worldPosition.z = 0; // 2D game
        return worldPosition;
    }
    
    /// <summary>
    /// Convert screen point ke world position
    /// </summary>
    private Vector3 GetWorldPositionFromScreenPoint(Vector3 screenPoint)
    {
        Vector3 worldPosition = playerCamera.ScreenToWorldPoint(new Vector3(screenPoint.x, screenPoint.y, playerCamera.nearClipPlane));
        worldPosition.z = 0; // 2D game
        return worldPosition;
    }
    
    /// <summary>
    /// Tambah highlight visual ke objek
    /// </summary>
    private void AddHighlightToObject(GameObject target)
    {
        SpriteRenderer renderer = target.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.color = highlightColor * highlightIntensity;
        }
    }
    
    /// <summary>
    /// Remove highlight visual dari objek
    /// </summary>
    private void RemoveHighlightFromObject(GameObject target)
    {
        SpriteRenderer renderer = target.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.color = Color.white; // Reset to default
        }
    }
    
    // === EVENT SYSTEM INTERFACES ===
    
    public void OnPointerDown(PointerEventData eventData)
    {
        Vector3 worldPos = GetWorldPositionFromScreenPoint(eventData.position);
        HandleTouchStart(worldPos, eventData.pointerId);
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        Vector3 worldPos = GetWorldPositionFromScreenPoint(eventData.position);
        HandleTouchEnd(worldPos);
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        Vector3 worldPos = GetWorldPositionFromScreenPoint(eventData.position);
        HandleTouchDrag(worldPos, eventData.position);
    }
}

/// <summary>
/// Interface untuk objek yang bisa di-drag
/// </summary>
public interface IDraggable
{
    void OnDragStart();
    void OnDragEnd();
    bool CanBeDragged();
}

/// <summary>
/// Interface untuk drop target
/// </summary>
public interface IDropTarget
{
    bool OnObjectDropped(GameObject droppedObject);
    bool CanAcceptDrop(GameObject draggedObject);
    void OnDropHover(GameObject hoveredObject);
    void OnDropExit(GameObject exitedObject);
}