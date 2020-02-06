using PixelGameAssets.Scripts.Actor;
using PixelGameAssets.Scripts.Entity.InteractableEntity.Pickup;
using UnityEngine;

namespace PixelGameAssets.Scripts.Entity.InteractableEntity
{
    public class WeaponChest : Interactable
    {
        public Animator animator;

        /// <summary>
        /// 宝箱内武器种类
        /// </summary>
        public WeaponPickup[] weaponPickups;
        
        private void Awake()
        {
            entityName = "WeaponChest";
        }

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

//        void OnTriggerStay2D(Collider2D col)
//        {
//            OnTriggerStay2DFunc(col);
//        }

        protected override void OnPlayerTrigger(BasePlayer commonPlayer)
        {
            base.OnPlayerTrigger(commonPlayer);
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Open")) animator.Play("Open");
        }

        public override void TriggerAction()
        {
            if (weaponPickups.Length > 0)
                Instantiate(weaponPickups[Random.Range(0, weaponPickups.Length)],
                    transform.position + new Vector3(0f, -1, 0f), Quaternion.identity);
            else
                Debug.Log("This currWeapon chest has no pickAble weaponBag");
        }
    }
}