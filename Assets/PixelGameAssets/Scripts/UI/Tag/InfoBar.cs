using PixelGameAssets.Scripts.UI.Tag.Base;
using PixelGameAssets.Scripts.UI.Tag.Model;
using TMPro;
using UnityEngine;

namespace PixelGameAssets.Scripts.UI.Tag
{
    public class InfoBar : BaseTag
    {
        [SerializeField] private TextMeshProUGUI _textMeshProUgui;

        public void SetupInfo(InfoData data)
        {
            _textMeshProUgui.text = data.Level + " " + data.Name;
        }
    }
}