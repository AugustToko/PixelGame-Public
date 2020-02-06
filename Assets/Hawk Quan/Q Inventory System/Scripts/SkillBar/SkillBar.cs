using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QInventory
{
    [AddComponentMenu("Q Inventory/SkillBar")]
    public class SkillBar : Q_Inventory
    {
        private GameObject m_SkillBarPanel;
        private GameObject m_SkillBarSlot;
        private GameObject m_SkillBarItem;
        // Use this for initialization

        protected override void Start()
        {
            //--------------------分割线----------------------
            //初始化赋值各种参数
            m_SkillBarPanel = transform.GetChild(0).gameObject;
            m_SkillBarSlot = Q_GameMaster.Instance.inventoryManager.m_SkillBarSlot;
            m_SkillBarItem = Q_GameMaster.Instance.inventoryManager.m_SkillBarItem;

            //--------------------分割线----------------------
            //初始化添加格子
            m_GridList = m_SkillBarPanel.transform.Find("GridList").gameObject;
            //scrollRect.Find("Scrollbar").GetComponent<Scrollbar>().value = 1;//开头将滚动条的值重设为1,好像没用？
            //m_Scrollbar = null;
            for (int i = 0; i < GetComponent<SkillBarInputManager>().ReturnKeyListCount(); i++)
            {
                items.Add(Item.CreateInstance<Item>());
                GameObject newSlot = Instantiate(m_SkillBarSlot, m_GridList.transform);
                Slot m_slot = newSlot.GetComponent<Slot>();
                m_slot.slotID = i;
                m_slot.inv = this;
                slots.Add(newSlot);
                cds.Add(new CoolDown());
            }
        }

        private void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public override ItemData AddItem(Item newItem, int slotID, int amount)
        {
            Debug.Log("get " + newItem.itemName);

            if (items[slotID].ID == -1)
            {
                items[slotID] = newItem;
                GameObject itemObj = Instantiate(m_SkillBarItem, slots[slotID].transform);
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

    }
}
