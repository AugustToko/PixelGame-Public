using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QInventory
{
    [System.Serializable]
    public class BluePrintToAdd
    {
        public CraftingBluePrint bluePrint;
        [HideInInspector]
        public bool isMoved = false;
        public BluePrintToAdd(CraftingBluePrint bluePrint, bool isMoved)
        {
            this.bluePrint = bluePrint;
            this.isMoved = isMoved;
        }
    }
}
