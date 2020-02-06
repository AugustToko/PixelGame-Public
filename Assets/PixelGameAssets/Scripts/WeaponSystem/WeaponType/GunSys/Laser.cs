using System;
using System.Collections.Generic;
using PixelGameAssets.Scripts.Core;
using PixelGameAssets.Scripts.Damage;
using PixelGameAssets.Scripts.EventManager;
using PixelGameAssets.Scripts.SkillSystem;
using PixelGameAssets.Scripts.UI.UICanvas;
using PixelGameAssets.Scripts.WeaponSystem.WeaponInfo;
using PixelGameAssets.Scripts.WeaponSystem.WeaponSkill;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PixelGameAssets.Scripts.WeaponSystem.WeaponType.GunSys
{
    /// <summary>
    /// 目前为泛指 激光武器
    /// TODO: 修改架构, 改为激光武器父类架构
    /// </summary>
    public class Laser : Weapon, ILaserType
    {
        public static readonly string TAG = "Laser";

        public LineRenderer lineRenderer;

        public Transform gunBarrel;

        public SpriteRenderer spriteRenderer;

        public WeaponData defInfo;

        public LayerMask layerMask;

        public GameObject fx;

        public int damage = 1;

        private void Awake()
        {
            if (!(weaponData is GunInfo))
            {
                defInfo = new GunInfo(weaponName, 500);
                weaponData = defInfo;
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
        }

        private void Update()
        {
            float width = Random.Range(2, 7);
            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width;
        }

        public override bool TryToTriggerWeapon()
        {
            TriggerWeapon();
            return true;
        }

        public bool TryTriggerWeapon(Vector3 d)
        {
            return TriggerWeapon(d);
        }

        public override void StopAttack()
        {
            lineRenderer.enabled = false;
        }

        protected override void TriggerWeapon()
        {
            TriggerWeapon(UnityEngine.Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }

        public bool TriggerWeapon(Vector3 d)
        {
            lineRenderer.enabled = true;
            var dir = d - gunBarrel.position;

            var hit = Physics2D.Raycast(gunBarrel.position, dir.normalized, 5400f);

            //光线投射，返回障碍物
            if (hit && lineRenderer.enabled) //如果遇到障碍物且射线打开
            {
                var scaleX = Vector3.Distance(hit.point, gunBarrel.position);

                Instantiate(fx, hit.point, Quaternion.Euler(Vector3.zero)); //在障碍物处产生爆炸效果

                var enemy = hit.transform.GetComponent<Health>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }

                //射线的起始点
                lineRenderer.SetPosition(0, new Vector3(gunBarrel.position.x, gunBarrel.position.y, 20));
                //因为激光只有一个终点，所以障碍物位置为终点
                lineRenderer.SetPosition(1, new Vector3(hit.point.x, hit.point.y, 20));
            }
            else
            {
                lineRenderer.SetPosition(0, gunBarrel.position);
                lineRenderer.SetPosition(1, d);
            }

            return true;
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
                EventCenter.Broadcast<int, int>(EventManager.EventType.UpdateAmmo, info.clipCapacity,
                    info.remainingBullet);
            }
        }

        public override WeaponData GetDefaultInfo()
        {
            return new GunInfo(weaponName, 500);;
        }
    }
}