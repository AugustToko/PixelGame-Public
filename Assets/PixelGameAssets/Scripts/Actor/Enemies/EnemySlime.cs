using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using PixelGameAssets.MonsterLove.StateMachine;
using PixelGameAssets.Scripts.Core;
using PixelGameAssets.Scripts.Damage;
using PixelGameAssets.Scripts.Helper;
using PixelGameAssets.Scripts.SkillSystem.Detail;
using PixelGameAssets.Scripts.WeaponSystem.WeaponType;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PixelGameAssets.Scripts.Actor.Enemies
{
    /// <summary>
    /// 碰撞伤害类型怪物 (泛指)
    /// </summary>
    public class EnemySlime : Base.Enemies
    {
        // States for the state machine
        public enum States
        {
            Normal,
            Attack,
            OnHit,
            Dead
        }

        [Header("Animator")] public Animator animator;
        public Vector2 direction;
        public float friction;

        // State Machine
        public StateMachine<States> fsm;
        public float moveSpeed;

        public LayerMask pit_layer;

        [Header("Movement")] public int walk;

        private new void Awake()
        {
            health = GetComponent<Health>();
            health.Actor = this;
            fsm = StateMachine<States>.Initialize(this);
            base.Awake();
        }

        public override bool Attack(ref Weapon weaponTarget)
        {
            fsm.ChangeState(States.Attack);
            return true;
        }

        private IEnumerator Attack_Enter()
        {
            health.invincible = true;
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
            fsm.ChangeState(States.Normal);
        }

        private void Attack_Exit()
        {
            health.invincible = false;
        }

        #region OnHit

        private IEnumerator OnHit_Enter()
        {
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
            fsm.ChangeState(States.Normal);
        }

        private void OnHit_Exit()
        {
        }

        #endregion

        public override bool TryUseSkill(SkillDetail skill)
        {
            throw new NotImplementedException();
        }

        public override void SetTakeHealth()
        {
            // NONE
        }

        public override void SetTakeHit(int amount)
        {
            base.SetTakeHit(amount);
            fsm.ChangeState(States.OnHit);
        }

        // Use this for initialization
        private void Start()
        {
            alarm = 1 + (int) (Random.value * 20);
            fsm.ChangeState(States.Normal);
        }

        private void Normal_Update()
        {
            if (!allowMove) return;

            if (alarm > 0)
            {
                alarm--;
                if (alarm <= 0) Behaviour();
            }

            if (walk > 0)
            {
                walk--;
                Speed = moveSpeed * Time.deltaTime * direction;
            }

            if (Speed != Vector2.zero)
            {
                // Turn around on pits
                var myPos = new Vector2(transform.position.x, transform.position.y);
                var targetPlace = direction * 12f;
                if (CheckColAtPlace(direction * 12f, pit_layer)
                    || !LineOfSight(myPos, myPos + targetPlace, pit_layer) ||
                    CollisionSelf(pit_layer))
                {
                    direction *= -1f;
                    Speed *= -1.4f;
                }

                // Horizontal Friction
                if (Speed.x != 0) Speed.x = CalcHelper.Approach(Speed.x, 0, friction);

                // Vertical Friction
                if (Speed.y != 0) Speed.y = CalcHelper.Approach(Speed.y, 0, friction);
            }
        }

        #region Dead

        private void Dead_Enter()
        {
            Speed = Vector2.zero;
        }

        private void Dead_Update()
        {
        }

        #endregion

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

        [SuppressMessage("ReSharper", "Unity.InefficientPropertyAccess")]
        private void Behaviour()
        {
            SetUpTarget();

            alarm = 10 + (int) (Random.value * 30);
            if (target != null)
            {
                var myPos = new Vector2(transform.position.x, transform.position.y);
                var targetPos = new Vector2(target.position.x, target.position.y);
                if (LineOfSight(myPos, targetPos, solid_layer) && LineOfSight(myPos, targetPos, pit_layer))
                {
                    //var degrees = Calc.Vector2ToDegree ((targetPos - myPos).normalized);
                    //degrees += Random.Range (-10, 10);
                    //var vector = Calc.DegreeToVector2 (degrees);
                    var vector = (targetPos - myPos).normalized;
                    direction = vector;
                    walk = 40 + (int) (Random.value * 10);
                    alarm = walk;
                }
                else if (Random.value < .6f)
                {
                    var degrees = Random.value * 360f;
                    var vector = CalcHelper.DegreeToVector2(degrees);
                    direction = vector;
                    walk = 10 + (int) (Random.value * 15);
                    alarm = walk + 10 + (int) (Random.value * 30);
                }

                // 改变朝向
                if (targetPos.x < myPos.x)
                    Facing = Facings.Left;
                else if (targetPos.x > myPos.x) Facing = Facings.Right;
            }
            else
            {
                var degrees = Random.value * 360f;
                var vector = CalcHelper.DegreeToVector2(degrees);
                direction = vector;
                walk = 10 + (int) (Random.value * 15);
                alarm = walk + 10 + (int) (Random.value * 30);
            }
        }

        public override void Die()
        {
            fsm.ChangeState(States.Dead, StateTransition.Overwrite);
            if (ScoreManager.Instance != null) ScoreManager.Instance.AddScore(scoreOnDeath);
            ExpManager.Instance.AddExp(2);
            base.Die();
        }

        private void UpdateSprite()
        {
            var targetScale = Facing == Facings.Right ? new Vector3(1f, 1f, 1f) : new Vector3(-1f, 1f, 1f);
            transform.localScale = targetScale;

            if (fsm.State == States.Dead)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Death")) animator.Play("Death");
            }
            else if (fsm.State == States.Attack)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")) animator.Play("Attack");
            }
            else if (fsm.State == States.OnHit)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("OnHit")) animator.Play("OnHit");
            }
            else if (Speed.x == 0 && Speed.y == 0)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") &&
                    !animator.GetCurrentAnimatorStateInfo(0).IsName("Death")) animator.Play("Idle");
            }
            else
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Run")) animator.Play("Run");
            }
        }

        private static bool LineOfSight(Vector2 startPosition, Vector2 targetPosition, LayerMask layer)
        {
            var result = true;

            var hit = Physics2D.Linecast(startPosition, targetPosition, layer);

            if (hit.collider != null) result = false;

            return result;
        }
    }
}