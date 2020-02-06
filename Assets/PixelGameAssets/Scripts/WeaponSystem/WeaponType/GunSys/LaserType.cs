using UnityEngine;

namespace PixelGameAssets.Scripts.WeaponSystem.WeaponType.GunSys
{
    /// <summary>
    /// 激光接口
    /// </summary>
    public interface ILaserType
    {
        bool TryTriggerWeapon(Vector3 d);
    }
}