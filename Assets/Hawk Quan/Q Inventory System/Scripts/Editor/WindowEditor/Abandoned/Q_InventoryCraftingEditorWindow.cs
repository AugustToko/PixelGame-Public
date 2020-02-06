using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace QInventory
{
    public class Q_InventoryCraftingEditorWindow : Q_InventoryEmptyEditorWindow
    {
        private static Q_InventoryCraftingEditorWindow _window;
        private static ItemList inventoryItemList;
        private int viewIndex = 1;

        private ReorderableList m_BluePrintList;
        private SerializedObject m_BluePrintSerializedObject;

        private ReorderableList m_BuyPriceList;
        private SerializedObject m_BuySerializedObject;

        public static Q_InventoryCraftingEditorWindow window
        {
            get
            {
                if (_window == null)
                    _window = GetWindow<Q_InventoryCraftingEditorWindow>(false, "Crafting Editor", false);
                return _window;
            }
        }

        public static void OpenQ_InventoryCraftingEditorWindow()
        {
            _window = GetWindow<Q_InventoryCraftingEditorWindow>(false, "Crafting Editor", true);
            _window.minSize = new Vector2(450.0f, 400.0f);
            //_window.maxSize = new Vector2(600.0f, 600.0f);
        }

        private void OnEnable()
        {
            if (EditorPrefs.HasKey("ObjectPath"))
            {
                string objectPath = EditorPrefs.GetString("ObjectPath");
                inventoryItemList = AssetDatabase.LoadAssetAtPath(objectPath, typeof(ItemList)) as ItemList;
                InitializeReorderableList();
            }
            viewIndex = 1;
        }

        Vector2 scrollPos;

        private void OnGUI()
        {
            //m_BluePrintList = Q_InventoryMainEditorWindow.inventoryItemList.bluePrints;
            GUILayout.Label("Crafting Editor", labelStyle);
            GUILayout.Space(10);

            //if(inventoryItemList.bluePrints == null)
            //{
            //    inventoryItemList.bluePrints = new List<CraftingBluePrint>();
            //    window.Repaint();
            //}
            using (var h = new EditorGUILayout.HorizontalScope())
            {
                using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPos))
                {
                    scrollPos = scrollView.scrollPosition;
                    if (GUILayout.Button("Create New BluePrint"))
                    {
                        CreateNewBluePrint();
                    }

                    GUILayout.Space(10);

                    if (inventoryItemList.bluePrints.Count > 0)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(10);
                        if (GUILayout.Button("Prev", GUILayout.ExpandWidth(false)))
                        {
                            ClearReorderableList();
                            if (viewIndex > 1)
                                viewIndex--;
                            else if (viewIndex == 1)
                                viewIndex = inventoryItemList.bluePrints.Count;
                            InitializeReorderableList();
                        }

                        GUILayout.Space(5);

                        if (GUILayout.Button("Next", GUILayout.ExpandWidth(false)))
                        {
                            ClearReorderableList();
                            if (viewIndex < inventoryItemList.bluePrints.Count)
                            {
                                viewIndex++;
                            }

                            else if (viewIndex == inventoryItemList.bluePrints.Count)
                            {
                                viewIndex = 1;
                            }
                            InitializeReorderableList();
                        }


                        GUILayout.Space(180);

                        if (GUILayout.Button("Delete BluePrint", GUILayout.ExpandWidth(false)))
                        {
                            DeleteBluePrint(viewIndex - 1);
                        }

                        GUILayout.EndHorizontal();

                        if (inventoryItemList.bluePrints.Count > 0)
                        {
                            GUILayout.BeginHorizontal();

                            viewIndex = Mathf.Clamp(EditorGUILayout.IntField("Current BluePrint", viewIndex, GUILayout.ExpandWidth(false)), 1, inventoryItemList.bluePrints.Count);
                            EditorGUILayout.LabelField("of   " + inventoryItemList.bluePrints.Count.ToString() + "  BluePrint", "", GUILayout.ExpandWidth(false));

                            GUILayout.EndHorizontal();

                            GUILayout.Space(10);
                            //数据显示
                            //name
                            EditorGUI.BeginChangeCheck();
                            inventoryItemList.bluePrints[viewIndex - 1].bluePrintName = EditorGUILayout.DelayedTextField("BluePrint Name", inventoryItemList.bluePrints[viewIndex - 1].bluePrintName as string);
                            if (EditorGUI.EndChangeCheck())
                            {
                                AssetDatabase.RenameAsset(EditorPrefs.GetString("DatabasePath") + "/BluePrints/" + inventoryItemList.bluePrints[viewIndex - 1].name + ".asset", inventoryItemList.bluePrints[viewIndex - 1].bluePrintName);
                                AssetDatabase.SaveAssets();
                            }
                            GUILayout.Space(5);
                            //icon
                            inventoryItemList.bluePrints[viewIndex - 1].icon = EditorGUILayout.ObjectField("BluePrint Icon", inventoryItemList.bluePrints[viewIndex - 1].icon, typeof(Sprite), false) as Sprite;
                            GUILayout.Space(5);
                            //target
                            inventoryItemList.bluePrints[viewIndex - 1].targetItem = EditorGUILayout.ObjectField("Product", inventoryItemList.bluePrints[viewIndex - 1].targetItem, typeof(Item), false) as Item;
                            GUILayout.Space(5);
                            //amount after crafting
                            inventoryItemList.bluePrints[viewIndex - 1].craftingAmount = Mathf.Clamp(EditorGUILayout.IntField("Product Amount", inventoryItemList.bluePrints[viewIndex - 1].craftingAmount), 0, 9999999);
                            GUILayout.Space(5);
                            //Success Chance
                            inventoryItemList.bluePrints[viewIndex - 1].successChance = Mathf.Clamp(EditorGUILayout.FloatField("Success Chance", inventoryItemList.bluePrints[viewIndex - 1].successChance), 0, 1);
                            GUILayout.Space(5);
                            //Crafting Time
                            inventoryItemList.bluePrints[viewIndex - 1].craftingTime = Mathf.Clamp(EditorGUILayout.FloatField("Crafting Time", inventoryItemList.bluePrints[viewIndex - 1].craftingTime), 0, 9999999);
                            GUILayout.Space(5);
                            //move after crafting
                            inventoryItemList.bluePrints[viewIndex - 1].moveAfterCrafting = EditorGUILayout.Toggle("Moved After Crafting", inventoryItemList.bluePrints[viewIndex - 1].moveAfterCrafting, GUILayout.ExpandWidth(false));
                            GUILayout.Space(10);


                            //ingredients
                            GUILayout.Label("Ingredients");
                            if (m_BluePrintList != null && m_BluePrintSerializedObject != null)
                            {
                                m_BluePrintSerializedObject.ApplyModifiedProperties();
                                m_BluePrintSerializedObject.Update();
                                m_BluePrintList.DoLayoutList();
                                m_BluePrintSerializedObject.ApplyModifiedProperties();
                            }

                            GUILayout.Space(10);

                            //prices
                            GUILayout.Label("Prices");
                            if (m_BuyPriceList != null && m_BuySerializedObject != null)
                            {
                                m_BuySerializedObject.ApplyModifiedProperties();
                                m_BuySerializedObject.Update();
                                m_BuyPriceList.DoLayoutList();
                                m_BuySerializedObject.ApplyModifiedProperties();
                            }
                        }

                    }

                    else
                    {
                        GUILayout.Space(50);
                        GUILayout.Label("Don't Have A BluePrint", normalCenterStyle);
                    }
                }
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(inventoryItemList);
                if (viewIndex > 0)
                    EditorUtility.SetDirty(inventoryItemList.bluePrints[viewIndex - 1]);
            }
        }

        void CreateNewBluePrint()
        {
            CraftingBluePrint newBluePrint = CreateBluePrint.Create();
            inventoryItemList.bluePrints.Add(newBluePrint);
            viewIndex = inventoryItemList.bluePrints.Count;
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
                inventoryItemList.bluePrints.RemoveAt(index);
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
            m_BluePrintList = null;
            m_BluePrintSerializedObject = null;

            m_BuyPriceList = null;
            m_BuySerializedObject = null;
        }

        void InitializeReorderableList()
        {
            if (inventoryItemList.bluePrints != null)
                if (inventoryItemList.bluePrints.Count > 0)
                {
                    CraftingBluePrint obj = inventoryItemList.bluePrints[viewIndex - 1];
                    m_BluePrintSerializedObject = new SerializedObject(obj);
                    BluePrintCreateReorderableList();
                    BluePrintSetupReoirderableListHeaderDrawer();
                    BluePrintSetupReorderableListElementDrawer();
                    BluePrintSetupReorderableListOnAddDropdownCallback();

                    m_BuySerializedObject = new SerializedObject(obj);
                    BuyCreateReorderableList();
                    BuySetupReoirderableListHeaderDrawer();
                    BuySetupReorderableListElementDrawer();
                    BuySetupReorderableListOnAddDropdownCallback();
                }
        }

        void RefreshEditorProjectWindow()
        {
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
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
            // drawElementCallback会定义列表中的每个元素是如何被绘制的
            // 同样，保证我们绘制的元素是相对于Rect参数绘制的
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

        }

        void BluePrintOnReorderableListAddDropdownClick(object target)
        {
            GameObjectData state = (GameObjectData)target;

            int index = m_BluePrintList.serializedProperty.arraySize;
            m_BluePrintList.serializedProperty.arraySize++;
            m_BluePrintList.index = index;

            SerializedProperty element = m_BluePrintList.serializedProperty.GetArrayElementAtIndex(index);
            element.FindPropertyRelative("ingredient").objectReferenceValue = state;
            element.FindPropertyRelative("amount").intValue = 0;

            m_BluePrintSerializedObject.ApplyModifiedProperties();
        }


        //..............................

        void BuyCreateReorderableList()
        {
            m_BuyPriceList = new ReorderableList(
                            m_BuySerializedObject,
                            m_BuySerializedObject.FindProperty("craftingPrices"),
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

        //..............................
    }
}
