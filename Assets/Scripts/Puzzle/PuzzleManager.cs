using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PuzzleManager - Mengelola logika puzzle, validasi jawaban, dan progression
/// Dirancang untuk berbagai jenis puzzle yang cocok untuk anak-anak
/// </summary>
public class PuzzleManager : MonoBehaviour
{
    [Header("Puzzle Configuration")]
    public PuzzleType puzzleType = PuzzleType.ShapeMatching;
    public int puzzleLevel = 1;
    public float timeLimit = 180f; // 3 menit per puzzle
    public bool hasTimeLimit = false;
    
    [Header("Difficulty Settings")]
    [Range(1, 5)]
    public int difficultyLevel = 1;
    public int piecesCount = 4; // Jumlah piece puzzle
    public bool allowHints = true;
    public int maxHints = 3;
    
    [Header("Puzzle Elements")]
    public Transform puzzleContainer;
    public Transform solutionContainer;
    public List<PuzzlePiece> puzzlePieces = new List<PuzzlePiece>();
    public List<Transform> dropTargets = new List<Transform>();
    
    [Header("Visual Feedback")]
    public GameObject completionParticles;
    public GameObject hintIndicator;
    public Color correctColor = Color.green;
    public Color incorrectColor = Color.red;
    public Color hintColor = Color.yellow;
    
    [Header("Audio References")]
    public AudioClip puzzleStartSound;
    public AudioClip correctPlacementSound;
    public AudioClip incorrectPlacementSound;
    public AudioClip puzzleCompleteSound;
    public AudioClip hintUsedSound;
    
    // Private variables
    private float currentTime;
    private int correctPlacements = 0;
    private int totalPlacements = 0;
    private int hintsUsed = 0;
    private bool isPuzzleComplete = false;
    private bool isPuzzleStarted = false;
    
    // Puzzle solution data
    private Dictionary<PuzzlePiece, Transform> correctSolutions = new Dictionary<PuzzlePiece, Transform>();
    private List<PuzzlePiece> completedPieces = new List<PuzzlePiece>();
    
    // Events
    public System.Action<int> OnCorrectPlacement;
    public System.Action<int> OnIncorrectPlacement;
    public System.Action<float> OnTimeUpdate;
    public System.Action<int, int> OnProgressUpdate; // (correct, total)
    public System.Action<PuzzleCompletionData> OnPuzzleComplete;
    public System.Action OnHintUsed;
    
    private void Start()
    {
        InitializePuzzle();
    }
    
    private void Update()
    {
        if (isPuzzleStarted && !isPuzzleComplete)
        {
            UpdateTimer();
        }
    }
    
    /// <summary>
    /// Inisialisasi puzzle berdasarkan level dan tipe
    /// </summary>
    private void InitializePuzzle()
    {
        // Setup puzzle berdasarkan difficulty level
        SetupDifficultySettings();
        
        // Generate puzzle layout
        GeneratePuzzleLayout();
        
        // Setup correct solutions
        SetupCorrectSolutions();
        
        // Start puzzle
        StartPuzzle();
        
        Debug.Log($"🧩 Puzzle Initialized - Type: {puzzleType}, Level: {puzzleLevel}, Difficulty: {difficultyLevel}");
    }
    
    /// <summary>
    /// Setup pengaturan kesulitan berdasarkan level
    /// </summary>
    private void SetupDifficultySettings()
    {
        switch (difficultyLevel)
        {
            case 1: // Easy
                piecesCount = 3;
                timeLimit = 300f; // 5 menit
                maxHints = 5;
                break;
            case 2: // Normal
                piecesCount = 4;
                timeLimit = 240f; // 4 menit
                maxHints = 3;
                break;
            case 3: // Medium
                piecesCount = 6;
                timeLimit = 180f; // 3 menit
                maxHints = 2;
                break;
            case 4: // Hard
                piecesCount = 8;
                timeLimit = 150f; // 2.5 menit
                maxHints = 1;
                break;
            case 5: // Expert
                piecesCount = 10;
                timeLimit = 120f; // 2 menit
                maxHints = 0;
                allowHints = false;
                break;
        }
        
        Debug.Log($"⚙️ Difficulty Settings: {piecesCount} pieces, {timeLimit}s time limit, {maxHints} hints");
    }
    
