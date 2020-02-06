using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QInventory
{
    [AddComponentMenu("Q Inventory/Add Item")]
    [RequireComponent(typeof(Q_Inventory))]
    public class AddItem : MonoBehaviour
    {
        public ItemToAdd[] itemsToAdd;
    }
}

