using PixelGameAssets.Scripts.Core;
using UnityEngine;

namespace PixelGameAssets.Scripts.Camera
{
    public class PixelCameraFollower : MonoBehaviour
    {
        private Vector2 _position;

        // 目标的位置 (以玩家的目标为基准)
        [Header("Camera Follow")] public Transform m_Target;
        public int m_XOffset;
        public int m_YOffset;
        public Vector2 m_OffsetDir;
        public float m_OffsetAmount = 10f;
        public float m_DampTime = .4f;

        [Header("Camera Limits/Bounds")] public bool m_EnableBounds;
        public int m_MinY, m_MaxY, m_MinX, m_MaxX;

        public static PixelCameraFollower Instance;

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

            GameManager.Instance.PixelCameraFollower = this;
        }

        /// <summary>
        /// 用于初始化
        /// 初始化位置, 摄像机目标的位置
        /// </summary>
        private void Start()
        {
            var position = transform.position;
            _position = new Vector2(Mathf.Round(position.x), Mathf.Round(position.y));

            // 如果没有目标，搜索玩家
            if (m_Target == null)
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                m_Target = player != null ? player.transform : null;

                if (m_Target == null)
                {
                    Debug.Log("There is no camera target");
                }
            }
        }

        private void Update()
        {

            // Camera Follow
            if (!m_Target) return;

            // 指向鼠标
            var vector = m_OffsetDir * m_OffsetAmount;
            m_XOffset = (int) vector.x;
            m_YOffset = (int) vector.y;

            var position = m_Target.position;
            var targetX = (int) position.x + m_XOffset;
            var targetY = (int) position.y + m_YOffset;

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
            _position = new Vector2(Mathf.Floor(_position.x), Mathf.Floor(_position.y));

            transform.position = new Vector3(_position.x, _position.y, -10f);
        }

        private bool OnInterval(float interval)
        {
            return (int) ((Time.time - Time.deltaTime) / interval) < (int) (Time.time / interval);
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