using System;
using UnityEngine.Serialization;

namespace PixelGameAssets.Scripts.SkillSystem.Detail
{
    /// <summary>
    /// 技能细节, 请使用 <see cref="CommonSkillDetail"/>
    /// TODO: 待完善
    /// </summary>
    [Serializable]
    public abstract class SkillDetail
    {
        public string skillName = "Skill Name";
        
        private int _index = 0;
        
        public float coldDownTime;

        public SkillType skillType;

        // 技能等级
        public byte skillLevel;

        // 备注
        public string tips;

        public byte maxTimes;

        /// <summary>
        /// TODO: SkillIcon 的添加
        /// </summary>
        public string skillIcon = "null";
        
        public void SetIndex(int i)
        {
            if (i < 0)
            {
                return;
            }
            _index = i;
        }

        public int GetIndex()
        {
            return _index;
        }

        public override string ToString()
        {
            return "SkillType: " + skillType.ToString() + ", coldDownTime: " + coldDownTime;
        }
    }
}