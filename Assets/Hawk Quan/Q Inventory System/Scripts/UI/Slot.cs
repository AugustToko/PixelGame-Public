using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace QInventory
{
    public class Slot : MonoBehaviour, IDropHandler
    {
        [HideInInspector] public int slotID;
        [HideInInspector] public Q_Inventory inv;

        public void OnDrop(PointerEventData eventData)
        {
            ItemData droppedItem = eventData.pointerDrag.GetComponent<ItemData>();
            if (!droppedItem)
                return;
            if (!droppedItem.inv)
                return;
            if (droppedItem.inv.tag == "Equipment")
                return;
            if ((droppedItem.inv.tag == "Inventory" || inv.tag == "SkillBar"))
            {
                if (droppedItem.inv.ReturnCoolingState(droppedItem.slot))
                {
                    //Q_GameMaster.Instance.inventoryManager.SetInformation(Q_GameMaster.Instance.inventoryManager.infoManager.cantMoveWhenCD);
                    return;
                }
            }

            string tag = eventData.pointerCurrentRaycast.gameObject.tag;
            if (tag == "Storage" || tag == "Inventory" || tag == "SkillBar")
            {
                //Debug.Log(eventData.pointerCurrentRaycast.gameObject.tag);
                if (inv.items[slotID].ID == -1)
                {
                    if (droppedItem.inv == inv)
                    {
                        if (droppedItem.inv.items[droppedItem.slot] != null)
                            droppedItem.inv.items[droppedItem.slot] = Item.CreateInstance<Item>();
                        inv.items[slotID] = droppedItem.item;
                        if (tag == "Inventory" || tag == "SkillBar")
                        {
                            inv.cds[slotID] = inv.cds[droppedItem.slot];
                            inv.cds[droppedItem.slot] = new CoolDown();
                        }

                        droppedItem.slot = slotID;
                    }

                    else
                    {
                        inv.AddItem(droppedItem.item, slotID, droppedItem.amount);

                        droppedItem.ClearItem();
                    }
                }

                else if ((inv.tag == "Inventory" || inv.tag == "SkillBar") && inv.ReturnCoolingState(slotID))
                {
                    return;
                }

                else if ((droppedItem.inv.tag == "Inventory" || droppedItem.inv.tag == "SkillBar") &&
                         droppedItem.inv.ReturnCoolingState(droppedItem.slot))
                {
                    return;
                }

                else
                {
                    Transform item = null;
                    int chileNum = transform.childCount;
                    if (chileNum == 1)
                    {
                        item = transform.GetChild(0);
                    }
                    else if (chileNum == 2)
                    {
                        item = transform.GetChild(1);
                    }

                    ItemData data = null;
                    if (item)
                        data = item.GetComponent<ItemData>();


                    if (droppedItem.inv == inv)
                    {
                        if (!item)
                            return;
                        //Debug.Log("Switch");
                        if (data)
                            data.slot = droppedItem.slot;
                        item.transform.SetParent(data.inv.slots[droppedItem.slot].transform);
                        item.transform.position = data.inv.slots[droppedItem.slot].transform.position;
                        data.inv.items[droppedItem.slot] = data.item;
                        inv.items[slotID] = droppedItem.item;

                        if (inv.tag == "Inventory" || inv.tag == "SkillBar")
                        {
                            inv.cds[droppedItem.slot] = new CoolDown(data.cd, data.item.coolDown);
                            droppedItem.inv.cds[slotID] = new CoolDown(droppedItem.cd, droppedItem.item.coolDown);
                        }

                        droppedItem.slot = slotID;
                        droppedItem.transform.SetParent(transform);
                        droppedItem.transform.position = transform.position;
                        //data.inv = droppedItem.inv;
                        //item.GetComponent<ItemDrag>().inv = droppedItem.inv;

                        //droppedItem.inv = inv;
                        //droppedItem.GetComponent<ItemDrag>().inv = inv;
                    }

                    else
                    {
                        if (data.item == droppedItem.item && data.item.isStackable &&
                            inv.CheckSlot(slotID) > droppedItem.amount)
                        {
                            droppedItem.ClearItem();
                            data.amount += droppedItem.amount;
                            data.UpdataText();
                        }

                        else
                        {
                            data.ClearItem();
                            droppedItem.ClearItem();

                            int temp = data.slot;
                            data.slot = droppedItem.slot;
                            droppedItem.slot = temp;

                            droppedItem.inv.AddItem(data.item, data.slot, data.amount);
                            inv.AddItem(droppedItem.item, droppedItem.slot, droppedItem.amount);
                        }
                    }
                }
            }
        }
    }
}