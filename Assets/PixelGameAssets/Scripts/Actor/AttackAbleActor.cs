using PixelGameAssets.Scripts.SkillSystem.Detail;
using PixelGameAssets.Scripts.WeaponSystem.WeaponType;
using UnityEngine;

namespace PixelGameAssets.Scripts.Actor
{
    public abstract class AttackAbleActor : MonoBehaviour
    {
        /// <summary>
        /// Actor 攻击的抽象方法
        /// </summary>
        /// <param name="weaponTarget">武器</param>
        /// <returns>是否进行了攻击</returns>
        public abstract bool Attack(ref Weapon weaponTarget);

        /// <summary>
        /// 使用技能
        /// </summary>
        /// <param name="skill">技能细节</param>
        /// <returns>是否攻击成功</returns>
        public abstract bool TryUseSkill(SkillDetail skill);
    }
}