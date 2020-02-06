using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QInventory
{
    public class ItemSorting : MonoBehaviour
    {
        public void SortItem()
        {
            List<ItemToSort> itemsToSort = new List<ItemToSort>();
            Q_Inventory inv = GetComponent<Q_Inventory>();

            if (inv.tag == "Inventory")
                for (int i = 0; i < inv.slots.Count; i++)
                {
                    if (inv.ReturnCoolingState(i))
                        return;
                }

            foreach (var slot in inv.slots)
            {
                if (slot.transform.childCount > 0)
                {
                    ItemData data = slot.transform.GetChild(0).GetComponent<ItemData>();
                    itemsToSort.Add(new ItemToSort(data.item, data.amount));
                    data.ClearItem();
                }
            }

            List<Variety> itemSortingOrders = Q_GameMaster.Instance.inventoryManager.itemSortingManager.itemSortingOrders;
            foreach (var sortType in itemSortingOrders)
            {
                foreach (var itemToSort in itemsToSort)
                {
                    if (itemToSort.item.variety == sortType)
                    {
                        StartCoroutine(ItemAdd(itemToSort.item.ID, itemToSort.amount));
                    }
                }
            }
        }

        IEnumerator ItemAdd(int id, int amount)
        {
            yield return 0;
            Q_Inventory inv = GetComponent<Q_Inventory>();
            inv.AddItem(id, amount);
        }

        [System.Serializable]
        class ItemToSort
        {
            public Item item;
            public int amount;

            public ItemToSort(Item item, int amount)
            {
                this.item = item;
                this.amount = amount;
            }
        }
    }
}
