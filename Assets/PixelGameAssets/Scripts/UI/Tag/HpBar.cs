using PixelGameAssets.Scripts.UI.Tag.Base;
using UnityEngine;
using UnityEngine.UI;

namespace PixelGameAssets.Scripts.UI.Tag
{
    public class HpBar : BaseTag
    {
        [SerializeField] private Slider slider;
        [SerializeField] private Text hpText;

        private int _hpMax = 1;

        /// <summary>
        /// 设置 HP 值
        /// </summary>
        /// <param name="hp"></param>
        public void SetHp(int hp)
        {
            if (hp < 0 || hp > 100)
            {
                return;
            }

            slider.value = hp / (float) _hpMax;

            hpText.text = hp.ToString();
        }

        public void SetMaxHp(int val)
        {
            if (val == 0)
            {
                return;
            }

            _hpMax = val;
        }
    }
}