using UnityEngine;
using UnityEngine.Tilemaps;

namespace PixelGameAssets.Scripts.Map
{
    public class RandomMapHelper : MonoBehaviour
    {
        
        #region Common

        /// <summary>
        /// 背景颜色
        /// </summary>
        public Color bgColor;

        /// <summary>
        /// 常规墙壁
        /// </summary>
        [Header("Common Tile Types")] public TileBase wallTile;

        /// <summary>
        /// 常规墙顶
        /// </summary>
        public TileBase wallTopTile;

        /// <summary>
        /// 常规路
        /// </summary>
        public TileBase roadTile;

        public TileBase debug;

        #endregion

        #region Random

        /// <summary>
        /// 随机障碍物
        /// </summary>
        [Header("Random Obstacle")] public GameObject[] obstacle;
        
        /// <summary>
        /// 可交互物品
        /// </summary>
        [Header("Random Obstacle")] public GameObject[] interactableObj;

        /// <summary>
        /// 随机敌人
        /// </summary>
        [Header("Random Enemies")] public GameObject[] enemies;

        /// <summary>
        /// 随机墙顶
        /// </summary>
        [Header("Random Tile Types")] public TileBase[] wallTopRandom;

        /// <summary>
        /// 随机墙壁
        /// </summary>
        public TileBase[] wallTileRandom;

        /// <summary>
        /// 随机道路
        /// </summary>
        public TileBase[] roadFloorTileRandom;

        #endregion

    }
}