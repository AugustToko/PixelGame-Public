using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QInventory
{
    public class PlayerInventoryManager : MonoBehaviour
    {
        [Header("Player Attribute Setting")]
        public List<PlayerAttribute> playerAttributes = new List<PlayerAttribute>();

        [Header("Player Attribute Displayer UI")] [SerializeField]
        public List<PlayerAttributeUI> playerAttributesUIs = new List<PlayerAttributeUI>();

        [Header("Player Currency")] public List<PlayerCurrency> playerCurrencies = new List<PlayerCurrency>();

        [HideInInspector] public float totalCurrencyValue;

        private void Start()
        {
            UpdateTotalCurrencyValue();
            ConvertPlayerCurrency();
        }

        private void FixedUpdate()
        {
            //--------------------分割线----------------------
            //这个是用于更新需要展示的玩家属性UI的数据的
            foreach (PlayerAttributeUI m_AttributeUI in playerAttributesUIs)
            {
                UpdateAttributeDisplayUI(m_AttributeUI);
            }
            //--------------------分割线----------------------

            //这个是用于更新展示的货币UI的
            foreach (PlayerCurrency m_Currency in playerCurrencies)
            {
                UpdateCurrencyDisplayUI(m_Currency);
            }

            //--------------------分割线----------------------
        }

        //--------------------分割线----------------------
        //public的method
        public static void UpdatePlayerCurrency(List<Price> prices, int itemAmount)
        {
            float totalPrice = 0;
            //计算通用价值
            foreach (Price m_Price in prices)
            {
                totalPrice += m_Price.amount * m_Price.currency.exchangeRate;
            }

            Q_GameMaster.Instance.inventoryManager.playerInventoryManager.totalCurrencyValue += totalPrice * itemAmount;
            float tempTotalCurrencyValue =
                Q_GameMaster.Instance.inventoryManager.playerInventoryManager.totalCurrencyValue;
            foreach (PlayerCurrency m_PlayerCurrency in Q_GameMaster.Instance.inventoryManager.playerInventoryManager
                .playerCurrencies)
            {
                m_PlayerCurrency.amount = ((int) tempTotalCurrencyValue / (int) m_PlayerCurrency.currency.exchangeRate);
                //Debug.Log(m_PlayerCurrency.currency.currencyName + m_PlayerCurrency.amount);
                tempTotalCurrencyValue -= m_PlayerCurrency.amount * m_PlayerCurrency.currency.exchangeRate;
            }

            Q_GameMaster.Instance.inventoryManager.PlayMoneyChangeClip();
        }

        public static float FindPlayerCurrencyByName(string name)
        {
            foreach (var currency in Q_GameMaster.Instance.inventoryManager.playerInventoryManager.playerCurrencies)
            {
                if (currency.currency.currencyName == name)
                    return currency.amount;
            }

            return 0;
        }

        public static float FindPlayerAttributeCurrentValueByName(string attributeName)
        {
            foreach (PlayerAttribute m_Attribute in Q_GameMaster.Instance.inventoryManager.playerInventoryManager
                .playerAttributes)
            {
                if (m_Attribute.playerAttribute.name == attributeName)
                    return m_Attribute.currentValue;
            }

            return 0;
        }

        public static float FindPlayerAttributeMaxValueByName(string attributeName)
        {
            foreach (PlayerAttribute m_Attribute in Q_GameMaster.Instance.inventoryManager.playerInventoryManager
                .playerAttributes)
            {
                if (m_Attribute.playerAttribute.name == attributeName)
                    return m_Attribute.maxValue;
            }

            return 0;
        }

        public static float FindPlayerAttributeMinValueByName(string attributeName)
        {
            foreach (PlayerAttribute m_Attribute in Q_GameMaster.Instance.inventoryManager.playerInventoryManager
                .playerAttributes)
            {
                if (m_Attribute.playerAttribute.name == attributeName)
                    return m_Attribute.minValue;
            }

            return 0;
        }


        public void ChangePlayerAttributeByItemAttribute(List<ConsumableAttribute> consumableItemAttributes) //遍历所有状态
        {
            for (int i = 0; i < playerAttributes.Count; i++)
            {
                PlayerAttribute m_Attribute = playerAttributes[i];
                foreach (ConsumableAttribute itemAttribute in consumableItemAttributes)
                {
                    if (m_Attribute.playerAttribute.name == itemAttribute.consumableItemAttribute.attributeName)
                    {
                        ChangePlayerAttribute(i, itemAttribute);
                    }
                }
            }
        }

        public void ChangePlayerAttributeByItemAttribute(List<EquipmentAttribute> equipmentItemAttributes)
        {
            for (int i = 0; i < playerAttributes.Count; i++)
            {
                PlayerAttribute m_Attribute = playerAttributes[i];
                foreach (EquipmentAttribute itemAttribute in equipmentItemAttributes)
                {
                    if (m_Attribute.playerAttribute.name == itemAttribute.equipmentItemAttribute.attributeName)
                    {
                        ChangePlayerAttribute(i, itemAttribute);
                    }
                }
            }
        }

        public static void SetPlayerAttributeByName(string attributeName, float amount, SetType type)
        {
            foreach (PlayerAttribute m_Attribute in Q_GameMaster.Instance.inventoryManager.playerInventoryManager
                .playerAttributes)
            {
                if (m_Attribute.playerAttribute.attributeName == attributeName)
                {
                    if (type == SetType.CurrentValue)
                    {
                        m_Attribute.currentValue = Mathf.Clamp(amount, m_Attribute.minValue, m_Attribute.maxValue);
                    }

                    if (type == SetType.MaxValue)
                    {
                        m_Attribute.maxValue = amount;
                    }

                    if (type == SetType.MinValue)
                    {
                        m_Attribute.minValue = amount;
                    }
                }
            }
        }

        public static void ChangePlayerAttributeByName(string attributeName, float changeAmount, Effect effect)
        {
            foreach (PlayerAttribute m_Attribute in Q_GameMaster.Instance.inventoryManager.playerInventoryManager
                .playerAttributes)
            {
                if (m_Attribute.playerAttribute.attributeName == attributeName)
                {
                    if (effect == Effect.Restore)
                    {
                        m_Attribute.currentValue += changeAmount;
                    }

                    else if (effect == Effect.Decrease)
                    {
                        m_Attribute.currentValue -= changeAmount;
                    }

                    else if (effect == Effect.IncreaseMaxValue)
                    {
                        m_Attribute.maxValue += changeAmount;
                    }

                    else if (effect == Effect.DecreaseMaxValue)
                    {
                        m_Attribute.maxValue -= changeAmount;
                    }

                    m_Attribute.currentValue = Mathf.Clamp(m_Attribute.currentValue, m_Attribute.minValue,
                        m_Attribute.maxValue);
                }
            }
        }

        //清除之前穿戴的装备的加上的属性
        public void ClearEquipmentAttributeByItemAttribute(List<EquipmentAttribute> equipmentItemAttributes)
        {
            for (int i = 0; i < playerAttributes.Count; i++)
            {
                PlayerAttribute m_Attribute = playerAttributes[i];
                foreach (EquipmentAttribute itemAttribute in equipmentItemAttributes)
                {
                    if (m_Attribute.playerAttribute.name == itemAttribute.equipmentItemAttribute.attributeName)
                    {
                        float temp = itemAttribute.value;
                        itemAttribute.value = -temp;
                        ChangePlayerAttribute(i, itemAttribute);
                        itemAttribute.value = temp;
                    }
                }
            }
        }

        public void ChangeCurrencyByName(string currencyName, float changeAmount)
        {
            foreach (PlayerCurrency m_Currency in playerCurrencies)
            {
                if (m_Currency.currency.currencyName == currencyName)
                {
                    m_Currency.amount += changeAmount;
                }
            }
        }

        public void UpdateTotalCurrencyValue()
        {
            foreach (PlayerCurrency m_Currency in playerCurrencies)
            {
                totalCurrencyValue += m_Currency.amount * m_Currency.currency.exchangeRate;
            }

            //Q_GameMaster.Instance.inventoryManager.PlayMoneyChangeClip();
        }

        //--------------------分割线----------------------
        void ChangePlayerAttribute(int i, ConsumableAttribute itemAttribute)
        {
            if (itemAttribute.effectType == Effect.Restore)
            {
                float changedValue = playerAttributes[i].currentValue + itemAttribute.value;
                if (changedValue > playerAttributes[i].maxValue)
                    playerAttributes[i].currentValue = playerAttributes[i].maxValue;
                else
                    playerAttributes[i].currentValue = changedValue;
            }

            else if (itemAttribute.effectType == Effect.Decrease)
            {
                float changedValue = playerAttributes[i].currentValue - itemAttribute.value;
                if (changedValue < playerAttributes[i].minValue)
                    playerAttributes[i].currentValue = playerAttributes[i].minValue;
                else
                    playerAttributes[i].currentValue = changedValue;
            }

            else if (itemAttribute.effectType == Effect.IncreaseMaxValue)
                playerAttributes[i].maxValue += itemAttribute.value;

            else if (itemAttribute.effectType == Effect.DecreaseMaxValue)
                playerAttributes[i].maxValue -= itemAttribute.value;

            if (playerAttributes[i].maxValue < playerAttributes[i].currentValue)
            {
                playerAttributes[i].currentValue = playerAttributes[i].maxValue;
            }
        }

        void ChangePlayerAttribute(int i, EquipmentAttribute itemAttribute)
        {
            if (itemAttribute.effectType == Effect.Restore)
            {
                float changedValue = playerAttributes[i].currentValue + itemAttribute.value;
                if (changedValue > playerAttributes[i].maxValue)
                    playerAttributes[i].currentValue = playerAttributes[i].maxValue;
                else
                    playerAttributes[i].currentValue = changedValue;
            }

            else if (itemAttribute.effectType == Effect.Decrease)
            {
                float changedValue = playerAttributes[i].currentValue - itemAttribute.value;
                if (changedValue < playerAttributes[i].minValue)
                    playerAttributes[i].currentValue = playerAttributes[i].minValue;
                else
                    playerAttributes[i].currentValue = changedValue;
            }

            else if (itemAttribute.effectType == Effect.IncreaseMaxValue)
                playerAttributes[i].maxValue += itemAttribute.value;

            else if (itemAttribute.effectType == Effect.DecreaseMaxValue)
                playerAttributes[i].maxValue -= itemAttribute.value;

            if (playerAttributes[i].maxValue < playerAttributes[i].currentValue)
            {
                playerAttributes[i].currentValue = playerAttributes[i].maxValue;
            }
        }

        void UpdateAttributeDisplayUI(PlayerAttributeUI m_playerAttributeUI)
        {
            //如果赋值了text就更新Text
            if (m_playerAttributeUI.showText)
            {
                if (m_playerAttributeUI.showType == ShowType.CurrentValue)
                {
                    string m_name = m_playerAttributeUI.m_Attribute.attributeName;
                    m_playerAttributeUI.showText.text =
                        m_name + " : " + FindPlayerAttributeCurrentValueByName(m_name).ToString();
                }

                if (m_playerAttributeUI.showType == ShowType.CurrentValueWithMaxValue)
                {
                    string m_name = m_playerAttributeUI.m_Attribute.attributeName;
                    m_playerAttributeUI.showText.text =
                        m_name + " : " + FindPlayerAttributeCurrentValueByName(m_name).ToString() + " / " +
                        FindPlayerAttributeMaxValueByName(m_name);
                }

                else if (m_playerAttributeUI.showType == ShowType.MaxValue)
                {
                    string m_name = m_playerAttributeUI.m_Attribute.attributeName;
                    m_playerAttributeUI.showText.text =
                        m_name + " : " + FindPlayerAttributeMaxValueByName(m_name).ToString();
                }

                else if (m_playerAttributeUI.showType == ShowType.MinValue)
                {
                    string m_name = m_playerAttributeUI.m_Attribute.attributeName;
                    m_playerAttributeUI.showText.text =
                        m_name + " : " + FindPlayerAttributeMinValueByName(m_name).ToString();
                }
            }

            //如果赋值了slider就更新slider
            if (m_playerAttributeUI.showSlider)
            {
                if (m_playerAttributeUI.showType == ShowType.CurrentValueWithMaxValue)
                {
                    string m_name = m_playerAttributeUI.m_Attribute.attributeName;
                    m_playerAttributeUI.showSlider.value = FindPlayerAttributeCurrentValueByName(m_name) /
                                                           FindPlayerAttributeMaxValueByName(m_name);
                }

                //else if (m_playerAttributeUI.showType == PlayerAttributeUI.ShowType.MaxValue)
                //{
                //    string m_name = m_playerAttributeUI.m_Attribute.attributeName;
                //    m_playerAttributeUI.showSlider.value = FindPlayerAttributeMaxValueByName(m_name) / FindPlayerAttributeMaxValueByName(m_name);
                //}

                //else if (m_playerAttributeUI.showType == PlayerAttributeUI.ShowType.MinValue)
                //{
                //    string m_name = m_playerAttributeUI.m_Attribute.attributeName;
                //    m_playerAttributeUI.showSlider.value = FindPlayerAttributeMinValueByName(m_name) / FindPlayerAttributeMaxValueByName(m_name);
                //}
            }
        }

        void UpdateCurrencyDisplayUI(PlayerCurrency m_Currency)
        {
            if (m_Currency.showText)
            {
                if (m_Currency.currency)
                    m_Currency.showText.text = m_Currency.amount.ToString();
                //m_Currency.showText.text =m_Currency.amount.ToString()+ " " + m_Currency.currency.currencyName;
            }
        }

        void ConvertPlayerCurrency()
        {
            float tempTotalCurrencyValue = totalCurrencyValue;
            foreach (PlayerCurrency m_PlayerCurrency in playerCurrencies)
            {
                m_PlayerCurrency.amount = ((int) tempTotalCurrencyValue / (int) m_PlayerCurrency.currency.exchangeRate);
                tempTotalCurrencyValue -= m_PlayerCurrency.amount * m_PlayerCurrency.currency.exchangeRate;
            }
        }
    }
}