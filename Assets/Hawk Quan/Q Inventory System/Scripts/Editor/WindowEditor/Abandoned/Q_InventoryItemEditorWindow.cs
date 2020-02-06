using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace QInventory
{
    public class Q_InventoryItemEditorWindow : Q_InventoryEmptyEditorWindow
    {
        private static ItemList inventoryItemList;
        private int viewIndex = 1;
        private ObjectType m_ObjectType;
        private ReorderableList m_ConsumbleList;
        private SerializedObject m_ConsumbleSerializedObject;

        private ReorderableList m_EquipmentList;
        private SerializedObject m_EquipmentSerializedObject;

        private ReorderableList m_BuyPriceList;
        private SerializedObject m_BuySerializedObject;

        private ReorderableList m_SellPriceList;
        private SerializedObject m_SellSerializedObject;

        private static Q_InventoryItemEditorWindow _window;

        public static Q_InventoryItemEditorWindow window
        {
            get
            {
                if (_window == null)
                    _window = GetWindow<Q_InventoryItemEditorWindow>(false, "Item Editor", false);
                return _window;
            }
        }

        public static void OpenQ_InventoryItemEditorWindow()
        {
            _window = GetWindow<Q_InventoryItemEditorWindow>(false, "Item Editor", true);
            _window.minSize = new Vector2(450.0f, 400.0f);
            //_window.maxSize = new Vector2(600.0f, 600.0f);
        }


        private void OnEnable()
        {
            if (EditorPrefs.HasKey("ObjectPath"))
            {
                viewIndex = 1;
                m_ObjectType = ObjectType.EmptyObject;
                string objectPath = EditorPrefs.GetString("ObjectPath");
                inventoryItemList = AssetDatabase.LoadAssetAtPath(objectPath, typeof(ItemList)) as ItemList;
                InitializeReorderableList();
                m_ObjectType = ObjectType.EmptyGameObject;
            }
        }

        Vector2 scrollPos;
        Vector2 scrollPos2;

        private void OnGUI()
        {
            using (var h = new EditorGUILayout.HorizontalScope())
            {
                using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPos))
                {
                    scrollPos = scrollView.scrollPosition;
                    GUILayout.Label("Item Editor", labelStyle);

                    GUILayout.Space(20);
                    m_ObjectType = (ObjectType) EditorGUILayout.EnumPopup("Object Creating Type", m_ObjectType);
                    GUILayout.Space(10);

                    if (inventoryItemList != null)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(10);
                        if (GUILayout.Button("Prev", GUILayout.ExpandWidth(false)))
                        {
                            ClearReorderableList();
                            if (viewIndex > 1)
                                viewIndex--;
                            else if (viewIndex == 1)
                                viewIndex = inventoryItemList.itemList.Count;
                            InitializeReorderableList();
                        }

                        GUILayout.Space(5);

                        if (GUILayout.Button("Next", GUILayout.ExpandWidth(false)))
                        {
                            ClearReorderableList();
                            if (viewIndex < inventoryItemList.itemList.Count)
                            {
                                viewIndex++;
                            }

                            else if (viewIndex == inventoryItemList.itemList.Count)
                            {
                                viewIndex = 1;
                            }

                            InitializeReorderableList();
                        }

                        GUILayout.Space(180);

                        if (GUILayout.Button("Add Item", GUILayout.ExpandWidth(false)))
                        {
                            AddItem();
                        }

                        if (inventoryItemList.itemList.Count > 0)
                        {
                            if (GUILayout.Button("Delete Item", GUILayout.ExpandWidth(false)))
                            {
                                DeleteItem(viewIndex - 1);
                            }
                        }

                        GUILayout.EndHorizontal();

                        //...........
                        if (inventoryItemList.itemList == null)
                            Debug.Log("Dont have item");

                        if (inventoryItemList.itemList.Count > 0)
                        {
                            GUILayout.Label("Normal Settings", tipLabelStyle);

                            GUILayout.BeginHorizontal();

                            viewIndex = Mathf.Clamp(
                                EditorGUILayout.IntField("Current Item", viewIndex, GUILayout.ExpandWidth(false)), 1,
                                inventoryItemList.itemList.Count);
                            EditorGUILayout.LabelField(
                                "of   " + inventoryItemList.itemList.Count.ToString() + "  items", "",
                                GUILayout.ExpandWidth(false));
                            GUILayout.EndHorizontal();
                            GUILayout.Space(10);
                            //下面是各种物品数据的显示
                            //ID
                            GUILayout.Label("Item ID:                         " +
                                            inventoryItemList.itemList[viewIndex - 1].ID.ToString());

                            //Name
                            EditorGUI.BeginChangeCheck();
                            inventoryItemList.itemList[viewIndex - 1].itemName =
                                EditorGUILayout.DelayedTextField("Item Name",
                                    inventoryItemList.itemList[viewIndex - 1].itemName as string);
                            if (EditorGUI.EndChangeCheck())
                            {
                                AssetDatabase.RenameAsset(
                                    EditorPrefs.GetString("DatabasePath") + "/Items/" +
                                    inventoryItemList.itemList[viewIndex - 1].m_object.name + ".prefab",
                                    inventoryItemList.itemList[viewIndex - 1].itemName);
                                AssetDatabase.RenameAsset(
                                    EditorPrefs.GetString("DatabasePath") + "/Items/ItemScriptObjects/" +
                                    inventoryItemList.itemList[viewIndex - 1].name + ".asset",
                                    inventoryItemList.itemList[viewIndex - 1].itemName);
                                AssetDatabase.SaveAssets();
                            }

                            GUILayout.Space(5);

                            //Description
                            GUILayout.Label("Item Description");
                            GUILayout.Space(5);
                            scrollPos2 = EditorGUILayout.BeginScrollView(scrollPos2, GUILayout.Height(100));
                            inventoryItemList.itemList[viewIndex - 1].description = EditorGUILayout.TextArea(
                                inventoryItemList.itemList[viewIndex - 1].description, GUILayout.ExpandHeight(true));
                            EditorGUILayout.EndScrollView();
                            GUILayout.Space(5);

                            //icon
                            EditorGUI.BeginChangeCheck();
                            inventoryItemList.itemList[viewIndex - 1].icon = EditorGUILayout.ObjectField("Item Icon",
                                inventoryItemList.itemList[viewIndex - 1].icon, typeof(Sprite), false) as Sprite;
                            if (EditorGUI.EndChangeCheck())
                            {
                                if (inventoryItemList.itemList[viewIndex - 1].m_object && inventoryItemList
                                        .itemList[viewIndex - 1].m_object.GetComponent<SpriteRenderer>())
                                {
                                    inventoryItemList.itemList[viewIndex - 1].m_object.GetComponent<SpriteRenderer>()
                                        .sprite = inventoryItemList.itemList[viewIndex - 1].icon;
                                    RefreshEditorProjectWindow();
                                }
                            }

                            //gameobject
                            inventoryItemList.itemList[viewIndex - 1].m_object =
                                EditorGUILayout.ObjectField("Item Object",
                                    inventoryItemList.itemList[viewIndex - 1].m_object, typeof(GameObject),
                                    false) as GameObject;

                            GUILayout.Space(10);

                            GUILayout.BeginHorizontal();
                            inventoryItemList.itemList[viewIndex - 1].isStackable = EditorGUILayout.Toggle(
                                "Is Stackable", inventoryItemList.itemList[viewIndex - 1].isStackable,
                                GUILayout.ExpandWidth(false));
                            GUILayout.EndHorizontal();
                            GUILayout.Space(5);
                            if (inventoryItemList.itemList[viewIndex - 1].isStackable)
                            {
                                inventoryItemList.itemList[viewIndex - 1].maxStackNumber = Mathf.Clamp(
                                    (int) EditorGUILayout.IntField("maxStackNumber",
                                        inventoryItemList.itemList[viewIndex - 1].maxStackNumber), 1, 999);
                                GUILayout.Space(10);
                            }

                            inventoryItemList.itemList[viewIndex - 1].rarity =
                                (Rarity) EditorGUILayout.EnumPopup("Item Rarity",
                                    inventoryItemList.itemList[viewIndex - 1].rarity);

                            GUILayout.Space(10);

                            inventoryItemList.itemList[viewIndex - 1].variety =
                                (Variety) EditorGUILayout.EnumPopup("Item Variety",
                                    inventoryItemList.itemList[viewIndex - 1].variety);

                            if (inventoryItemList.itemList[viewIndex - 1].variety == Variety.Consumable)
                            {
                                GUILayout.Space(10);
                                GUILayout.Label("Consumable Settings", tipLabelStyle);
                                inventoryItemList.itemList[viewIndex - 1].coolDown = Mathf.Clamp(
                                    (float) EditorGUILayout.FloatField("Cool Down",
                                        inventoryItemList.itemList[viewIndex - 1].coolDown), 0, 99999999);
                                //

                                if (m_ConsumbleList != null && m_ConsumbleSerializedObject != null)
                                {
                                    m_ConsumbleSerializedObject.ApplyModifiedProperties();
                                    m_ConsumbleSerializedObject.Update();
                                    m_ConsumbleList.DoLayoutList();
                                    m_ConsumbleSerializedObject.ApplyModifiedProperties();
                                }
                            }
                            else if (inventoryItemList.itemList[viewIndex - 1].variety == Variety.Equipment)
                            {
                                GUILayout.Space(10);
                                GUILayout.Label("Equipment Settings", tipLabelStyle);
                                inventoryItemList.itemList[viewIndex - 1].equipmentPart =
                                    (EquipmentPart) EditorGUILayout.EnumPopup("Equipment Part",
                                        inventoryItemList.itemList[viewIndex - 1].equipmentPart);

                                //
                                if (m_EquipmentList != null && m_EquipmentSerializedObject != null)
                                {
                                    m_EquipmentSerializedObject.ApplyModifiedProperties();
                                    m_EquipmentSerializedObject.Update();
                                    m_EquipmentList.DoLayoutList();
                                    m_EquipmentSerializedObject.ApplyModifiedProperties();
                                }
                            }

                            GUILayout.Space(10);
                            if (inventoryItemList.itemList[viewIndex - 1].variety == Variety.Equipment ||
                                inventoryItemList.itemList[viewIndex - 1].variety == Variety.Consumable)
                            {
                                inventoryItemList.itemList[viewIndex - 1].clipOnUse =
                                    EditorGUILayout.ObjectField("Clip",
                                        inventoryItemList.itemList[viewIndex - 1].clipOnUse, typeof(AudioClip),
                                        false) as AudioClip;
                                if (inventoryItemList.itemList[viewIndex - 1].clipOnUse)
                                {
                                    inventoryItemList.itemList[viewIndex - 1].playClipTimes =
                                        Mathf.Clamp(
                                            (int) EditorGUILayout.IntField("Clip Play Times",
                                                inventoryItemList.itemList[viewIndex - 1].playClipTimes), 1, 99);
                                }
                            }

                            GUILayout.Space(20);

                            GUILayout.Label("Price Settings", tipLabelStyle);
                            GUILayout.Space(10);

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


                            GUILayout.Space(10);
                            GUILayout.Space(20);
                        }
                        else
                        {
                            GUILayout.Space(50);
                            GUILayout.Label("This Inventory List is Empty.", normalCenterStyle);
                        }
                    }
                }
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(inventoryItemList);
                if (viewIndex > 0)
                    EditorUtility.SetDirty(inventoryItemList.itemList[viewIndex - 1]);
            }

            //window.Repaint();
        }

        //void CreateNewItemList()
        //{
        //    viewIndex = 1;
        //    inventoryItemList = CreateItemDatabase.Create();
        //    if (inventoryItemList)
        //    {
        //        inventoryItemList.itemList = new List<Item>();
        //        string relPath = AssetDatabase.GetAssetPath(inventoryItemList);
        //        EditorPrefs.SetString("ObjectPath", relPath);
        //    }
        //}

        //void OpenItemList()
        //{
        //    string absPath = EditorUtility.OpenFilePanel("Select Inventory Item List", "", "");
        //    if (absPath.StartsWith(Application.dataPath))
        //    {
        //        string relPath = absPath.Substring(Application.dataPath.Length - "Assets".Length);
        //        inventoryItemList = AssetDatabase.LoadAssetAtPath(relPath, typeof(ItemList)) as ItemList;
        //        if (inventoryItemList.itemList == null)
        //            inventoryItemList.itemList = new List<Item>();
        //        if (inventoryItemList)
        //        {
        //            EditorPrefs.SetString("ObjectPath", relPath);
        //        }
        //    }
        //}

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
            viewIndex = inventoryItemList.itemList.Count;

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
                    EditorPrefs.GetString("DatabasePath") + "/Items" + "/New Item" + "(" + num.ToString() + ")" +
                    ".prefab", go);
                DestroyImmediate(go);

                if (m_ObjectType == ObjectType._2DGameObject)
                {
                    newPrefab.AddComponent<SpriteRenderer>();
                    newPrefab.AddComponent<BoxCollider2D>();
                    newPrefab.AddComponent<Rigidbody2D>();
                }

                //3D?

                GameObjectData newGameObjectData = newPrefab.AddComponent<GameObjectData>();
                newGameObjectData.item = newItem;
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
                    //inventoryItemList.gameObjectData.RemoveAt(index);
                    string objName = inventoryItemList.itemList[index].m_object.name;
                    System.IO.File.Delete(EditorPrefs.GetString("DatabasePath") + "/Items/" + objName + ".prefab");
                }

                string assetName = inventoryItemList.itemList[index].name;
                System.IO.File.Delete(EditorPrefs.GetString("DatabasePath") + "/Items/ItemScriptObjects/" + assetName +
                                      ".asset");

                inventoryItemList.itemList.RemoveAt(index);
                for (int i = 0; i < inventoryItemList.itemList.Count; i++)
                {
                    inventoryItemList.itemList[i].ID = i + 1;
                }

                ClearReorderableList();

                if (viewIndex > 1)
                {
                    viewIndex--;
                    InitializeReorderableList();
                }

                else
                {
                    viewIndex--;
                }
            }

            RefreshEditorProjectWindow();
            window.Repaint();
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
        }

        void InitializeReorderableList()
        {
            if (inventoryItemList.itemList != null)
                if (inventoryItemList.itemList.Count > 0)
                {
                    Item obj = inventoryItemList.itemList[viewIndex - 1];
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
        }

        //................................
        void RefreshEditorProjectWindow()
        {
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
        }

        void ConsumbleCreateReorderableList()
        {
            // ReorderableList是一个非常棒的查看数组类型变量的实现类。它位于UnityEditorInternal中，这意味着Unity并没有觉得该类足够好到可以开放给公众
            // 更多关于ReorderableLists的内容可参考：
            // http://va.lent.in/unity-make-your-lists-functional-with-reorderablelist/
            m_ConsumbleList = new ReorderableList(
                m_ConsumbleSerializedObject,
                m_ConsumbleSerializedObject.FindProperty("consumableItemAttributes"),
                true, true, true, true);
        }

        void ConsumbleSetupReoirderableListHeaderDrawer()
        {
            // ReorderableList有一系列回调函数来让我们重载绘制这些数组
            // 这里我们使用drawHeaderCallback来绘制表格的头headers
            // 每个回调会接受一个Rect变量，它包含了该元素绘制的位置
            // 因此我们可以使用这个变量来决定我们把当前的元素绘制在哪里
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
            // drawElementCallback会定义列表中的每个元素是如何被绘制的
            // 同样，保证我们绘制的元素是相对于Rect参数绘制的
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
                        new Rect(rect.x + rect.width / 2 + 70, rect.y, rect.width / 3,
                            EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("value"), GUIContent.none);
                };
        }

        void ConsumbleSetupReorderableListOnAddDropdownCallback()
        {
            // onAddDropdownCallback定义当我们点击列表下面的[+]按钮时发生的事件
            // 在本例里，我们想要显示一个下拉菜单来给出预定义的一些States
            m_ConsumbleList.onAddDropdownCallback =
                (Rect buttonRect, ReorderableList l) =>
                {
                    if (inventoryItemList.attributes == null || inventoryItemList.attributes.Count == 0)
                    {
                        EditorApplication.Beep();
                        EditorUtility.DisplayDialog("Error", "You don't have any attribute defined in the Database",
                            "Ok");
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

        // 这个回调函数会在用户选择了[+]下拉菜单中的某一项后调用
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
            // drawElementCallback会定义列表中的每个元素是如何被绘制的
            // 同样，保证我们绘制的元素是相对于Rect参数绘制的
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
                        new Rect(rect.x + rect.width / 2 + 70, rect.y, rect.width / 3,
                            EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("value"), GUIContent.none);
                };
        }

        void EquipmentSetupReorderableListOnAddDropdownCallback()
        {
            // onAddDropdownCallback定义当我们点击列表下面的[+]按钮时发生的事件
            // 在本例里，我们想要显示一个下拉菜单来给出预定义的一些States
            m_EquipmentList.onAddDropdownCallback =
                (Rect buttonRect, ReorderableList l) =>
                {
                    if (inventoryItemList.attributes == null || inventoryItemList.attributes.Count == 0)
                    {
                        EditorApplication.Beep();
                        EditorUtility.DisplayDialog("Error", "You don't have any attribute defined in the Database",
                            "Ok");
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
            // drawElementCallback会定义列表中的每个元素是如何被绘制的
            // 同样，保证我们绘制的元素是相对于Rect参数绘制的
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
            // onAddDropdownCallback定义当我们点击列表下面的[+]按钮时发生的事件
            // 在本例里，我们想要显示一个下拉菜单来给出预定义的一些States
            m_BuyPriceList.onAddDropdownCallback =
                (Rect buttonRect, ReorderableList l) =>
                {
                    if (inventoryItemList.currencies == null || inventoryItemList.currencies.Count == 0)
                    {
                        EditorApplication.Beep();
                        EditorUtility.DisplayDialog("Error", "You don't have any currency defined in the Database",
                            "Ok");
                        return;
                    }

                    //var menu = new GenericMenu();

                    //foreach (Currency state in inventoryItemList.currencies)
                    //{
                    //    menu.AddItem(new GUIContent(state.currencyName),
                    //                  false,
                    //                  BuyOnReorderableListAddDropdownClick,
                    //                  state);
                    //}

                    //menu.ShowAsContext();

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
            // drawElementCallback会定义列表中的每个元素是如何被绘制的
            // 同样，保证我们绘制的元素是相对于Rect参数绘制的
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
            // onAddDropdownCallback定义当我们点击列表下面的[+]按钮时发生的事件
            // 在本例里，我们想要显示一个下拉菜单来给出预定义的一些States
            m_SellPriceList.onAddDropdownCallback =
                (Rect buttonRect, ReorderableList l) =>
                {
                    if (inventoryItemList.currencies == null || inventoryItemList.currencies.Count == 0)
                    {
                        EditorApplication.Beep();
                        EditorUtility.DisplayDialog("Error", "You don't have any currency defined in the Database",
                            "Ok");
                        return;
                    }

                    //var menu = new GenericMenu();

                    //foreach (Currency state in inventoryItemList.currencies)
                    //{
                    //    menu.AddItem(new GUIContent(state.currencyName),
                    //                  false,
                    //                  SellOnReorderableListAddDropdownClick,
                    //                  state);
                    //}

                    //menu.ShowAsContext();
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
    }
}