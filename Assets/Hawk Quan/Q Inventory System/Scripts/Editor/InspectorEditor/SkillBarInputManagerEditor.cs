using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace QInventory
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SkillBarInputManager))]
    public class SkillBarInputManagerEditor : Editor
    {
        public ReorderableList m_SkillKeyList;

        private void OnEnable()
        {
            InitializeReorderableList();
        }

        public override void OnInspectorGUI()
        {
            GUILayout.Space(5);
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
            m_SkillKeyList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }

        void InitializeReorderableList()
        {
            SkillKeyCreateReorderableList();
            SkillKeySetupReoirderableListHeaderDrawer();
            SkillKeySetupReorderableListElementDrawer();
            SkillKeySetupReorderableListOnAddDropdownCallback();
        }

        void ClearReorderableList()
        {
            m_SkillKeyList = null;
        }

        void SkillKeyCreateReorderableList()
        {
            m_SkillKeyList = new ReorderableList(
                serializedObject,
                serializedObject.FindProperty("skillBarInputs"),
                true, true, true, true);
        }

        void SkillKeySetupReoirderableListHeaderDrawer()
        {
            m_SkillKeyList.drawHeaderCallback =
                (Rect rect) =>
                {
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width - 60, rect.height), "Slot ID");
                    EditorGUI.LabelField(new Rect(rect.x + rect.width / 3 + 10, rect.y, 80, rect.height), "Key");
                    EditorGUI.LabelField(new Rect(rect.x + rect.width / 3 * 2, rect.y, 80, rect.height), "Key Name");
                };
        }

        void SkillKeySetupReorderableListElementDrawer()
        {
            m_SkillKeyList.drawElementCallback =
                (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    var element = m_SkillKeyList.serializedProperty.GetArrayElementAtIndex(index);
                    rect.y += 2;

                    EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y, rect.width / 3, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("SlotID"), GUIContent.none);

                    EditorGUI.PropertyField(
                    new Rect(rect.x + rect.width / 3 + 5, rect.y, rect.width / 4, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("key"), GUIContent.none);

                    EditorGUI.PropertyField(
                    new Rect(rect.x + rect.width / 3 * 2, rect.y, rect.width / 3, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("keyName"), GUIContent.none);
                };
        }

        void SkillKeySetupReorderableListOnAddDropdownCallback()
        {
            m_SkillKeyList.onAddDropdownCallback =
                (Rect buttonRect, ReorderableList l) =>
                {
                    SkillKeyOnReorderableListAddDropdownClick();
                };
        }

        void SkillKeyOnReorderableListAddDropdownClick()
        {

            int index = m_SkillKeyList.serializedProperty.arraySize;
            m_SkillKeyList.serializedProperty.arraySize++;
            m_SkillKeyList.index = index;

            SerializedProperty element = m_SkillKeyList.serializedProperty.GetArrayElementAtIndex(index);
            element.FindPropertyRelative("SlotID").intValue = 0;
            element.FindPropertyRelative("key").enumValueIndex = 0;
            element.FindPropertyRelative("keyName").stringValue = null;

            serializedObject.ApplyModifiedProperties();
        }
    }
}
