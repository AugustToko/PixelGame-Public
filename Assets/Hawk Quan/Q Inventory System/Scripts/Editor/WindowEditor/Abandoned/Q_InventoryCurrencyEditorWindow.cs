using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace QInventory
{
    public class Q_InventoryCurrencyEditorWindow : Q_InventoryEmptyEditorWindow
    {
        private static Q_InventoryCurrencyEditorWindow _window;
        private static ItemList inventoryItemList;
        private int viewIndex = 1;

        public static Q_InventoryCurrencyEditorWindow window
        {
            get
            {
                if (_window == null)
                    _window = GetWindow<Q_InventoryCurrencyEditorWindow>(false, "Currency Editor", false);
                return _window;
            }
        }

        public static void OpenQ_InventoryCurrencyEditorWindow()
        {
            _window = GetWindow<Q_InventoryCurrencyEditorWindow>(false, "Currency Editor", true);
            _window.minSize = new Vector2(450.0f, 400.0f);
            //_window.maxSize = new Vector2(600.0f, 600.0f);
        }

        private void OnEnable()
        {
            if (EditorPrefs.HasKey("ObjectPath"))
            {
                string objectPath = EditorPrefs.GetString("ObjectPath");
                inventoryItemList = AssetDatabase.LoadAssetAtPath(objectPath, typeof(ItemList)) as ItemList;
            }
            //inventoryItemList = Q_InventoryMainEditorWindow.inventoryItemList;

            viewIndex = 1;
        }

        Vector2 scrollPos;

        private void OnGUI()
        {
            //m_BluePrintList = Q_InventoryMainEditorWindow.inventoryItemList.bluePrints;
            GUILayout.Label("Currency Editor", labelStyle);
            GUILayout.Space(10);

            //if (inventoryItemList.currencies == null)
            //{
            //    Debug.Log("change");
            //    inventoryItemList.currencies = new List<Currency>();
            //    window.Repaint();
            //}
            using (var h = new EditorGUILayout.HorizontalScope())
            {
                using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPos))
                {
                    scrollPos = scrollView.scrollPosition;
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
                            if (viewIndex > 1)
                                viewIndex--;
                            else if (viewIndex == 1)
                                viewIndex = inventoryItemList.currencies.Count;
                        }

                        GUILayout.Space(5);

                        if (GUILayout.Button("Next", GUILayout.ExpandWidth(false)))
                        {
                            if (viewIndex < inventoryItemList.currencies.Count)
                            {
                                viewIndex++;
                            }

                            else if (viewIndex == inventoryItemList.currencies.Count)
                            {
                                viewIndex = 1;
                            }
                        }


                        GUILayout.Space(180);

                        if (GUILayout.Button("Delete Currency", GUILayout.ExpandWidth(false)))
                        {
                            DeleteCurrency(viewIndex - 1);
                        }

                        GUILayout.EndHorizontal();

                        if (inventoryItemList.currencies.Count > 0)
                        {
                            GUILayout.BeginHorizontal();

                            viewIndex = Mathf.Clamp(EditorGUILayout.IntField("Current Currency", viewIndex, GUILayout.ExpandWidth(false)), 1, inventoryItemList.currencies.Count);
                            EditorGUILayout.LabelField("of   " + inventoryItemList.currencies.Count.ToString() + "  Currency", "", GUILayout.ExpandWidth(false));

                            GUILayout.EndHorizontal();

                            GUILayout.Space(10);
                            //数据显示
                            //name
                            EditorGUI.BeginChangeCheck();
                            inventoryItemList.currencies[viewIndex - 1].currencyName = EditorGUILayout.DelayedTextField("Currency Name", inventoryItemList.currencies[viewIndex - 1].currencyName as string);
                            if (EditorGUI.EndChangeCheck())
                            {
                                AssetDatabase.RenameAsset(EditorPrefs.GetString("DatabasePath") + "/Currencies/" + inventoryItemList.currencies[viewIndex - 1].name + ".asset", inventoryItemList.currencies[viewIndex - 1].currencyName);
                                AssetDatabase.SaveAssets();
                            }
                            GUILayout.Space(5);
                            //icon
                            inventoryItemList.currencies[viewIndex - 1].icon = EditorGUILayout.ObjectField("Currency Icon", inventoryItemList.currencies[viewIndex - 1].icon, typeof(Sprite), false) as Sprite;
                            GUILayout.Space(5);

                            //exchangerate
                            inventoryItemList.currencies[viewIndex - 1].exchangeRate = Mathf.Clamp(EditorGUILayout.FloatField("Exchange Rate", inventoryItemList.currencies[viewIndex - 1].exchangeRate), 0.000001f, 9999999);
                            GUILayout.Space(5);
                            GUILayout.Space(20);
                        }

                    }

                    else
                    {
                        GUILayout.Space(50);
                        GUILayout.Label("Don't Have A Currency", normalCenterStyle);
                    }
                }
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(inventoryItemList);
                if (viewIndex > 0)
                    EditorUtility.SetDirty(inventoryItemList.currencies[viewIndex - 1]);
            }
        }

        void CreateNewCurrency()
        {
            Currency newCurrency = CreateCurrency.Create();
            inventoryItemList.currencies.Add(newCurrency);
            viewIndex = inventoryItemList.currencies.Count;
        }

        void DeleteCurrency(int index)
        {
            if (inventoryItemList.currencies.Count >= 0)
            {
                string objName = inventoryItemList.currencies[index].name;
                System.IO.File.Delete(EditorPrefs.GetString("DatabasePath") + "/Currencies/" + objName + ".asset");
                inventoryItemList.currencies.RemoveAt(index);
                viewIndex--;
            }

            RefreshEditorProjectWindow();
            window.Repaint();
        }

        void RefreshEditorProjectWindow()
        {
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
        }
    }
}
