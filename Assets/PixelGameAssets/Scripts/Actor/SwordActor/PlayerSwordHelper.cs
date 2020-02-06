using PixelGameAssets.Scripts.Core.Audio;
using PixelGameAssets.Scripts.Damage;
using PixelGameAssets.Scripts.WeaponSystem.WeaponType.MeleeSys;
using UnityEngine;

namespace PixelGameAssets.Scripts.Actor.SwordActor
{
    public class PlayerSwordHelper : MonoBehaviour, IMeleeAttackHelper
    {
        public int currentStage { get; set; } = 1;

        private const int MaxStage = 2;

        public Animator atkAnimator;

        private bool _canDamage = false;

        public Collider2D Collider2D;

        public float ColdTime = 0.1f;

        private float _coldTimer = 0f;

        public bool canDoAnimation { get; set; } = true;

        public bool isAttack = false;

        public int currAni = 1;

        private void Awake()
        {
            Collider2D.enabled = false;
        }

        private void Update()
        {
            if (_coldTimer > 0)
            {
                _coldTimer -= Time.deltaTime;
            }
            else
            {
                currentStage = 1;
            }
        }

        public void BeginAttack()
        {
            canDoAnimation = false;
        }

        public void Attack()
        {
            AudioManager.Instance.PlayAudioEffectShoot("sword_atk_state_" + currAni);
            Collider2D.enabled = true;
            _canDamage = true;
        }

        public void AfterAttack()
        {
            Collider2D.enabled = false;
        }

        public void Done()
        {
            if (currentStage == 2) currentStage = 1;
            else currentStage++;

            Collider2D.enabled = false;
            _coldTimer = ColdTime;
            canDoAnimation = true;
            isAttack = false;

            if (!atkAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle") &&
                !atkAnimator.GetCurrentAnimatorStateInfo(0).IsName("TakeHit")
            )
            {
                atkAnimator.Play("Idle");
            }
        }

        public void DoAnimation()
        {
            if (!canDoAnimation && isAttack) return;

            switch (currentStage)
            {
                case 1:
                    isAttack = true;
                    atkAnimator.Play("Attack1", 0, 0f);
                    currAni = 1;
                    break;
                case 2:
                    isAttack = true;
                    atkAnimator.Play("Attack2", 0, 0f);
                    currAni = 2;
                    break;
                default:
                    break;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var health = other.gameObject.GetComponent<Health>();

            if (health != null && health.Actor is Enemies.Base.Enemies)
            {
                health.TakeDamage(20);
            }
        }
    }
}