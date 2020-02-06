using PixelGameAssets.Scripts.WeaponSystem.WeaponType.GunSys;
using UnityEngine;

namespace PixelGameAssets.Scripts.WeaponSystem
{
    public class BowHelper : MonoBehaviour
    {
        public Bow weapon;

        public void BowAnimationDone()
        {
            weapon.TriggerWeaponPublic();
            weapon.animator.Play("Idle");
        }
    }
}