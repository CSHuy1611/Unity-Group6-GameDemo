using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("=== MANAGERS ===")]
    public InventoryManager inventoryManager;
    public TreasureManager treasureManager;

    [Header("=== RESOURCE SYSTEM ===")]
    public GameObject treePrefab;
    public List<GameObject> activeTrees = new List<GameObject>();
    [SerializeField] private int maxTrees = 5;
    [SerializeField] private float treeSpawnRadius = 15f;
    
    [Header("=== COLLECTIBLES ===")]
    public GameObject coinPrefab;
    public List<GameObject> activeCoins = new List<GameObject>();
    [SerializeField] private int fixedCoins = 10;
    [SerializeField] private float coinSpawnRadius = 20f;
    
    private GameObject treesParent;
    private GameObject coinsParent;
    
    [Header("=== SCORE ===")]
    public int score = 0;
    
    void Start()
    {
        InitializeGame();
    }
    
    void Update()
    {
        HandleDebugInput();
    }
    
    private void InitializeGame()
    {
        SetupManagers();
        InitializeManagers();
        SpawnTrees();
        SpawnCoins();
        LogAllLists();
    }
    
    private void SetupManagers()
    {
        if (inventoryManager == null)
        {
            GameObject invObj = new GameObject("InventoryManager");
            invObj.transform.SetParent(transform);
            inventoryManager = invObj.AddComponent<InventoryManager>();
        }
        
        if (treasureManager == null)
        {
            GameObject treasureObj = new GameObject("TreasureManager");
            treasureObj.transform.SetParent(transform);
            treasureManager = treasureObj.AddComponent<TreasureManager>();
        }
        
        treesParent = new GameObject("=== TREES ===");
        coinsParent = new GameObject("=== COINS ===");
    }
    
    private void InitializeManagers()
    {
        if (treasureManager != null)
        {
            treasureManager.Initialize(inventoryManager);
            Debug.Log("? TreasureManager initialized");
        }
        
        Debug.Log("? InventoryManager initialized");
    }
    
    // =============================================
    // TREE SYSTEM
    // =============================================
    
    private void SpawnTrees()
    {
        for (int i = 0; i < maxTrees; i++)
        {
            SpawnTree();
        }
        
        Debug.Log($"? Spawned {activeTrees.Count} trees");
    }
    
    private void SpawnTree()
    {
        Vector3 randomPos = GetRandomPositionInRadius(Vector3.zero, treeSpawnRadius);
        randomPos.y = 0.5f;
        
        GameObject tree = Instantiate(treePrefab, randomPos, Quaternion.identity, treesParent.transform);
        tree.name = $"Tree_{activeTrees.Count + 1}";
        
        activeTrees.Add(tree);
        
        // TODO: Phase 2
        // Tree treeScript = tree.GetComponent<Tree>();
        // if (treeScript != null)
        // {
        //     treeScript.Initialize(this);
        // }
    }
    
    public void OnTreeDestroyed(GameObject tree, Vector3 position)
    {
        if (activeTrees.Contains(tree))
        {
            activeTrees.Remove(tree);
            Debug.Log($"?? Tree chopped! Remaining trees: {activeTrees.Count}");
            
            Destroy(tree, 0.3f);
            
            SpawnTree();
        }
    }
    
    // =============================================
    // COIN SYSTEM
    // =============================================
    
    private void SpawnCoins()
    {
        for (int i = 0; i < fixedCoins; i++)
        {
            Vector3 randomPos = GetRandomPositionInRadius(Vector3.zero, coinSpawnRadius);
            randomPos.y = 0.3f;
            
            GameObject coin = Instantiate(coinPrefab, randomPos, Quaternion.identity, coinsParent.transform);
            coin.name = $"Coin_{i + 1}";
            
            activeCoins.Add(coin);
        }
        
        Debug.Log($"? Spawned {activeCoins.Count} coins");
    }
    
    public void OnCoinCollected(GameObject coin)
    {
        if (activeCoins.Contains(coin))
        {
            activeCoins.Remove(coin);
            Debug.Log($"?? Coin collected! Remaining coins: {activeCoins.Count}");
        }
    }
    
    // =============================================
    // ITEM COLLECTION & SCORE
    // =============================================
    
    public void OnItemCollected(Item item)
    {
        if (inventoryManager != null)
        {
            inventoryManager.AddItemStackable(item, item.quantity);
        }
        
        int points = item.value * item.quantity;
        score += points;
        
        Debug.Log($"? Collected {item.itemName} x{item.quantity} (+{points} points) | Total Score: {score}");
    }

    public void OpenInventory()
    {
        if (inventoryManager != null)
        {
            inventoryManager.OpenInventory();
        }
    }
    
    public void CloseInventory()
    {
        if (inventoryManager != null)
        {
            inventoryManager.CloseInventory();
        }
    }
    
    // =============================================
    // DEBUG INPUT
    // =============================================
    
    private void HandleDebugInput()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            LogAllLists();
        }
        
        if (Input.GetKeyDown(KeyCode.F2))
        {
            if (inventoryManager != null)
            {
                inventoryManager.DisplayInventory();
            }
        }
        
        if (Input.GetKeyDown(KeyCode.F3))
        {
            Debug.Log($"?? Current Score: {score}");
        }
        
        if (Input.GetKeyDown(KeyCode.F5))
        {
            if (inventoryManager != null)
            {
                inventoryManager.ClearInventory();
            }
        }
        
        if (Input.GetKeyDown(KeyCode.F6))
        {
            if (inventoryManager != null)
            {
                inventoryManager.SortInventory();
            }
        }
    }
    
    private void LogAllLists()
    {
        Debug.Log("\n?????????????????????????????????????????????");
        Debug.Log("?         ALL LISTS STATUS                  ?");
        Debug.Log("?????????????????????????????????????????????");
        
        if (inventoryManager != null)
        {
            int totalValue = inventoryManager.GetTotalValue();
            Debug.Log($"?? Inventory Items: {inventoryManager.GetItemCount()} (Value: {totalValue})");
        }
        
        if (treasureManager != null)
        {
            Debug.Log($"?? Active Treasures: {treasureManager.GetTreasureCount()}");
        }
        
        Debug.Log($"?? Active Trees: {activeTrees.Count}");
        Debug.Log($"?? Active Coins: {activeCoins.Count}");
        Debug.Log($"?? Score: {score}");
        
        Debug.Log("???????????????????????????????????????????\n");
    }
    
    // =============================================
    // UTILITIES
    // =============================================
    
    private Vector3 GetRandomPositionInRadius(Vector3 center, float radius)
    {
        Vector2 randomCircle = Random.insideUnitCircle * radius;
        return center + new Vector3(randomCircle.x, 0f, randomCircle.y);
    }
}

// =============================================
// DATA CLASSES - Shared by all managers
// =============================================

[System.Serializable]
public class Item
{
    public int id;
    public string itemName;
    public int value;
    public int quantity;
    
    public Item(int id, string name, int val, int qty = 1)
    {
        this.id = id;
        this.itemName = name;
        this.value = val;
        this.quantity = qty;
    }
}
