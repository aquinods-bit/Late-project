using UnityEngine;

public class InventoryItem : MonoBehaviour, IInteractable
{
    public bool Interact()
    {
        Inventory playerInventory = FindObjectOfType<Inventory>();
        if (playerInventory != null)
        {
            int beforeCount = playerInventory.GetItems().Count;
            playerInventory.AddItem(gameObject);
            int afterCount = playerInventory.GetItems().Count;
            if (afterCount > beforeCount)
            {
                gameObject.SetActive(false);
                return true;
            }
        }
        return false;
    }
}