using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace PixelGameAssets.Scripts.UI.SkillBar
{
    /// <summary>
    /// 技能框
    /// </summary>
    public class SkillBox : MonoBehaviour
    {
        [SerializeField] private Image skillIcon;

        [SerializeField] private Image mask;

        public void SetSkillIcon([NotNull] Sprite icon)
        {
            skillIcon.sprite = icon;
        }

        /// <summary>
        /// 启用(显示) MASK
        /// </summary>
        public void EnableMask()
        {
            mask.fillAmount = 1;
        }

        /// <summary>
        /// 设置 MASK 值
        /// </summary>
        /// <param name="val">val</param>
        public void SetMaskAmount(float val)
        {
            mask.fillAmount = val;
        }
    }
}