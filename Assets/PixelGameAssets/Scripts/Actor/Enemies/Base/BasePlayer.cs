using System;
using System.Collections.Generic;
using PixelGameAssets.Scripts.Actor.ControllerSys;
using PixelGameAssets.Scripts.Core;
using PixelGameAssets.Scripts.Damage;
using PixelGameAssets.Scripts.EventManager;
using PixelGameAssets.Scripts.InputSystem;
using PixelGameAssets.Scripts.Misc;
using PixelGameAssets.Scripts.SkillSystem.Obj;
using PixelGameAssets.Scripts.UI.UICanvas;
using PixelGameAssets.Scripts.WeaponSystem.WeaponInfo;
using PixelGameAssets.Scripts.WeaponSystem.WeaponType;
using UnityEngine;

namespace PixelGameAssets.Scripts.Actor
{
    /// <summary>
    /// Player 基类
    /// </summary>
    public abstract class BasePlayer : PhysicalObject2DController
    {
        [Header("Animation or FX")] public Animator animator;

        /// <summary>
        /// 极限速度, 即加速过后的速度
        /// </summary>
        public float limitSpeed = 40f;

        /// <summary>
        /// 目前移动速度
        /// </summary>
        public float movingSpeed = 20f;

        [Header("Some Sprite Holders")] public Transform spriteHolder;

        public Transform pointerSpriteHolder;

        public Transform weaponHolder;

        /// <summary>
        /// TODO: remove
        /// </summary>
        public TrailRenderer trailRenderer;

        public IInputManager Input { get; protected set; }

        private bool _canRoll = false;

        public bool CanRoll => Input.GetButtonDown(playerNumber, InputAction.Roll) && RollCooldownTimer <= 0f &&
                               !UiManager.CheckGuiRaycastObjects();

        // 滚动冷却时间
        [Header("Roll")] public float RollCooldownTime;

        // 滚动速度
        public int RollSpeed = 100;

        // 滚动时间
        public float RollTime = 0.3f;

        /// <summary>
        /// TODO: 多玩家支持
        /// </summary>
        [Header("CommonPlayer Settings")] public int playerNumber = 0;

        // “坑” 层
        [Header("Pit")] public LayerMask PitLayer;

        /// <summary>
        /// 掉落进坑的时间
        /// </summary>
        public float FallingTime;

        /// <summary>
        /// 默认移动速度
        /// </summary>
        public float DefaultSpeed { get; set; }

        /// <summary>
        /// 健康
        /// </summary>
        public Health Health { get; protected set; }

        // 掉落进坑的计时器
        protected float FallingTimer;

        // 滚动冷却计时器
        protected float RollCooldownTimer;

        // 滚动方向
        protected Vector2 RollDir;

        // TODO: for pc
        // Helper private Variables
        protected int _inputMoveX; // 存储每帧的水平输入
        protected int _inputMoveY; // 存储每帧的垂直输入
        protected Vector3 _direction;

        /// <summary>
        /// 当前武器
        /// </summary>
        public Weapon CurrWeapon { get; set; }

        /// <summary>
        /// 武器背包
        /// </summary>
        public List<Weapon> WeaponBag { get; private set; } = new List<Weapon>();

        /// <summary>
        /// 最大武器持有数
        /// </summary>
        public int MaxWeapons { get; private set; } = 2;

        /// <summary>
        /// 当前的方向角度
        /// </summary>
        public float CurrentAngle { get; set; }

        // 角色朝向的角度
        public float FaceAngle { get; set; }

        protected bool IsMoving;

        protected bool PointerIsNull;

        protected SpriteRenderer SpriteRenderer;

        public void ResumeControl()
        {
            allowAttack = true;
            allowMove = true;
            velocity = Vector2.zero;
            _canControl = true;

            if (animator.isActiveAndEnabled && !animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") &&
                !animator.GetCurrentAnimatorStateInfo(0).IsName("TakeHit"))
                animator.Play("Idle");
        }

        /// <summary>
        /// 检测玩家是否可控
        /// </summary>
        /// <returns>是否可控</returns>
        public bool CheckCanControl()
        {
            var can = !GameManager.IsStop && _canControl && allowMove;
            return can;
        }

        public override void SetTakeHit(int amount)
        {
            
        }

        /// <summary>
        /// 玩家是否可控
        /// </summary>
        private bool _canControl = true;

        /// <summary>
        /// 停止玩家控制
        /// </summary>
        public void StopControl()
        {
            allowAttack = false;
            allowMove = false;
            velocity = Vector2.zero;
            _canControl = false;

            if (animator.isActiveAndEnabled && !animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") &&
                !animator.GetCurrentAnimatorStateInfo(0).IsName("TakeHit"))
                animator.Play("Idle");
        }

        public virtual void ShowPlayer(Vector3 pos)
        {
            ResumeControl();
            gameObject.SetActive(true);
            transform.position = pos;

            Health.invincible = false;
            Health.dead = false;
            Health.health = (int) QMhelper.GetMaxHpVal();
            QMhelper.SetCurrentHp(Health.health);

            if (CurrWeapon == null) return;

            if (CurrWeapon.weaponData is GunInfo info)
            {
                // TODO: 重生子弹
                info.clipCapacity = 100;
                info.remainingBullet = 500;

                EventCenter.Broadcast(EventManager.EventType.UpdateAmmo, info.clipCapacity,
                    info.remainingBullet);
            }

            SetupWeaponIco();

            // 更新 Exp Ui
            ExpManager.Instance.AddExp(0);
            GameManager.Instance.PlayerInfo.OperateCoin(0);

            GetComponent<BoxCollider2D>().enabled = true;
        }

        public void SetupWeaponIco()
        {
            UiManager.Instance.SetWeaponMainIcon(CurrWeapon.weaponUiIcon);

            if (WeaponBag.Count > 1)
            {
                UiManager.Instance.SetWeaponSecondIcon(WeaponBag.IndexOf(CurrWeapon) == 0
                    ? WeaponBag[1].weaponUiIcon
                    : WeaponBag[0].weaponUiIcon);
            }
        }

        public virtual void Update()
        {
            if (!CheckCanControl()) return;

            // 移动
            if (!GameManager.Instance.isMobile)
            {
                _inputMoveX = (int) Input.GetAxis(playerNumber, InputAction.MoveX);

                _inputMoveY = (int) Input.GetAxis(playerNumber, InputAction.MoveY);
            }
            else
            {
                _direction = UiManager.Instance.joystickMove.Direction;
            }

            if (RollCooldownTimer > 0f) RollCooldownTimer -= Time.deltaTime;
        }

        public void HidePlayer()
        {
            StopControl();
            gameObject.SetActive(false);
        }
    }
}