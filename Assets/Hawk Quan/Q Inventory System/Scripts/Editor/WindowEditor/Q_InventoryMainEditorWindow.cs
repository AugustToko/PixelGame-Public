using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditorInternal;
using QInventory;


public class Q_InventoryMainEditorWindow : Q_InventoryEmptyEditorWindow
{
    private static ItemList inventoryItemList;
    private static EditorInfomation editorInfomation;
    private static Q_InventoryMainEditorWindow _window;

    private string[] titleBar =
        {"Main Editor", "Item Editor", "Crafting Editor", "Currency Editor", "Attribute Editor"};

    private string[] bluePrintBar = {"BluePrints", "Categories"};
    private int showWindowInt;

    private int showBluePrintInt;

    //settings
    private int itemViewIndex = 1;
    private int bluePrintViewIndex = 1;
    private int currencyViewIndex = 1;
    private int attributeViewIndex = 1;

    private int categoryViewIndex = 1;
    //ItemEditor

    #region

    private ObjectType m_ObjectType;
    private ItemShownType m_ItemShownType;
    private ReorderableList m_ConsumbleList;
    private SerializedObject m_ConsumbleSerializedObject;

    private ReorderableList m_EquipmentList;
    private SerializedObject m_EquipmentSerializedObject;

    private ReorderableList m_BuyPriceList;
    private SerializedObject m_BuySerializedObject;

    private ReorderableList m_SellPriceList;
    private SerializedObject m_SellSerializedObject;

    #endregion

    //CratfingEditor

    #region

    private ReorderableList m_BluePrintList;
    private SerializedObject m_BluePrintSerializedObject;

    private ReorderableList m_BluePrintBuyPriceList;
    private SerializedObject m_BluePrintBuySerializedObject;

    #endregion

    public static Q_InventoryMainEditorWindow window
    {
        get
        {
            if (_window == null)
                _window = GetWindow<Q_InventoryMainEditorWindow>(false, "Inventory Editor", false);
            return _window;
        }
    }


    [MenuItem("Tools/Hawk Quan/Q Inventory/Inventory Editor")]
    static void OpenQ_InventoryMainEditorWindow()
    {
        _window = GetWindow<Q_InventoryMainEditorWindow>(false, "Inventory Editor", true);
        _window.minSize = new Vector2(600.0f, 400.0f);
        //_window.maxSize = new Vector2(600.0f, 600.0f);
    }

    private void OnEnable()
    {
        if (EditorPrefs.HasKey("ObjectPath"))
        {
            string objectPath = EditorPrefs.GetString("ObjectPath");
            inventoryItemList = AssetDatabase.LoadAssetAtPath(objectPath, typeof(ItemList)) as ItemList;
            editorInfomation =
                AssetDatabase.LoadAssetAtPath(EditorPrefs.GetString("DatabasePath") + "/Editor/EditorInformation.asset",
                    typeof(EditorInfomation)) as EditorInfomation;
            itemViewIndex = 1;
            bluePrintViewIndex = 1;
            currencyViewIndex = 1;
            attributeViewIndex = 1;
            if (inventoryItemList)
                InitializeReorderableList();
            m_ObjectType = ObjectType._2DGameObject;
        }

        if (editorInfomation)
        {
            labelStyle.normal.textColor = editorInfomation.titleColor;
            tipLabelStyle.normal.textColor = editorInfomation.labelColor;
        }
    }

    Vector2 scrollPos;
    Vector2 scrollPos2;
    bool showPosition1 = true;
    bool showPosition2 = true;
    bool showPosition3 = true;
    bool showPosition4 = true;
    bool showPosition5 = false;

    private void OnGUI()
    {
        if (inventoryItemList == null)
        {
            DrawMainEditor();
        }

        else
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            showWindowInt = GUILayout.Toolbar(showWindowInt, titleBar, GUILayout.MinHeight(50));
            if (showWindowInt == 0)
                DrawMainEditor();
            if (showWindowInt == 1)
                DrawItemEditor();
            if (showWindowInt == 2)
                DrawCraftingEditor();
            if (showWindowInt == 3)
                DrawCurrencyEditor();
            if (showWindowInt == 4)
                DrawAttributeEditor();
            EditorGUILayout.EndScrollView();
        }

        showPosition5 = EditorGUILayout.Foldout(showPosition5, "Editor Settings");
        if (showPosition5)
        {
            GUILayout.BeginHorizontal();

            EditorGUI.BeginChangeCheck();
            editorInfomation.titleColor = EditorGUILayout.ColorField("Title Color", editorInfomation.titleColor);
            if (EditorGUI.EndChangeCheck())
            {
                labelStyle.normal.textColor = editorInfomation.titleColor;
            }

            EditorGUI.BeginChangeCheck();
            editorInfomation.labelColor =
                EditorGUILayout.ColorField("Highlight Label Color", editorInfomation.labelColor);
            if (EditorGUI.EndChangeCheck())
            {
                tipLabelStyle.normal.textColor = editorInfomation.labelColor;
            }

            GUILayout.EndHorizontal();
        }

