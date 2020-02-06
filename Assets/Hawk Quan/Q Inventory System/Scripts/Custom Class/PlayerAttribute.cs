using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QInventory
{
    [System.Serializable]
    public class PlayerAttribute
    {
        public ItemAttribute playerAttribute;
        public float currentValue = 100f;
        public float minValue = 0f;
        public float maxValue = 100f;
        public PlayerAttribute(ItemAttribute playerAttribute,float currentValue,float minValue,float maxValue)
        {
            this.playerAttribute = playerAttribute;
            this.currentValue = currentValue;
            this.minValue = minValue;
            this.maxValue = maxValue;
        }
    }
}
