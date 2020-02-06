using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EditorInfomation : ScriptableObject {
    public Color titleColor = Color.white;
    public Color labelColor = Color.white;
    public bool createRigidbody2D = true;
    public bool AutoChangeSprite = true;
}
