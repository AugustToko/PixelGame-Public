using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;

namespace QInventory
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ItemSortingManager))]
    public class ItemSortingManagerEditor : Editor
    {
        private ItemList itemlist;
        private ReorderableList m_itemSortingOrders;

        private void OnEnable()
        {
            ItemSortingManager itemSorting = (ItemSortingManager)target;
            //itemSorting.itemSortingOrders.Add(Variety.Equipment);
            //itemSorting.itemSortingOrders.Add(Variety.Consumable);
            //itemSorting.itemSortingOrders.Add(Variety.Material);
            //itemlist = (target as ItemSortingManager).GetComponent<InventoryManager>().itemDataBase;
            InitializeReorderableList();
        }

        public override void OnInspectorGUI()
        {
            if (itemlist != (target as ItemSortingManager).GetComponent<InventoryManager>().itemDataBase || itemlist == null)
            {
                itemlist = (target as ItemSortingManager).GetComponent<InventoryManager>().itemDataBase;
            }

            if (itemlist != null)
            {
                GUILayout.Space(5);
                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();
                m_itemSortingOrders.DoLayoutList();
                serializedObject.ApplyModifiedProperties();
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }

        void InitializeReorderableList()
        {
            ItemSortingOrdersCreateReorderableList();
            ItemSortingOrdersSetupReoirderableListHeaderDrawer();
            ItemSortingOrdersSetupReorderableListElementDrawer();
            //ItemSortingOrdersSetupReorderableListOnAddDropdownCallback();
        }

        void ClearReorderableList()
        {
            m_itemSortingOrders = null;
        }

        #region
        void ItemSortingOrdersCreateReorderableList()
        {
            m_itemSortingOrders = new ReorderableList(
                            serializedObject,
                            serializedObject.FindProperty("itemSortingOrders"),
                            true, true, true, true);
        }

        void ItemSortingOrdersSetupReoirderableListHeaderDrawer()
        {
            m_itemSortingOrders.drawHeaderCallback =
                 (Rect rect) =>
                 {
                     EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width - 60, rect.height), "Varity");
                 };
        }

        void ItemSortingOrdersSetupReorderableListElementDrawer()
        {
            m_itemSortingOrders.drawElementCallback =
                (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    var element = m_itemSortingOrders.serializedProperty.GetArrayElementAtIndex(index);
                    rect.y += 2;

                    EditorGUI.PropertyField(
                        new Rect(rect.x, rect.y, rect.width / 4, EditorGUIUtility.singleLineHeight),
                        element, GUIContent.none);
                };
        }

        void ItemSortingOrdersSetupReorderableListOnAddDropdownCallback()
        {
            m_itemSortingOrders.onAddDropdownCallback =
                (Rect buttonRect, ReorderableList l) =>
                {

                };

            m_itemSortingOrders.onRemoveCallback = (ReorderableList l) => {

            };
        }
        #endregion
    }
}