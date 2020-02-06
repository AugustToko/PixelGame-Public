using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QInventory
{
    public class InventorySaveData : ScriptableObject
    {
        public List<ItemToSave> playerInventoryItems = new List<ItemToSave>();
        public List<CurrencyToSave> playerCurrencies = new List<CurrencyToSave>();
        public List<PlayerAttribute> playerAttributes = new List<PlayerAttribute>();
        public List<int> onEquipmentItems = new List<int>();
    }

    [System.Serializable]
    public class ItemToSave
    {
        public Item item;
        public int amount;
        public ItemToSave(Item item, int amount)
        {
            this.item = item;
            this.amount = amount;
        }
    }

    [System.Serializable]
    public class CurrencyToSave
    {
        public Currency currency;
        public float amount;
        public CurrencyToSave(Currency currency, float amount)
        {
            this.currency = currency;
            this.amount = amount;
        }
    }
}
