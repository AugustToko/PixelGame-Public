using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QInventory;

public class Q_GameMaster : MonoBehaviour
{
    public static Q_GameMaster Instance;

    [Header("Inventory")]
    public InventoryManager inventoryManager;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
}
