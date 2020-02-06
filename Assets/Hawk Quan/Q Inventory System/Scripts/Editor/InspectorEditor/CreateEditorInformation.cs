using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CreateEditorInformation {
    public static EditorInfomation Create()
    {
        EditorInfomation asset = ScriptableObject.CreateInstance<EditorInfomation>();
        AssetDatabase.CreateAsset(asset, EditorPrefs.GetString("DatabasePath") + "/Editor/EditorInformation.asset");
        AssetDatabase.SaveAssets();
        return asset;
    }
}
