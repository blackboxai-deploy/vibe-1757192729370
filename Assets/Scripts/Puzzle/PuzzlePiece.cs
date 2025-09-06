using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PuzzlePiece - Komponen untuk individual puzzle piece
/// Mengimplementasikan IDraggable interface untuk interaksi drag and drop
/// </summary>
public class PuzzlePiece : MonoBehaviour, IDraggable
{
    [Header("Piece Properties")]
    public string pieceId;
    public PieceType pieceType = PieceType.Shape;
    public Sprite pieceSprite;
    public Color pieceColor = Color.white;
    
    [Header("Piece Data")]
    public Dictionary<string, object> pieceData = new Dictionary<string, object>();
    
    [Header("Visual Settings")]
    public bool glowWhenSelected = true;
    public Color glowColor = Color.yellow;
    public float glowIntensity = 1.5f;
    public AnimationCurve bounceCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("Animation Settings")]
    public float snapAnimationDuration = 0.3f;
    public float bounceAnimationDuration = 0.2f;
    public LeanTweenType snapEaseType = LeanTweenType.easeOutBack;
    
    // Private variables
    private bool isPlaced = false;
    private bool isDragging = false;
    private bool canBeDragged = true;
    private Vector3 originalPosition;
    private Vector3 originalScale;
    private Color originalColor;
    
    // Components references
    private SpriteRenderer spriteRenderer;
    private Collider2D pieceCollider;
    private Rigidbody2D pieceRigidbody;
    
    // Visual effects
    private GameObject glowEffect;
    private ParticleSystem placeParticles;
    
    // Events
    public System.Action<PuzzlePiece> OnPieceSelected;
    public System.Action<PuzzlePiece> OnPieceDeselected;
    public System.Action<PuzzlePiece, Vector3> OnPieceMoved;
    public System.Action<PuzzlePiece> OnPiecePlaced;
    
    private void Awake()
    {
        InitializePiece();
    }
    
    private void Start()
    {
        SetupVisuals();
        StoreOriginalValues();
    }
    
    /// <summary>
    /// Inisialisasi puzzle piece
    /// </summary>
    private void InitializePiece()
    {
        // Generate unique ID jika belum ada
        if (string.IsNullOrEmpty(pieceId))
        {
            pieceId = System.Guid.NewGuid().ToString();
        }
        
        // Get atau add required components
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
        
        pieceCollider = GetComponent<Collider2D>();
        if (pieceCollider == null)
        {
            pieceCollider = gameObject.AddComponent<CircleCollider2D>();
        }
        
        // Optional rigidbody untuk physics-based interactions
        pieceRigidbody = GetComponent<Rigidbody2D>();
        if (pieceRigidbody == null)
        {
            pieceRigidbody = gameObject.AddComponent<Rigidbody2D>();
            pieceRigidbody.bodyType = RigidbodyType2D.Kinematic; // Kinematic untuk controlled movement
        }
        
        Debug.Log($"🧩 Puzzle Piece Initialized: {pieceId}");
    }
    
    /// <summary>
    /// Setup visual components
    /// </summary>
    private void SetupVisuals()
    {
        // Apply sprite dan color
        if (pieceSprite != null)
        {
            spriteRenderer.sprite = pieceSprite;
        }
        
        spriteRenderer.color = pieceColor;
        
        // Setup sorting layer
        spriteRenderer.sortingLayerName = "PuzzlePieces";
        spriteRenderer.sortingOrder = 0;
        
        // Create glow effect object
        if (glowWhenSelected)
        {
            CreateGlowEffect();
        }
        
        // Setup particle effect untuk placement
        CreatePlacementParticles();
    }
    
    /// <summary>
    /// Store nilai original untuk reset
    /// </summary>
    private void StoreOriginalValues()
    {
        originalPosition = transform.position;
        originalScale = transform.localScale;
        originalColor = spriteRenderer.color;
    }
    
    /// <summary>
    /// Create glow effect untuk visual feedback
    /// </summary>
    private void CreateGlowEffect()
    {
        glowEffect = new GameObject($"{name}_Glow");
        glowEffect.transform.SetParent(transform);
        glowEffect.transform.localPosition = Vector3.zero;
        glowEffect.transform.localScale = Vector3.one * 1.2f;
        
        SpriteRenderer glowRenderer = glowEffect.AddComponent<SpriteRenderer>();
        glowRenderer.sprite = spriteRenderer.sprite;
        glowRenderer.color = glowColor;
        glowRenderer.sortingLayerName = "Effects";
        glowRenderer.sortingOrder = -1; // Behind main sprite
        
        glowEffect.SetActive(false);
    }
    
    /// <summary>
    /// Create particle system untuk placement effect
    /// </summary>
    private void CreatePlacementParticles()
    {
        GameObject particleObj = new GameObject($"{name}_Particles");
        particleObj.transform.SetParent(transform);
        particleObj.transform.localPosition = Vector3.zero;
        
        placeParticles = particleObj.AddComponent<ParticleSystem>();
        
        // Configure particle system
        var main = placeParticles.main;
        main.startLifetime = 1f;
        main.startSpeed = 2f;
        main.maxParticles = 20;
        main.startColor = pieceColor;
        
        var emission = placeParticles.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[]
        {
            new ParticleSystem.Burst(0.0f, 15)
        });
        
        var shape = placeParticles.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.5f;
        
        placeParticles.Stop();
    }
    
    /// <summary>
    /// Set data piece untuk puzzle logic
    /// </summary>
    public void SetPieceData(string key, object value)
    {
        pieceData[key] = value;
        Debug.Log($"🔧 Piece Data Set: {key} = {value}");
    }
    
    /// <summary>
    /// Get data piece
    /// </summary>
    public T GetPieceData<T>(string key)
    {
        if (pieceData.ContainsKey(key))
        {
            return (T)pieceData[key];
        }
        return default(T);
    }
    
    /// <summary>
    /// Check apakah piece memiliki data tertentu
    /// </summary>
    public bool HasPieceData(string key)
    {
        return pieceData.ContainsKey(key);
    }
    
    /// <summary>
    /// Set piece sebagai placed (solved)
    /// </summary>
    public void SetPlaced(bool placed)
    {
        isPlaced = placed;
        canBeDragged = !placed; // Tidak bisa di-drag lagi setelah placed
        
        if (placed)
        {
            // Visual feedback untuk placed piece
            PlayPlacementAnimation();
            PlayPlacementParticles();
            
            // Audio feedback
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayPuzzleComplete();
            }
            
            OnPiecePlaced?.Invoke(this);
        }
        
        Debug.Log($"📌 Piece {pieceId} placed status: {placed}");
    }
    
    /// <summary>
    /// Play animasi ketika piece di-place dengan benar
    /// </summary>
    private void PlayPlacementAnimation()
    {
        // Scale bounce animation
        Vector3 targetScale = originalScale * 1.2f;
        
        LeanTween.scale(gameObject, targetScale, bounceAnimationDuration / 2f)
            .setEase(LeanTweenType.easeOutQuad)
            .setOnComplete(() =>
            {
                LeanTween.scale(gameObject, originalScale, bounceAnimationDuration / 2f)
                    .setEase(LeanTweenType.easeInQuad);
            });
        
        // Color flash animation
        Color flashColor = Color.white;
        LeanTween.value(gameObject, 0f, 1f, bounceAnimationDuration)
            .setOnUpdate((float val) =>
            {
                Color currentColor = Color.Lerp(originalColor, flashColor, bounceCurve.Evaluate(val));
                spriteRenderer.color = currentColor;
            })
            .setOnComplete(() =>
            {
                spriteRenderer.color = originalColor;
            });
    }
    
    /// <summary>
    /// Play particle effect ketika piece placed
    /// </summary>
    private void PlayPlacementParticles()
    {
        if (placeParticles != null)
        {
            placeParticles.Play();
        }
    }
    
    /// <summary>
    /// Animate piece snap ke target position
    /// </summary>
    public void SnapToPosition(Vector3 targetPosition)
    {
        LeanTween.move(gameObject, targetPosition, snapAnimationDuration)
            .setEase(snapEaseType)
            .setOnComplete(() =>
            {
                transform.position = targetPosition;
            });
    }
    
    /// <summary>
    /// Animate piece return ke original position
    /// </summary>
    public void ReturnToOriginalPosition()
    {
        LeanTween.move(gameObject, originalPosition, snapAnimationDuration)
            .setEase(LeanTweenType.easeOutQuad);
    }
    
    /// <summary>
    /// Set piece dapat di-drag atau tidak
    /// </summary>
    public void SetDraggable(bool draggable)
    {
        canBeDragged = draggable && !isPlaced;
    }
    
    /// <summary>
    /// Update posisi piece (dipanggil dari PlayerController)
    /// </summary>
    public void UpdatePosition(Vector3 newPosition)
    {
        if (isDragging && canBeDragged)
        {
            transform.position = newPosition;
            OnPieceMoved?.Invoke(this, newPosition);
        }
    }
    
    /// <summary>
    /// Set visual highlight state
    /// </summary>
    public void SetHighlight(bool highlighted)
    {
        if (glowEffect != null)
        {
            glowEffect.SetActive(highlighted && glowWhenSelected);
        }
        
        // Sorting order adjustment untuk highlight
        if (highlighted)
        {
            spriteRenderer.sortingOrder = 10; // Bring to front
        }
        else
        {
            spriteRenderer.sortingOrder = 0; // Normal order
        }
    }
    
    /// <summary>
    /// Reset piece ke state awal
    /// </summary>
    public void ResetPiece()
    {
        isPlaced = false;
        isDragging = false;
        canBeDragged = true;
        
        transform.position = originalPosition;
        transform.localScale = originalScale;
        spriteRenderer.color = originalColor;
        
        SetHighlight(false);
        
        Debug.Log($"🔄 Piece {pieceId} reset to original state");
    }
    
    // === IDraggable Interface Implementation ===
    
    /// <summary>
    /// Called when drag starts
    /// </summary>
    public void OnDragStart()
    {
        if (!canBeDragged) return;
        
        isDragging = true;
        SetHighlight(true);
        
        // Store current position as original jika belum di-set
        if (originalPosition == Vector3.zero)
        {
            originalPosition = transform.position;
        }
        
        OnPieceSelected?.Invoke(this);
        
        Debug.Log($"🎯 Started dragging piece: {pieceId}");
    }
    
    /// <summary>
    /// Called when drag ends
    /// </summary>
    public void OnDragEnd()
    {
        if (!isDragging) return;
        
        isDragging = false;
        SetHighlight(false);
        
        OnPieceDeselected?.Invoke(this);
        
        Debug.Log($"🏁 Stopped dragging piece: {pieceId}");
    }
    
    /// <summary>
    /// Check if this piece can be dragged
    /// </summary>
    public bool CanBeDragged()
    {
        return canBeDragged && !isPlaced;
    }
    
    // === Public Getters ===
    
    public bool IsPlaced() { return isPlaced; }
    public bool IsDragging() { return isDragging; }
    public string GetPieceId() { return pieceId; }
    public PieceType GetPieceType() { return pieceType; }
    public Vector3 GetOriginalPosition() { return originalPosition; }
    
    /// <summary>
    /// Get piece description untuk accessibility atau debug
    /// </summary>
    public string GetPieceDescription()
    {
        string description = $"Piece {pieceId} - Type: {pieceType}";
        
        if (pieceData.Count > 0)
        {
            description += " - Data: ";
            foreach (var data in pieceData)
            {
                description += $"{data.Key}={data.Value} ";
            }
        }
        
        return description.Trim();
    }
    
    /// <summary>
    /// Compare piece dengan piece lain untuk matching
    /// </summary>
    public bool MatchesPiece(PuzzlePiece otherPiece)
    {
        if (otherPiece == null) return false;
        
        // Basic matching berdasarkan type dan color
        if (pieceType != otherPiece.pieceType) return false;
        if (pieceColor != otherPiece.pieceColor) return false;
        
        // Extended matching berdasarkan custom data
        foreach (var data in pieceData)
        {
            if (!otherPiece.pieceData.ContainsKey(data.Key)) return false;
            if (!otherPiece.pieceData[data.Key].Equals(data.Value)) return false;
        }
        
        return true;
    }
    
    // === Events for UI Integration ===
    
    private void OnMouseDown()
    {
        // Backup untuk mouse input jika tidak menggunakan PlayerController
        if (CanBeDragged())
        {
            OnDragStart();
        }
    }
    
    private void OnMouseUp()
    {
        // Backup untuk mouse input
        if (isDragging)
        {
            OnDragEnd();
        }
    }
    
    // === Debug Helpers ===
    
    private void OnDrawGizmosSelected()
    {
        // Draw piece boundaries untuk debugging
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        
        // Draw original position
        if (originalPosition != Vector3.zero)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(originalPosition, 0.3f);
            Gizmos.DrawLine(transform.position, originalPosition);
        }
    }
}

