using PixelGameAssets.Scripts.UI.UICanvas;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PixelGameAssets.Scripts.InputSystem.Buttons
{
    public class SpaceButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler

    {
        public void OnPointerDown(PointerEventData eventData)
        {
            UiManager.Instance.SpaceButtonDone = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            UiManager.Instance.SpaceButtonDone = false;
        }
    }
}