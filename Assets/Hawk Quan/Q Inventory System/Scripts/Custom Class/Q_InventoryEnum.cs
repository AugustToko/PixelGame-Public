using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QInventory
{
    public enum Effect : uint
    {
        Restore,
        Decrease,
        IncreaseMaxValue,
        DecreaseMaxValue
    }

    public enum Rarity : uint
    {
        Junk,
        Normal,
        Rare,
        Unique,
        Legendary
    }

    public enum Variety : uint
    {
        Consumable,
        Equipment,
        Material,
    }

    public enum EquipmentPart : uint
    {
        None,
        RightHand,
        LeftHand,
        Body,
        Head,
        Neck,
        RightFoot,
        LeftFoot
    }

    public enum ShowType : uint
    {
        None,
        CurrentValue,
        CurrentValueWithMaxValue,
        MaxValue,
        MinValue
    }

    public enum DropType : uint
    {
        DropByThrown,
        DropWithMouse,
        Drop3D
    }

    public enum PickUp : uint
    {
        Pick2D,
        Pick3D
    }

    public enum ObjectType : uint
    {
        _2DGameObject,
        EmptyGameObject,
        EmptyObject,
        //_3DGameObject
    }

    public enum ItemShownType : uint
    {
        All,
        Consumable,
        Equipment,
        Material
    }

    public enum SetType : uint
    {
        CurrentValue,
        MaxValue,
        MinValue
    }

    public enum ConsumableItemType : uint
    {
        None,
        UnlockableBluePrint
    }

}


