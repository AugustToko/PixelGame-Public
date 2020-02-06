using System;
using PixelGameAssets.Scripts.Core;
using PixelGameAssets.Scripts.Core.Audio;
using PixelGameAssets.Scripts.SkillSystem;
using PixelGameAssets.Scripts.WeaponSystem.WeaponInfo;
using PixelGameAssets.Scripts.WeaponSystem.WeaponSkill;
using UnityEngine;

namespace PixelGameAssets.Scripts.WeaponSystem.WeaponType.GunSys
{
    /// <summary>
    /// 普通步枪
    /// </summary>
    public class Rifle : CommonGun
    {
        public static readonly string TAG = "Rifle";

        public WeaponData defInfo;

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
            TriggerWeapon();

            return true;
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
            InstantiateProjectile();

            base.TriggerWeapon();
        }

        public override void StopAttack()
        {
            
        }

        public override WeaponData GetDefaultInfo()
        {
            return new GunInfo(weaponName,100);;
        }
    }
}