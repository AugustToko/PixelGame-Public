using PixelGameAssets.Scripts.Actor;
using PixelGameAssets.Scripts.Core;
using PixelGameAssets.Scripts.InputSystem;
using PixelGameAssets.Scripts.UI.UICanvas;
using UnityEngine;

namespace PixelGameAssets.Scripts.Entity.InteractableEntity
{
    public class Interactable : MonoBehaviour
    {
        public const string InteractableTag = "Player";
        
        [Header("Property")] public InputAction actionToTrigger;

        public bool oneTimes = true;

        public bool Pickable = true;

        public string entityName = null;

        protected BasePlayer BasePlayer;

        public Vector2 PopupOffset;

        protected bool IsInside;

        private static Interactable _target;

        // 按下按钮以作用
        [Header("Input")] public bool PressToInteract = true;

        public bool spawnsPopup = true;

        protected bool wasInside;

        protected void Update()
        {

            if (!wasInside && IsInside && spawnsPopup && PressToInteract)
                GameManager.Instance.SpawnPopup(new Vector2(transform.position.x, transform.position.y) +
                                                PopupOffset);
            if (IsInside && BasePlayer != null && Pickable)
            {
                if (PressToInteract)
                {
                    if (BasePlayer.Input.GetButtonDown(BasePlayer.playerNumber, actionToTrigger))
                        if (_target != null)
                            _target.OnPlayerTrigger(BasePlayer);
                }
                else
                {
                    if (_target != null) _target.OnPlayerTrigger(BasePlayer);
                }
            }

            wasInside = IsInside;
        }

        /// <summary>
        /// 检测碰撞者是否为 Player
        /// 如是, 进入 Enter 方法
        /// </summary>
        /// <param name="other"></param>
        protected void OnTriggerEnter2DFunc(Collider2D other)
        {
            if (other.CompareTag(InteractableTag) && Pickable)
            {
                var playercomponent = other.GetComponent<BasePlayer>();
                if (playercomponent != null) OnPlayerEnter(playercomponent);
            }
        }

        /// <summary>
        /// 检测取消碰撞的物体是否为 Player
        /// 如是, 进入 Exit 方法
        /// </summary>
        /// <param name="other"></param>
        protected void OnTriggerExit2DFunc(Collider2D other)
        {
            if (other.CompareTag(InteractableTag) && Pickable)
            {
                var playercomponent = other.GetComponent<BasePlayer>();
                if (playercomponent != null) OnPlayerExit(playercomponent);
            }
        }

        protected void OnTriggerStay2DFunc(Collider2D other)
        {
            if (other.CompareTag(InteractableTag) && Pickable && _target != null)
            {
                _target = this;
                var playercomponent = other.GetComponent<BasePlayer>();
                if (playercomponent != null) OnPlayerEnter(playercomponent);
            }
        }

        protected virtual void OnPlayerEnter(BasePlayer commonPlayer)
        {
            _target = this;
            IsInside = true;
            BasePlayer = commonPlayer;
            UiManager.Instance.SetTargetInfo(entityName);
        }

        protected void UpdateTargetInfo()
        {
            UiManager.Instance.SetTargetInfo(entityName);
        }

        protected virtual void OnPlayerExit(BasePlayer commonPlayer)
        {
            if (IsInside && BasePlayer == commonPlayer)
            {
                IsInside = false;
                BasePlayer = null;
                if (GameManager.Instance != null && PressToInteract && spawnsPopup)
                {
                    GameManager.Instance.DeSpawnPopup();
                }
            }

            UiManager.Instance.SetTargetInfo("");
        }

        /// <summary>
        /// for <see cref="Scripts.Actor.BasePlayer"/>
        /// </summary>
        /// <param name="commonPlayer"></param>
        protected virtual void OnPlayerTrigger(BasePlayer commonPlayer)
        {
            UiManager.Instance.EventDown = false;
            if (oneTimes)
            {
                Pickable = false;
                _target = null;
            }
            
            if (PressToInteract && spawnsPopup)
            {
                GameManager.Instance.FadePopup();
            }
        }

        public virtual void TriggerAction()
        {
            // Here we drop items, or trigger levers whatever
        }
    }
}