using System;
using PixelGameAssets.Scripts.SkillSystem.Obj;
using PixelGameAssets.Scripts.UI;
using UnityEngine;

namespace PixelGameAssets.Scripts.Misc
{
    public class ResourceLoader : MonoBehaviour
    {
        // DEBUG

        // 通知资源预制件
        public static GameObject NotificationRes;

        public static GameObject ChapterSelectionUi;

        // 对话框资源 (Normal)
        public static GameObject NormalDialogRes;

        public static GameObject DamageRes;

        public static GameObject DeadUi;

        public static GameObject PlayerRes;

        public static GameObject RandomMapTypeDungeon;

        private static GameObject _randomMapTypeSkyCity;

        #region PopupPrefab

        private static ActionButton _popupPrefab;

        /// <summary>
        /// 浮动按钮
        /// </summary>
        public static ActionButton PopupPrefab
        {
            get
            {
                if (_popupPrefab == null)
                {
                    _popupPrefab = Resources.Load<ActionButton>("Prefabs/FX/ActionFPopup");
                }

                return _popupPrefab;
            }
        }

        #endregion

        public static GameObject RandomMapTypeSkyCity
        {
            get
            {
                if (_randomMapTypeSkyCity == null)
                {
                    _randomMapTypeSkyCity = Resources.Load<GameObject>("Prefabs/Map/RandomMapTypeSkyCiry");
                }

                return _randomMapTypeSkyCity;
            }
        }

        private static GameObject _randomMapTypeC;

        public static GameObject RandomMapTypeC
        {
            get
            {
                if (_randomMapTypeC == null)
                {
                    _randomMapTypeC = Resources.Load<GameObject>("Prefabs/Map/RandomMapTypeC");
                }

                return _randomMapTypeC;
            }
        }

        public static GameObject TransferPosition;

        private static GameObject _cloud;

        public static GameObject Cloud
        {
            get
            {
                if (_cloud == null)
                {
                    _cloud = Resources.Load<GameObject>("Prefabs/Entities/Cloud");
                }

                return _cloud;
            }
        }

        private static GameObject _fog;

        public static GameObject Fog
        {
            get
            {
                if (_fog == null)
                {
                    _fog = Resources.Load<GameObject>("Prefabs/Entities/PW/PW_2D_Fog03");
                }

                return _fog;
            }
        }

        private static GameObject _menu;

        public static GameObject Menu
        {
            get
            {
                if (_menu == null)
                {
                    _menu = Resources.Load<GameObject>("Prefabs/UI/Menu");
                }

                return _menu;
            }
        }

        private static GameObject _crossHair;

        public static GameObject CrossHair
        {
            get
            {
                if (_crossHair == null)
                {
                    _crossHair = Resources.Load<GameObject>("Prefabs/Core/Crosshair");
                }

                return _crossHair;
            }
        }

        private static GameObject _sceneNameShow;

        public static GameObject SceneNameShow
        {
            get
            {
                if (_sceneNameShow == null)
                {
                    _sceneNameShow = Resources.Load<GameObject>("Prefabs/UI/SceneNameShow");
                }

                return _sceneNameShow;
            }
        }

        private static GameObject _hpBottle;
        private static GameObject _coin;
        private static GameObject _dialog;

        #region Skill Entity

        private static SkillEntity _fireBall;
        private static SkillEntity _lighting;
        
        /// <summary>
        /// 闪电
        /// </summary>
        public static SkillEntity Lighting
        {
            get
            {
                if (_lighting == null)
                {
                    _lighting = Resources.Load<SkillEntity>("Prefabs/Skill/Lightning_Bolt/Lightning_Bolt");
                }

                return _lighting;
            }
        }

        /// <summary>
        /// 火球
        /// </summary>
        public static SkillEntity FireBall
        {
            get
            {
                if (_fireBall == null)
                {
                    _fireBall = Resources.Load<SkillEntity>("Prefabs/Skill/Fire_Ball/FireBall");
                }

                return _fireBall;
            }
        }

        #endregion
        
        /// <summary>
        /// 血瓶
        /// </summary>
        public static GameObject HpBottle
        {
            get
            {
                if (_hpBottle == null)
                {
                    _hpBottle = Resources.Load<GameObject>("Prefabs/Entities/HealthBottle");
                }

                return _hpBottle;
            }
        }

        /// <summary>
        /// 金币
        /// </summary>
        public static GameObject Coin
        {
            get
            {
                if (_coin == null)
                {
                    _coin = Resources.Load<GameObject>("Prefabs/Entities/Coin");
                }

                return _coin;
            }
        }

        /// <summary>
        /// 对话框
        /// </summary>
        public static GameObject Dialog
        {
            get
            {
                if (_dialog == null)
                {
                    _dialog = Resources.Load<GameObject>("Prefabs/UI/Dialog");
                }

                return _dialog;
            }
        }

        public static GameObject MessageText;

        [Obsolete("Use Get & Set")]
        public static void Init()
        {
            if (NotificationRes == null)
            {
                NotificationRes = Resources.Load<GameObject>("Notification");
            }

            if (ChapterSelectionUi == null)
            {
                ChapterSelectionUi = Resources.Load<GameObject>("ChapterSelectionUI");
            }

            if (NormalDialogRes == null)
            {
                NormalDialogRes = Resources.Load<GameObject>("NpcDialog");
            }

            if (DamageRes == null)
            {
                DamageRes = Resources.Load<GameObject>("DamageNumber");
            }

            if (DeadUi == null)
            {
                DeadUi = Resources.Load<GameObject>("DeadUI");
            }

            if (PlayerRes == null)
            {
//                PlayerRes = Resources.Load<GameObject>("Prefabs/Actors/PlayerSister");
                PlayerRes = Resources.Load<GameObject>("Prefabs/Actors/PlayerDef");
//                PlayerRes = Resources.Load<GameObject>("Prefabs/Actors/PlayerSword/PlayerSword");
//                PlayerRes = Resources.Load<GameObject>("Prefabs/Actors/PlayerDef");
            }

            if (RandomMapTypeDungeon == null)
            {
                RandomMapTypeDungeon = Resources.Load<GameObject>("Prefabs/Map/RandomMapTypeDungeon_Dark");
            }

            if (TransferPosition == null)
            {
                TransferPosition = Resources.Load<GameObject>("Prefabs/Entities/TransferPosition");
            }

            if (MessageText == null)
            {
                MessageText = Resources.Load<GameObject>("Prefabs/UI/MessageShow");
            }
        }
    }
}