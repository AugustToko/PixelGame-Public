using UnityEngine;

namespace PixelGameAssets.Scripts.Actor.ControllerSys
{
    /// <summary>
    /// 敌人 (NPC) 控制器
    /// </summary>
    public abstract class EnemiesController : BaseController
    {
        /// <summary>
        /// One Way / Fall Through 平台所在的层
        /// </summary>
        public LayerMask entities_layer;

        public Vector2 movementCounter = Vector2.zero;

//        水平加速度
//        public float runAccel = 1000f;

        /// <summary>
        /// 变量来存储 Actor 是否在当前帧的地面上
        /// </summary>
        [Header("Variabless")] public bool onGround = true;

        /// <summary>
        /// 放置固体的层
        /// </summary>
        [Header("Collision Layers")] public LayerMask solid_layer;

        /// <summary>
        /// Actor 的速度
        /// </summary>
        [Header("Speed & SubPixel Movement Counter")]
        public Vector2 Speed;

        /// <summary>
        /// 与 <see cref="onGround"/> 相同但在当前帧之前的帧
        /// </summary>
        public bool wasOnGround = true;

        protected virtual void Awake()
        {
            // 如果没有手动分配对撞机，我们将尝试获取此对象的collider2d组件
            if (myCollider != null) return;

            myCollider = GetComponent<Collider2D>();
            if (myCollider == null) Debug.Log("This Actor has no Collider2D component");
        }

        /// <summary>
        /// 用于水平移动Actor的函数，这仅存储运动的浮点值以允许子像素移动并调用MoveHExact函数来执行实际移动
        /// </summary>
        /// <param name="moveH">val</param>
        /// <returns>是否完成</returns>
        public bool MoveH(float moveH)
        {
            if (!allowMove) return false;
            movementCounter.x += moveH;
            var num = (int) Mathf.Round(movementCounter.x);
            if (num == 0) return false;
            movementCounter.x -= num;
            return MoveHExact(num);
        }

        /// <summary>
        /// 用于垂直移动Actor的函数，这只存储运动的浮点值以允许子像素移动并调用MoveVExact函数来执行实际移动的bool MoveV（float moveV）
        /// </summary>
        /// <param name="moveV">val</param>
        /// <returns>是否完成</returns>
        public bool MoveV(float moveV)
        {
            if (!allowMove) return false;

            movementCounter.y += moveV;
            var num = (int) Mathf.Round(movementCounter.y);
            if (num != 0)
            {
                movementCounter.y -= num;
                return MoveVExact(num);
            }

            return false;
        }

        /// <summary>
        /// 将Actor水平移动一个精确整数的函数
        /// </summary>
        /// <param name="moveH">val</param>
        /// <returns>是否完成</returns>
        public bool MoveHExact(int moveH)
        {
            var num = (int) Mathf.Sign(moveH);
            while (moveH != 0)
            {
                var solid = CheckColInDir(Vector2.right * num, solid_layer);
                if (solid)
                {
                    movementCounter.x = 0f;
                    return true;
                }

                moveH -= num;
                var position = transform.position;
                position = new Vector2(position.x + num, position.y);

                transform.position = position;
            }

            return false;
        }

        // 将Actor垂直移动一个精确整数的函数
        public bool MoveVExact(int moveV)
        {
            var num = (int) Mathf.Sign(moveV);

            while (moveV != 0)
            {
                var solid = num > 0 ? CheckColInDir(Vector2.up * num, solid_layer) : OnGround();
                if (solid)
                {
                    movementCounter.y = 0f;
                    return true;
                }

                moveV -= num;
                var position = transform.position;
                position = new Vector2(position.x, position.y + num);

                transform.position = position;
            }

            return false;
        }

        // 检查 Actor 是否接地
        public bool OnGround()
        {
            return CheckColInDir(Vector2.down, solid_layer);
        }

