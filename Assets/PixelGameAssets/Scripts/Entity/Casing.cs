using System;
using PixelGameAssets.Scripts.Actor;
using PixelGameAssets.Scripts.Actor.ControllerSys;
using PixelGameAssets.Scripts.Actor.Enemies;
using PixelGameAssets.Scripts.Core;
using PixelGameAssets.Scripts.Helper;
using PixelGameAssets.Scripts.SkillSystem;
using PixelGameAssets.Scripts.SkillSystem.Detail;
using PixelGameAssets.Scripts.WeaponSystem.WeaponType;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PixelGameAssets.Scripts.Entity
{
    public class Casing : EnemiesController
    {
        public Vector2 direction;
        public float friction = 1f;
        public float maxSpeed = 100f;

        public float minSpeed = 50f;
        public float speed = 100f;

        //public int fadeFrames = 40;

        private void Start()
        {
            transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, Random.value * 360f));

            direction = CalcHelper.DegreeToVector2(Random.Range(105f, 165f));
            direction.x *= (float) Facing;

            speed = Random.Range(minSpeed, maxSpeed);
            Speed = speed * direction;
        }

        private void Update()
        {
//            if (fadeFrames > 0)
//            {
//                fadeFrames--;

//                if (fadeFrames <= 0)
//                {
//                    Destroy(gameObject);
//                }
//            }

            if (Speed != Vector2.zero)
            {
                if (Speed.x != 0)
                    Speed.x = CalcHelper.Approach(Speed.x, 0, friction * Time.deltaTime);

                if (Speed.y != 0)
                    Speed.y = CalcHelper.Approach(Speed.y, 0, friction * Time.deltaTime);
            }
        }

        private void LateUpdate()
        {
            // Horizontal Movement
            var moveh = MoveH(Speed.x * Time.deltaTime);
            if (moveh)
                Speed.x = 0;

            // Vertical Movement
            var movev = MoveV(Speed.y * Time.deltaTime);
            if (movev)
                Speed.y = 0;
        }

        public override bool Attack(ref Weapon weaponTarget)
        {
            throw new NotImplementedException();
        }

        public override bool TryUseSkill(SkillDetail skill)
        {
            throw new NotImplementedException();
        }

        public override void SetTakeHealth()
        {
            throw new NotImplementedException();
        }

        public override void SetTakeHit(int amount)
        {
            throw new NotImplementedException();
        }

        public override void Die()
        {
            throw new NotImplementedException();
        }
    }
}