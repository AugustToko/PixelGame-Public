using System;
using PixelGameAssets.Scripts.Actor.Enemies;
using PixelGameAssets.Scripts.Core;
using PixelGameAssets.Scripts.Core.Audio;
using PixelGameAssets.Scripts.Damage;
using PixelGameAssets.Scripts.Entity.InteractableEntity;
using PixelGameAssets.Scripts.Entity.InteractableEntity.Pickup;
using PixelGameAssets.Scripts.EventManager;
using PixelGameAssets.Scripts.Misc;
using PixelGameAssets.Scripts.WeaponSystem.WeaponInfo;
using PixelGameAssets.Scripts.WeaponSystem.WeaponSkill;
using UnityEngine;
using UnityEngine.Serialization;
using EventType = PixelGameAssets.Scripts.EventManager.EventType;
using Random = System.Random;

namespace PixelGameAssets.Scripts.WeaponSystem.WeaponType
{
    public abstract class Weapon : MonoBehaviour
    {
        public enum WeaponType
        {
            None,
            MeleeWeapon,
            Gun,
            LaserGun,
            Bow
        }

        public string weaponName = "None";

        public WeaponType weaponType = WeaponType.None;

        /// <summary>
        /// 用取消装备武器时, 在地上产生同武器的 WeaponPickup (保存 WeaponData)
        /// </summary>
        public WeaponPickup weaponPickup;

        /// <summary>
        /// 武器冷却时间
        /// </summary>
        [Header("Fire Rate/CoolDown")] public float cooldown = .1f;

        /// <summary>
        /// 后坐力(角度)
        /// </summary>
        public float randomAngle = 20;

        /// <summary>
        /// 后坐力强度
        /// </summary>
        [Header("Camera Recoil")] public float recoilForce = 10f;

        public Health owner { set; get; }

        [Header("UI Icon")] public Sprite weaponUiIcon;

        [Header("Other")] public float zRotationOffSet;

        public GunTopDownZOrder zOrderComponent;

        [Header("Positions & Offsets")] public Vector2 offsetNormal;

        // 攻击音效
        public string soundFxAtk = null;

        // 子类必须修改
        public WeaponData weaponData { get; set; }

        public WeaponSkillDetail WeaponSkill { get; set; }

        // 武器冷却计时器
        protected float CooldownTimer;

        // 当前角度
        protected float CurrentAngle;

        //--------------- METHODS --------------

        public abstract bool TryToTriggerWeapon();

        /// <summary>
        /// 必须实现此方法, 然后在方法尾调用 base.TriggerWeapon()
        /// </summary>
        protected virtual void TriggerWeapon()
        {
            AudioManager.Instance.PlayAudioEffectShoot(soundFxAtk);
        }

        public abstract void StopAttack();

        public virtual void SetRotation(float angle)
        {
            CurrentAngle = angle;

            angle += zRotationOffSet;
            if (transform.lossyScale.x < 0f) angle = 180 - angle;

            //Weapon backwards or infront like in Nuclear Throne
            //if (angle > 25 && angle <= 90) {
            //	spriteRenderer.sortingOrder = -1;
            //} else {
            //	spriteRenderer.sortingOrder = 1;
            //}

            transform.localRotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }


        public abstract void HideWeapon();

        public abstract void ShowWeapon();

        /// <summary>
        /// 获取默认的武器数据
        /// </summary>
        /// <returns></returns>
        public abstract WeaponData GetDefaultInfo();

        public override string ToString()
        {
            return weaponName;
        }
    }
}