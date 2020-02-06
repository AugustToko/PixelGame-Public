using PixelGameAssets.Scripts.Actor;
using PixelGameAssets.Scripts.GKUtils;
using UnityEngine;

namespace PixelGameAssets.Scripts.Misc
{
    public class GunTopDownZOrder : MonoBehaviour
    {
        public SpriteRenderer targetSpriteRenderer { get; set; }
        public int offSet = 1;

        private SpriteRenderer _spriteRenderer;

        private bool _isSpriteRendererNotNull;

        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void LateUpdate()
        {
            if (targetSpriteRenderer == null || _spriteRenderer == null) return;

            _spriteRenderer.sortingOrder = targetSpriteRenderer.sortingOrder + offSet;
        }
    }
}