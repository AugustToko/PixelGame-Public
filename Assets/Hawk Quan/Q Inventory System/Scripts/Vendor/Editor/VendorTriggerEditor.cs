using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace QInventory
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(VendorTrigger))]
    public class VendorTriggerEditor : Editor
    {
        public override void OnInspectorGUI()
        {

            //VendorTrigger m_VendorTrigger = target as VendorTrigger;

            //GUILayout.Label("Vendor ID : " + m_VendorTrigger.GetVendorID());
            //GUILayout.Space(5);

            base.OnInspectorGUI();
        }
    }
}
