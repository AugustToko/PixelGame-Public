using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QInventory
{
    [AddComponentMenu("Q Inventory/Storage")]
    public class Storage : Q_Inventory
    {
        [SerializeField]
        private bool destroyWhenClear = false;
        private GameObject m_StoragePanel;
        private GameObject m_StorageSlot;
        private GameObject m_StorageItem;

        protected override void Start()
        {
            //--------------------分割线----------------------
            //初始化赋值各种参数
            m_StoragePanel = transform.GetChild(0).gameObject;
            m_StorageSlot = Q_GameMaster.Instance.inventoryManager.m_StorageSlot;
            m_StorageItem = Q_GameMaster.Instance.inventoryManager.m_StorageItem;

            //--------------------分割线----------------------
            //初始化添加格子
            Transform scrollRect = m_StoragePanel.transform.Find("Scroll Rect");
            //m_Scrollbar = scrollRect.Find("Scrollbar").GetComponent<Scrollbar>();
            //scrollRect.Find("Scrollbar").GetComponent<Scrollbar>().value = 1;//开头将滚动条的值重设为1,好像没用？
            m_GridList = scrollRect.Find("GridList").gameObject;
            for (int i = 0; i < slotAmount; i++)
            {
                items.Add(Item.CreateInstance<Item>());
                GameObject newSlot = Instantiate(m_StorageSlot, m_GridList.transform);
                Slot m_slot = newSlot.GetComponent<Slot>();
                m_slot.slotID = i;
                m_slot.inv = this;
                slots.Add(newSlot);
            }

            //初始化添加物品
            AddItemAtStart();

            //延时是为了初始化滑动条的
            StartCoroutine(SetFalse());
            if (destroyWhenClear)
                InvokeRepeating("CheckISClear", 1f, 0.2f);
        }

        //private void FixedUpdate()
        //{
        //    //ResetScrollBar(m_StorgePanel,m_Scrollbar);
        //}
        public override ItemData AddItem(int id)
        {
            Item newItem = Q_GameMaster.Instance.inventoryManager.itemDataBase.getItemByID(id);

            int temp = CheckItem(id);

            if (newItem.isStackable && temp != -1)//如果item是可以重叠并且背包中已经有该物品了
            {
                ItemData data = slots[temp].transform.GetChild(0).GetComponent<ItemData>();
                data.amount++;
                data.transform.Find("Num").GetComponent<Text>().text = data.amount.ToString();

                return data;
            }

            else
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].ID == -1)
                    {
                        items[i] = newItem;
                        GameObject itemObj = Instantiate(m_StorageItem, slots[i].transform);
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
                            //data.cd = itemObj.transform.Find("CD").GetComponent<Image>();
                            //data.cd.gameObject.SetActive(true);
                        }

                        if (newItem.isStackable)
                            numText.text = data.amount.ToString();
                        else//如果不可重叠就关闭num text
                            numText.gameObject.SetActive(false);

                        //Debug.Log("Add item to storge " + data.item.name);
                        return data;
                    }
                }
                return null;
            }

        }

        public override ItemData AddItem(Item newItem, int slotID, int amount)
        {
            //Item newItem = Q_GameMaster.Instance.inventoryManager.itemDataBase.getItemByID(id);

            if (items[slotID].ID == -1)
            {
                items[slotID] = newItem;
                GameObject itemObj = Instantiate(m_StorageItem, slots[slotID].transform);
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
                    //data.cd = itemObj.transform.Find("CD").GetComponent<Image>();
                    //if (data.item.coolDown > 0)
                    //    data.cd.gameObject.SetActive(true);
                    //cds[data.slot] = new CoolDown(data.cd, data.item.coolDown);
                }


                if (newItem.isStackable)
                {
                    numText.text = data.amount.ToString();
                }

                else//如果不可重叠就关闭num text
                    numText.gameObject.SetActive(false);

                return data;
            }

            return null;
        }

        public override int CheckSlot(int slotID)
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

        public void TakeAll()
        {
            bool flag = false;
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].ID != -1)
                {
                    ItemData data = slots[i].GetComponentInChildren<ItemData>();
                    for (int j = 0; j < data.amount; j++)
                    {
                        flag = Q_GameMaster.Instance.inventoryManager.playerInventory.CheckIsFull(items[i].ID);
                        if (flag)
                        {
                            Q_GameMaster.Instance.inventoryManager.SetInformation(Q_GameMaster.Instance.inventoryManager.infoManager.inventoryFull);
                            Debug.Log("Inventory is full");
                            data.amount -= j;
                            data.UpdataText();
                            break;
                        }
                        Q_GameMaster.Instance.inventoryManager.playerInventory.AddItem(items[i].ID);
                    }
                    if (!flag)
                        data.ClearItem();
                }
            }
            Q_GameMaster.Instance.inventoryManager.PlayAddItemClip();
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

        public void CheckISClear()
        {
            foreach (var item in items)
            {
                if (item.ID != -1)
                    return;
            }

            if (destroyWhenClear)
                Destroy(gameObject);
        }

        int CheckItem(int id)//看物品是不是已经在背包中了
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].ID == id)
                {
                    ItemData data = slots[i].transform.GetChild(0).GetComponent<ItemData>();
                    if (data.item.maxStackNumber == data.amount)
                        continue;
                    return i;
                }
            }
            return -1;
        }

        IEnumerator SetFalse()
        {
            yield return new WaitForSeconds(0.01f);
            m_StoragePanel.SetActive(false);
        }
    }
}

