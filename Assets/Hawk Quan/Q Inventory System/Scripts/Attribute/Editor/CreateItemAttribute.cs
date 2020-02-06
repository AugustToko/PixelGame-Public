using UnityEngine;
using System.Collections;
using UnityEditor;

namespace QInventory
{
    public class CreateItemAttribute
    {

        public static ItemAttribute Create()
        {
            int num = 1;
            while (System.IO.File.Exists(EditorPrefs.GetString("DatabasePath") + "/ItemAttributes/New ItemAttribute" + "(" + num.ToString() + ")" + ".asset"))
                num++;
            ItemAttribute asset = ScriptableObject.CreateInstance<ItemAttribute>();
            AssetDatabase.CreateAsset(asset, EditorPrefs.GetString("DatabasePath") + "/ItemAttributes/New ItemAttribute(" + num.ToString() + ")" + ".asset");
            asset.attributeName = asset.name;
            AssetDatabase.SaveAssets();
            return asset;
        }
    }
}
