using System;
using System.Collections.Generic;
using PixelGameAssets.Scripts.Actor.Enemies;
using PixelGameAssets.Scripts.Actor.Enemies.Base;
using PixelGameAssets.Scripts.Damage;
using UnityEngine;

namespace PixelGameAssets.Scripts.WeaponSystem.WeaponType.MeleeSys
{
    public abstract class Melee : Weapon
    {
        /// <summary>
        /// 存储已伤害过的组件
        /// </summary>
        public List<Component> components = new List<Component>();
        
        public SpriteRenderer spriteRenderer;

        public MeleeFrame meleeFrame;
        
        public Collider2D Collider2D;

        /// <summary>
        /// 不统状态的伤害
        /// </summary>
        public int[] damage;

        public void TryTakeAttackDamage(Component c, int state = 0)
        {
            if (components.Contains(c))
            {
                return;
            }
            
            var health = c.GetComponent<Health>();

            if (health != null && health.Actor is Enemies)
            {
                health.TakeDamage(damage[0]);
                components.Add(c);
            }
            
            var projectile = c.GetComponent<Projectile>();
            
            if (projectile != null && projectile.owner.Actor is Enemies)
            {
                projectile.DestroyMe();
            }
        }
    }
}