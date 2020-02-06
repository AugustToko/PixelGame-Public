using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace QInventory
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(AddItem))]
    public class AddItemEditor : Editor
    {
        private ReorderableList m_ItemList;
        private static ItemList inventoryItemList;

        private void OnEnable()
        {
            if (EditorPrefs.HasKey("ObjectPath"))
            {
                string objectPath = EditorPrefs.GetString("ObjectPath");
                inventoryItemList = AssetDatabase.LoadAssetAtPath(objectPath, typeof(ItemList)) as ItemList;
                InitializeReorderableList();
            }
        }

        public override void OnInspectorGUI()
        {
            GUILayout.Space(10);
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
            m_ItemList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }

        void InitializeReorderableList()
        {
            ItemCreateReorderableList();
            ItemSetupReoirderableListHeaderDrawer();
            ItemSetupReorderableListElementDrawer();
            ItemSetupReorderableListOnAddDropdownCallback();
        }

        void ClearReorderableList()
        {
            m_ItemList = null;
        }

        void ItemCreateReorderableList()
        {
            m_ItemList = new ReorderableList(
                            serializedObject,
                            serializedObject.FindProperty("itemsToAdd"),
                            true, true, true, true);
        }

        void ItemSetupReoirderableListHeaderDrawer()
        {
            m_ItemList.drawHeaderCallback =
                (Rect rect) =>
                {
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width - 60, rect.height), "Item");
                    EditorGUI.LabelField(new Rect(rect.x + rect.width * 0.5f, rect.y, 60, rect.height), "Amount");
                    EditorGUI.LabelField(new Rect(rect.x + rect.width - 50f, rect.y, 60, rect.height), "Chance");
                };
        }

        void ItemSetupReorderableListElementDrawer()
        {
            // drawElementCallback会定义列表中的每个元素是如何被绘制的
            // 同样，保证我们绘制的元素是相对于Rect参数绘制的
            m_ItemList.drawElementCallback =
                (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    var element = m_ItemList.serializedProperty.GetArrayElementAtIndex(index);
                    rect.y += 2;

                    float delayWidth = 60;
                    float nameWidth = rect.width - delayWidth;

                    EditorGUI.PropertyField(
                        new Rect(rect.x, rect.y, nameWidth / 2, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("itemsToAdd"), GUIContent.none);

                    EditorGUI.PropertyField(
                        new Rect(rect.x + rect.width / 2 - 10, rect.y, nameWidth / 3, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("amount"), GUIContent.none);

                    EditorGUI.PropertyField(
                        new Rect(rect.x + rect.width - 40, rect.y, 40, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("chance"), GUIContent.none);
                };
        }

        void ItemSetupReorderableListOnAddDropdownCallback()
        {
            // onAddDropdownCallback定义当我们点击列表下面的[+]按钮时发生的事件
            // 在本例里，我们想要显示一个下拉菜单来给出预定义的一些States
            m_ItemList.onAddDropdownCallback =
                (Rect buttonRect, ReorderableList l) =>
                {
                    if (inventoryItemList.itemList == null || inventoryItemList.itemList.Count == 0)
                    {
                        EditorApplication.Beep();
                        EditorUtility.DisplayDialog("Error", "You don't have any item defined in the Database", "Ok");
                        return;
                    }

                    ItemOnReorderableListAddDropdownClick();
                };
        }

        // 这个回调函数会在用户选择了[+]下拉菜单中的某一项后调用
        void ItemOnReorderableListAddDropdownClick()
        {

            int index = m_ItemList.serializedProperty.arraySize;
            m_ItemList.serializedProperty.arraySize++;
            m_ItemList.index = index;

            SerializedProperty element = m_ItemList.serializedProperty.GetArrayElementAtIndex(index);
            element.FindPropertyRelative("itemsToAdd").objectReferenceValue = null;
            element.FindPropertyRelative("amount").intValue = 1;
            element.FindPropertyRelative("chance").floatValue = 1f;

            serializedObject.ApplyModifiedProperties();
        }
    }
}
