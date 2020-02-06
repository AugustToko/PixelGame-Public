using System;
using System.Collections.Generic;
using PixelGameAssets.Scripts.EventManager;
using PixelGameAssets.Scripts.SkillSystem;
using PixelGameAssets.Scripts.UI.UICanvas;
using PixelGameAssets.Scripts.WeaponSystem.WeaponInfo;

namespace PixelGameAssets.Scripts.Core
{
    /// <summary>
    /// 玩家数据类
    /// </summary>
    [Serializable]
    public class PlayerInfo
    {
        /// <summary>
        /// 玩家昵称
        /// </summary>
        public string playerName = "Player";

        /// <summary>
        /// 当前装备的武器
        /// </summary>
        public List<WeaponData> weapons;

        /// <summary>
        /// 经验值
        /// </summary>
        public double exp;

        /// <summary>
        /// 等级
        /// </summary>
        public int level;

        /// <summary>
        /// 分数
        /// </summary>
        public int score;

        /// <summary>
        /// 硬币
        /// </summary>
        private int Coin { get; set; }

        public double spendTime;

        /// <summary>
        /// 技能包 (当前装备的武器)
        /// </summary>
        public SkillPack skillPack;

        public PlayerInfo(string playerName, List<WeaponData> weapon, SkillPack skillPack)
        {
            this.playerName = playerName;
            weapons = weapon;
            this.skillPack = skillPack;
        }

        /// <summary>
        /// 操作硬币
        /// </summary>
        /// <param name="amount">值</param>
        /// <param name="add">是否为增加</param>
        /// <returns>是否成功</returns>
        public bool OperateCoin(int amount, bool add = true)
        {
            var result = false;
            if (add)
            {
                Coin += amount;
                result = true;
            }
            else
            {
                if (amount <= Coin)
                {
                    Coin -= amount;
                    result = true;
                }
            }

            EventCenter.Broadcast(EventType.UpdateCoin, Coin);
            return result;
        }

        public override string ToString()
        {
            var data1 = "";

            weapons.ForEach(data => { data1 += data.ToString(); });
            return "exp: " + exp + ", Skill: " + skillPack + "WEAPON: " + data1;
        }
    }
}