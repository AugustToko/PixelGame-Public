using System;

namespace PixelGameAssets.Scripts.WeaponSystem.WeaponInfo
{
    /// <summary>
    /// 武器信息根类
    /// </summary>
    [Serializable]
    public abstract class WeaponData
    {
        /// <summary>
        /// 武器名称, 加载存档用
        /// </summary>
        public string weaponName = "None";

        /// <summary>
        /// TODO: 武器等级
        /// </summary>
        public byte level = 1;

        /// <summary>
        /// TODO: 武器稀有度
        /// </summary>
        public Rareness rareness = Rareness.Normal;

        public override string ToString()
        {
            return "WeaponName: " + weaponName + ", level: " + level + ", Rareness: " + rareness;
        }
    }
}