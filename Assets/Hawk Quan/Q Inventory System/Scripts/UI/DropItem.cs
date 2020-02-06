using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QInventory
{
    public class DropItem : MonoBehaviour
    {
        [Header("Drop Currency")]
        [SerializeField]
        List<Price> minDropCurrencyAmount;
        [SerializeField]
        List<Price> maxDropCurrencyAmount;
        [SerializeField]

        [Header("Drop Item")]
        List<ItemDrop> itemsToDrop = new List<ItemDrop>();

        public void DropItems()
        {
            for (int i = 0; i < itemsToDrop.Count; i++)
            {
                int chance = Random.Range(0, 100);
                if (chance < itemsToDrop[i].dropChance * 100)
                {
                    int itemDropAmount = Random.Range(itemsToDrop[i].minDropAmount, itemsToDrop[i].maxDropAmount);
                    //Debug.Log(itemDropAmount);
                    for (int j = 0; j < itemDropAmount; j++)
                    {
                        Instantiate(itemsToDrop[i].itemToDrop.m_object, transform.position, Quaternion.identity);
                    }
                }
            }


            RandomCurrencyAmount();
        }

        void RandomCurrencyAmount()
        {
            List<Price> m_AmountToDrop = maxDropCurrencyAmount;
            if (minDropCurrencyAmount.Count == maxDropCurrencyAmount.Count)
            {
                for (int i = 0; i < maxDropCurrencyAmount.Count; i++)
                {
                    m_AmountToDrop[i].amount = Random.Range(minDropCurrencyAmount[i].amount, maxDropCurrencyAmount[i].amount);
                }
            }

            else if (minDropCurrencyAmount.Count < maxDropCurrencyAmount.Count && minDropCurrencyAmount[0].currency != maxDropCurrencyAmount[0].currency)
            {
                int gapLength = maxDropCurrencyAmount.Count - minDropCurrencyAmount.Count;

                for (int i = 0; i < gapLength; i++)
                {
                    m_AmountToDrop[i].amount = Random.Range(0, maxDropCurrencyAmount[i].amount);
                }

                for (int i = gapLength; i < maxDropCurrencyAmount.Count; i++)
                {
                    m_AmountToDrop[i].amount = Random.Range(minDropCurrencyAmount[i - gapLength].amount, maxDropCurrencyAmount[i].amount);
                }
            }

            else
            {
                Debug.Log("Currency Setting Wrong");
                return;
            }

            GameObject CurrencyDropItem = Instantiate(Q_GameMaster.Instance.inventoryManager.m_CurrencyDropItem, transform.position, Quaternion.identity);
            CurrencyDropItem.GetComponent<CurrencyData>().currencyAmounts = m_AmountToDrop;
        }


    }
}


