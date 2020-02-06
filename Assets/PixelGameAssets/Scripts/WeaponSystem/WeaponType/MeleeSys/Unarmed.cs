using PixelGameAssets.Scripts.WeaponSystem.WeaponInfo;

namespace PixelGameAssets.Scripts.WeaponSystem.WeaponType.MeleeSys
{
    public class Unarmed : Sword
    {
        public override bool TryToTriggerWeapon()
        {
            return true;
        }

        public override void SetRotation(float angle)
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
            throw new System.NotImplementedException();
        }
    }
}