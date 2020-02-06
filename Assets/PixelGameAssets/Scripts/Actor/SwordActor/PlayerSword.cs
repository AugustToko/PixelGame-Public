using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using PixelGameAssets.MonsterLove.StateMachine;
using PixelGameAssets.Scripts.Actor.SwordActor;
using PixelGameAssets.Scripts.Camera;
using PixelGameAssets.Scripts.Core;
using PixelGameAssets.Scripts.Damage;
using PixelGameAssets.Scripts.EventManager;
using PixelGameAssets.Scripts.InputSystem;
using PixelGameAssets.Scripts.Misc;
using PixelGameAssets.Scripts.SkillSystem;
using PixelGameAssets.Scripts.SkillSystem.Detail;
using PixelGameAssets.Scripts.UI.UICanvas;
using PixelGameAssets.Scripts.WeaponSystem.WeaponInfo;
using PixelGameAssets.Scripts.WeaponSystem.WeaponType;
using UnityEngine;
using EventType = PixelGameAssets.Scripts.EventManager.EventType;

namespace PixelGameAssets.Scripts.Actor
{
    public class PlayerSword : BasePlayer
    {
        // States for the state machine
        public enum States
        {
            Normal,
            Roll,
            FallInPit,
            Dead
        }

        public Weapon sword;

        // State Machine
        public StateMachine<States> fsm { get; set; }

        private new void Awake()
        {
            var position = weaponHolder.position;

            CurrWeapon = Instantiate(sword, new Vector2(position.x, position.y)
                                            + sword.offsetNormal, Quaternion.identity, weaponHolder);

            DefaultSpeed = movingSpeed;

            SetUpType(ActorType.Player);
            fsm = StateMachine<States>.Initialize(this);
            Health = GetComponent<Health>();
            base.Awake();

            SpriteRenderer = spriteHolder.GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            Input = InputManager.Instance;
            fsm.ChangeState(States.Normal);
            Health.Actor = this;
            _skillManager = gameObject.AddComponent<SkillManager>();
            InitSkillDetail();

            // TODO: 生命值设置
            Health.health = (int) 100f;
            GameManager.Instance.Player = this;

            DontDestroyOnLoad(this);

            PointerIsNull = pointerSpriteHolder == null;
        }

        // 技能管理
        private SkillManager _skillManager;

        // 持有的技能
        [HideInInspector] public SkillPack skillPack;

        /// <summary>
        /// 加载技能
        /// TODO: 把 <see cref="SkillType.Displacement"/> 设置为固定技能
        /// </summary>
        private void InitSkillDetail()
        {
            var skillDodge = new CommonSkillDetail.Builder(SkillType.DodgeNone, 0, "DodgeNone").SetColdDownTime(0)
                .Build();
            var skillWpNone = new CommonSkillDetail.Builder(SkillType.WpNone, 0, "WpNone").SetColdDownTime(0).Build();
            var skillDis = new CommonSkillDetail.Builder(SkillType.Displacement, 0, "Displacement")
                .SetColdDownTime(0.5f).Build();

            var skillList = new List<SkillDetail>
            {
                new CommonSkillDetail.Builder(SkillType.FireBall, 0, "FireBall").SetColdDownTime(0.5f).Build(),
                new CommonSkillDetail.Builder(SkillType.LightningBolt, 0, "LightningBolt").SetColdDownTime(2f).Build(),
                skillDodge,
                skillWpNone,
                skillDis
            };

            // Skill-pack (All skills)
            var skillPack = new SkillPack(
                skillDodge, skillWpNone, skillDis, skillList
            );

            BasePlayer player = this;
            _skillManager.Init(ref player, ref skillPack);
        }

        public override bool Attack([NotNull] ref Weapon weaponTarget)
        {
            var result = weaponTarget.TryToTriggerWeapon();

            if (weaponTarget.weaponData is GunInfo info)
            {
                EventCenter.Broadcast<int, int>(EventType.UpdateAmmo, info.clipCapacity, info.remainingBullet);
            }

            return result;
        }

        /// <summary>
        /// 尝试使用技能
        /// </summary>
        /// <param name="skill"></param>
        /// <returns></returns>
        public override bool TryUseSkill(SkillDetail skill)
        {
            if (skill.skillType == SkillType.None)
            {
                return false;
            }

            Base.Actor actor = this;
            return _skillManager.UseSkill(ref skill, ref actor);
        }

        public PlayerSwordHelper playerSwordHelper;

