using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureManager : MonoBehaviour
{

    [Header("=== TREASURES LIST ===")]
    public List<GameObject> activeTreasures = new List<GameObject>();
    
    [Header("=== PREFABS ===")]
    public GameObject chestPrefab;
    public GameObject treasurePrefab;
    
    [Header("=== SPAWN SETTINGS ===")]
    [SerializeField] private int numberOfChests = 3;
    [SerializeField] private float chestSpawnRadius = 15f;
    [SerializeField] private float treasureSpawnRadius = 5f;
    [SerializeField] private int treasuresPerChest = 4;
    
    private GameObject chestsParent;
    private GameObject treasuresParent;
    private InventoryManager inventoryManager;

    public void Initialize(InventoryManager invManager)
    {
        inventoryManager = invManager;
        CreateParentObjects();
        SpawnChests();
    }
    
    private void CreateParentObjects()
    {
        chestsParent = new GameObject("=== CHESTS ===");
        treasuresParent = new GameObject("=== TREASURES ===");
    }

    // CHEST SYSTEM 
    private void SpawnChests()
    {
        if (chestPrefab == null)
        {
            Debug.LogError("❌ TreasureManager: Chest Prefab chưa được gán! Vui lòng kéo vào Inspector.");
            return;
        }
        
        for (int i = 0; i < numberOfChests; i++)
        {
            Vector3 randomPos = GetRandomPositionInRadius(Vector3.zero, chestSpawnRadius);
            randomPos.y = 0.5f;
            
            GameObject chest = Instantiate(chestPrefab, randomPos, Quaternion.identity, chestsParent.transform);
            chest.name = $"Chest_{i + 1}";

            Chest chestScript = chest.GetComponent<Chest>();
            if (chestScript != null)
            {
                chestScript.Initialize(this);
            }
        }
        
        Debug.Log($"✅ Spawned {numberOfChests} chests");
    }
    
    public void OnChestCollected(Vector3 chestPosition)
    {
        Debug.Log($"Chest collected at {chestPosition}! Spawning treasures...");
        SpawnTreasuresAroundPosition(chestPosition, treasuresPerChest);
    }
    
    // TREASURE SYSTEM
    private void SpawnTreasuresAroundPosition(Vector3 center, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 randomOffset = GetRandomPositionInRadius(center, treasureSpawnRadius);
            randomOffset.y = 0.3f;
            
            GameObject treasure = Instantiate(treasurePrefab, randomOffset, Quaternion.identity, treasuresParent.transform);
            treasure.name = $"Treasure_{activeTreasures.Count + 1}";
            
            activeTreasures.Add(treasure);
        }
        
        Debug.Log($"✅ Spawned {count} treasures. Total treasures: {activeTreasures.Count}");
    }
    
    public void OnTreasurePickedUp(GameObject treasure)
    {
        if (activeTreasures.Contains(treasure))
        {
            activeTreasures.Remove(treasure);
            Debug.Log($"Treasure picked up! Remaining treasures: {activeTreasures.Count}");
            
            Item randomItem = GenerateRandomItem();
            if (inventoryManager != null)
            {
                inventoryManager.AddItemStackable(randomItem, randomItem.quantity);
            }
            
            Destroy(treasure);
        }
    }
    
    // UTILITIES
    public GameObject FindNearestTreasure(Vector3 position, float maxRange)
    {
        if (activeTreasures.Count == 0) return null;
        
        GameObject nearest = null;
        float nearestDistance = maxRange;
        
        foreach (GameObject treasure in activeTreasures)
        {
            if (treasure == null) continue;
            
            float distance = Vector3.Distance(position, treasure.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = treasure;
            }
        }
        
        return nearest;
    }
    
    public List<GameObject> FindTreasuresInRange(Vector3 position, float range)
    {
        List<GameObject> treasuresInRange = activeTreasures.FindAll(treasure =>
        {
            if (treasure == null) return false;
            float distance = Vector3.Distance(position, treasure.transform.position);
            return distance <= range;
        });
        
        return treasuresInRange;
    }
    
    public int GetTreasureCount()
    {
        return activeTreasures.Count;
    }
    
    public void OnTreasureSpawned(GameObject treasure)
    {
        activeTreasures.Add(treasure);
        Debug.Log($"💎 Treasure spawned from chest. Total: {activeTreasures.Count}");
    }
    
    private Vector3 GetRandomPositionInRadius(Vector3 center, float radius)
    {
        Vector2 randomCircle = Random.insideUnitCircle * radius;
        return center + new Vector3(randomCircle.x, 0f, randomCircle.y);
    }
    
    private Item GenerateRandomItem()
    {
        int randomType = Random.Range(0, 2);
        
        if (randomType == 0)
        {
            return new Item(2, "Diamond", 10, 1);
        }
        else
        {
            return new Item(3, "Coin", 5, 1);
        }
    }
}
