using System;
using JetBrains.Annotations;
using PixelGameAssets.MonsterLove.StateMachine;
using PixelGameAssets.Scripts.Core;
using PixelGameAssets.Scripts.Damage;
using PixelGameAssets.Scripts.Helper;
using PixelGameAssets.Scripts.SkillSystem.Detail;
using PixelGameAssets.Scripts.WeaponSystem.WeaponType;
using PixelGameAssets.Scripts.WeaponSystem.WeaponType.GunSys;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PixelGameAssets.Scripts.Actor.Enemies
{
    public class EnemySkeletonBoss : Base.Enemies
    {
        public enum States
        {
            Spawn,
            Normal,
            Dead
        }

        [Header("Animator")] public Animator animator;

        public Transform weaponHolder;

        public Transform spriteHolder;

        public Vector2 direction;

        public float friction;

        // State Machine
        public StateMachine<States> fsm;

        public float moveSpeed;

        public LayerMask pit_layer;

        private float spawnTimer = 1.5f;

        [Header("Movement")] public int walk;

        protected override void Awake()
        {
            fsm = StateMachine<States>.Initialize(this);
            health = GetComponent<Health>();
            health.Actor = this;
            base.Awake();
        }

        private void Start()
        {
            alarm = 1 + (int) (Random.value * 20);
            fsm.ChangeState(States.Spawn);

            if (currWeapon != null)
            {
                var position = weaponHolder.position;

                currWeapon = Instantiate(currWeapon, new Vector2(position.x, position.y)
                                                     + currWeapon.offsetNormal, Quaternion.identity, weaponHolder);

                if (currWeapon.weaponData == null)
                {
                    currWeapon.weaponData = currWeapon.GetDefaultInfo();
                }

                currWeapon.owner = health;
                currWeapon.zOrderComponent.targetSpriteRenderer = spriteHolder.GetComponent<SpriteRenderer>();
            }
        }

        public override bool Attack([NotNull] ref Weapon weaponTarget)
        {
            if (allowAttack && !GameManager.Instance.Player.Health.dead)
            {
                if (currWeapon is ILaserType laserType)
                {
                    return laserType.TryTriggerWeapon(target.transform.position);
                }

                return weaponTarget.TryToTriggerWeapon();
            }

            return false;
        }

        public override bool TryUseSkill(SkillDetail skill)
        {
            throw new NotImplementedException();
        }

        public override void SetTakeHealth()
        {
        }

        public override void SetTakeHit(int amount)
        {
            // TODO: 事件
            base.SetTakeHit(amount);
        }

        private new void LateUpdate()
        {
            // Horizontal Movement
            var moveh = MoveH(Speed.x * Time.deltaTime);
            if (moveh)
                Speed.x = 0;

            // Vertical Movement
            var movev = MoveV(Speed.y * Time.deltaTime);
            if (movev)
                Speed.y = 0;

            UpdateSprite();
            
            base.LateUpdate();
        }

        private void UpdateSprite()
        {
            var targetScale = Facing == Facings.Right ? new Vector3(1f, 1f, 1f) : new Vector3(-1f, 1f, 1f);
            transform.localScale = targetScale;

            if (fsm.State == States.Spawn)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Spawn")) animator.Play("Spawn");
            }
            else if (fsm.State == States.Dead)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Death")) animator.Play("Death");
            }
            else if (Speed == Vector2.zero)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) animator.Play("Idle");
            }
            else
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Run")) animator.Play("Run");
            }
        }

        private void Spawn_Enter()
        {
            // just the time the spawn animation lasts for
            spawnTimer = 1.5f;
        }

        private void Spawn_Update()
        {
            if (spawnTimer > 0f)
            {
                spawnTimer -= Time.deltaTime;

                if (spawnTimer <= 0f) fsm.ChangeState(States.Normal);
            }
        }

        private void Normal_Update()
        {
            if (alarm > 0)
            {
                alarm--;
                if (alarm <= 0) Behaviour();
            }

            if (walk > 0)
            {
                walk--;
                Speed = direction * moveSpeed * Time.deltaTime;
            }

            if (Speed != Vector2.zero)
            {
                // Turn around on pits
                var myPos = new Vector2(transform.position.x, transform.position.y);
                var targetPlace = direction * 12f;
                if (CheckColAtPlace(direction * 12f, pit_layer) ||
                    !LineOfSight(myPos, myPos + targetPlace, pit_layer) ||
                    CollisionSelf(pit_layer))
                {
                    direction *= -1f;
                    Speed *= -1.4f;
                }

                // Horizontal Friction
                if (Speed.x != 0)
                    Speed.x = CalcHelper.Approach(Speed.x, 0, friction);

                // Vertical Friction
                if (Speed.y != 0)
                    Speed.y = CalcHelper.Approach(Speed.y, 0, friction);
            }
        }

        /// <summary>
        ///     行为管理
        ///     如果目标为空, 则默认为玩家目标
        /// </summary>
        private void Behaviour()
        {
            SetUpTarget();

            //var degrees = Calc.Vector2ToDegree ((targetPos - myPos).normalized);
            //degrees += Random.Range (-10, 10);
            //var vector = Calc.DegreeToVector2 (degrees);

            alarm = 20 + (int) (Random.value * 10);
            if (target != null)
            {
                var myPos = new Vector2(transform.position.x, transform.position.y);
                var targetPos = new Vector2(target.position.x, target.position.y);
                if (LineOfSight(myPos, targetPos, solid_layer) && LineOfSight(myPos, targetPos, pit_layer))
                {
                    if (Vector2.Distance(myPos, targetPos) > 24)
                    {
                        if (Random.value < 0.45f)
                        {
                            PointTowardsPlayer();
                            while (Random.value > 0.5f)
                            {
                                if (Attack(ref currWeapon)) alarm = 20 + (int) (Random.value * 5);
                            }
                        }
                        else
                        {
                            var degrees = CalcHelper.Vector2ToDegree((targetPos - myPos).normalized);
                            degrees += Random.Range(-90, 90);
                            var vector = CalcHelper.DegreeToVector2(degrees);
                            direction = vector;
                            walk = 10 + (int) (Random.value * 10);
                            PointTowardsPlayer();
                        }
                    }
                    else
                    {
                        var degrees = CalcHelper.Vector2ToDegree((myPos - targetPos).normalized);
                        degrees += Random.Range(-10, 10);
                        var vector = CalcHelper.DegreeToVector2(degrees);
                        direction = vector;
                        walk = 40 + (int) (Random.value * 10);
                        PointTowardsPlayer();
                        while (Random.value > 0.3f)
                        {
                            if (Attack(ref currWeapon)) alarm = 20 + (int) (Random.value * 5);
                        }
                    }

                    if (targetPos.x < myPos.x) Facing = Facings.Left;
                    else if (targetPos.x > myPos.x) Facing = Facings.Right;
                }
                else if (Random.value < 0.25f)
                {
                    var degrees = Random.value * 360f;
                    var vector = CalcHelper.DegreeToVector2(degrees);
                    direction = vector;
                    walk = 20 + (int) (Random.value * 10);
                    alarm = walk + 10 + (int) (Random.value * 30);
                    PointTowardsDirection(vector);

                    if (Speed.x < 0)
                        Facing = Facings.Left;
                    else if (Speed.x > 0) Facing = Facings.Right;
                }
            }
            else if (Random.value < 0.1f)
            {
                var degrees = Random.value * 360f;
                var vector = CalcHelper.DegreeToVector2(degrees);
                direction = vector;
                walk = 20 + (int) (Random.value * 10);
                alarm = walk + 10 + (int) (Random.value * 30);
                PointTowardsDirection(vector);

                if (Speed.x < 0)
                    Facing = Facings.Left;
                else if (Speed.x > 0) Facing = Facings.Right;
            }
        }

        public bool LineOfSight(Vector2 startPosition, Vector2 targetPosition, LayerMask layer)
        {
            var result = true;

            var hit = Physics2D.Linecast(startPosition, targetPosition, layer);

            if (hit.collider != null)
                result = false;

            return result;
        }

        public void PointTowardsPlayer()
        {
            var myPos = new Vector2(transform.position.x, transform.position.y);
            var targetPos = new Vector2(target.position.x, target.position.y);
            var vector = (targetPos - myPos).normalized;
            var angle = Mathf.Atan2(targetPos.y - myPos.y, targetPos.x - myPos.x) * 180 / Mathf.PI;
            if (angle < 0) angle += 360f;

            currWeapon.SetRotation(angle);
        }

        public void PointTowardsDirection(Vector2 dir)
        {
            var myPos = new Vector2(transform.position.x, transform.position.y);
            var targetPos = new Vector2(myPos.x + dir.x * 999, myPos.y * (dir.y * 999));
            var vector = (targetPos - myPos).normalized;
            var angle = Mathf.Atan2(targetPos.y - myPos.y, targetPos.x - myPos.x) * 180 / Mathf.PI;
            if (angle < 0) angle += 360f;

            currWeapon.SetRotation(angle);
        }

        public override void Die()
        {
            if (ScoreManager.Instance != null) ScoreManager.Instance.AddScore(scoreOnDeath);
            ExpManager.Instance.AddExp(3);
            myCollider.enabled = false;
            fsm.ChangeState(States.Dead, StateTransition.Overwrite);
            base.Die();
        }
    }
}