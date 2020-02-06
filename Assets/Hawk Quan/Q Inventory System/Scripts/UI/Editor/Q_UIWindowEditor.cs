using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace QInventory
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Q_UIWindow))]
    public class Q_UIWindowEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            Q_UIWindow m_UIWindow = (Q_UIWindow)target;
            base.OnInspectorGUI();
            GUILayout.Space(10);
            if (GUILayout.Button("Set Position"))
            {
                m_UIWindow.SetUIposition();
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }
    }
}