        public override void Update()
        {
            base.Update();

            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                playerSwordHelper.DoAnimation();
                velocity = Vector2.zero;
                _inputMoveX = 0;
                _inputMoveY = 0;
                _direction = Vector3.zero;
            }
        }

        private void LateUpdate()
        {
            if (GameManager.IsStop || !CheckCanControl() || playerSwordHelper.isAttack) return;
            UpdateSprite();
        }

        private void FixedUpdate()
        {
            if (GameManager.IsStop || !CheckCanControl() || !allowMove || playerSwordHelper.isAttack) return;
            var deltaPosition = 5f * Time.deltaTime * velocity;
            if (deltaPosition == Vector2.zero) return;
            Movement(deltaPosition);
        }

        /// <summary>
        /// CallBy: <see cref="IStateMachine" />
        /// </summary>
        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        private void Normal_Update()
        {
            if (!CheckCanControl() || playerSwordHelper.isAttack)
            {
                return;
            }

            // 掉落坑检测
            if (CollisionSelf(PitLayer)) fsm.ChangeState(States.FallInPit, StateTransition.Overwrite);

            // 滚动检测
            if (CanRoll)
            {
                fsm.ChangeState(States.Roll, StateTransition.Overwrite);
                return;
            }

            if (!Application.isMobilePlatform)
            {
                var value = new Vector2(_inputMoveX, _inputMoveY);

                velocity = value * movingSpeed;
            }
            else
            {
                velocity = _direction * movingSpeed;
            }

            if (Application.isMobilePlatform)
            {
                // ---- for mobile ----

                if (UiManager.Instance.joystickAtk.Direction != Vector2.zero)
                {
                    FaceAngle = Mathf.Atan2(UiManager.Instance.joystickAtk.Direction.y,
                                    UiManager.Instance.joystickAtk.Direction.x) * Mathf.Rad2Deg;
                }

                // ---- for mobile ----
            }
            else
            {
                // ---- for pc ----
                // 瞄准
                var mousePos = UnityEngine.Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);

                var position = transform.position;

                FaceAngle = Mathf.Atan2(mousePos.y - position.y, mousePos.x - position.x) * 180 / Mathf.PI;
                var vectorMouse = new Vector2(mousePos.x - position.x, mousePos.y - position.y).normalized;
                if (PixelCameraFollower.Instance != null) PixelCameraFollower.Instance.m_OffsetDir = vectorMouse;
                // ---- for pc ----
            }

            if (FaceAngle < 0) FaceAngle += 360f;

            if (FaceAngle > 270 || FaceAngle < 90)
                Facing = Facings.Right;
            else
                Facing = Facings.Left;

            CurrentAngle = FaceAngle;

            if (!PointerIsNull)
            {
                pointerSpriteHolder.localRotation = Quaternion.Euler(0, 0, FaceAngle + 45);
            }

//            var fire = (input.GetButtonDown(playerNumber, InputAction.Fire) ||
//                        input.GetButton(playerNumber, InputAction.Fire) ||
//                        Mathf.Abs(UiManager.Instance.joystickAtk.Direction.x) >= 0.5f ||
//                        Mathf.Abs(UiManager.Instance.joystickAtk.Direction.y) >= 0.5f) && currWeapon != null &&
//                       allowAttack && !UiManager.CheckGuiRaycastObjects();

//            var w = currWeapon;
//            if (fire) Attack(ref w);
//            else if (currWeapon != null)
//            {
//                currWeapon.StopAttack();
//            }

            // 监听加速
            if (Input.GetButtonDown(playerNumber, InputAction.SpeedUp) ||
                Input.GetButton(playerNumber, InputAction.SpeedUp))
                movingSpeed = limitSpeed;
            else
                movingSpeed = DefaultSpeed;

            // 闪避技能释放
            if (Input.GetButtonDown(playerNumber, InputAction.DisplacementSkill) ||
                Input.GetButton(playerNumber, InputAction.DisplacementSkill))
                TryUseSkill(skillPack.GetDisplacementSkill());

            // 主技能释放
            if (Input.GetButtonDown(playerNumber, InputAction.FirstSkill) ||
                Input.GetButton(playerNumber, InputAction.FirstSkill))
                TryUseSkill(skillPack.skillDetails[0]);

            // 主技能释放
            if (Input.GetButtonDown(playerNumber, InputAction.SecondSkill) ||
                Input.GetButton(playerNumber, InputAction.SecondSkill))
                TryUseSkill(skillPack.skillDetails[1]);

