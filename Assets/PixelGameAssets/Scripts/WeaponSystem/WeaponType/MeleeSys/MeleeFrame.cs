using UnityEngine;

namespace PixelGameAssets.Scripts.WeaponSystem.WeaponType.MeleeSys
{
    public class MeleeFrame : MonoBehaviour
    {
        public Melee weapon { get; set; }

        public Animator atkAnimator;

        public int currentStage = 1;

        private int _maxStage = 2;

        private bool _canDamage = false;

        private const float ColdTime = 1f;

        private float _coldTimer;

        public bool animationDone = true;

        private void Awake()
        {
            weapon = GetComponentInParent<Melee>();
        }

        private void Update()
        {
            if (_coldTimer > 0f)
            {
                _coldTimer -= Time.deltaTime;
                return;
            }

            if (_coldTimer <= 0f && currentStage > 1)
            {
                currentStage--;
            }
        }

        public void DoAnimation()
        {
            switch (currentStage)
            {
                case 1:
                    atkAnimator.Play("Atk1", 0, 0f);
                    break;
                case 2:
                    atkAnimator.Play("Pierce", 0, 0f);
                    break;
                default:
                    break;
            }
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (!_canDamage) return;

            weapon.TryTakeAttackDamage(col, currentStage - 1);
        }

        private void OnTriggerStay2D(Collider2D col)
        {
            if (!_canDamage) return;

            weapon.TryTakeAttackDamage(col, currentStage - 1);
        }

        public class DamagePack
        {
            public Collider2D Collider2D;

            public int Stage;

            public DamagePack(Collider2D collider2D, int stage)
            {
                Collider2D = collider2D;
                this.Stage = stage;
            }
        }

        public void StartAttack()
        {
            if (weapon.Collider2D != null)
            {
                weapon.Collider2D.enabled = true;
            }
            _canDamage = true;
            animationDone = false;
        }

        public void Attacked()
        {
            _canDamage = false;
        }

        public void EndAttack()
        {
            weapon.components.Clear();
            animationDone = true;
            _coldTimer = ColdTime;
            currentStage++;

            if (currentStage > _maxStage)
            {
                currentStage = 1;
            }

            if (weapon.Collider2D != null)
            {
                weapon.Collider2D.enabled = false;
            }
        }
    }
}