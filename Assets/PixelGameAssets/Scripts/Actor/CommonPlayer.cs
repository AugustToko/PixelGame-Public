using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using PixelGameAssets.MonsterLove.StateMachine;
using PixelGameAssets.Scripts.Camera;
using PixelGameAssets.Scripts.Core;
using PixelGameAssets.Scripts.Core.Audio;
using PixelGameAssets.Scripts.Damage;
using PixelGameAssets.Scripts.EventManager;
using PixelGameAssets.Scripts.InputSystem;
using PixelGameAssets.Scripts.Misc;
using PixelGameAssets.Scripts.SkillSystem;
using PixelGameAssets.Scripts.SkillSystem.Detail;
using PixelGameAssets.Scripts.UI.UICanvas;
using PixelGameAssets.Scripts.WeaponSystem.WeaponInfo;
using PixelGameAssets.Scripts.WeaponSystem.WeaponType;
using QInventory;
using UnityEngine;
using EventType = PixelGameAssets.Scripts.EventManager.EventType;

namespace PixelGameAssets.Scripts.Actor
{
    /// <summary>
    /// 常规 Player 可更换武器
    /// </summary>
    public class CommonPlayer : BasePlayer
    {
        // States for the state machine
        public enum States
        {
            Normal,
            Roll,
            FallInPit,
            Dead
        }

        // State Machine
        private StateMachine<States> Fsm { get; set; }

        private AudioSource _audioSource;

        [SerializeField] private AudioClip _walkClip;

        private new void Awake()
        {
            DefaultSpeed = movingSpeed;
            SetUpType(ActorType.Player);
            Fsm = StateMachine<States>.Initialize(this);
            Health = GetComponent<Health>();
            SkillManager = gameObject.AddComponent<SkillManager>();
            SpriteRenderer = spriteHolder.GetComponent<SpriteRenderer>();

            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.clip = _walkClip;
            _audioSource.volume = 0.1f;

            base.Awake();
        }

        /// <summary>
        /// 技能管理
        /// </summary>
        public SkillManager SkillManager { get; private set; }

        private void Start()
        {
            Input = InputManager.Instance;
            Fsm.ChangeState(States.Normal);
            Health.Actor = this;
            PointerIsNull = pointerSpriteHolder == null;
        }

        public override bool Attack([NotNull] ref Weapon weaponTarget)
        {
            var result = weaponTarget.TryToTriggerWeapon();

            if (weaponTarget.weaponData is GunInfo info)
            {
                EventCenter.Broadcast(EventType.UpdateAmmo, info.clipCapacity, info.remainingBullet);
            }

            return result;
        }

        /// <summary>
        /// 尝试使用技能
        /// </summary>
        /// <param name="skill"></param>
        /// <returns>释放是否成功</returns>
        public override bool TryUseSkill(SkillDetail skill)
        {
            if (skill.skillType == SkillType.None)
            {
                return false;
            }

            Base.Actor actor = this;
            return SkillManager.UseSkill(ref skill, ref actor);
        }

        public override void Update()
        {
            base.Update();
            // 滚轮切枪
            if (Math.Abs(UnityEngine.Input.GetAxis("Mouse ScrollWheel")) > 0.01) NextWeapon();
        }

        private void LateUpdate()
        {
            if (GameManager.IsStop || !CheckCanControl()) return;
            UpdateSprite();
        }

        private void FixedUpdate()
        {
            var deltaPosition = 5f * Time.deltaTime * velocity;
            if (deltaPosition == Vector2.zero)
            {
                IsMoving = false;
                if (_audioSource.isPlaying)
                {
                    _audioSource.Stop();
                }
            }
            else
            {
                IsMoving = true;
                Movement(deltaPosition);
                if (!_audioSource.isPlaying)
                {
                    _audioSource.Play();
                }
            }
        }

