using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [Header("=== INVENTORY LIST ===")]
    public List<Item> inventory = new List<Item>();
    
    private bool inventoryOpen = false;
    private int selectedInventoryIndex = 0;

    public void AddItem(Item item)
    {
        inventory.Add(item);
        Debug.Log($"✅ Added '{item.itemName}' to inventory. Total items: {inventory.Count}");
    }
    
    public void RemoveItem(int index)
    {
        if (index >= 0 && index < inventory.Count)
        {
            Item removedItem = inventory[index];
            inventory.RemoveAt(index);
            Debug.Log($"❌ Removed '{removedItem.itemName}' from inventory. Remaining: {inventory.Count}");
        }
        else
        {
            Debug.LogWarning($"Invalid inventory index: {index}");
        }
    }
    
    public void ClearInventory()
    {
        int count = inventory.Count;
        inventory.Clear();
        Debug.Log($"Cleared inventory! Removed {count} items.");
    }
    
    public void SortInventory()
    {
        inventory.Sort((a, b) => b.value.CompareTo(a.value));
        Debug.Log("📊 Inventory sorted by value (descending)");
        DisplayInventory();
    }
    public Item FindItemByName(string itemName)
    {
        Item found = inventory.Find(item => item.itemName == itemName);
        return found;
    }
    
    public bool HasItem(Item item)
    {
        return inventory.Contains(item);
    }
    
    public int GetItemCount()
    {
        return inventory.Count;
    }
    
    // =============================================
    // UI METHODS
    // =============================================
    
    public void OpenInventory()
    {
        inventoryOpen = true;
        selectedInventoryIndex = 0;
        DisplayInventory();
    }
    
    public void CloseInventory()
    {
        inventoryOpen = false;
    }
    
    public bool IsInventoryOpen()
    {
        return inventoryOpen;
    }
    
    public void DisplayInventory()
    {
        Debug.Log("\n╔════════════════════════════════════╗");
        Debug.Log("║         INVENTORY                  ║");
        Debug.Log("╚════════════════════════════════════╝");
        
        if (inventory.Count == 0)
        {
            Debug.Log("  (Empty)");
        }
        else
        {
            for (int i = 0; i < inventory.Count; i++)
            {
                string cursor = (i == selectedInventoryIndex) ? ">>>" : "   ";
                Item item = inventory[i];
                Debug.Log($"{cursor} [{i}] {item.itemName} (Value: {item.value})");
            }
        }
        
        Debug.Log($"\nTotal items: {inventory.Count}");
        Debug.Log("Controls: W/S = Navigate, R = Remove, E = Close\n");
    }
    
    // =============================================
    // NAVIGATION
    // =============================================
    
    void Update()
    {
        HandleInventoryNavigation();
    }
    
    private void HandleInventoryNavigation()
    {
        if (!inventoryOpen || inventory.Count == 0) return;
        
        // Navigate UP
        if (Input.GetKeyDown(KeyCode.W))
        {
            selectedInventoryIndex--;
            if (selectedInventoryIndex < 0)
                selectedInventoryIndex = inventory.Count - 1;
            DisplayInventory();
        }
        
        // Navigate DOWN
        if (Input.GetKeyDown(KeyCode.S))
        {
            selectedInventoryIndex++;
            if (selectedInventoryIndex >= inventory.Count)
                selectedInventoryIndex = 0;
            DisplayInventory();
        }
        
        // Remove selected item
        if (Input.GetKeyDown(KeyCode.R))
        {
            RemoveItem(selectedInventoryIndex);
            
            // Adjust cursor after removal
            if (selectedInventoryIndex >= inventory.Count && inventory.Count > 0)
            {
                selectedInventoryIndex = inventory.Count - 1;
            }
            
            DisplayInventory();
        }
    }
}
