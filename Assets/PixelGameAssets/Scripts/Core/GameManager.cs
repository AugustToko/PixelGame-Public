using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using PixelGameAssets.Scripts.Actor;
using PixelGameAssets.Scripts.Camera;
using PixelGameAssets.Scripts.EventManager;
using PixelGameAssets.Scripts.Misc;
using PixelGameAssets.Scripts.SceneUtils;
using PixelGameAssets.Scripts.SkillSystem;
using PixelGameAssets.Scripts.SkillSystem.Detail;
using PixelGameAssets.Scripts.UI;
using PixelGameAssets.Scripts.UI.NotificationSystem;
using PixelGameAssets.Scripts.UI.UICanvas;
using PixelGameAssets.Scripts.WeaponSystem.WeaponInfo;
using PixelGameAssets.Scripts.WeaponSystem.WeaponType;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using EventType = PixelGameAssets.Scripts.EventManager.EventType;
using Object = System.Object;

namespace PixelGameAssets.Scripts.Core
{
    /// <summary>
    /// 游戏管理类
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [Obsolete("Use SceneUtils.Scene")] public const string TownSceneName = "TownScene";

        // TODO 架构重置
        public static string RandomSceneType;

        // 敌人总数
        // 用于监听全部敌人死亡, 然后开门
        public static int EnemyCount;

        public static GameManager Instance;

        /// <summary>
        /// 层遮罩
        /// 固态层 (墙壁)
        /// </summary>
        [Header("Layers")] public LayerMask solidLayer;

        [Space(10)] [HideInInspector] public PixelCameraFollower PixelCameraFollower;

        [HideInInspector] public MinimapPixelCameraFollower miniPixelCameraFollower;

        public static bool IsStop = false;

        private ActionButton _currentlySpawnedPopup;

        public ActionButton popupPrefab;

        /// <summary>
        /// 玩家信息, 单一引用 ref
        /// </summary>
        public PlayerInfo PlayerInfo { get; set; }

        /// <summary>
        /// 当前玩家, 单一引用 ref
        /// </summary>
        public BasePlayer Player { get; set; }

        public const string PlayerTag = "Player";

        [HideInInspector] public bool isMobile;

        private void Awake()
        {
            // TODO: 添加 FPS 选项
#if !UNITY_EDITOR
            Application.targetFrameRate = 60;
#endif

            isMobile = Application.isMobilePlatform;

            if (Instance == null) Instance = this;
            else if (Instance != this) Destroy(gameObject);

            // TODO: 添加分辨率选项
//            Screen.SetResolution(1280, 720, true);

            EventCenter.AddListener(EventType.NotifyHide, NotifyHide);
        }

        private void Start()
        {
            DontDestroyOnLoad(this);

            LoadPlayerInfo();

            SceneManager.sceneLoaded += OnSceneLoaded;

            // 加载场景时隐藏 CommonPlayer
            EventCenter.AddListener(EventType.BeginLoadScene, () =>
            {
                if (Player != null) Player.HidePlayer();
            });
        }

        private void Update()
        {
            PlayerInfo.spendTime += Time.deltaTime;
        }

        private void NotifyHide()
        {
            Player.HidePlayer();
        }

        /// <summary>
        /// 场景加载完成回调
        /// </summary>
        /// <param name="scene">加载完成的场景</param>
        /// <param name="arg1">加载模式</param>
        private void OnSceneLoaded(Scene scene, LoadSceneMode arg1)
        {
            if (SceneUtil.IsGameScene(scene.name))
            {
                UiManager.Instance.ShowSceneNameOnUi(scene.name);

                QMhelper.SetActive(true);
                
                Player.ShowPlayer(Vector3.zero);

                if (PixelCameraFollower != null)
                {
                    PixelCameraFollower.m_Target = Player.transform;
                    return;
                }

                if (miniPixelCameraFollower != null)
                {
                    miniPixelCameraFollower.m_Target = Player.transform;
                    return;
                }
            }
            else
            {
                Player.HidePlayer();
                QMhelper.SetActive(false);
            }
        }

