using System;

namespace PixelGameAssets.Scripts.SkillSystem.Detail
{
    /// <summary>
    ///     技能细节
    /// </summary>
    [Serializable]
    public class CommonSkillDetail : SkillDetail
    {
        private CommonSkillDetail(Builder builder)
        {
            skillType = builder.SkillType;
            skillLevel = builder.SkillLevel;
            tips = builder.Tips;
            maxTimes = builder.MaxTimes;
            coldDownTime = builder.ColdDownTime;
            skillName = builder.SkillName;
        }

        public class Builder
        {
            public Builder(SkillType skillType, byte skillLevel, string skillName)
            {
                SkillType = skillType;
                SkillLevel = skillLevel;
                SkillName = skillName;
            }

            public SkillType SkillType { get; }
            
            public string SkillName { get; set; }
            
            public byte SkillLevel { get; }
            public byte MaxTimes { get; private set; }
            public string Tips { get; private set; }

            public float ColdDownTime = 0;

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

            public Builder SetColdDownTime(float time)
            {
                ColdDownTime = time;
                return this;
            }

            public CommonSkillDetail Build()
            {
                return new CommonSkillDetail(this);
            }
        }
    }
}