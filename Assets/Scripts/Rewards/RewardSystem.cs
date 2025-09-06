using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// RewardSystem - Mengelola sistem reward, achievement, dan item collection
/// Dirancang untuk memberikan motivation dan progression yang menyenangkan untuk anak-anak
/// </summary>
public class RewardSystem : MonoBehaviour
{
    [Header("Reward Configuration")]
    public int baseStarsPerLevel = 1;
    public int maxStarsPerLevel = 3;
    public int baseCoinsPerLevel = 10;
    public float accuracyBonusMultiplier = 1.5f;
    public float timeBonusMultiplier = 1.2f;
    
    [Header("Item Shop")]
    public List<ShopItem> shopItems = new List<ShopItem>();
    public List<StickerItem> stickerCollection = new List<StickerItem>();
    public List<CharacterCostume> costumes = new List<CharacterCostume>();
    
    [Header("Achievement System")]
    public List<Achievement> achievements = new List<Achievement>();
    
    [Header("Daily Rewards")]
    public List<DailyReward> dailyRewards = new List<DailyReward>();
    public bool enableDailyRewards = true;
    
    // Private variables
    private Dictionary<string, ShopItem> itemsDict = new Dictionary<string, ShopItem>();
    private Dictionary<string, Achievement> achievementsDict = new Dictionary<string, Achievement>();
    private List<string> unlockedItems = new List<string>();
    private List<string> completedAchievements = new List<string>();
    
    // Player collection
    private List<string> ownedStickers = new List<string>();
    private List<string> ownedCostumes = new List<string>();
    private string currentCostume = "";
    
    // Daily reward tracking
    private System.DateTime lastDailyRewardClaim;
    private int dailyRewardStreak = 0;
    
    // Singleton pattern
    public static RewardSystem Instance;
    
