using PixelGameAssets.Scripts.WeaponSystem.WeaponInfo;

namespace PixelGameAssets.Scripts.WeaponSystem.WeaponType.MeleeSys
{
    public class Sword : Melee
    {
        public virtual void Awake()
        {
            if (soundFxAtk == null)
            {
                soundFxAtk = "shot_gun_shoot";
            }
        }

        public override bool TryToTriggerWeapon()
        {
            if (!meleeFrame.animationDone) return false;
            
            meleeFrame.DoAnimation();
            TriggerWeapon();
            return true;

        }

        protected override void TriggerWeapon()
        {
            meleeFrame.DoAnimation();
            base.TriggerWeapon();
        }

        public override void StopAttack()
        {
            
        }

        public override void HideWeapon()
        {
            spriteRenderer.enabled = false;
        }

        public override void ShowWeapon()
        {
            spriteRenderer.enabled = true;
        }

        public override WeaponData GetDefaultInfo()
        {
            return new MeleeWeaponData();
        }
    }
}