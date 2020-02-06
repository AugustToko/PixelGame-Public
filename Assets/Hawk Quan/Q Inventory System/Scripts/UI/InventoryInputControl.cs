using System.Collections.Generic;
using UnityEngine;

namespace QInventory
{
    public class InventoryInputControl : MonoBehaviour
    {
        [SerializeField] List<InventoryUIControl> inventoryUIControls = new List<InventoryUIControl>();

        private void Update()
        {
            for (int i = 0; i < inventoryUIControls.Count; i++)
            {
                if (Input.GetKeyDown(inventoryUIControls[i].m_KeyCode))
                {
                    if (inventoryUIControls[i].m_InventoryUI != null)
                    {
                        Q_GameMaster.Instance.inventoryManager.PlayOpenPanelClip();
                        Q_GameMaster.Instance.inventoryManager.toolTip.tooltip.SetActive(false);
                        inventoryUIControls[i].m_InventoryUI
                            .SetActive(!inventoryUIControls[i].m_InventoryUI.activeSelf);
                    }
                }
            }
        }
    }
}