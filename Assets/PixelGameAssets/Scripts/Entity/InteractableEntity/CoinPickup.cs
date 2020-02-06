using PixelGameAssets.Scripts.Actor;
using PixelGameAssets.Scripts.Camera;
using PixelGameAssets.Scripts.Core;
using PixelGameAssets.Scripts.Core.Audio;
using UnityEngine;

namespace PixelGameAssets.Scripts.Entity.InteractableEntity
{
    public class CoinPickup : Interactable
    {

        public int amount = 1;
        
        public GameObject PickFxPrefab;

        // Update is called once per frame
        private new void Update()
        {
            base.Update();
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            OnTriggerEnter2DFunc(col);
        }

        private void OnTriggerStay2D(Collider2D col)
        {
            OnTriggerStay2DFunc(col);
        }

        private void OnTriggerExit2D(Collider2D col)
        {
            OnTriggerExit2DFunc(col);
        }

        protected override void OnPlayerTrigger(BasePlayer commonPlayer)
        {
            base.OnPlayerTrigger(commonPlayer);

            GameManager.Instance.PlayerInfo.OperateCoin(amount);

            // Instantiate the pickup fx
            if (PickFxPrefab != null) Instantiate(PickFxPrefab, transform.position, Quaternion.identity);

            AudioManager.Instance.PlayAudioEffectHit("coin_pickup");
            
            // Destroy the gameObject
            Destroy(gameObject);
        }
    }
}