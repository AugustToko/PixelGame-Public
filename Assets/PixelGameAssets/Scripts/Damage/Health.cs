using System;
using System.Collections;
using PixelGameAssets.Scripts.Actor;
using PixelGameAssets.Scripts.Camera;
using PixelGameAssets.Scripts.Misc;
using PixelGameAssets.Scripts.UI;
using UnityEngine;

namespace PixelGameAssets.Scripts.Damage
{
    /// <summary>
    /// 物体生命值实现
    /// 管理: 伤害, 加血, 死亡事件, 无敌时间
    /// </summary>
    public class Health : MonoBehaviour
    {
        /// <summary>
        /// 存储 Actor 引用
        /// </summary>
        public Actor.Base.Actor Actor { get; set; }
        
        // 是否在击中后有无敌时间
        public bool becomeInvincibleOnHit;

        [Header("IsDeathOrNot")] public bool dead;

        /// <summary>
        /// 在x时间后执行死亡事件 def: 1f
        /// </summary>
        [Header("Perform Dead Events after x time")]
        public float dieEventsAfterTime = 1f;

        // 当前血量
        [Header("Current Health")] public int health;

        // 无敌
        [Header("Invincible")] public bool invincible;

        // 被击中后的无敌时间 def: 0.5f
        public float invincibleTimeOnHit = 0.5f;

        // 无敌时间
        private float _invincibleTimer;

        /// <summary>
        /// 游戏开始时的无敌时间判定
        /// </summary>
        private void Update()
        {
            if (_invincibleTimer > 0f)
            {
                _invincibleTimer -= Time.deltaTime;

                if (_invincibleTimer <= 0f && invincible) invincible = false;
            }
        }

        /// <summary>
        ///     进行伤害
        /// </summary>
        /// <param name="amount">伤害值</param>
        /// <returns>是否被伤害了</returns>
        public bool TakeDamage(int amount)
        {
            if (dead || invincible) return false;

            health = GetDamagedHp(amount);
            
            MakeDamageNumber(amount);

            Actor.SetTakeHit(amount);

            if (health <= 0)
            {
                dead = true;

                if (Actor is BasePlayer)
                {
                    CameraShaker.InitShake(0.2f, 1f);
                }

                // 静止 Actor 死亡前移动 (死亡延迟 0.5 秒)
                //TODO: 如果不延迟死亡 Actor 无法及时销毁
                Actor.allowAttack = false;
                Actor.allowMove = false;
                StartCoroutine(DeathEventsRoutine(0.1f));
                
            }
            else if (becomeInvincibleOnHit)
            {
                invincible = true;
                _invincibleTimer = invincibleTimeOnHit;
            }

            return true;
        }

        /// <summary>
        /// 获取伤害后的 HP
        /// </summary>
        /// <param name="amount">HP</param>
        /// <returns>New HP</returns>
        private int GetDamagedHp(int amount)
        {
            return health = Mathf.Max(0, health - amount);
        }

        /// <summary>
        /// 生成伤害数字
        /// </summary>
        /// <param name="val">val</param>
        private void MakeDamageNumber(int val)
        {
            var number = Instantiate(ResourceLoader.DamageRes, Actor.transform.position,
                Quaternion.Euler(Vector3.zero));
            number.GetComponent<DamageNumber>().SetUpDamage(val);
        }

        /// <summary>
        /// 加血
        /// </summary>
        /// <param name="amount">加血值</param>
        /// <returns></returns>
        public bool TakeHeal(int amount)
        {
            if (dead || Math.Abs(health - QMhelper.GetMaxHpVal()) < 0.1f) return false;

            health = (int) Mathf.Min(QMhelper.GetMaxHpVal(), health + amount);

            Actor.SetTakeHealth();

            return true;
        }

        private IEnumerator DeathEventsRoutine(float time)
        {
            yield return new WaitForSeconds(time);
            Actor.Die();
        }
    }
}