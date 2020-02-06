using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace QInventory
{
    public class CreateCurrency
    {

        public static Currency Create()
        {
            int num = 1;
            while (System.IO.File.Exists(EditorPrefs.GetString("DatabasePath") + "/Currencies/New Currency" + "(" + num.ToString() + ")" + ".asset"))
                num++;
            Currency asset = ScriptableObject.CreateInstance<Currency>();
            AssetDatabase.CreateAsset(asset, EditorPrefs.GetString("DatabasePath") + "/Currencies/New Currency(" + num.ToString() + ").asset");
            asset.currencyName = asset.name;
            AssetDatabase.SaveAssets();
            return asset;
        }
    }
}

