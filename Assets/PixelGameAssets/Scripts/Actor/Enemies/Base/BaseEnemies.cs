using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using PixelGameAssets.Scripts.Actor.ControllerSys;
using PixelGameAssets.Scripts.Core;
using PixelGameAssets.Scripts.Damage;
using PixelGameAssets.Scripts.UI.Tag;
using PixelGameAssets.Scripts.UI.Tag.Model;
using PixelGameAssets.Scripts.WeaponSystem.WeaponType;
using UnityEngine;
using Random = System.Random;

namespace PixelGameAssets.Scripts.Actor.Enemies.Base
{
    /// <summary>
    ///     敌人抽象类
    /// </summary>
    public abstract class Enemies : EnemiesController
    {
        /// <summary>
        /// 怪物等级
        /// TODO: 怪物等级设定
        /// </summary>
        public enum Level
        {
            L1,
            L2,
            L3,
            L4,
            L5,
        }

        [Header("Probability Settings")] public Level eLevel = Level.L1;

        public GameObject[] fallingObjects;

        /// <summary>
        /// 掉落概率 (1/x)
        /// </summary>
        public int probability = 5;

        /// <summary>
        /// 是否中立, 中立即以场景内随机选择一个
        /// </summary>
        [Space(10)] [Header("Nature")] public bool isNeutral;

        // 目标, 影响移动, 攻击
        [Header("Attack")] public Transform target;

        public int alarm;

        [Header("Death")] public int scoreOnDeath = 3;

        public Health health;

        [Header("Other")] public Weapon currWeapon;

        /// <summary>
        /// 是否死亡
        /// </summary>
        private bool _isDead;

        /// <summary>
        /// 实体是否被销毁
        /// </summary>
        private bool _isDestoryed = false;

        #region UI_TAG

        private GameObject _infoBar;
        
        private RectTransform _infoBarRect;
        
        private HpBar _hpBar;

        private RectTransform _hpBarRect;

        #endregion

        [SerializeField] protected int defaultHealth = 10;

        protected override void Awake()
        {
            base.Awake();
            health.health = defaultHealth;
            SetupTag();
        }

        /// <summary>
        /// 设置头部 UI 信息
        /// </summary>
        private void SetupTag()
        {
            var ib = GetComponentInChildren<InfoBar>();
            _infoBar = ib.gameObject;
            _infoBarRect = _infoBar.GetComponent<RectTransform>();
            ib.SetupInfo(new InfoData(Level.L1, actorName));

            _hpBar = GetComponentInChildren<HpBar>();
            _hpBarRect = _hpBar.GetComponent<RectTransform>();
            _hpBar.SetMaxHp(health.health);
            UpdateHpBar(health.health);
        }

        private void UpdateHpBar(int val)
        {
            _hpBar.SetHp(val);
        }

        protected void LateUpdate()
        {
            FixDir();
        }

        private void FixDir()
        {
            if (_infoBarRect == null || _hpBarRect == null) return;

            var localScale = _infoBarRect.localScale;
            
            switch (Facing)
            {
                case Facings.Left:
                    localScale = new Vector3(-Mathf.Abs(_infoBarRect.localScale.x), localScale.y,
                        localScale.z);
                    break;
                case Facings.Right:
                    localScale = new Vector3(Mathf.Abs(_infoBarRect.localScale.x), localScale.y,
                        localScale.z);
                    break;
            }
            
            _infoBarRect.localScale = localScale;
            _hpBarRect.localScale = localScale;
        }

        /// <summary>
        /// 获取目标位置 <see cref="Transform" />
        /// </summary>
        protected void SetUpTarget()
        {
            // 中立判断 (全场随机找 Actor)
            if (isNeutral)
            {
                var actors = FindObjectsOfType<Actor.Base.Actor>();
                var random = new Random();
                var act = actors[random.Next(0, actors.Length - 1)];
                target = act.transform;
            }

            if (target == null)
            {
                var player = FindObjectOfType<BasePlayer>();

                if (player != null) target = player.transform;
            }
        }

        public override void SetTakeHit(int amount)
        {
            UpdateHpBar(health.health);
        }

        public override void Die()
        {
            myCollider.enabled = false;
            allowAttack = false;
            allowMove = false;
            _isDead = true;

            if (fallingObjects.Length > 0)
            {
                // TODO: 掉落物品几率设定
                switch (eLevel)
                {
                    case Level.L1:
                    {
                        if (UnityEngine.Random.Range(0, probability) == 0)
                        {
                            Instantiate(fallingObjects[UnityEngine.Random.Range(0, fallingObjects.Length - 1)],
                                transform.position, Quaternion.Euler(Vector3.zero));
                        }
                    }
                        break;
                }
            }

            StartCoroutine(FixDie());
        }

        private void OnDestroy()
        {
            _isDestoryed = true;
        }

        /// <summary>
        /// 修复当死亡动画结束后后怪物不消失 (死亡动画不得超过 5 秒)
        /// </summary>
        /// <returns>IEnumerator</returns>
        private IEnumerator FixDie()
        {
            yield return new WaitForSeconds(5f);
            if (_isDestoryed) yield return null;
            Destroy(gameObject);
        }
    }
}