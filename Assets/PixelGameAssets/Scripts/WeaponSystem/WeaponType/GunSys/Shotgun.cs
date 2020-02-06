using System;
using PixelGameAssets.Scripts.Camera;
using PixelGameAssets.Scripts.Core;
using PixelGameAssets.Scripts.Core.Audio;
using PixelGameAssets.Scripts.SkillSystem;
using PixelGameAssets.Scripts.WeaponSystem.WeaponInfo;
using PixelGameAssets.Scripts.WeaponSystem.WeaponSkill;
using UnityEngine;

namespace PixelGameAssets.Scripts.WeaponSystem.WeaponType.GunSys
{
    /// <summary>
    /// 散弹
    /// </summary>
    public class Shotgun : CommonGun
    {
        public override void Awake()
        {
            weaponType = WeaponType.Gun;
            if (weaponData == null)
            {
                weaponData = new GunInfo(weaponName, 10000, 25, 5);
            }
            else
            {
                throw new Exception("WeaponInfo is not the type of GunInfo.");
            }

            WeaponSkill = new WeaponSkillDetail.Builder(SkillType.WpBurst, 0).SetColdDownTime(0).Build();

            if (soundFxAtk == null || soundFxAtk.Equals(""))
            {
                soundFxAtk = "shot_gun_shoot";
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


        /// <summary>
        /// 实例化弹丸, 过程:
        /// 1. 摄像机震动
        /// 2. 实例化弹丸 (5次)
        /// </summary>
        protected override void InstantiateProjectile()
        {
            if (CameraShaker.Instance != null) CameraShaker.InitShake(0.1f, 2f);

            for (var i = 0; i < 5; i++) base.InstantiateProjectile();
        }

        /// <summary>
        /// 范围打击
        /// </summary>
        public void RangeAtk()
        {
            for (var i = 0; i < 360; i += 10) InstantiateProjectile(i);
        }

        public override WeaponData GetDefaultInfo()
        {
            return new GunInfo(weaponName, 10000, 25, 5);
        }
    }
}