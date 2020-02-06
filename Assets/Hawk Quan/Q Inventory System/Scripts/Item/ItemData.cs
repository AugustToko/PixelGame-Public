using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

namespace QInventory
{
    public class ItemData : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerExitHandler
    {
        public Item item;
        public int amount = 1;

        [HideInInspector]
        public bool isStorged = false;
        [HideInInspector]
        public bool isOnSell = false;
        [HideInInspector]
        public bool isOnCrafting = false;
        [HideInInspector]
        public bool isOnEquipment = false;
        [HideInInspector]
        public bool isCrafting = false;
        [HideInInspector]
        public int amountToCraft;
        [HideInInspector]
        public int amountToBuy;

        //[HideInInspector]
        public int slot;
        [HideInInspector]
        public Image cd;
        [HideInInspector]
        public Text amountText;
        [HideInInspector]
        public Image icon;
        //[HideInInspector]
        public Q_Inventory inv;
        [HideInInspector]
        public InputField inputField;

        //Vendor
        [HideInInspector]
        public float realPrice;
        [HideInInspector]
        public bool moveAfterPurchase = false;

        //Crafting
        [HideInInspector]
        public Crafting m_Crafting;
        [HideInInspector]
        public bool moveAfterCrafting = false;
        [HideInInspector]
        public CraftingBluePrint m_CraftingBluePrint;

        [HideInInspector]
        public int flag;
        private Tooltip tooltip;
        private bool isOnItem = false;

        private void Start()
        {

            tooltip = Q_GameMaster.Instance.inventoryManager.toolTip;
            //默认数量为一
            //amount = 1;
            amountToBuy = 1;
            amountToCraft = 1;
        }

        // 分界线 下面是接口的实现
        // **************************
        // **************************
        // **************************
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (inv.tag != "SkillBar")
            {
                if (!isOnSell)
                {
                    isOnItem = true;
                    StartCoroutine(toolTipHoverCD());
                }

                else if (eventData.pointerCurrentRaycast.gameObject.name == "Icon")
                {
                    isOnItem = true;
                    StartCoroutine(toolTipHoverCD());
                }
            }
            //tooltip.Activate(item);
        }


