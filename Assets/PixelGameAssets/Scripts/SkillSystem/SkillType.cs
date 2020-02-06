using System;

namespace PixelGameAssets.Scripts.SkillSystem
{
    /// <summary>
    /// 展示所有的技能
    /// > 1. Dodge开头为闪避技能
    /// > 2. Wp开头为武器技能
    /// </summary>
    [Serializable]
    public enum SkillType
    {
        None = -1,

        /// 通常
        /// For: <see cref="DebugSkillImp.Normal_Update" />
        Normal = 0,
        
        //COMMON-SKILL//

        /// <summary>
        /// 快速移动 固定技能
        /// <see cref="DebugSkillImp.Displacement_Enter" />
        /// <see cref="DebugSkillImp.Displacement_Exit" />
        /// </summary>
        Displacement = 1,

        /// 闪现 for debug
        /// <see cref="DebugSkillImp.Flash_Enter" />
        /// <see cref="DebugSkillImp.Flash_Exit" />
        Flash = 2,
        
        /// <summary>
        /// 火球术
        /// <see cref="DebugSkillImp.FireBall_Enter"/>
        /// <see cref="DebugSkillImp.FireBall_Exit"/>
        /// </summary>
        FireBall,
        
        /// <summary>
        /// 闪电
        /// <see cref="DebugSkillImp.LightningBolt_Enter"/>
        /// <see cref="DebugSkillImp.LightningBolt_Exit"/>
        /// </summary>
        LightningBolt,
        
        CometShoot,
        
        #region DodgeSkill

        DodgeNone,

        #endregion
        
        #region WeaponSkill

        WpNone,
        
        /// <summary>
        /// 子弹爆裂 for debug
        /// </summary>
        WpBurst

        #endregion

    }
}