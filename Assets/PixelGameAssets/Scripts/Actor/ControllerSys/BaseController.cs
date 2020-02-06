using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace PixelGameAssets.Scripts.Actor.ControllerSys
{
    /// <summary>
    /// Controller 基类
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public abstract class BaseController : Base.Actor
    {
        /// <summary>
        /// 可推动物体所在层 (参考 推箱子)
        /// </summary>
        [SerializeField] private LayerMask pushBlockLayer;
        
        [SerializeField] [Header("Collider")] protected Collider2D myCollider; // 缓存对撞机（仅使用Collider2D）

        /// <summary>
        /// 检查给定图层中 Actor 的顶部是否存在碰撞
        /// </summary>
        /// <param name="layer">给定的图层</param>
        /// <returns></returns>
        [SuppressMessage("ReSharper", "Unity.InefficientPropertyAccess")]
        public bool CollisionSelf(LayerMask layer)
        {
            var leftCorner = new Vector2(myCollider.bounds.center.x - myCollider.bounds.extents.x + .1f,
                myCollider.bounds.center.y + myCollider.bounds.extents.y - .1f);
            var rightCorner = new Vector2(myCollider.bounds.center.x + myCollider.bounds.extents.x - .1f,
                myCollider.bounds.center.y - myCollider.bounds.extents.y + .1f);
            return Physics2D.OverlapArea(leftCorner, rightCorner, layer);
        }

        /// <summary>
        /// 辅助函数，用于检查给定图层中是否存在任何具有额外设置位置的碰撞
        /// </summary>
        /// <param name="extraPos"></param>
        /// <param name="layer"></param>
        /// <returns></returns>
        [SuppressMessage("ReSharper", "Unity.InefficientPropertyAccess")]
        public bool CheckColAtPlace(Vector2 extraPos, LayerMask layer)
        {
            var leftCorner = new Vector2(myCollider.bounds.center.x - myCollider.bounds.extents.x + .1f,
                                     myCollider.bounds.center.y + myCollider.bounds.extents.y - .1f) + extraPos;
            var rightCorner = new Vector2(myCollider.bounds.center.x + myCollider.bounds.extents.x - .1f,
                                      myCollider.bounds.center.y - myCollider.bounds.extents.y + .1f) + extraPos;

            return Physics2D.OverlapArea(leftCorner, rightCorner, layer);
        }
    }
}