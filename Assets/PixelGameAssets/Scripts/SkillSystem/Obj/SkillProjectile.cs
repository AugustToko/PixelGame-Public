using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using PixelGameAssets.Scripts.Actor;
using PixelGameAssets.Scripts.Actor.Enemies;
using PixelGameAssets.Scripts.Actor.Enemies.Base;
using PixelGameAssets.Scripts.Damage;
using PixelGameAssets.Scripts.EventManager;
using UnityEngine;
using Random = UnityEngine.Random;

//[RequireComponent (typeof(Rigidbody2D))]
namespace PixelGameAssets.Scripts.SkillSystem.Obj
{
    public class SkillProjectile : SkillEntity
    {
        // 列出存储损坏的健康状况 (只允许对同一 Actor 造成一次伤害)
        private readonly List<Health> _healthsDamaged = new List<Health>();

        public bool canBreakProjectile = false;

        // 亚像素运动的计数器
        private Vector2 _movementCounter = Vector2.zero;

        // 当前位置
        private Vector2 _position;

        [Header("Speed")] public float baseSpeed;

        // 弹丸弹跳
        [Header("Bounce")] public bool BounceOnCollide;

        // 弹丸弹跳次数
        public int bouncesLeft = 1;

        // 伤害
        [Header("Damage")] public int DamageOnHit;

        public Vector2 direction;

        public GameObject dustFxPrefab;

        [Header("OnHit FX")] public GameObject hitFxPrefab;

        public BoxCollider2D myCollider;
        
        public float randomSpeed;

        private bool _isHitFxNull;


        public Vector2 SpeedV2;

        private bool _isDestroy = false;

        /// <summary>
        /// 子弹存活时间
        /// </summary>
        private float _aliveTime = 3f;

        private void Awake()
        {
            if (myCollider == null) myCollider = GetComponent<BoxCollider2D>();
            _isHitFxNull = hitFxPrefab == null;
        }

        private void Start()
        {
            // keeping everything Pixel perfect
            var position = transform.position;
            _position = new Vector2(Mathf.Round(position.x), Mathf.Round(position.y));
            position = _position;
            transform.position = position;
        }

        private void Update()
        {
            _aliveTime -= Time.deltaTime;
            var right = transform.right;
            SpeedV2 = (baseSpeed + Random.value * randomSpeed) * Time.deltaTime * new Vector2(right.x, right.y);
            if (_aliveTime <= 0)
            {
                DestroyMe();
            }
        }

//        private void LateUpdate()
//        {
//            if (Math.Abs(SpeedV2.x) > 0) MoveH(SpeedV2.x);
//
//            if (Math.Abs(SpeedV2.y) > 0) MoveV(SpeedV2.y);
//        }

        private void FixedUpdate()
        {
            if (Math.Abs(SpeedV2.x) > 0) MoveH(SpeedV2.x);

            if (Math.Abs(SpeedV2.y) > 0) MoveV(SpeedV2.y);
            
        }

        [SuppressMessage("ReSharper", "Unity.InefficientPropertyAccess")]
        private void DestroyMe()
        {
            if (_isDestroy)
            {
                return;
            }

            _isDestroy = true;

            if (_isHitFxNull)
            {
                Destroy(gameObject);
                return;
            }
            var h = Instantiate(hitFxPrefab, transform.position, transform.rotation);
            h.transform.localScale = transform.lossyScale;
            h.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, Random.value * 360f));

