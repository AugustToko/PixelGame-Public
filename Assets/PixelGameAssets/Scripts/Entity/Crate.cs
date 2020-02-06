using PixelGameAssets.Scripts.Camera;
using UnityEngine;

namespace PixelGameAssets.Scripts.Entity
{
    public class Crate : MonoBehaviour
    {
        public GameObject DestroyFX;

        public int hitsTaken;
        public int maxHitsNumber = 3;

        public BoxCollider2D myCollider;

        public SpriteRenderer spriteRenderer;

        public Sprite[] sprites;

        public void TakeHit()
        {
            hitsTaken = Mathf.Min(hitsTaken + 1, maxHitsNumber);

            // Camera Shake
            if (CameraShaker.Instance != null) CameraShaker.InitShake(0.075f, 1f);

            // Set the right sprite
            if (spriteRenderer != null) spriteRenderer.sprite = sprites[hitsTaken];
        }

        public void Die()
        {
            // Disable the collider
            if (myCollider != null) myCollider.enabled = false;

            // Instantiate the destroy FX
            if (DestroyFX != null) Instantiate(DestroyFX, transform.position, Quaternion.identity);

            // Camera Shake
            if (CameraShaker.Instance != null) CameraShaker.InitShake(0.12f, 1f);

            // Destroy the gameobject
            Destroy(gameObject);
        }
    }
}