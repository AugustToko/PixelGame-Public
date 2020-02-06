using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QInventory
{
    public class LoadInventoryData : MonoBehaviour
    {
        [Header("Loading Settings")]
        public bool isLoadingAtStart;

        void Start()
        {
            if (isLoadingAtStart)
                Load();
        }

        public void Load()
        {
            //Player
            Q_Inventory playerInventory = Q_GameMaster.Instance.inventoryManager.playerInventory;
            PlayerInventoryManager playerInventoryManager = Q_GameMaster.Instance.inventoryManager.playerInventoryManager;
            InventorySaveData saveData = Q_GameMaster.Instance.inventoryManager.itemDataBase.saveData;
            GameObject equipmentContainer = Q_GameMaster.Instance.inventoryManager.m_EquipmentContainer;
            for (int i = 0; i < equipmentContainer.transform.childCount; i++)
            {
                if (equipmentContainer.transform.GetChild(i).childCount == 2)
                {
                    ItemData data = equipmentContainer.transform.GetChild(i).GetChild(1).GetComponent<ItemData>();
                    data.UnwearItemWithoutAdd();
                }
            }

            playerInventory.ClearAllItems();

            StartCoroutine(LoadDelay());
        }

        IEnumerator LoadDelay()
        {
            yield return 0;
            Q_Inventory playerInventory = Q_GameMaster.Instance.inventoryManager.playerInventory;
            PlayerInventoryManager playerInventoryManager = Q_GameMaster.Instance.inventoryManager.playerInventoryManager;
            InventorySaveData saveData = Q_GameMaster.Instance.inventoryManager.itemDataBase.saveData;
            GameObject equipmentContainer = Q_GameMaster.Instance.inventoryManager.m_EquipmentContainer;

            foreach (var item in saveData.playerInventoryItems)
            {
                for (int i = 0; i < item.amount; i++)
                {
                    playerInventory.AddItem(item.item.ID);
                }
            }

            for (int i = 0; i < saveData.playerAttributes.Count; i++)
            {
                playerInventoryManager.playerAttributes[i].currentValue = saveData.playerAttributes[i].currentValue;
                playerInventoryManager.playerAttributes[i].minValue = saveData.playerAttributes[i].minValue;
                playerInventoryManager.playerAttributes[i].maxValue = saveData.playerAttributes[i].maxValue;
            }

            for (int i = 0; i < saveData.playerCurrencies.Count; i++)
            {
                playerInventoryManager.playerCurrencies[i].amount = saveData.playerCurrencies[i].amount;
            }

            for (int i = 0; i < saveData.onEquipmentItems.Count; i++)
            {
                playerInventory.AddItem(saveData.onEquipmentItems[i]).WearEquipmentItemWithoutChangeAttribute();
            }
        }
    }
}