    /// <summary>
    /// Generate layout puzzle berdasarkan tipe
    /// </summary>
    private void GeneratePuzzleLayout()
    {
        switch (puzzleType)
        {
            case PuzzleType.ShapeMatching:
                GenerateShapeMatchingPuzzle();
                break;
            case PuzzleType.ColorSorting:
                GenerateColorSortingPuzzle();
                break;
            case PuzzleType.PatternCompletion:
                GeneratePatternCompletionPuzzle();
                break;
            case PuzzleType.NumberSequence:
                GenerateNumberSequencePuzzle();
                break;
            case PuzzleType.JigsawPuzzle:
                GenerateJigsawPuzzle();
                break;
        }
    }
    
    /// <summary>
    /// Generate shape matching puzzle
    /// </summary>
    private void GenerateShapeMatchingPuzzle()
    {
        // Create basic shapes untuk matching
        string[] shapes = { "Circle", "Square", "Triangle", "Star", "Heart", "Diamond" };
        Color[] colors = { Color.red, Color.blue, Color.green, Color.yellow, Color.magenta, Color.cyan };
        
        for (int i = 0; i < piecesCount && i < shapes.Length; i++)
        {
            // Create puzzle piece
            GameObject pieceObj = CreatePuzzlePiece(shapes[i], colors[i]);
            PuzzlePiece piece = pieceObj.GetComponent<PuzzlePiece>();
            puzzlePieces.Add(piece);
            
            // Create corresponding drop target
            GameObject targetObj = CreateDropTarget(shapes[i], colors[i]);
            dropTargets.Add(targetObj.transform);
            
            // Store correct solution
            correctSolutions[piece] = targetObj.transform;
        }
        
        // Shuffle puzzle pieces positions
        ShufflePuzzlePieces();
    }
    
    /// <summary>
    /// Generate color sorting puzzle
    /// </summary>
    private void GenerateColorSortingPuzzle()
    {
        Color[] colors = { Color.red, Color.blue, Color.green, Color.yellow };
        
        for (int i = 0; i < piecesCount; i++)
        {
            Color pieceColor = colors[i % colors.Length];
            
            // Create colored object to sort
            GameObject pieceObj = CreatePuzzlePiece($"ColorPiece_{i}", pieceColor);
            PuzzlePiece piece = pieceObj.GetComponent<PuzzlePiece>();
            piece.SetPieceData("color", pieceColor);
            puzzlePieces.Add(piece);
        }
        
        // Create color sorting bins
        foreach (Color color in colors)
        {
            GameObject binObj = CreateDropTarget($"ColorBin", color);
            DropTarget dropTarget = binObj.GetComponent<DropTarget>();
            dropTarget.SetAcceptedData("color", color);
            dropTargets.Add(binObj.transform);
        }
        
        ShufflePuzzlePieces();
    }
    
    /// <summary>
    /// Generate pattern completion puzzle
    /// </summary>
    private void GeneratePatternCompletionPuzzle()
    {
        // Create pattern dengan missing pieces
        int[] pattern = { 1, 2, 3, 4, 5 };
        bool[] missingIndices = new bool[pattern.Length];
        
        // Randomly remove beberapa pieces
        int missingCount = Mathf.Min(piecesCount, pattern.Length - 1);
        for (int i = 0; i < missingCount; i++)
        {
            int randomIndex = Random.Range(0, pattern.Length);
            if (!missingIndices[randomIndex])
            {
                missingIndices[randomIndex] = true;
            }
        }
        
        // Create pattern display dan missing pieces
        for (int i = 0; i < pattern.Length; i++)
        {
            if (missingIndices[i])
            {
                // Create missing piece yang harus diisi
                GameObject targetObj = CreateDropTarget($"PatternSlot_{i}", Color.gray);
                dropTargets.Add(targetObj.transform);
                
                // Create corresponding piece
                GameObject pieceObj = CreatePuzzlePiece($"PatternPiece_{pattern[i]}", Color.white);
                PuzzlePiece piece = pieceObj.GetComponent<PuzzlePiece>();
                piece.SetPieceData("patternValue", pattern[i]);
                puzzlePieces.Add(piece);
                
                // Store solution
                correctSolutions[piece] = targetObj.transform;
            }
        }
        
        ShufflePuzzlePieces();
    }
    