/// <summary>
/// Enum untuk jenis-jenis piece
/// </summary>
public enum PieceType
{
    Shape,
    Color,
    Number,
    Pattern,
    Jigsaw,
    Custom
}

/// <summary>
/// Drop Target component untuk receiving dropped pieces
/// </summary>
public class DropTarget : MonoBehaviour, IDropTarget
{
    [Header("Drop Target Settings")]
    public bool acceptAnyPiece = false;
    public PieceType acceptedPieceType = PieceType.Shape;
    public Color acceptedColor = Color.white;
    public Dictionary<string, object> acceptedData = new Dictionary<string, object>();
    
    [Header("Visual Feedback")]
    public Color highlightColor = Color.green;
    public Color rejectColor = Color.red;
    public bool showDropPreview = true;
    
    private bool isHighlighted = false;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    
    // Events
    public System.Action<GameObject> OnObjectAccepted;
    public System.Action<GameObject> OnObjectRejected;
    
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }
    
    /// <summary>
    /// Set accepted data untuk validation
    /// </summary>
    public void SetAcceptedData(string key, object value)
    {
        acceptedData[key] = value;
    }
    
    /// <summary>
    /// Check apakah piece bisa diterima
    /// </summary>
    public bool CanAcceptPiece(PuzzlePiece piece)
    {
        if (acceptAnyPiece) return true;
        
        // Check piece type
        if (piece.GetPieceType() != acceptedPieceType) return false;
        
        // Check custom data
        foreach (var requirement in acceptedData)
        {
            if (!piece.HasPieceData(requirement.Key)) return false;
            if (!piece.GetPieceData<object>(requirement.Key).Equals(requirement.Value)) return false;
        }
        
        return true;
    }
    
    /// <summary>
    /// Handle dropped object
    /// </summary>
    public bool OnObjectDropped(GameObject droppedObject)
    {
        PuzzlePiece piece = droppedObject.GetComponent<PuzzlePiece>();
        if (piece == null) return false;
        
        bool accepted = CanAcceptPiece(piece);
        
        if (accepted)
        {
            OnObjectAccepted?.Invoke(droppedObject);
            ShowAcceptedFeedback();
        }
        else
        {
            OnObjectRejected?.Invoke(droppedObject);
            ShowRejectedFeedback();
        }
        
        return accepted;
    }
    
    public bool CanAcceptDrop(GameObject draggedObject)
    {
        PuzzlePiece piece = draggedObject.GetComponent<PuzzlePiece>();
        return piece != null && CanAcceptPiece(piece);
    }
    
    public void OnDropHover(GameObject hoveredObject)
    {
        if (!isHighlighted && showDropPreview)
        {
            bool canAccept = CanAcceptDrop(hoveredObject);
            Color previewColor = canAccept ? highlightColor : rejectColor;
            
            if (spriteRenderer != null)
            {
                spriteRenderer.color = previewColor;
                isHighlighted = true;
            }
        }
    }
    
    public void OnDropExit(GameObject exitedObject)
    {
        if (isHighlighted)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = originalColor;
                isHighlighted = false;
            }
        }
    }
    
    private void ShowAcceptedFeedback()
    {
        // Flash green
        StartCoroutine(FlashColor(highlightColor, 0.5f));
    }
    
    private void ShowRejectedFeedback()
    {
        // Flash red
        StartCoroutine(FlashColor(rejectColor, 0.5f));
    }
    
    private IEnumerator FlashColor(Color flashColor, float duration)
    {
        if (spriteRenderer == null) yield break;
        
        Color startColor = spriteRenderer.color;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.PingPong(elapsed * 4f, 1f); // Flash effect
            spriteRenderer.color = Color.Lerp(startColor, flashColor, t);
            yield return null;
        }
        
        spriteRenderer.color = originalColor;
        isHighlighted = false;
    }
}