using PixelGameAssets.Scripts.Core;
using PixelGameAssets.Scripts.WeaponSystem.WeaponType;
using PixelGameAssets.Scripts.WeaponSystem.WeaponType.MeleeSys;
using UnityEngine;

namespace PixelGameAssets.Scripts.Damage
{
    public class DamageOnTouch : MonoBehaviour
    {
        [Header("Damage")] public int DamageToCause = 1;

        [Header("Targets")] public string TargetTag = GameManager.PlayerTag;

        private Weapon _unarmed;

        private void Awake()
        {
            _unarmed = gameObject.AddComponent<Unarmed>();
        }

        public virtual void OnTriggerEnter2D(Collider2D collider)
        {
            Colliding(collider);
        }

        public virtual void OnTriggerStay2D(Collider2D collider)
        {
            Colliding(collider);
        }

        protected virtual void Colliding(Collider2D collider)
        {
            if (!isActiveAndEnabled) return;

            if (!collider.gameObject.CompareTag(TargetTag)) return;

            var health = collider.gameObject.GetComponent<Health>();

            if (health == null) return;
            if (health.health > 0 && !health.invincible)
                // Apply the Damage
            {
                GetComponent<Actor.Base.Actor>().Attack(ref _unarmed);
                health.TakeDamage(DamageToCause);
            }
        }
    }
}