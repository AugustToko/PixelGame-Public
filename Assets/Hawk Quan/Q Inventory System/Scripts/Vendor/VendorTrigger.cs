using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QInventory
{
    public class VendorTrigger : MonoBehaviour
    {
        public List<ItemToSell> itemsToSell = new List<ItemToSell>();
        [SerializeField]
        private float openDistance = 1f;
        [SerializeField]
        private bool panelAppearAbove = false;
        [SerializeField]
        private Vector3 offset;

        private GameObject m_Panel;
        private Scrollbar m_Scrollbar;
        private Vendor m_Vendor;
        private static GameObject insideVendor;

        private int vendorID;

        private void Start()
        {
            m_Panel = Q_GameMaster.Instance.inventoryManager.vendorPanel;
            m_Scrollbar = m_Panel.transform.Find("Scroll Rect").Find("Scrollbar").GetComponent<Scrollbar>();
            m_Vendor = Q_GameMaster.Instance.inventoryManager.vendor;
        }

        private void Update()
        {
            if (!Q_GameMaster.Instance.inventoryManager.player)
                return;

            if (!m_Panel)
                return;

            if (Vector3.Distance(transform.position, Q_GameMaster.Instance.inventoryManager.player.transform.position) <= openDistance)
            {
                if (insideVendor != this.gameObject && insideVendor != null)
                {
                    if (Vector3.Distance(insideVendor.transform.position, Q_GameMaster.Instance.inventoryManager.player.transform.position) > Vector3.Distance(transform.position, Q_GameMaster.Instance.inventoryManager.player.transform.position))
                    {
                        insideVendor = this.gameObject;
                        m_Panel.SetActive(false);
                    }
                }

                if(insideVendor == null)
                {
                    insideVendor = gameObject;
                }

                if (Input.GetKeyDown(Q_InputManager.Instance.interact) && insideVendor == this.gameObject)
                {
                    if (m_Panel)
                    {
                        Q_GameMaster.Instance.inventoryManager.PlayOpenPanelClip();
                        OpenPanel();
                    }
                }
            }


            else if (m_Panel.activeSelf && insideVendor == this.gameObject)
            {
                if (m_Scrollbar)
                    m_Scrollbar.value = 1;
                m_Panel.SetActive(false);
                Q_GameMaster.Instance.inventoryManager.toolTip.Deactivate();
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, openDistance);
        }

        private void OpenPanel()
        {
            if (!m_Panel.activeSelf)
            {
                AddNewItemToVendor();

                Q_GameMaster.Instance.inventoryManager.activeVendorTrigger = this;

                if (panelAppearAbove)
                    m_Panel.transform.parent.position = Camera.main.WorldToScreenPoint(transform.position + offset);

            }


            else if (m_Panel.activeSelf)
            {
                Q_GameMaster.Instance.inventoryManager.toolTip.Deactivate();
            }

            m_Panel.SetActive(!m_Panel.activeSelf);
        }

        private void AddNewItemToVendor()
        {
            m_Vendor.ClearAllItems();
            for (int i = 0; i < itemsToSell.Count; i++)
            {
                if(!itemsToSell[i].isMoved)
                {
                    ItemData itemdata = m_Vendor.AddItem(itemsToSell[i]);
                    itemdata.flag = i;
                }
            }
        }

        //private void Reset()
        //{
        //    InventorySaveData inventorySaveData = null;
        //    inventorySaveData = GameObject.Find("Q_InventoryManager").GetComponent<InventoryManager>().itemDataBase.saveData;
        //    vendorID = inventorySaveData.vendorData.Count;
        //    inventorySaveData.vendorData.Add(new VendorData(vendorID, this));
        //}

        public int GetVendorID()
        {
            return vendorID;
        }
    }
}