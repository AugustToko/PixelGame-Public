using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QInventory
{
    [System.Serializable]
    public class PlayerCurrency
    {
        public Currency currency;
        public float amount;
        public Text showText;

        public PlayerCurrency(Currency currency,float amount)
        {
            this.currency = currency;
            this.amount = amount;
        }
    }
}