        /// <summary>
        /// CallBy: <see cref="IStateMachine" />
        /// </summary>
        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        private void Normal_Update()
        {
            if (!CheckCanControl())
            {
                return;
            }

            // 掉落坑检测
            if (CollisionSelf(PitLayer)) Fsm.ChangeState(States.FallInPit, StateTransition.Overwrite);

            // 滚动检测
            if (CanRoll)
            {
                Fsm.ChangeState(States.Roll, StateTransition.Overwrite);
                return;
            }

            if (!GameManager.Instance.isMobile)
            {
                // ---- for pc ----
                var value = new Vector2(_inputMoveX, _inputMoveY);

                velocity = value * movingSpeed;

                // 瞄准
                if (UnityEngine.Camera.main != null)
                {
                    var mousePos = UnityEngine.Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
                    var position = transform.position;
                    FaceAngle = Mathf.Atan2(mousePos.y - position.y, mousePos.x - position.x) * 180 / Mathf.PI;
                    var vectorMouse = new Vector2(mousePos.x - position.x, mousePos.y - position.y).normalized;
                    if (PixelCameraFollower.Instance != null) PixelCameraFollower.Instance.m_OffsetDir = vectorMouse;
                }

                // ---- for pc ----
            }
            else
            {
                // ---- for mobile ---- 
                velocity = _direction * movingSpeed;

                if (UiManager.Instance.joystickAtk.Direction != Vector2.zero)
                {
                    FaceAngle = Mathf.Atan2(UiManager.Instance.joystickAtk.Direction.y,
                                    UiManager.Instance.joystickAtk.Direction.x) * Mathf.Rad2Deg;
                }

                // ---- for mobile ----
            }

//            if (GameManager.Instance.isMobile)
//            {
//
//                if (UiManager.Instance.joystickAtk.Direction != Vector2.zero)
//                {
//                    FaceAngle = Mathf.Atan2(UiManager.Instance.joystickAtk.Direction.y,
//                                    UiManager.Instance.joystickAtk.Direction.x) * Mathf.Rad2Deg;
//                }
//
//            }
//            else
//            {
//                // ---- for pc ----
//                // 瞄准
//                var mousePos = UnityEngine.Camera.main.ScreenToWorldPoint(Input.mousePosition);
//
//                var position = transform.position;
//
//                FaceAngle = Mathf.Atan2(mousePos.y - position.y, mousePos.x - position.x) * 180 / Mathf.PI;
//                var vectorMouse = new Vector2(mousePos.x - position.x, mousePos.y - position.y).normalized;
//                if (PixelCameraFollower.Instance != null) PixelCameraFollower.Instance.m_OffsetDir = vectorMouse;
//                // ---- for pc ----
//            }

            if (FaceAngle < 0) FaceAngle += 360f;

            if (FaceAngle > 270 || FaceAngle < 90)
                Facing = Facings.Right;
            else
                Facing = Facings.Left;

            CurrentAngle = FaceAngle;

            if (CurrWeapon != null) CurrWeapon.SetRotation(FaceAngle);

            if (!PointerIsNull)
            {
                pointerSpriteHolder.localRotation = Quaternion.Euler(0, 0, FaceAngle + 45);
            }

            bool fire;

            if (GameManager.Instance.isMobile)
            {
                fire = allowAttack && CurrWeapon != null &&
                       (Mathf.Abs(UiManager.Instance.joystickAtk.Direction.x) >= 0.5f ||
                        Mathf.Abs(UiManager.Instance.joystickAtk.Direction.y) >= 0.5f);
            }
            else
            {
                fire = (Input.GetButtonDown(playerNumber, InputAction.Fire) ||
                        Input.GetButton(playerNumber, InputAction.Fire))
                       && CurrWeapon != null && allowAttack && !UiManager.CheckGuiRaycastObjects();
            }

            var w = CurrWeapon;
            if (fire) Attack(ref w);
            else if (CurrWeapon != null)
            {
                CurrWeapon.StopAttack();
            }

            // 监听加速
            if (Input.GetButtonDown(playerNumber, InputAction.SpeedUp) ||
                Input.GetButton(playerNumber, InputAction.SpeedUp))
                movingSpeed = limitSpeed;
            else
                movingSpeed = DefaultSpeed;

            // 闪避技能释放
            if (Input.GetButtonDown(playerNumber, InputAction.DisplacementSkill) ||
                Input.GetButton(playerNumber, InputAction.DisplacementSkill))
                TryUseSkill(GameManager.Instance.PlayerInfo.skillPack.GetDisplacementSkill());

            // 主技能释放
            if (Input.GetButtonDown(playerNumber, InputAction.FirstSkill) ||
                Input.GetButton(playerNumber, InputAction.FirstSkill))
                TryUseSkill(GameManager.Instance.PlayerInfo.skillPack.skillDetails[0]);

            // 主技能释放
            if (Input.GetButtonDown(playerNumber, InputAction.SecondSkill) ||
                Input.GetButton(playerNumber, InputAction.SecondSkill))
                TryUseSkill(GameManager.Instance.PlayerInfo.skillPack.skillDetails[1]);

            // 主技能释放
            if (Input.GetButtonDown(playerNumber, InputAction.ThirdSkill) ||
                Input.GetButton(playerNumber, InputAction.ThirdSkill))
                TryUseSkill(GameManager.Instance.PlayerInfo.skillPack.skillDetails[2]);
        }

