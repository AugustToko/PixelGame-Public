using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Q_InventoryEmptyEditorWindow : EditorWindow {

    private static GUIStyle _labelStyle;

    protected static GUIStyle labelStyle
    {
        get
        {
            if (_labelStyle == null)
            {
                _labelStyle = new GUIStyle();
                _labelStyle.fontStyle = FontStyle.Bold;
                _labelStyle.fontSize = 30;
                _labelStyle.alignment = TextAnchor.MiddleCenter;
                _labelStyle.normal.textColor = Color.white;
            }
            return _labelStyle;
        }
    }

    //
    private static GUIStyle _tipLabelStyle;

    protected static GUIStyle tipLabelStyle
    {
        get
        {
            if (_tipLabelStyle == null)
            {
                _tipLabelStyle = new GUIStyle();
                //_tipLabelStyle.fontStyle = FontStyle.Bold;
                _tipLabelStyle.fontSize = 20;
                _tipLabelStyle.normal.textColor = Color.white;
            }
            return _tipLabelStyle;
        }
    }

    private static GUIStyle _normalCenterStyle;

    protected static GUIStyle normalCenterStyle
    {
        get
        {
            if (_normalCenterStyle == null)
            {
                _normalCenterStyle = new GUIStyle();
                _normalCenterStyle.fontStyle = FontStyle.Bold;
                _normalCenterStyle.fontSize = 15;
                _normalCenterStyle.alignment = TextAnchor.MiddleCenter;
                _normalCenterStyle.normal.textColor = Color.white;
            }
            return _normalCenterStyle;
        }
    }

    private static GUIStyle _normalHighlightStyle;

    protected static GUIStyle normalHighlightStyle
    {
        get
        {
            if (_normalHighlightStyle == null)
            {
                _normalHighlightStyle = new GUIStyle();
                _normalHighlightStyle.fontStyle = FontStyle.Bold;
                _normalHighlightStyle.fontSize = 15;
                _normalHighlightStyle.normal.textColor = Color.white;
            }
            return _normalHighlightStyle;
        }
    }
}
