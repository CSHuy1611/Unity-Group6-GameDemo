using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    [Header("Item Data")]
    public int itemID;
    public string itemName;
    public int itemValue;
    public int itemQuantity = 1;
    
    private GameManager gameManager;
    private bool isCollected = false;
    
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("CollectibleItem: GameManager not found!");
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isCollected)
        {
            Collect();
        }
    }
    
    private void Collect()
    {
        isCollected = true;
        
        Item item = new Item(itemID, itemName, itemValue, itemQuantity);
        
        if (gameManager != null)
        {
            gameManager.OnItemCollected(item);
            
            if (itemName == "Coin")
            {
                gameManager.OnCoinCollected(gameObject);
            }
        }
        
        Destroy(gameObject);
    }
}
