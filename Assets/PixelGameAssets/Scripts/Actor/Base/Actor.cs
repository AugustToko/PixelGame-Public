using PixelGameAssets.Scripts.Core;
using PixelGameAssets.Scripts.EventManager;
using UnityEngine;

namespace PixelGameAssets.Scripts.Actor.Base
{
    /// <summary>
    /// BaseActor
    /// </summary>
    public abstract class Actor : AttackAbleActor
    {
        /// <summary>
        /// Actor 阵营, 囊括所有
        /// </summary>
        // TODO ActorCamp 阵营待完善
        public enum ActorCamp
        {
            Player,
            Neutral,
            HostileForces,
            None,
            Unknown
        }

        /// <summary>
        /// Actor 类型, 囊括所有
        /// </summary>
        // TODO Actor 类型待完善
        public enum ActorType
        {
            Player,
            Enemies,
            NormalNpc,
            Unknown
        }

        public string actorName = "Actor Name";

        /// <summary>
        /// Actor 类型
        /// </summary>
        [Header("Standpoint")] // TODO Actor 立场待完善
        public ActorType actorTypeDebug = ActorType.Unknown;

        public ActorType actorType = ActorType.Unknown;

        public ActorCamp actorCamp = ActorCamp.Unknown;

        // 允许攻击
        [Header("Property")] public bool allowAttack = true;

        /// <summary>
        /// 用来标识该角色是否可以移动
        /// 常用于死亡，冻结角色
        /// </summary>
        public bool allowMove = true;

        /// <summary>
        /// 角色朝向
        /// </summary>
        [Header("Facing Direction")] public Facings Facing = Facings.Right;

        /// <summary>
        /// 设置 <see cref="Actor" /> 类型
        /// </summary>
        /// <param name="type">Actor 类型</param>
        protected void SetUpType(ActorType type)
        {
            actorTypeDebug = type;
            actorType = type;
        }

        /// <summary>
        /// 加血
        /// </summary>
        public abstract void SetTakeHealth();

        /// <summary>
        /// 伤害
        /// </summary>
        public abstract void SetTakeHit(int amount);

        /// <summary>
        /// 死亡回调
        /// </summary>
        public virtual void Die()
        {
            EventCenter.Broadcast(EventManager.EventType.UpdateSystemEvent, name + " died");
            Destroy(gameObject);
        }
    }
}