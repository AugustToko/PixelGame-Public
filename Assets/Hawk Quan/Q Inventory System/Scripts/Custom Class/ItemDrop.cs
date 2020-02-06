using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QInventory
{
    [System.Serializable]
    public class ItemDrop
    {
        public Item itemToDrop = null;
        public int maxDropAmount = 0;
        public int minDropAmount = 0;
        public float dropChance = 0;
    }
}

