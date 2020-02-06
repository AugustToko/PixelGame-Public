using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace QInventory
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(InventoryInputControl))]
    public class Q_UIControlsEditor : Editor
    {
        private ReorderableList m_UIControlList;

        private void OnEnable()
        {
            InitializeReorderableList();
        }

        public override void OnInspectorGUI()
        {
            GUILayout.Space(5);
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
            m_UIControlList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }

        void InitializeReorderableList()
        {
            UIControlCreateReorderableList();
            UIControlSetupReoirderableListHeaderDrawer();
            UIControlSetupReorderableListElementDrawer();
            UIControlSetupReorderableListOnAddDropdownCallback();
        }

        void ClearReorderableList()
        {
            m_UIControlList = null;

        }

        void UIControlCreateReorderableList()
        {
            m_UIControlList = new ReorderableList(
                            serializedObject,
                            serializedObject.FindProperty("inventoryUIControls"),
                            true, true, true, true);
        }

        void UIControlSetupReoirderableListHeaderDrawer()
        {
            m_UIControlList.drawHeaderCallback =
                (Rect rect) =>
                {
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width / 2, rect.height), "UI Panel");
                    EditorGUI.LabelField(new Rect(rect.x + rect.width / 2 + 10, rect.y, rect.width / 2, rect.height), "KeyCode");
                };
        }

        void UIControlSetupReorderableListElementDrawer()
        {
            // drawElementCallback会定义列表中的每个元素是如何被绘制的
            // 同样，保证我们绘制的元素是相对于Rect参数绘制的
            m_UIControlList.drawElementCallback =
                (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    var element = m_UIControlList.serializedProperty.GetArrayElementAtIndex(index);
                    rect.y += 2;

                    EditorGUI.PropertyField(
                        new Rect(rect.x, rect.y, rect.width / 2, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("m_InventoryUI"), GUIContent.none);

                    EditorGUI.PropertyField(
                        new Rect(rect.x + rect.width / 2 + 5, rect.y, rect.width / 3, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("m_KeyCode"), GUIContent.none);
                };
        }

        void UIControlSetupReorderableListOnAddDropdownCallback()
        {
            m_UIControlList.onAddDropdownCallback =
                (Rect buttonRect, ReorderableList l) =>
                {
                    UIControlOnReorderableListAddDropdownClick();
                };
        }

        void UIControlOnReorderableListAddDropdownClick()
        {
            int index = m_UIControlList.serializedProperty.arraySize;
            m_UIControlList.serializedProperty.arraySize++;
            m_UIControlList.index = index;

            SerializedProperty element = m_UIControlList.serializedProperty.GetArrayElementAtIndex(index);
            element.FindPropertyRelative("m_InventoryUI").objectReferenceValue = null;
            element.FindPropertyRelative("m_KeyCode").enumValueIndex = 0;

            serializedObject.ApplyModifiedProperties();
        }
    }
}

