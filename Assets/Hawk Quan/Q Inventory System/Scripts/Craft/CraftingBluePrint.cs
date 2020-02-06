using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QInventory
{
    public class CraftingBluePrint : ScriptableObject
    {
        public string bluePrintName;
        public Category category;
        public Sprite icon;
        public Item targetItem;
        public int craftingAmount; //就是合成一次会产出多少个
        public List<Ingredient> ingredients;
        public List<Price> craftingPrices;
        public float successChance = 1;
        public float craftingTime;
        public bool CDAllWhenCrafting = false;
        public bool moveAfterCrafting = false;
    }
}
