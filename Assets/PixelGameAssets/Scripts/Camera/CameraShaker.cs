using PixelGameAssets.Scripts.Core;
using UnityEngine;

namespace PixelGameAssets.Scripts.Camera
{
    /// <summary>
    /// 用于控制摄像机抖动 (比如发射子弹时摄像机抖动)
    /// </summary>
    public class CameraShaker : MonoBehaviour
    {
        public static CameraShaker Instance;

        /// <summary>
        /// 默认位置
        /// </summary>
        private readonly Vector3 _defPos = new Vector3(0f, 0f, 0f);

        private Vector3 _lastPosition;

        /// <summary>
        /// 控制抖动强度 (shake power)
        /// </summary>
        private float _shakePower;

        /// <summary>
        /// 抖动时间
        /// </summary>
        private float _shakeTime;

        private void Awake()
        {
            Instance = this;
        }

        /// <summary>
        /// 随机抖动: 以摄像机 "中心点" 为圆心, 半径为1, 圆内随机取值, 然后乘以 <see cref="_shakePower" />
        /// </summary>
        private void Update()
        {
            if (_shakeTime < 0f || GameManager.IsStop) return;

            var value = Random.insideUnitCircle.normalized * _shakePower;
            transform.localPosition = _defPos + new Vector3(value.x, value.y, 0f);
            _shakeTime -= Time.deltaTime;

            if (_shakeTime >= 0f) return;

            _shakePower = 0f;
            transform.localPosition = _defPos;
        }

        /// <summary>
        ///     初始化
        /// </summary>
        /// <param name="time">抖动时间</param>
        /// <param name="pwr">抖动强度</param>
        public static void InitShake(float time, float pwr)
        {
            if (pwr <= Instance._shakePower || time <= Instance._shakeTime) return;
            Instance._shakeTime = time;
            Instance._shakePower = pwr;
        }
    }
}