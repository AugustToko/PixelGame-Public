using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QInventory
{
    [System.Serializable]
    public class ItemToSell
    {
        public Item itemToSell;
        public bool moveAfterPurchase = false;
        public bool useDefaultPrice = true;
        [HideInInspector]
        public bool isMoved = false;
        public List<Price> prices;
    }
}
