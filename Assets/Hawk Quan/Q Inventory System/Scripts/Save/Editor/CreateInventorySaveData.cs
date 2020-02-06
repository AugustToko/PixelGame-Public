using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace QInventory
{
    public class CreateInventorySaveData
    {
        [MenuItem("Assets/Create/Q Inventory/Saving")]
        public static InventorySaveData Create()
        {
            InventorySaveData asset = ScriptableObject.CreateInstance<InventorySaveData>();
            AssetDatabase.CreateAsset(asset, EditorPrefs.GetString("DatabasePath") + "/Save/Saving.asset");
            AssetDatabase.SaveAssets();
            return asset;
        }
    }
}

