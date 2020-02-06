using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace QInventory
{
    [CustomEditor(typeof(InventoryManager))]
    public class InventoryManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(20);

            if(GUILayout.Button("Save Inventory Data"))
            {
                InventoryManager.SaveInventoryData();
            }

            GUILayout.Space(5);

            if(GUILayout.Button("Clear Inventory Data"))
            {
                InventoryManager.ClearInventoryData();
            }
        }
    }
}
