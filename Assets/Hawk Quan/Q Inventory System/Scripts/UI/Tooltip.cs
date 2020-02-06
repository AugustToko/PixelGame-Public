using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QInventory
{
    public class Tooltip : MonoBehaviour
    {
        [Header("ToolTip")]
        public GameObject tooltip;
        public GameObject gridList;
        public GameObject toolTipText;
        public float toolTipHoverTime;
        [Header("PickUp Tip")]
        public GameObject pickTip;
        public Text pickTipText;
        private Vector2 m_Pivot = new Vector2(-0.1f, 1);
        private Item item;
        //private string data;
        private RectTransform rectTransform;
        private bool structFlag = true;
        private void Start()
        {
            rectTransform = tooltip.GetComponent<RectTransform>();
            //tooltip = GameObject.Find("Tooltip");
        }

        private void Update()
        {
            if (tooltip.activeSelf)
            {
                tooltip.transform.position = Input.mousePosition;
            }
        }

        public void Activate(Item item, string tag, ItemData itemdata)
        {
            this.item = item;
            ConstructDataString(tag, itemdata);
            tooltip.transform.position = Input.mousePosition; //让信息条的移动更加自然
            AdjustPosition();
        }

        public void Deactivate()
        {
            if (tooltip.activeSelf)
                tooltip.SetActive(false);
            structFlag = true;
            //清空text物体
            for (int i = 0; i < gridList.transform.childCount; i++)
            {
                Destroy(gridList.transform.GetChild(i).gameObject);
            }
        }

        public void ConstructDataString(string tag, ItemData itemdata)
        {
            if (!structFlag)
                return;

            GameObject go = null;
            //data = "<color=#00f0ff><b>" + item.itemName + "</b></color>\n" + "Description:  " + item.description + "\n";
            //名字
            go = Instantiate(toolTipText, gridList.transform);
            go.GetComponent<Text>().text = "<color=#00f0ff><b>" + item.itemName + "</b></color>";
            go.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
            //描述
            go = Instantiate(toolTipText, gridList.transform);
            go.GetComponent<Text>().text = "Description:";

            go = Instantiate(toolTipText, gridList.transform);
            go.GetComponent<Text>().text = item.description;

            AddSellPriceInformation();

            if (tag == "Craft")
            {
                AddCraftingInfomation(itemdata);
            }

            else
            {
                if (tag == "Vendor")
                {
                    AddVendorInformation();
                }

                if (item.variety == Variety.Consumable)
                {
                    AddConsumableInfomation();
                }

                else if (item.variety == Variety.Equipment)
                {
                    AddEquipmentInfomation();
                }

                structFlag = false;
            }
        }

        void AddSellPriceInformation()
        {
            GameObject go = null;

            //价格
            //data += "Sell Price:\n";
            go = Instantiate(toolTipText, gridList.transform);
            go.GetComponent<Text>().text = "Sell Price:";

            foreach (var price in item.sell_Price)
            {
                go = Instantiate(toolTipText, gridList.transform);
                go.GetComponent<Text>().text = price.amount + "  " + price.currency.currencyName;
                //data += price.amount + " " + price.currency.currencyName + "\n";
            }
        }

        void AddVendorInformation()
        {

        }

        void AddConsumableInfomation()
        {
            GameObject go = null;
            //data += "<color=#ff0000><b> ConsumableAttributes: </b></color>\n";
            go = Instantiate(toolTipText, gridList.transform);
            go.GetComponent<Text>().text = "<color=#ff0000><b> ConsumableAttributes: </b></color>";

            foreach (ConsumableAttribute m_Attribute in item.consumableItemAttributes)
            {
                //data += "<color=#c0d54f>" + m_Attribute.consumableItemAttribute.attributeName + "</color>  " + m_Attribute.effectType + "  " + m_Attribute.value + "\n";
                go = Instantiate(toolTipText, gridList.transform);
                go.GetComponent<Text>().text = "<color=#c0d54f>" + m_Attribute.consumableItemAttribute.attributeName + "</color>  " + m_Attribute.effectType + "  " + m_Attribute.value;
            }
        }

        void AddEquipmentInfomation()
        {
            GameObject go = null;
            //data += "<color=#ff0000><b>EquipmentAttributes: </b></color>\n";
            go = Instantiate(toolTipText, gridList.transform);
            go.GetComponent<Text>().text = "<color=#ff0000><b>EquipmentAttributes: </b></color>";
            foreach (EquipmentAttribute m_Attribute in item.equipmentItemAttributes)
            {
                //data += "<color=#c0d54f>" + m_Attribute.equipmentItemAttribute.attributeName + "</color>  " + m_Attribute.effectType + "  " + m_Attribute.value + "\n";
                go = Instantiate(toolTipText, gridList.transform);
                go.GetComponent<Text>().text = "<color=#c0d54f>" + m_Attribute.equipmentItemAttribute.attributeName + "</color>  " + m_Attribute.effectType + "  " + m_Attribute.value;
            }
        }

        void AddCraftingInfomation(ItemData itemdata)
        {
            //更新合成的信息
            GameObject go = null;
            //data += "<color=#ff0000><b>Crafting Ingredients</b></color>\n";
            go = Instantiate(toolTipText, gridList.transform);
            go.GetComponent<Text>().text = "<color=#ff0000><b>Crafting Ingredients</b></color>";
            foreach (Ingredient m_Ingredient in itemdata.m_CraftingBluePrint.ingredients)
            {
                //data += m_Ingredient.ingredient.name + "  " + m_Ingredient.amount + "\n";
                go = Instantiate(toolTipText, gridList.transform);
                go.GetComponent<Text>().text = m_Ingredient.ingredient.name + "  " + m_Ingredient.amount;
            }
        }

        void AdjustPosition()
        {
            Vector2 pivot = m_Pivot;
            //Debug.Log(Screen.width);
            //Debug.Log(tooltip.transform.position.x + rectTransform.rect.size.x / 2);
            if ((tooltip.transform.position.x + rectTransform.rect.size.x) > Screen.width)
                pivot.x = 1f;
            //if ((tooltip.transform.position.x - rectTransform.rect.size.x / 2) < 0)
            //    pivot.x = -0.1f;
            if ((tooltip.transform.position.y - rectTransform.rect.size.y) < 0)
            {
                pivot.y = 0;
            }
            rectTransform.pivot = pivot;
            tooltip.SetActive(true);
        }
    }
}
