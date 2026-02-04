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
        
        SnapToGround();
    }
    
    private void SnapToGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 2f, Vector3.down, out hit, 10f))
        {
            transform.position = hit.point + Vector3.up * 0.15f;
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
        }
        
        Destroy(gameObject);
    }
}
