using UnityEngine;

namespace PixelGameAssets.Scripts.InputSystem
{
    public abstract class InputManager : MonoBehaviour, IInputManager
    {
        private static InputManager _instance;

        public static IInputManager Instance => _instance;

        public static void SetInstance(InputManager instance)
        {
            if (_instance == instance) return;

            if (_instance != null)
            {
                _instance.enabled = false;
            }

            _instance = instance;
        }

        private bool _dontDestroyOnLoad = true;

        protected virtual void Awake()
        {
            if (_dontDestroyOnLoad) DontDestroyOnLoad(transform.root.gameObject);
        }

        public virtual bool isEnabled
        {
            get => isActiveAndEnabled;
            set => enabled = value;
        }

        public abstract bool GetButton(int playerId, InputAction action);
        public abstract bool GetButtonDown(int playerId, InputAction action);
        public abstract bool GetButtonUp(int playerId, InputAction action);
        public abstract float GetAxis(int playerId, InputAction action);
    }
}