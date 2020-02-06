using PixelGameAssets.Scripts.Actor;
using PixelGameAssets.Scripts.EventManager;
using PixelGameAssets.Scripts.WeaponSystem.WeaponInfo;
using UnityEngine;
using Notification = PixelGameAssets.Scripts.UI.NotificationSystem.Notification;

namespace PixelGameAssets.Scripts.Entity.InteractableEntity
{
    /// <summary>
    /// 弹药箱 (无限) (for <see cref="CommonPlayer"/>)
    /// </summary>
    public class AmmunitionBox : Interactable
    {
        public int ammo = 500;

        private void Awake()
        {
            entityName = ammo == -1 ? "AmmunitionBox" : "AmmunitionBox: " + ammo;
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

        protected override void OnPlayerTrigger(BasePlayer commonPlayer)
        {
            if (commonPlayer.CurrWeapon == null || ammo == 0)
            {
                return;
            }

            if (commonPlayer.CurrWeapon.weaponData is GunInfo info)
            {
                info.remainingBullet += 100;
                ammo -= 100;

                if (ammo >= 0)
                {
                    entityName = "AmmunitionBox: " + ammo;
                    UpdateTargetInfo();
                }

                var tra = GameObject.Find("UI-Canvas").transform;
                var builder = new Notification.Builder(ref tra, "Ammunition supply box", "Total ammunition + 100");
                builder.Show();

                EventCenter.Broadcast(EventManager.EventType.UpdateAmmo, info.clipCapacity, info.remainingBullet);
            }

            base.OnPlayerTrigger(commonPlayer);
        }
    }
}