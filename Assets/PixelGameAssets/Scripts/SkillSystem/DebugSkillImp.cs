using System.Collections;
using PixelGameAssets.MonsterLove.StateMachine;
using PixelGameAssets.Scripts.Actor;
using PixelGameAssets.Scripts.Core;
using PixelGameAssets.Scripts.Core.Audio;
using PixelGameAssets.Scripts.GKUtils;
using PixelGameAssets.Scripts.InputSystem;
using PixelGameAssets.Scripts.Misc;
using PixelGameAssets.Scripts.SkillSystem.Detail;
using PixelGameAssets.Scripts.SkillSystem.Obj;
using PixelGameAssets.Scripts.UI.UICanvas;
using UnityEngine;

namespace PixelGameAssets.Scripts.SkillSystem
{
    /// <summary>
    /// 对于 <see cref="ISkill" /> 的一种实现规范 (DEBUG 版本)
    /// </summary>
    public class DebugSkillImp : MonoBehaviour, ISkill
    {
//        public const string Tag = "DebugSkillImp";

        private BasePlayer _commonPlayer;

        private float _timeSlow;

        private SkillManager _skillManager;

        // State Machine
        private StateMachine<SkillType> _fsm;

        private StateMachine<SkillType> _fsmWp;

        // 被动技能状态机
        private StateMachine<SkillType> _fsmDodge;

        private void Awake()
        {
            _fsm = StateMachine<SkillType>.Initialize(this);
            _fsmWp = StateMachine<SkillType>.Initialize(this);
            _fsmDodge = StateMachine<SkillType>.Initialize(this);
            _timeSlow = 0;
            Time.timeScale = 1;
        }

        public void Init(BasePlayer commonPlayer)
        {
            _commonPlayer = commonPlayer;
        }

        private void Start()
        {
            ResetSkill();
        }

        public void ResetSkill()
        {
            _fsm.ChangeState(SkillType.Normal, StateTransition.Overwrite);
            _fsmWp.ChangeState(SkillType.Normal, StateTransition.Overwrite);
            _fsmDodge.ChangeState(SkillType.Normal, StateTransition.Overwrite);
        }

        /// <summary>
        ///     调用状态机以使用技能
        /// </summary>
        /// <param name="skill">技能细节</param>
        public void UseSkill(ref SkillDetail skill)
        {
            _fsm.ChangeState(skill.skillType, StateTransition.Overwrite);
        }

        /// <summary>
        /// 调用状态机以使用 "闪避" 技能
        /// 只允许内部调用, 属于被动技能
        /// </summary>
        /// <param name="skill">技能细节</param>
        private void UseDodge(ref SkillDetail skill)
        {
            _fsmDodge.ChangeState(skill.skillType, StateTransition.Overwrite);
        }

        private void MakeTimeSlow(float duration, float speed)
        {
            _timeSlow = duration;
            Time.timeScale = speed;
        }

        private void Update()
        {
            if (_timeSlow > 0f)
            {
                _timeSlow -= Time.deltaTime;

                if (_timeSlow <= 0f) Time.timeScale = 1;
            }
        }

        #region None

        private void None_Enter()
        {
            // none
            GkLog.Debug(tag, "Call None Skill!!!");
        }

        private void None_Exit()
        {
            _fsm.ChangeState(SkillType.Normal, StateTransition.Overwrite);
        }

        #endregion None

        private void Normal_Update()
        {
        }

        #region Displacement

