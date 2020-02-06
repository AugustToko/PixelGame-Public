using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace QInventory
{
    public class ItemList : ScriptableObject
    {
        public List<Item> itemList = new List<Item>();
        public List<Currency> currencies = new List<Currency>();
        public List<ItemAttribute> attributes = new List<ItemAttribute>();
        public List<CraftingBluePrint> bluePrints = new List<CraftingBluePrint>();
        public List<Category> categories = new List<Category>();
        public InventorySaveData saveData;

        public Item getItemByID(int id)
        {
            for (int i = 0; i < itemList.Count; i++)
            {
                if (itemList[i].ID == id)
                    return itemList[i];
            }
            return null;
        }

        public Item getItemByName(string name)
        {
            for (int i = 0; i < itemList.Count; i++)
            {
                if (itemList[i].itemName == name)
                    return itemList[i];
            }
            return null;
        }
    }
}
