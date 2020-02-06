using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QInventory
{
    public class EquipmentSlot : MonoBehaviour
    {
        public EquipmentPart equipmentPart;
        public EquipmentInventory inv;

        public void Start()
        {
            inv = Q_GameMaster.Instance.inventoryManager.equipmentInventory;
            inv.equipmentSlot.Add(this);
        }
    }

}