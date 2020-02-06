using UnityEngine;
using UnityEditor;

namespace QInventory
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Storage))]
    public class StorageEditor : Editor
    {
        int itemID = 0;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Q_Inventory m_Storage = target as Storage;

            EditorGUILayout.LabelField("Add Item");
            itemID = Mathf.Clamp(EditorGUILayout.IntField("Item ID", itemID), 0, 99999);

            if (GUILayout.Button("Add Item"))
            {
                m_Storage.AddItem(itemID);
            }
        }
    }
}

