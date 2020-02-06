using UnityEngine;
using UnityEngine.UI;

namespace PixelGameAssets.Scripts.UI.UICanvas
{
    /// <summary>
    ///     显示在 UI 界面上的血量
    /// </summary>
    public class HealthBar : MonoBehaviour
    {
        public static HealthBar Instance;
        public Image[] sprites;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else if (Instance != this) Destroy(gameObject);
        }

        public static void SetHearts(int amount)
        {
            for (var i = 0; i < Instance.sprites.Length; i++)
            {
                if (i < amount)
                {
                    if (!Instance.sprites[i].enabled) Instance.sprites[i].enabled = true;
                }
                else
                {
                    if (Instance.sprites[i].enabled) Instance.sprites[i].enabled = false;
                }
            }
        }
    }
}