using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QInventory
{
    public class InventoryManager : MonoBehaviour
    {
        [Header("Base Scripts")]
        public PlayerInventoryManager playerInventoryManager;
        public EquipmentManager equipmentManager;
        public Tooltip toolTip;
        public EquipmentInventory equipmentInventory;
        public SkillBar skillBar;
        public Q_InfoManager infoManager;
        public ItemSortingManager itemSortingManager;

        [Header("Inventory Scripts")]
        public Q_Inventory playerInventory;
        public Storage storage;
        public Vendor vendor;
        public Crafting craft;
        public Storage chestloot;

        [Header("Inventory Panels")]
        public GameObject playerInventoryPanel;
        public GameObject storagePanel;
        public GameObject vendorPanel;
        public GameObject craftPanel;
        public GameObject chestLootPanel;

        [Header("Data Base")]
        public ItemList itemDataBase;

        [Header("Audio")]
        public AudioSource m_AudioSource = null;
        [Space(5)]
        [SerializeField]
        private AudioClip m_AddItemClip = null;
        [SerializeField]
        private AudioClip m_WearItemClip = null;
        [SerializeField]
        private AudioClip m_ChangeMoneyClip = null;
        [SerializeField]
        private AudioClip m_OpenPanelClip = null;

        [Header("Player")]
        public GameObject player;
        public CharacterController controller;
        public DropType dropType;
        public PickUp pickUp;
        public float playerWidth;
        public float playerHeight;
        public float dropForce;

        [Header("BaseUI")]
        public GameObject Canvas;

        [Header("Inventory UI")]
        public GameObject m_InventorySlot;
        public GameObject m_InventoryItem;

        [Header("Storage UI")]
        public GameObject m_StorageSlot;
        public GameObject m_StorageItem;

        [Header("Vendor UI")]
        public GameObject m_VendorSlot;
        public GameObject m_VendorItem;

        [Header("Crafting UI")]
        public GameObject m_CraftSlot;
        public GameObject m_CraftItem;

        [Header("Equipment UI")]
        public GameObject m_EquipmentItem;
        public GameObject m_EquipmentContainer;

        [Header("SkillBar UI")]
        public GameObject m_SkillBarSlot;
        public GameObject m_SkillBarItem;

        [Header("Currency Drop Item")]
        public GameObject m_CurrencyDropItem;

        [Header("Information")]
        public GameObject informationPanel;
        public Text informationText;

        [HideInInspector]
        public VendorTrigger activeVendorTrigger;
        [HideInInspector]
        public CraftTrigger activeCraftTrigger;

        public void PlayAddItemClip()
        {
            m_AudioSource.PlayOneShot(m_AddItemClip);
        }

        public void PlayWearClip()
        {
            m_AudioSource.PlayOneShot(m_WearItemClip);
        }

        public void PlayMoneyChangeClip()
        {
            m_AudioSource.PlayOneShot(m_ChangeMoneyClip);
        }

        public void PlayOpenPanelClip()
        {
            m_AudioSource.PlayOneShot(m_OpenPanelClip);
        }

        public void SetInformation(string info)
        {
            informationText.text = info;
            informationPanel.gameObject.SetActive(true);
        }

        #region 这里是用户用来get和change属性值用的
        public static float GetPlayerAttributeCurrentValue(string attributeName)
        {
            return PlayerInventoryManager.FindPlayerAttributeCurrentValueByName(attributeName);
        }

        public static float GetPlayerAttributeMaxValue(string attributeName)
        {
            return PlayerInventoryManager.FindPlayerAttributeMaxValueByName(attributeName);
        }

        public static float GetPlayerAttributeMinValue(string attributeName)
        {
            return PlayerInventoryManager.FindPlayerAttributeMinValueByName(attributeName);
        }

        public static void ChangePlayerAttributeValue(string attributeName, float amount, Effect effect)
        {
            PlayerInventoryManager.ChangePlayerAttributeByName(attributeName, amount, effect);
        }

        public static void SetPlayerAttributeValue(string attributeName, float amount, SetType type)
        {
            PlayerInventoryManager.SetPlayerAttributeByName(attributeName, amount, type);
        }

        public static float GetPlayerCurrency(string name)
        {
            return PlayerInventoryManager.FindPlayerCurrencyByName(name);
        }

        public static void ChangePlayerCurrency(List<Price> prices, int itemAmount)
        {
            PlayerInventoryManager.UpdatePlayerCurrency(prices, itemAmount);
        }
        #endregion

        #region 这里是用来进行数据存储操作的
        public static void SaveInventoryData()
        {
            Q_Inventory m_PlayerInventory = Q_GameMaster.Instance.inventoryManager.playerInventory;
            InventorySaveData m_SaveData = Q_GameMaster.Instance.inventoryManager.itemDataBase.saveData;
            List<Item> items = m_PlayerInventory.items;
            List<GameObject> slots = m_PlayerInventory.slots;

            m_SaveData.playerInventoryItems.Clear();

            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].ID != -1)
                {
                    int amount = slots[i].transform.GetChild(0).GetComponent<ItemData>().amount;
                    m_SaveData.playerInventoryItems.Add(new ItemToSave(items[i], amount));
                }
            }

            List<PlayerCurrency> playerCurrencies = Q_GameMaster.Instance.inventoryManager.playerInventoryManager.playerCurrencies;
            m_SaveData.playerCurrencies.Clear();
            for (int i = 0; i < playerCurrencies.Count; i++)
            {
                m_SaveData.playerCurrencies.Add(new CurrencyToSave(playerCurrencies[i].currency, playerCurrencies[i].amount));
            }

            List<PlayerAttribute> playerAttributes = Q_GameMaster.Instance.inventoryManager.playerInventoryManager.playerAttributes;
            m_SaveData.playerAttributes.Clear();
            for (int i = 0; i < playerAttributes.Count; i++)
            {
                m_SaveData.playerAttributes.Add(new PlayerAttribute(playerAttributes[i].playerAttribute, playerAttributes[i].currentValue, playerAttributes[i].minValue, playerAttributes[i].maxValue));
            }

            m_SaveData.onEquipmentItems.Clear();
            GameObject equipmentContainer = Q_GameMaster.Instance.inventoryManager.m_EquipmentContainer;
            for (int i = 0; i < equipmentContainer.transform.childCount; i++)
            {
                if(equipmentContainer.transform.GetChild(i).childCount == 2)
                {
                    ItemData data = equipmentContainer.transform.GetChild(i).GetChild(1).GetComponent<ItemData>();
                    m_SaveData.onEquipmentItems.Add(data.item.ID);
                }
            }


            Debug.Log("Successfully Save Inventory Data");
        }

        public static void ClearInventoryData()
        {
            InventorySaveData m_SaveData = Q_GameMaster.Instance.inventoryManager.itemDataBase.saveData;
            m_SaveData.playerInventoryItems.Clear();
            m_SaveData.playerAttributes.Clear();
            m_SaveData.playerCurrencies.Clear();
            m_SaveData.onEquipmentItems.Clear();
            Debug.Log("Successfully Clear Inventory Data");
        }
        #endregion
    }
}

