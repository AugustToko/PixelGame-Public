using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace QInventory
{
    public class CreateItem
    {
        public static Item Create()
        {
            int num = 1;
            while (System.IO.File.Exists(EditorPrefs.GetString("DatabasePath") + "/Items/ItemScriptObjects/New Item" + "(" + num.ToString() + ")" + ".asset"))
                num++;
            Item asset = ScriptableObject.CreateInstance<Item>();
            AssetDatabase.CreateAsset(asset, EditorPrefs.GetString("DatabasePath") + "/Items/ItemScriptObjects/New Item(" + num.ToString() + ")" + ".asset");
            asset.itemName = asset.name;
            AssetDatabase.SaveAssets();
            return asset;
        }
    }
}

