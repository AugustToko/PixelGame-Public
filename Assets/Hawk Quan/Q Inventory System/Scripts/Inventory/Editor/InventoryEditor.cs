using UnityEngine;
using UnityEditor;

namespace QInventory
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Q_Inventory))]
    public class InventoryEditor : Editor
    {
        int itemID = 0;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Q_Inventory m_Inventory = target as Q_Inventory;

            EditorGUILayout.LabelField("Add Item");
            itemID = Mathf.Clamp(EditorGUILayout.IntField("Item ID", itemID), 0, 99999);

            if (GUILayout.Button("Add Item"))
            {
                m_Inventory.AddItem(itemID);
            }
        }
    }
}
