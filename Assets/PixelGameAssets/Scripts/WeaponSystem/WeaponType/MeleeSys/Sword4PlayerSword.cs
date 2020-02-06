using PixelGameAssets.Scripts.WeaponSystem.WeaponInfo;

namespace PixelGameAssets.Scripts.WeaponSystem.WeaponType.MeleeSys
{
    /// <summary>
    /// PlayerSword 专用
    /// </summary>
    public class Sword4PlayerSword : Melee
    {
        public override bool TryToTriggerWeapon()
        {
            return true;
        }

        public override void StopAttack()
        {
        }

        public override void HideWeapon()
        {
        }

        public override void ShowWeapon()
        {
        }

        public override WeaponData GetDefaultInfo()
        {
            return null;
        }
    }
}