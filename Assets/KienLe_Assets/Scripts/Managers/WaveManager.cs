using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WaveManager : MonoBehaviour
{ 
    [Header("=== LISTS DEMO ===")]
    public List<GameObject> activeEnemies = new List<GameObject>();
    public List<WaveData> waves = new List<WaveData>();
    
    [Header("=== ENEMY PREFABS ===")]
    public GameObject enemyType1Prefab;
    public GameObject enemyType2Prefab;
    public GameObject enemyType3Prefab;
    
    [Header("=== WAVE STATUS ===")]
    [SerializeField] private int currentWaveIndex = 0;
    [SerializeField] private bool waveInProgress = false;
    
    private GameObject enemiesParent;
    
    public void Initialize()
    {
        CreateParentObjects();
        InitializeWaves();
        StartWave(0);
    }
    
    private void CreateParentObjects()
    {
        enemiesParent = new GameObject("=== ENEMIES ===");
    }
    
    private void InitializeWaves()
    {
        waves.Clear();
        
        WaveData wave1 = new WaveData
        {
            waveNumber = 1,
            enemySpawns = new List<EnemySpawnInfo>
            {
                new EnemySpawnInfo { enemyType = "Type1", spawnPosition = new Vector3(10f, 0.5f, 10f) }
            }
        };
        waves.Add(wave1);
        
        WaveData wave2 = new WaveData
        {
            waveNumber = 2,
            enemySpawns = new List<EnemySpawnInfo>
            {
                new EnemySpawnInfo { enemyType = "Type1", spawnPosition = new Vector3(8f, 0.5f, 8f) },
                new EnemySpawnInfo { enemyType = "Type2", spawnPosition = new Vector3(12f, 0.5f, 8f) }
            }
        };
        waves.Add(wave2);
        
        WaveData wave3 = new WaveData
        {
            waveNumber = 3,
            enemySpawns = new List<EnemySpawnInfo>
            {
                new EnemySpawnInfo { enemyType = "Type1", spawnPosition = new Vector3(8f, 0.5f, 12f) },
                new EnemySpawnInfo { enemyType = "Type2", spawnPosition = new Vector3(10f, 0.5f, 12f) },
                new EnemySpawnInfo { enemyType = "Type3", spawnPosition = new Vector3(12f, 0.5f, 12f) }
            }
        };
        waves.Add(wave3);
        
        Debug.Log($"✅ Initialized {waves.Count} waves");
    }
    
    // =============================================
    // WAVE SYSTEM
    // =============================================
    
    void Update()
    {
        CheckWaveCompletion();
    }
    
    public void StartWave(int waveIndex)
    {
        if (waveIndex >= waves.Count)
        {
            Debug.Log("🎉 ALL WAVES COMPLETED! YOU WIN!");
            return;
        }
        
        currentWaveIndex = waveIndex;
        WaveData wave = waves[waveIndex];
        waveInProgress = true;
        
        Debug.Log($"\n🌊 === WAVE {wave.waveNumber} START === 🌊");
        Debug.Log($"Enemies to spawn: {wave.enemySpawns.Count}");
        
        SpawnWaveEnemies(wave);
    }
    
    private void SpawnWaveEnemies(WaveData wave)
    {
        foreach (var spawnInfo in wave.enemySpawns)
        {
            GameObject enemyPrefab = GetEnemyPrefab(spawnInfo.enemyType);
            
            if (enemyPrefab != null)
            {
                GameObject enemy = Instantiate(enemyPrefab, spawnInfo.spawnPosition, Quaternion.identity, enemiesParent.transform);
                enemy.name = $"{spawnInfo.enemyType}_{activeEnemies.Count + 1}";
                
                activeEnemies.Add(enemy);
                
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                if (enemyScript != null)
                {
                    enemyScript.Initialize(this);
                }
                
                Debug.Log($"✅ Spawned {spawnInfo.enemyType} at {spawnInfo.spawnPosition}");
            }
        }
        
        Debug.Log($"Total active enemies: {activeEnemies.Count}");
    }
    
    private GameObject GetEnemyPrefab(string enemyType)
    {
        switch (enemyType)
        {
            case "Type1": return enemyType1Prefab;
            case "Type2": return enemyType2Prefab;
            case "Type3": return enemyType3Prefab;
            default:
                Debug.LogError($"Unknown enemy type: {enemyType}");
                return null;
        }
    }
    
    public void OnEnemyDied(GameObject enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
            Debug.Log($"❌ Enemy died! Remaining: {activeEnemies.Count}");
            
            Destroy(enemy, 0.5f);
        }
    }
    
    private void CheckWaveCompletion()
    {
        if (waveInProgress && activeEnemies.Count == 0)
        {
            waveInProgress = false;
            Debug.Log($"✅ Wave {currentWaveIndex + 1} completed!");
            
            StartCoroutine(StartNextWaveDelayed(3f));
        }
    }
    
    private IEnumerator StartNextWaveDelayed(float delay)
    {
        Debug.Log($"Next wave in {delay} seconds...");
        yield return new WaitForSeconds(delay);
        StartWave(currentWaveIndex + 1);
    }
    
    // =============================================
    // ENEMY UTILITIES
    // =============================================
    
    public GameObject FindNearestEnemy(Vector3 position, float maxRange)
    {
        if (activeEnemies.Count == 0) return null;
        
        GameObject nearest = null;
        float nearestDistance = maxRange;
        
        foreach (GameObject enemy in activeEnemies)
        {
            if (enemy == null) continue;
            
            float distance = Vector3.Distance(position, enemy.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = enemy;
            }
        }
        
        return nearest;
    }
    
    public List<GameObject> FindAliveEnemies()
    {
        List<GameObject> aliveEnemies = activeEnemies.FindAll(enemy =>
        {
            if (enemy == null) return false;
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            return enemyScript != null && enemyScript.currentHealth > 0;
        });
        
        return aliveEnemies;
    }
    
    public void KillAllEnemies()
    {
        int count = activeEnemies.Count;
        
        List<GameObject> enemiesCopy = new List<GameObject>(activeEnemies);
        
        foreach (GameObject enemy in enemiesCopy)
        {
            if (enemy != null)
            {
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                if (enemyScript != null)
                {
                    enemyScript.Die();
                }
            }
        }
        
        Debug.Log($"💀 Killed all {count} enemies!");
    }
    
    public int GetEnemyCount()
    {
        return activeEnemies.Count;
    }
    
    public int GetCurrentWave()
    {
        return currentWaveIndex + 1;
    }
    
    public int GetTotalWaves()
    {
        return waves.Count;
    }
    
    // =============================================
    // DEBUG
    // =============================================
    
    public void LogEnemies()
    {
        Debug.Log("\n=== ACTIVE ENEMIES ===");
        if (activeEnemies.Count == 0)
        {
            Debug.Log("  (No enemies)");
        }
        else
        {
            for (int i = 0; i < activeEnemies.Count; i++)
            {
                GameObject enemy = activeEnemies[i];
                if (enemy != null)
                {
                    Enemy enemyScript = enemy.GetComponent<Enemy>();
                    string info = enemyScript != null ? $"HP: {enemyScript.currentHealth}" : "No script";
                    Debug.Log($"  [{i}] {enemy.name} - {info}");
                }
            }
        }
        Debug.Log($"Total: {activeEnemies.Count}\n");
    }
}
