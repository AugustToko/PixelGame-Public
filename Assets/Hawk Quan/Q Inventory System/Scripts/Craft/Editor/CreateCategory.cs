using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace QInventory
{
    public class CreateCategory
    {
        public static Category Create()
        {
            int num = 1;
            while (System.IO.File.Exists(EditorPrefs.GetString("DatabasePath") + "/BluePrints/Categories/New Category" + "(" + num.ToString() + ")" + ".asset"))
                num++;

            Category asset = ScriptableObject.CreateInstance<Category>();
            AssetDatabase.CreateAsset(asset, EditorPrefs.GetString("DatabasePath") + "/BluePrints/Categories/New Category(" + num.ToString() + ").asset");
            asset.categoryName = asset.name;
            AssetDatabase.SaveAssets();
            return asset;
        }
    }
}

