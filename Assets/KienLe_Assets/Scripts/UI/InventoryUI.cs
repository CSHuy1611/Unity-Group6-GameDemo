using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject inventoryPanel;
    public GameObject slotPrefab;
    public Transform slotsParent;
    
    [Header("Settings")]
    public int maxSlots = 10;
    
    private List<InventorySlot> slots = new List<InventorySlot>();
    private InventoryManager inventoryManager;
    
    void Start()
    {
        inventoryManager = FindObjectOfType<InventoryManager>();
        CreateSlots();
        
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }
    }
    
    private void CreateSlots()
    {
        if (slotPrefab == null || slotsParent == null)
        {
            Debug.LogError("InventoryUI: slotPrefab or slotsParent is null!");
            return;
        }
        
        for (int i = 0; i < maxSlots; i++)
        {
            GameObject slotObj = Instantiate(slotPrefab, slotsParent);
            InventorySlot slot = slotObj.GetComponent<InventorySlot>();
            
            if (slot != null)
            {
                slots.Add(slot);
            }
        }
        
        Debug.Log($"? Created {slots.Count} inventory slots");
    }
    
    public void ShowInventory()
    {
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(true);
            UpdateDisplay();
            Debug.Log("?? Inventory opened");
        }
    }
    
    public void HideInventory()
    {
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
            Debug.Log("?? Inventory closed");
        }
    }
    
    public void ToggleInventory()
    {
        if (inventoryPanel != null)
        {
            if (inventoryPanel.activeSelf)
                HideInventory();
            else
                ShowInventory();
        }
    }
    
    public void UpdateDisplay()
    {
        if (inventoryManager == null)
        {
            Debug.LogWarning("InventoryUI: InventoryManager not found!");
            return;
        }
        
        List<Item> items = inventoryManager.inventory;
        
        foreach (var slot in slots)
        {
            slot.ClearSlot();
        }
        
        for (int i = 0; i < items.Count && i < maxSlots; i++)
        {
            slots[i].SetItem(items[i]);
        }
    }
}
