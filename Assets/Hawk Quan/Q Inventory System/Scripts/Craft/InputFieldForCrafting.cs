using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QInventory
{
    public class InputFieldForCrafting : MonoBehaviour
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
            //Debug.Log(amount);
            m_ItemData.SetAmountToCraft(amount);
        }

        public void AddAmount()
        {
            m_ItemData.amountToCraft++;
            m_InputField.text = m_ItemData.amountToCraft.ToString();
        }

        public void MinusAmount()
        {
            if (m_ItemData.amountToCraft > 1)
            {
                m_ItemData.amountToCraft--;
                m_InputField.text = m_ItemData.amountToCraft.ToString();
            }
        }

        public void CraftItem()
        {
            m_ItemData.CraftingItem();
        }

    }   
}
