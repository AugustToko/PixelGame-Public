using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace QInventory
{
    public class Q_InventoryAttributeEditorWindow : Q_InventoryEmptyEditorWindow
    {
        private static Q_InventoryAttributeEditorWindow _window;
        private static ItemList inventoryItemList;
        private int viewIndex = 1;

        public static Q_InventoryAttributeEditorWindow window
        {
            get
            {
                if (_window == null)
                    _window = GetWindow<Q_InventoryAttributeEditorWindow>(false, "Attribute Editor", false);
                return _window;
            }
        }

        public static void OpenQ_InventoryAttributeEditorWindow()
        {
            _window = GetWindow<Q_InventoryAttributeEditorWindow>(false, "Attribute Editor", true);
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
            viewIndex = 1;
        }

        private void OnGUI()
        {
            //m_ItemAttributeList = Q_InventoryMainEditorWindow.inventoryItemList.attributes;
            GUILayout.Label("Attribute Editor", labelStyle);
            GUILayout.Space(10);

            //if(inventoryItemList.attributes == null)
            //{
            //    inventoryItemList.attributes = new List<ItemAttribute>();
            //    window.Repaint();
            //}
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
                    if (viewIndex > 1)
                        viewIndex--;
                    else if (viewIndex == 1)
                        viewIndex = inventoryItemList.attributes.Count;
                }

                GUILayout.Space(5);

                if (GUILayout.Button("Next", GUILayout.ExpandWidth(false)))
                {
                    if (viewIndex < inventoryItemList.attributes.Count)
                    {
                        viewIndex++;
                    }

                    else if (viewIndex == inventoryItemList.attributes.Count)
                    {
                        viewIndex = 1;
                    }
                }


                GUILayout.Space(180);

                if (GUILayout.Button("Delete Attribute", GUILayout.ExpandWidth(false)))
                {
                    DeleteAttribute(viewIndex - 1);
                }
                GUILayout.EndHorizontal();

                if (inventoryItemList.attributes.Count > 0)
                {

                    GUILayout.BeginHorizontal();

                    viewIndex = Mathf.Clamp(EditorGUILayout.IntField("Current Attribute", viewIndex, GUILayout.ExpandWidth(false)), 1, inventoryItemList.attributes.Count);
                    EditorGUILayout.LabelField("of   " + inventoryItemList.attributes.Count.ToString() + "  Attributes", "", GUILayout.ExpandWidth(false));

                    GUILayout.EndHorizontal();

                    GUILayout.Space(10);
                    //下面是各种物品数据的显示
                    //name
                    EditorGUI.BeginChangeCheck();
                    inventoryItemList.attributes[viewIndex - 1].attributeName = EditorGUILayout.DelayedTextField("Attribute Name", inventoryItemList.attributes[viewIndex - 1].attributeName as string);
                    if (EditorGUI.EndChangeCheck())
                    {
                        AssetDatabase.RenameAsset(EditorPrefs.GetString("DatabasePath") + "/ItemAttributes/" + inventoryItemList.attributes[viewIndex - 1].name + ".asset", inventoryItemList.attributes[viewIndex - 1].attributeName);
                        AssetDatabase.SaveAssets();
                    }
                    GUILayout.Space(5);
                    //icon
                    inventoryItemList.attributes[viewIndex - 1].icon = EditorGUILayout.ObjectField("Attribute Icon", inventoryItemList.attributes[viewIndex - 1].icon, typeof(Sprite), false) as Sprite;

                }
            }

            else
            {
                GUILayout.Space(50);
                GUILayout.Label("Don't Have An Attribute", normalCenterStyle);
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(inventoryItemList);
                if (viewIndex > 0)
                    EditorUtility.SetDirty(inventoryItemList.attributes[viewIndex - 1]);
            }
        }

        void CreateNewAttribute()
        {
            ItemAttribute newAttribute = CreateItemAttribute.Create();
            inventoryItemList.attributes.Add(newAttribute);
            viewIndex = inventoryItemList.attributes.Count;
        }

        void DeleteAttribute(int index)
        {
            if (inventoryItemList.attributes.Count > 0)
            {
                string objName = inventoryItemList.attributes[index].name;
                System.IO.File.Delete(EditorPrefs.GetString("DatabasePath") + "/ItemAttributes/" + objName + ".asset");
                inventoryItemList.attributes.RemoveAt(index);
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
