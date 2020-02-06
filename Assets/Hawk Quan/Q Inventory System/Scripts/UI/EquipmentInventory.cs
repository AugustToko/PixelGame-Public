using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QInventory
{
    [AddComponentMenu("Q Inventory/Equipment Inventory")]
    public class EquipmentInventory : Q_Inventory
    {
        [HideInInspector]
        public List<EquipmentSlot> equipmentSlot;
        private GameObject m_EquipmentItem;
        private GameObject m_EquipmentPanel;
        protected override void Start()
        {
            m_EquipmentItem = Q_GameMaster.Instance.inventoryManager.m_EquipmentItem;
            m_EquipmentPanel = transform.GetChild(0).gameObject;
            StartCoroutine(SetFalse());
            //m_Scrollbar = m_EquipmentPanel.transform.Find("Scroll Rect").Find("Scrollbar").GetComponent<Scrollbar>();
        }

        //private void FixedUpdate()
        //{
        //    //ResetScrollBar(m_EquipmentPanel, m_Scrollbar);
        //}
        public void AddItem(ItemData itemData)
        {
            foreach (var slot in equipmentSlot)
            {
                if (slot.equipmentPart == itemData.item.equipmentPart)
                {
                    if (slot.transform.childCount == 1)
                    {
                        //Debug.Log("wear");
                        slot.transform.Find("bg").gameObject.SetActive(false);
                        GameObject itemObj = Instantiate(m_EquipmentItem, slot.transform);
                        ItemData data = itemObj.GetComponent<ItemData>();
                        data.inv = this;
                        itemObj.transform.Find("Icon").GetComponent<Image>().sprite = itemData.item.icon;
                        data.isOnEquipment = true;
                        data.item = itemData.item;
                        break;
                    }

                    else
                    {
                        Destroy(slot.transform.Find("EquipmentItem(Clone)").gameObject);

                        slot.transform.Find("bg").gameObject.SetActive(false);
                        GameObject itemObj = Instantiate(m_EquipmentItem, slot.transform);
                        ItemData data = itemObj.GetComponent<ItemData>();
                        data.inv = this;
                        itemObj.transform.Find("Icon").GetComponent<Image>().sprite = itemData.item.icon;
                        data.isOnEquipment = true;
                        data.item = itemData.item;
                        break;
                    }
                }
            }
        }

        IEnumerator SetFalse()
        {
            yield return new WaitForSeconds(0.01f);
            m_EquipmentPanel.SetActive(false);
        }
    }
}

