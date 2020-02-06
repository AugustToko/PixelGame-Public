using PixelGameAssets.Scripts.UI.UICanvas;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PixelGameAssets.Scripts.InputSystem.Buttons
{
    public class EventButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {

        public void OnPointerDown(PointerEventData eventData)
        {
            // ...
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            UiManager.Instance.EventDown = true;
        }
    }
}