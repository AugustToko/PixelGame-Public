using PixelGameAssets.Scripts.Actor;
using PixelGameAssets.Scripts.Core;
using PixelGameAssets.Scripts.Misc;
using PixelGameAssets.Scripts.UI.UICanvas;
using UnityEngine;

namespace PixelGameAssets.Scripts.Entity.InteractableEntity
{
    public class BulletinBoard : Interactable
    {
        private GameObject _gameObject;
        
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
            if (_gameObject != null) return;

            _gameObject = Instantiate(ResourceLoader.ChapterSelectionUi, Vector3.zero, Quaternion.Euler(Vector3.zero));
            UiManager.Instance.ShowedUi(ref _gameObject);
            base.OnPlayerTrigger(commonPlayer);
        }
    }
}