        /// <summary>
        /// 加载玩家信息
        /// TODO：加载存档选项
        /// </summary>
        public void LoadPlayerInfo()
        {
            QMhelper.Load();

            var fileInfo = new FileInfo("Data.bin");
//            if (fileInfo.Exists)
            if (false)
            {
                Debug.Log("---------READ FROM FILE----------");
                Debug.Log("Path: " + fileInfo.Directory);

                var formatter = new BinaryFormatter();
                Stream stream = new FileStream("Data.bin", FileMode.Open, FileAccess.Read, FileShare.Read);
                PlayerInfo = (PlayerInfo) formatter.Deserialize(stream);
#if UNITY_EDITOR
                Debug.Log(PlayerInfo.ToString());
#endif
            }
            else
            {
                Debug.Log("---------NEW DATA----------");

                // New skills (Common skill)
                var skillDodge = new CommonSkillDetail.Builder(SkillType.DodgeNone, 0, "DodgeNone").SetColdDownTime(0)
                    .Build();
                var skillWpNone = new CommonSkillDetail.Builder(SkillType.WpNone, 0, "WpNone").SetColdDownTime(0)
                    .Build();
                var skillDis = new CommonSkillDetail.Builder(SkillType.Displacement, 0, "Displacement")
                    .SetColdDownTime(0.5f).Build();

                // Data and skill order
                var skillList = new List<SkillDetail>
                {
                    new CommonSkillDetail.Builder(SkillType.FireBall, 0, "FireBall").SetColdDownTime(0.5f).Build(),
                    new CommonSkillDetail.Builder(SkillType.LightningBolt, 0, "LightningBolt").SetColdDownTime(2f)
                        .Build(),
                    skillDodge,
                    skillWpNone,
                    skillDis
                };

                // Skill-pack (All skills)
                var skillPack = new SkillPack(
                    skillDodge, skillWpNone, skillDis, skillList
                );

                // TODO: 玩家昵称设置
                PlayerInfo = new PlayerInfo("Player", new List<WeaponData>(), skillPack);
            }

            var playerGo = Instantiate(ResourceLoader.PlayerRes, new Vector3(16.5f, -71),
                Quaternion.Euler(Vector3.zero));
            Player = playerGo.GetComponent<BasePlayer>();
            Player.Health.health = (int) QMhelper.GetMaxHpVal();

            QMhelper.SetupPlayer(Player.gameObject);

            ExpManager.Instance.Level = PlayerInfo.level;
            ExpManager.Instance.Exp = PlayerInfo.exp;

            PlayerInfo.OperateCoin(0, true);

            var player = Player;

            // Init if CommonPlayer
            if (Player is CommonPlayer commonPlayer)
            {
                commonPlayer.SkillManager.Init(ref player, ref PlayerInfo.skillPack);

                foreach (var weaponRes in PlayerInfo.weapons.Select(data =>
                    Resources.LoadAsync<Weapon>("Prefabs/Weapons/" + data.weaponName + "/" + data.weaponName)))
                {
                    weaponRes.completed += operation =>
                    {
                        commonPlayer.EquipWeapon((Weapon) weaponRes.asset, true);
                        Debug.Log("LoadDone");
                    };
                }
            }

            PixelCameraFollower.m_Target = Player.transform;

            // 设置小地图
            if (miniPixelCameraFollower != null) miniPixelCameraFollower.m_Target = Player.transform;

            DontDestroyOnLoad(playerGo);
        }

        /// <summary>
        /// 添加 <see cref="SkillPack"/> 的支持
        /// </summary>
        public void SavePlayerInfo()
        {
            Debug.Log("---------SAVE TO FILE----------");

            PlayerInfo.weapons.Clear();
            foreach (var weapon in Player.WeaponBag)
            {
                PlayerInfo.weapons.Add(weapon.weaponData);
            }

            PlayerInfo.exp = ExpManager.Instance.Exp;
            PlayerInfo.level = ExpManager.Instance.Level;
            PlayerInfo.score = ScoreManager.Instance.Score;

            var formatter = new BinaryFormatter();
            Stream stream = new FileStream("Data.bin", FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, PlayerInfo);
            stream.Close();

            QMhelper.Save();

            new Notification.Builder("Game Data", "Game data has been saved.").Show();
        }

        /// <summary>
        /// 生成一个浮动菜单
        /// </summary>
        /// <param name="position">生成位置</param>
        public void SpawnPopup(Vector2 position)
        {
            DeSpawnPopup();
            _currentlySpawnedPopup = Instantiate(popupPrefab, position, Quaternion.identity);
        }

        /// <summary>
        /// 销毁浮动菜单
        /// </summary>
        public void DeSpawnPopup()
        {
            if (_currentlySpawnedPopup == null) return;
            _currentlySpawnedPopup.DestroySelf();
            _currentlySpawnedPopup = null;
        }

        /// <summary>
        /// 淡入淡出浮动菜单
        /// </summary>
        public void FadePopup()
        {
            if (_currentlySpawnedPopup == null) return;
            _currentlySpawnedPopup.FadeMe();
            _currentlySpawnedPopup = null;
        }

        /// <summary>
        /// TODO
        /// </summary>
        public static void ExitGame()
        {
            Application.Quit();
        }

        private void OnDestroy()
        {
            EventCenter.RemoveListener(EventType.NotifyHide, NotifyHide);
        }
    }
}