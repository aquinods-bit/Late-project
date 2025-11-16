using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    // Inventory UI state
    private bool isOpen = false;

    // Reference to inventory UI (assign in Inspector if you have a Canvas/UI Panel)
    // public Transform HUD;
    // List to hold inventory items
    private List<GameObject> items = new List<GameObject>();
    // Maximum number of items allowed
    public int maxItems = 5;

    // Add an item to the inventory
    public void AddItem(GameObject item)
    {
        if (items.Count >= maxItems)
        {
            return;
        }
        if (!items.Contains(item))
        {
            items.Add(item);
            Debug.Log($"[Inventory] Added item: {item.name}");
        }
    }

    // Get all items in inventory
    public List<GameObject> GetItems()
    {
        return items;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
