using PixelGameAssets.Scripts.Core;
using UnityEngine;

namespace PixelGameAssets.Scripts.Camera
{
    /// <summary>
    /// 小地图专用跟随
    /// </summary>
    public class MinimapPixelCameraFollower : MonoBehaviour
    {
        private Vector2 _cameraPreShake;
        private Vector2 _shakeVector;
        private Vector2 _shakeDirection;
        private int _lastDirectionalShake;
        private float _shakeTimer;
        private Vector2 _position;

        // 目标的位置 (以玩家的目标为基准)
        [Header("Camera Follow")] public Transform m_Target;

        private bool isTargetNull;

        public int m_XOffset;
        public int m_YOffset;
        public Vector2 m_OffsetDir;
        public float m_OffsetAmount = 10f;
        public float m_DampTime = .4f;

        [Header("Camera Limits/Bounds")] public bool m_EnableBounds;
        public int m_MinY, m_MaxY, m_MinX, m_MaxX;

        public static MinimapPixelCameraFollower Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }

            GameManager.Instance.miniPixelCameraFollower = this;
        }

        /// <summary>
        /// 用于初始化
        /// 初始化位置, 摄像机目标的位置
        /// </summary>
        private void Start()
        {
            var position = transform.position;
            _position = new Vector2(Mathf.Round(position.x), Mathf.Round(position.y));
            _cameraPreShake = _position;

            isTargetNull = m_Target == null;
        }

        private void Update()
        {
            if (isTargetNull)
            {
                var player = GameObject.FindGameObjectWithTag(GameManager.PlayerTag);
                m_Target = player != null ? player.transform : null;
            }

            // Camera Shake
            _position = _cameraPreShake;

            if (_shakeTimer > 0f)
            {
                if (OnInterval(0.04f))
                {
                    var num = (int) Mathf.Ceil(_shakeTimer * 10f);
                    if (_shakeDirection == Vector2.zero)
                    {
                        _shakeVector.x = -(float) num + new System.Random().Next(num * 2 + 1);
                        _shakeVector.y = -(float) num + new System.Random().Next(num * 2 + 1);

                        // 在屏幕抖动时使用tilemap修复切片之间的白线
                        _shakeVector = _lastDirectionalShake * num * -_shakeVector;
                        if (Mathf.Abs(_shakeVector.y) > 1f)
                        {
                            _shakeVector.y = 1f * Mathf.Sign(_shakeVector.y);
                        }
                    }
                    else
                    {
                        if (_lastDirectionalShake == 0)
                        {
                            _lastDirectionalShake = 1;
                        }
                        else
                        {
                            _lastDirectionalShake *= -1;
                        }

                        // 在屏幕抖动时使用tilemap修复切片之间的白线
                        _shakeVector = _lastDirectionalShake * num * -_shakeDirection;
                        if (Mathf.Abs(_shakeVector.y) > 1f)
                        {
                            _shakeVector.y = 1f * Mathf.Sign(_shakeVector.y);
                        }
                    }
                }

                _shakeTimer -= Time.deltaTime;
            }
            else
            {
                _shakeVector = Vector2.zero;
            }

            // Camera Follow
            if (!m_Target) return;

            // 指向鼠标
            var vector = m_OffsetDir * m_OffsetAmount;
            m_XOffset = (int) vector.x;
            m_YOffset = (int) vector.y;

            Vector3 position = m_Target.position;
            int targetX = (int) position.x + m_XOffset;
            int targetY = (int) position.y + m_YOffset;

            // 水平跟随
            if (_position.x != targetX)
            {
                // Smooth
                _position.x = (int) Mathf.Lerp(_position.x, targetX, 1 / m_DampTime * Time.deltaTime);

                // 水平边界
                if (m_EnableBounds)
                {
                    _position.x = Mathf.Clamp((int) _position.x, m_MinX, m_MaxX);
                }
            }

            // 垂直跟随
            if (_position.y != targetY)
            {
                // Smooth
                _position.y = (int) Mathf.Lerp(_position.y, targetY, 1 / m_DampTime * Time.deltaTime);

                // 垂直边界
                if (m_EnableBounds)
                {
                    _position.y = Mathf.Clamp((int) _position.y, m_MinY, m_MaxY);
                }
            }
        }

        private void LateUpdate()
        {
            _cameraPreShake = _position;
            _position += _shakeVector;
            _position = new Vector2(Mathf.Floor(_position.x), Mathf.Floor(_position.y));

            transform.position = new Vector3(_position.x, _position.y, -10f);
        }

        private bool OnInterval(float interval)
        {
            return (int) ((Time.time - Time.deltaTime) / interval) < (int) (Time.time / interval);
        }

        public void DirectionalShake(Vector2 dir, float time = 0.15f)
        {
            _shakeDirection = dir.normalized;
            _lastDirectionalShake = 0;
            _shakeTimer = Mathf.Max(_shakeTimer, time);
        }

        public void Shake(float time = 0.45f)
        {
            _shakeDirection = Vector2.zero;
            _shakeTimer = Mathf.Max(_shakeTimer, time);
        }

        public void StopShake()
        {
            _shakeTimer = 0f;
        }

        public void CameraRecoil(float force)
        {
            var amount = m_OffsetDir * force;
            amount.x = -(int) amount.x;
            amount.y = -(int) amount.y;

            _position += amount;
            transform.position += new Vector3(amount.x, amount.y, 0);
        }
    }
}