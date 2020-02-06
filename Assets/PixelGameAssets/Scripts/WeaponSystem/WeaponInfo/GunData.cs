using System;

namespace PixelGameAssets.Scripts.WeaponSystem.WeaponInfo
{
    /// <summary>
    /// 武器信息 (枪械)
    /// </summary>
    [Serializable]
    public class GunInfo : WeaponData
    {
        // 备用子弹数量
        public int remainingBullet;

        // 当前弹夹容量
        public int clipCapacity;

        // 一次射出所消耗的弹药
        public int shootAmmo = 1;

        // 固定弹夹容量
        public int maxClip = 25;

        public GunInfo(string weaponName, int ammo, int clipCapacity = 25, int shootAmmo = 1)
        {
            this.weaponName = weaponName;
            this.clipCapacity = clipCapacity;
            maxClip = clipCapacity;
            remainingBullet = ammo - clipCapacity;
            this.shootAmmo = shootAmmo;
        }

        public override string ToString()
        {
            return "WeaponName: " + weaponName + ", level: " + level + ", Rareness: " + rareness +
                   ", remainingBullet: " + remainingBullet + ", clipCapacity: " + clipCapacity + ", maxClip: " +
                   maxClip + ", shootAmmo: " + shootAmmo;
        }
    }
}