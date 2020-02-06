using PixelGameAssets.Scripts.UI.UICanvas;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PixelGameAssets.Scripts.InputSystem.Buttons
{
    public class RollButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler

    {
        public void OnPointerDown(PointerEventData eventData)
        {
            UiManager.Instance.RollButtonDone = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            UiManager.Instance.RollButtonDone = false;
        }
    }
}