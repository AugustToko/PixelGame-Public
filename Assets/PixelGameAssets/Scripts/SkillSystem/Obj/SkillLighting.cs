using System;
using System.Collections.Generic;
using PixelGameAssets.Scripts.Actor.Enemies;
using PixelGameAssets.Scripts.Actor.Enemies.Base;
using PixelGameAssets.Scripts.Damage;
using UnityEngine;

namespace PixelGameAssets.Scripts.SkillSystem.Obj
{
    public class SkillLighting : SkillEntity
    {
        public Collider2D mCollider2D;

        public Rigidbody2D mRigidbody2D;

        public GameObject point;

        private void Start()
        {
            mCollider2D.enabled = false;
        }

        public void Attack()
        {
            mCollider2D.enabled = true;
            var pointPos = point.transform.position;

            var resu = new List<RaycastHit2D>();
            mRigidbody2D.Cast(Vector2.down, resu, 50f);

            foreach (var hit2D in resu)
            {
                if (!mCollider2D.enabled) continue;
                var enemy = hit2D.transform.GetComponent<Health>();
                if (enemy != null && enemy.Actor is Enemies) enemy.TakeDamage(50);
            }
        }

        public void AfterAttack()
        {
            mCollider2D.enabled = false;
        }

        public void Done()
        {
            mCollider2D.enabled = false;
            Destroy(this);
            Destroy(gameObject);
        }

    }
}