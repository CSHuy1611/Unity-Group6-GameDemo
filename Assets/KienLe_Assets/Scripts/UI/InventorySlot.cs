using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [Header("UI References")]
    public Image iconImage;
    public Text quantityText;
    public Text nameText;
    
    private Item currentItem;
    
    public void SetItem(Item item)
    {
        currentItem = item;
        
        if (nameText != null)
        {
            nameText.text = item.itemName;
        }
        
        if (quantityText != null)
        {
            quantityText.text = $"x{item.quantity}";
        }
        
        gameObject.SetActive(true);
    }
    
    public void ClearSlot()
    {
        currentItem = null;
        
        if (nameText != null)
        {
            nameText.text = "";
        }
        
        if (quantityText != null)
        {
            quantityText.text = "";
        }
        
        gameObject.SetActive(false);
    }
}
