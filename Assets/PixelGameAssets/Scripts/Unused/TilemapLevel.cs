using System;
using PixelGameAssets.Scripts.Core;
using PixelGameAssets.Scripts.Map;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace PixelGameAssets.Scripts.Unused
{
    /// <summary>
    /// 随机地图
    /// TODO: 待完善
    
    /// </summary>
    [Obsolete("Use TilemapLevelLite")]
    public class TilemapLevel : MonoBehaviour
    {
        public Tilemap bg;

        public Tilemap debugTileMap;

        public Tilemap solidsBack;

        public Tilemap solidsFront;

        public Tilemap pit;

        public Tilemap prefabs;

        public static int Step;

        public static MapBuilder Builder;

        private void Awake()
        {
            Builder.ClearAllObj();
            MakeObj();
            UnDraw();
            Draw();
        }

        private void Start()
        {
            GameManager.Instance.Player.ShowPlayer(Builder.StartPos);
        }

        private void Draw()
        {
            // 生成起点
            bg.SetTile(new Vector3Int(0, 0, 0), Builder.RandomRoad());

            foreach (var v3 in Builder.WallUp)
            {
                solidsBack.SetTile(v3, Builder.RandomWall());
            }

            foreach (var v3 in Builder.WallDown)
            {
                solidsBack.SetTile(v3, Builder.RandomWall());
            }

            foreach (var v3 in Builder.WallTop)
            {
                solidsFront.SetTile(v3, Builder.RandomWallTop());
            }

            foreach (var item in Builder.Road.Keys)
            {
                bg.SetTile(item, Builder.RandomRoad());
            }
        }

        private void MakeObj()
        {
            foreach (var target in Builder.ObstaclePos)
            {
                if (Random.Range(0, 1) != 0) return;
                Builder.ObstacleObj.Add(Instantiate(Builder.Obstacle[0], target, Quaternion.Euler(Vector3.zero),
                    prefabs.transform));
            }

            foreach (var target in Builder.EnemiesPos)
            {
                Builder.EnemiesObj.Add(Instantiate(Builder.Enemies[Random.Range(0, Builder.Enemies.Length)], target,
                    Quaternion.Euler(Vector3.zero), prefabs.transform));
            }
            
            foreach (var target in Builder.InteractableObjPos)
            {
                Builder.InteractableObj.Add(Instantiate(Builder.Interactable[Random.Range(0, Builder.Interactable.Length)], target,
                    Quaternion.Euler(Vector3.zero), prefabs.transform));
            }
        }

//        /// <summary>
//        /// 建立玩家初始房间
//        /// </summary>
//        public static void BuildPlayerRoom()
//        {
//            var maxV3 = new Vector3Int(0, 0, 0);
//
//            foreach (var v3 in WallTop)
//            {
//                maxV3 = Vector3Int.Max(v3, maxV3);
//            }
//
//            maxV3 *= 16;
//
//            maxV3 += new Vector3Int(500, 500, 0);
//
//            var task = MakeMap(() => { GameManager.Instance.commonPlayer.ShowPlayer(maxV3); }, true, maxV3, 100);
//        }

        private void UnDraw()
        {
            bg.ClearAllTiles();
            solidsBack.ClearAllTiles();
            solidsFront.ClearAllTiles();
            debugTileMap.ClearAllTiles();
        }

        private void OnDestroy()
        {
            Debug.Log("OnDestroy");
            Builder.ClearAllVector3();
            Builder.ClearAllObj();
            UnDraw();
        }
    }
}