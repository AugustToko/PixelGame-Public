using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using PixelGameAssets.Scripts.Core;
using PixelGameAssets.Scripts.Core.Audio;
using PixelGameAssets.Scripts.EventManager;
using PixelGameAssets.Scripts.InputSystem;
using PixelGameAssets.Scripts.Misc;
using PixelGameAssets.Scripts.SkillSystem;
using PixelGameAssets.Scripts.UI.Menu;
using PixelGameAssets.Scripts.UI.MusicSystem;
using PixelGameAssets.Scripts.UI.SkillBar;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace PixelGameAssets.Scripts.UI.UICanvas
{
    /// <summary>
    /// 常驻 UI (战斗 UI)
    /// TODO: 用于对当前场景的 UI 进行统一管理
    /// </summary>
    public class UiManager : MonoBehaviour
    {
        public static UiManager Instance;

        public enum EventType
        {
            UpdateCoin,
        }

        /// <summary>
        /// 移动端按钮组合
        /// </summary>
        [SerializeField] private Canvas mobileButtons;

        [SerializeField] private Button pauseButton;

        private CanvasGroup _pauseButtonCanvasGroup;

        /// <summary>
        /// 技能栏
        /// </summary>
        public SkillBarUi skillBarUi;

        public SystemEventArea systemEventArea;

        /// <summary>
        /// 是否有可交互 UI (非此 ui ) 浮动在屏幕(关卡场景)上
        /// </summary>
        private bool HasUiOverlay { get; set; }

        public TextMeshProUGUI ammoInfo;

        [SerializeField] private Text musicName;

        public Canvas skillBar;

        [SerializeField] private Text targetInfo;

        /// <summary>
        /// 控制攻击的虚拟手柄
        /// </summary>
        public VariableJoystick joystickAtk;

        /// <summary>
        /// 控制移动的虚拟手柄
        /// </summary>
        public VariableJoystick joystickMove;

        public Image weaponIcon;

        public Image secondWeaponIcon;

        public Button eventButton;

        public Slider levelBar;

        /// <summary>
        /// 等级
        /// </summary>
        public TextMeshProUGUI levelText;

        public Text sceneText;

        [SerializeField] private Text coinText;

        #region Hits

        public Text hitInfo;

        private CanvasGroup _hitCanvasGroup;

        private Animator _hitsAnimator;

        private int _hits;

        private const float HitInfoHideTime = 3f;

        private float _hitInfoHideTimer;

        [SerializeField] private Text hpText;

        #endregion

        /// <summary>
        /// 监听 UI 按钮是否按下 (针对于移动平台)
        /// </summary>

        #region Event

        public bool EventDown { get; set; }

        public bool RollButtonDone { get; set; }

        public bool SpaceButtonDone { get; set; }

        public bool SpeedUpButton { get; set; }

        #endregion

        private void Awake()
        {
            Instance = this;
            EventCenter.AddListener<int, int>(EventManager.EventType.UpdateAmmo, SetAmmoInfo);
            EventCenter.AddListener<Song>(EventManager.EventType.SetUpMusicInfo, UpdateMusicName);
            EventCenter.AddListener<int>(EventManager.EventType.UpdateHits, UpdateHits);
            EventCenter.AddListener<int>(EventManager.EventType.UpdateCoin, UpdateCoin);
            EventCenter.AddListener<ExpManager.LevelDataPack>(EventManager.EventType.UpdateLevel, UpdateLevel);

            _hitCanvasGroup = hitInfo.GetComponent<CanvasGroup>();
            _hitsAnimator = hitInfo.GetComponent<Animator>();

            var scale = GetComponent<CanvasScaler>();

            var sws = Screen.width / scale.referenceResolution.x;
            var shs = Screen.height / scale.referenceResolution.y;

            scale.matchWidthOrHeight = sws > shs ? 1 : 0;

            _pauseButtonCanvasGroup = pauseButton.GetComponent<CanvasGroup>();

            QMhelper.SetupCoinText(coinText);
            QMhelper.SetupHpText(hpText);
        }

        private void Start()
        {
            // 主动获取当前歌曲名称
            if (AudioManager.Instance != null && AudioManager.Instance.CurrentBgmClip != null)
                UpdateMusicName(AudioManager.Instance.CurrentBgmClip.name);
            else
                UpdateMusicName("None");

            if (!GameManager.Instance.isMobile)
            {
                joystickAtk.gameObject.SetActive(false);
                joystickMove.gameObject.SetActive(false);
                mobileButtons.gameObject.SetActive(false);
            }
            else
            {
                joystickAtk.gameObject.SetActive(true);
                joystickMove.gameObject.SetActive(true);
                mobileButtons.gameObject.SetActive(true);
            }
        }

        private void Update()
        {
            if (_hitInfoHideTimer > 0f)
            {
                _hitInfoHideTimer -= Time.deltaTime;
            }
            else
            {
                _hitCanvasGroup.alpha = 0;
                _hits = 0;
            }

            if (InputManager.Instance.GetButtonDown(0, InputAction.Cancel))
            {
                if (!shownMenu)
                {
                    ShowMenu();
                }
                else if (PauseMenu.Instance != null)
                {
                    HideMenu();
                }
            }
        }

        public void Shoot()
        {
        }

        /// <summary>
        /// 更新音乐信息
        /// </summary>
        /// <param name="song">Music Name</param>
        private void UpdateMusicName(Song song)
        {
            musicName.text = song.musicName;
        }

        /// <summary>
        /// 更新音乐信息
        /// </summary>
        /// <param name="songName">Music Name</param>
        private void UpdateMusicName(string songName)
        {
            musicName.text = songName;
        }

        /// <summary>
        /// 更新连击数
        /// </summary>
        /// <param name="hits"></param>
        private void UpdateHits(int hits)
        {
            _hitsAnimator.Play("UpdateHits", 0, 0f);
            _hitCanvasGroup.alpha = 1;
            _hits++;
            hitInfo.text = _hits + " Hits!!!";
            _hitInfoHideTimer = HitInfoHideTime;
        }

        /// <summary>
        /// 升级
        /// </summary>
        /// <param name="dataPack"></param>
        private void UpdateLevel([NotNull] ExpManager.LevelDataPack dataPack)
        {
            levelText.text = "Level: " + dataPack.Level;
            levelBar.value = dataPack.LevelBarPercentage;
        }

        /// <summary>
        /// 更新硬币
        /// </summary>
        /// <param name="amount">val</param>
        private void UpdateCoin(int amount)
        {
            coinText.text = amount.ToString();
        }

        /// <summary>
        /// 移除屏幕上的交互 UI (非固定UI (非 "UI-Canvas"))
        /// </summary>
        /// <param name="go">UI gameObj</param>
        public void DestroyUi(GameObject go)
        {
            Destroy(go);
            HasUiOverlay = false;
            GameManager.Instance.Player.ResumeControl();
        }

        /// <summary>
        /// 展示 UI (TOP)
        /// </summary>
        /// <param name="go">Obj</param>
        public void ShowedUi(ref GameObject go)
        {
            HasUiOverlay = true;
            GameManager.Instance.Player.StopControl();
        }

        public void SetPauseButtonVisible(bool visible)
        {
            _pauseButtonCanvasGroup.alpha = visible ? 1 : 0;
            pauseButton.enabled = visible;
        }

        /// <summary>
        /// TODO: 设置技能栏
        /// </summary>
        /// <param name="skillPack">skill 包</param>
        public void SetUpSkillBar(SkillPack skillPack)
        {
        }

        private void SetAmmoInfo(int clipCapacity, int remainingBullet)
        {
            ammoInfo.text = clipCapacity + " / " + remainingBullet;
        }

        /// <summary>
        /// 设置当前目标信息
        /// </summary>
        /// <param name="info">data</param>
        public void SetTargetInfo([NotNull] string info)
        {
            targetInfo.text = info;
        }

        [HideInInspector] public bool shownMenu { get; set; } = false;

        /// <summary>
        /// 隐藏或显示 menu
        /// </summary>
        public void ShowMenu()
        {
//            var cam = UnityEngine.Camera.main;
//            if (cam == null) return;

            shownMenu = true;
//            var targetPos = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f));
            var menuPanel = Instantiate(ResourceLoader.Menu,
                new Vector3(0f, 0f, 0f), Quaternion.Euler(Vector3.zero), transform);
            var pauseMenu = menuPanel.GetComponentInChildren<PauseMenu>();
            pauseMenu.Show();

            SetPauseButtonVisible(false);
        }

        public void HideMenu()
        {
            PauseMenu.Instance.HideMenu();
        }

        private RectTransform _rect;

        /// <summary>
        /// 设置武器图片
        /// </summary>
        /// <param name="sprite"></param>
        public void SetWeaponMainIcon(Sprite sprite)
        {
            weaponIcon.sprite = sprite;
            weaponIcon.SetNativeSize();
            _rect = weaponIcon.GetComponent<RectTransform>();
            _rect.anchorMax = new Vector2(1, 1);
            _rect.anchorMin = new Vector2(0, 0);
            _rect.pivot = new Vector2(0.5f, 0.5f);
        }

        /// <summary>
        /// 设置武器图片
        /// </summary>
        /// <param name="sprite">精灵</param>
        public void SetWeaponSecondIcon(Sprite sprite)
        {
            if (sprite == null) return;

            secondWeaponIcon.sprite = sprite;
            secondWeaponIcon.SetNativeSize();
            _rect = secondWeaponIcon.GetComponent<RectTransform>();
            _rect.anchorMax = new Vector2(1, 1);
            _rect.anchorMin = new Vector2(0, 0);
            _rect.pivot = new Vector2(0.5f, 0.5f);
        }

        /// <summary>
        /// 检测是否点到了常驻 UI <see cref="UiManager"/>
        /// </summary>
        /// <returns>bool</returns>
        public static bool CheckGuiRaycastObjects()
        {
            var eventDataCurrentPosition = new PointerEventData(EventSystem.current)
            {
                position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
            };

            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

            return results.Any(raycastResult =>
            {
                var go = raycastResult.gameObject;
                return go.CompareTag("UI block") || go.CompareTag("Equipment") || go.CompareTag("Inventory") ||
                       go.CompareTag("SkillBar") || go.CompareTag("Vendor");
            });
        }

        /// <summary>
        /// 在屏幕上显示场景名称
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <param name="subText">Sub</param>
        public void ShowSceneNameOnUi(string sceneName, string subText = "")
        {
            var go = Instantiate(ResourceLoader.SceneNameShow, gameObject.transform);
            go.GetComponentInChildren<TextMeshProUGUI>().text = sceneName;

            sceneText.text = sceneName;
        }

        public void ShowMessageOnUi(string message)
        {
            var go = Instantiate(ResourceLoader.MessageText, gameObject.transform);
            go.GetComponentInChildren<TextMeshProUGUI>().text = message;
        }

        private void OnDestroy()
        {
            EventCenter.RemoveListener<int, int>(EventManager.EventType.UpdateAmmo, SetAmmoInfo);
            EventCenter.RemoveListener<Song>(EventManager.EventType.SetUpMusicInfo, UpdateMusicName);
            EventCenter.RemoveListener<int>(EventManager.EventType.UpdateHits, UpdateHits);
            EventCenter.RemoveListener<int>(EventManager.EventType.UpdateCoin, UpdateCoin);
            EventCenter.RemoveListener<ExpManager.LevelDataPack>(EventManager.EventType.UpdateLevel, UpdateLevel);
        }
    }
}