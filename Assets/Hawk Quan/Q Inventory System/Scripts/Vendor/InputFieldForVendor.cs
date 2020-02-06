using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QInventory
{
    public class InputFieldForVendor : MonoBehaviour
    {
        [SerializeField]
        private InputField m_InputField;
        [SerializeField]
        private Text m_InputFieldText;
        [SerializeField]
        private ItemData m_ItemData;

        private void Start()
        {
            var se = new InputField.SubmitEvent();
            se.AddListener(OnSubmit);
            m_InputField.onEndEdit = se;
        }

        private void OnSubmit(string amount)
        {
            Debug.Log(amount);
            m_ItemData.SetAmountToBuy(amount);
        }

        public void AddAmount()
        {
            m_ItemData.amountToBuy++;
            m_InputField.text = m_ItemData.amountToBuy.ToString();
        }

        public void MinusAmount()
        {
            if (m_ItemData.amountToBuy > 1)
            {
                m_ItemData.amountToBuy--;
                m_InputField.text = m_ItemData.amountToBuy.ToString();
            }
        }

        public void BuyItem()
        {
            m_ItemData.BuyItem();
        }
    }
}

