using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QInventory
{
    [System.Serializable]
    public class ConsumableAttribute
    {
        public ItemAttribute consumableItemAttribute;
        public Effect effectType;
        public float value;
    }
}