        public bool CheckColInDir(Vector2 dir, LayerMask layer)
        {
            var leftcorner = Vector2.zero;
            var rightcorner = Vector2.zero;

            if (dir.x > 0)
            {
                leftcorner = new Vector2(myCollider.bounds.center.x + myCollider.bounds.extents.x,
                    myCollider.bounds.center.y + myCollider.bounds.extents.y - .1f);
                rightcorner = new Vector2(myCollider.bounds.center.x + myCollider.bounds.extents.x + .5f,
                    myCollider.bounds.center.y - myCollider.bounds.extents.y + .1f);
            }
            else if (dir.x < 0)
            {
                leftcorner = new Vector2(myCollider.bounds.center.x - myCollider.bounds.extents.x - .5f,
                    myCollider.bounds.center.y + myCollider.bounds.extents.y - .1f);
                rightcorner = new Vector2(myCollider.bounds.center.x - myCollider.bounds.extents.x,
                    myCollider.bounds.center.y - myCollider.bounds.extents.y + .1f);
            }
            else if (dir.y > 0)
            {
                leftcorner = new Vector2(myCollider.bounds.center.x - myCollider.bounds.extents.x + .1f,
                    myCollider.bounds.center.y + myCollider.bounds.extents.y + .5f);
                rightcorner = new Vector2(myCollider.bounds.center.x + myCollider.bounds.extents.x - .1f,
                    myCollider.bounds.center.y + myCollider.bounds.extents.y);
            }
            else if (dir.y < 0)
            {
                leftcorner = new Vector2(myCollider.bounds.center.x - myCollider.bounds.extents.x + .1f,
                    myCollider.bounds.center.y - myCollider.bounds.extents.y);
                rightcorner = new Vector2(myCollider.bounds.center.x + myCollider.bounds.extents.x - .1f,
                    myCollider.bounds.center.y - myCollider.bounds.extents.y - .5f);
            }

            return Physics2D.OverlapArea(leftcorner, rightcorner, layer);
        }

        public Collider2D[] CheckColsInDirAll(Vector2 dir, LayerMask layer)
        {
            var leftCorner = Vector2.zero;
            var rightCorner = Vector2.zero;

            if (dir.x > 0)
            {
                leftCorner = new Vector2(myCollider.bounds.center.x + myCollider.bounds.extents.x,
                    myCollider.bounds.center.y + myCollider.bounds.extents.y - .1f);
                rightCorner = new Vector2(myCollider.bounds.center.x + myCollider.bounds.extents.x + .5f,
                    myCollider.bounds.center.y - myCollider.bounds.extents.y + .1f);
            }
            else if (dir.x < 0)
            {
                leftCorner = new Vector2(myCollider.bounds.center.x - myCollider.bounds.extents.x - .5f,
                    myCollider.bounds.center.y + myCollider.bounds.extents.y - .1f);
                rightCorner = new Vector2(myCollider.bounds.center.x - myCollider.bounds.extents.x,
                    myCollider.bounds.center.y - myCollider.bounds.extents.y + .1f);
            }
            else if (dir.y > 0)
            {
                leftCorner = new Vector2(myCollider.bounds.center.x - myCollider.bounds.extents.x + .1f,
                    myCollider.bounds.center.y + myCollider.bounds.extents.y + .5f);
                rightCorner = new Vector2(myCollider.bounds.center.x + myCollider.bounds.extents.x - .1f,
                    myCollider.bounds.center.y + myCollider.bounds.extents.y);
            }
            else if (dir.y < 0)
            {
                leftCorner = new Vector2(myCollider.bounds.center.x - myCollider.bounds.extents.x + .1f,
                    myCollider.bounds.center.y - myCollider.bounds.extents.y);
                rightCorner = new Vector2(myCollider.bounds.center.x + myCollider.bounds.extents.x - .1f,
                    myCollider.bounds.center.y - myCollider.bounds.extents.y - .5f);
            }

            return Physics2D.OverlapAreaAll(leftCorner, rightCorner, layer);
        }
    }
}