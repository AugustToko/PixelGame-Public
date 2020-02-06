using PixelGameAssets.Scripts.Actor;

namespace PixelGameAssets.Scripts.SkillSystem
{
    /// 技能实现接口，有别于技能类型枚举:
    /// <see cref="SkillType" />
    /// 所有技能实现的接口
    /// DEBUG实现, Release实现
    /// 本接口用于调试
    public interface ISkill
    {
        void Init(BasePlayer player);
    }
}