    // Events
    public System.Action<int> OnStarsEarned;
    public System.Action<int> OnCoinsEarned;
    public System.Action<ShopItem> OnItemPurchased;
    public System.Action<Achievement> OnAchievementUnlocked;
    public System.Action<DailyReward> OnDailyRewardClaimed;
    public System.Action<string> OnStickerUnlocked;
    public System.Action<string> OnCostumeUnlocked;
    
    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeRewardSystem();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        LoadRewardData();
        SetupDefaultItems();
        CheckDailyRewards();
    }
    
    /// <summary>
    /// Inisialisasi reward system
    /// </summary>
    private void InitializeRewardSystem()
    {
        // Setup item dictionaries untuk akses cepat
        foreach (var item in shopItems)
        {
            itemsDict[item.itemId] = item;
        }
        
        foreach (var achievement in achievements)
        {
            achievementsDict[achievement.achievementId] = achievement;
        }
        
        Debug.Log("🎁 Reward System Initialized");
    }
    
    /// <summary>
    /// Load reward data dari PlayerPrefs
    /// </summary>
    private void LoadRewardData()
    {
        // Load owned items
        string ownedItemsData = PlayerPrefs.GetString("OwnedItems", "");
        if (!string.IsNullOrEmpty(ownedItemsData))
        {
            unlockedItems = ownedItemsData.Split(',').ToList();
        }
        
        // Load completed achievements
        string achievementsData = PlayerPrefs.GetString("CompletedAchievements", "");
        if (!string.IsNullOrEmpty(achievementsData))
        {
            completedAchievements = achievementsData.Split(',').ToList();
        }
        
        // Load sticker collection
        string stickersData = PlayerPrefs.GetString("OwnedStickers", "");
        if (!string.IsNullOrEmpty(stickersData))
        {
            ownedStickers = stickersData.Split(',').ToList();
        }
        
        // Load costume collection
        string costumesData = PlayerPrefs.GetString("OwnedCostumes", "");
        if (!string.IsNullOrEmpty(costumesData))
        {
            ownedCostumes = costumesData.Split(',').ToList();
        }
        
        currentCostume = PlayerPrefs.GetString("CurrentCostume", "");
        
        // Load daily reward data
        string lastClaimDate = PlayerPrefs.GetString("LastDailyRewardClaim", "");
        if (!string.IsNullOrEmpty(lastClaimDate))
        {
            System.DateTime.TryParse(lastClaimDate, out lastDailyRewardClaim);
        }
        dailyRewardStreak = PlayerPrefs.GetInt("DailyRewardStreak", 0);
        
        Debug.Log($"💾 Reward Data Loaded - Items: {unlockedItems.Count}, Achievements: {completedAchievements.Count}");
    }
    
    /// <summary>
    /// Save reward data ke PlayerPrefs
    /// </summary>
    private void SaveRewardData()
    {
        // Save owned items
        PlayerPrefs.SetString("OwnedItems", string.Join(",", unlockedItems.ToArray()));
        
        // Save completed achievements
        PlayerPrefs.SetString("CompletedAchievements", string.Join(",", completedAchievements.ToArray()));
        
        // Save sticker collection
        PlayerPrefs.SetString("OwnedStickers", string.Join(",", ownedStickers.ToArray()));
        
        // Save costume collection
        PlayerPrefs.SetString("OwnedCostumes", string.Join(",", ownedCostumes.ToArray()));
        
        PlayerPrefs.SetString("CurrentCostume", currentCostume);
        
        // Save daily reward data
        PlayerPrefs.SetString("LastDailyRewardClaim", lastDailyRewardClaim.ToString());
        PlayerPrefs.SetInt("DailyRewardStreak", dailyRewardStreak);
        
        PlayerPrefs.Save();
        
        Debug.Log("💾 Reward Data Saved");
    }
    
    /// <summary>
    /// Setup default shop items dan rewards
    /// </summary>
    private void SetupDefaultItems()
    {
        // Setup default stickers jika list kosong
        if (stickerCollection.Count == 0)
        {
            AddDefaultStickers();
        }
        
        // Setup default costumes jika list kosong
        if (costumes.Count == 0)
        {
            AddDefaultCostumes();
        }
        
        // Setup default achievements jika list kosong
        if (achievements.Count == 0)
        {
            AddDefaultAchievements();
        }
        
        // Setup daily rewards jika list kosong
        if (dailyRewards.Count == 0)
        {
            AddDefaultDailyRewards();
        }
    }
    
    /// <summary>
    /// Add default stickers
    /// </summary>
    private void AddDefaultStickers()
    {
        stickerCollection.Add(new StickerItem("star_gold", "Bintang Emas", 50, StickerCategory.Stars));
        stickerCollection.Add(new StickerItem("heart_pink", "Hati Pink", 30, StickerCategory.Love));
        stickerCollection.Add(new StickerItem("rainbow", "Pelangi Cerah", 80, StickerCategory.Nature));
        stickerCollection.Add(new StickerItem("smile_happy", "Senyum Bahagia", 25, StickerCategory.Emotions));
        stickerCollection.Add(new StickerItem("trophy_gold", "Piala Emas", 100, StickerCategory.Achievement));
    }
    
    /// <summary>
    /// Add default costumes
    /// </summary>
    private void AddDefaultCostumes()
    {
        costumes.Add(new CharacterCostume("superhero", "Superhero", 150, "Kostum superhero keren!"));
        costumes.Add(new CharacterCostume("princess", "Putri", 120, "Kostum putri yang cantik!"));
        costumes.Add(new CharacterCostume("astronaut", "Astronot", 200, "Jelajahi luar angkasa!"));
        costumes.Add(new CharacterCostume("pirate", "Bajak Laut", 180, "Ahoy, matey!"));
    }
    
    /// <summary>
    /// Add default achievements
    /// </summary>
    private void AddDefaultAchievements()
    {
        achievements.Add(new Achievement("first_puzzle", "Puzzle Pertama", "Selesaikan puzzle pertama", 10, 1));
        achievements.Add(new Achievement("perfect_score", "Nilai Sempurna", "Selesaikan puzzle tanpa kesalahan", 50, 1));
        achievements.Add(new Achievement("speed_master", "Master Kecepatan", "Selesaikan puzzle dalam 30 detik", 75, 1));
        achievements.Add(new Achievement("collector", "Kolektor", "Kumpulkan 10 stiker", 100, 10));
        achievements.Add(new Achievement("puzzle_master", "Master Puzzle", "Selesaikan 50 level", 200, 50));
    }
    
    /// <summary>
    /// Add default daily rewards
    /// </summary>
    private void AddDefaultDailyRewards()
    {
        dailyRewards.Add(new DailyReward(1, RewardType.Coins, 20, "20 Koin"));
        dailyRewards.Add(new DailyReward(2, RewardType.Coins, 30, "30 Koin"));
        dailyRewards.Add(new DailyReward(3, RewardType.Sticker, 1, "Stiker Spesial"));
        dailyRewards.Add(new DailyReward(4, RewardType.Coins, 40, "40 Koin"));
        dailyRewards.Add(new DailyReward(5, RewardType.Coins, 50, "50 Koin"));
        dailyRewards.Add(new DailyReward(6, RewardType.Costume, 1, "Kostum Baru"));
        dailyRewards.Add(new DailyReward(7, RewardType.Coins, 100, "100 Koin Bonus!"));
    }
    
    /// <summary>
    /// Calculate stars earned berdasarkan performance
    /// </summary>
    public int CalculateStarsEarned(float accuracy, float timeBonus, bool usedHints)
    {
        int stars = baseStarsPerLevel; // Base 1 star
        
        // Accuracy bonus
        if (accuracy >= 0.9f) stars++; // 90%+ accuracy
        if (accuracy >= 1.0f) stars++; // Perfect accuracy
        
        // Time bonus
        if (timeBonus > 0.7f) stars++; // Completed quickly
        
        // Hints penalty
        if (usedHints && stars > 1) stars--; // Lose 1 star if used hints
        
        // Clamp between 1 and max stars
        stars = Mathf.Clamp(stars, 1, maxStarsPerLevel);
        
        return stars;
    }
    
    /// <summary>
    /// Calculate coins earned berdasarkan performance
    /// </summary>
    public int CalculateCoinsEarned(int level, float accuracy, float timeBonus, int hintsUsed)
    {
        int baseCoins = baseCoinsPerLevel + (level * 2); // Increase dengan level
        
        // Accuracy bonus
        int accuracyBonus = Mathf.RoundToInt(baseCoins * accuracy * accuracyBonusMultiplier);
        
        // Time bonus
        int timeBonusCoins = Mathf.RoundToInt(baseCoins * timeBonus * timeBonusMultiplier);
        
        // Hints penalty
        int hintsPenalty = hintsUsed * 5;
        
        int totalCoins = baseCoins + accuracyBonus + timeBonusCoins - hintsPenalty;
        
        return Mathf.Max(totalCoins, 5); // Minimum 5 coins
    }
    
    /// <summary>
    /// Award stars ke player
    /// </summary>
    public void AwardStars(int starsAmount)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.totalStars += starsAmount;
            GameManager.Instance.SavePlayerProgress();
        }
        
        OnStarsEarned?.Invoke(starsAmount);
        
        // Check star-based achievements
        CheckStarAchievements();
        
        Debug.Log($"⭐ Awarded {starsAmount} stars");
    }
    
    /// <summary>
    /// Award coins ke player
    /// </summary>
    public void AwardCoins(int coinsAmount)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.totalCoins += coinsAmount;
            GameManager.Instance.SavePlayerProgress();
        }
        
        OnCoinsEarned?.Invoke(coinsAmount);
        
        // Check coin-based achievements
        CheckCoinAchievements();
        
        Debug.Log($"💰 Awarded {coinsAmount} coins");
    }
    
    /// <summary>
    /// Purchase item dari shop
    /// </summary>
    public bool PurchaseItem(string itemId)
    {
        if (!itemsDict.ContainsKey(itemId))
        {
            Debug.LogWarning($"⚠️ Item '{itemId}' not found in shop");
            return false;
        }
        
        ShopItem item = itemsDict[itemId];
        
        // Check if already owned
        if (unlockedItems.Contains(itemId))
        {
            Debug.LogWarning($"⚠️ Item '{itemId}' already owned");
            return false;
        }
        
        // Check if player has enough coins
        if (GameManager.Instance != null && GameManager.Instance.totalCoins >= item.cost)
        {
            // Deduct coins
            GameManager.Instance.SpendCoins(item.cost);
            
            // Add item to collection
            unlockedItems.Add(itemId);
            
            // Special handling berdasarkan item type
            HandleItemPurchase(item);
            
            // Save progress
            SaveRewardData();
            
            OnItemPurchased?.Invoke(item);
            
            Debug.Log($"🛒 Purchased item: {item.itemName} for {item.cost} coins");
            return true;
        }
        else
        {
            Debug.LogWarning($"⚠️ Not enough coins to purchase {item.itemName}");
            return false;
        }
    }
    
    /// <summary>
    /// Handle special actions setelah item purchased
    /// </summary>
    private void HandleItemPurchase(ShopItem item)
    {
        switch (item.itemType)
        {
            case ItemType.Sticker:
                if (!ownedStickers.Contains(item.itemId))
                {
                    ownedStickers.Add(item.itemId);
                    OnStickerUnlocked?.Invoke(item.itemId);
                }
                break;
                
            case ItemType.Costume:
                if (!ownedCostumes.Contains(item.itemId))
                {
                    ownedCostumes.Add(item.itemId);
                    OnCostumeUnlocked?.Invoke(item.itemId);
                }
                break;
                
            case ItemType.PowerUp:
                // Handle power-up items
                break;
        }
    }
    
    /// <summary>
    /// Check dan unlock achievements
    /// </summary>
    public void CheckAchievement(string achievementId, int currentProgress)
    {
        if (!achievementsDict.ContainsKey(achievementId)) return;
        if (completedAchievements.Contains(achievementId)) return;
        
        Achievement achievement = achievementsDict[achievementId];
        
        if (currentProgress >= achievement.targetValue)
        {
            UnlockAchievement(achievement);
        }
    }
    
    /// <summary>
    /// Unlock achievement dan berikan reward
    /// </summary>
    private void UnlockAchievement(Achievement achievement)
    {
        completedAchievements.Add(achievement.achievementId);
        
        // Award coins sebagai reward
        AwardCoins(achievement.coinReward);
        
        // Play achievement sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayStarEarned(); // Use star sound untuk achievement
        }
        
        OnAchievementUnlocked?.Invoke(achievement);
        SaveRewardData();
        
        Debug.Log($"🏆 Achievement Unlocked: {achievement.title}");
    }
    
    /// <summary>
    /// Check star-based achievements
    /// </summary>
    private void CheckStarAchievements()
    {
        if (GameManager.Instance != null)
        {
            int totalStars = GameManager.Instance.totalStars;
            
            // Check various star milestones
            if (totalStars >= 10) CheckAchievement("star_collector_10", totalStars);
            if (totalStars >= 50) CheckAchievement("star_collector_50", totalStars);
            if (totalStars >= 100) CheckAchievement("star_master", totalStars);
        }
    }
    
    /// <summary>
    /// Check coin-based achievements
    /// </summary>
    private void CheckCoinAchievements()
    {
        if (GameManager.Instance != null)
        {
            int totalCoins = GameManager.Instance.totalCoins;
            
            // Check coin milestones
            if (totalCoins >= 100) CheckAchievement("coin_collector_100", totalCoins);
            if (totalCoins >= 500) CheckAchievement("coin_collector_500", totalCoins);
        }
    }
    
    /// <summary>
    /// Check daily rewards availability
    /// </summary>
    private void CheckDailyRewards()
    {
        if (!enableDailyRewards) return;
        
        System.DateTime now = System.DateTime.Now;
        System.TimeSpan timeSinceLastClaim = now - lastDailyRewardClaim;
        
        // Reset streak jika lebih dari 2 hari
        if (timeSinceLastClaim.TotalDays > 2)
        {
            dailyRewardStreak = 0;
        }
        
        Debug.Log($"📅 Daily Reward Check - Last claim: {timeSinceLastClaim.TotalDays:F1} days ago, Streak: {dailyRewardStreak}");
    }
    
    /// <summary>
    /// Claim daily reward
    /// </summary>
    public bool ClaimDailyReward()
    {
        if (!CanClaimDailyReward()) return false;
        
        dailyRewardStreak = (dailyRewardStreak + 1) % 7; // 7-day cycle
        if (dailyRewardStreak == 0) dailyRewardStreak = 7; // Keep it 1-7 instead of 0-6
        
        DailyReward reward = dailyRewards[dailyRewardStreak - 1];
        
        // Give reward based on type
        switch (reward.rewardType)
        {
            case RewardType.Coins:
                AwardCoins(reward.rewardAmount);
                break;
                
            case RewardType.Sticker:
                // Unlock random sticker
                UnlockRandomSticker();
                break;
                
            case RewardType.Costume:
                // Unlock random costume
                UnlockRandomCostume();
                break;
        }
        
        lastDailyRewardClaim = System.DateTime.Now;
        OnDailyRewardClaimed?.Invoke(reward);
        SaveRewardData();
        
        Debug.Log($"🎁 Daily reward claimed: {reward.rewardDescription}");
        return true;
    }
    
    /// <summary>
    /// Check if daily reward dapat di-claim
    /// </summary>
    public bool CanClaimDailyReward()
    {
        if (!enableDailyRewards) return false;
        
        System.DateTime now = System.DateTime.Now;
        System.TimeSpan timeSinceLastClaim = now - lastDailyRewardClaim;
        
        return timeSinceLastClaim.TotalHours >= 20; // Allow claim after 20 hours
    }
    
    /// <summary>
    /// Unlock random sticker
    /// </summary>
    private void UnlockRandomSticker()
    {
        List<StickerItem> availableStickers = stickerCollection.Where(s => !ownedStickers.Contains(s.stickerId)).ToList();
        
        if (availableStickers.Count > 0)
        {
            StickerItem randomSticker = availableStickers[Random.Range(0, availableStickers.Count)];
            ownedStickers.Add(randomSticker.stickerId);
            OnStickerUnlocked?.Invoke(randomSticker.stickerId);
            
            Debug.Log($"🌟 Random sticker unlocked: {randomSticker.stickerName}");
        }
    }
    
    /// <summary>
    /// Unlock random costume
    /// </summary>
    private void UnlockRandomCostume()
    {
        List<CharacterCostume> availableCostumes = costumes.Where(c => !ownedCostumes.Contains(c.costumeId)).ToList();
        
        if (availableCostumes.Count > 0)
        {
            CharacterCostume randomCostume = availableCostumes[Random.Range(0, availableCostumes.Count)];
            ownedCostumes.Add(randomCostume.costumeId);
            OnCostumeUnlocked?.Invoke(randomCostume.costumeId);
            
            Debug.Log($"👕 Random costume unlocked: {randomCostume.costumeName}");
        }
    }
    
    /// <summary>
    /// Set current costume
    /// </summary>
    public bool SetCurrentCostume(string costumeId)
    {
        if (ownedCostumes.Contains(costumeId) || costumeId == "")
        {
            currentCostume = costumeId;
            SaveRewardData();
            Debug.Log($"👕 Costume changed to: {costumeId}");
            return true;
        }
        return false;
    }
    
    // === PUBLIC GETTERS ===
    
    public List<string> GetOwnedStickers() { return ownedStickers; }
    public List<string> GetOwnedCostumes() { return ownedCostumes; }
    public List<string> GetUnlockedItems() { return unlockedItems; }
    public List<string> GetCompletedAchievements() { return completedAchievements; }
    public string GetCurrentCostume() { return currentCostume; }
    public int GetDailyRewardStreak() { return dailyRewardStreak; }
    
    public bool IsItemOwned(string itemId) { return unlockedItems.Contains(itemId); }
    public bool IsAchievementCompleted(string achievementId) { return completedAchievements.Contains(achievementId); }
    public bool IsStickerOwned(string stickerId) { return ownedStickers.Contains(stickerId); }
    public bool IsCostumeOwned(string costumeId) { return ownedCostumes.Contains(costumeId); }
}