            Destroy(gameObject);
        }

        [SuppressMessage("ReSharper", "Unity.InefficientPropertyAccess")]
        public void DestroyMeWall()
        {
            if (_isDestroy)
            {
                return;
            }

            _isDestroy = true;

            if (_isHitFxNull)
            {
                Destroy(gameObject);
                return;
            }

            var h = Instantiate(hitFxPrefab, transform.position, transform.rotation);
            h.transform.localScale = transform.lossyScale;
            h.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, Random.value * 360f));

            Destroy(gameObject);
        }

        private void OnCollisionEnter(Collision other)
        {
            Debug.Log("OnCollisionEnter");
        }

        [SuppressMessage("ReSharper", "Unity.InefficientPropertyAccess")]
        private void BounceHorizontal()
        {
            bouncesLeft--;
            var right = transform.right;
            right = new Vector3(-right.x, right.y, right.z);
            transform.right = right;
            SpeedV2 *= 0.8f;
        }

        [SuppressMessage("ReSharper", "Unity.InefficientPropertyAccess")]
        private void BounceVertical()
        {
            bouncesLeft--;
            var right = transform.right;
            right = new Vector3(right.x, -right.y, right.z);
            transform.right = right;
            SpeedV2 *= 0.8f;
        }

        private void OnCollideWith(Component col, bool horizontalCol = true)
        {
            Debug.Log("OnCollideWith");
            var component = col.GetComponent<Health>();

            // 如果 "hitbox" 与之碰撞的目标具有健康组件且它不是我们的所有者，并且它不在当前 "hitbox" 损坏的健康列表上
            if (component != null && component != owner && !_healthsDamaged.Contains(component))
            {
                // Add the health component to the list of damaged healths
                _healthsDamaged.Add(component);

                // Apply the damage
                var didDamage = component.TakeDamage(DamageOnHit);
                // Destroy the projectile after applying damage
                if (didDamage)
                {
                    if (component.Actor is Enemies)
                    {
                        EventCenter.Broadcast(EventManager.EventType.UpdateHits, 0);
                    }

                    DestroyMe();
                    return;
                }
            }

            // 如果射弹撞击到固体物体，则将其摧毁
            if (col.gameObject.layer != (int) Mathf.Log(solid_layer.value, 2)) return;
            DestroyMeWall();
        }

        /// <summary>
        /// 与实体碰撞
        /// </summary>
        /// <param name="col">组件</param>
        private void OnCollideWithEntity(Component col)
        {
            var healthComponent = col.GetComponent<Health>();

            // 如果 "hitBox" 与之碰撞的目标具有健康组件且它不是我们的所有者，并且它不在当前 "hitbox" 损坏的健康列表上
            if (_healthsDamaged.Contains(healthComponent) || healthComponent == null || healthComponent == owner)
                return;

            // Enemies 不允许相互伤害
            if (owner.Actor is Enemies && healthComponent.Actor is Enemies) return;

            // 将运行状况组件添加到损坏的运行状况列表中
            _healthsDamaged.Add(healthComponent);

            // 应用伤害
            var didDamage = healthComponent.TakeDamage(DamageOnHit);

            if (!didDamage) return;

            // 更新连击数
            if (owner.Actor is CommonPlayer && healthComponent.Actor is Enemies)
            {
                EventCenter.Broadcast(EventManager.EventType.UpdateHits, 0);
            }

            // 在施加伤害后摧毁抛射物
            DestroyMe();
        }

        /// <summary>
        ///     用于水平移动Actor的函数，这仅存储要允许的移动的浮点值, 子像素移动并调用MoveHExact函数来进行实际移动
        /// </summary>
        /// <param name="moveH">水平移动值</param>
        /// <returns></returns>
        public bool MoveH(float moveH)
        {
            _movementCounter.x += moveH;
            var num = (int) Mathf.Round(_movementCounter.x);
            if (num == 0) return false;

            _movementCounter.x -= num;
            return MoveHExact(num);
        }

        public bool MoveV(float moveV)
        {
            _movementCounter.y += moveV;
            var num = (int) Mathf.Round(_movementCounter.y);
            if (num != 0)
            {
                _movementCounter.y = _movementCounter.y - num;
                return MoveVExact(num);
            }

            return false;
        }

        // 将Actor垂直移动一个精确整数的函数
        private bool MoveVExact(int moveV)
        {
            var num = (int) Mathf.Sign(moveV);
            while (moveV != 0)
            {
                var solid = CheckColInDir(Vector2.up * num, solid_layer);
                if (solid)
                {
                    if (BounceOnCollide && bouncesLeft > 0)
                    {
                        num = -num;
                        moveV = -moveV;
                        BounceVertical();
                    }
                    else
                    {
                        _movementCounter.x = 0f;
                        DestroyMeWall();
                        return true;
                    }
                }
                else
                {
                    var entity = CheckColInDir(Vector2.up * num, entities_layer);
                    if (entity)
                    {
                        var entit = CheckColsInDirAll(Vector2.up * num, entities_layer);
                        OnCollideWithEntity(entit[0]);
                    }
                }

                moveV -= num;
                var position = transform.position;
                position = new Vector2(position.x, position.y + num);
                // ReSharper disable once Unity.InefficientPropertyAccess
                transform.position = position;
            }

            return false;
        }


        // 将Actor水平移动一个精确整数的函数
        private bool MoveHExact(int moveH)
        {
            var num = (int) Mathf.Sign(moveH);
            while (moveH != 0)
            {
                // 层检测
                var solid = CheckColInDir(Vector2.right * num, solid_layer);
                if (solid)
                {
                    if (BounceOnCollide && bouncesLeft > 0)
                    {
                        num = -num;
                        moveH = -moveH;
                        BounceHorizontal();
                    }
                    else
                    {
                        _movementCounter.x = 0f;
                        DestroyMeWall();
                        return true;
                    }
                }
                else
                {
                    var entity = CheckColInDir(Vector2.right * num, entities_layer);
                    if (entity)
                    {
                        var entit = CheckColsInDirAll(Vector2.right * num, entities_layer);
                        OnCollideWithEntity(entit[0]);
                    }
                }

                moveH -= num;
                var position = transform.position;
                position = new Vector2(position.x + num, position.y);
                // ReSharper disable once Unity.InefficientPropertyAccess
                transform.position = position;
            }

            return false;
        }

        /// Helper function to check if there is any collision within a given layer in a set direction (only use up,
        /// down,left, right)
        /// 辅助函数，用于检查给定图层中是否存在任何碰撞设定方向（仅用于向上，向下，向左，向右）
        /// BUG: 存在墙裂缝, 如果子弹撞击到裂缝则不会按照玩家意愿进行反弹
        /// <see cref="BounceHorizontal" />
        /// <see cref="BounceHorizontal" />
        public bool CheckColInDir(Vector2 dir, LayerMask layer)
        {
            var leftCorner = Vector2.zero;
            var rightCorner = Vector2.zero;

            if (dir.x > 0)
            {
                var bounds = myCollider.bounds;

                leftCorner = new Vector2(bounds.center.x + bounds.extents.x,
                    bounds.center.y + bounds.extents.y - .1f);
                rightCorner = new Vector2(bounds.center.x + bounds.extents.x + .5f,
                    bounds.center.y - bounds.extents.y + .1f);
            }
            else if (dir.x < 0)
            {
                var bounds = myCollider.bounds;

                leftCorner = new Vector2(bounds.center.x - bounds.extents.x - .5f,
                    bounds.center.y + bounds.extents.y - .1f);
                rightCorner = new Vector2(bounds.center.x - bounds.extents.x,
                    bounds.center.y - bounds.extents.y + .1f);
            }
            else if (dir.y > 0)
            {
                var bounds = myCollider.bounds;

                leftCorner = new Vector2(bounds.center.x - bounds.extents.x + .1f,
                    bounds.center.y + bounds.extents.y + .5f);
                rightCorner = new Vector2(bounds.center.x + bounds.extents.x - .1f,
                    bounds.center.y + bounds.extents.y);
            }
            else if (dir.y < 0)
            {
                var bounds = myCollider.bounds;

                leftCorner = new Vector2(bounds.center.x - bounds.extents.x + .1f,
                    bounds.center.y - bounds.extents.y);
                rightCorner = new Vector2(bounds.center.x + bounds.extents.x - .1f,
                    bounds.center.y - bounds.extents.y - .5f);
            }

            return Physics2D.OverlapArea(leftCorner, rightCorner, layer);
        }


        // 与CheckColInDir相同，但它返回与您正在碰撞的碰撞器的Collider2D数组
        public Collider2D[] CheckColsInDirAll(Vector2 dir, LayerMask layer)
        {
            var leftCorner = Vector2.zero;
            var rightCorner = Vector2.zero;

            if (dir.x > 0)
            {
                var bounds = myCollider.bounds;

                leftCorner = new Vector2(bounds.center.x + bounds.extents.x,
                    bounds.center.y + bounds.extents.y - .1f);
                rightCorner = new Vector2(bounds.center.x + bounds.extents.x + .5f,
                    bounds.center.y - bounds.extents.y + .1f);
            }
            else if (dir.x < 0)
            {
                var bounds = myCollider.bounds;

                leftCorner = new Vector2(bounds.center.x - bounds.extents.x - .5f,
                    bounds.center.y + bounds.extents.y - .1f);
                rightCorner = new Vector2(bounds.center.x - bounds.extents.x,
                    bounds.center.y - bounds.extents.y + .1f);
            }
            else if (dir.y > 0)
            {
                var bounds = myCollider.bounds;

                leftCorner = new Vector2(bounds.center.x - bounds.extents.x + .1f,
                    bounds.center.y + bounds.extents.y + .5f);
                rightCorner = new Vector2(bounds.center.x + bounds.extents.x - .1f,
                    bounds.center.y + bounds.extents.y);
            }
            else if (dir.y < 0)
            {
                var bounds = myCollider.bounds;

                leftCorner = new Vector2(bounds.center.x - bounds.extents.x + .1f,
                    bounds.center.y - bounds.extents.y);
                rightCorner = new Vector2(bounds.center.x + bounds.extents.x - .1f,
                    bounds.center.y - bounds.extents.y - .5f);
            }

            return Physics2D.OverlapAreaAll(leftCorner, rightCorner, layer);
        }
    }
}