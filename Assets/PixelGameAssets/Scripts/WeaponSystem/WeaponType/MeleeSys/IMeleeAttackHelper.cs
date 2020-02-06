namespace PixelGameAssets.Scripts.WeaponSystem.WeaponType.MeleeSys
{
    public interface IMeleeAttackHelper
    {
        void BeginAttack();
        void Attack();
        void AfterAttack();
        void Done();
        void DoAnimation();
    }
}