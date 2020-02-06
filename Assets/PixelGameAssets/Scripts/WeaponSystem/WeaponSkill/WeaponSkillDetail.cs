using PixelGameAssets.Scripts.SkillSystem;
using PixelGameAssets.Scripts.SkillSystem.Detail;

namespace PixelGameAssets.Scripts.WeaponSystem.WeaponSkill
{
    public class WeaponSkillDetail : SkillDetail
    {
        
        private WeaponSkillDetail(Builder builder)
        {
            skillType = builder.SkillType;
            skillLevel = builder.SkillLevel;
            tips = builder.Tips;
            maxTimes = builder.MaxTimes;
            coldDownTime = builder.ColdDownTime;
        }
        
        public class Builder
        {
            public Builder(SkillType skillType, byte skillLevel)
            {
                SkillType = skillType;
                SkillLevel = skillLevel;
            }

            public SkillType SkillType { get; }
            
            public byte SkillLevel { get; }
            
            public byte MaxTimes { get; private set; }
            
            public string Tips { get; private set; }

            public int ColdDownTime = 0;

            public Builder SetTips(string tips)
            {
                Tips = tips;
                return this;
            }

            public Builder SetMaxTimes(byte times)
            {
                MaxTimes = times;
                return this;
            }

            public Builder SetColdDownTime(int time)
            {
                ColdDownTime = time;
                return this;
            }

            public WeaponSkillDetail Build()
            {
                return new WeaponSkillDetail(this);
            }
        }
    }
}