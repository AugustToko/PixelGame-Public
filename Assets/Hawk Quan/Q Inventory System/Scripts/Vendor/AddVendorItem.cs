using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QInventory
{
    [AddComponentMenu("Q Inventory/Add Vendor Item")]
    [RequireComponent(typeof(Vendor))]
    public class AddVendorItem : MonoBehaviour
    {
        public ItemToSell[] itemsToSell;
    }
}

