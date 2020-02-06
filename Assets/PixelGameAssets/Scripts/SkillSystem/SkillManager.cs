using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using PixelGameAssets.Scripts.Actor;
using PixelGameAssets.Scripts.EventManager;
using PixelGameAssets.Scripts.SkillSystem.Detail;
using PixelGameAssets.Scripts.UI.UICanvas;
using UnityEngine;
using EventType = UnityEngine.EventType;

namespace PixelGameAssets.Scripts.SkillSystem
{
    /// <summary>
    /// 技能管理类 (每个 Actor 需分配一个 <see cref="SkillManager" />)
    /// <para>挂载到 Actor 下</para>
    /// </summary>
    public class SkillManager : MonoBehaviour
    {
        /// <summary>
        /// TEMP
        /// </summary>
        private readonly List<SkillDetail> _needColdDown = new List<SkillDetail>();

        private SkillPack _skillPackBackUp;

        /// <summary>
        /// 存储当前玩家装备的技能以及冷却时间
        /// </summary>
        private readonly Dictionary<SkillDetail, float> _skillColdTime = new Dictionary<SkillDetail, float>();

        private BasePlayer _player;
        
        /// <summary>
        /// 技能实现类
        /// </summary>
        private DebugSkillImp _skillImp;

        /// <summary>
        /// 用来提醒 “技能处于冷却时间” 的间隔时间
        /// 两秒内如果多次按下尚处于冷却时间的技能，则只提醒一次
        /// </summary>
        private float noticeSkillInColdTime = 0F;
        
        private const float DefaultNoticeSkillInColdTime = 1F;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="player">玩家</param>
        /// <param name="skillPack">技能包</param>
        public void Init(ref BasePlayer player, ref SkillPack skillPack)
        {
            _player = player;

            _skillPackBackUp = skillPack;

            _skillImp = gameObject.AddComponent<DebugSkillImp>();

            _skillImp.Init(_player);

            InitColdTime(skillPack);
        }

        // 初始化冷却时间
        private void InitColdTime(SkillPack skillPack)
        {
            _skillColdTime.Clear();
            for (var index = 0; index < skillPack.skillDetails.Count; index++)
            {
                var skill = skillPack.skillDetails[index];
                _skillColdTime.Add(skill, 0f);
                skill.SetIndex(index);
            }
        }

        /// <summary>
        /// 使用技能
        /// </summary>
        /// <param name="skill">技能类型</param>
        /// <param name="actor">Actor</param>
        /// <returns>是否释放成功</returns>
        public bool UseSkill(ref SkillDetail skill, ref Actor.Base.Actor actor)
        {

            if (!CheckSkill(ref skill))
            {
                return false;
            }
            
            EventCenter.Broadcast(EventManager.EventType.UpdateSystemEvent, actor.actorType + " use skill: " + skill.skillName);

            UiManager.Instance.skillBarUi.skillBoxes[skill.GetIndex()].EnableMask();
            
            // 释放技能
            _skillImp.UseSkill(ref skill);

            // 计入冷却时间
            _needColdDown.Add(skill);

            // 设定冷却时间
            _skillColdTime[skill] = skill.coldDownTime;

            return true;
        }

        /// <summary>
        /// 检测技能及其冷却时间
        /// </summary>
        /// <param name="skill">Skill</param>
        /// <returns>是否可释放</returns>
        private bool CheckSkill(ref SkillDetail skill)
        {
            // 没有该技能
            // 处于冷却时间
            if (!_skillColdTime.ContainsKey(skill))
            {
                EventCenter.Broadcast(EventManager.EventType.UpdateSystemEvent, "未匹配到该技能");
                return false;
            }

            if (skill.skillType == SkillType.None)
            {
                EventCenter.Broadcast(EventManager.EventType.UpdateSystemEvent, "未匹配到该技能");
                return false;
            }

            if (_skillColdTime[skill] > 0f)
            {
                if (Math.Abs(noticeSkillInColdTime) < 0.1)
                {
                    EventCenter.Broadcast(EventManager.EventType.UpdateSystemEvent, "技能处于冷却中");
                    noticeSkillInColdTime = DefaultNoticeSkillInColdTime;
                }
                
                return false;
            }

            return true;
        }

        /// <summary>
        /// 重置所有技能
        /// </summary>
        public void ResetSkill()
        {
            _needColdDown.Clear();
            InitColdTime(_skillPackBackUp);
            _skillImp.ResetSkill();
        }

        /// <summary>
        /// 获取技能冷却时间
        /// </summary>
        /// <param name="commonSkillType">技能类型</param>
        /// <returns>是否成功释放技能</returns>
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [Obsolete("在创建技能是同时进行冷却时间赋予")]
        public static float GetSkillColdTime(ref CommonSkillDetail commonSkillType)
        {
            // 默认冷却时间 10f (10s)
            var coldDownTime = 0f;
            switch (commonSkillType.skillType)
            {
                case SkillType.Displacement:
                    coldDownTime = 0f;
                    break;
                default:
                    coldDownTime = 0f;
                    break;
            }

            return coldDownTime;
        }

        /// <summary>
        ///     更新冷却时间
        /// </summary>
        private void Update()
        {
            if (noticeSkillInColdTime > 0)
            {
                noticeSkillInColdTime -= Time.deltaTime;
            }
            
            for (var index = 0; index < _needColdDown.Count; index++)
            {
                var skillType = _needColdDown[index];

                // 未匹配到技能类型
                if (!_skillColdTime.ContainsKey(skillType)) continue;

                // 技能冷却时间大于 0f
                if (_skillColdTime[skillType] > 0f)
                {
                    var t = _skillColdTime[skillType] -= Time.deltaTime;
                    UiManager.Instance.skillBarUi.skillBoxes[skillType.GetIndex()]
                        .SetMaskAmount(t / skillType.coldDownTime);

                    if (!(_skillColdTime[skillType] <= 0f)) continue;

                    _skillColdTime[skillType] = 0f;
                    _needColdDown.Remove(skillType);
                }
                else if (_skillColdTime[skillType] < 0f)
                {
                    _skillColdTime[skillType] = 0f;
                }
            }
        }
    }
}