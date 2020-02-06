using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QInventory
{
    [AddComponentMenu("Q Inventory/Vendor")]
    public class Vendor : Q_Inventory
    {
        private GameObject m_VendorPanel;
        private GameObject m_VendorSlot;
        private GameObject m_VendorItem;

        protected override void Start()
        {
            //--------------------分割线----------------------
            //初始化赋值各种参数
            m_VendorPanel = transform.GetChild(0).gameObject;
            m_VendorSlot = Q_GameMaster.Instance.inventoryManager.m_VendorSlot;
            m_VendorItem = Q_GameMaster.Instance.inventoryManager.m_VendorItem;

            //--------------------分割线----------------------
            //初始化添加格子
            Transform scrollRect = m_VendorPanel.transform.Find("Scroll Rect");
            //m_Scrollbar = scrollRect.Find("Scrollbar").GetComponent<Scrollbar>();
            //scrollRect.Find("Scrollbar").GetComponent<Scrollbar>().value = 1;//开头将滚动条的值重设为1,好像没用？
            m_GridList = scrollRect.Find("GridList").gameObject;
            for (int i = 0; i < slotAmount; i++)
            {
                items.Add(Item.CreateInstance<Item>());
                GameObject newSlot = Instantiate(m_VendorSlot, m_GridList.transform);
                Slot m_slot = newSlot.GetComponent<Slot>();
                m_slot.slotID = i;
                m_slot.inv = this;
                slots.Add(newSlot);
            }

            //添加物品
            //ItemToSell[] itemsToSell = GetComponent<AddVendorItem>().itemsToSell;
            //foreach (ItemToSell m_Item in itemsToSell)
            //{
            //    AddItem(m_Item);
            //}

            //延时是为了初始化滑动条的
            StartCoroutine(SetFalse());
        }


        //--------------------分割线----------------------
        public ItemData AddItem(ItemToSell m_Item)
        {
            Item newItem = m_Item.itemToSell;
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].ID == -1)
                {
                    items[i] = newItem;
                    GameObject itemObj = Instantiate(m_VendorItem, slots[i].transform);
                    itemObj.name = newItem.itemName;
                    Transform price = itemObj.transform.Find("Price");
                    List<Text> priceText = new List<Text>();

                    for (int j = 0; j < price.childCount; j++)
                    {
                        priceText.Add(price.GetChild(j).GetChild(0).GetComponent<Text>());
                    }
                    ItemData data = itemObj.GetComponent<ItemData>();//更新数据

                    //更新购买信息
                    data.moveAfterPurchase = m_Item.moveAfterPurchase;

                    if (!m_Item.useDefaultPrice)
                        for (int j = 0; j < m_Item.prices.Count; j++)
                        {
                            data.realPrice += m_Item.prices[j].amount * m_Item.prices[j].currency.exchangeRate;
                            if (j < priceText.Count)
                                priceText[j].text = m_Item.prices[j].amount.ToString();
                        }

                    else
                    {
                        List<Price> m_price = m_Item.itemToSell.buy_Price;
                        for (int j = 0; j < m_price.Count; j++)
                        {
                            data.realPrice += m_price[j].amount * m_price[j].currency.exchangeRate;
                            if (j < priceText.Count)
                                priceText[j].text = m_price[j].amount.ToString();
                        }
                    }

                    //更新所在背包状态
                    data.isStorged = false;
                    data.isOnSell = true;
                    data.isOnCrafting = false;

                    data.inv = this;

                    //加inputfield
                    data.inputField = itemObj.transform.Find("Button").Find("InputField").GetComponent<InputField>();

                    //如果是限定合成移除，则只能造一个，禁用inputfield
                    if (data.moveAfterPurchase)
                    {
                        data.inputField.gameObject.SetActive(false);
                        itemObj.transform.Find("Button").Find("ButtonMin").gameObject.SetActive(false);
                        itemObj.transform.Find("Button").Find("ButtonPlus").gameObject.SetActive(false);
                    }

                    //change icon
                    data.icon = itemObj.transform.Find("Icon").GetComponent<Image>();
                    data.icon.sprite = newItem.icon;

                    data.item = newItem;
                    data.slot = i;
                    return data;
                }
            }

            return null;
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

        IEnumerator SetFalse()
        {
            yield return new WaitForSeconds(0.01f);
            m_VendorPanel.SetActive(false);
        }

    }
}