        /// <summary>
        /// 闪避技能的具体实现
        /// </summary>
        /// <returns></returns>
        private IEnumerator Displacement_Enter()
        {
            _commonPlayer.trailRenderer.enabled = true;

            _commonPlayer.velocity = Vector2.zero;

            Vector2 value;

            if (Application.isMobilePlatform)
            {
                value = new Vector2(UiManager.Instance.joystickMove.Direction.x,
                    UiManager.Instance.joystickMove.Direction.y);
            }
            else
            {
                // TODO: for pc
                value = new Vector2(
                    (int) _commonPlayer.Input.GetAxis(0, InputAction.MoveX),
                    (int) _commonPlayer.Input.GetAxis(0, InputAction.MoveY)
                );
            }

            if (value == Vector2.zero) value = new Vector2((int) _commonPlayer.Facing, 0f);

            value.Normalize();

            _commonPlayer.velocity = value * 300f;

            AudioManager.Instance.AddToRandomFxSource("transfer");

            _commonPlayer.Health.invincible = true;

            yield return new WaitForSeconds(0.1f);
            _commonPlayer.trailRenderer.enabled = false;

            // Wait one extra frame
            yield return null;

            _fsm.ChangeState(SkillType.Normal, StateTransition.Overwrite);
        }

        private void Displacement_Exit()
        {
            _commonPlayer.Health.invincible = false;
        }

        #endregion

        #region Flash

        // TODO: 闪现的实现方法
        private IEnumerator Flash_Enter()
        {
//            const float distance = 60f;
//
//            var playerPosition = _commonPlayer.transform.position;
//
//            var mouseCoords = UnityEngine.Camera.main.ScreenToWorldPoint(Input.mousePosition);
//
//            var targetPos = VectorUtils.GetBetweenPoint(
//                playerPosition,
//                new Vector3(mouseCoords.x, mouseCoords.y, 0f),
//                distance
//            );
//
//            var hit2D = Physics2D.RaycastAll(playerPosition, targetPos, GameManager.Instance.solidLayer);
//
//            if (hit2D.Length == 0)
//            {
//                var angle = Mathf.Round(_commonPlayer.faceAngle);
//
//                _commonPlayer.transform.position = targetPos;
//
//                // ScreenSake
//                if (CameraShaker.Instance != null) CameraShaker.InitShake(0.125f, 1f);
//            }
//            else
//            {
//                Debug.Log(hit2D[0].collider.gameObject.name);
//                Debug.Log(targetPos);
//                _commonPlayer.transform.position = hit2D[0].point;
//            }

//            Debug.DrawLine(playerPosition, targetPos, Color.red, distance);


//            if (!isCollider)
//            {

//            }

            yield return null;

            _fsm.ChangeState(SkillType.Normal, StateTransition.Overwrite);
        }

        private void Flash_Exit()
        {
        }

        #endregion

        #region FireBall

        private IEnumerator FireBall_Enter()
        {
            AudioManager.Instance.AddToRandomFxSource("fire_ball");
            var p = Instantiate(ResourceLoader.FireBall, _commonPlayer.transform.position,
                Quaternion.Euler(new Vector3(0f, 0f, _commonPlayer.CurrentAngle)));
            p.GetComponent<SkillEntity>().owner = _commonPlayer.Health;
            yield return null;
            _fsm.ChangeState(SkillType.Normal, StateTransition.Overwrite);
        }

        private void FireBall_Exit()
        {
            
        }

        #endregion

        #region CometShoot

        public IEnumerator CometShoot_Enter()
        {
            yield break;
        }

        public void CometShoot_Exit()
        {
        }

        #endregion

        #region Lighting

        public IEnumerator LightningBolt_Enter()
        {
            var playerPos = _commonPlayer.transform.position;
            Instantiate(ResourceLoader.Lighting, new Vector3(playerPos.x + 30, playerPos.y + 20, 0),
                Quaternion.Euler(new Vector3(0, 0, -90f)));
            Instantiate(ResourceLoader.Lighting, new Vector3(playerPos.x - 30, playerPos.y + 20, 0),
                Quaternion.Euler(new Vector3(0, 0, -90f)));
            AudioManager.Instance.AddToRandomFxSource("lighting");
            yield return null;
            _fsm.ChangeState(SkillType.Normal, StateTransition.Overwrite);
        }

        public void LightningBolt_Exit()
        {
        }

        #endregion

        #region DodgeSkills

        private IEnumerator DodgeNone_Enter()
        {
            _fsmDodge.ChangeState(SkillType.Normal, StateTransition.Overwrite);
            yield return null;
        }

        private void DodgeNone_Exit()
        {
        }

        #endregion
    }
}