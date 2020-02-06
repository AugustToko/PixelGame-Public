using PixelGameAssets.Scripts.Actor;
using PixelGameAssets.Scripts.WeaponSystem;
using PixelGameAssets.Scripts.WeaponSystem.WeaponType;
using UnityEngine;

namespace PixelGameAssets.Scripts.Entity.InteractableEntity.Pickup
{
    public class MeleePickup : Interactable
    { 
        public Animator scrollAnimator;

        /// <summary>
        /// 武器资源
        /// </summary>
        public Weapon wepRes;

        private new void Update()
        {
            // 如果玩家碰撞到该武器, 则展开动画, 滚动卷轴
            if (!wasInside && IsInside)
            {
                if (!scrollAnimator.GetCurrentAnimatorStateInfo(0).IsName("ScrollAppear"))
                    scrollAnimator.Play("ScrollAppear");
            }
            else if (wasInside && !IsInside)
            {
                if (!scrollAnimator.GetCurrentAnimatorStateInfo(0).IsName("ScrollDisappear"))
                    scrollAnimator.Play("ScrollDisappear");
            }

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

        protected override void OnPlayerTrigger(BasePlayer player)
        {
            if (player is CommonPlayer commonPlayer)
            {
                base.OnPlayerTrigger(player);
                commonPlayer.EquipWeapon(wepRes);
                Destroy(gameObject);
            }
        }
    }
}