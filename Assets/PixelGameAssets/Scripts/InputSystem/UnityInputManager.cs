using System.Collections.Generic;
using PixelGameAssets.Scripts.Core;
using PixelGameAssets.Scripts.UI.UICanvas;
using UnityEngine;

namespace PixelGameAssets.Scripts.InputSystem
{
    /// <summary>
    /// 操作输入管理类
    /// </summary>
    public class UnityInputManager : InputManager
    {
        [SerializeField] private string _playerAxisPrefix = "CommonPlayer";

        [SerializeField] private int _maxNumberOfPlayers = 1;

        [Header("Unity Axis Mapping")] [SerializeField]
        private string _fireAxis = "Jump";

        [SerializeField] private string _speedUpAxis = "SpeedUp";

        [SerializeField] private string _displacementSkill = "DisplacementSkill";

        [SerializeField] private string _firstSkill = "FirstSkill";
        [SerializeField] private string _secondSkill = "SecondSkill";
        [SerializeField] private string _thirdSkill = "ThirdSkill";

        [SerializeField] private string _rollAxis = "Dash";

        [SerializeField] private string _interactAxis = "Interact";

        [SerializeField] private string _moveXAxis = "Horizontal";

        [SerializeField] private string _moveYAxis = "Vertical";

        [SerializeField] private string _cancel = "Cancel";

        private Dictionary<int, string>[] _actions;

        protected override void Awake()
        {
            base.Awake();

            if (Instance != null)
            {
                isEnabled = false;
                return;
            }

            SetInstance(this);

            _actions = new Dictionary<int, string>[_maxNumberOfPlayers];

            for (var i = 0; i < _maxNumberOfPlayers; i++)
            {
                var playerActions = new Dictionary<int, string>();
                _actions[i] = playerActions;
                var prefix = !string.IsNullOrEmpty(_playerAxisPrefix) ? _playerAxisPrefix + i : string.Empty;
                AddAction(InputAction.Fire, prefix + _fireAxis, playerActions);
                AddAction(InputAction.Roll, prefix + _rollAxis, playerActions);
                AddAction(InputAction.Interact, prefix + _interactAxis, playerActions);
                AddAction(InputAction.MoveX, prefix + _moveXAxis, playerActions);
                AddAction(InputAction.MoveY, prefix + _moveYAxis, playerActions);
                AddAction(InputAction.SpeedUp, prefix + _speedUpAxis, playerActions);
                AddAction(InputAction.DisplacementSkill, prefix + _displacementSkill, playerActions);
                AddAction(InputAction.FirstSkill, prefix + _firstSkill, playerActions);
                AddAction(InputAction.SecondSkill, prefix + _secondSkill, playerActions);
                AddAction(InputAction.ThirdSkill, prefix + _thirdSkill, playerActions);
                AddAction(InputAction.Cancel, _cancel, playerActions);
            }
        }

        private static void AddAction(InputAction action, string actionName, IDictionary<int, string> actions)
        {
            if (string.IsNullOrEmpty(actionName))
            {
                return;
            }

            actions.Add((int) action, actionName);
        }

        /// <summary>
        /// move
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public override bool GetButton(int playerId, InputAction action)
        {
            // for pc
            bool value = Input.GetButton(_actions[playerId][(int) action]);
            return CheckUiButtonUp(playerId, action) || value;
        }

        public override bool GetButtonDown(int playerId, InputAction action)
        {
            bool value;
            value = !GameManager.Instance.isMobile
                ? Input.GetButtonDown(_actions[playerId][(int) action])
                : CheckUiButtonUp(playerId, action);

            return value;
        }

        public override bool GetButtonUp(int playerId, InputAction action)
        {
            bool value;
            value = GameManager.Instance.isMobile
                ? CheckUiButtonUp(playerId, action)
                : Input.GetButtonUp(_actions[playerId][(int) action]);

            return value;
        }

        public override float GetAxis(int playerId, InputAction action)
        {
            // only for pc
            float value = Input.GetAxisRaw(_actions[playerId][(int) action]);
            return value;
        }

        private bool CheckUiButtonUp(int playerId, InputAction action)
        {
            if (UiManager.Instance.EventDown && _actions[playerId][(int) action].Equals("Player0Interact"))
            {
                return true;
            }

            if (UiManager.Instance.RollButtonDone && _actions[playerId][(int) action].Equals("Player0Roll"))
            {
                return true;
            }

            if (UiManager.Instance.SpaceButtonDone && _actions[playerId][(int) action].Equals("Player0MainSkill"))
            {
                return true;
            }

            if (UiManager.Instance.SpeedUpButton && _actions[playerId][(int) action].Equals("Player0SpeedUp"))
            {
                return true;
            }

            return false;
        }
    }
}