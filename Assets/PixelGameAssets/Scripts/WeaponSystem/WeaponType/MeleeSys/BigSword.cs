using System;
using System.Collections;
using PixelGameAssets.Scripts.WeaponSystem.WeaponInfo;
using UnityEngine;

namespace PixelGameAssets.Scripts.WeaponSystem.WeaponType.MeleeSys
{
    /// <summary>
    /// BigSword 具体剑, 非泛指剑名
    /// </summary>
    public class BigSword : Melee
    {
        public Projectile[] Projectile;
        
        public Transform gunBarrel;
        
        public virtual void Awake()
        {
            if (soundFxAtk == null)
            {
                soundFxAtk = "sword_atk_4";
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
            soundFxAtk = "sword_atk_state_" + meleeFrame.currentStage;
            meleeFrame.DoAnimation();
            StartCoroutine(MakeProjectile());
            base.TriggerWeapon();
        }

        public override void StopAttack()
        {
        }

        private IEnumerator MakeProjectile()
        {
            var p1 = Instantiate(Projectile[meleeFrame.currentStage - 1], gunBarrel.position, Quaternion.Euler(new Vector3(0f, 0f, CurrentAngle)));
            p1.owner = owner;
            yield return new WaitForSeconds(0.05f);
            var p2 = Instantiate(Projectile[meleeFrame.currentStage - 1], gunBarrel.position, Quaternion.Euler(new Vector3(0f, 0f, CurrentAngle)));
            p2.owner = owner;
            yield return new WaitForSeconds(0.05f);
            var p3 = Instantiate(Projectile[meleeFrame.currentStage - 1], gunBarrel.position, Quaternion.Euler(new Vector3(0f, 0f, CurrentAngle)));
            p3.owner = owner;
            yield return null;
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