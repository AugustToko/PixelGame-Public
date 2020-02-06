using PixelGameAssets.Scripts.Core;
using PixelGameAssets.Scripts.UI.UICanvas;
using UnityEngine;

namespace PixelGameAssets.Scripts.UI
{
    public class ChapterSelectionUi : MonoBehaviour
    {
        public void GoBack()
        {
            UiManager.Instance.DestroyUi(gameObject);
        }
    }
}