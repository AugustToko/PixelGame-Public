using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QInventory
{
    public class ChestDrop : MonoBehaviour
    {
        [SerializeField]
        GameObject chestPrefab;
        [SerializeField]
        List<ItemDrop> itemsToDrop;

        public void DropChest()
        {
            GameObject chestGO = Instantiate(chestPrefab, transform.position, Quaternion.identity);
            ChestLoot m_ChestLoot = chestGO.GetComponent<ChestLoot>();

            if (!m_ChestLoot)
                m_ChestLoot = chestGO.AddComponent<ChestLoot>();

            m_ChestLoot.SetItemsToDrop(itemsToDrop);
        }
    }
}