        //一般点击使用都在这里
        public void OnPointerDown(PointerEventData eventData)
        {
            //不在卖或者存储就能使用
            if (Input.GetKeyDown(Q_InputManager.Instance.useItem))
            {
                UseItem(eventData);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isOnItem = false;
            tooltip.Deactivate();
        }

        //分界线 下面是各种函数
        // **************************
        // **************************
        // **************************

        //穿戴装备

        //使用物品
        public void UseItem(PointerEventData eventData)
        {
            if (!isStorged && !isOnSell && !isOnCrafting && !isOnEquipment)
            {
                if (item.variety == Variety.Consumable)//对应消耗品的使用
                {
                    UseConsumableItem();
                }

                if (item.variety == Variety.Equipment)
                {
                    WearEquipmentItem();
                    //PlayAudioClip
                }
            }

            if (isOnSell && eventData.pointerCurrentRaycast.gameObject.name == "Icon")
            {
                BuyItem();
            }

            if (isOnCrafting && Input.GetKeyDown(Q_InputManager.Instance.useItem))
            {
                CraftingItem();
            }

            if (isOnEquipment && eventData.pointerCurrentRaycast.gameObject.name == "Icon" && eventData.pointerCurrentRaycast.gameObject.tag == "Equipment")
            {
                UnwearItem();
            }
        }

        public void UseItem()
        {
            if (!isStorged && !isOnSell && !isOnCrafting && !isOnEquipment)
            {
                if (item.variety == Variety.Consumable)//对应消耗品的使用
                {
                    UseConsumableItem();
                }

                if (item.variety == Variety.Equipment)
                {
                    WearEquipmentItem();
                    //PlayAudioClip
                }
            }
        }

        public void UseItem(int numberToUse)
        {
            if (!isStorged && !isOnSell && !isOnCrafting && !isOnEquipment)
            {
                if (item.variety == Variety.Consumable)//对应消耗品的使用
                {
                    for (int i = 0; i < numberToUse; i++)
                    {
                        UseConsumableItem();
                    }
                }
            }
        }

        public void ClearItem()
        {
            if (inv && inv.items.Count > 0)
                inv.items[slot] = Item.CreateInstance<Item>();
            if (tooltip)
                tooltip.Deactivate();
            Destroy(gameObject);
            //item = new Item();
            //icon.gameObject.SetActive(false);
            //amountText.gameObject.SetActive(false);
            //cd.gameObject.SetActive(false);
        }


        //购买物品
        public void BuyItem()
        {
            float totalPrice = amountToBuy * realPrice; //计算总价值

            if (totalPrice > Q_GameMaster.Instance.inventoryManager.playerInventoryManager.totalCurrencyValue)//检验钱够不够
            {
                Q_GameMaster.Instance.inventoryManager.SetInformation(Q_GameMaster.Instance.inventoryManager.infoManager.lockMoney);
                Debug.Log("Cant Afford!");
            }

            else if (Q_GameMaster.Instance.inventoryManager.playerInventory.CheckIsFull(item.ID))
            {
                Q_GameMaster.Instance.inventoryManager.SetInformation(Q_GameMaster.Instance.inventoryManager.infoManager.inventoryFull);
                Debug.Log("Player Inventory is Full!");
            }

            else
            {
                //Debug.Log("Buy Item" + item.name + " "+ amountToBuy + "times");

                for (int i = 0; i < amountToBuy; i++)
                {
                    Q_GameMaster.Instance.inventoryManager.playerInventory.AddItem(item.ID);
                }

                Q_GameMaster.Instance.inventoryManager.playerInventoryManager.totalCurrencyValue -= totalPrice;
                //Debug.Log(GameMaster.Instance.playerManager.totalCurrencyValue);
                //Debug.Log("totalPrice: " + totalPrice);
                float tempTotalCurrencyValue = Q_GameMaster.Instance.inventoryManager.playerInventoryManager.totalCurrencyValue;
                //减去货币,按总价值从最高位开始算
                foreach (PlayerCurrency m_PlayerCurrency in Q_GameMaster.Instance.inventoryManager.playerInventoryManager.playerCurrencies)
                {
                    m_PlayerCurrency.amount = ((int)tempTotalCurrencyValue / (int)m_PlayerCurrency.currency.exchangeRate);
                    //Debug.Log(m_PlayerCurrency.currency.currencyName + m_PlayerCurrency.amount);
                    tempTotalCurrencyValue -= m_PlayerCurrency.amount * m_PlayerCurrency.currency.exchangeRate;
                }

                Q_GameMaster.Instance.inventoryManager.PlayMoneyChangeClip();

                //如果是购买后消除的，则消除
                if (moveAfterPurchase)
                {
                    Q_GameMaster.Instance.inventoryManager.activeVendorTrigger.itemsToSell[flag].isMoved = true;
                    ClearItem();
                }
            }

            amountToBuy = 1;
            inputField.text = amountToBuy.ToString();
        }

        //卖东西
        public void SellItem()
        {
            PlayerInventoryManager.UpdatePlayerCurrency(item.sell_Price, amount);

            ClearItem();

        }

        //合成东西
        public void CraftingItem()
        {
            if (inv.ReturnCoolingState(slot))
            {
                Q_GameMaster.Instance.inventoryManager.SetInformation(Q_GameMaster.Instance.inventoryManager.infoManager.cantCrafting);
                return;
            }

            if (m_CraftingBluePrint.CDAllWhenCrafting)
            {
                for (int i = 0; i < inv.items.Count; i++)
                {
                    if (inv.ReturnCoolingState(i))
                    {
                        Q_GameMaster.Instance.inventoryManager.SetInformation(Q_GameMaster.Instance.inventoryManager.infoManager.cantCrafting);
                        Debug.Log("cant crafting");
                        return;
                    }
                }
            }


            if (!isCrafting)
            {
                float tempTotalPrice = 0;
                foreach (Price m_Price in m_CraftingBluePrint.craftingPrices)
                {
                    tempTotalPrice += m_Price.amount * m_Price.currency.exchangeRate;
                }

                if (Q_GameMaster.Instance.inventoryManager.playerInventoryManager.totalCurrencyValue < tempTotalPrice * amountToCraft)
                {
                    Q_GameMaster.Instance.inventoryManager.SetInformation(Q_GameMaster.Instance.inventoryManager.infoManager.lockMoney);
                    Debug.Log("Cant Afford Crafting");
                }
                else
                {
                    bool canCrafting = m_Crafting.CraftingItem(this);
                    if (canCrafting)
                    {
                        //UpdataPlayerCurrency(m_CraftingBluePrint.craftingPrices, -amountToCraft);
                        isCrafting = true;

                        if (m_CraftingBluePrint.moveAfterCrafting)
                        {
                            StartCoroutine(DeleteAfterCrafting(m_CraftingBluePrint.craftingTime));
                        }
                    }
                    else
                    {
                        Q_GameMaster.Instance.inventoryManager.SetInformation(Q_GameMaster.Instance.inventoryManager.infoManager.lockIngredient);
                        //Q_GameMaster.Instance.inventoryManager.informationText.text = "Dont Have Enough Ingredients";
                        //Q_GameMaster.Instance.inventoryManager.informationPanel.SetActive(true);

                        Debug.Log("Don't Have Enough Ingredients");
                    }
                }
            }
        }

        public void UnwearItem()
        {
            foreach (var deletePart in Q_GameMaster.Instance.inventoryManager.equipmentManager.equipmentParts)
            {
                if (deletePart.equipmentPart == item.equipmentPart)
                {
                    if (!deletePart.playerPart)
                        break;

                    if (deletePart.playerPart.childCount > 0)
                        Destroy(deletePart.playerPart.GetChild(0).gameObject);
                    break;
                }
            }
            Q_GameMaster.Instance.inventoryManager.PlayWearClip();
            Q_GameMaster.Instance.inventoryManager.playerInventory.AddItem(item.ID);
            transform.parent.Find("bg").gameObject.SetActive(true);
            Q_GameMaster.Instance.inventoryManager.playerInventoryManager.ClearEquipmentAttributeByItemAttribute(item.equipmentItemAttributes);
            ClearItem();
        }

        public void UnwearItemWithoutAdd()
        {
            foreach (var deletePart in Q_GameMaster.Instance.inventoryManager.equipmentManager.equipmentParts)
            {
                if (deletePart.equipmentPart == item.equipmentPart)
                {
                    if (!deletePart.playerPart)
                        break;

                    if (deletePart.playerPart.childCount > 0)
                        Destroy(deletePart.playerPart.GetChild(0).gameObject);
                    break;
                }
            }
            transform.parent.Find("bg").gameObject.SetActive(true);
            Q_GameMaster.Instance.inventoryManager.playerInventoryManager.ClearEquipmentAttributeByItemAttribute(item.equipmentItemAttributes);
            ClearItem();
        }

        //不知道为什么inputfield不返回正确的值,好吧,自己加个listener解决了
        public void SetAmountToBuy(string amount)
        {
            amountToBuy = Mathf.Clamp(int.Parse(amount), 1, 1000);
        }

        public void SetAmountToCraft(string amount)
        {
            amountToCraft = Mathf.Clamp(int.Parse(amount), 1, 1000);
        }

        public int GetAmountToCraft()
        {
            return amountToCraft;
        }

        public void UpdataText()
        {
            if (amountText)
                amountText.text = amount.ToString();
        }

        //--------------------分割线----------------------
        //使用消耗品
        void UseConsumableItem()
        {
            if (cd && !inv.ReturnCoolingState(slot) && amount > 0)
            {
                PlayAudioClip();
                //这里将来要写传递使用信息，进行相应属性操作
                Q_GameMaster.Instance.inventoryManager.playerInventoryManager.ChangePlayerAttributeByItemAttribute(item.consumableItemAttributes);
                amount--;
                if (amount > 0)
                {
                    if (item.coolDown > 0)
                    {
                        if (item.CDAllConsumbleWhenUse)
                        {
                            inv.CDAllConsumble(item.coolDown);
                        }

                        else if (item.CDAllThisWhenUse)
                        {
                            inv.CDAllThis(item);
                        }
                        inv.StartCD(slot, item.coolDown);
                    }

                    amountText.text = amount.ToString();
                }

                else
                {
                    ClearItem();
                    inv.DeleteCDList(slot);
                }
            }

        }

        public void WearEquipmentItemWithoutChangeAttribute()
        {
            Q_GameMaster.Instance.inventoryManager.equipmentInventory.AddItem(this);

            foreach (var equipment in Q_GameMaster.Instance.inventoryManager.equipmentManager.equipmentParts)
            {
                if (item.equipmentPart == equipment.equipmentPart)
                {
                    if (equipment.playerPart)
                    {
                        ClearItem();
                        SpawnEquipmentItem(equipment.playerPart);
                    }
                    else
                    {
                        ClearItem();
                        Debug.Log("dont Assign the part");
                    }
                    break;
                }
            }

        }

        void WearEquipmentItem()
        {
            Q_GameMaster.Instance.inventoryManager.equipmentInventory.AddItem(this);

            foreach (var equipment in Q_GameMaster.Instance.inventoryManager.equipmentManager.equipmentParts)
            {
                if (item.equipmentPart == equipment.equipmentPart)
                {
                    if (equipment.playerPart)
                    {
                        Q_GameMaster.Instance.inventoryManager.playerInventoryManager.ChangePlayerAttributeByItemAttribute(item.equipmentItemAttributes);
                        ClearItem();
                        SpawnEquipmentItem(equipment.playerPart);
                    }
                    else
                    {
                        Q_GameMaster.Instance.inventoryManager.playerInventoryManager.ChangePlayerAttributeByItemAttribute(item.equipmentItemAttributes);
                        ClearItem();
                        Debug.Log("dont Assign the part");
                    }
                    break;
                }
            }

        }

        void SpawnEquipmentItem(Transform itemParent)
        {
            ReplaceEquipmentItem(itemParent);
            GameObject equipmentItem = Instantiate(item.m_object, itemParent);
            equipmentItem.GetComponent<GameObjectData>().isEquipped = true;
            if (equipmentItem.GetComponent<Collider2D>())
            {
                equipmentItem.GetComponent<Collider2D>().isTrigger = true;
                if (equipmentItem.GetComponent<Rigidbody2D>())
                    equipmentItem.GetComponent<Rigidbody2D>().isKinematic = true;
            }

            else
            {
                equipmentItem.GetComponent<Collider>().isTrigger = true;
                if (equipmentItem.GetComponent<Rigidbody>())
                    equipmentItem.GetComponent<Rigidbody>().isKinematic = true;
            }

            Q_GameMaster.Instance.inventoryManager.PlayWearClip();
        }

        void ReplaceEquipmentItem(Transform itemParent)
        {
            if (itemParent.childCount != 0)
            {
                Transform itemReplaced = itemParent.GetChild(0);
                GameObjectData itemdata = itemReplaced.transform.GetComponent<GameObjectData>();
                Q_GameMaster.Instance.inventoryManager.playerInventoryManager.ClearEquipmentAttributeByItemAttribute(itemdata.item.equipmentItemAttributes);
                inv.AddItem(itemdata.item, slot, amount);
                Destroy(itemReplaced.gameObject);
            }
        }

        private void PlayAudioClip()
        {
            if (item.clipOnUse && Q_GameMaster.Instance.inventoryManager.m_AudioSource)
                for (int i = 0; i < item.playClipTimes; i++)
                {
                    Q_GameMaster.Instance.inventoryManager.m_AudioSource.PlayOneShot(item.clipOnUse);
                }
        }

        //--------------------分割线----------------------
        IEnumerator toolTipHoverCD()
        {
            yield return new WaitForSeconds(tooltip.toolTipHoverTime);
            if (isOnItem)
                tooltip.Activate(item, tag, this);
        }

        IEnumerator DeleteAfterCrafting(float cd)
        {
            yield return new WaitForSeconds(cd);
            inv.GetComponent<Crafting>().DeleteCDList(slot);
            Q_GameMaster.Instance.inventoryManager.activeCraftTrigger.bluePrints[flag].isMoved = true;
            Debug.Log(flag);
            ClearItem();
        }
    }
}
