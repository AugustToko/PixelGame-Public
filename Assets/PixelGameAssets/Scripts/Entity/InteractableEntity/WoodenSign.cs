using PixelGameAssets.Scripts.Actor;
using PixelGameAssets.Scripts.UI.UICanvas;
using UnityEngine;

namespace PixelGameAssets.Scripts.Entity.InteractableEntity
{
    public class WoodenSign : Interactable
    {
        public string message = "...";

        private new void Update()
        {
            base.Update();
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            OnTriggerEnter2DFunc(col);
        }

        private void OnTriggerExit2D(Collider2D col)
        {
            OnTriggerExit2DFunc(col);
        }

        protected override void OnPlayerTrigger(BasePlayer commonPlayer)
        {
            UiManager.Instance.ShowMessageOnUi(message);
            base.OnPlayerTrigger(commonPlayer);
        }
    }
}