    /// <summary>
    /// Generate number sequence puzzle
    /// </summary>
    private void GenerateNumberSequencePuzzle()
    {
        // Create simple number sequence (1, 2, 3, 4, 5...)
        for (int i = 1; i <= piecesCount; i++)
        {
            // Create number piece
            GameObject pieceObj = CreatePuzzlePiece($"Number_{i}", Color.white);
            PuzzlePiece piece = pieceObj.GetComponent<PuzzlePiece>();
            piece.SetPieceData("number", i);
            puzzlePieces.Add(piece);
            
            // Create number slot
            GameObject slotObj = CreateDropTarget($"NumberSlot_{i}", Color.gray);
            DropTarget slot = slotObj.GetComponent<DropTarget>();
            slot.SetAcceptedData("number", i);
            dropTargets.Add(slotObj.transform);
            
            // Store solution
            correctSolutions[piece] = slotObj.transform;
        }
        
        ShufflePuzzlePieces();
    }
    
    /// <summary>
    /// Generate jigsaw puzzle
    /// </summary>
    private void GenerateJigsawPuzzle()
    {
        // Create simple jigsaw pieces
        for (int i = 0; i < piecesCount; i++)
        {
            GameObject pieceObj = CreatePuzzlePiece($"JigsawPiece_{i}", Color.white);
            PuzzlePiece piece = pieceObj.GetComponent<PuzzlePiece>();
            piece.SetPieceData("jigsawId", i);
            puzzlePieces.Add(piece);
            
            GameObject slotObj = CreateDropTarget($"JigsawSlot_{i}", Color.gray);
            DropTarget slot = slotObj.GetComponent<DropTarget>();
            slot.SetAcceptedData("jigsawId", i);
            dropTargets.Add(slotObj.transform);
            
            correctSolutions[piece] = slotObj.transform;
        }
        
        ShufflePuzzlePieces();
    }
    
    /// <summary>
    /// Create puzzle piece GameObject
    /// </summary>
    private GameObject CreatePuzzlePiece(string pieceName, Color pieceColor)
    {
        GameObject piece = new GameObject(pieceName);
        piece.transform.SetParent(puzzleContainer);
        
        // Add visual components
        SpriteRenderer renderer = piece.AddComponent<SpriteRenderer>();
        renderer.color = pieceColor;
        renderer.sortingLayerName = "PuzzlePieces";
        
        // Add collider untuk interaction
        CircleCollider2D collider = piece.AddComponent<CircleCollider2D>();
        collider.radius = 0.5f;
        
        // Add puzzle piece component
        PuzzlePiece puzzlePieceComponent = piece.AddComponent<PuzzlePiece>();
        
        return piece;
    }
    
    /// <summary>
    /// Create drop target GameObject
    /// </summary>
    private GameObject CreateDropTarget(string targetName, Color targetColor)
    {
        GameObject target = new GameObject(targetName);
        target.transform.SetParent(solutionContainer);
        
        // Add visual components
        SpriteRenderer renderer = target.AddComponent<SpriteRenderer>();
        renderer.color = new Color(targetColor.r, targetColor.g, targetColor.b, 0.3f); // Semi-transparent
        renderer.sortingLayerName = "DropTargets";
        
        // Add collider untuk drop detection
        CircleCollider2D collider = target.AddComponent<CircleCollider2D>();
        collider.radius = 0.6f;
        collider.isTrigger = true;
        
        // Add drop target component
        DropTarget dropTargetComponent = target.AddComponent<DropTarget>();
        
        return target;
    }
    
    /// <summary>
    /// Shuffle posisi puzzle pieces
    /// </summary>
    private void ShufflePuzzlePieces()
    {
        for (int i = 0; i < puzzlePieces.Count; i++)
        {
            Vector3 randomPosition = GetRandomPiecePosition();
            puzzlePieces[i].transform.position = randomPosition;
        }
    }
    
