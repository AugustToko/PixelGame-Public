using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QInventory
{
    [AddComponentMenu("Q Inventory/Crafting")]
    public class Crafting : Q_Inventory
    {
        [Header("Base Setting")]
        public Transform craftingSpawningPosition;
        [SerializeField]
        private Vector3 spawnOffset;
        [Header("Blue Print Setting")]
        //public List<Category> categories;
        //public CraftingBluePrint[] craftingBluePrints;
        private GameObject m_CraftPanel;
        private GameObject m_CraftSlot;
        private GameObject m_CraftItem;
        //private Inventory playerInventory;
        //private List<CoolDown> cds = new List<CoolDown>(); 

        protected override void Start()
        {
            //--------------------分割线----------------------
            //初始化赋值各种参数
            m_CraftPanel = transform.GetChild(0).gameObject;
            m_CraftSlot = Q_GameMaster.Instance.inventoryManager.m_CraftSlot;
            m_CraftItem = Q_GameMaster.Instance.inventoryManager.m_CraftItem;
            //playerInventory = Q_GameMaster.Instance.inventoryManager.playerInventory;

            //--------------------分割线----------------------
            //初始化添加格子
            Transform scrollRect = m_CraftPanel.transform.Find("Scroll Rect");
            //m_Scrollbar = scrollRect.Find("Scrollbar").GetComponent<Scrollbar>();
            //scrollRect.Find("Scrollbar").GetComponent<Scrollbar>().value = 1;//开头将滚动条的值重设为1,好像没用？
            m_GridList = scrollRect.Find("GridList").gameObject;
            for (int i = 0; i < slotAmount; i++)
            {
                items.Add(Item.CreateInstance<Item>());
                GameObject newSlot = Instantiate(m_CraftSlot, m_GridList.transform);
                Slot m_slot = newSlot.GetComponent<Slot>();
                m_slot.slotID = i;
                m_slot.inv = this;
                slots.Add(newSlot);
                cds.Add(new CoolDown());
            }

            //AddBluePrints();

            StartCoroutine(SetFalse());
        }

        private void FixedUpdate()
        {
            CDsCoolDown();
            //ResetScrollBar(m_CraftPanel,m_Scrollbar);
        }

        public ItemData AddItem(CraftingBluePrint craftingBluePrint)
        {
            Item newItem = Q_GameMaster.Instance.inventoryManager.itemDataBase.getItemByID(craftingBluePrint.targetItem.ID);
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].ID == -1)
                {
                    items[i] = newItem;
                    GameObject itemObj = Instantiate(m_CraftItem, slots[i].transform);
                    itemObj.name = newItem.itemName;

                    //Text nameText = itemObj.transform.Find("Name").GetComponent<Text>();
                    //Text priceText = itemObj.transform.Find("Price").GetComponent<Text>();
                    ItemData data = itemObj.GetComponent<ItemData>();//更新数据

                    Transform price = itemObj.transform.Find("Price");
                    List<Text> priceText = new List<Text>();

                    for (int j = 0; j < price.childCount; j++)
                    {
                        priceText.Add(price.GetChild(j).GetChild(0).GetComponent<Text>());
                    }

                    //更新购买信息
                    data.moveAfterCrafting = craftingBluePrint.moveAfterCrafting;
                    for (int j = 0; j < craftingBluePrint.craftingPrices.Count; j++)
                    {
                        data.realPrice += craftingBluePrint.craftingPrices[j].amount * craftingBluePrint.craftingPrices[j].currency.exchangeRate;
                        if (j < priceText.Count)
                            priceText[j].text = craftingBluePrint.craftingPrices[j].amount.ToString();
                    }

                    //更新所在背包状态
                    data.isStorged = false;
                    data.isOnSell = false;
                    data.isOnCrafting = true;

                    data.inv = this;
                    data.m_Crafting = this;

                    //change icon
                    data.icon = itemObj.transform.Find("Icon").GetComponent<Image>();
                    if (!craftingBluePrint.icon)
                        data.icon.sprite = newItem.icon;
                    else
                        data.icon.sprite = craftingBluePrint.icon;


                    data.cd = itemObj.transform.Find("CD").GetComponent<Image>();
                    data.cd.gameObject.SetActive(true);

                    data.item = newItem;
                    data.slot = i;

                    //加inputfield
                    data.inputField = itemObj.transform.Find("Button").Find("InputField").GetComponent<InputField>();

                    //加名字
                    //nameText.text = data.item.itemName;

                    //更新蓝图信息
                    data.m_CraftingBluePrint = craftingBluePrint;

                    //如果是限定合成移除，则只能造一个，禁用inputfield
                    if (craftingBluePrint.moveAfterCrafting)
                    {
                        data.inputField.gameObject.SetActive(false);
                        itemObj.transform.Find("Button").Find("ButtonMin").gameObject.SetActive(false);
                        itemObj.transform.Find("Button").Find("ButtonPlus").gameObject.SetActive(false);
                    }

                    cds[data.slot] = new CoolDown(data.cd, craftingBluePrint.craftingTime);
                    return data;
                }
            }
            return null;

        }

        public bool CraftingItem(ItemData itemdata)
        {
            bool canCrafting = CheckIsHaveEnoughItems(itemdata.m_CraftingBluePrint.ingredients, itemdata.amountToCraft);
            //bool canCrafting = ConsumeIngredients(craftingBluePrint.ingredients,amount);
            if (canCrafting && gameObject.activeSelf)
            {
                StartCoroutine(AddCraftingItem(itemdata));
                return true;
            }

            return false;
        }

        public bool ConsumeIngredients(List<Ingredient> ingredients, int amountToCraft)
        {
            //Debug.Log(CheckIsHaveEnoughItems(ingredients, amountToCraft));
            if (CheckIsHaveEnoughItems(ingredients, amountToCraft))
            {
                //Debug.Log("Have Enough Ingredients");
                CostIngredients(ingredients, amountToCraft);
                return true;
            }

            else
            {
                return false;
            }
        }


        bool CheckIsHaveEnoughItems(List<Ingredient> ingredients, int amountToCraft)
        {
            List<Item> items = Q_GameMaster.Instance.inventoryManager.playerInventory.items;
            foreach (Ingredient m_Ingredient in ingredients)
            {
                int tempAmount = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].ID == m_Ingredient.ingredient.ID)
                    {
                        ItemData data = Q_GameMaster.Instance.inventoryManager.playerInventory.slots[i].transform.GetChild(0).GetComponent<ItemData>();
                        tempAmount += data.amount;
                    }
                }

                //Debug.Log("name" + ingredients[0].ingredient.name + "amount" + tempAmount);

                if (tempAmount < amountToCraft * m_Ingredient.amount)
                {
                    return false;
                }

            }

            return true;
        }

        void CostIngredients(List<Ingredient> ingredients, int amountToCraft)
        {
            foreach (Ingredient m_Ingredient in ingredients)
            {
                List<Item> items = Q_GameMaster.Instance.inventoryManager.playerInventory.items;
                int tempAmountNeed = m_Ingredient.amount * amountToCraft;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].ID == m_Ingredient.ingredient.ID)
                    {
                        ItemData data = Q_GameMaster.Instance.inventoryManager.playerInventory.slots[i].transform.GetChild(0).GetComponent<ItemData>();
                        if (data.amount > tempAmountNeed)
                        {
                            data.amount -= tempAmountNeed;
                            data.amountText.text = data.amount.ToString();
                            break;
                        }

                        else
                        {
                            tempAmountNeed -= data.amount;
                            data.amount = 0;
                            data.ClearItem();
                        }

                        if (tempAmountNeed == 0)
                            break;
                    }
                }
            }
        }

        //void AddBluePrints()
        //{
        //    if (categories.Count > 0)
        //    {
        //        foreach (var bluePrint in Q_GameMaster.Instance.inventoryManager.itemDataBase.bluePrints)
        //        {
        //            foreach (var category in categories)
        //            {
        //                if(bluePrint.category == category)
        //                {
        //                    AddItem(bluePrint);
        //                    break;
        //                }
        //            }
        //        }
        //    }
        //    foreach (CraftingBluePrint m_CraftingBluePrint in craftingBluePrints)
        //    {
        //        AddItem(m_CraftingBluePrint);
        //    }

        //}

        public void ClearAllItems()
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].ID != -1)
                {
                    ItemData data = slots[i].GetComponentInChildren<ItemData>();
                    data.ClearItem();
                }
            }
        }

        IEnumerator SetFalse()
        {
            yield return new WaitForSeconds(0.01f);
            m_CraftPanel.SetActive(false);
        }

        //用于实现制作CD的
        IEnumerator AddCraftingItem(ItemData itemdata)
        {
            int amount = itemdata.amountToCraft;
            CostIngredients(itemdata.m_CraftingBluePrint.ingredients, amount);
            PlayerInventoryManager.UpdatePlayerCurrency(itemdata.m_CraftingBluePrint.craftingPrices, -1 * amount);
            //Debug.Log(amount);
            for (int i = 0; i < amount; i++)
            {
                StartCD(itemdata.slot, itemdata.m_CraftingBluePrint.craftingTime);
                if (itemdata.m_CraftingBluePrint.CDAllWhenCrafting)
                    AllStartCD(itemdata.m_CraftingBluePrint.craftingTime);

                yield return new WaitForSeconds(itemdata.m_CraftingBluePrint.craftingTime);
                int chance = Random.Range(0, 100);
                if (chance < itemdata.m_CraftingBluePrint.successChance * 100)
                {
                    Debug.Log("Crafting Success");
                    //废用的,直接加到玩家背包里，他们说不好
                    //playerInventory.AddItem(id);

                    //现在是在指定合成地点掉落
                    Instantiate(Q_GameMaster.Instance.inventoryManager.itemDataBase.getItemByID(itemdata.item.ID).m_object, craftingSpawningPosition.position + spawnOffset, Quaternion.identity);
                    itemdata.inputField.text = (--itemdata.amountToCraft).ToString();
                }


                else
                {
                    Debug.Log("Crafting Failed");
                }
            }

            itemdata.amountToCraft = 1;
            itemdata.isCrafting = false;
            itemdata.inputField.text = (itemdata.amountToCraft).ToString();

        }
    }
}
