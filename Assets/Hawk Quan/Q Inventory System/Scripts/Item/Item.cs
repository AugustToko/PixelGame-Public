using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QInventory
{
    [System.Serializable]
    public class Item : ScriptableObject
    {
        public string itemName;
        [Header("Base Setting")]
        public int ID = -1;
        public Sprite icon;
        public GameObject m_object;
        public bool isStackable = false;
        public int maxStackNumber = 1;

        [Header("Base Information")]
        [TextArea(3, 100)]
        public string description;
        [Space(10)]
        public Variety variety;
        public Rarity rarity;
        public List<Price> buy_Price;
        public List<Price> sell_Price;
        //public float weight;

        [Header("Audio Clip")]
        public AudioClip clipOnUse;
        public int playClipTimes;

        [Header("Consumable Item Setting")]
        public float coolDown;
        public ConsumableItemType consumableItemType;
        public List<ConsumableAttribute> consumableItemAttributes = new List<ConsumableAttribute>();
        public bool useOnPickUp = false;
        public bool CDAllThisWhenUse = false;
        public bool CDAllConsumbleWhenUse = false;

        [Header("Equipment Item Setting")]
        public EquipmentPart equipmentPart;
        public List<EquipmentAttribute> equipmentItemAttributes = new List<EquipmentAttribute>();
    }
}