    /// <summary>
    /// Get posisi random untuk puzzle piece
    /// </summary>
    private Vector3 GetRandomPiecePosition()
    {
        float x = Random.Range(-4f, 4f);
        float y = Random.Range(-3f, 3f);
        return new Vector3(x, y, 0);
    }
    
    /// <summary>
    /// Setup correct solutions mapping
    /// </summary>
    private void SetupCorrectSolutions()
    {
        // Solutions sudah di-setup di generate methods
        Debug.Log($"✅ Setup {correctSolutions.Count} correct solutions");
    }
    
    /// <summary>
    /// Start puzzle gameplay
    /// </summary>
    public void StartPuzzle()
    {
        isPuzzleStarted = true;
        currentTime = hasTimeLimit ? timeLimit : 0f;
        
        // Play start sound
        if (AudioManager.Instance != null && puzzleStartSound != null)
        {
            AudioManager.Instance.PlaySFX(puzzleStartSound);
        }
        
        // Subscribe to player controller events
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.OnObjectReleased += OnPieceDropped;
        }
        
        Debug.log("🚀 Puzzle Started!");
    }
    
    /// <summary>
    /// Handle when puzzle piece dropped
    /// </summary>
    private void OnPieceDropped(GameObject droppedObject)
    {
        PuzzlePiece piece = droppedObject.GetComponent<PuzzlePiece>();
        if (piece == null) return;
        
        // Check apakah placement benar
        Transform nearestTarget = GetNearestDropTarget(droppedObject.transform.position);
        if (nearestTarget != null)
        {
            bool isCorrect = ValidatePlacement(piece, nearestTarget);
            
            if (isCorrect)
            {
                HandleCorrectPlacement(piece, nearestTarget);
            }
            else
            {
                HandleIncorrectPlacement(piece);
            }
        }
    }
    
    /// <summary>
    /// Get drop target terdekat dari posisi
    /// </summary>
    private Transform GetNearestDropTarget(Vector3 position)
    {
        Transform nearest = null;
        float nearestDistance = float.MaxValue;
        
        foreach (Transform target in dropTargets)
        {
            float distance = Vector3.Distance(position, target.position);
            if (distance < nearestDistance && distance < 1f) // Threshold untuk drop
            {
                nearest = target;
                nearestDistance = distance;
            }
        }
        
        return nearest;
    }
    
    /// <summary>
    /// Validasi apakah placement piece benar
    /// </summary>
    private bool ValidatePlacement(PuzzlePiece piece, Transform target)
    {
        // Check direct solution mapping
        if (correctSolutions.ContainsKey(piece))
        {
            return correctSolutions[piece] == target;
        }
        
        // Additional validation berdasarkan puzzle type
        DropTarget dropTarget = target.GetComponent<DropTarget>();
        if (dropTarget != null)
        {
            return dropTarget.CanAcceptPiece(piece);
        }
        
        return false;
    }
    
    /// <summary>
    /// Handle correct placement
    /// </summary>
    private void HandleCorrectPlacement(PuzzlePiece piece, Transform target)
    {
        correctPlacements++;
        totalPlacements++;
        
        // Snap piece ke target position
        piece.transform.position = target.position;
        piece.SetPlaced(true);
        completedPieces.Add(piece);
        
        // Visual feedback
        ShowFeedback(target.position, true);
        
        // Audio feedback
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(correctPlacementSound);
        }
        
        // Events
        OnCorrectPlacement?.Invoke(correctPlacements);
        OnProgressUpdate?.Invoke(correctPlacements, piecesCount);
        
        // Check puzzle completion
        if (correctPlacements >= piecesCount)
        {
            CompletePuzzle();
        }
        
        Debug.Log($"✅ Correct placement! Progress: {correctPlacements}/{piecesCount}");
    }
    
    /// <summary>
    /// Handle incorrect placement
    /// </summary>
    private void HandleIncorrectPlacement(PuzzlePiece piece)
    {
        totalPlacements++;
        
        // Visual feedback
        ShowFeedback(piece.transform.position, false);
        
        // Audio feedback
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(incorrectPlacementSound);
        }
        
        // Return piece ke posisi semula atau posisi default
        StartCoroutine(ReturnPieceToSafePosition(piece));
        
        // Events
        OnIncorrectPlacement?.Invoke(totalPlacements - correctPlacements);
        
        Debug.Log($"❌ Incorrect placement. Wrong attempts: {totalPlacements - correctPlacements}");
    }
    
    /// <summary>
    /// Return piece ke posisi aman setelah salah
    /// </summary>
    private IEnumerator ReturnPieceToSafePosition(PuzzlePiece piece)
    {
        Vector3 originalPosition = piece.transform.position;
        Vector3 safePosition = GetRandomPiecePosition();
        
        float duration = 0.5f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            piece.transform.position = Vector3.Lerp(originalPosition, safePosition, t);
            yield return null;
        }
        
        piece.transform.position = safePosition;
    }
    
    /// <summary>
    /// Show visual feedback untuk placement
    /// </summary>
    private void ShowFeedback(Vector3 position, bool isCorrect)
    {
        Color feedbackColor = isCorrect ? correctColor : incorrectColor;
        
        // Create temporary feedback object
        GameObject feedback = new GameObject("Feedback");
        feedback.transform.position = position;
        
        SpriteRenderer renderer = feedback.AddComponent<SpriteRenderer>();
        renderer.color = feedbackColor;
        renderer.sortingLayerName = "Effects";
        
        // Animate feedback
        StartCoroutine(AnimateFeedback(feedback, isCorrect));
    }
    
    /// <summary>
    /// Animate feedback visual
    /// </summary>
    private IEnumerator AnimateFeedback(GameObject feedbackObject, bool isCorrect)
    {
        Vector3 startScale = Vector3.zero;
        Vector3 endScale = isCorrect ? Vector3.one * 1.5f : Vector3.one * 0.8f;
        
        float duration = 0.8f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            feedbackObject.transform.localScale = Vector3.Lerp(startScale, endScale, t);
            
            SpriteRenderer renderer = feedbackObject.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                Color color = renderer.color;
                color.a = Mathf.Lerp(1f, 0f, t);
                renderer.color = color;
            }
            
            yield return null;
        }
        
        Destroy(feedbackObject);
    }
    
    /// <summary>
    /// Complete puzzle dengan celebrasi
    /// </summary>
    private void CompletePuzzle()
    {
        isPuzzleComplete = true;
        isPuzzleStarted = false;
        
        // Play completion particles
        if (completionParticles != null)
        {
            completionParticles.SetActive(true);
        }
        
        // Play completion sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(puzzleCompleteSound);
        }
        
        // Calculate performance
        PuzzleCompletionData completionData = CalculatePerformance();
        
        // Trigger completion event
        OnPuzzleComplete?.Invoke(completionData);
        
        // Auto proceed ke GameManager
        if (GameManager.Instance != null)
        {
            int starsEarned = completionData.starsEarned;
            int coinsEarned = completionData.coinsEarned;
            GameManager.Instance.CompleteLevel(starsEarned, coinsEarned);
        }
        
        Debug.Log($"🎉 Puzzle Complete! Stars: {completionData.starsEarned}, Coins: {completionData.coinsEarned}");
    }
    
    /// <summary>
    /// Calculate performance dan rewards
    /// </summary>
    private PuzzleCompletionData CalculatePerformance()
    {
        PuzzleCompletionData data = new PuzzleCompletionData();
        
        // Calculate completion time
        data.completionTime = hasTimeLimit ? timeLimit - currentTime : currentTime;
        
        // Calculate accuracy
        data.accuracy = totalPlacements > 0 ? (float)correctPlacements / totalPlacements : 1f;
        
        // Calculate stars (1-3)
        int stars = 1; // Base star untuk completion
        
        if (data.accuracy >= 0.9f) stars++; // High accuracy bonus
        if (hasTimeLimit && currentTime > timeLimit * 0.5f) stars++; // Time bonus
        if (hintsUsed == 0) stars++; // No hints bonus
        
        data.starsEarned = Mathf.Min(stars, 3);
        
        // Calculate coins
        int baseCoins = 10 * difficultyLevel;
        int accuracyBonus = Mathf.RoundToInt(data.accuracy * 20);
        int timeBonus = hasTimeLimit && currentTime > timeLimit * 0.7f ? 15 : 0;
        int hintspenalty = hintsUsed * 5;
        
        data.coinsEarned = Mathf.Max(baseCoins + accuracyBonus + timeBonus - hintspenalty, 5);
        
        // Additional data
        data.wrongAttempts = totalPlacements - correctPlacements;
        data.hintsUsed = hintsUsed;
        
        return data;
    }
    
    /// <summary>
    /// Update timer
    /// </summary>
    private void UpdateTimer()
    {
        if (hasTimeLimit)
        {
            currentTime -= Time.deltaTime;
            
            if (currentTime <= 0)
            {
                currentTime = 0;
                HandleTimeUp();
            }
        }
        else
        {
            currentTime += Time.deltaTime;
        }
        
        OnTimeUpdate?.Invoke(currentTime);
    }
    
    /// <summary>
    /// Handle when time runs out
    /// </summary>
    private void HandleTimeUp()
    {
        isPuzzleStarted = false;
        
        Debug.Log("⏰ Time's up!");
        
        // Show time up feedback
        // Bisa ditambahkan UI atau restart option
    }
    
    /// <summary>
    /// Use hint untuk membantu player
    /// </summary>
    public void UseHint()
    {
        if (!allowHints || hintsUsed >= maxHints) return;
        
        hintsUsed++;
        
        // Find piece yang belum placed
        PuzzlePiece hintPiece = null;
        foreach (PuzzlePiece piece in puzzlePieces)
        {
            if (!piece.IsPlaced())
            {
                hintPiece = piece;
                break;
            }
        }
        
        if (hintPiece != null && correctSolutions.ContainsKey(hintPiece))
        {
            Transform correctTarget = correctSolutions[hintPiece];
            
            // Show hint indicator
            ShowHintIndicator(hintPiece.transform.position, correctTarget.position);
            
            // Play hint sound
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(hintUsedSound);
            }
        }
        
        OnHintUsed?.Invoke();
        
        Debug.Log($"💡 Hint used! Remaining: {maxHints - hintsUsed}");
    }
    
    /// <summary>
    /// Show hint indicator
    /// </summary>
    private void ShowHintIndicator(Vector3 fromPosition, Vector3 toPosition)
    {
        if (hintIndicator != null)
        {
            hintIndicator.SetActive(true);
            // Implement hint animation (arrow, line, highlight, etc.)
            StartCoroutine(AnimateHint(fromPosition, toPosition));
        }
    }
    
    /// <summary>
    /// Animate hint indicator
    /// </summary>
    private IEnumerator AnimateHint(Vector3 from, Vector3 to)
    {
        float duration = 2f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // Animate hint (implementation tergantung visual design)
            
            yield return null;
        }
        
        if (hintIndicator != null)
        {
            hintIndicator.SetActive(false);
        }
    }
    
    // === PUBLIC GETTERS ===
    
    public float GetCurrentTime() { return currentTime; }
    public int GetCorrectPlacements() { return correctPlacements; }
    public int GetTotalPlacements() { return totalPlacements; }
    public int GetHintsUsed() { return hintsUsed; }
    public int GetMaxHints() { return maxHints; }
    public bool CanUseHint() { return allowHints && hintsUsed < maxHints; }
    public bool IsPuzzleComplete() { return isPuzzleComplete; }
    public float GetAccuracy() { return totalPlacements > 0 ? (float)correctPlacements / totalPlacements : 0f; }
}

/// <summary>
/// Enum untuk jenis-jenis puzzle
/// </summary>
public enum PuzzleType
{
    ShapeMatching,
    ColorSorting,
    PatternCompletion,
    NumberSequence,
    JigsawPuzzle
}

/// <summary>
/// Data completion puzzle untuk rewards calculation
/// </summary>
[System.Serializable]
public class PuzzleCompletionData
{
    public int starsEarned;
    public int coinsEarned;
    public float completionTime;
    public float accuracy;
    public int wrongAttempts;
    public int hintsUsed;
}