using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameManager : MonoBehaviour
{
    [Header("=== MANAGERS ===")]
    public InventoryManager inventoryManager;
    public TreasureManager treasureManager;
    public WaveManager waveManager;
    
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
        Debug.Log("?????????????????????????????????????????????");
        Debug.Log("?      GAME MANAGER INITIALIZATION          ?");
        Debug.Log("?????????????????????????????????????????????");
        SetupManagers();
        InitializeManagers();
      
        Debug.Log("? Game initialized successfully!");
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
        
        if (waveManager == null)
        {
            GameObject waveObj = new GameObject("WaveManager");
            waveObj.transform.SetParent(transform);
            waveManager = waveObj.AddComponent<WaveManager>();
        }
    }
    
    private void InitializeManagers()
    {
        if (treasureManager != null)
        {
            treasureManager.Initialize(inventoryManager);
            Debug.Log("? TreasureManager initialized");
        }
        
        if (waveManager != null)
        {
            waveManager.Initialize();
            Debug.Log("? WaveManager initialized");
        }
        
        Debug.Log("? InventoryManager initialized");
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
    
    public GameObject FindNearestEnemy(Vector3 position, float range)
    {
        if (waveManager != null)
        {
            return waveManager.FindNearestEnemy(position, range);
        }
        return null;
    }
    
    // =============================================
    // DEBUG INPUT
    // =============================================
    
    private void HandleDebugInput()
    {
        // F1 - Log all lists
        if (Input.GetKeyDown(KeyCode.F1))
        {
            LogAllLists();
        }
        
        // F2 - Log inventory details
        if (Input.GetKeyDown(KeyCode.F2))
        {
            if (inventoryManager != null)
            {
                inventoryManager.DisplayInventory();
            }
        }
        
        // F3 - Log enemies details
        if (Input.GetKeyDown(KeyCode.F3))
        {
            if (waveManager != null)
            {
                waveManager.LogEnemies();
            }
        }
        
        // F5 - Clear inventory
        if (Input.GetKeyDown(KeyCode.F5))
        {
            if (inventoryManager != null)
            {
                inventoryManager.ClearInventory();
            }
        }
        
        // F6 - Sort inventory
        if (Input.GetKeyDown(KeyCode.F6))
        {
            if (inventoryManager != null)
            {
                inventoryManager.SortInventory();
            }
        }
        
        // F7 - Kill all enemies
        if (Input.GetKeyDown(KeyCode.F7))
        {
            if (waveManager != null)
            {
                waveManager.KillAllEnemies();
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
            Debug.Log($"?? Inventory Items: {inventoryManager.GetItemCount()}");
        }
        
        if (treasureManager != null)
        {
            Debug.Log($"?? Active Treasures: {treasureManager.GetTreasureCount()}");
        }
        
        if (waveManager != null)
        {
            Debug.Log($"?? Active Enemies: {waveManager.GetEnemyCount()}");
            Debug.Log($"?? Current Wave: {waveManager.GetCurrentWave()}/{waveManager.GetTotalWaves()}");
        }
        
        Debug.Log("???????????????????????????????????????????\n");
    }
}


[System.Serializable]
public class Item
{
    public int id;
    public string itemName;
    public string description;
    public int value;
    
    public Item(int id, string name, string desc, int val)
    {
        this.id = id;
        this.itemName = name;
        this.description = desc;
        this.value = val;
    }
}

[System.Serializable]
public class WaveData
{
    public int waveNumber;
    public List<EnemySpawnInfo> enemySpawns = new List<EnemySpawnInfo>();
}


[System.Serializable]
public class EnemySpawnInfo
{
    public string enemyType;
    public Vector3 spawnPosition;
}