        if (GUI.changed && inventoryItemList && editorInfomation)
        {
            EditorUtility.SetDirty(inventoryItemList);
            EditorUtility.SetDirty(editorInfomation);
        }
    }

    void ClearReorderableList()
    {
        m_ConsumbleList = null;
        m_ConsumbleSerializedObject = null;
        m_EquipmentList = null;
        m_EquipmentSerializedObject = null;
        m_BuyPriceList = null;
        m_BuySerializedObject = null;
        m_SellPriceList = null;
        m_SellSerializedObject = null;

        m_BluePrintList = null;
        m_BluePrintSerializedObject = null;
        m_BluePrintBuyPriceList = null;
        m_BluePrintSerializedObject = null;
    }

    void InitializeReorderableList()
    {
        if (inventoryItemList.itemList != null)
            if (inventoryItemList.itemList.Count > 0)
            {
                Item obj = inventoryItemList.itemList[itemViewIndex - 1];
                m_ConsumbleSerializedObject = new SerializedObject(obj);
                ConsumbleCreateReorderableList();
                ConsumbleSetupReoirderableListHeaderDrawer();
                ConsumbleSetupReorderableListElementDrawer();
                ConsumbleSetupReorderableListOnAddDropdownCallback();

                m_EquipmentSerializedObject = new SerializedObject(obj);
                EquipmentCreateReorderableList();
                EquipmentSetupReoirderableListHeaderDrawer();
                EquipmentSetupReorderableListElementDrawer();
                EquipmentSetupReorderableListOnAddDropdownCallback();

                m_BuySerializedObject = new SerializedObject(obj);
                BuyCreateReorderableList();
                BuySetupReoirderableListHeaderDrawer();
                BuySetupReorderableListElementDrawer();
                BuySetupReorderableListOnAddDropdownCallback();

                m_SellSerializedObject = new SerializedObject(obj);
                SellCreateReorderableList();
                SellSetupReoirderableListHeaderDrawer();
                SellSetupReorderableListElementDrawer();
                SellSetupReorderableListOnAddDropdownCallback();
            }

        if (inventoryItemList.bluePrints != null)
            if (inventoryItemList.bluePrints.Count > 0)
            {
                CraftingBluePrint blueprintobj = inventoryItemList.bluePrints[bluePrintViewIndex - 1];
                m_BluePrintSerializedObject = new SerializedObject(blueprintobj);
                BluePrintCreateReorderableList();
                BluePrintSetupReoirderableListHeaderDrawer();
                BluePrintSetupReorderableListElementDrawer();
                BluePrintSetupReorderableListOnAddDropdownCallback();

                m_BluePrintBuySerializedObject = new SerializedObject(blueprintobj);
                BluePrintBuyCreateReorderableList();
                BluePrintBuySetupReoirderableListHeaderDrawer();
                BluePrintBuySetupReorderableListElementDrawer();
                BluePrintBuySetupReorderableListOnAddDropdownCallback();
            }
    }

    void RefreshEditorProjectWindow()
    {
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }

    //MainEditor

    #region

    void DrawMainEditor()
    {
        GUILayout.Label("Main Editor", labelStyle);

        GUILayout.Space(30f);

        if (GUILayout.Button("Create Database Folder", GUILayout.MinHeight(50f), GUILayout.ExpandWidth(true)))
        {
            CreateFolders();
            //CreateDatabase();
        }

        GUILayout.Space(30f);

        if (inventoryItemList == null)
        {
            if (GUILayout.Button("Create New Item Database", GUILayout.MinHeight(50f), GUILayout.ExpandWidth(true)))
            {
                CreateNewItemList();
            }

            GUILayout.Space(5);
            if (GUILayout.Button("Open Existing Item Database", GUILayout.MinHeight(50f), GUILayout.ExpandWidth(true)))
            {
                OpenItemList();
            }

            GUILayout.Space(50);
            GUILayout.Label("Please Select or Create An Item Database", normalCenterStyle);
        }

        else
        {
            if (GUILayout.Button("Show Item Database in Assets", GUILayout.MinHeight(50f)))
            {
                ShowObjectInAssets(inventoryItemList);
            }

            GUILayout.Space(30f);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Choose Another Database", GUILayout.MinHeight(50f)))
            {
                OpenItemList();
            }

            GUILayout.Space(10f);

            if (GUILayout.Button("Create A New Database", GUILayout.MinHeight(50f)))
            {
                CreateNewItemList();
            }

            GUILayout.EndHorizontal();
        }

        if (GUI.changed)
        {
            if (inventoryItemList)
                EditorUtility.SetDirty(inventoryItemList);
        }
    }

    void CreateFolders()
    {
        string basePath = Application.dataPath;
        string absPath = EditorUtility.OpenFolderPanel("Select A Folder To Place Inventory Folders", "", "");
        if (absPath.StartsWith(Application.dataPath))
        {
            string relPath = absPath.Substring(Application.dataPath.Length - "Assets".Length);
            basePath = relPath;
        }

        //Inventory Database
        if (!Directory.Exists(basePath + "/" + "Inventory Database"))
        {
            Debug.Log("Success Create Inventory Database");
            AssetDatabase.CreateFolder(basePath, "Inventory Database");
            EditorPrefs.SetString("DatabasePath", basePath + "/Inventory Database");
        }

        else
            Debug.Log("Inventory Database Already Exists");


        //BluePrints
        if (!Directory.Exists(basePath + "/" + "Inventory Database" + "/" + "BluePrints"))
        {
            Debug.Log("Success Create BluePrints");
            AssetDatabase.CreateFolder(basePath + "/Inventory Database", "BluePrints");
        }
        else
            Debug.Log("BluePrints Already Exists");


        //ItemAttributes
        if (!Directory.Exists(basePath + "/" + "Inventory Database" + "/" + "ItemAttributes"))
        {
            Debug.Log("Success Create ItemAttributes");
            AssetDatabase.CreateFolder(basePath + "/Inventory Database", "ItemAttributes");
        }
        else
            Debug.Log("ItemAttributes Already Exists");


        //Currencies
        if (!Directory.Exists(basePath + "/" + "Inventory Database" + "/" + "Currencies"))
        {
            Debug.Log("Success Create Currencies");
            AssetDatabase.CreateFolder(basePath + "/Inventory Database", "Currencies");
        }
        else
            Debug.Log("Currencies Already Exists");


        //Items
        if (!Directory.Exists(basePath + "/" + "Inventory Database" + "/" + "Items"))
        {
            Debug.Log("Success Create Items");
            AssetDatabase.CreateFolder(basePath + "/Inventory Database", "Items");
        }
        else
            Debug.Log("Items Already Exists");

        if (!Directory.Exists(basePath + "/" + "Inventory Database" + "/Items/" + "ItemScriptObjects"))
        {
            Debug.Log("Success Create ItemScriptObjects");
            AssetDatabase.CreateFolder(basePath + "/Inventory Database/Items", "ItemScriptObjects");
        }
        else
            Debug.Log("ItemScriptObjects Already Exists");

        //Items
        if (!Directory.Exists(basePath + "/" + "Inventory Database" + "/" + "Editor"))
        {
            Debug.Log("Success Create Editor");
            AssetDatabase.CreateFolder(basePath + "/Inventory Database", "Editor");
            CreateEditorInformation.Create();
        }
        else
            Debug.Log("Editor Already Exists");

        //Category
        if (!Directory.Exists(basePath + "/" + "Inventory Database" + "/" + "BluePrints" + "/" + "Categories"))
        {
            Debug.Log("Success Create Categories");
            AssetDatabase.CreateFolder(basePath + "/Inventory Database/BluePrints", "Categories");
        }
        else
            Debug.Log("Categories Already Exists");

        //Save
        if (!Directory.Exists(basePath + "/" + "Inventory Database" + "/" + "Save"))
        {
            Debug.Log("Success Create Save");
            AssetDatabase.CreateFolder(basePath + "/Inventory Database", "Save");
        }
        else
            Debug.Log("Save Already Exists");
    }

    void CreateNewItemList()
    {
        inventoryItemList = CreateItemDatabase.Create();
        if (inventoryItemList)
        {
            inventoryItemList.itemList = new List<Item>();
            string relPath = AssetDatabase.GetAssetPath(inventoryItemList);
            EditorPrefs.SetString("ObjectPath", relPath);
        }

        window.Repaint();
    }

    void OpenItemList()
    {
        string absPath = EditorUtility.OpenFilePanel("Select Inventory Item List", "", "");
        if (absPath.StartsWith(Application.dataPath))
        {
            string relPath = absPath.Substring(Application.dataPath.Length - "Assets".Length);
            inventoryItemList = AssetDatabase.LoadAssetAtPath(relPath, typeof(ItemList)) as ItemList;
            if (inventoryItemList.itemList == null)
                inventoryItemList.itemList = new List<Item>();
            if (inventoryItemList.bluePrints == null)
                inventoryItemList.bluePrints = new List<CraftingBluePrint>();
            if (inventoryItemList.currencies == null)
                inventoryItemList.currencies = new List<Currency>();
            if (inventoryItemList.attributes == null)
                inventoryItemList.attributes = new List<ItemAttribute>();

            if (inventoryItemList)
            {
                EditorPrefs.SetString("ObjectPath", relPath);
                EditorPrefs.SetString("DatabasePath",
                    relPath.Substring(0, relPath.Length - (inventoryItemList.name.Length + 1 + ".asset".Length)));
            }
        }
    }

    #endregion

    //ItemEditor

    #region

    void DrawItemEditor()
    {
        GUILayout.Label("Item Editor", labelStyle);

        GUILayout.Space(20);
        m_ObjectType = (ObjectType) EditorGUILayout.EnumPopup("Object Creating Type", m_ObjectType);
        GUILayout.Space(10);
        if (m_ObjectType == ObjectType._2DGameObject)
        {
            editorInfomation.AutoChangeSprite =
                EditorGUILayout.Toggle("Auto Change Sprite", editorInfomation.AutoChangeSprite);
            GUILayout.Space(10);
            editorInfomation.createRigidbody2D =
                EditorGUILayout.Toggle("Create Rigidbody 2D", editorInfomation.createRigidbody2D);
            GUILayout.Space(10);
        }

        m_ItemShownType = (ItemShownType) EditorGUILayout.EnumPopup("Show Item of Type", m_ItemShownType);
        GUILayout.Space(10);

        if (inventoryItemList != null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            if (inventoryItemList.itemList.Count > 0)
            {
                if (GUILayout.Button("Prev", GUILayout.ExpandWidth(false)))
                {
                    ClearReorderableList();
                    if (m_ItemShownType == ItemShownType.All)
                    {
                        if (itemViewIndex > 1)
                            itemViewIndex--;
                        else if (itemViewIndex == 1)
                            itemViewIndex = inventoryItemList.itemList.Count;
                    }

                    else
                    {
                        for (int i = 0; i < inventoryItemList.itemList.Count; i++)
                        {
                            if (itemViewIndex > 1)
                                itemViewIndex--;
                            else if (itemViewIndex == 1)
                                itemViewIndex = inventoryItemList.itemList.Count;

                            if (inventoryItemList.itemList[itemViewIndex - 1].variety.ToString() ==
                                m_ItemShownType.ToString())
                                break;
                        }
                    }

                    InitializeReorderableList();
                }

                GUILayout.Space(5);

                if (GUILayout.Button("Next", GUILayout.ExpandWidth(false)))
                {
                    ClearReorderableList();
                    if (m_ItemShownType == ItemShownType.All)
                    {
                        if (itemViewIndex < inventoryItemList.itemList.Count)
                        {
                            itemViewIndex++;
                        }

                        else if (itemViewIndex == inventoryItemList.itemList.Count)
                        {
                            itemViewIndex = 1;
                        }
                    }

                    else
                    {
                        for (int i = 0; i < inventoryItemList.itemList.Count; i++)
                        {
                            if (itemViewIndex < inventoryItemList.itemList.Count)
                            {
                                itemViewIndex++;
                            }

                            else if (itemViewIndex == inventoryItemList.itemList.Count)
                            {
                                itemViewIndex = 1;
                            }

                            if (inventoryItemList.itemList[itemViewIndex - 1].variety.ToString() ==
                                m_ItemShownType.ToString())
                                break;
                        }
                    }

                    InitializeReorderableList();
                }
            }

            GUILayout.Space(180);

            if (GUILayout.Button("Add Item", GUILayout.ExpandWidth(false)))
            {
                AddItem();
            }

            GUILayout.Space(5);

            if (inventoryItemList.itemList.Count > 0)
            {
                if (GUILayout.Button("Delete Item", GUILayout.ExpandWidth(false)))
                {
                    DeleteItem(itemViewIndex - 1);
                }
            }

            GUILayout.Space(10);

            if (inventoryItemList.itemList.Count > 0)
            {
                if (GUILayout.Button("Reorder Items", GUILayout.ExpandWidth(false)))
                {
                    Q_ReorderItemWindow.OpenQ_ReorderItemWindow();
                }
            }

            GUILayout.EndHorizontal();

            //...........
            //if (inventoryItemList.itemList == null)
            //    Debug.Log("Dont have item");

            if (inventoryItemList.itemList.Count > 0)
            {
                GUILayout.Space(5);
                GUILayout.BeginHorizontal();

                itemViewIndex =
                    Mathf.Clamp(EditorGUILayout.IntField("Current Item", itemViewIndex, GUILayout.ExpandWidth(false)),
                        1, inventoryItemList.itemList.Count);
                EditorGUILayout.LabelField("of   " + inventoryItemList.itemList.Count.ToString() + "  items", "",
                    GUILayout.ExpandWidth(false));
                GUILayout.EndHorizontal();
                GUILayout.Space(5);

                if (m_ItemShownType != ItemShownType.All)
                {
                    if (((int) m_ItemShownType - 1) != (int) inventoryItemList.itemList[itemViewIndex - 1].variety)
                    {
                        for (int i = 0; i < inventoryItemList.itemList.Count; i++)
                        {
                            if ((int) inventoryItemList.itemList[i].variety == ((int) m_ItemShownType - 1))
                            {
                                itemViewIndex = i + 1;
                                break;
                            }
                        }
                    }
                }

                if (GUILayout.Button("Show Item In Assets", GUILayout.ExpandWidth(false)) &&
                    inventoryItemList.itemList[itemViewIndex - 1].m_object)
                {
                    ShowObjectInAssets(inventoryItemList.itemList[itemViewIndex - 1].m_object);
                }

                GUILayout.Space(10);

                showPosition4 = EditorGUILayout.Foldout(showPosition4, "Normal Settings");
                if (showPosition4)
                {
                    GUILayout.Space(5);
                    GUILayout.Label("Normal Settings", tipLabelStyle);
                    GUILayout.Space(5);
                    //下面是各种物品数据的显示
                    //ID
                    GUILayout.Label("Item ID:                         " +
                                    inventoryItemList.itemList[itemViewIndex - 1].ID.ToString());

                    //Name
                    EditorGUI.BeginChangeCheck();
                    inventoryItemList.itemList[itemViewIndex - 1].itemName =
                        EditorGUILayout.DelayedTextField("Item Name",
                            inventoryItemList.itemList[itemViewIndex - 1].itemName as string);
                    if (EditorGUI.EndChangeCheck())
                    {
                        AssetDatabase.RenameAsset(
                            EditorPrefs.GetString("DatabasePath") + "/Items/" +
                            inventoryItemList.itemList[itemViewIndex - 1].m_object.name + ".prefab",
                            inventoryItemList.itemList[itemViewIndex - 1].itemName);
                        AssetDatabase.RenameAsset(
                            EditorPrefs.GetString("DatabasePath") + "/Items/ItemScriptObjects/" +
                            inventoryItemList.itemList[itemViewIndex - 1].name + ".asset",
                            inventoryItemList.itemList[itemViewIndex - 1].itemName);
                        AssetDatabase.SaveAssets();
                    }

                    GUILayout.Space(5);

                    //Description
                    GUILayout.Label("Item Description");
                    GUILayout.Space(5);
                    scrollPos2 = EditorGUILayout.BeginScrollView(scrollPos2, GUILayout.Height(100));
                    inventoryItemList.itemList[itemViewIndex - 1].description = EditorGUILayout.TextArea(
                        inventoryItemList.itemList[itemViewIndex - 1].description, GUILayout.ExpandHeight(true));
                    EditorGUILayout.EndScrollView();
                    GUILayout.Space(5);

                    //icon
                    EditorGUI.BeginChangeCheck();
                    inventoryItemList.itemList[itemViewIndex - 1].icon = EditorGUILayout.ObjectField("Item Icon",
                        inventoryItemList.itemList[itemViewIndex - 1].icon, typeof(Sprite), false) as Sprite;
                    if (EditorGUI.EndChangeCheck())
                    {
                        if (editorInfomation.AutoChangeSprite)
                            if (inventoryItemList.itemList[itemViewIndex - 1].m_object && inventoryItemList
                                    .itemList[itemViewIndex - 1].m_object.GetComponent<SpriteRenderer>())
                            {
                                DestroyImmediate(
                                    inventoryItemList.itemList[itemViewIndex - 1].m_object
                                        .GetComponent<PolygonCollider2D>(), true);
                                inventoryItemList.itemList[itemViewIndex - 1].m_object.GetComponent<SpriteRenderer>()
                                    .sprite = inventoryItemList.itemList[itemViewIndex - 1].icon;
                                inventoryItemList.itemList[itemViewIndex - 1].m_object
                                    .AddComponent<PolygonCollider2D>();
                                RefreshEditorProjectWindow();
                            }
                    }

                    GUILayout.BeginHorizontal();
                    inventoryItemList.itemList[itemViewIndex - 1].isStackable = EditorGUILayout.Toggle("Is Stackable",
                        inventoryItemList.itemList[itemViewIndex - 1].isStackable, GUILayout.ExpandWidth(false));
                    GUILayout.EndHorizontal();
                    GUILayout.Space(5);
                    if (inventoryItemList.itemList[itemViewIndex - 1].isStackable)
                    {
                        inventoryItemList.itemList[itemViewIndex - 1].maxStackNumber = Mathf.Clamp(
                            (int) EditorGUILayout.IntField("maxStackNumber",
                                inventoryItemList.itemList[itemViewIndex - 1].maxStackNumber), 1, 999);
                        GUILayout.Space(10);
                    }

                    inventoryItemList.itemList[itemViewIndex - 1].rarity =
                        (Rarity) EditorGUILayout.EnumPopup("Item Rarity",
                            inventoryItemList.itemList[itemViewIndex - 1].rarity);

                    GUILayout.Space(10);

                    inventoryItemList.itemList[itemViewIndex - 1].variety =
                        (Variety) EditorGUILayout.EnumPopup("Item Variety",
                            inventoryItemList.itemList[itemViewIndex - 1].variety);
                }

                GUILayout.Space(5);
                if (inventoryItemList.itemList[itemViewIndex - 1].variety == Variety.Consumable)
                {
                    GUILayout.Space(10);
                    showPosition3 = EditorGUILayout.Foldout(showPosition3, "Consumable Settings");
                    if (showPosition3)
                    {
                        GUILayout.Space(5);
                        GUILayout.Label("Consumable Settings", tipLabelStyle);
                        GUILayout.Space(5);

                        //inventoryItemList.itemList[itemViewIndex - 1].consumableItemType = (ConsumableItemType)EditorGUILayout.EnumPopup("Consumable Item Type", inventoryItemList.itemList[itemViewIndex - 1].consumableItemType);

                        //GUILayout.Space(5);
                        inventoryItemList.itemList[itemViewIndex - 1].coolDown = Mathf.Clamp(
                            (float) EditorGUILayout.FloatField("Cool Down",
                                inventoryItemList.itemList[itemViewIndex - 1].coolDown), 0, 99999999);
                        if (inventoryItemList.itemList[itemViewIndex - 1].coolDown == 0)
                        {
                            inventoryItemList.itemList[itemViewIndex - 1].useOnPickUp =
                                EditorGUILayout.Toggle("Use Immediately When picking up",
                                    inventoryItemList.itemList[itemViewIndex - 1].useOnPickUp,
                                    GUILayout.ExpandWidth(false));
                        }

                        if (inventoryItemList.itemList[itemViewIndex - 1].coolDown > 0)
                        {
                            inventoryItemList.itemList[itemViewIndex - 1].CDAllThisWhenUse =
                                EditorGUILayout.Toggle("CD All This When Use",
                                    inventoryItemList.itemList[itemViewIndex - 1].CDAllThisWhenUse,
                                    GUILayout.ExpandWidth(false));
                            inventoryItemList.itemList[itemViewIndex - 1].CDAllConsumbleWhenUse =
                                EditorGUILayout.Toggle("CD All Consumble When Use",
                                    inventoryItemList.itemList[itemViewIndex - 1].CDAllConsumbleWhenUse,
                                    GUILayout.ExpandWidth(false));
                        }

                        //
                        GUILayout.Space(5);
                        if (m_ConsumbleList != null && m_ConsumbleSerializedObject != null)
                        {
                            m_ConsumbleSerializedObject.ApplyModifiedProperties();
                            m_ConsumbleSerializedObject.Update();
                            m_ConsumbleList.DoLayoutList();
                            m_ConsumbleSerializedObject.ApplyModifiedProperties();
                            GUILayout.Space(10);
                        }

                        if (inventoryItemList.itemList[itemViewIndex - 1].variety == Variety.Equipment ||
                            inventoryItemList.itemList[itemViewIndex - 1].variety == Variety.Consumable)
                        {
                            inventoryItemList.itemList[itemViewIndex - 1].clipOnUse =
                                EditorGUILayout.ObjectField("Clip",
                                    inventoryItemList.itemList[itemViewIndex - 1].clipOnUse, typeof(AudioClip),
                                    false) as AudioClip;
                            GUILayout.Space(5);
                            if (inventoryItemList.itemList[itemViewIndex - 1].clipOnUse)
                            {
                                inventoryItemList.itemList[itemViewIndex - 1].playClipTimes =
                                    Mathf.Clamp(
                                        (int) EditorGUILayout.IntField("Clip Play Times",
                                            inventoryItemList.itemList[itemViewIndex - 1].playClipTimes), 1, 99);
                            }
                        }
                    }
                }
                else if (inventoryItemList.itemList[itemViewIndex - 1].variety == Variety.Equipment)
                {
                    GUILayout.Space(10);
                    showPosition3 = EditorGUILayout.Foldout(showPosition3, "Equipment Settings");
                    if (showPosition3)
                    {
                        GUILayout.Space(5);
                        GUILayout.Label("Equipment Settings", tipLabelStyle);
                        GUILayout.Space(5);
                        inventoryItemList.itemList[itemViewIndex - 1].equipmentPart =
                            (EquipmentPart) EditorGUILayout.EnumPopup("Equipment Part",
                                inventoryItemList.itemList[itemViewIndex - 1].equipmentPart);
                        GUILayout.Space(5);
                        //
                        if (m_EquipmentList != null && m_EquipmentSerializedObject != null)
                        {
                            m_EquipmentSerializedObject.ApplyModifiedProperties();
                            m_EquipmentSerializedObject.Update();
                            m_EquipmentList.DoLayoutList();
                            m_EquipmentSerializedObject.ApplyModifiedProperties();
                            GUILayout.Space(10);
                        }
                    }
                }

                GUILayout.Space(20);
                showPosition1 = EditorGUILayout.Foldout(showPosition1, "GameObject Settings");
                if (showPosition1)
                {
                    GUILayout.Space(5);
                    GUILayout.Label("GameObject Settings", tipLabelStyle);
                    GUILayout.Space(5);
                    //gameobject
                    inventoryItemList.itemList[itemViewIndex - 1].m_object = EditorGUILayout.ObjectField("Item Object",
                        inventoryItemList.itemList[itemViewIndex - 1].m_object, typeof(GameObject),
                        false) as GameObject;
                    GUILayout.Space(10);

                    if (inventoryItemList.itemList[itemViewIndex - 1] != null &&
                        inventoryItemList.itemList[itemViewIndex - 1].m_object)
                    {
                        inventoryItemList.itemList[itemViewIndex - 1].m_object.layer =
                            EditorGUILayout.LayerField("GameObject Layer",
                                inventoryItemList.itemList[itemViewIndex - 1].m_object.layer);
                        GUILayout.Space(10);
                        inventoryItemList.itemList[itemViewIndex - 1].m_object.tag =
                            EditorGUILayout.TagField("GameObject Tag",
                                inventoryItemList.itemList[itemViewIndex - 1].m_object.tag);
                        GUILayout.Space(10);

                        MeasureGameObjectData();
                    }
                }

                GUILayout.Space(20);
                showPosition2 = EditorGUILayout.Foldout(showPosition2, "Price Settings");
                if (showPosition2)
                {
                    GUILayout.Space(5);
                    GUILayout.Label("Price Settings", tipLabelStyle);
                    GUILayout.Space(5);

                    GUILayout.Label("Buy Price");
                    GUILayout.Space(5);

                    if (m_BuyPriceList != null && m_BuySerializedObject != null)
                    {
                        m_BuySerializedObject.ApplyModifiedProperties();
                        m_BuySerializedObject.Update();
                        m_BuyPriceList.DoLayoutList();
                        m_BuySerializedObject.ApplyModifiedProperties();
                    }

                    GUILayout.Label("Sell Price");
                    GUILayout.Space(5);

                    if (m_SellPriceList != null && m_SellSerializedObject != null)
                    {
                        m_SellSerializedObject.ApplyModifiedProperties();
                        m_SellSerializedObject.Update();
                        m_SellPriceList.DoLayoutList();
                        m_SellSerializedObject.ApplyModifiedProperties();
                    }
                }


                GUILayout.Space(10);
                GUILayout.Space(20);
            }
            else
            {
                GUILayout.Space(50);
                GUILayout.Label("This Inventory List is Empty.", normalCenterStyle);
            }
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(inventoryItemList);
            if (itemViewIndex > 0 && inventoryItemList.itemList.Count > 0)
                EditorUtility.SetDirty(inventoryItemList.itemList[itemViewIndex - 1]);
        }
    }

    void AddItem()
    {
        int num = 1;
        //创建物体
        while (System.IO.File.Exists(EditorPrefs.GetString("DatabasePath") + "/Items" + "/New Item" + "(" +
                                     num.ToString() + ")" + ".prefab"))
            num++;

        Item newItem = CreateItem.Create();
        //newItem.itemName = "New Item" + "("+ num.ToString() + ")";
        newItem.ID = inventoryItemList.itemList.Count + 1;
        inventoryItemList.itemList.Add(newItem);
        itemViewIndex = inventoryItemList.itemList.Count;

        if (m_ObjectType == ObjectType.EmptyObject)
        {
            PrefabUtility.CreateEmptyPrefab(EditorPrefs.GetString("DatabasePath") + "/Items" + "/New Item" + "(" +
                                            num.ToString() + ")" + ".prefab");
            //newItem.m_object = (GameObject)newPrefab;
        }

        else
        {
            GameObject go = new GameObject();
            GameObject newPrefab = PrefabUtility.CreatePrefab(
                EditorPrefs.GetString("DatabasePath") + "/Items" + "/New Item" + "(" + num.ToString() + ")" + ".prefab",
                go);
            DestroyImmediate(go);

            if (m_ObjectType == ObjectType._2DGameObject)
            {
                newPrefab.AddComponent<SpriteRenderer>();
                newPrefab.AddComponent<PolygonCollider2D>();
                if (editorInfomation.createRigidbody2D)
                    newPrefab.AddComponent<Rigidbody2D>();
            }
            //3D的暂时先不支持
            //else if(m_ObjectType == ObjectType._3DGameObject)
            //{
            //    newPrefab.AddComponent<MeshRenderer>();
            //    newPrefab.AddComponent<BoxCollider>();
            //    newPrefab.AddComponent<Rigidbody>();
            //}

            //newPrefab.layer = LayerMask.NameToLayer("PickUps");
            GameObjectData newGameObjectData = newPrefab.AddComponent<GameObjectData>();
            newGameObjectData.item = newItem;
            //inventoryItemList.gameObjectData.Add(newGameObjectData);
            newItem.m_object = newPrefab;
        }

        ClearReorderableList();
        InitializeReorderableList();

        RefreshEditorProjectWindow();
        window.Repaint();
    }

    void DeleteItem(int index)
    {
        if (inventoryItemList.itemList.Count > 0)
        {
            //删除物体
            if (inventoryItemList.itemList[index].m_object)
            {
                string objName = inventoryItemList.itemList[index].m_object.name;
                System.IO.File.Delete(EditorPrefs.GetString("DatabasePath") + "/Items/" + objName + ".prefab");
                System.IO.File.Delete(EditorPrefs.GetString("DatabasePath") + "/Items/" + objName + ".prefab.meta");
            }

            string assetName = inventoryItemList.itemList[index].name;
            System.IO.File.Delete(EditorPrefs.GetString("DatabasePath") + "/Items/ItemScriptObjects/" + assetName +
                                  ".asset");
            System.IO.File.Delete(EditorPrefs.GetString("DatabasePath") + "/Items/ItemScriptObjects/" + assetName +
                                  ".asset.meta");

            inventoryItemList.itemList.RemoveAt(index);
            for (int i = 0; i < inventoryItemList.itemList.Count; i++)
            {
                inventoryItemList.itemList[i].ID = i + 1;
            }

            ClearReorderableList();

            if (itemViewIndex > 1)
            {
                itemViewIndex--;
                InitializeReorderableList();
            }

            else
            {
                itemViewIndex--;
            }
        }

        RefreshEditorProjectWindow();
        window.Repaint();
    }

    void MeasureGameObjectData()
    {
        GameObjectData measureData =
            inventoryItemList.itemList[itemViewIndex - 1].m_object.GetComponent<GameObjectData>();
        if (measureData == null)
        {
            measureData = inventoryItemList.itemList[itemViewIndex - 1].m_object.AddComponent<GameObjectData>();
            measureData.item = inventoryItemList.itemList[itemViewIndex - 1];
            RefreshEditorProjectWindow();
        }

        else if (measureData.item != inventoryItemList.itemList[itemViewIndex - 1])
        {
            measureData.item = inventoryItemList.itemList[itemViewIndex - 1];
            RefreshEditorProjectWindow();
        }
    }

    void ConsumbleCreateReorderableList()
    {
        m_ConsumbleList = new ReorderableList(
            m_ConsumbleSerializedObject,
            m_ConsumbleSerializedObject.FindProperty("consumableItemAttributes"),
            true, true, true, true);
    }

    void ConsumbleSetupReoirderableListHeaderDrawer()
    {
        m_ConsumbleList.drawHeaderCallback =
            (Rect rect) =>
            {
                EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width - 60, rect.height), "Attribute");
                EditorGUI.LabelField(new Rect(rect.x + rect.width / 2, rect.y, 60, rect.height), "Type");
                EditorGUI.LabelField(new Rect(rect.x + rect.width / 2 + 75, rect.y, 60, rect.height), "Value");
            };
    }

    void ConsumbleSetupReorderableListElementDrawer()
    {
        m_ConsumbleList.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = m_ConsumbleList.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;

                float delayWidth = 60;
                float nameWidth = rect.width - delayWidth;

                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y, nameWidth / 2, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("consumableItemAttribute"), GUIContent.none);

                EditorGUI.PropertyField(
                    new Rect(rect.x + rect.width / 2 - 10, rect.y, delayWidth, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("effectType"), GUIContent.none);

                EditorGUI.PropertyField(
                    new Rect(rect.x + rect.width / 2 + 70, rect.y, rect.width / 3, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("value"), GUIContent.none);
            };
    }

    void ConsumbleSetupReorderableListOnAddDropdownCallback()
    {
        m_ConsumbleList.onAddDropdownCallback =
            (Rect buttonRect, ReorderableList l) =>
            {
                if (inventoryItemList.attributes == null || inventoryItemList.attributes.Count == 0)
                {
                    EditorApplication.Beep();
                    EditorUtility.DisplayDialog("Error", "You don't have any attribute defined in the Database", "Ok");
                    return;
                }

                var menu = new GenericMenu();

                foreach (ItemAttribute state in inventoryItemList.attributes)
                {
                    menu.AddItem(new GUIContent(state.attributeName),
                        false,
                        ConsumbleOnReorderableListAddDropdownClick,
                        state);
                }

                menu.ShowAsContext();
            };
    }

    void ConsumbleOnReorderableListAddDropdownClick(object target)
    {
        ItemAttribute state = (ItemAttribute) target;

        int index = m_ConsumbleList.serializedProperty.arraySize;
        m_ConsumbleList.serializedProperty.arraySize++;
        m_ConsumbleList.index = index;

        SerializedProperty element = m_ConsumbleList.serializedProperty.GetArrayElementAtIndex(index);
        element.FindPropertyRelative("consumableItemAttribute").objectReferenceValue = state;
        element.FindPropertyRelative("effectType").enumValueIndex = 0;
        element.FindPropertyRelative("value").floatValue = 0f;

        m_ConsumbleSerializedObject.ApplyModifiedProperties();
    }


    //...............................
    void EquipmentCreateReorderableList()
    {
        m_EquipmentList = new ReorderableList(
            m_EquipmentSerializedObject,
            m_EquipmentSerializedObject.FindProperty("equipmentItemAttributes"),
            true, true, true, true);
    }

    void EquipmentSetupReoirderableListHeaderDrawer()
    {
        m_EquipmentList.drawHeaderCallback =
            (Rect rect) =>
            {
                EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width - 60, rect.height), "Attribute");
                EditorGUI.LabelField(new Rect(rect.x + rect.width / 2, rect.y, 60, rect.height), "Type");
                EditorGUI.LabelField(new Rect(rect.x + rect.width / 2 + 75, rect.y, 60, rect.height), "Value");
            };
    }

    void EquipmentSetupReorderableListElementDrawer()
    {
        m_EquipmentList.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = m_EquipmentList.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;

                float delayWidth = 60;
                float nameWidth = rect.width - delayWidth;

                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y, nameWidth / 2, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("equipmentItemAttribute"), GUIContent.none);

                EditorGUI.PropertyField(
                    new Rect(rect.x + rect.width / 2 - 10, rect.y, delayWidth, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("effectType"), GUIContent.none);

                EditorGUI.PropertyField(
                    new Rect(rect.x + rect.width / 2 + 70, rect.y, rect.width / 3, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("value"), GUIContent.none);
            };
    }

    void EquipmentSetupReorderableListOnAddDropdownCallback()
    {
        m_EquipmentList.onAddDropdownCallback =
            (Rect buttonRect, ReorderableList l) =>
            {
                if (inventoryItemList.attributes == null || inventoryItemList.attributes.Count == 0)
                {
                    EditorApplication.Beep();
                    EditorUtility.DisplayDialog("Error", "You don't have any attribute defined in the Database", "Ok");
                    return;
                }

                var menu = new GenericMenu();

                foreach (ItemAttribute state in inventoryItemList.attributes)
                {
                    menu.AddItem(new GUIContent(state.attributeName),
                        false,
                        EquipmentOnReorderableListAddDropdownClick,
                        state);
                }

                menu.ShowAsContext();
            };
    }

    void EquipmentOnReorderableListAddDropdownClick(object target)
    {
        ItemAttribute state = (ItemAttribute) target;

        int index = m_EquipmentList.serializedProperty.arraySize;
        m_EquipmentList.serializedProperty.arraySize++;
        m_EquipmentList.index = index;

        SerializedProperty element = m_EquipmentList.serializedProperty.GetArrayElementAtIndex(index);
        element.FindPropertyRelative("equipmentItemAttribute").objectReferenceValue = state;
        element.FindPropertyRelative("effectType").enumValueIndex = 0;
        element.FindPropertyRelative("value").floatValue = 0f;

        m_ConsumbleSerializedObject.ApplyModifiedProperties();
    }
    //................................


    void BuyCreateReorderableList()
    {
        m_BuyPriceList = new ReorderableList(
            m_BuySerializedObject,
            m_BuySerializedObject.FindProperty("buy_Price"),
            true, true, true, true);
    }

    void BuySetupReoirderableListHeaderDrawer()
    {
        m_BuyPriceList.drawHeaderCallback =
            (Rect rect) =>
            {
                EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width - 60, rect.height), "Currency");
                EditorGUI.LabelField(new Rect(rect.x + rect.width / 2, rect.y, 60, rect.height), "amount");
            };
    }

    void BuySetupReorderableListElementDrawer()
    {
        m_BuyPriceList.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = m_BuyPriceList.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;

                float delayWidth = 60;
                float nameWidth = rect.width - delayWidth;

                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y, nameWidth / 2, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("currency"), GUIContent.none);

                EditorGUI.PropertyField(
                    new Rect(rect.x + nameWidth / 2 + 20, rect.y, nameWidth / 2, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("amount"), GUIContent.none);
            };
    }

    void BuySetupReorderableListOnAddDropdownCallback()
    {
        m_BuyPriceList.onAddDropdownCallback =
            (Rect buttonRect, ReorderableList l) =>
            {
                if (inventoryItemList.currencies == null || inventoryItemList.currencies.Count == 0)
                {
                    EditorApplication.Beep();
                    EditorUtility.DisplayDialog("Error", "You don't have any currency defined in the Database", "Ok");
                    return;
                }

                BuyOnReorderableListAddDropdownClick();
            };
    }

    void BuyOnReorderableListAddDropdownClick()
    {
        //Currency state = (Currency)target;

        foreach (var state in inventoryItemList.currencies)
        {
            int index = m_BuyPriceList.serializedProperty.arraySize;
            m_BuyPriceList.serializedProperty.arraySize++;
            m_BuyPriceList.index = index;

            SerializedProperty element = m_BuyPriceList.serializedProperty.GetArrayElementAtIndex(index);
            element.FindPropertyRelative("currency").objectReferenceValue = state;
            element.FindPropertyRelative("amount").floatValue = 0;
        }

        m_BuySerializedObject.ApplyModifiedProperties();
    }

    //...................................


    void SellCreateReorderableList()
    {
        m_SellPriceList = new ReorderableList(
            m_SellSerializedObject,
            m_SellSerializedObject.FindProperty("sell_Price"),
            true, true, true, true);
    }

    void SellSetupReoirderableListHeaderDrawer()
    {
        m_SellPriceList.drawHeaderCallback =
            (Rect rect) =>
            {
                EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width - 60, rect.height), "Currency");
                EditorGUI.LabelField(new Rect(rect.x + rect.width / 2, rect.y, 60, rect.height), "amount");
            };
    }

    void SellSetupReorderableListElementDrawer()
    {
        m_SellPriceList.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = m_SellPriceList.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;

                float delayWidth = 60;
                float nameWidth = rect.width - delayWidth;

                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y, nameWidth / 2, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("currency"), GUIContent.none);

                EditorGUI.PropertyField(
                    new Rect(rect.x + nameWidth / 2 + 20, rect.y, nameWidth / 2, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("amount"), GUIContent.none);
            };
    }

    void SellSetupReorderableListOnAddDropdownCallback()
    {
        m_SellPriceList.onAddDropdownCallback =
            (Rect buttonRect, ReorderableList l) =>
            {
                if (inventoryItemList.currencies == null || inventoryItemList.currencies.Count == 0)
                {
                    EditorApplication.Beep();
                    EditorUtility.DisplayDialog("Error", "You don't have any currency defined in the Database", "Ok");
                    return;
                }

                SellOnReorderableListAddDropdownClick();
            };
    }

    void SellOnReorderableListAddDropdownClick()
    {
        //Currency state = (Currency)target;
        foreach (var state in inventoryItemList.currencies)
        {
            int index = m_SellPriceList.serializedProperty.arraySize;
            m_SellPriceList.serializedProperty.arraySize++;
            m_SellPriceList.index = index;

            SerializedProperty element = m_SellPriceList.serializedProperty.GetArrayElementAtIndex(index);
            element.FindPropertyRelative("currency").objectReferenceValue = state;
            element.FindPropertyRelative("amount").floatValue = 0;
        }

        m_SellSerializedObject.ApplyModifiedProperties();
    }

    //...................................
    //

    #endregion

    //CraftingEditor

    #region

    void DrawCraftingEditor()
    {
        GUILayout.Label("Crafting Editor", labelStyle);
        GUILayout.Space(10);
        showBluePrintInt = GUILayout.Toolbar(showBluePrintInt, bluePrintBar, GUILayout.MinHeight(30));
        GUILayout.Space(10);
        if (showBluePrintInt == 0)
        {
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(EditorStyles.helpBox);
            DrawCategoryChoose();
            GUILayout.EndVertical();
            GUILayout.Space(10);
            GUILayout.BeginVertical(EditorStyles.helpBox);
            DrawBluePrint();
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }

        //这里是绘制category的地方
        else if (showBluePrintInt == 1)
        {
            DrawCategory();
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(inventoryItemList);
            if (bluePrintViewIndex > 0 && inventoryItemList.bluePrints.Count > 0)
                EditorUtility.SetDirty(inventoryItemList.bluePrints[bluePrintViewIndex - 1]);
        }
    }

    int categoryChooseIndex = 0;
    List<string> categoryNames = new List<string>();

    void DrawCategoryChoose()
    {
        categoryNames.Clear();
        categoryNames.Add("All");

        for (int i = 0; i < inventoryItemList.categories.Count; i++)
        {
            categoryNames.Add(inventoryItemList.categories[i].categoryName);
        }

        if (categoryNames.Count > 0)
        {
            string[] _categoryNames = categoryNames.ToArray();
            categoryChooseIndex = GUILayout.SelectionGrid(categoryChooseIndex, _categoryNames, 1);
        }
    }

    void DrawBluePrint()
    {
        if (GUILayout.Button("Create New BluePrint"))
        {
            CreateNewBluePrint();
        }

        GUILayout.Space(10);

        if (inventoryItemList.bluePrints.Count > 0)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);

            if (categoryChooseIndex >= 1)
            {
                if (inventoryItemList.categories[categoryChooseIndex - 1] !=
                    inventoryItemList.bluePrints[bluePrintViewIndex - 1].category)
                {
                    for (int i = 0; i < inventoryItemList.bluePrints.Count; i++)
                    {
                        if (inventoryItemList.bluePrints[i].category ==
                            inventoryItemList.categories[categoryChooseIndex - 1])
                        {
                            bluePrintViewIndex = i + 1;
                            break;
                        }
                    }
                }
            }

            if (GUILayout.Button("Prev", GUILayout.ExpandWidth(false)))
            {
                ClearReorderableList();
                if (inventoryItemList.categories.Count > 0 && categoryChooseIndex >= 1)
                {
                    for (int i = 0; i < inventoryItemList.bluePrints.Count; i++)
                    {
                        if (bluePrintViewIndex > 1)
                            bluePrintViewIndex--;
                        else if (bluePrintViewIndex <= 1)
                            bluePrintViewIndex = inventoryItemList.bluePrints.Count;

                        if (inventoryItemList.bluePrints[bluePrintViewIndex - 1].category ==
                            inventoryItemList.categories[categoryChooseIndex - 1])
                            break;
                    }
                }

                else
                {
                    if (bluePrintViewIndex > 1)
                        bluePrintViewIndex--;
                    else if (bluePrintViewIndex <= 1)
                        bluePrintViewIndex = inventoryItemList.bluePrints.Count;
                }

                InitializeReorderableList();
            }

            GUILayout.Space(5);

            if (GUILayout.Button("Next", GUILayout.ExpandWidth(false)))
            {
                ClearReorderableList();
                if (inventoryItemList.categories.Count > 0 && categoryChooseIndex >= 1)
                {
                    for (int i = 0; i < inventoryItemList.bluePrints.Count; i++)
                    {
                        if (bluePrintViewIndex < inventoryItemList.bluePrints.Count)
                        {
                            bluePrintViewIndex++;
                        }

                        else if (bluePrintViewIndex >= inventoryItemList.bluePrints.Count)
                        {
                            bluePrintViewIndex = 1;
                        }

                        if (inventoryItemList.bluePrints[bluePrintViewIndex - 1].category ==
                            inventoryItemList.categories[categoryChooseIndex - 1])
                            break;
                    }
                }

                else
                {
                    if (bluePrintViewIndex < inventoryItemList.bluePrints.Count)
                    {
                        bluePrintViewIndex++;
                    }

                    else if (bluePrintViewIndex >= inventoryItemList.bluePrints.Count)
                    {
                        bluePrintViewIndex = 1;
                    }
                }

                InitializeReorderableList();
            }


            GUILayout.Space(180);

            if (GUILayout.Button("Delete BluePrint", GUILayout.ExpandWidth(false)))
            {
                DeleteBluePrint(bluePrintViewIndex - 1);
            }

            GUILayout.EndHorizontal();

            if (inventoryItemList.bluePrints.Count > 0)
            {
                GUILayout.BeginHorizontal();

                bluePrintViewIndex =
                    Mathf.Clamp(
                        EditorGUILayout.IntField("Current BluePrint", bluePrintViewIndex, GUILayout.ExpandWidth(false)),
                        1, inventoryItemList.bluePrints.Count);
                EditorGUILayout.LabelField("of   " + inventoryItemList.bluePrints.Count.ToString() + "  BluePrint", "",
                    GUILayout.ExpandWidth(false));

                GUILayout.EndHorizontal();

                GUILayout.Space(5);

                if (GUILayout.Button("Show Blueprint In Assets", GUILayout.ExpandWidth(false)))
                {
                    ShowObjectInAssets(inventoryItemList.bluePrints[bluePrintViewIndex - 1]);
                }

                GUILayout.Space(10);
                //数据显示
                //name
                EditorGUI.BeginChangeCheck();
                inventoryItemList.bluePrints[bluePrintViewIndex - 1].bluePrintName = EditorGUILayout.DelayedTextField(
                    "BluePrint Name", inventoryItemList.bluePrints[bluePrintViewIndex - 1].bluePrintName as string);
                if (EditorGUI.EndChangeCheck())
                {
                    AssetDatabase.RenameAsset(
                        EditorPrefs.GetString("DatabasePath") + "/BluePrints/" +
                        inventoryItemList.bluePrints[bluePrintViewIndex - 1].name + ".asset",
                        inventoryItemList.bluePrints[bluePrintViewIndex - 1].bluePrintName);
                    AssetDatabase.SaveAssets();
                }

                GUILayout.Space(5);
                inventoryItemList.bluePrints[bluePrintViewIndex - 1].category = EditorGUILayout.ObjectField("Category",
                    inventoryItemList.bluePrints[bluePrintViewIndex - 1].category, typeof(Category), false) as Category;
                GUILayout.Space(5);
                //icon
                inventoryItemList.bluePrints[bluePrintViewIndex - 1].icon =
                    EditorGUILayout.ObjectField("BluePrint Icon",
                        inventoryItemList.bluePrints[bluePrintViewIndex - 1].icon, typeof(Sprite), false) as Sprite;
                GUILayout.Space(5);
                //target
                inventoryItemList.bluePrints[bluePrintViewIndex - 1].targetItem = EditorGUILayout.ObjectField("Product",
                    inventoryItemList.bluePrints[bluePrintViewIndex - 1].targetItem, typeof(Item), false) as Item;
                GUILayout.Space(5);
                //amount after crafting
                inventoryItemList.bluePrints[bluePrintViewIndex - 1].craftingAmount = Mathf.Clamp(
                    EditorGUILayout.IntField("Product Amount",
                        inventoryItemList.bluePrints[bluePrintViewIndex - 1].craftingAmount), 0, 9999999);
                GUILayout.Space(5);
                //Success Chance
                inventoryItemList.bluePrints[bluePrintViewIndex - 1].successChance = Mathf.Clamp(
                    EditorGUILayout.FloatField("Success Chance",
                        inventoryItemList.bluePrints[bluePrintViewIndex - 1].successChance), 0, 1);
                GUILayout.Space(5);
                //Crafting Time
                inventoryItemList.bluePrints[bluePrintViewIndex - 1].craftingTime = Mathf.Clamp(
                    EditorGUILayout.FloatField("Crafting Time",
                        inventoryItemList.bluePrints[bluePrintViewIndex - 1].craftingTime), 0, 9999999);
                GUILayout.Space(5);
                //CDAll
                inventoryItemList.bluePrints[bluePrintViewIndex - 1].CDAllWhenCrafting = EditorGUILayout.Toggle(
                    "CD All When Crafting", inventoryItemList.bluePrints[bluePrintViewIndex - 1].CDAllWhenCrafting,
                    GUILayout.ExpandWidth(false));
                GUILayout.Space(5);
                //move after crafting
                inventoryItemList.bluePrints[bluePrintViewIndex - 1].moveAfterCrafting = EditorGUILayout.Toggle(
                    "Moved After Crafting", inventoryItemList.bluePrints[bluePrintViewIndex - 1].moveAfterCrafting,
                    GUILayout.ExpandWidth(false));
                GUILayout.Space(10);


                //ingredients
                GUILayout.Label("Ingredients", tipLabelStyle);
                GUILayout.Space(5);
                if (m_BluePrintList != null && m_BluePrintSerializedObject != null)
                {
                    m_BluePrintSerializedObject.ApplyModifiedProperties();
                    m_BluePrintSerializedObject.Update();
                    m_BluePrintList.DoLayoutList();
                    m_BluePrintSerializedObject.ApplyModifiedProperties();
                }

                GUILayout.Space(10);

                //prices
                GUILayout.Label("Prices", tipLabelStyle);
                GUILayout.Space(5);
                if (m_BluePrintBuyPriceList != null && m_BluePrintBuySerializedObject != null)
                {
                    m_BluePrintBuySerializedObject.ApplyModifiedProperties();
                    m_BluePrintBuySerializedObject.Update();
                    m_BluePrintBuyPriceList.DoLayoutList();
                    m_BluePrintBuySerializedObject.ApplyModifiedProperties();
                }
            }
        }

        else
        {
            GUILayout.Space(50);
            GUILayout.Label("Don't Have Any BluePrint", normalCenterStyle);
        }
    }

    void DrawCategory()
    {
        if (GUILayout.Button("Create New Category"))
        {
            CreateNewCategory();
        }

        GUILayout.Space(10);

        if (inventoryItemList.categories.Count > 0)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            if (GUILayout.Button("Prev", GUILayout.ExpandWidth(false)))
            {
                ClearReorderableList();
                if (categoryViewIndex > 1)
                    categoryViewIndex--;
                else if (categoryViewIndex == 1)
                    categoryViewIndex = inventoryItemList.categories.Count;
                InitializeReorderableList();
            }

            GUILayout.Space(5);

            if (GUILayout.Button("Next", GUILayout.ExpandWidth(false)))
            {
                ClearReorderableList();
                if (categoryViewIndex < inventoryItemList.categories.Count)
                {
                    categoryViewIndex++;
                }

                else if (categoryViewIndex == inventoryItemList.categories.Count)
                {
                    categoryViewIndex = 1;
                }

                InitializeReorderableList();
            }


            GUILayout.Space(180);

            if (GUILayout.Button("Delete Category", GUILayout.ExpandWidth(false)))
            {
                DeleteCategory(categoryViewIndex - 1);
            }

            GUILayout.EndHorizontal();

            if (inventoryItemList.categories.Count > 0)
            {
                GUILayout.BeginHorizontal();

                categoryViewIndex =
                    Mathf.Clamp(
                        EditorGUILayout.IntField("Current Category", categoryViewIndex, GUILayout.ExpandWidth(false)),
                        1, inventoryItemList.categories.Count);
                EditorGUILayout.LabelField("of   " + inventoryItemList.categories.Count.ToString() + "  Category", "",
                    GUILayout.ExpandWidth(false));

                GUILayout.EndHorizontal();

                GUILayout.Space(5);

                if (GUILayout.Button("Show Category In Assets", GUILayout.ExpandWidth(false)))
                {
                    ShowObjectInAssets(inventoryItemList.categories[categoryViewIndex - 1]);
                }

                GUILayout.Space(10);

                //数据显示
                //name
                EditorGUI.BeginChangeCheck();
                inventoryItemList.categories[categoryViewIndex - 1].categoryName = EditorGUILayout.DelayedTextField(
                    "Category Name", inventoryItemList.categories[categoryViewIndex - 1].categoryName as string);
                if (EditorGUI.EndChangeCheck())
                {
                    AssetDatabase.RenameAsset(
                        EditorPrefs.GetString("DatabasePath") + "/BluePrints/Categories/" +
                        inventoryItemList.categories[categoryViewIndex - 1].name + ".asset",
                        inventoryItemList.categories[categoryViewIndex - 1].categoryName);
                    AssetDatabase.SaveAssets();
                }
            }
        }
        else
        {
            GUILayout.Space(50);
            GUILayout.Label("Don't Have Any Category", normalCenterStyle);
        }
    }

    void CreateNewBluePrint()
    {
        CraftingBluePrint newBluePrint = CreateBluePrint.Create();
        inventoryItemList.bluePrints.Add(newBluePrint);
        bluePrintViewIndex = inventoryItemList.bluePrints.Count;
        categoryChooseIndex = 0;
        ClearReorderableList();
        InitializeReorderableList();
    }

    void CreateNewCategory()
    {
        Category newCategory = CreateCategory.Create();
        inventoryItemList.categories.Add(newCategory);
        categoryViewIndex = inventoryItemList.categories.Count;
        ClearReorderableList();
        InitializeReorderableList();
    }

    void DeleteBluePrint(int index)
    {
        if (inventoryItemList.bluePrints.Count > 0)
        {
            ClearReorderableList();
            string objName = inventoryItemList.bluePrints[index].name;
            System.IO.File.Delete(EditorPrefs.GetString("DatabasePath") + "/BluePrints/" + objName + ".asset");
            System.IO.File.Delete(EditorPrefs.GetString("DatabasePath") + "/BluePrints/" + objName + ".asset.meta");
            inventoryItemList.bluePrints.RemoveAt(index);
            if (bluePrintViewIndex > 1)
            {
                bluePrintViewIndex--;
                InitializeReorderableList();
            }

            else
            {
                bluePrintViewIndex--;
            }
        }

        RefreshEditorProjectWindow();
        window.Repaint();
    }

    void DeleteCategory(int index)
    {
        if (inventoryItemList.categories.Count > 0)
        {
            ClearReorderableList();
            string objName = inventoryItemList.categories[index].name;
            System.IO.File.Delete(
                EditorPrefs.GetString("DatabasePath") + "/BluePrints/Categories/" + objName + ".asset");
            System.IO.File.Delete(EditorPrefs.GetString("DatabasePath") + "/BluePrints/Categories/" + objName +
                                  ".asset.meta");
            inventoryItemList.categories.RemoveAt(index);
            if (categoryViewIndex > 1)
            {
                categoryViewIndex--;
                InitializeReorderableList();
            }

            else
            {
                categoryViewIndex--;
            }
        }

        RefreshEditorProjectWindow();
        window.Repaint();
    }

    //..............................

    void BluePrintCreateReorderableList()
    {
        m_BluePrintList = new ReorderableList(
            m_BluePrintSerializedObject,
            m_BluePrintSerializedObject.FindProperty("ingredients"),
            true, true, true, true);
    }

    void BluePrintSetupReoirderableListHeaderDrawer()
    {
        m_BluePrintList.drawHeaderCallback =
            (Rect rect) =>
            {
                EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width - 60, rect.height), "Ingredient");
                EditorGUI.LabelField(new Rect(rect.x + rect.width / 2, rect.y, 60, rect.height), "amount");
            };
    }

    void BluePrintSetupReorderableListElementDrawer()
    {
        m_BluePrintList.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = m_BluePrintList.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;

                float delayWidth = 60;
                float nameWidth = rect.width - delayWidth;

                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y, nameWidth / 2, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("ingredient"), GUIContent.none);

                EditorGUI.PropertyField(
                    new Rect(rect.x + nameWidth / 2 + 20, rect.y, nameWidth / 2, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("amount"), GUIContent.none);
            };
    }

    void BluePrintSetupReorderableListOnAddDropdownCallback()
    {
        m_BluePrintList.onAddDropdownCallback =
            (Rect buttonRect, ReorderableList l) =>
            {
                if (inventoryItemList.itemList == null || inventoryItemList.itemList.Count == 0)
                {
                    EditorApplication.Beep();
                    EditorUtility.DisplayDialog("Error", "You don't have any item defined in the Database", "Ok");
                    return;
                }


                BluePrintOnReorderableListAddDropdownClick();
                //var menu = new GenericMenu();

                //foreach (GameObjectData state in inventoryItemList.gameObjectData)
                //{
                //    menu.AddItem(new GUIContent(state.name), false, BluePrintOnReorderableListAddDropdownClick, state);
                //}

                //menu.ShowAsContext();
            };
    }

    void BluePrintOnReorderableListAddDropdownClick()
    {
        int index = m_BluePrintList.serializedProperty.arraySize;
        m_BluePrintList.serializedProperty.arraySize++;
        m_BluePrintList.index = index;

        SerializedProperty element = m_BluePrintList.serializedProperty.GetArrayElementAtIndex(index);
        element.FindPropertyRelative("ingredient").objectReferenceValue = null;
        element.FindPropertyRelative("amount").intValue = 1;

        m_BluePrintSerializedObject.ApplyModifiedProperties();
    }


    //..............................

    void BluePrintBuyCreateReorderableList()
    {
        m_BluePrintBuyPriceList = new ReorderableList(
            m_BluePrintBuySerializedObject,
            m_BluePrintBuySerializedObject.FindProperty("craftingPrices"),
            true, true, true, true);
    }

    void BluePrintBuySetupReoirderableListHeaderDrawer()
    {
        m_BluePrintBuyPriceList.drawHeaderCallback =
            (Rect rect) =>
            {
                EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width - 60, rect.height), "Currency");
                EditorGUI.LabelField(new Rect(rect.x + rect.width / 2, rect.y, 60, rect.height), "amount");
            };
    }

    void BluePrintBuySetupReorderableListElementDrawer()
    {
        m_BluePrintBuyPriceList.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = m_BluePrintBuyPriceList.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;

                float delayWidth = 60;
                float nameWidth = rect.width - delayWidth;

                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y, nameWidth / 2, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("currency"), GUIContent.none);

                EditorGUI.PropertyField(
                    new Rect(rect.x + nameWidth / 2 + 20, rect.y, nameWidth / 2, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("amount"), GUIContent.none);
            };
    }

    void BluePrintBuySetupReorderableListOnAddDropdownCallback()
    {
        m_BluePrintBuyPriceList.onAddDropdownCallback =
            (Rect buttonRect, ReorderableList l) =>
            {
                if (inventoryItemList.currencies == null || inventoryItemList.currencies.Count == 0)
                {
                    EditorApplication.Beep();
                    EditorUtility.DisplayDialog("Error", "You don't have any currency defined in the Database", "Ok");
                    return;
                }

                //    var menu = new GenericMenu();

                //    foreach (Currency state in inventoryItemList.currencies)
                //    {
                //        menu.AddItem(new GUIContent(state.currencyName),
                //                      false,
                //                      BuyOnReorderableListAddDropdownClick,
                //                      state);
                //    }

                //    menu.ShowAsContext();
                BluePrintBuyOnReorderableListAddDropdownClick();
            };
    }

    void BluePrintBuyOnReorderableListAddDropdownClick()
    {
        //Currency state = (Currency)target;

        foreach (var state in inventoryItemList.currencies)
        {
            int index = m_BluePrintBuyPriceList.serializedProperty.arraySize;
            m_BluePrintBuyPriceList.serializedProperty.arraySize++;
            m_BluePrintBuyPriceList.index = index;

            SerializedProperty element = m_BluePrintBuyPriceList.serializedProperty.GetArrayElementAtIndex(index);
            element.FindPropertyRelative("currency").objectReferenceValue = state;
            element.FindPropertyRelative("amount").floatValue = 0;
        }

        m_BluePrintBuySerializedObject.ApplyModifiedProperties();
    }

    #endregion

    //CurrencyEditor

    #region

    void DrawCurrencyEditor()
    {
        GUILayout.Label("Currency Editor", labelStyle);
        GUILayout.Space(10);

        if (GUILayout.Button("Create New Currency"))
        {
            CreateNewCurrency();
        }

        GUILayout.Space(10);

        if (inventoryItemList.currencies.Count > 0)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            if (GUILayout.Button("Prev", GUILayout.ExpandWidth(false)))
            {
                if (currencyViewIndex > 1)
                    currencyViewIndex--;
                else if (currencyViewIndex == 1)
                    currencyViewIndex = inventoryItemList.currencies.Count;
            }

            GUILayout.Space(5);

            if (GUILayout.Button("Next", GUILayout.ExpandWidth(false)))
            {
                if (currencyViewIndex < inventoryItemList.currencies.Count)
                {
                    currencyViewIndex++;
                }

                else if (currencyViewIndex == inventoryItemList.currencies.Count)
                {
                    currencyViewIndex = 1;
                }
            }


            GUILayout.Space(180);

            if (GUILayout.Button("Delete Currency", GUILayout.ExpandWidth(false)))
            {
                DeleteCurrency(currencyViewIndex - 1);
            }

            GUILayout.EndHorizontal();

            if (inventoryItemList.currencies.Count > 0)
            {
                GUILayout.BeginHorizontal();

                currencyViewIndex =
                    Mathf.Clamp(
                        EditorGUILayout.IntField("Current Currency", currencyViewIndex, GUILayout.ExpandWidth(false)),
                        1, inventoryItemList.currencies.Count);
                EditorGUILayout.LabelField("of   " + inventoryItemList.currencies.Count.ToString() + "  Currency", "",
                    GUILayout.ExpandWidth(false));

                GUILayout.EndHorizontal();

                GUILayout.Space(5);
                if (GUILayout.Button("Show Currency In Assets", GUILayout.ExpandWidth(false)))
                {
                    ShowObjectInAssets(inventoryItemList.currencies[currencyViewIndex - 1]);
                }

                GUILayout.Space(10);
                //数据显示
                //name
                EditorGUI.BeginChangeCheck();
                inventoryItemList.currencies[currencyViewIndex - 1].currencyName = EditorGUILayout.DelayedTextField(
                    "Currency Name", inventoryItemList.currencies[currencyViewIndex - 1].currencyName as string);
                if (EditorGUI.EndChangeCheck())
                {
                    AssetDatabase.RenameAsset(
                        EditorPrefs.GetString("DatabasePath") + "/Currencies/" +
                        inventoryItemList.currencies[currencyViewIndex - 1].name + ".asset",
                        inventoryItemList.currencies[currencyViewIndex - 1].currencyName);
                    AssetDatabase.SaveAssets();
                }

                GUILayout.Space(5);
                //icon
                inventoryItemList.currencies[currencyViewIndex - 1].icon = EditorGUILayout.ObjectField("Currency Icon",
                    inventoryItemList.currencies[currencyViewIndex - 1].icon, typeof(Sprite), false) as Sprite;
                GUILayout.Space(5);

                //exchangerate
                inventoryItemList.currencies[currencyViewIndex - 1].exchangeRate = Mathf.Clamp(
                    EditorGUILayout.FloatField("Exchange Rate",
                        inventoryItemList.currencies[currencyViewIndex - 1].exchangeRate), 0.000001f, 9999999);
                GUILayout.Space(5);
                GUILayout.Space(20);
            }
        }

        else
        {
            GUILayout.Space(50);
            GUILayout.Label("Don't Have Any Currency", normalCenterStyle);
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(inventoryItemList);
            if (currencyViewIndex > 0 && inventoryItemList.currencies.Count > 0)
                EditorUtility.SetDirty(inventoryItemList.currencies[currencyViewIndex - 1]);
        }
    }

    void CreateNewCurrency()
    {
        Currency newCurrency = CreateCurrency.Create();
        inventoryItemList.currencies.Add(newCurrency);
        currencyViewIndex = inventoryItemList.currencies.Count;
    }

    void DeleteCurrency(int index)
    {
        if (inventoryItemList.currencies.Count >= 0)
        {
            string objName = inventoryItemList.currencies[index].name;
            System.IO.File.Delete(EditorPrefs.GetString("DatabasePath") + "/Currencies/" + objName + ".asset");
            System.IO.File.Delete(EditorPrefs.GetString("DatabasePath") + "/Currencies/" + objName + ".asset.meta");
            inventoryItemList.currencies.RemoveAt(index);
            currencyViewIndex--;
        }

        RefreshEditorProjectWindow();
        window.Repaint();
    }

    #endregion

    //AttributeEditor

    #region

    void DrawAttributeEditor()
    {
        GUILayout.Label("Attribute Editor", labelStyle);
        GUILayout.Space(10);
        if (GUILayout.Button("Create New Attribute"))
        {
            CreateNewAttribute();
        }

        GUILayout.Space(10);

        if (inventoryItemList.attributes.Count > 0)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            if (GUILayout.Button("Prev", GUILayout.ExpandWidth(false)))
            {
                if (attributeViewIndex > 1)
                    attributeViewIndex--;
                else if (attributeViewIndex == 1)
                    attributeViewIndex = inventoryItemList.attributes.Count;
            }

            GUILayout.Space(5);

            if (GUILayout.Button("Next", GUILayout.ExpandWidth(false)))
            {
                if (attributeViewIndex < inventoryItemList.attributes.Count)
                {
                    attributeViewIndex++;
                }

                else if (attributeViewIndex == inventoryItemList.attributes.Count)
                {
                    attributeViewIndex = 1;
                }
            }


            GUILayout.Space(180);

            if (GUILayout.Button("Delete Attribute", GUILayout.ExpandWidth(false)))
            {
                DeleteAttribute(attributeViewIndex - 1);
            }

            GUILayout.EndHorizontal();

            if (inventoryItemList.attributes.Count > 0)
            {
                GUILayout.BeginHorizontal();

                attributeViewIndex =
                    Mathf.Clamp(
                        EditorGUILayout.IntField("Current Attribute", attributeViewIndex, GUILayout.ExpandWidth(false)),
                        1, inventoryItemList.attributes.Count);
                EditorGUILayout.LabelField("of   " + inventoryItemList.attributes.Count.ToString() + "  Attributes", "",
                    GUILayout.ExpandWidth(false));

                GUILayout.EndHorizontal();

                GUILayout.Space(5);
                if (GUILayout.Button("Show Attribute In Assets", GUILayout.ExpandWidth(false)))
                {
                    ShowObjectInAssets(inventoryItemList.attributes[attributeViewIndex - 1]);
                }

                GUILayout.Space(10);
                //下面是各种物品数据的显示
                //name
                EditorGUI.BeginChangeCheck();
                inventoryItemList.attributes[attributeViewIndex - 1].attributeName = EditorGUILayout.DelayedTextField(
                    "Attribute Name", inventoryItemList.attributes[attributeViewIndex - 1].attributeName as string);
                if (EditorGUI.EndChangeCheck())
                {
                    AssetDatabase.RenameAsset(
                        EditorPrefs.GetString("DatabasePath") + "/ItemAttributes/" +
                        inventoryItemList.attributes[attributeViewIndex - 1].name + ".asset",
                        inventoryItemList.attributes[attributeViewIndex - 1].attributeName);
                    AssetDatabase.SaveAssets();
                }

                GUILayout.Space(5);
                //icon
                inventoryItemList.attributes[attributeViewIndex - 1].icon =
                    EditorGUILayout.ObjectField("Attribute Icon",
                        inventoryItemList.attributes[attributeViewIndex - 1].icon, typeof(Sprite), false) as Sprite;
            }
        }

        else
        {
            GUILayout.Space(50);
            GUILayout.Label("Don't Have Any Attribute", normalCenterStyle);
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(inventoryItemList);
            if (attributeViewIndex > 0 && inventoryItemList.attributes.Count > 0)
                EditorUtility.SetDirty(inventoryItemList.attributes[attributeViewIndex - 1]);
        }
    }

    void CreateNewAttribute()
    {
        ItemAttribute newAttribute = CreateItemAttribute.Create();
        inventoryItemList.attributes.Add(newAttribute);
        attributeViewIndex = inventoryItemList.attributes.Count;
    }

    void DeleteAttribute(int index)
    {
        if (inventoryItemList.attributes.Count > 0)
        {
            string objName = inventoryItemList.attributes[index].name;
            System.IO.File.Delete(EditorPrefs.GetString("DatabasePath") + "/ItemAttributes/" + objName + ".asset");
            System.IO.File.Delete(EditorPrefs.GetString("DatabasePath") + "/ItemAttributes/" + objName + ".asset.meta");
            inventoryItemList.attributes.RemoveAt(index);
            attributeViewIndex--;
        }

        RefreshEditorProjectWindow();
        window.Repaint();
    }

    #endregion

    void ShowObjectInAssets(Object target)
    {
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = target;
    }
}