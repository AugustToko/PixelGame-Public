using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QInventory
{
    [AddComponentMenu("Q Inventory/Q_Inventory")]
    public class Q_Inventory : MonoBehaviour
    {

        [Header("Inventory UI")]
        private GameObject m_InventoryPanel;
        public int slotAmount = 18;


        public List<Item> items = new List<Item>();
        [HideInInspector]
        public List<GameObject> slots = new List<GameObject>();
        [HideInInspector]
        public List<CoolDown> cds = new List<CoolDown>();

        protected GameObject m_GridList;
        private GameObject m_InventorySlot;
        private GameObject m_InventoryItem;

        protected virtual void Start()
        {
            //--------------------分割线----------------------
            //初始化赋值各种参数
            m_InventoryPanel = transform.GetChild(0).gameObject;
            m_InventorySlot = Q_GameMaster.Instance.inventoryManager.m_InventorySlot;
            m_InventoryItem = Q_GameMaster.Instance.inventoryManager.m_InventoryItem;

            //--------------------分割线----------------------
            //初始化添加格子
            Transform scrollRect = m_InventoryPanel.transform.Find("Scroll Rect");
            m_GridList = scrollRect.Find("GridList").gameObject;
            for (int i = 0; i < slotAmount; i++)
            {
                items.Add(Item.CreateInstance<Item>());
                GameObject newSlot = Instantiate(m_InventorySlot, m_GridList.transform);
                Slot m_slot = newSlot.GetComponent<Slot>();
                m_slot.slotID = i;
                m_slot.inv = this;
                slots.Add(newSlot);
                cds.Add(new CoolDown());
            }

            //添加物品
            AddItemAtStart();

            StartCoroutine(SetFalse());
        }

        protected void FixedUpdate()
        {
            CDsCoolDown();
            //ResetScrollBar(m_InventoryPanel,m_Scrollbar);
        }

        public bool CheckIsFull(int id)
        {
            Item newItem = Q_GameMaster.Instance.inventoryManager.itemDataBase.getItemByID(id);
            int temp = CheckItem(id);
            if (newItem.isStackable && temp != -1)
            {
                return false;
            }

            else
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].ID == -1)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        //--------------------分割线----------------------
        //这个方法是最基本的利用物品id添加物品的
        public virtual ItemData AddItem(int id)
        {
            Item newItem = Q_GameMaster.Instance.inventoryManager.itemDataBase.getItemByID(id);
            Debug.Log("get " + newItem.itemName);
            int temp = CheckItem(id);
            if (newItem.isStackable && temp != -1)//如果item是可以重叠并且背包中已经有该物品了
            {
                ItemData data = slots[temp].transform.GetChild(0).GetComponent<ItemData>();
                data.amount++;
                //Debug.Log("slot " + temp + "amount " + data.amount);
                data.UpdataText();

                return data;
            }

            else
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].ID == -1)
                    {
                        items[i] = newItem;
                        GameObject itemObj = Instantiate(m_InventoryItem, slots[i].transform);
                        itemObj.name = newItem.itemName;

                        itemObj.GetComponent<ItemDrag>().inv = this;

                        ItemData data = itemObj.GetComponent<ItemData>();//更新数据

                        //更新所在背包状态
                        if (tag == "Storage")//更改物品状态
                        {
                            data.isStorged = true;
                        }

                        else if (tag == "Vendor")
                        {
                            data.isOnSell = true;
                        }

                        else
                        {
                            data.isStorged = false;
                            data.isOnSell = false;
                        }

                        data.inv = this;

                        //change icon
                        data.icon = itemObj.transform.Find("Icon").GetComponent<Image>();
                        data.icon.sprite = newItem.icon;

                        data.amount = 1;
                        data.item = newItem;
                        data.slot = i;
                        Text numText = itemObj.transform.Find("Num").GetComponent<Text>();

                        //如果是可以消耗的，开启对应子物体
                        if (newItem.variety == Variety.Consumable)
                        {
                            data.amountText = numText;

                            //CD的设置
                            data.cd = itemObj.transform.Find("CD").GetComponent<Image>();
                            if (data.item.coolDown > 0)
                                data.cd.gameObject.SetActive(true);
                            cds[data.slot] = new CoolDown(data.cd, data.item.coolDown);
                        }


                        if (newItem.isStackable)
                        {
                            numText.text = data.amount.ToString();
                        }

                        else//如果不可重叠就关闭num text
                            numText.gameObject.SetActive(false);

                        if (newItem.useOnPickUp)
                        {
                            data.UseItem();
                        }

                        return data;
                    }
                }

                return null;
            }
        }

        public virtual ItemData AddItem(int id, int amount)
        {

            Item newItem = Q_GameMaster.Instance.inventoryManager.itemDataBase.getItemByID(id);
            int amountLeft;
            int amountToAdd = amount;
            int temp = CheckItem(id);
            Debug.Log(temp);
            if (newItem.isStackable && temp != -1)//如果item是可以重叠并且背包中已经有该物品了
            {
                ItemData data = slots[temp].transform.GetChild(0).GetComponent<ItemData>();
                amountLeft = CheckSlot(temp);
                if (amountLeft >= amountToAdd)
                {
                    data.amount += amountToAdd;
                    data.UpdataText();
                }

                else
                {
                    data.amount += amountLeft;
                    data.UpdataText();
                    amountToAdd -= amountLeft;
                    AddItem(id, amountToAdd);
                }

                return data;
            }

            else
            {
                return AddItemAtNewSlot(newItem, amountToAdd);
            }
        }

        public virtual ItemData AddItem(Item newItem, int slotID, int amount)
        {
            Debug.Log("get " + newItem.itemName);

            if (items[slotID].ID == -1)
            {
                items[slotID] = newItem;
                GameObject itemObj = Instantiate(m_InventoryItem, slots[slotID].transform);
                itemObj.name = newItem.itemName;

                itemObj.GetComponent<ItemDrag>().inv = this;

                ItemData data = itemObj.GetComponent<ItemData>();//更新数据

                //更新所在背包状态
                if (tag == "Storage")//更改物品状态
                {
                    data.isStorged = true;
                }

                else if (tag == "Vendor")
                {
                    data.isOnSell = true;
                }

                else
                {
                    data.isStorged = false;
                    data.isOnSell = false;
                }

                data.inv = this;

                //change icon
                data.icon = itemObj.transform.Find("Icon").GetComponent<Image>();
                data.icon.sprite = newItem.icon;

                data.amount = amount;
                data.item = newItem;
                data.slot = slotID;
                Text numText = itemObj.transform.Find("Num").GetComponent<Text>();

                //如果是可以消耗的，开启对应子物体
                if (newItem.variety == Variety.Consumable)
                {
                    data.amountText = numText;

                    //CD的设置
                    data.cd = itemObj.transform.Find("CD").GetComponent<Image>();
                    if (data.item.coolDown > 0)
                        data.cd.gameObject.SetActive(true);
                    cds[data.slot] = new CoolDown(data.cd, data.item.coolDown);
                }


                if (newItem.isStackable)
                {
                    numText.text = data.amount.ToString();
                }

                else//如果不可重叠就关闭num text
                    numText.gameObject.SetActive(false);


                if (newItem.useOnPickUp)
                {
                    for (int i = 0; i < amount; i++)
                    {
                        data.UseItem();
                    }
                }

                return data;
            }

            else
            {
                Debug.Log("the slot has something");
            }
            return null;
        }

        public virtual int CheckSlot(int slotID)
        {
            ItemData data = slots[slotID].transform.GetChild(0).GetComponent<ItemData>();
            if (data.item.maxStackNumber == data.amount)
            {
                //Debug.Log("This Slot is Full " + i);
                return 0;
            }

            else
            {
                return data.item.maxStackNumber - data.amount;
            }
        }

        public bool ReturnCoolingState(int slotID)
        {
            return cds[slotID].isCooling;
        }

        public void StartCD(int slotID, float coolTime)
        {
            //Debug.Log(slotID);
            if (!cds[slotID].cd)
                return;
            cds[slotID].isCooling = true;
            cds[slotID].cd.fillAmount = 1;
            cds[slotID].coolTime = coolTime;
        }

        public void ClearAllItems()
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].ID != -1)
                {
                    ItemData data = slots[i].GetComponentInChildren<ItemData>();
                    data.ClearItem();
                }
            }
        }

        public void AllStartCD(float coolTime)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].ID != -1)
                {
                    if (!ReturnCoolingState(i))
                    {
                        StartCD(i, coolTime);
                    }
                }
            }

            if (this.tag == "Inventory")
            {
                for (int i = 0; i < Q_GameMaster.Instance.inventoryManager.skillBar.items.Count; i++)
                {
                    if (Q_GameMaster.Instance.inventoryManager.skillBar.items[i].ID != -1)
                    {
                        if (!Q_GameMaster.Instance.inventoryManager.skillBar.ReturnCoolingState(i))
                        {
                            Q_GameMaster.Instance.inventoryManager.skillBar.StartCD(i, coolTime);
                        }
                    }
                }
            }
            else if (this.tag == "SkillBar")
            {
                for (int i = 0; i < Q_GameMaster.Instance.inventoryManager.playerInventory.items.Count; i++)
                {
                    if (Q_GameMaster.Instance.inventoryManager.playerInventory.items[i].ID != -1)
                    {
                        if (!Q_GameMaster.Instance.inventoryManager.playerInventory.ReturnCoolingState(i))
                        {
                            Q_GameMaster.Instance.inventoryManager.playerInventory.StartCD(i, coolTime);
                        }
                    }
                }
            }
        }

        public void CDAllConsumble(float coolTime)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].ID != -1)
                {
                    if (items[i].variety == Variety.Consumable)
                    {
                        if (!ReturnCoolingState(i))
                        {
                            StartCD(i, coolTime);
                        }
                    }
                }
            }

            if (this.tag == "Inventory")
            {
                for (int i = 0; i < Q_GameMaster.Instance.inventoryManager.skillBar.items.Count; i++)
                {
                    if (Q_GameMaster.Instance.inventoryManager.skillBar.items[i].variety == Variety.Consumable)
                    {
                        if (!Q_GameMaster.Instance.inventoryManager.skillBar.ReturnCoolingState(i))
                        {
                            Q_GameMaster.Instance.inventoryManager.skillBar.StartCD(i, coolTime);
                        }
                    }
                }
            }
            else if (this.tag == "SkillBar")
            {
                for (int i = 0; i < Q_GameMaster.Instance.inventoryManager.playerInventory.items.Count; i++)
                {
                    if (Q_GameMaster.Instance.inventoryManager.playerInventory.items[i].variety == Variety.Consumable)
                    {
                        if (!Q_GameMaster.Instance.inventoryManager.playerInventory.ReturnCoolingState(i))
                        {
                            Q_GameMaster.Instance.inventoryManager.playerInventory.StartCD(i, coolTime);
                        }
                    }
                }
            }

        }

        public void CDAllThis(Item item)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] == item)
                {
                    if (!ReturnCoolingState(i))
                    {
                        StartCD(i, item.coolDown);
                    }
                }
            }

            if (this.tag == "Inventory")
            {
                for (int i = 0; i < Q_GameMaster.Instance.inventoryManager.skillBar.items.Count; i++)
                {
                    if (Q_GameMaster.Instance.inventoryManager.skillBar.items[i] == item)
                    {
                        if (!Q_GameMaster.Instance.inventoryManager.skillBar.ReturnCoolingState(i))
                        {
                            Q_GameMaster.Instance.inventoryManager.skillBar.StartCD(i, item.coolDown);
                        }
                    }
                }
            }
            else if (this.tag == "SkillBar")
            {
                for (int i = 0; i < Q_GameMaster.Instance.inventoryManager.playerInventory.items.Count; i++)
                {
                    if (Q_GameMaster.Instance.inventoryManager.playerInventory.items[i] == item)
                    {
                        if (!Q_GameMaster.Instance.inventoryManager.playerInventory.ReturnCoolingState(i))
                        {
                            Q_GameMaster.Instance.inventoryManager.playerInventory.StartCD(i, item.coolDown);
                        }
                    }
                }
            }
        }

        public void DeleteCDList(int slotID)
        {
            if (slotID < cds.Count)
                cds[slotID] = new CoolDown();
        }

        protected void AddItemAtStart()
        {
            ItemToAdd[] itemsAddToStorage = GetComponent<AddItem>().itemsToAdd;
            foreach (ItemToAdd item in itemsAddToStorage)
            {
                if (item.itemsToAdd)
                    for (int i = 0; i < item.amount; i++)
                    {
                        float chance = Random.Range(0f, 1f);
                        if (item.chance > chance)
                            AddItem(item.itemsToAdd.ID);
                    }
            }
        }

        ItemData AddItemAtNewSlot(Item newItem, int amountToAdd)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].ID == -1)
                {
                    items[i] = newItem;
                    GameObject itemObj = Instantiate(m_InventoryItem, slots[i].transform);
                    itemObj.name = newItem.itemName;

                    itemObj.GetComponent<ItemDrag>().inv = this;

                    ItemData data = itemObj.GetComponent<ItemData>();//更新数据

                    //更新所在背包状态
                    if (tag == "Storage")//更改物品状态
                    {
                        data.isStorged = true;
                    }

                    else if (tag == "Vendor")
                    {
                        data.isOnSell = true;
                    }

                    else
                    {
                        data.isStorged = false;
                        data.isOnSell = false;
                    }

                    data.inv = this;

                    //change icon
                    data.icon = itemObj.transform.Find("Icon").GetComponent<Image>();
                    data.icon.sprite = newItem.icon;

                    data.amount = amountToAdd;
                    data.item = newItem;
                    data.slot = i;
                    Text numText = itemObj.transform.Find("Num").GetComponent<Text>();

                    //如果是可以消耗的，开启对应子物体
                    if (newItem.variety == Variety.Consumable)
                    {
                        data.amountText = numText;

                        //CD的设置
                        data.cd = itemObj.transform.Find("CD").GetComponent<Image>();
                        if (data.item.coolDown > 0)
                            data.cd.gameObject.SetActive(true);
                        cds[data.slot] = new CoolDown(data.cd, data.item.coolDown);
                    }


                    if (newItem.isStackable)
                    {
                        numText.text = data.amount.ToString();
                    }

                    else//如果不可重叠就关闭num text
                        numText.gameObject.SetActive(false);

                    if (newItem.useOnPickUp)
                    {
                        data.UseItem();
                    }

                    return data;
                }
            }
            return null;
        }

        int CheckItem(int id)//看物品是不是已经在背包中了
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].ID == id)
                {
                    ItemData data = slots[i].transform.GetChild(0).GetComponent<ItemData>();
                    if (data.item.maxStackNumber == data.amount)
                    {
                        //Debug.Log("This Slot is Full " + i);
                        continue;
                    }

                    else
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        protected void CDsCoolDown()
        {
            foreach (CoolDown m_CoolDown in cds)
            {
                if (m_CoolDown.cd)
                    if (m_CoolDown.isCooling && m_CoolDown.cd.fillAmount > 0)
                    {
                        m_CoolDown.cd.fillAmount -= Time.deltaTime * (1 / m_CoolDown.coolTime);
                        if (m_CoolDown.cd.fillAmount <= 0)
                        {
                            m_CoolDown.isCooling = false;
                            m_CoolDown.cd.fillAmount = 0;
                        }
                    }
            }
        }

        IEnumerator SetFalse()
        {
            yield return new WaitForSeconds(0.01f);
            m_InventoryPanel.SetActive(false);
        }
    }
}
