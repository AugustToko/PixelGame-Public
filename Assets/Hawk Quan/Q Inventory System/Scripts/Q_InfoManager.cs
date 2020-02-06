using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Q_InfoManager : MonoBehaviour {
    [TextArea(3, 100)]
    public string lockMoney;
    [TextArea(3, 100)]
    public string lockIngredient;
    [TextArea(3, 100)]
    public string craftingFailed;
    [TextArea(3, 100)]
    public string cantMoveWhenCD;
    [TextArea(3, 100)]
    public string inventoryFull;
    [TextArea(3, 100)]
    public string cantCrafting;
}
