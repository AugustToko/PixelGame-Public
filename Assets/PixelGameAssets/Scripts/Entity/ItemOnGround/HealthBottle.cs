using PixelGameAssets.Scripts.Actor;
using PixelGameAssets.Scripts.Damage;
using UnityEngine;

namespace PixelGameAssets.Scripts.Entity.ItemOnGround
{
    public class HealthBottle : MonoBehaviour
    {
        private bool _pickable = true;

        [Header("Heal Amount")] public int healAmount = 1;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player") || !_pickable) return;
            var playerComponent = other.GetComponent<CommonPlayer>();
            if (playerComponent != null)
            {
                OnPlayerTrigger(playerComponent);
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!other.CompareTag("Player") || !_pickable) return;
            
            var playerComponent = other.GetComponent<CommonPlayer>();
            if (playerComponent != null)
            {
                OnPlayerTrigger(playerComponent);
            }
        }

        private void OnPlayerTrigger(Component player)
        {
            var healthComp = player.GetComponent<Health>();
            if (healthComp == null || !healthComp.TakeHeal(healAmount)) return;
            
            _pickable = false;
            Destroy(gameObject);
        }
    }
}