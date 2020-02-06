using System;
using PixelGameAssets.Scripts.Actor;
using PixelGameAssets.Scripts.Camera;
using PixelGameAssets.Scripts.Core.Audio;
using PixelGameAssets.Scripts.Entity;
using PixelGameAssets.Scripts.EventManager;
using PixelGameAssets.Scripts.WeaponSystem.WeaponInfo;
using UnityEngine;
using EventType = PixelGameAssets.Scripts.EventManager.EventType;
using Random = System.Random;
using PixelGameAssets.Scripts.Actor.Enemies;
using PixelGameAssets.Scripts.Actor.Enemies.Base;

namespace PixelGameAssets.Scripts.WeaponSystem.WeaponType.GunSys.Base
{
    public abstract class Gun : Weapon
    {
        // 空弹夹却尝试开火的音效
        protected string ammoEmptySoundFx { get; set; }
        
        // 换弹夹
        public string soundFxReloading { get; set; }

        // 弹壳
        [Header("Prefabs")] public Casing casingPrefab;

        // 弹药
        public Projectile projectile;

        public Transform spriteHolder, gunBarrel;

        public SpriteRenderer spriteRenderer;

        // 枪口闪光
        [Obsolete("已使用动画闪光来代替实体闪光")]
        public GameObject muzzleFlash;

        /// <summary>
        /// 更新
        /// </summary>
        /// <exception cref="Exception"></exception>
        public virtual void Awake()
        {
            //TODO: 差异化音效，而非统一
            ammoEmptySoundFx = "empty_ammo";
            
            if (soundFxAtk == null || soundFxAtk.Equals(""))
            {
                soundFxAtk = "rifle_shoot";
            }
        }

        public virtual void Update()
        {
            if (CooldownTimer > 0f) CooldownTimer -= Time.deltaTime;
        }

        protected override void TriggerWeapon()
        {
            base.TriggerWeapon();
            
            if (!(owner.Actor is Enemies))
            {
                UpdateAmmo();
            }
            if (!(owner.Actor is Enemies))
            {
                AudioManager.Instance.AddToRandomFxSource("子弹掉落_" + new Random().Next(1, 6));
            }
        }

        /// <summary>
        /// 更新子弹数量及 UI
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">当前弹夹数量小于 0</exception>
        /// <exception cref="Exception"></exception>
        private void UpdateAmmo()
        {
            if (weaponData is GunInfo info)
            {
                // 当前弹夹为空, 但是备用弹夹不为空
                if (info.clipCapacity == 0 && info.remainingBullet > 0)
                {
                    if (info.remainingBullet < info.maxClip)
                    {
                        info.clipCapacity = info.remainingBullet;
                        info.remainingBullet = 0;
                    }
                    else
                    {
                        info.clipCapacity = info.maxClip;
                        info.remainingBullet -= info.maxClip;
                    }
                }

                info.clipCapacity -= info.shootAmmo;

                if (info.clipCapacity == 0 && info.remainingBullet > 0)
                {
                    info.clipCapacity = info.maxClip;
                    info.remainingBullet -= info.maxClip;
                }
                else if (info.clipCapacity < 0)
                    throw new ArgumentOutOfRangeException(nameof(info.clipCapacity));

            }
            else
            {
                throw new Exception("WeaponInfo is not the type of GunInfo.");
            }
        }
        
        /// <summary>
        ///  实例化弹丸 (带角度)
        /// </summary>
        protected void InstantiateProjectile(float amount)
        {
            if (CameraShaker.Instance != null && owner.Actor is CommonPlayer) CameraShaker.InitShake(0.07f, recoilForce);
            
            var p = Instantiate(projectile, gunBarrel.position,
                Quaternion.Euler(new Vector3(0f, 0f, CurrentAngle + amount)));

            p.owner = owner;
        }
        
        /// <summary>
        ///     实例化弹丸
        /// </summary>
        protected virtual void InstantiateProjectile()
        {
            if (CameraShaker.Instance != null && owner.Actor is CommonPlayer) CameraShaker.InitShake(0.07f, recoilForce);

            // 设置武器发射子弹的角度 (用随机来模拟后坐力)
            var amount = UnityEngine.Random.Range(-randomAngle, randomAngle);
            
            var p = Instantiate(projectile, gunBarrel.position,
                Quaternion.Euler(new Vector3(0f, 0f, CurrentAngle + amount)));

            p.owner = owner;
        }

        /// <summary>
        /// 实例枪口闪光
        /// </summary>
        [Obsolete("已使用动画闪光来替代实体闪光")]
        private void InstantiateMuzzleFlash()
        {
            if (muzzleFlash == null) return;
            var m = Instantiate(muzzleFlash, gunBarrel.position, gunBarrel.rotation);
            var localScale = m.transform.localScale;
            localScale = new Vector3(transform.lossyScale.x, localScale.y, localScale.z);
            m.transform.localScale = localScale;
        }

        public override void HideWeapon()
        {
            spriteRenderer.enabled = false;
        }

        public override void ShowWeapon()
        {
            spriteRenderer.enabled = true;
            if (weaponData is GunInfo info)
            {
                EventCenter.Broadcast<int, int>(EventType.UpdateAmmo, info.clipCapacity, info.remainingBullet);
            }
        }
    }
}