using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace QInventory
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(EquipmentManager))]
    public class EquipmentManagerEditor : Editor
    {
        private ReorderableList m_EquipmentPartList;

        private void OnEnable()
        {
            InitializeReorderableList();
        }

        public override void OnInspectorGUI()
        {
            GUILayout.Space(5);
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
            m_EquipmentPartList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }

        void InitializeReorderableList()
        {
            EquipmentPartCreateReorderableList();
            EquipmentPartSetupReoirderableListHeaderDrawer();
            EquipmentPartSetupReorderableListElementDrawer();
            EquipmentPartSetupReorderableListOnAddDropdownCallback();
        }

        void ClearReorderableList()
        {
            m_EquipmentPartList = null;
        }

        void EquipmentPartCreateReorderableList()
        {
            m_EquipmentPartList = new ReorderableList(
                            serializedObject,
                            serializedObject.FindProperty("equipmentParts"),
                            true, true, true, true);
        }

        void EquipmentPartSetupReoirderableListHeaderDrawer()
        {
            m_EquipmentPartList.drawHeaderCallback =
                (Rect rect) =>
                {
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width - 60, rect.height), "Equipment Part");
                    EditorGUI.LabelField(new Rect(rect.x + rect.width / 2, rect.y, 80, rect.height), "Player Part");
                };
        }

        void EquipmentPartSetupReorderableListElementDrawer()
        {
            // drawElementCallback会定义列表中的每个元素是如何被绘制的
            // 同样，保证我们绘制的元素是相对于Rect参数绘制的
            m_EquipmentPartList.drawElementCallback =
                (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    var element = m_EquipmentPartList.serializedProperty.GetArrayElementAtIndex(index);
                    rect.y += 2;

                    float delayWidth = 60;
                    float nameWidth = rect.width - delayWidth;

                    EditorGUI.PropertyField(
                        new Rect(rect.x, rect.y, nameWidth / 2, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("equipmentPart"), GUIContent.none);

                    EditorGUI.PropertyField(
                        new Rect(rect.x + nameWidth / 2 + 20, rect.y, nameWidth / 2, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("playerPart"), GUIContent.none);
                };
        }

        void EquipmentPartSetupReorderableListOnAddDropdownCallback()
        {
            m_EquipmentPartList.onAddDropdownCallback =
                (Rect buttonRect, ReorderableList l) =>
                {
                    EquipmentPartOnReorderableListAddDropdownClick();
                };
        }

        void EquipmentPartOnReorderableListAddDropdownClick()
        {
            int index = m_EquipmentPartList.serializedProperty.arraySize;
            m_EquipmentPartList.serializedProperty.arraySize++;
            m_EquipmentPartList.index = index;

            SerializedProperty element = m_EquipmentPartList.serializedProperty.GetArrayElementAtIndex(index);
            element.FindPropertyRelative("equipmentPart").enumValueIndex = 0;
            element.FindPropertyRelative("playerPart").objectReferenceValue = null;

            serializedObject.ApplyModifiedProperties();
        }
    }

}