        private void UpdateSprite()
        {
            var targetScale = Facing == Facings.Right
                ? new Vector3(1f, 1f, 1f)
                : new Vector3(-1f, 1f, 1f);

            spriteHolder.transform.localScale = targetScale;

            switch (Fsm.State)
            {
                case States.Dead:
                {
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Dead")) animator.Play("Dead");
                    break;
                }
                case States.FallInPit:
                {
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Fall")) animator.Play("Fall");
                    break;
                }
                case States.Roll:
                {
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Roll")) animator.Play("Roll");
                    break;
                }
                default:
                {
                    if (_inputMoveX == 0 && _inputMoveY == 0
                                         && Math.Abs(UiManager.Instance.joystickMove.Horizontal) <= 0
                                         && Math.Abs(UiManager.Instance.joystickMove.Vertical) <= 0
                    )
                    {
                        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") &&
                            !animator.GetCurrentAnimatorStateInfo(0).IsName("TakeHit"))
                            animator.Play("Idle");
                    }
                    else
                    {
                        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Run")
                            && !animator.GetCurrentAnimatorStateInfo(0).IsName("TakeHit"))
                            animator.Play("Run");
                    }

                    break;
                }
            }
        }

        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        private IEnumerator Roll_Enter()
        {
            if (CurrWeapon != null) CurrWeapon.HideWeapon();

            RollCooldownTimer = RollCooldownTime;
            velocity = Vector2.zero;
            RollDir = Vector2.zero;

            Vector2 value;

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

            Fsm.ChangeState(States.Normal, StateTransition.Overwrite);
        }

        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        private void Roll_Exit()
        {
            if (CurrWeapon != null) CurrWeapon.ShowWeapon();

            // Reset Invincibility
            Health.invincible = false;
        }

        /// <summary>
        /// 掉进坑中
        /// </summary>
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
        /// 切换至下个武器
        /// </summary>
        public void NextWeapon()
        {
            if (CurrWeapon == null || WeaponBag.Count < 2) return;

            var index = WeaponBag.IndexOf(CurrWeapon);
            index++;
            if (index > WeaponBag.Count - 1)
            {
                index = 0;
            }

            UnEquipWeapon(false);
            EquipWeapon(WeaponBag[index], false);
            UiManager.Instance.SetWeaponMainIcon(WeaponBag[index].weaponUiIcon);
            UiManager.Instance.SetWeaponSecondIcon(WeaponBag[index + 1 > WeaponBag.Count - 1 ? 0 : index + 1]
                .weaponUiIcon);
        }

        /// <summary>
        /// 取消装备武器, 同时在地上产生同武器的 weaponPickup
        /// <param name="remove">是否删除当前武器</param>
        /// </summary>
        public void UnEquipWeapon(bool remove)
        {
            if (CurrWeapon == null) return;

            if (remove)
            {
                var position = transform.position;
                var parent = GameObject.Find("LevelGrid").transform;
                var pickUp = Instantiate(CurrWeapon.weaponPickup, new Vector2(position.x, position.y)
                                                                  + Vector2.zero, Quaternion.Euler(Vector3.zero),
                    parent);
                pickUp.wepRes.weaponData = CurrWeapon.weaponData;

                WeaponBag.Remove(CurrWeapon);
                Destroy(CurrWeapon.gameObject);
            }
            else
            {
                CurrWeapon.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 装备武器
        /// 新武器将加入背包
        /// </summary>
        /// <param name="wepRes">武器</param>
        /// <param name="newWeapon">是否为新武器</param>
        public void EquipWeapon(Weapon wepRes, bool newWeapon = true)
        {
            if (wepRes == null) return;

            if (newWeapon)
            {
                if (CurrWeapon != null) UnEquipWeapon(WeaponBag.Count >= MaxWeapons);

                var position = weaponHolder.position;

                CurrWeapon = Instantiate(wepRes, new Vector2(position.x, position.y)
                                                 + wepRes.offsetNormal, Quaternion.identity, weaponHolder);

                CurrWeapon.weaponData = wepRes.weaponData ?? CurrWeapon.GetDefaultInfo();

                // 加入背包
                WeaponBag.Add(CurrWeapon);

                SetupWeaponIco();
            }
            else
            {
                CurrWeapon = wepRes;
                wepRes.gameObject.SetActive(true);
            }

            CurrWeapon.owner = Health;
            CurrWeapon.zOrderComponent.targetSpriteRenderer = SpriteRenderer;

            if (CurrWeapon.weaponData is GunInfo info)
            {
                EventCenter.Broadcast(EventType.UpdateAmmo, info.clipCapacity,
                    info.remainingBullet);
            }
            else
            {
                // TODO: 近身武器
            }
        }

        /// <summary>
        /// 设置收到打击动画
        /// </summary>
        public override void SetTakeHit(int amount)
        {
            if (Fsm.State != States.Normal) return;
            base.SetTakeHit(amount);

            //TODO: 被打动画
//            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("TakeHit")) animator.Play("TakeHit");

            QMhelper.SetCurrentHp(Health.health);
            if (CameraShaker.Instance != null) CameraShaker.InitShake(0.075f, 2f);
        }

        /// <summary>
        /// 加血
        /// </summary>
        public override void SetTakeHealth()
        {
            QMhelper.SetCurrentHp(Health.health);
        }

        /// <summary>
        /// 显示玩家
        /// </summary>
        /// <param name="pos">显示的位置</param>
        public override void ShowPlayer(Vector3 pos)
        {
            Fsm.ChangeState(States.Normal, StateTransition.Overwrite);
            SkillManager.ResetSkill();
            base.ShowPlayer(pos);
        }

        /// <summary>
        /// 死亡
        /// </summary>
        public override void Die()
        {
            allowMove = false;
            Health.dead = true;
            allowAttack = false;
            if (CameraShaker.Instance != null) CameraShaker.InitShake(0.25f, 1f);

            Fsm.ChangeState(States.Dead, StateTransition.Overwrite);
            velocity = Vector2.zero;

            Instantiate(ResourceLoader.DeadUi);

            Fsm.ChangeState(States.Normal, StateTransition.Overwrite);

            HidePlayer();
        }
    }
}