using System;
using PixelGameAssets.Scripts.Core;
using PixelGameAssets.Scripts.Core.Audio;
using PixelGameAssets.Scripts.SkillSystem;
using PixelGameAssets.Scripts.UI.UICanvas;
using PixelGameAssets.Scripts.WeaponSystem.WeaponInfo;
using PixelGameAssets.Scripts.WeaponSystem.WeaponSkill;
using PixelGameAssets.Scripts.WeaponSystem.WeaponType.GunSys.Base;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace PixelGameAssets.Scripts.WeaponSystem.WeaponType.GunSys
{
    /// <summary>
    /// 普通步枪
    /// </summary>
    public class Bow : Gun
    {
        public static readonly string TAG = "Bow";
        public Animator animator;

        private int _state = 1;

        public const int MAX_STATUS = 2;

        /// <summary>
        /// 初始化武器
        /// </summary>
        public override void Awake()
        {
            weaponType = WeaponType.Gun;
            if (!(weaponData is GunInfo))
            {
                weaponData = new GunInfo(weaponName,100);
            }
            else
            {
                throw new Exception("WeaponInfo is not the type of GunInfo.");
            }

            WeaponSkill = new WeaponSkillDetail.Builder(SkillType.WpNone, 0).Build();


            if (soundFxAtk == null || soundFxAtk.Equals(""))
            {
                soundFxAtk = "rifle_shoot";
            }

            base.Awake();
        }

        /// <summary>
        /// 尝试开火
        /// 判断是否处于冷却时间
        /// 判断是否有弹药
        /// </summary>
        /// <returns>是否开火成功</returns>
        public override bool TryToTriggerWeapon()
        {
            if (CooldownTimer > 0f) return false;

            if (weaponData is GunInfo gunInfo && gunInfo.clipCapacity == 0 && gunInfo.remainingBullet == 0)
            {
                AudioManager.Instance.PlayAudioEffectShoot(ammoEmptySoundFx);
                return false;
            }

            CooldownTimer = cooldown;

            if (animator != null && !animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
            {
                animator.Play("Attack");
                return true;
            }

            return false;
        }

        /// <summary>
        /// 直接开火, 跳过安全检查
        /// </summary>
        protected override void TriggerWeapon()
        {
            if (casingPrefab != null)
            {
                var c = Instantiate(casingPrefab, transform.position, Quaternion.identity);
                c.Facing = (Facings) transform.lossyScale.x;
            }

//            InstantiateMuzzleFlash();

            if (_state == 1)
            {
                InstantiateProjectile();
                _state++;
            }
            else if (_state == 2)
            {
                for (int i = 0; i < 3; i++)
                {
                    InstantiateProjectile(0);
                    InstantiateProjectile(30);
                    InstantiateProjectile(-30);
                }

                _state--;
            }
            
            base.TriggerWeapon();
        }

        public override void StopAttack()
        {
            
        }

        /// <summary>
        /// TODO: remove this
        /// </summary>
        public void TriggerWeaponPublic()
        {
            TriggerWeapon();
        }

        public override WeaponData GetDefaultInfo()
        {
            return new GunInfo(weaponName,100);;
        }
    }
}