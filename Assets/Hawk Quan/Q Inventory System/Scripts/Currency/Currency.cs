using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QInventory
{
    public class Currency : ScriptableObject
    {
        public string currencyName;
        public Sprite icon;
        public float exchangeRate;
    }
}
