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
        Debug.Log($"✅ Added '{item.itemName}' x{item.quantity} to inventory. Total items: {inventory.Count}");
    }
    
    public void AddItemStackable(Item newItem, int quantity = 1)
    {
        Item existingItem = inventory.Find(item => item.id == newItem.id);
        
        if (existingItem != null)
        {
            existingItem.quantity += quantity;
            Debug.Log($"✅ Stacked '{newItem.itemName}' +{quantity}. Now: x{existingItem.quantity}");
        }
        else
        {
            newItem.quantity = quantity;
            inventory.Add(newItem);
            Debug.Log($"✅ Added new '{newItem.itemName}' x{quantity} to inventory");
        }
    }
    
    public void RemoveItem(int index)
    {
        if (index >= 0 && index < inventory.Count)
        {
            Item removedItem = inventory[index];
            
            if (removedItem.quantity > 1)
            {
                removedItem.quantity--;
                Debug.Log($"🔽 Decreased '{removedItem.itemName}' quantity. Now: x{removedItem.quantity}");
            }
            else
            {
                inventory.RemoveAt(index);
                Debug.Log($"❌ Removed '{removedItem.itemName}' from inventory. Remaining: {inventory.Count}");
            }
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
    
    public int GetTotalValue()
    {
        int total = 0;
        foreach (Item item in inventory)
        {
            total += item.value * item.quantity;
        }
        return total;
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
                int totalValue = item.value * item.quantity;
                Debug.Log($"{cursor} [{i}] {item.itemName} x{item.quantity} (Value: {totalValue})");
            }
        }
        
        Debug.Log($"\nTotal items: {inventory.Count}");
        Debug.Log($"Total value: {GetTotalValue()}");
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
