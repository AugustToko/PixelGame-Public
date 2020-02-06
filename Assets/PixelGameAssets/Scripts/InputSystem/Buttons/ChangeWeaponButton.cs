using System;
using System.Collections;
using PixelGameAssets.Scripts.Actor;
using PixelGameAssets.Scripts.Core;
using PixelGameAssets.Scripts.UI.UICanvas;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PixelGameAssets.Scripts.InputSystem.Buttons
{
    public class ChangeWeaponButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {

        public void OnPointerDown(PointerEventData eventData)
        {
            
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            
        }

        public void ChangeWeapon()
        {
            if (GameManager.Instance != null && GameManager.Instance.Player != null && GameManager.Instance.Player is CommonPlayer player)
            {
                player.NextWeapon();
            }
        }
    }
}