using PixelGameAssets.Scripts.Actor;
using PixelGameAssets.Scripts.Core;
using PixelGameAssets.Scripts.Misc;
using UnityEngine;

namespace PixelGameAssets.Scripts.Entity.InteractableEntity
{
    public class TransferPosition : Interactable
    {
        public Vector3 target;

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
            base.OnPlayerTrigger(commonPlayer);
            GameManager.Instance.Player.transform.position = target;
        }
    }
}