using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using PixelGameAssets.Scripts.Core;
using PixelGameAssets.Scripts.Entity.InteractableEntity;
using PixelGameAssets.Scripts.Helper;
using PixelGameAssets.Scripts.Misc;
using UnityEngine;
using UnityEngine.Tilemaps;
using Object = System.Object;
using Random = UnityEngine.Random;

namespace PixelGameAssets.Scripts.Map
{
    /// <summary>
    /// 随机地图 带有玩家出生房间
    /// TODO: 待完善
    /// </summary>
    [Obsolete]
    public class TilemapLevelForest : MonoBehaviour
    {
//        public Tilemap bg;
//
//        public Tilemap debugTileMap;
//
//        public Tilemap solidsBack;
//
//        public Tilemap solidsFront;
//
//        public Tilemap pit;
//
//        public Tilemap prefabs;
//
//        public static int Step;
//
//        public static MapBuilder Builder;
//
//        public static MapBuilder PlayerRoomBuilder;
//
//        public static Vector3Int TransferPosPlayerRoom;
//        public static Vector3Int TransferPosMainRoom;
//        public static Vector3Int PlayerPos;
//
//        public static Color32 CameraBg = new Color32(24, 20, 37, 0);
//
//        public static Action After;
//
//        public static List<GameObject> bgObj = new List<GameObject>();
//
//        private void Awake()
//        {
//            TilemapLevelForest.Step = 10000;
//
//            var helper = ResourceLoader.RandomMapTypeDungeon.GetComponent<RandomMapHelper>();
//
//            if (TilemapLevelForest.Builder == null)
//            {
//                TilemapLevelForest.Builder = new MapBuilder(ref helper.wallTile,
//                    ref helper.wallTopTile, ref helper.roadTile, ref helper.obstacle,
//                    ref helper.enemies, ref helper.wallTopRandom, ref helper.wallTileRandom,
//                    ref helper.roadFloorTileRandom);
//            }
//            else
//            {
//                TilemapLevelForest.Builder.ClearAllObj();
//                TilemapLevelForest.Builder.ClearAllVector3();
//            }
//
//            if (TilemapLevelForest.PlayerRoomBuilder == null)
//            {
//                TilemapLevelForest.PlayerRoomBuilder = new MapBuilder(ref helper.wallTile,
//                    ref helper.wallTopTile, ref helper.roadTile, ref helper.obstacle,
//                    ref helper.enemies, ref helper.wallTopRandom, ref helper.wallTileRandom,
//                    ref helper.roadFloorTileRandom);
//            }
//            else
//            {
//                TilemapLevelForest.PlayerRoomBuilder.ClearAllObj();
//                TilemapLevelForest.PlayerRoomBuilder.ClearAllVector3();
//            }
//
//            var task = TilemapLevelForest.Builder.MakeMapData(() =>
//            {
//                var maxV3 = new Vector3Int(0, 0, 0);
//
//                foreach (var v3 in TilemapLevelForest.Builder.WallTop)
//                {
//                    maxV3 = Vector3Int.Max(v3, maxV3);
//                }
//
//                // 偏移
//                maxV3 += new Vector3Int(50, 50, 0);
//
//                var task2 = TilemapLevelForest.PlayerRoomBuilder.MakeMapData(
//                    () =>
//                    {
//                        TilemapLevelForest.TransferPosPlayerRoom = maxV3;
//
//                        foreach (var v3 in TilemapLevelForest.PlayerRoomBuilder.Road.Keys)
//                        {
//                            if (TilemapLevelForest.PlayerRoomBuilder.WallTop.Contains(v3 + Vector3Int.up) ||
//                                TilemapLevelForest.PlayerRoomBuilder.WallTop.Contains(
//                                    v3 + Vector3Int.up + Vector3Int.up) ||
//                                TilemapLevelForest.PlayerRoomBuilder.WallTop.Contains(
//                                    v3 + Vector3Int.left) || TilemapLevelLite.PlayerRoomBuilder.WallTop.Contains(
//                                    v3 + Vector3Int.left + Vector3Int.left) ||
//                                TilemapLevelForest.PlayerRoomBuilder.WallTop.Contains(
//                                    v3 + Vector3Int.down) || TilemapLevelLite.PlayerRoomBuilder.WallTop.Contains(
//                                    v3 + Vector3Int.down + Vector3Int.down) ||
//                                TilemapLevelForest.PlayerRoomBuilder.WallTop.Contains(
//                                    v3 + Vector3Int.right) ||
//                                TilemapLevelForest.PlayerRoomBuilder.WallTop.Contains(
//                                    v3 + Vector3Int.right + Vector3Int.right)
//                            )
//                            {
//                                continue;
//                            }
//
//                            if (TilemapLevelForest.TransferPosPlayerRoom == maxV3)
//                            {
//                                TilemapLevelForest.TransferPosPlayerRoom = v3 * 16;
//                            }
//                            else
//                            {
//                                TilemapLevelForest.PlayerPos = v3 * 16;
//                                break;
//                            }
//                        }
//
//                        foreach (var v3 in TilemapLevelForest.Builder.Road.Keys)
//                        {
//                            if (TilemapLevelForest.Builder.WallTop.Contains(v3 + Vector3Int.up) ||
//                                TilemapLevelForest.Builder.WallTop.Contains(v3 + Vector3Int.up + Vector3Int.up) ||
//                                TilemapLevelForest.Builder.WallTop.Contains(v3 + Vector3Int.left) ||
//                                TilemapLevelForest.Builder.WallTop.Contains(v3 + Vector3Int.left + Vector3Int.left) ||
//                                TilemapLevelForest.Builder.WallTop.Contains(v3 + Vector3Int.down) ||
//                                TilemapLevelForest.Builder.WallTop.Contains(v3 + Vector3Int.down + Vector3Int.down) ||
//                                TilemapLevelForest.Builder.WallTop.Contains(v3 + Vector3Int.right) ||
//                                TilemapLevelForest.Builder.WallTop.Contains(v3 + Vector3Int.right + Vector3Int.right))
//                                continue;
//                            TilemapLevelForest.TransferPosMainRoom = v3 * 16;
//                            break;
//                        }
//                    },
//                    maxV3, 200);
//            }, Vector3Int.zero, TilemapLevelLite.Step);
//
//            MakeObj();
//            UnDraw();
//            Draw();
//        }
//
//        private void Start()
//        {
//            UnityEngine.Camera.main.backgroundColor = CameraBg;
//
//            After?.Invoke();
//
//            GameManager.Instance.commonPlayer.ShowPlayer(PlayerPos);
//
//            Instantiate(ResourceLoader.TransferPosition, new Vector3(TransferPosPlayerRoom.x, TransferPosPlayerRoom.y),
//                Quaternion.Euler(Vector3.zero)).GetComponent<TransferPosition>().target = TransferPosMainRoom;
//
//            Instantiate(ResourceLoader.TransferPosition, new Vector3(TransferPosMainRoom.x, TransferPosMainRoom.y),
//                Quaternion.Euler(Vector3.zero)).GetComponent<TransferPosition>().target = TransferPosPlayerRoom;
//        }
//
//        private void Draw()
//        {
//            // 生成起点
//            bg.SetTile(new Vector3Int(0, 0, 0), Builder.RandomRoad());
//
//            foreach (var v3 in Builder.WallUp)
//            {
//                solidsBack.SetTile(v3, Builder.RandomWall());
//            }
//
//            foreach (var v3 in Builder.WallDown)
//            {
//                solidsBack.SetTile(v3, Builder.RandomWall());
//            }
//
//            foreach (var v3 in Builder.WallTop)
//            {
//                solidsFront.SetTile(v3, Builder.RandomWallTop());
//            }
//
//            foreach (var item in Builder.Road.Keys)
//            {
//                bg.SetTile(item, Builder.RandomRoad());
//            }
//
//            foreach (var v3 in PlayerRoomBuilder.WallUp)
//            {
//                solidsBack.SetTile(v3, PlayerRoomBuilder.RandomWall());
//            }
//
//            foreach (var v3 in PlayerRoomBuilder.WallDown)
//            {
//                solidsBack.SetTile(v3, PlayerRoomBuilder.RandomWall());
//            }
//
//            foreach (var v3 in PlayerRoomBuilder.WallTop)
//            {
//                solidsFront.SetTile(v3, PlayerRoomBuilder.RandomWallTop());
//            }
//
//            foreach (var item in PlayerRoomBuilder.Road.Keys)
//            {
//                bg.SetTile(item, PlayerRoomBuilder.RandomRoad());
//            }
//        }
//
//        private void MakeObj()
//        {
//            foreach (var target in Builder.ObstaclePos)
//            {
//                if (Random.Range(0, 1) != 0) return;
//                Builder.ObstacleObj.Add(Instantiate(Builder.Obstacle[0], target, Quaternion.Euler(Vector3.zero),
//                    prefabs.transform));
//            }
//
//            foreach (var target in Builder.EnemiesPos)
//            {
//                Builder.EnemiesObj.Add(Instantiate(Builder.Enemies[Random.Range(0, Builder.Enemies.Length)], target,
//                    Quaternion.Euler(Vector3.zero), prefabs.transform));
//            }
//        }
//
//        private void UnDraw()
//        {
//            bg.ClearAllTiles();
//            solidsBack.ClearAllTiles();
//            solidsFront.ClearAllTiles();
//            debugTileMap.ClearAllTiles();
//        }
//
//        private void OnDestroy()
//        {
//            PlayerRoomBuilder.ClearAllVector3();
//            Builder.ClearAllVector3();
//            PlayerRoomBuilder.ClearAllObj();
//            Builder.ClearAllObj();
//            UnDraw();
//
//            After = null;
//
//            bgObj.Clear();
//
//            foreach (var o in bgObj)
//            {
//                Destroy(o);
//            }
//        }
    }
}