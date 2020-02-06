using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QInventory
{
    public class CraftTrigger : MonoBehaviour
    {
        public List<BluePrintToAdd> bluePrints = new List<BluePrintToAdd>();
        public List<Category> categories = new List<Category>();
        [SerializeField]
        private float openDistance = 1f;
        [SerializeField]
        private bool panelAppearAbove = false;
        [SerializeField]
        private Vector3 offset;

        private GameObject m_Panel;
        private Scrollbar m_Scrollbar;
        private Crafting m_Craft;
        private static GameObject insideCraft;

        private void Start()
        {
            m_Panel = Q_GameMaster.Instance.inventoryManager.craftPanel;
            m_Scrollbar = m_Panel.transform.Find("Scroll Rect").Find("Scrollbar").GetComponent<Scrollbar>();
            m_Craft = Q_GameMaster.Instance.inventoryManager.craft;

            if (categories.Count > 0)
            {
                foreach (var bluePrint in Q_GameMaster.Instance.inventoryManager.itemDataBase.bluePrints)
                {
                    foreach (var category in categories)
                    {
                        if (bluePrint.category == category)
                        {
                            bluePrints.Add(new BluePrintToAdd(bluePrint,false));
                            break;
                        }
                    }
                }
            }
        }

        private void Update()
        {
            if (!Q_GameMaster.Instance.inventoryManager.player)
                return;

            if (!m_Panel)
                return;

            if (Vector3.Distance(transform.position, Q_GameMaster.Instance.inventoryManager.player.transform.position) <= openDistance)
            {
                if (insideCraft != this.gameObject && insideCraft != null)
                {
                    if (Vector3.Distance(insideCraft.transform.position, Q_GameMaster.Instance.inventoryManager.player.transform.position) > Vector3.Distance(transform.position, Q_GameMaster.Instance.inventoryManager.player.transform.position))
                    {
                        insideCraft = this.gameObject;
                        m_Panel.SetActive(false);
                    }
                }

                if (insideCraft == null)
                {
                    insideCraft = this.gameObject;
                }

                if (Input.GetKeyDown(Q_InputManager.Instance.interact) && insideCraft == this.gameObject)
                {
                    if (m_Panel)
                    {
                        Q_GameMaster.Instance.inventoryManager.PlayOpenPanelClip();
                        OpenPanel();
                    }
                }
            }


            else if (m_Panel.activeSelf && insideCraft == this.gameObject)
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

        void OpenPanel()
        {
            if (!m_Panel.activeSelf)
            {
                AddNewItemToVendor();

                Q_GameMaster.Instance.inventoryManager.activeCraftTrigger = this;

                if (panelAppearAbove)
                    m_Panel.transform.parent.position = Camera.main.WorldToScreenPoint(transform.position + offset);

            }


            else if (m_Panel.activeSelf)
            {
                Q_GameMaster.Instance.inventoryManager.toolTip.Deactivate();
            }

            m_Panel.SetActive(!m_Panel.activeSelf);
        }

        void AddNewItemToVendor()
        {
            m_Craft.ClearAllItems();

            for (int i = 0; i < bluePrints.Count; i++)
            {
                if (!bluePrints[i].isMoved)
                {
                    ItemData itemdata = m_Craft.AddItem(bluePrints[i].bluePrint);
                    itemdata.flag = i;
                }
            }
        }
    }
}