            // 主技能释放
            if (Input.GetButtonDown(playerNumber, InputAction.ThirdSkill) ||
                Input.GetButton(playerNumber, InputAction.ThirdSkill))
                TryUseSkill(skillPack.skillDetails[2]);
        }

        private void UpdateSprite()
        {
            var targetScale = Facing == Facings.Right
                ? new Vector3(1f, 1f, 1f)
                : new Vector3(-1f, 1f, 1f);
            spriteHolder.transform.localScale = targetScale;

            if (fsm.State == States.Dead)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Dead")) animator.Play("Dead");
            }
            else if (fsm.State == States.FallInPit)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Fall")) animator.Play("Fall");
            }
            else if (fsm.State == States.Roll)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Roll")) animator.Play("Roll");
            }
            else if ((_inputMoveX == 0
                      && _inputMoveY == 0)
//                     || (Math.Abs(UiManager.Instance.joystickMove.Horizontal) <= 0 
//                         && Math.Abs(UiManager.Instance.joystickMove.Vertical) <= 0)
            )
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") &&
                    !animator.GetCurrentAnimatorStateInfo(0).IsName("TakeHit") &&
                    !animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1") &&
                    !animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2")
                )
                    animator.Play("Idle");
            }
            else
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Run")
                    && !animator.GetCurrentAnimatorStateInfo(0).IsName("TakeHit"))
                    animator.Play("Run");
            }
        }

        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        private IEnumerator Roll_Enter()
        {
            if (CurrWeapon != null) CurrWeapon.HideWeapon();

            RollCooldownTimer = RollCooldownTime;
            velocity = Vector2.zero;
            RollDir = Vector2.zero;

            Vector2 value = Vector2.zero;

            if (Application.isMobilePlatform)
            {
                value = new Vector2(UiManager.Instance.joystickMove.Direction.x,
                    UiManager.Instance.joystickMove.Direction.y);
            }
            else
            {
                value = new Vector2(_inputMoveX, _inputMoveY);
            }

            if (value == Vector2.zero) value = new Vector2((int) Facing, 0f);

            // TODO: for pc

            value.Normalize();
            velocity = value * RollSpeed;

            RollDir = value;

            if (RollDir.x != 0f) Facing = (Facings) Mathf.Sign(RollDir.x);

            // ScreenSake
            if (CameraShaker.Instance != null) CameraShaker.InitShake(0.125f, 1f);

            // Invincibility
            Health.invincible = true;

            yield return new WaitForSeconds(RollTime);

            // Wait one extra frame
            yield return null;

            fsm.ChangeState(States.Normal, StateTransition.Overwrite);
        }

        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        private void Roll_Exit()
        {
            if (CurrWeapon != null) CurrWeapon.ShowWeapon();

            // Reset Invincibility
            Health.invincible = false;
        }

        // 掉进坑中
        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        private void FallInPit_Enter()
        {
            // Set the falling timer
            FallingTimer = FallingTime;

            // Disable collider and reset speed
            myCollider.enabled = false;
            velocity = Vector2.zero;

            //TODO: for pc
            _inputMoveX = 0;
            _inputMoveY = 0;

            if (CurrWeapon != null) CurrWeapon.HideWeapon();
        }

        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        private void FallInPit_Update()
        {
            FallingTimer -= Time.deltaTime;

            // Die if the time has passed
            if (FallingTimer <= 0) Die();
        }

        /// <summary>
        /// 设置收到打击动画
        /// </summary>
        public override void SetTakeHit(int amount)
        {
            if (fsm.State != States.Normal) return;
            base.SetTakeHit(amount);

            //TODO: 被打动画
//            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("TakeHit")) animator.Play("TakeHit");

//            HealthBar.SetHearts(Health.health);
            QMhelper.SetCurrentHp(Health.health);

            if (CameraShaker.Instance != null) CameraShaker.InitShake(0.075f, 2f);
        }

        // 加血
        public override void SetTakeHealth()
        {
//            HealthBar.SetHearts(Health.health);
            QMhelper.SetCurrentHp(Health.health);
        }

        public override void Die()
        {
            playerSwordHelper.isAttack = false;
            allowMove = false;
            Health.dead = true;
            allowAttack = false;
            if (CameraShaker.Instance != null) CameraShaker.InitShake(0.25f, 1f);

            fsm.ChangeState(States.Dead, StateTransition.Overwrite);
            velocity = Vector2.zero;

            Instantiate(ResourceLoader.DeadUi);

            fsm.ChangeState(States.Normal, StateTransition.Overwrite);

            HidePlayer();
        }
    }
}