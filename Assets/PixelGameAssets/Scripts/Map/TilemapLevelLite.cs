using System;
using System.Collections.Generic;
using PixelGameAssets.Scripts.Core;
using PixelGameAssets.Scripts.Entity.InteractableEntity;
using PixelGameAssets.Scripts.Misc;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace PixelGameAssets.Scripts.Map
{
    /// <summary>
    /// 随机地图
    /// 带有玩家出身房间和主房间
    /// TODO: 待完善
    /// </summary>
    public class TilemapLevelLite : MonoBehaviour
    {
        public Tilemap bg;

        public Tilemap debugTileMap;

        public Tilemap solidsBack;

        /// <summary>
        /// 为了修复tilemap碰撞 再叠一层
        /// </summary>
        public Tilemap solidsBackColl;

        public Tilemap solidsFront;

        /// <summary>
        /// 为了修复tilemap碰撞 再叠一层
        /// </summary>
        public Tilemap solidsFrontColl;

        public Tilemap solidsFrontColl2;

        public Tilemap pit;

        public Tilemap prefabs;

        public static MapBuilder Builder;

        public static MapBuilder PlayerRoomBuilder;

        public static Vector3Int TransferPosPlayerRoom;
        public static Vector3Int TransferPosMainRoom;
        public static Vector3Int PlayerPos;

        /// <summary>
        /// 摄像机背景
        /// </summary>
        public readonly Color32 DefCameraBg = new Color32(24, 20, 37, 0);

        public static Color32 CameraBg = new Color32(24, 20, 37, 0);

        public static Action After;

        /// <summary>
        /// 背景
        /// </summary>
        public static List<GameObject> bgObj = new List<GameObject>();

        /// <summary>
        /// 传送门(玩家)
        /// </summary>
        private GameObject _transferPosPlayerRoom;

        /// <summary>
        /// 主房间(玩家)
        /// </summary>
        private GameObject _transferPosMainRoom;

        private void Awake()
        {
            MakeObj();
            UnDraw();
            Draw();
        }

        private void Start()
        {
            After?.Invoke();

            GameManager.Instance.Player.ShowPlayer(PlayerPos);

            _transferPosPlayerRoom = Instantiate(ResourceLoader.TransferPosition,
                new Vector3(TransferPosPlayerRoom.x, TransferPosPlayerRoom.y),
                Quaternion.Euler(Vector3.zero));
            _transferPosPlayerRoom.GetComponent<TransferPosition>().target = TransferPosMainRoom;

            _transferPosMainRoom = Instantiate(ResourceLoader.TransferPosition,
                new Vector3(TransferPosMainRoom.x, TransferPosMainRoom.y),
                Quaternion.Euler(Vector3.zero));

            _transferPosMainRoom.GetComponent<TransferPosition>().target = TransferPosPlayerRoom;

            if (UnityEngine.Camera.main != null)
            {
                UnityEngine.Camera.main.backgroundColor = CameraBg;
            }
        }

        private void Draw()
        {
            // 生成起点
            bg.SetTile(new Vector3Int(0, 0, 0), Builder.RandomRoad());

            foreach (var v3 in Builder.WallUp)
            {
                solidsBack.SetTile(v3, Builder.RandomWall());
                solidsBackColl.SetTile(v3, Builder.RandomWall());
            }

            foreach (var v3 in Builder.WallDown)
            {
                solidsBack.SetTile(v3, Builder.RandomWall());
                solidsBackColl.SetTile(v3, Builder.RandomWall());
            }

            foreach (var v3 in Builder.WallTop)
            {
                solidsFront.SetTile(v3, Builder.RandomWallTop());
                solidsFrontColl.SetTile(v3, Builder.RandomWallTop());
                solidsFrontColl2.SetTile(v3, Builder.RandomWallTop());
            }

            foreach (var item in Builder.Road.Keys)
            {
                bg.SetTile(item, Builder.RandomRoad());
            }

            foreach (var v3 in PlayerRoomBuilder.WallUp)
            {
                solidsBack.SetTile(v3, PlayerRoomBuilder.RandomWall());
                solidsBackColl.SetTile(v3, PlayerRoomBuilder.RandomWall());
            }

            foreach (var v3 in PlayerRoomBuilder.WallDown)
            {
                solidsBack.SetTile(v3, PlayerRoomBuilder.RandomWall());
                solidsBackColl.SetTile(v3, PlayerRoomBuilder.RandomWall());
            }

            foreach (var v3 in PlayerRoomBuilder.WallTop)
            {
                solidsFront.SetTile(v3, PlayerRoomBuilder.RandomWallTop());
                solidsFrontColl.SetTile(v3, PlayerRoomBuilder.RandomWallTop());
                solidsFrontColl2.SetTile(v3, PlayerRoomBuilder.RandomWallTop());
            }

            foreach (var item in PlayerRoomBuilder.Road.Keys)
            {
                bg.SetTile(item, PlayerRoomBuilder.RandomRoad());
            }
        }

        private void MakeObj()
        {
            foreach (var target in Builder.ObstaclePos)
            {
                Builder.ObstacleObj.Add(Instantiate(Builder.Obstacle[Random.Range(0, Builder.Obstacle.Length)],
                    target * 16, Quaternion.Euler(Vector3.zero),
                    prefabs.transform));
            }

            foreach (var target in Builder.EnemiesPos)
            {

                Builder.EnemiesObj.Add(Instantiate(Builder.Enemies[Random.Range(0, Builder.Enemies.Length)],
                    target * 16,
                    Quaternion.Euler(Vector3.zero), prefabs.transform));
            }

            foreach (var target in Builder.InteractableObjPos)
            {
                Builder.InteractableObj.Add(Instantiate(
                    Builder.Interactable[Random.Range(0, Builder.Interactable.Length)], target * 16,
                    Quaternion.Euler(Vector3.zero), prefabs.transform));
            }
        }

        private void UnDraw()
        {
            bg.ClearAllTiles();
            solidsBack.ClearAllTiles();
            solidsBackColl.ClearAllTiles();
            solidsFront.ClearAllTiles();
            solidsFrontColl.ClearAllTiles();
            solidsFrontColl2.ClearAllTiles();
            debugTileMap.ClearAllTiles();
        }

        private void OnDestroy()
        {
            CameraBg = DefCameraBg;

            if (_transferPosMainRoom != null)
            {
                _transferPosMainRoom.SetActive(false);
                Destroy(_transferPosMainRoom);
            }

            if (_transferPosPlayerRoom != null)
            {
                _transferPosPlayerRoom.SetActive(false);
                Destroy(_transferPosPlayerRoom);
            }


            PlayerRoomBuilder.ClearAllVector3();
            Builder.ClearAllVector3();
            PlayerRoomBuilder.ClearAllObj();
            Builder.ClearAllObj();
            UnDraw();

            After = null;

            bgObj.Clear();

            foreach (var o in bgObj)
            {
                Destroy(o);
            }
        }
    }
}