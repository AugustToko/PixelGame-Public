using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace PixelGameAssets.Scripts.Map
{
    /// <summary>
    /// 随机地图建造者, 生产数值
    /// </summary>
    public class MapBuilder : Object
    {
        private readonly TileBase _wallTile;

        private readonly TileBase _wallTopTile;

        private readonly TileBase _roadTile;

        public readonly TileBase DebugTile;

        private readonly TileBase[] _wallTopRandom;

        private readonly TileBase[] _wallTileRandom;

        private readonly TileBase[] _roadFloorTileRandom;

        /// <summary>
        /// 无交互障碍物
        /// </summary>
        public readonly GameObject[] Obstacle;

        public readonly GameObject[] Enemies;

        public readonly GameObject[] Interactable;

        public Vector3Int StartPos;

        /// <summary>
        /// 地图规模
        /// </summary>
        private int _steps;

        #region POSITION (VECTOR3INT)

        /// <summary>
        /// 生成路的坐标
        /// </summary>
        public readonly Dictionary<Vector3Int, int> Road = new Dictionary<Vector3Int, int>();

        /// <summary>
        /// 墙顶
        /// </summary>
        public readonly HashSet<Vector3Int> WallTop = new HashSet<Vector3Int>();

        /// <summary>
        /// 墙壁上
        /// </summary>
        public readonly HashSet<Vector3Int> WallUp = new HashSet<Vector3Int>();

        /// <summary>
        /// 墙壁下
        /// </summary>
        public readonly HashSet<Vector3Int> WallDown = new HashSet<Vector3Int>();

        public readonly HashSet<Vector3Int> WallAll = new HashSet<Vector3Int>();

        /// <summary>
        /// 修复道路的坐标
        /// </summary>
        public readonly HashSet<Vector3Int> RoadFixed = new HashSet<Vector3Int>();

        /// <summary>
        /// 敌人坐标
        /// </summary>
        public readonly HashSet<Vector3Int> EnemiesPos = new HashSet<Vector3Int>();

        /// <summary>
        /// 障碍物坐标
        /// </summary>
        public readonly HashSet<Vector3Int> ObstaclePos = new HashSet<Vector3Int>();

        /// <summary>
        /// 可交互物品的坐标
        /// </summary>
        public readonly HashSet<Vector3Int> InteractableObjPos = new HashSet<Vector3Int>();

        #endregion

        // END POSITION (VECTOR3INT)

        /// <summary>
        /// 所有已用位置
        /// </summary>
        public readonly HashSet<Vector3Int> AllPos = new HashSet<Vector3Int>();

        /// <summary>
        /// 所有可利用空间
        /// </summary>
        public readonly List<Vector3Int> AvailableSpace = new List<Vector3Int>();

        /// <summary>
        /// 可交互物品的列表
        /// </summary>
        public readonly List<GameObject> InteractableObj = new List<GameObject>();

        /// <summary>
        /// 障碍物列表
        /// </summary>
        public readonly List<GameObject> ObstacleObj = new List<GameObject>();

        /// <summary>
        /// 敌人列表(All)
        /// </summary>
        public readonly List<GameObject> EnemiesObj = new List<GameObject>();

        private int _wallTimes = 0;
        private int _wallTopTimes = 0;
        private int _roadTimes = 0;
        private int _enemiesTimes = 0;

        #region Build

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="wallTile">墙壁（常规）</param>
        /// <param name="wallTopTile">墙顶（常规）</param>
        /// <param name="roadTile">路面（常规）</param>
        /// <param name="obstacle">障碍</param>
        /// <param name="enemies">敌人</param>
        /// <param name="interactable">可交互物体</param>
        /// <param name="wallTopRandom">在常规墙顶上添加随机的不同样式的墙顶</param>
        /// <param name="wallTileRandom">在常规墙壁上添加随机的不同样式的墙壁</param>
        /// <param name="roadFloorTileRandom">在常规地面上添加随机的不同样式的地面</param>
        /// <param name="debugTileTile">DEBUG</param>
        public MapBuilder(ref TileBase wallTile, ref TileBase wallTopTile, ref TileBase roadTile,
            ref GameObject[] obstacle,
            ref GameObject[] enemies, ref GameObject[] interactable, ref TileBase[] wallTopRandom,
            ref TileBase[] wallTileRandom,
            ref TileBase[] roadFloorTileRandom, ref TileBase debugTileTile)
        {
            _wallTile = wallTile;
            _wallTopTile = wallTopTile;
            _roadTile = roadTile;
            Obstacle = obstacle;
            Enemies = enemies;
            _wallTopRandom = wallTopRandom;
            _wallTileRandom = wallTileRandom;
            _roadFloorTileRandom = roadFloorTileRandom;
            this.Interactable = interactable;
            DebugTile = debugTileTile;
        }

        /// <summary>
        /// 初始化Builder, 初始化地图
        /// </summary>
        /// <param name="helper">资源来源</param>
        /// <param name="builder">建造者</param>
        public static void InitBuilder(ref RandomMapHelper helper, [NotNull] ref MapBuilder builder)
        {
            builder = new MapBuilder(ref helper.wallTile,
                ref helper.wallTopTile, ref helper.roadTile, ref helper.obstacle,
                ref helper.enemies, ref helper.interactableObj, ref helper.wallTopRandom, ref helper.wallTileRandom,
                ref helper.roadFloorTileRandom, ref helper.debug);
        }

        /// <summary>
        /// 随机障碍物
        /// </summary>
        private void RandomObstacle()
        {
            var random = new System.Random();

            for (var i = 0; i < _steps / 40; i++)
            {
                var pos = AvailableSpace[random.Next(0, AvailableSpace.Count)];

                if (!AllPos.Contains(pos))
                {
                    ObstaclePos.Add(pos);
                }
            }
        }

        /// <summary>
        /// 随机障碍物
        /// </summary>
        private void RandomInteractable()
        {
            var random = new System.Random(DateTime.Now.Millisecond);

            for (var i = 0; i < _steps / 2000; i++)
            {
                var pos = AvailableSpace[random.Next(0, AvailableSpace.Count)];

                if (!AllPos.Contains(pos))
                {
                    InteractableObjPos.Add(pos);
                }
            }
        }

        private void GenerateEnemiesPos()
        {
            var random = new System.Random(DateTime.Now.Millisecond);

            for (var i = 0; i < _steps / 100; i++)
            {
                var pos = AvailableSpace[random.Next(0, AvailableSpace.Count)];

                if (!AllPos.Contains(pos))
                {
                    EnemiesPos.Add(pos);
                }
            }
        }

        private void BuildRoad(int steps, Vector3Int startPos)
        {
            var nowPoint = startPos;

            // 生成路径
            for (; Road.Count < steps;)
            {
                if (!Road.ContainsKey(nowPoint))
                {
                    Road.Add(nowPoint, 0);
                }

                nowPoint += RandomRoadPoint();
            }
        }

        private void BuildWall_Up()
        {
            foreach (var item in Road.Keys)
            {
                var target = item + Vector3Int.up;

                if (!Road.ContainsKey(target) && !Road.ContainsKey(target + Vector3Int.up))
                {
                    WallUp.Add(target);
                }
            }
        }

        private void BuildWallTop_Up()
        {
            foreach (var item in WallUp)
            {
                var target = item + Vector3Int.up;
                WallTop.Add(target);

                if (WallUp.Contains(target + Vector3Int.left) || WallUp.Contains(target + Vector3Int.right))
                {
                    WallTop.Add(target + Vector3Int.up);
                }
            }
        }

        /// <summary>
        /// 水平建造 walltop
        /// </summary>
        private void BuildWallTop_H()
        {
            foreach (var item in Road.Keys)
            {
                var target = item + Vector3Int.right;

                if (!WallTop.Contains(target)
                    && !Road.ContainsKey(target)
                    && !WallUp.Contains(target)
                )
                {
                    WallTop.Add(target);
                }
            }

            foreach (var item in Road.Keys)
            {
                var target2 = item + Vector3Int.left;

                if (!WallTop.Contains(target2)
                    && !Road.ContainsKey(target2)
                    && !WallUp.Contains(target2)
                )

                {
                    WallTop.Add(target2);
//                    solidsFront.SetTile(target2, RandomWallTop());
                }
            }
        }

        /// <summary>
        /// 修复 wallTop_down 拐角
        /// </summary>
        private void FixWallTop_Down()
        {
            var hashSet = new HashSet<Vector3Int>();

            foreach (var v3 in WallTop)
            {
                var target = v3 + Vector3Int.down;

                if (!WallUp.Contains(target) && WallTop.Contains(target + Vector3Int.left))
                {
                    hashSet.Add(target);
                }

                if (!WallUp.Contains(target) && WallTop.Contains(target + Vector3Int.right))
                {
                    hashSet.Add(target);
                }
            }

            foreach (var v3 in hashSet)
            {
                WallTop.Add(v3);
//                solidsFront.SetTile(v3, RandomWallTop());
            }
        }

        /// <summary>
        /// 全局检测 wallTop 修复空隙，拐角
        /// </summary>
        private void FixWallTop1()
        {
            var l1 = new List<Vector3Int>();

            foreach (var v3 in WallTop)
            {
                if (!WallUp.Contains(v3 + Vector3Int.down) &&
                    !Road.ContainsKey(v3 + Vector3Int.down) &&
                    (WallTop.Contains(v3 + Vector3Int.down + Vector3Int.left) ||
                     WallTop.Contains(v3 + Vector3Int.down + Vector3Int.right)))
                {
                    l1.Add(v3 + Vector3Int.down);
                }

                if (!WallUp.Contains(v3 + Vector3Int.up) &&
                    !Road.ContainsKey(v3 + Vector3Int.up) &&
                    (WallTop.Contains(v3 + Vector3Int.up + Vector3Int.left) ||
                     WallTop.Contains(v3 + Vector3Int.up + Vector3Int.right)))
                {
                    l1.Add(v3 + Vector3Int.up);
                }
            }

            var l2 = new List<Vector3Int>();

            foreach (var target in from v3 in WallTop
                let target = v3 + Vector3Int.up
                where !Road.ContainsKey(v3) && (WallUp.Contains(target + Vector3Int.right) ||
                                                WallUp.Contains(target + Vector3Int.left))
                select target)
            {
                l2.Add(target);
                if (!Road.ContainsKey(target + Vector3Int.up))
                {
                    l2.Add(target + Vector3Int.up);
                }
            }

            var l3 = new List<Vector3Int>();

            foreach (var v3 in WallTop)
            {
                var target = v3 + Vector3Int.up + Vector3Int.up;

                var target2 = v3 + Vector3Int.up + Vector3Int.up;

                if (!Road.ContainsKey(target) && !WallTop.Contains(target) && !WallUp.Contains(target) &&
                    WallTop.Contains(target + Vector3Int.up))
                {
                    l3.Add(v3 + Vector3Int.up);
//                    solidsFront.SetTile(v3 + Vector3Int.up, RandomWallTop());
                }

                if (!Road.ContainsKey(target2) && !WallTop.Contains(target2) && !WallUp.Contains(target2) &&
                    WallTop.Contains(target2 + Vector3Int.up))
                {
                    l3.Add(v3 + Vector3Int.up + Vector3Int.up);
//                    solidsFront.SetTile(v3 + Vector3Int.up + Vector3Int.up, RandomWallTop());
                }
            }

            l1.AddRange(l2);
            l1.AddRange(l3);

            foreach (var v3 in l1)
            {
                WallTop.Add(v3);
            }
        }

        private void BuildWallTop_Down()
        {
            foreach (var target in Road.Keys.Select(item => item + Vector3Int.down).Where(target =>
                !Road.ContainsKey(target)
                && !WallTop.Contains(target)
                && !Road.ContainsKey(target + Vector3Int.down)))
            {
                WallTop.Add(target);
//                    solidsFront.SetTile(target, RandomWallTop());
            }
        }

        private void BuildWall_Down()
        {
            foreach (var target in WallTop.Select(item => item + Vector3Int.down).Where(target =>
                !WallTop.Contains(target)
                && !WallUp.Contains(target)
                && !Road.ContainsKey(target)))
            {
                WallDown.Add(target);
            }
        }

        /// <summary>
        /// 填充陆地空隙
        /// </summary>
        private void FixRoad()
        {
            foreach (var up in Road.Keys.Select(item => item + Vector3Int.up)
                .Where(up => !Road.ContainsKey(up) && Road.ContainsKey(up + Vector3Int.up)))
            {
                RoadFixed.Add(up);
            }

            foreach (var v3 in RoadFixed)
            {
                Road.Add(v3, 0);
            }
        }


        private static Vector3Int RandomRoadPoint()
        {
            switch (Random.Range(0, 4))
            {
                case 0: return new Vector3Int(-1, 0, 0);
                case 1: return new Vector3Int(1, 0, 0);
                case 2: return new Vector3Int(0, -1, 0);
                default:
                    return new Vector3Int(0, 1, 0);
            }
        }

        public TileBase RandomRoad()
        {
            _roadTimes++;

            return _roadTimes % 40 == 0
                ? _roadFloorTileRandom[Random.Range(0, _roadFloorTileRandom.Length)]
                : _roadTile;
        }

        public TileBase RandomWall()
        {
            _wallTimes++;

            return _wallTimes % 5 == 0 ? _wallTileRandom[Random.Range(0, _wallTileRandom.Length)] : _wallTile;
        }

        public TileBase RandomWallTop()
        {
            _wallTopTimes++;

            return _wallTopTimes % 20 == 0 ? _wallTopRandom[Random.Range(0, _wallTopRandom.Length)] : _wallTopTile;
        }

        private Task<string> Build()
        {
            var task = Task.Run(() =>
            {
                FixRoad();

                BuildWall_Up();

                BuildWallTop_Up();

                BuildWallTop_Down();

                FixWallTop_Down();

                BuildWallTop_H();

                FixWallTop1();

                BuildWall_Down();

                // 整理 Wall
                foreach (var vector3Int in WallUp)
                {
                    WallAll.Add(vector3Int);
                }

                foreach (var vector3Int in WallDown)
                {
                    WallAll.Add(vector3Int);
                }

                // 整理可用位置
                foreach (var item in Road.Keys)
                {
                    if (
                        // wallTop
                        !WallTop.Contains(item + Vector3Int.up) &&
                        !WallTop.Contains(item + Vector3Int.left) &&
                        !WallTop.Contains(item + Vector3Int.down) &&
                        !WallTop.Contains(item + Vector3Int.right) &&
                        // conner
                        !WallTop.Contains(item + Vector3Int.up + Vector3Int.right) &&
                        !WallTop.Contains(item + Vector3Int.up + Vector3Int.left) &&
                        !WallTop.Contains(item + Vector3Int.down + Vector3Int.left) &&
                        !WallTop.Contains(item + Vector3Int.down + Vector3Int.right) &&
                        // wall
                        !WallAll.Contains(item + Vector3Int.up) &&
                        !WallAll.Contains(item + Vector3Int.left) &&
                        !WallAll.Contains(item + Vector3Int.down) &&
                        !WallAll.Contains(item + Vector3Int.right) &&
                        // conner
                        !WallAll.Contains(item + Vector3Int.up + Vector3Int.right) &&
                        !WallAll.Contains(item + Vector3Int.up + Vector3Int.left) &&
                        !WallAll.Contains(item + Vector3Int.down + Vector3Int.left) &&
                        !WallAll.Contains(item + Vector3Int.down + Vector3Int.right)
                    )
                    {
                        AvailableSpace.Add(item);
                    }
                }

                // Done 1

                RandomObstacle();

                foreach (var i in ObstaclePos)
                {
                    AllPos.Add(i);
                }

                GenerateEnemiesPos();

                foreach (var i in EnemiesPos)
                {
                    AllPos.Add(i);
                }

                RandomInteractable();

                foreach (var i in InteractableObjPos)
                {
                    AllPos.Add(i);
                }

                return "done";
            });
            return task;
        }

        #endregion

        /// <summary>
        /// 制作地图
        /// </summary>
        /// <param name="a">建造地图完成后的操作(主线程)</param>
        /// <param name="startPosition">地图起始点</param>
        /// <param name="steps">地图规模</param>
        /// <returns>Task</returns>
        public async Task MakeMapData(Action a, Vector3Int startPosition, int steps)
        {
            StartPos = startPosition;
            _steps = steps;

            // Random.Range 必须在主线程工作...
            BuildRoad(steps <= 20 ? 100 : steps, startPosition);

            var task = await Build();

            a?.Invoke();
        }

        /// <summary>
        /// 获取传送门位置
        /// TODO: 随机化 <see cref="TilemapLevelLite.Builder.Road.Keys"/> 以便在地图上更随机的放置传送门
        /// </summary>
        /// <returns>传送门位置</returns>
        [Obsolete("Use GetRandomPosOnRoad")]
        public Vector3Int GetTransferPos()
        {
            return GetRandomPosOnRoad();
        }

        /// <summary>
        /// 在 Road 上任取一点
        /// </summary>
        /// <returns>点坐标</returns>
        public Vector3Int GetRandomPosOnRoad()
        {
            while (true)
            {
                var pos = AvailableSpace[Random.Range(0, AvailableSpace.Count)] * 16;
                if (AllPos.Contains(pos)) continue;

                AllPos.Add(pos);
                return pos;
            }
        }

        public void ClearAllObj()
        {
            ObstacleObj.Clear();
            EnemiesObj.Clear();
            InteractableObj.Clear();

            foreach (var o in ObstacleObj)
            {
                Destroy(o);
            }

            foreach (var o in EnemiesObj)
            {
                Destroy(o);
            }

            foreach (var o in InteractableObj)
            {
                Destroy(o);
            }
        }

        public void ClearAllVector3()
        {
            WallTop.Clear();
            WallUp.Clear();
            WallDown.Clear();
            Road.Clear();
            RoadFixed.Clear();

            EnemiesPos.Clear();
            ObstaclePos.Clear();

            InteractableObjPos.Clear();

            _steps = 0;
            StartPos = Vector3Int.zero;
        }
    }
}