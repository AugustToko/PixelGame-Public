using System;
using System.Collections.Generic;
using PixelGameAssets.Scripts.SkillSystem.Detail;

namespace PixelGameAssets.Scripts.SkillSystem
{
    [Serializable]
    public class SkillPack
    {
        /// <summary>
        /// 闪避后释放的技能
        /// </summary>
        private readonly SkillDetail _dodgeSkill;

        public List<SkillDetail> skillDetails = new List<SkillDetail>();

        /// <summary>
        /// 武器技能 (非必需)
        /// </summary>
        private readonly SkillDetail _wpSkill;

        /// <summary>
        /// 闪避技能 (只读, 固定)
        /// </summary>
        private readonly SkillDetail _displacementSkill;

        public SkillPack(SkillDetail dodgeSkill, SkillDetail wpSkill, SkillDetail displacementSkill,
            IEnumerable<SkillDetail> skillDetails)
        {
            _dodgeSkill = dodgeSkill;
            _wpSkill = wpSkill;
            _displacementSkill = displacementSkill;

            this.skillDetails.Clear();
            this.skillDetails.AddRange(skillDetails);
        }

        public SkillDetail GetDisplacementSkill()
        {
            return _displacementSkill;
        }

        public override string ToString()
        {
            return _dodgeSkill.ToString() + skillDetails.ToString() + _wpSkill.ToString() +
                   _displacementSkill.ToString();
        }
    }
}