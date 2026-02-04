using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("MANAGERS")]
    public InventoryManager inventoryManager;
    public TreasureManager treasureManager;
    
    [Header("SCORE")]
    public int score = 0;
    
    [Header("UI")]
    public UIManager uiManager;
    
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
        UpdateScoreUI();
        LogAllLists();
    }
    
    private void SetupManagers()
    {
        if (inventoryManager == null)
        {
            return;
        }
        
        if (treasureManager == null)
        {
            return;
        }
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
    
    public void OnItemCollected(Item item)
    {
        if (inventoryManager != null)
        {
            inventoryManager.AddItemStackable(item, item.quantity);
        }
        
        int points = item.value * item.quantity;
        score += points;
        
        UpdateScoreUI();
        
        Debug.Log($"? Collected {item.itemName} x{item.quantity} (+{points} points) | Total Score: {score}");
    }
    
    private void UpdateScoreUI()
    {
        if (uiManager != null)
        {
            uiManager.UpdateScore(score);
        }
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
    
    // DEBUG INPUT
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
        if (inventoryManager != null)
        {
            int totalValue = inventoryManager.GetTotalValue();
            Debug.Log($"?? Inventory Items: {inventoryManager.GetItemCount()} (Value: {totalValue})");
        }
        
        if (treasureManager != null)
        {
            Debug.Log($"?? Active Treasures: {treasureManager.GetTreasureCount()}");
        }
        
        Debug.Log($"?? Score: {score}");
    }
    
    // UTILITIES
    private Vector3 GetRandomPositionInRadius(Vector3 center, float radius)
    {
        Vector2 randomCircle = Random.insideUnitCircle * radius;
        return center + new Vector3(randomCircle.x, 0f, randomCircle.y);
    }
}

// DATA CLASSES - Shared by all managers
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
