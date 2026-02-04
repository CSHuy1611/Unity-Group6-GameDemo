using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [Header("Treasure Prefabs")]
    public GameObject diamondPrefab;
    public GameObject coinPrefab;
    
    [Header("Spawn Settings")]
    [SerializeField] private int minTreasures = 2;
    [SerializeField] private int maxTreasures = 5;
    [SerializeField] private float spawnRadius = 2f;
    
    private TreasureManager treasureManager;
    private bool isOpened = false;
    
    public void Initialize(TreasureManager manager)
    {
        treasureManager = manager;
    }
    
    public void Collect()
    {
        if (isOpened) return;
        
        isOpened = true;
        Debug.Log($"?? {gameObject.name} opened!");
        
        SpawnTreasures();
        
        Destroy(gameObject, 0.5f);
    }
    
    private void SpawnTreasures()
    {
        int treasureCount = Random.Range(minTreasures, maxTreasures + 1);
        
        for (int i = 0; i < treasureCount; i++)
        {
            bool isDiamond = Random.value > 0.5f;
            GameObject prefabToSpawn = isDiamond ? diamondPrefab : coinPrefab;
            
            if (prefabToSpawn == null)
            {
                Debug.LogWarning($"Chest: {(isDiamond ? "Diamond" : "Coin")} prefab is null!");
                continue;
            }
            
            Vector3 spawnPos = transform.position + Random.insideUnitSphere * spawnRadius;
            spawnPos.y = 0.3f;
            
            GameObject treasure = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
            treasure.name = $"{(isDiamond ? "Diamond" : "Coin")}_{i + 1}";
            
            if (treasureManager != null)
            {
                treasureManager.OnTreasureSpawned(treasure);
            }
        }
        
        Debug.Log($"? Spawned {treasureCount} treasures from chest");
    }
}
