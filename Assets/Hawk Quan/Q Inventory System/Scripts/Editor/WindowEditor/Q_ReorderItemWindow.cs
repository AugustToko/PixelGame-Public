using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace QInventory
{
    public class Q_ReorderItemWindow : EditorWindow
    {
        private static Q_ReorderItemWindow _window;
        public static ItemList inventoryItemList;

        private ReorderableList m_ReorderItemList;
        private SerializedObject m_ReorderItemSerializedObject;

        public static Q_ReorderItemWindow window
        {
            get
            {
                if (_window == null)
                    _window = GetWindow<Q_ReorderItemWindow>(false, "Reorder Items", false);
                return _window;
            }
        }

        public static void OpenQ_ReorderItemWindow()
        {
            _window = GetWindow<Q_ReorderItemWindow>(false, "Reorder Items", true);
            _window.minSize = new Vector2(550.0f, 400.0f);
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
        }

        Vector2 scrollPos;

        private void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            m_ReorderItemSerializedObject.ApplyModifiedProperties();
            m_ReorderItemSerializedObject.Update();
            m_ReorderItemList.DoLayoutList();
            m_ReorderItemSerializedObject.ApplyModifiedProperties();

            GUILayout.Space(20);

            if (GUILayout.Button("Reorder ID", GUILayout.MinHeight(30)))
            {
                ReOrderID();
            }

            EditorGUILayout.EndScrollView();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(inventoryItemList);
            }
        }


        void ClearReorderableList()
        {
            m_ReorderItemList = null;
            m_ReorderItemSerializedObject = null;

        }

        void InitializeReorderableList()
        {
            if (inventoryItemList.itemList != null)
                if (inventoryItemList.itemList.Count > 0)
                {
                    ItemList obj = inventoryItemList;
                    m_ReorderItemSerializedObject = new SerializedObject(obj);
                    ReorderItemCreateReorderableList();
                    ReorderItemSetupReoirderableListHeaderDrawer();
                    ReorderItemSetupReorderableListElementDrawer();
                    ReorderItemSetupReorderableListOnDropdownCallback();
                }
        }


        //.......................................................................
        void ReorderItemCreateReorderableList()
        {
            m_ReorderItemList = new ReorderableList(
                            m_ReorderItemSerializedObject,
                            m_ReorderItemSerializedObject.FindProperty("itemList"),
                            true, true, true, true);
        }

        void ReorderItemSetupReoirderableListHeaderDrawer()
        {
            m_ReorderItemList.drawHeaderCallback =
                (Rect rect) =>
                {
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width - 60, rect.height), "Item");
                    EditorGUI.LabelField(new Rect(rect.x + rect.width / 2, rect.y, 60, rect.height), "Type");
                    EditorGUI.LabelField(new Rect(rect.x + rect.width - 30, rect.y, 60, rect.height), "ID");
                };
        }

        void ReorderItemSetupReorderableListElementDrawer()
        {
            m_ReorderItemList.drawElementCallback =
                (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    var element = m_ReorderItemList.serializedProperty.GetArrayElementAtIndex(index);
                    rect.y += 2;

                    float delayWidth = 60;
                    float nameWidth = rect.width - delayWidth;
                    Item disPlayItem = (Item)element.objectReferenceValue;
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, nameWidth / 2, EditorGUIUtility.singleLineHeight), disPlayItem.itemName);
                    EditorGUI.LabelField(new Rect(rect.x + rect.width / 2 - 5, rect.y, nameWidth / 2, EditorGUIUtility.singleLineHeight), disPlayItem.variety.ToString());
                    EditorGUI.LabelField(new Rect(rect.x + rect.width - 25, rect.y, nameWidth / 2, EditorGUIUtility.singleLineHeight), disPlayItem.ID.ToString());

                };
        }

        void ReorderItemSetupReorderableListOnDropdownCallback()
        {
            m_ReorderItemList.onAddDropdownCallback =
                (Rect buttonRect, ReorderableList l) =>
                {

                };

            m_ReorderItemList.onRemoveCallback = (ReorderableList l) => {

            };
        }

        void ReOrderID()
        {
            for (int i = 0; i < inventoryItemList.itemList.Count; i++)
            {
                inventoryItemList.itemList[i].ID = i + 1;
                EditorUtility.SetDirty(inventoryItemList.itemList[i]);
            }
        }


    }
}
