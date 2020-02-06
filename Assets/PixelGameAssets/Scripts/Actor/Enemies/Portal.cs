using System.Collections;
using PixelGameAssets.MonsterLove.StateMachine;
using UnityEngine;

namespace PixelGameAssets.Scripts.Actor.Enemies
{
    public class Portal : MonoBehaviour
    {
        // States for the state machine
        public enum States
        {
            WaitToActivate,
            Activating,
            Deactivating,
            Idle,
            SpawningSlime
        }

        [Header("Activating")] public float ActivatingTime = 2.1f;

        private float activatingTimer;


        public Animator animator;

        [Header("Deactivating")] public float DeactivatingTime = 2.1f;


        // State Machine
        public StateMachine<States> fsm;
        public Vector2 SlimeOffSet;

        public GameObject SlimePrefab;
        public float slimeSpawnTime = 1.1f;

        [Header("Spawn Slime")] public int spawnAmount = 1;

        public float TimeBeforeDeactivating = 1f;
        private float timeBeforeDeactivatingTimer;

        [Header("Idle")] public float TimeBetweenSpawns = 1f;

        private float timeBetweenSpawnsTimer;

        public float waitTime = 1f;

        private void Awake()
        {
            fsm = StateMachine<States>.Initialize(this);
        }

        private void Update()
        {
            UpdateSprite();
        }

        // Use this for initialization
        private void Start()
        {
            fsm.ChangeState(States.WaitToActivate);
            Invoke(nameof(EnterActivating), waitTime);
        }

        private void EnterActivating()
        {
            fsm.ChangeState(States.Activating);
        }

        private void Activating_Enter()
        {
            activatingTimer = ActivatingTime;
        }

        private void Activating_Update()
        {
            if (activatingTimer > 0f)
            {
                activatingTimer -= Time.deltaTime;

                if (activatingTimer <= 0f) fsm.ChangeState(States.Idle, StateTransition.Overwrite);
            }
        }

        private void Idle_Enter()
        {
            if (spawnAmount > 0)
            {
                spawnAmount--;
                timeBetweenSpawnsTimer = TimeBetweenSpawns;
            }
            else
            {
                timeBeforeDeactivatingTimer = TimeBeforeDeactivating;
            }
        }

        private void Idle_Update()
        {
            if (timeBetweenSpawnsTimer > 0f)
            {
                timeBetweenSpawnsTimer -= Time.deltaTime;

                if (timeBetweenSpawnsTimer <= 0f) SpawnSlime();
            }

            if (timeBeforeDeactivatingTimer > 0f)
            {
                timeBeforeDeactivatingTimer -= Time.deltaTime;

                if (timeBeforeDeactivatingTimer <= 0f) fsm.ChangeState(States.Deactivating, StateTransition.Overwrite);
            }
        }

        private IEnumerator SpawningSlime_Enter()
        {
            yield return new WaitForSeconds(slimeSpawnTime);

            Instantiate(SlimePrefab, new Vector2(transform.position.x, transform.position.y) + SlimeOffSet,
                Quaternion.identity);

            fsm.ChangeState(States.Idle, StateTransition.Overwrite);
        }

        private IEnumerator Deactivating_Enter()
        {
            yield return new WaitForSeconds(DeactivatingTime);

            Destroy(gameObject);
        }

        private void SpawnSlime()
        {
            fsm.ChangeState(States.SpawningSlime, StateTransition.Overwrite);
        }

        private void UpdateSprite()
        {
            if (fsm.State == States.Activating)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Activate")) animator.Play("Activate");
            }
            else if (fsm.State == States.Idle)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) animator.Play("Idle");
            }
            else if (fsm.State == States.SpawningSlime)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("SpawnSlime")) animator.Play("SpawnSlime");
            }
            else if (fsm.State == States.Deactivating)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Deactivate")) animator.Play("Deactivate");
            }
        }
    }
}