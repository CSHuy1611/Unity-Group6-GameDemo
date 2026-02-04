using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    [Header("Wood Drop")]
    public GameObject woodPrefab;
    [SerializeField] private int woodDropAmount = 1;
    
    private GameManager gameManager;
    private bool isChopped = false;
    
    public void Initialize(GameManager manager)
    {
        gameManager = manager;
    }
    
    public void ChopDown()
    {
        if (isChopped) return;
        
        isChopped = true;
        Debug.Log($"?? {gameObject.name} chopped down!");
        
        SpawnWood();
        
        if (gameManager != null)
        {
            gameManager.OnTreeDestroyed(gameObject, transform.position);
        }
    }
    
    private void SpawnWood()
    {
        for (int i = 0; i < woodDropAmount; i++)
        {
            Vector3 dropPos = transform.position + Random.insideUnitSphere * 0.5f;
            dropPos.y = 0.3f;
            
            GameObject wood = Instantiate(woodPrefab, dropPos, Quaternion.identity);
            wood.name = $"Wood_{i + 1}";
        }
        
        Debug.Log($"?? Dropped {woodDropAmount} wood");
    }
}
