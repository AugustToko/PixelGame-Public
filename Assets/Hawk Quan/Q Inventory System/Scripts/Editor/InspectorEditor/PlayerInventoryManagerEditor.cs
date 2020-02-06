using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace QInventory
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(PlayerInventoryManager))]
    public class PlayerInventoryManagerEditor : Editor
    {
        private ItemList itemlist;
        private ReorderableList m_playerAttributeList;
        private ReorderableList m_playerAttributesUIList;
        private ReorderableList m_playerCurrencyList;
        private GameObject player;
        private void OnEnable()
        {
            itemlist = (target as PlayerInventoryManager).GetComponent<InventoryManager>().itemDataBase;
            player = (target as PlayerInventoryManager).GetComponent<InventoryManager>().player;
            InitializeReorderableList();
        }

        public override void OnInspectorGUI()
        {
            if (itemlist != (target as PlayerInventoryManager).GetComponent<InventoryManager>().itemDataBase || itemlist == null)
            {
                itemlist = (target as PlayerInventoryManager).GetComponent<InventoryManager>().itemDataBase;
            }

            if (itemlist == null)
            {
                Debug.Log("Don't assign Item Database! (Inventory Manager)");
            }

            if (player == null)
            {
                Debug.Log("Don't assign player! (Inventory Manager)");
            }
            else if (!player.GetComponent<PlayerPickUp>())
            {
                player.AddComponent<PlayerPickUp>();
            }

            if (itemlist != null)
            {
                GUILayout.Space(5);
                GUILayout.Label("Player Attributes");
                GUILayout.Space(5);
                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();
                m_playerAttributeList.DoLayoutList();
                serializedObject.ApplyModifiedProperties();
                GUILayout.Space(5);

                GUILayout.Label("Player Currencies");
                GUILayout.Space(5);
                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();
                m_playerCurrencyList.DoLayoutList();
                serializedObject.ApplyModifiedProperties();
                GUILayout.Space(5);

                GUILayout.Label("Attributes Show UI");
                GUILayout.Space(5);
                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();
                m_playerAttributesUIList.DoLayoutList();
                serializedObject.ApplyModifiedProperties();

                if (GUI.changed)
                {
                    EditorUtility.SetDirty(target);
                }
            }
        }

        void InitializeReorderableList()
        {
            playerAttributeCreateReorderableList();
            playerAttributeSetupReoirderableListHeaderDrawer();
            playerAttributeSetupReorderableListElementDrawer();
            playerAttributeSetupReorderableListOnAddDropdownCallback();

            playerAttributesUICreateReorderableList();
            playerAttributesUISetupReoirderableListHeaderDrawer();
            playerAttributesUISetupReorderableListElementDrawer();
            playerAttributesUISetupReorderableListOnAddDropdownCallback();

            playerCurrencyCreateReorderableList();
            playerCurrencySetupReoirderableListHeaderDrawer();
            playerCurrencySetupReorderableListElementDrawer();
            playerCurrencySetupReorderableListOnAddDropdownCallback();
        }

        void ClearReorderableList()
        {
            m_playerAttributeList = null;
            m_playerAttributesUIList = null;
            m_playerCurrencyList = null;
        }

        //attribute
        #region
        void playerAttributeCreateReorderableList()
        {
            m_playerAttributeList = new ReorderableList(
                            serializedObject,
                            serializedObject.FindProperty("playerAttributes"),
                            true, true, true, true);
        }

        void playerAttributeSetupReoirderableListHeaderDrawer()
        {
            m_playerAttributeList.drawHeaderCallback =
                (Rect rect) =>
                {
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width - 60, rect.height), "Attribute");
                    EditorGUI.LabelField(new Rect(rect.x + rect.width / 4 + 5, rect.y, 50, rect.height), "Current");
                    EditorGUI.LabelField(new Rect(rect.x + rect.width / 2 + 5, rect.y, 40, rect.height), "Min");
                    EditorGUI.LabelField(new Rect(rect.x + rect.width / 4 * 3, rect.y, 40, rect.height), "Max");
                };
        }

        void playerAttributeSetupReorderableListElementDrawer()
        {
            m_playerAttributeList.drawElementCallback =
                (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    var element = m_playerAttributeList.serializedProperty.GetArrayElementAtIndex(index);
                    rect.y += 2;

                    EditorGUI.PropertyField(
                        new Rect(rect.x, rect.y, rect.width / 4, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("playerAttribute"), GUIContent.none);

                    EditorGUI.PropertyField(
                        new Rect(rect.x + rect.width / 4 + 5, rect.y, rect.width / 5, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("currentValue"), GUIContent.none);

                    EditorGUI.PropertyField(
                        new Rect(rect.x + rect.width / 2, rect.y, rect.width / 5, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("minValue"), GUIContent.none);

                    EditorGUI.PropertyField(
                        new Rect(rect.x + rect.width / 4 * 3, rect.y, rect.width / 5, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("maxValue"), GUIContent.none);
                };
        }

        void playerAttributeSetupReorderableListOnAddDropdownCallback()
        {
            m_playerAttributeList.onAddDropdownCallback =
                (Rect buttonRect, ReorderableList l) =>
                {
                    if (itemlist.attributes == null || itemlist.attributes.Count == 0)
                    {
                        EditorApplication.Beep();
                        EditorUtility.DisplayDialog("Error", "You don't have any attribute defined in the Database", "Ok");
                        return;
                    }

                    var menu = new GenericMenu();

                    foreach (ItemAttribute state in itemlist.attributes)
                    {
                        menu.AddItem(new GUIContent(state.attributeName),
                                      false,
                                      playerAttributeOnReorderableListAddDropdownClick,
                                      state);
                    }

                    menu.ShowAsContext();
                };
        }

        void playerAttributeOnReorderableListAddDropdownClick(object target)
        {
            ItemAttribute state = (ItemAttribute)target;

            int index = m_playerAttributeList.serializedProperty.arraySize;
            m_playerAttributeList.serializedProperty.arraySize++;
            m_playerAttributeList.index = index;

            SerializedProperty element = m_playerAttributeList.serializedProperty.GetArrayElementAtIndex(index);
            element.FindPropertyRelative("playerAttribute").objectReferenceValue = state;
            element.FindPropertyRelative("currentValue").floatValue = 0f;
            element.FindPropertyRelative("minValue").floatValue = 0f;
            element.FindPropertyRelative("maxValue").floatValue = 100f;

            serializedObject.ApplyModifiedProperties();
        }
        #endregion

        //ui
        #region
        void playerAttributesUICreateReorderableList()
        {
            m_playerAttributesUIList = new ReorderableList(
                            serializedObject,
                            serializedObject.FindProperty("playerAttributesUIs"),
                            true, true, true, true);
        }

        void playerAttributesUISetupReoirderableListHeaderDrawer()
        {
            m_playerAttributesUIList.drawHeaderCallback =
                (Rect rect) =>
                {
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, 50, rect.height), "Attribute");
                    EditorGUI.LabelField(new Rect(rect.x + rect.width / 4 + 10, rect.y, 50, rect.height), "Type");
                    EditorGUI.LabelField(new Rect(rect.x + rect.width / 2, rect.y, 50, rect.height), "Text");
                    EditorGUI.LabelField(new Rect(rect.x + rect.width / 4 * 3, rect.y, 50, rect.height), "Slider");
                };
        }

        void playerAttributesUISetupReorderableListElementDrawer()
        {
            // drawElementCallback会定义列表中的每个元素是如何被绘制的
            // 同样，保证我们绘制的元素是相对于Rect参数绘制的
            m_playerAttributesUIList.drawElementCallback =
                (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    var element = m_playerAttributesUIList.serializedProperty.GetArrayElementAtIndex(index);
                    rect.y += 2;

                    EditorGUI.PropertyField(
                        new Rect(rect.x, rect.y, rect.width / 4, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("m_Attribute"), GUIContent.none);

                    EditorGUI.PropertyField(
                        new Rect(rect.x + rect.width / 4 + 5, rect.y, rect.width / 5, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("showType"), GUIContent.none);

                    EditorGUI.PropertyField(
                        new Rect(rect.x + rect.width / 2, rect.y, rect.width / 4, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("showText"), GUIContent.none);

                    EditorGUI.PropertyField(
                        new Rect(rect.x + rect.width / 4 * 3 + 5, rect.y, rect.width / 4, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("showSlider"), GUIContent.none);
                };
        }

        void playerAttributesUISetupReorderableListOnAddDropdownCallback()
        {
            m_playerAttributesUIList.onAddDropdownCallback =
                (Rect buttonRect, ReorderableList l) =>
                {
                    if (itemlist.attributes == null || itemlist.attributes.Count == 0)
                    {
                        EditorApplication.Beep();
                        EditorUtility.DisplayDialog("Error", "You don't have any attribute defined in the Database", "Ok");
                        return;
                    }

                    var menu = new GenericMenu();

                    foreach (ItemAttribute state in itemlist.attributes)
                    {
                        menu.AddItem(new GUIContent(state.attributeName),
                                      false,
                                      playerAttributesUIOnReorderableListAddDropdownClick,
                                      state);
                    }

                    menu.ShowAsContext();
                };
        }

        void playerAttributesUIOnReorderableListAddDropdownClick(object target)
        {
            ItemAttribute state = (ItemAttribute)target;

            int index = m_playerAttributesUIList.serializedProperty.arraySize;
            m_playerAttributesUIList.serializedProperty.arraySize++;
            m_playerAttributesUIList.index = index;

            SerializedProperty element = m_playerAttributesUIList.serializedProperty.GetArrayElementAtIndex(index);
            element.FindPropertyRelative("m_Attribute").objectReferenceValue = state;
            element.FindPropertyRelative("showType").enumValueIndex = 0;
            element.FindPropertyRelative("showText").objectReferenceValue = null;
            element.FindPropertyRelative("showSlider").objectReferenceValue = null;

            serializedObject.ApplyModifiedProperties();
        }
        #endregion

        //currency
        #region
        void playerCurrencyCreateReorderableList()
        {
            m_playerCurrencyList = new ReorderableList(
                            serializedObject,
                            serializedObject.FindProperty("playerCurrencies"),
                            true, true, true, true);
        }

        void playerCurrencySetupReoirderableListHeaderDrawer()
        {
            m_playerCurrencyList.drawHeaderCallback =
                (Rect rect) =>
                {
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width - 60, rect.height), "Currency");
                    EditorGUI.LabelField(new Rect(rect.x + rect.width / 3 + 5, rect.y, 80, rect.height), "Amount");
                    EditorGUI.LabelField(new Rect(rect.x + rect.width / 3 * 2, rect.y, 80, rect.height), "Show Text");
                };
        }

        void playerCurrencySetupReorderableListElementDrawer()
        {
            m_playerCurrencyList.drawElementCallback =
                (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    var element = m_playerCurrencyList.serializedProperty.GetArrayElementAtIndex(index);
                    rect.y += 2;

                    EditorGUI.PropertyField(
                        new Rect(rect.x, rect.y, rect.width / 3, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("currency"), GUIContent.none);

                    EditorGUI.PropertyField(
                        new Rect(rect.x + rect.width / 3 + 5, rect.y, rect.width / 4, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("amount"), GUIContent.none);

                    EditorGUI.PropertyField(
                        new Rect(rect.x + rect.width / 3 * 2, rect.y, rect.width / 3, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("showText"), GUIContent.none);
                };
        }

        void playerCurrencySetupReorderableListOnAddDropdownCallback()
        {
            m_playerCurrencyList.onAddDropdownCallback =
                (Rect buttonRect, ReorderableList l) =>
                {
                    if (itemlist.currencies == null || itemlist.currencies.Count == 0)
                    {
                        EditorApplication.Beep();
                        EditorUtility.DisplayDialog("Error", "You don't have any currency defined in the Database", "Ok");
                        return;
                    }

                    var menu = new GenericMenu();

                    foreach (Currency state in itemlist.currencies)
                    {
                        menu.AddItem(new GUIContent(state.currencyName),
                                      false,
                                      playerCurrencyOnReorderableListAddDropdownClick,
                                      state);
                    }

                    menu.ShowAsContext();
                };
        }

        void playerCurrencyOnReorderableListAddDropdownClick(object target)
        {
            Currency state = (Currency)target;

            int index = m_playerCurrencyList.serializedProperty.arraySize;
            m_playerCurrencyList.serializedProperty.arraySize++;
            m_playerCurrencyList.index = index;

            SerializedProperty element = m_playerCurrencyList.serializedProperty.GetArrayElementAtIndex(index);
            element.FindPropertyRelative("currency").objectReferenceValue = state;
            element.FindPropertyRelative("amount").floatValue = 0;
            element.FindPropertyRelative("showText").objectReferenceValue = null;

            serializedObject.ApplyModifiedProperties();
        }
        #endregion
    }
}
