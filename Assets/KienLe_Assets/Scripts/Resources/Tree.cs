using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    [Header("Wood Drop")]
    public GameObject woodPrefab;
    [SerializeField] private int woodDropAmount = 1;
    
    private bool isChopped = false;
    
    public void ChopDown()
    {
        if (isChopped) return;
        
        isChopped = true;
        Debug.Log($"?? {gameObject.name} chopped down!");
        
        SpawnWood();
        
        Destroy(gameObject, 0.3f);
    }
    
    private void SpawnWood()
    {
        for (int i = 0; i < woodDropAmount; i++)
        {
            Vector3 dropPos = transform.position + Random.insideUnitSphere * 0.5f;
            dropPos.y = transform.position.y;
            
            GameObject wood = Instantiate(woodPrefab, dropPos, Quaternion.identity);
            wood.name = $"Wood_{i + 1}";
        }
        
        Debug.Log($"?? Dropped {woodDropAmount} wood");
    }
}
