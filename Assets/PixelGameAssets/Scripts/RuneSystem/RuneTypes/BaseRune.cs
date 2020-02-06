using UnityEngine;

namespace PixelGameAssets.Scripts.RuneSystem.RuneTypes
{
    public abstract class BaseRune : MonoBehaviour
    {
        /// <summary>
        /// 符文描述, 请在 Awake 方法中赋值
        /// </summary>
        public string description { get; protected set; } = "The Rune description.";

        /// <summary>
        /// 激活符文
        /// </summary>
        public abstract void Active();
        
        /// <summary>
        /// 取消激活符文
        /// </summary>
        public abstract void DisActive();
    }
}