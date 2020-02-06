using UnityEngine;
using UnityEngine.Serialization;

namespace PixelGameAssets.Scripts.Misc
{
    /// <summary>
    /// 准星
    /// </summary>
    public class Crosshair : MonoBehaviour
    {
        private Vector3 _mouseCoords;
        
        // 鼠标灵敏度
        public float mouseSensitivity = 1f;

        /// <summary>
        /// 实时跟随
        /// </summary>
        private void Update()
        {
            _mouseCoords = UnityEngine.Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = Vector2.Lerp(transform.position, _mouseCoords, mouseSensitivity);
        }
    }
}