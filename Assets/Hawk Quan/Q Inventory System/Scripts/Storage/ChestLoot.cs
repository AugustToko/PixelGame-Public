using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QInventory
{
    public class ChestLoot : MonoBehaviour
    {

        [SerializeField]
        private float openDistance = 1f;
        [SerializeField]
        private bool panelAppearAbove = false;
        [SerializeField]
        private Vector3 offset;

        private GameObject m_Panel;
        private Scrollbar m_Scrollbar;
        private Storage m_Storage;
        private static GameObject insideLoot;
        private List<int> numbersofItem = new List<int>();
        private List<ItemDrop> itemsToDrop = new List<ItemDrop>();

        private void Start()
        {
            m_Panel = Q_GameMaster.Instance.inventoryManager.chestLootPanel;
            m_Scrollbar = m_Panel.transform.Find("Scroll Rect").Find("Scrollbar").GetComponent<Scrollbar>();
            m_Storage = Q_GameMaster.Instance.inventoryManager.chestloot;
        }

        private void Update()
        {
            if (!Q_GameMaster.Instance.inventoryManager.player)
                return;

            if (!m_Panel)
                return;

            if (Vector3.Distance(transform.position, Q_GameMaster.Instance.inventoryManager.player.transform.position) <= openDistance)
            {
                if(insideLoot != this.gameObject && insideLoot != null)
                {
                    if(Vector3.Distance(insideLoot.transform.position, Q_GameMaster.Instance.inventoryManager.player.transform.position) > Vector3.Distance(transform.position, Q_GameMaster.Instance.inventoryManager.player.transform.position))
                    {
                        insideLoot = this.gameObject;
                        m_Panel.SetActive(false);
                    }
                }

                if(insideLoot == null)
                {
                    insideLoot = this.gameObject;
                }

                if (Input.GetKeyDown(Q_InputManager.Instance.interact) && insideLoot == this.gameObject)
                {
                    if (m_Panel)
                    {
                        Q_GameMaster.Instance.inventoryManager.PlayOpenPanelClip();
                        OpenPanel();
                    }
                }
            }


            else if (m_Panel.activeSelf && insideLoot == this.gameObject)
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
                AddNewItemToLoot();
                if (panelAppearAbove)
                    m_Panel.transform.parent.position = Camera.main.WorldToScreenPoint(transform.position + offset);

            }


            else if (m_Panel.activeSelf)
            {
                Q_GameMaster.Instance.inventoryManager.toolTip.Deactivate();
            }

            m_Panel.SetActive(!m_Panel.activeSelf);
        }

        void AddNewItemToLoot()
        {
            m_Storage.ClearAllItems();
            for (int i = 0; i < itemsToDrop.Count; i++)
            {
                for (int j = 0; j < numbersofItem[i]; j++)
                {
                    m_Storage.AddItem(itemsToDrop[i].itemToDrop.ID);
                }
            }
        }

        public void SetItemsToDrop(List<ItemDrop> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                int chance = Random.Range(0, 100);
                if (chance < items[i].dropChance * 100)
                {
                    itemsToDrop.Add(items[i]);
                    numbersofItem.Add(Random.Range(items[i].minDropAmount, items[i].maxDropAmount));
                }
            }
        }
    }
}

