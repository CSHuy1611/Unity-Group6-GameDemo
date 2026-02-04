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
        RaycastHit2D hit = Physics2D.Raycast(transform.position + Vector3.up * 2f, Vector2.down, 10f);
        if (hit.collider != null)
        {
            transform.position = hit.point + Vector2.up * 0.15f;
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isCollected)
        {
            Collect();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isCollected)
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