// === DATA CLASSES ===

[System.Serializable]
public class ShopItem
{
    public string itemId;
    public string itemName;
    public string itemDescription;
    public int cost;
    public ItemType itemType;
    public Sprite itemIcon;
    
    public ShopItem(string id, string name, string description, int itemCost, ItemType type)
    {
        itemId = id;
        itemName = name;
        itemDescription = description;
        cost = itemCost;
        itemType = type;
    }
}

[System.Serializable]
public class StickerItem
{
    public string stickerId;
    public string stickerName;
    public int unlockCost;
    public StickerCategory category;
    public Sprite stickerSprite;
    
    public StickerItem(string id, string name, int cost, StickerCategory cat)
    {
        stickerId = id;
        stickerName = name;
        unlockCost = cost;
        category = cat;
    }
}

[System.Serializable]
public class CharacterCostume
{
    public string costumeId;
    public string costumeName;
    public int unlockCost;
    public string costumeDescription;
    public Sprite costumeSprite;
    
    public CharacterCostume(string id, string name, int cost, string description)
    {
        costumeId = id;
        costumeName = name;
        unlockCost = cost;
        costumeDescription = description;
    }
}

[System.Serializable]
public class Achievement
{
    public string achievementId;
    public string title;
    public string description;
    public int coinReward;
    public int targetValue;
    public bool isCompleted;
    
    public Achievement(string id, string achievementTitle, string achievementDescription, int reward, int target)
    {
        achievementId = id;
        title = achievementTitle;
        description = achievementDescription;
        coinReward = reward;
        targetValue = target;
        isCompleted = false;
    }
}

[System.Serializable]
public class DailyReward
{
    public int day;
    public RewardType rewardType;
    public int rewardAmount;
    public string rewardDescription;
    
    public DailyReward(int rewardDay, RewardType type, int amount, string description)
    {
        day = rewardDay;
        rewardType = type;
        rewardAmount = amount;
        rewardDescription = description;
    }
}

// === ENUMS ===

public enum ItemType
{
    Sticker,
    Costume,
    PowerUp,
    Decoration
}

public enum StickerCategory
{
    Stars,
    Love,
    Nature,
    Emotions,
    Achievement,
    Animals,
    Food
}

public enum RewardType
{
    Coins,
    Stars,
    Sticker,
    Costume,
    PowerUp
}