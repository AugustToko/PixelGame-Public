using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QInventory
{
    [System.Serializable]
    public class ItemToAdd
    {
        public Item itemsToAdd;
        public int amount;
        public float chance = 1;
    }
}

