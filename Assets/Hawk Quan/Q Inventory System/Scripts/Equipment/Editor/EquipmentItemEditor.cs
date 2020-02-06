using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(EquipmentItem))]
public class EquipmentItemEditor : Editor
{
    SerializedProperty equipmentPosition;
    SerializedProperty equipmentRotation;
    private void OnEnable()
    {
        equipmentPosition = serializedObject.FindProperty("equipmentPosition");
        equipmentRotation = serializedObject.FindProperty("equipmentRotation");
    }

    public override void OnInspectorGUI()
    {
        var equipmentItem = target as EquipmentItem;
        EditorGUILayout.PropertyField(equipmentPosition);
        EditorGUILayout.PropertyField(equipmentRotation);
        GUILayout.Space(50f);
        if(GUILayout.Button("Set Position"))
        {
            equipmentItem.equipmentPosition = equipmentItem.transform.position;
            equipmentItem.equipmentRotation = equipmentItem.transform.rotation.eulerAngles;
        }
    }
}
